using System;
using System.Collections.Generic;
using System.Text;
namespace IsengardClient
{
    public interface IOutputItemSequence
    {
        OutputItemInfo FeedByte(int nextByte);
    }

    public class OutputItemInfo
    {
        public OutputItemSequenceType SequenceType { get; set; }
        public int? HP { get; set; }
        public int? MP { get; set; }
    }

    public enum OutputItemSequenceType
    {
        UserNamePrompt,
        PasswordPrompt,
        HPMPStatus,
        ContinueToNextScreen,
        Goodbye,
    }

    internal interface IOutputProcessingSequence
    {
        void FeedLine(FeedLineParameters Parameters);
    }

    public class FeedLineParameters
    {
        public FeedLineParameters(List<string> Lines)
        {
            this.Lines = Lines;
        }
        public List<string> Lines { get; set; }
        public BackgroundCommandType? BackgroundCommandType { get; set; }
        public string CurrentlyFightingMob { get; set; }
        public bool FinishedProcessing { get; set; }
        public bool SuppressEcho { get; set; }
        public CommandResult? CommandResult { get; set; }
        public HashSet<string> PlayerNames { get; set; }
    }

    internal class ConstantOutputItemSequence : IOutputItemSequence
    {
        private ConstantSequence _cs;
        private OutputItemSequenceType _sequenceType;
        public ConstantOutputItemSequence(OutputItemSequenceType sequenceType, string pattern, Dictionary<char, int> asciiMapping)
        {
            _cs = new ConstantSequence(pattern, null, asciiMapping);
            _sequenceType = sequenceType;
        }

        public OutputItemInfo FeedByte(int nextByte)
        {
            OutputItemInfo ret = null;
            if (_cs.FeedByte(nextByte))
            {
                ret = new OutputItemInfo();
                ret.SequenceType = _sequenceType;
            }
            return ret;
        }
    }

    public class ConstantOutputSequence : IOutputProcessingSequence
    {
        private Action<FeedLineParameters> _onSatisfied;
        private string _characters;
        private ConstantSequenceMatchType _matchType;
        private int? _exactLine;
        private List<BackgroundCommandType> _backgroundCommandTypes;

        public ConstantOutputSequence(string characters, Action<FeedLineParameters> onSatisfied, ConstantSequenceMatchType MatchType, int? exactLine)
        {
            _onSatisfied = onSatisfied;
            _characters = characters;
            _matchType = MatchType;
            _exactLine = exactLine;
        }

        public ConstantOutputSequence(string characters, Action<FeedLineParameters> onSatisfied, ConstantSequenceMatchType MatchType, int? exactLine, BackgroundCommandType? backgroundCommandType) :
            this(characters, onSatisfied, MatchType, exactLine)
        {
            if (backgroundCommandType.HasValue)
            {
                _backgroundCommandTypes = new List<BackgroundCommandType>() { backgroundCommandType.Value };
            }
        }

        public ConstantOutputSequence(string characters, Action<FeedLineParameters> onSatisfied, ConstantSequenceMatchType MatchType, int? exactLine, List<BackgroundCommandType> backgroundCommandTypes) :
            this(characters, onSatisfied, MatchType, exactLine)
        {
            _backgroundCommandTypes = backgroundCommandTypes;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (_backgroundCommandTypes != null && (!backgroundCommandType.HasValue || !_backgroundCommandTypes.Contains(backgroundCommandType.Value)))
            {
                return;
            }
            bool match = false;
            if (_exactLine.HasValue)
            {
                int exactLineVal = _exactLine.Value;
                if (Lines.Count < exactLineVal)
                {
                    return;
                }
                match = CheckLine(Lines[exactLineVal]);
            }
            else
            {
                foreach (string Line in Lines)
                {
                    if (CheckLine(Line))
                    {
                        match = true;
                        break;
                    }
                }
            }
            if (match)
            {
                _onSatisfied(flParams);
            }
        }

        private bool CheckLine(string Line)
        {
            bool ret;
            switch (_matchType)
            {
                case ConstantSequenceMatchType.ExactMatch:
                    ret = Line.Equals(_characters);
                    break;
                case ConstantSequenceMatchType.StartsWith:
                    ret = Line.StartsWith(_characters);
                    break;
                case ConstantSequenceMatchType.Contains:
                    ret = Line.Contains(_characters);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }
    }

    public enum ConstantSequenceMatchType
    {
        ExactMatch,
        StartsWith,
        Contains,
    }

    internal class ConstantSequence
    {
        private int _currentMatchPoint = -1;
        private List<int> _chars;
        private Action _onSatisfied;
        public ConstantSequence(string characters, Action onSatisfied, Dictionary<char, int> asciiMapping)
        {
            _onSatisfied = onSatisfied;
            _chars = GenerateBytesForPattern(characters, asciiMapping);
        }

        public static List<int> GenerateBytesForPattern(string pattern, Dictionary<char, int> asciiMapping)
        {
            List<int> ret = new List<int>(pattern.Length);
            foreach (char c in pattern)
            {
                ret.Add(asciiMapping[c]);
            }
            return ret;
        }

        public bool FeedByte(int nextByte)
        {
            if (_chars[_currentMatchPoint + 1] == nextByte)
            {
                _currentMatchPoint++;
                if (_currentMatchPoint == _chars.Count - 1) //reached end of sequence
                {
                    if (_onSatisfied != null)
                    {
                        _onSatisfied();
                    }
                    _currentMatchPoint = -1;
                    return true;
                }
            }
            else
            {
                _currentMatchPoint = -1;
            }
            return false;
        }
    }

    internal class HPMPSequence : IOutputItemSequence
    {
        private List<int> HPNumbers = new List<int>();
        private List<int> MPNumbers = new List<int>();
        private HPMPStep CurrentStep = HPMPStep.None;

        private enum HPMPStep
        {
            None,
            LeftParen,
            AfterHPNumberSpace,
            H,
            BeforeMPNumberSpace,
            AfterMPNumberSpace,
            M,
            RightParen,
            Colon,
        }

        public OutputItemInfo FeedByte(int nextByte)
        {
            OutputItemInfo ret = null;
            switch (CurrentStep)
            {
                case HPMPStep.None:
                    if (nextByte == AsciiMapping.ASCII_LEFT_PAREN)
                        CurrentStep = HPMPStep.LeftParen;
                    break;
                case HPMPStep.LeftParen:
                    if (nextByte == AsciiMapping.ASCII_SPACE)
                    {
                        if (HPNumbers.Count == 0)
                            CurrentStep = HPMPStep.None;
                        else
                            CurrentStep = HPMPStep.AfterHPNumberSpace;
                    }
                    else if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                    {
                        HPNumbers.Add(nextByte - AsciiMapping.ASCII_NUMBER_ZERO);
                    }
                    else
                    {
                        CurrentStep = HPMPStep.None;
                    }
                    break;
                case HPMPStep.AfterHPNumberSpace:
                    if (nextByte == AsciiMapping.ASCII_UPPERCASE_H)
                        CurrentStep = HPMPStep.H;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.H:
                    if (nextByte == AsciiMapping.ASCII_SPACE)
                        CurrentStep = HPMPStep.BeforeMPNumberSpace;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.BeforeMPNumberSpace:
                    if (nextByte == AsciiMapping.ASCII_SPACE)
                    {
                        if (MPNumbers.Count == 0)
                            CurrentStep = HPMPStep.None;
                        else
                            CurrentStep = HPMPStep.AfterMPNumberSpace;
                    }
                    else if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                    {
                        MPNumbers.Add(nextByte - AsciiMapping.ASCII_NUMBER_ZERO);
                    }
                    else
                    {
                        CurrentStep = HPMPStep.None;
                    }
                    break;
                case HPMPStep.AfterMPNumberSpace:
                    if (nextByte == AsciiMapping.ASCII_UPPERCASE_M)
                        CurrentStep = HPMPStep.M;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.M:
                    if (nextByte == AsciiMapping.ASCII_RIGHT_PAREN)
                        CurrentStep = HPMPStep.RightParen;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.RightParen:
                    if (nextByte == AsciiMapping.ASCII_COLON)
                        CurrentStep = HPMPStep.Colon;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.Colon:
                    CurrentStep = HPMPStep.None;
                    if (nextByte == AsciiMapping.ASCII_SPACE) //finished
                    {
                        int hp = 0;
                        for (int i = 0; i < HPNumbers.Count; i++)
                        {
                            hp = (hp * 10) + HPNumbers[i];
                        }
                        int mp = 0;
                        for (int i = 0; i < MPNumbers.Count; i++)
                        {
                            mp = (mp * 10) + MPNumbers[i];
                        }
                        ret = new OutputItemInfo();
                        ret.SequenceType = OutputItemSequenceType.HPMPStatus;
                        ret.HP = hp;
                        ret.MP = mp;
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (CurrentStep == HPMPStep.None)
            {
                HPNumbers.Clear();
                MPNumbers.Clear();
            }
            return ret;
        }
    }

    public class TimeOutputSequence : IOutputProcessingSequence
    {
        private Action<FeedLineParameters, bool> _onSatisfied;
        private const string PREFIX = "Game-Time: ";

        public TimeOutputSequence(Action<FeedLineParameters, bool> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0)
            {
                string sFirstLine = Lines[0];
                if (!sFirstLine.StartsWith(PREFIX))
                {
                    return;
                }
                DayHalf dayHalf;
                if (sFirstLine.EndsWith(" o'clock PM."))
                {
                    dayHalf = DayHalf.PM;
                }
                else if (sFirstLine.EndsWith(" o'clock AM."))
                {
                    dayHalf = DayHalf.AM;
                }
                else
                {
                    return;
                }
                int iNumber;
                int iIndex = PREFIX.Length;
                char cFirstNumber = sFirstLine[iIndex++];
                char cSecondNumber = sFirstLine[iIndex];
                switch (cFirstNumber)
                {
                    case '1':
                        if (cSecondNumber == ' ')
                            iNumber = 1;
                        else if (cSecondNumber == '0')
                            iNumber = 10;
                        else if (cSecondNumber == '1')
                            iNumber = 11;
                        else if (cSecondNumber == '2')
                            iNumber = 12;
                        else
                            return;
                        break;
                    case '2':
                        iNumber = 2;
                        break;
                    case '3':
                        iNumber = 3;
                        break;
                    case '4':
                        iNumber = 4;
                        break;
                    case '5':
                        iNumber = 5;
                        break;
                    case '6':
                        iNumber = 6;
                        break;
                    case '7':
                        iNumber = 7;
                        break;
                    case '8':
                        iNumber = 8;
                        break;
                    case '9':
                        iNumber = 9;
                        break;
                    default:
                        return;
                }
                bool isNight;
                if (dayHalf == DayHalf.AM)
                {
                    isNight = iNumber == 12 || iNumber <= 5;
                }
                else //PM
                {
                    isNight = iNumber != 12 && iNumber >= 8;
                }
                _onSatisfied(flParams, isNight);
                flParams.FinishedProcessing = true;
            }
        }

        private enum DayHalf
        {
            AM,
            PM,
        }
    }

    public class ScoreOutputSequence : IOutputProcessingSequence
    {
        public Action<FeedLineParameters, List<SkillCooldown>, List<string>> _onSatisfied;
        private const string SKILLS_PREFIX = "Skills: ";
        private const string SPELLS_PREFIX = "Spells cast: ";
        private const string POWER_ATTACK_PREFIX = "(power) attack ";
        private const string MANASHIELD_PREFIX = "manashield ";

        private string _username;
        public ScoreOutputSequence(string username, Action<FeedLineParameters, List<SkillCooldown>, List<string>> onSatisfied)
        {
            _username = username;
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0)
            {
                string sNextLine = Lines[0];
                if (!sNextLine.StartsWith(_username + " the ", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                int iNextIndex;
                List<string> skillsRaw = StringProcessing.GetList(Lines, 1, SKILLS_PREFIX, false, out iNextIndex);
                if (skillsRaw == null)
                {
                    return;
                }

                List<string> spellsRaw = StringProcessing.GetList(Lines, iNextIndex, SPELLS_PREFIX, false, out iNextIndex);
                if (spellsRaw == null)
                {
                    return;
                }

                List<SkillCooldown> cooldowns = new List<SkillCooldown>();

                foreach (string sNextSkill in skillsRaw)
                {
                    string sTempSkill = sNextSkill;
                    SkillWithCooldownType? eType = null;
                    int iIndex = sTempSkill.IndexOf(POWER_ATTACK_PREFIX);
                    if (iIndex >= 0)
                    {
                        eType = SkillWithCooldownType.PowerAttack;
                        if (sTempSkill.Length == POWER_ATTACK_PREFIX.Length)
                        {
                            return;
                        }
                        sTempSkill = sTempSkill.Substring(POWER_ATTACK_PREFIX.Length);
                    }
                    if (!eType.HasValue)
                    {
                        iIndex = sTempSkill.IndexOf(MANASHIELD_PREFIX);
                        if (iIndex >= 0)
                        {
                            eType = SkillWithCooldownType.Manashield;
                            if (sTempSkill.Length == MANASHIELD_PREFIX.Length)
                            {
                                return;
                            }
                            sTempSkill = sTempSkill.Substring(MANASHIELD_PREFIX.Length);
                        }
                    }
                    if (!eType.HasValue || sTempSkill.Length <= 2 || !sTempSkill.StartsWith("[") || !sTempSkill.EndsWith("]"))
                    {
                        return;
                    }
                    sTempSkill = sTempSkill.Substring(1, sTempSkill.Length - 2); //remove the brackets
                    SkillWithCooldownType eTypeValue = eType.Value;
                    SkillCooldown cooldown = new SkillCooldown();
                    cooldown.SkillType = eTypeValue;
                    if (sTempSkill == "ACTIVE")
                    {
                        cooldown.Active = true;
                    }
                    else //parse timing
                    {
                        int? iSeconds = PleaseWaitSequence.ParseMinutesAndSecondsToSeconds(sTempSkill);
                        if (!iSeconds.HasValue) return;
                        int iSecondsValue = iSeconds.Value;
                        if (iSecondsValue > 0)
                        {
                            cooldown.NextAvailable = DateTime.UtcNow.AddSeconds(iSecondsValue);
                        }
                    }
                    cooldowns.Add(cooldown);
                }

                List<string> spells = new List<string>();
                foreach (string sNextSpell in spellsRaw)
                {
                    string sNext = sNextSpell.Trim();
                    if (string.IsNullOrEmpty(sNext))
                    {
                        return;
                    }
                    spells.Add(sNext);
                }
                if (spells.Count == 0)
                {
                    return;
                }

                _onSatisfied(flParams, cooldowns, spells);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class RemoveEquipmentSequence : IOutputProcessingSequence
    {
        private Action<FeedLineParameters> _onSatisfied;
        public RemoveEquipmentSequence(Action<FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0)
            {
                string sFirstLine = Lines[0];
                if (sFirstLine.StartsWith ("You remove ") || sFirstLine == "You aren't wearing anything that can be removed.")
                {
                    _onSatisfied(flParams);
                    flParams.FinishedProcessing = true;
                }
            }
        }
    }

    public class WhoOutputSequence : IOutputProcessingSequence
    {
        private Action<FeedLineParameters, HashSet<string>> _onSatisfied;
        public WhoOutputSequence(Action<FeedLineParameters, HashSet<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int index = 0;
            HashSet<string> playerNames = null;
            foreach (string nextLine in Lines)
            {
                if (index == 0)
                {
                    if (!string.IsNullOrEmpty(nextLine)) break;
                }
                else if (index == 1)
                {
                    if (!nextLine.StartsWith("Player ")) break;
                }
                else if (index == 2)
                {
                    foreach (char c in nextLine)
                    {
                        if (c != '-')
                        {
                            return;
                        }
                    }
                }
                else if (nextLine.StartsWith("-"))
                {
                    break;
                }
                else
                {
                    int iSpaceIndex = nextLine.IndexOf(' ');
                    if (iSpaceIndex <= 0) break;
                    string playerName = nextLine.Substring(0, iSpaceIndex);
                    foreach (char c in playerName)
                    {
                        if (!char.IsLetter(c))
                        {
                            return;
                        }
                    }
                    if (playerNames == null)
                    {
                        playerNames = new HashSet<string>();
                    }
                    if (playerNames.Contains(playerName))
                    {
                        return;
                    }
                    playerNames.Add(playerName);
                }
                index++;
            }
            if (playerNames != null)
            {
                flParams.FinishedProcessing = true;
                _onSatisfied(flParams, playerNames);
            }
        }
    }

    public class SkillCooldown
    {
        public SkillWithCooldownType SkillType { get; set; }
        public bool Active { get; set; }
        public DateTime? NextAvailable { get; set; }
    }

    internal enum RoomTransitionType
    {
        Initial,
        Move,
        Flee,
        Hazy,
    }

    internal class InitialLoginInfo
    {
        public string RoomName { get; set; }
        public string ObviousExits { get; set; }
        public string List1 { get; set; }
        public string List2 { get; set; }
        public string List3 { get; set; }
    }

    internal class RoomTransitionInfo
    {
        public RoomTransitionType TransitionType { get; set; }
        public string RoomName { get; set; }
        public List<string> ObviousExits { get; set; }
        public FeedLineParameters FeedLineParameters { get; set; }
        public List<PlayerEntity> Players { get; set; }
        public List<ItemEntity> Items { get; set; }
        public List<MobEntity> Mobs { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    internal class InitialLoginSequence : IOutputProcessingSequence
    {
        public Action<InitialLoginInfo> _onSatisfied;

        public InitialLoginSequence(Action<InitialLoginInfo> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> lines = flParams.Lines;
            bool foundLoggedInBroadcast = false;
            int? foundBlankLine = null;
            for (int i = 0; i < lines.Count; i++)
            {
                string nextLine = lines[i];
                if (foundLoggedInBroadcast)
                {
                    if (string.IsNullOrEmpty(nextLine))
                    {
                        foundBlankLine = i;
                        break;
                    }
                }
                else if (nextLine.StartsWith("###") && nextLine.EndsWith(" just logged in."))
                {
                    foundLoggedInBroadcast = true;
                }
            }
            if (foundBlankLine.HasValue)
            {
                InitialLoginInfo ili = RoomTransitionSequence.ProcessRoomForInitialization(lines, foundBlankLine.Value);
                if (ili != null)
                {
                    _onSatisfied(ili);
                    flParams.FinishedProcessing = true;
                }
            }
        }
    }

    internal class RoomTransitionSequence : IOutputProcessingSequence
    {
        private Action<RoomTransitionInfo> _onSatisfied;
        public RoomTransitionSequence(Action<RoomTransitionInfo> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            RoomTransitionType rtType = RoomTransitionType.Move;
            int nextLineIndex = 0;

            //skip fleeing messages for scared exits
            while (nextLineIndex < Lines.Count &&
                (Lines[nextLineIndex].StartsWith("Scared of going ") || Lines[nextLineIndex].StartsWith("You fell and hurt yourself for ")))
            {
                nextLineIndex++;
            }

            string sNextLine = Lines[nextLineIndex];
            if (Lines[nextLineIndex] == "You run like a chicken.")
            {
                rtType = RoomTransitionType.Flee;
                nextLineIndex++;
            }
            else if (sNextLine == "You phase in and out of existence.")
            {
                rtType = RoomTransitionType.Hazy;
                nextLineIndex++;
            }

            //blank line before room name
            if (Lines[nextLineIndex] != string.Empty) return;

            if (ProcessRoom(Lines, nextLineIndex, rtType, flParams, _onSatisfied))
            {
                flParams.FinishedProcessing = true;
            }
        }

        internal static InitialLoginInfo ProcessRoomForInitialization(List<string> Lines, int nextLineIndex)
        {
            if (Lines[nextLineIndex] != string.Empty) return null;
            nextLineIndex++;

            if (nextLineIndex >= Lines.Count) return null;
            string sRoomName = Lines[nextLineIndex];
            if (string.IsNullOrEmpty(sRoomName)) return null;
            nextLineIndex++;

            //blank line after room name
            if (Lines[nextLineIndex] != string.Empty) return null;
            nextLineIndex++;

            string exitsString = StringProcessing.GetListAsString(Lines, nextLineIndex, "Obvious exits: ", true, out nextLineIndex);
            if (exitsString == null)
            {
                return null;
            }
            string room1List = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
            string room2List = null;
            string room3List = null;
            if (room1List != null)
            {
                room2List = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
                if (room2List != null)
                {
                    room3List = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
                }
            }

            InitialLoginInfo ili = new InitialLoginInfo();
            ili.RoomName = sRoomName;
            ili.ObviousExits = exitsString;
            ili.List1 = room1List;
            ili.List2 = room2List;
            ili.List3 = room3List;
            return ili;
        }

        internal static bool ProcessRoom(string sRoomName, string exitsList, string list1, string list2, string list3, Action<RoomTransitionInfo> onSatisfied, FeedLineParameters flParams, RoomTransitionType rtType)
        {
            List<string> exits = StringProcessing.ParseList(exitsList);
            if (exits == null)
            {
                return false;
            }

            List<ItemEntity> items = new List<ItemEntity>();
            List<MobEntity> mobs = new List<MobEntity>();
            List<PlayerEntity> players = new List<PlayerEntity>();
            List<string> errorMessages = new List<string>();
            HashSet<string> playerNames = flParams.PlayerNames;

            List<string> roomList3 = null;
            List<string> roomList2 = null;
            List<string> roomList1 = null;
            if (list3 != null)
            {
                roomList3 = StringProcessing.ParseList(list3);
            }
            if (list2 != null)
            {
                roomList2 = StringProcessing.ParseList(list2);
            }
            if (list1 != null)
            {
                roomList1 = StringProcessing.ParseList(list1);
            }

            if (roomList3 != null) //this is known to be the item list
            {
                LoadItems(items, roomList3, errorMessages);
            }
            if (roomList2 != null)
            {
                EntityTypeFlags possibleTypes = roomList3 != null ? EntityTypeFlags.Mob : EntityTypeFlags.Mob | EntityTypeFlags.Item;
                EntityType? foundType = null;
                foreach (string next in roomList2)
                {
                    Entity e = Entity.GetEntity(next, possibleTypes, errorMessages, playerNames);
                    EntityType eType = e.Type;
                    if (eType != EntityType.Unknown)
                    {
                        foundType = eType;
                        break;
                    }
                }
                if (foundType.HasValue)
                {
                    EntityType foundTypeValue = foundType.Value;
                    if (foundTypeValue == EntityType.Mob)
                    {
                        LoadMobs(mobs, roomList2, errorMessages);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList2, errorMessages);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else //could not identity anything
                {
                    foreach (string s in roomList2)
                    {
                        errorMessages.Add("Failed to identify " + s);
                    }
                }
            }
            if (roomList1 != null)
            {
                EntityTypeFlags possibleTypes;
                if (roomList3 != null)
                {
                    possibleTypes = EntityTypeFlags.Player;
                }
                else if (roomList2 != null)
                {
                    if (mobs.Count > 0)
                        possibleTypes = EntityTypeFlags.Player;
                    else
                        possibleTypes = EntityTypeFlags.Player | EntityTypeFlags.Mob;
                }
                else
                {
                    if (mobs.Count > 0)
                        possibleTypes = EntityTypeFlags.Player;
                    else if (items.Count > 0)
                        possibleTypes = EntityTypeFlags.Player | EntityTypeFlags.Mob;
                    else
                        possibleTypes = EntityTypeFlags.Player | EntityTypeFlags.Mob | EntityTypeFlags.Item;
                }
                EntityType? foundType = null;
                bool canBePlayers = true;
                foreach (string next in roomList1)
                {
                    Entity e = Entity.GetEntity(next, possibleTypes, errorMessages, playerNames);
                    EntityType eType = e.Type;
                    if (eType != EntityType.Unknown)
                    {
                        foundType = eType;
                        break;
                    }
                    int index = 0;
                    foreach (char c in next)
                    {
                        bool ok;
                        if (index == 0)
                            ok = char.IsUpper(c);
                        else
                            ok = char.IsLower(c);
                        if (!ok)
                        {
                            canBePlayers = false;
                            break;
                        }
                    }
                    if (!playerNames.Contains(next))
                    {
                        canBePlayers = false;
                    }
                }
                if (foundType.HasValue)
                {
                    EntityType foundTypeValue = foundType.Value;
                    if (foundTypeValue == EntityType.Player)
                    {
                        LoadPlayers(players, roomList1, errorMessages, playerNames);
                    }
                    else if (foundTypeValue == EntityType.Mob)
                    {
                        LoadMobs(mobs, roomList1, errorMessages);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList1, errorMessages);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else if (canBePlayers) //presumably players
                {
                    LoadPlayers(players, roomList1, errorMessages, playerNames);
                }
                else if (mobs.Count == 0)
                {
                    LoadMobs(mobs, roomList1, errorMessages);
                }
                else
                {
                    LoadItems(items, roomList1, errorMessages);
                }
            }

            foreach (var nextItem in items)
            {
                UnknownItemEntity uie = nextItem as UnknownItemEntity;
                if (uie != null)
                {
                    errorMessages.Add("Unknown mob/item: " + uie.Name);
                }
            }
            foreach (var nextMob in mobs)
            {
                UnknownMobEntity ume = nextMob as UnknownMobEntity;
                if (ume != null)
                {
                    errorMessages.Add("Unknown mob/item: " + ume.Name);
                }
            }

            RoomTransitionInfo rti = new RoomTransitionInfo();
            rti.TransitionType = rtType;
            rti.RoomName = sRoomName;
            rti.ObviousExits = exits;
            rti.FeedLineParameters = flParams;
            rti.Players = players;
            rti.Mobs = mobs;
            rti.Items = items;
            rti.ErrorMessages = errorMessages;
            onSatisfied(rti);
            return true;
        }

        internal static bool ProcessRoom(List<string> Lines, int nextLineIndex, RoomTransitionType rtType, FeedLineParameters flParams, Action<RoomTransitionInfo> onSatisfied)
        {
            if (Lines[nextLineIndex] != string.Empty) return false;
            nextLineIndex++;

            if (nextLineIndex >= Lines.Count) return false;
            string sRoomName = Lines[nextLineIndex];
            if (string.IsNullOrEmpty(sRoomName)) return false;
            nextLineIndex++;

            //blank line after room name
            if (Lines[nextLineIndex] != string.Empty) return false;
            nextLineIndex++;

            string exitsString = StringProcessing.GetListAsString(Lines, nextLineIndex, "Obvious exits: ", true, out nextLineIndex);
            string list1String = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
            string list2String = null;
            string list3String = null;
            if (list1String != null)
            {
                list2String = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
            }
            if (list2String != null)
            {
                list3String = StringProcessing.GetListAsString(Lines, nextLineIndex, "You see ", true, out nextLineIndex);
            }

            return ProcessRoom(sRoomName, exitsString, list1String, list2String, list3String, onSatisfied, flParams, rtType);
        }

        private static void LoadItems(List<ItemEntity> items, List<string> itemNames, List<string> errorMessages)
        {
            foreach (string next in itemNames)
            {
                Entity e = Entity.GetEntity(next, EntityTypeFlags.Item, errorMessages, null);
                if (e != null)
                {
                    if (e is ItemEntity)
                    {
                        items.Add((ItemEntity)e);
                    }
                    else
                    {
                        errorMessages.Add("Nonitem found in item list: " + next);
                    }
                }
            }
        }
        private static void LoadMobs(List<MobEntity> mobs, List<string> mobNames, List<string> errorMessages)
        {
            foreach (string next in mobNames)
            {
                Entity e = Entity.GetEntity(next, EntityTypeFlags.Mob, errorMessages, null);
                if (e != null)
                {
                    if (e is MobEntity)
                    {
                        mobs.Add((MobEntity)e);
                    }
                    else
                    {
                        errorMessages.Add("Nonmob found in mob list: " + next);
                    }
                }
            }
        }

        private static void LoadPlayers(List<PlayerEntity> players, List<string> currentPlayerNames, List<string> errorMessages, HashSet<string> allPlayerNames)
        {
            foreach (string next in currentPlayerNames)
            {
                PlayerEntity pEntity = (PlayerEntity)Entity.GetEntity(next, EntityTypeFlags.Player, errorMessages, allPlayerNames);
                if (pEntity != null)
                {
                    if (pEntity.Name.Contains(" "))
                    {
                        errorMessages.Add("Unexpected player name: " + next);
                    }
                    players.Add(pEntity);
                }
                else
                {
                    errorMessages.Add("Failed to process player: " + next);
                }
            }
        }
    }

    public class CastOffensiveSpellSequence : IOutputProcessingSequence
    {
        public Action<int, FeedLineParameters> _onSatisfied;
        public CastOffensiveSpellSequence(Action<int, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (!backgroundCommandType.HasValue || backgroundCommandType.Value != BackgroundCommandType.OffensiveSpell)
            {
                return;
            }
            bool satisfied = false;
            string sStart = "You cast a ";
            string sMiddle1 = " spell on ";
            string sMiddle2 = " for ";
            string sEnd = " damage.";
            int damage = 0;
            foreach (string nextLine in Lines)
            {
                if (!nextLine.StartsWith(sStart))
                {
                    continue;
                }
                int findIndex = nextLine.IndexOf(sMiddle1, sStart.Length);
                if (findIndex < 0)
                {
                    continue;
                }
                damage = AttackSequence.GetDamage(nextLine, findIndex + sMiddle1.Length, sMiddle2, sEnd);
                if (damage > 0)
                {
                    satisfied = true;
                    break;
                }
            }
            if (satisfied)
            {
                _onSatisfied(damage, flParams);
            }
        }
    }

    public class AttackSequence : IOutputProcessingSequence
    {
        public Action<bool, int, FeedLineParameters> _onSatisfied;
        public AttackSequence(Action<bool, int, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (!backgroundCommandType.HasValue || backgroundCommandType.Value != BackgroundCommandType.Attack)
            {
                return;
            }
            bool fumbled = false;
            bool satisfied = false;
            int damage = 0;
            foreach (string nextLine in Lines)
            {
                if (nextLine == "You missed.")
                {
                    satisfied = true;
                    break;
                }
                else if (nextLine.StartsWith("Your attack has no effect on "))
                {
                    satisfied = true;
                    break;
                }
                else if (nextLine.StartsWith("Your power attack has no effect on "))
                {
                    satisfied = true;
                    break;
                }
                else if (nextLine == "You FUMBLED your weapon.")
                {
                    satisfied = true;
                    fumbled = true;
                    break;
                }
                else if (nextLine == "You STUMBLE and miss your unarmed attack.")
                {
                    satisfied = true;
                    break;
                }
                else if (MatchesPowerAttackMissPattern(nextLine))
                {
                    satisfied = true;
                    break;
                }
                else
                {
                    damage = MatchesHitPattern(nextLine);
                    if (damage > 0)
                    {
                        satisfied = true;
                        break;
                    }
                }
            }
            if (satisfied)
            {
                _onSatisfied(fumbled, damage, flParams);
            }
        }

        public bool MatchesPowerAttackMissPattern(string nextLine)
        {
            return nextLine.StartsWith("Your power attack ") && nextLine.EndsWith(" missed.");
        }

        public int MatchesHitPattern(string nextLine)
        {
            string sStart = "Your ";
            string sMiddle1 = " hits for ";
            string sEnd = " damage.";
            if (!nextLine.StartsWith(sStart))
            {
                return 0;
            }
            return GetDamage(nextLine, sStart.Length, sMiddle1, sEnd);
        }

        public static int GetDamage(string nextLine, int start, string beforeDamageText, string afterDamageText)
        {
            int findIndex = nextLine.IndexOf(beforeDamageText, start);
            if (findIndex < 0)
            {
                return 0;
            }
            int damageStart = findIndex + beforeDamageText.Length;
            int endDamageIndex = nextLine.IndexOf(afterDamageText, damageStart);
            if (endDamageIndex < 0)
            {
                return 0;
            }
            if (damageStart == endDamageIndex) //make sure there actually is text for the damage number
            {
                return 0;
            }
            if (endDamageIndex + afterDamageText.Length != nextLine.Length) //make sure the text ends with the after damage text
            {
                return 0;
            }
            if (!int.TryParse(nextLine.Substring(damageStart, endDamageIndex - damageStart), out int damageCount))
            {
                return 0;
            }
            return damageCount;
        }
    }

    internal class MobStatusSequence : IOutputProcessingSequence
    {
        private Action<MonsterStatus, FeedLineParameters> _onSatisfied;

        public MobStatusSequence(Action<MonsterStatus, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                return;
            }
            bool firstLine = true;
            MonsterStatus? status = null;
            foreach (string nextLine in Lines)
            {
                if (firstLine)
                {
                    if (!nextLine.StartsWith("You see "))
                    {
                        return;
                    }
                    firstLine = false;
                }
                else
                {
                    if (nextLine.EndsWith(" is in excellent condition."))
                    {
                        status = MonsterStatus.ExcellentCondition;
                    }
                    else if (nextLine.EndsWith(" has a few small scratches."))
                    {
                        status = MonsterStatus.FewSmallScratches;
                    }
                    else if (nextLine.EndsWith(" is wincing in pain."))
                    {
                        status = MonsterStatus.WincingInPain;
                    }
                    else if (nextLine.EndsWith(" is slightly bruised and battered."))
                    {
                        status = MonsterStatus.SlightlyBruisedAndBattered;
                    }
                    else if (nextLine.EndsWith(" has some minor wounds."))
                    {
                        status = MonsterStatus.SomeMinorWounds;
                    }
                    else if (nextLine.EndsWith(" is bleeding profusely."))
                    {
                        status = MonsterStatus.BleedingProfusely;
                    }
                    else if (nextLine.EndsWith(" has a nasty and gaping wound."))
                    {
                        status = MonsterStatus.NastyAndGapingWound;
                    }
                    else if (nextLine.EndsWith(" has many grevious wounds."))
                    {
                        status = MonsterStatus.ManyGreviousWounds;
                    }
                    else if (nextLine.EndsWith(" is mortally wounded."))
                    {
                        status = MonsterStatus.MortallyWounded;
                    }
                    else if (nextLine.EndsWith(" is barely clinging to life."))
                    {
                        status = MonsterStatus.BarelyClingingToLife;
                    }
                    if (status.HasValue)
                    {
                        flParams.FinishedProcessing = true;
                        flParams.SuppressEcho = !string.IsNullOrEmpty(flParams.CurrentlyFightingMob);
                        _onSatisfied(status.Value, flParams);
                        return;
                    }
                }
            }
        }
    }

    public class PleaseWaitSequence : IOutputProcessingSequence
    {
        private const string PLEASE_WAIT_PREFIX = "Please wait ";
        private const string ENDS_WITH_MINUTES_SUFFIX = " minutes.";
        private const string ENDS_WITH_SECONDS_SUFFIX = " seconds.";
        private Action<int, FeedLineParameters> _onSatisfied;
        private int? _lastMeleeWaitSeconds;
        private int? _lastMagicWaitSeconds;

        public void ClearLastMeleeWaitSeconds()
        {
            _lastMeleeWaitSeconds = null;
        }

        public void ClearLastMagicWaitSeconds()
        {
            _lastMagicWaitSeconds = null;
        }

        public PleaseWaitSequence(Action<int, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (!backgroundCommandType.HasValue)
            {
                return;
            }
            BackgroundCommandType bctValue = backgroundCommandType.Value;
            int? waitSeconds = null;
            foreach (string nextLine in Lines)
            {
                //skip empty lines. I don't have evidence this ever happens, but adding to be safer.
                if (string.IsNullOrEmpty(nextLine))
                {
                    continue;
                }
                if (!nextLine.StartsWith(PLEASE_WAIT_PREFIX) || nextLine.Length == PLEASE_WAIT_PREFIX.Length)
                {
                    break;
                }
                string remainder = nextLine.Substring(PLEASE_WAIT_PREFIX.Length);
                if (remainder == "1 more second.")
                {
                    waitSeconds = 1;
                    break;
                }
                if (remainder.EndsWith(ENDS_WITH_MINUTES_SUFFIX))
                {
                    if (remainder.Length == ENDS_WITH_MINUTES_SUFFIX.Length)
                    {
                        break;
                    }
                    string rest = remainder.Substring(0, remainder.Length - ENDS_WITH_MINUTES_SUFFIX.Length);
                    int? iParsedSeconds = ParseMinutesAndSecondsToSeconds(rest);
                    if (!iParsedSeconds.HasValue)
                    {
                        break;
                    }
                    waitSeconds = iParsedSeconds.Value;
                }
                else if (remainder.EndsWith(ENDS_WITH_SECONDS_SUFFIX))
                {
                    if (remainder.Length == ENDS_WITH_SECONDS_SUFFIX.Length)
                    {
                        break;
                    }
                    string rest = remainder.Substring(0, remainder.Length - ENDS_WITH_SECONDS_SUFFIX.Length);
                    if (int.TryParse(rest, out int iSeconds))
                    {
                        waitSeconds = iSeconds;
                    }
                }
                break;
            }
            if (waitSeconds.HasValue)
            {
                int? lastWaitSeconds = null;
                int newWaitSeconds = waitSeconds.Value;
                if (bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.OffensiveSpell)
                {
                    lastWaitSeconds = _lastMagicWaitSeconds;
                }
                else if (bctValue == BackgroundCommandType.Attack)
                {
                    lastWaitSeconds = _lastMeleeWaitSeconds;
                }
                if (lastWaitSeconds.HasValue && lastWaitSeconds.Value == newWaitSeconds)
                {
                    flParams.SuppressEcho = true;
                }
                if (bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.OffensiveSpell)
                {
                    _lastMagicWaitSeconds = newWaitSeconds;
                }
                else if (bctValue == BackgroundCommandType.Attack)
                {
                    _lastMeleeWaitSeconds = newWaitSeconds;
                }
                flParams.FinishedProcessing = true;
                _onSatisfied(newWaitSeconds, flParams);
            }
        }

        internal static int? ParseMinutesAndSecondsToSeconds(string input)
        {
            int? ret = null;
            if (input != null && input.Contains(":"))
            {
                string[] sPieces = input.Split(new string[] { ":" }, StringSplitOptions.None);
                int iMinutes;
                int iSeconds;
                if (sPieces.Length == 2 && int.TryParse(sPieces[0], out iMinutes) && int.TryParse(sPieces[1], out iSeconds))
                {
                    ret = (iMinutes * 60) + iSeconds;
                }
            }
            return ret;
        }
    }

    public class SuccessfulSearchSequence : IOutputProcessingSequence
    {
        private const string YOU_FIND_A_HIDDEN_EXIT = "You find a hidden exit: ";
        private Action<List<string>, FeedLineParameters> _onSatisfied;
        public SuccessfulSearchSequence(Action<List<string>, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (!backgroundCommandType.HasValue || backgroundCommandType.Value != BackgroundCommandType.Search)
            {
                return;
            }
            List<string> foundExits = null;
            foreach (string nextLine in Lines)
            {
                if (!nextLine.StartsWith(YOU_FIND_A_HIDDEN_EXIT))
                {
                    continue;
                }
                int beginLength = YOU_FIND_A_HIDDEN_EXIT.Length;
                int periodIndex = nextLine.IndexOf('.', beginLength);
                if (periodIndex > 0 && periodIndex != beginLength)
                {
                    if (foundExits == null) foundExits = new List<string>();
                    foundExits.Add(nextLine.Substring(beginLength, periodIndex - beginLength));
                }
            }
            if (foundExits != null && foundExits.Count > 0)
            {
                _onSatisfied(foundExits, flParams);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class InformationalMessagesSequence : IOutputProcessingSequence
    {
        public Action<List<InformationalMessages>, List<string>, List<string>, List<string>> _onSatisfied;

        public InformationalMessagesSequence(Action<List<InformationalMessages>, List<string>, List<string>, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters Parameters)
        {
            List<InformationalMessages> messages = null;
            List<string> broadcastMessages = null;
            List<string> Lines = Parameters.Lines;
            List<string> addedPlayers = null;
            List<string> removedPlayers = null;
            List<int> linesToRemove = null;
            bool haveDataToDisplay = false;
            for (int i = 0; i < Lines.Count; i++)
            {
                bool whitespaceLine = false;
                InformationalMessages? im = null;
                string sLine = Lines[i];
                bool isBroadcast = false;
                if (sLine == "The sun rises.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.DayStart;
                }
                else if (sLine == "The sun disappears over the horizon.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.NightStart;
                }
                else if (sLine == "The Bullroarer has arrived in Mithlond.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.BullroarerInMithlond;
                }
                else if (sLine == "The Bullroarer has arrived in Nindamos.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.BullroarerInNindamos;
                }
                else if (sLine == "The air is still and quiet." ||
                         sLine == "Light clouds appear over the mountains." ||
                         sLine == "A light breeze blows from the south." ||
                         sLine == "Clear, blue skies cover the land." ||
                         sLine == "It's a beautiful day today." ||
                         sLine == "The glaring sun beats down upon the inhabitants of the world." ||
                         sLine == "The sun shines brightly across the land." ||
                         sLine == "The earth trembles under your feet." ||
                         sLine == "The sky is dark as pitch." ||
                         sLine == "The full moon shines across the land." ||
                         sLine == "A sliver of silver can be seen in the night sky." ||
                         sLine == "Thunderheads roll in from the east." ||
                         sLine == "A heavy fog blankets the earth." ||
                         sLine == "A light rain falls quietly." ||
                         sLine == "Sheets of rain pour down from the skies." ||
                         sLine == "A torrent soaks the ground." ||
                         sLine == "A strong wind blows across the land." ||
                         sLine == "Player saved." ||
                         sLine == "### The Celduin Express is ready for boarding in Bree.")
                {
                    //These lines will be removed.
                }
                else if (sLine.StartsWith("###"))
                {
                    if (sLine.StartsWith("### "))
                    {
                        bool? logFlag = null;
                        if (sLine.EndsWith(" just logged in."))
                        {
                            logFlag = true;
                        }
                        else if (sLine.EndsWith(" just logged off."))
                        {
                            logFlag = false;
                        }
                        if (logFlag.HasValue)
                        {
                            bool logFlagValue = logFlag.Value;
                            int startCharacters = "### ".Length;
                            int iSpaceIndex = sLine.IndexOf(' ', startCharacters);
                            List<string> playerList;
                            if (logFlagValue)
                            {
                                if (addedPlayers == null) addedPlayers = new List<string>();
                                playerList = addedPlayers;
                            }
                            else
                            {
                                if (removedPlayers == null) removedPlayers = new List<string>();
                                playerList = removedPlayers;
                            }
                            playerList.Add(sLine.Substring(startCharacters, iSpaceIndex - startCharacters));
                        }
                    }
                    isBroadcast = true;
                }
                else if (sLine.StartsWith(" ###"))
                {
                    isBroadcast = true;
                }
                else if (!string.IsNullOrWhiteSpace(sLine))
                {
                    haveDataToDisplay = true;
                    break;
                }
                else //whitespace
                {
                    whitespaceLine = true;
                }
                if (isBroadcast)
                {
                    im = InformationalMessages.Broadcast;
                    if (broadcastMessages == null)
                        broadcastMessages = new List<string>();
                }
                if (!whitespaceLine)
                {
                    bool removeLine = false;
                    if (im.HasValue)
                    {
                        InformationalMessages imValue = im.Value;
                        if (imValue == InformationalMessages.Broadcast)
                        {
                            broadcastMessages.Add(sLine);
                            removeLine = true;
                        }
                        else
                        {
                            if (messages == null)
                                messages = new List<InformationalMessages>();
                            messages.Add(im.Value);
                        }
                    }
                    else
                    {
                        removeLine = true;
                    }
                    if (removeLine)
                    {
                        if (linesToRemove == null)
                            linesToRemove = new List<int>();
                        linesToRemove.Add(i);
                    }
                }
            }
            if (linesToRemove != null)
            {
                linesToRemove.Reverse();
                foreach (int i in linesToRemove)
                {
                    Lines.RemoveAt(i);
                }
                if (!haveDataToDisplay)
                {
                    Parameters.SuppressEcho = true;
                    Parameters.FinishedProcessing = true;
                }
            }
            if (messages != null || broadcastMessages != null)
            {
                _onSatisfied(messages, broadcastMessages, addedPlayers, removedPlayers);
            }
        }
    }

    public static class StringProcessing
    {
        public static string GetListAsString(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex)
        {
            nextLineIndex = 0;
            bool foundStartsWith = false;
            bool finished = false;
            StringBuilder sb = null;
            string sNextLine;
            while (!foundStartsWith)
            {
                if (lineIndex >= inputs.Count) //reached the end of the list
                {
                    return null;
                }
                sNextLine = inputs[lineIndex];
                if (sNextLine == null || !sNextLine.StartsWith(startsWith))
                {
                    if (requireExactStartsWith)
                    {
                        return null;
                    }
                    else
                    {
                        lineIndex++;
                        continue;
                    }
                }
                foundStartsWith = true;
                sb = new StringBuilder();
                int startsWithLength = startsWith.Length;
                if (sNextLine.Length != startsWithLength)
                {
                    int iPeriodIndex = sNextLine.IndexOf('.', startsWithLength);
                    if (iPeriodIndex == 0)
                    {
                        return null;
                    }
                    else if (iPeriodIndex < 0)
                    {
                        sb.AppendLine(sNextLine.Substring(startsWithLength));
                    }
                    else
                    {
                        sb.AppendLine(sNextLine.Substring(startsWithLength, iPeriodIndex - startsWithLength));
                        finished = true;
                        nextLineIndex = lineIndex + 1;
                    }
                }
            }
            if (!finished)
            {
                while (true)
                {
                    lineIndex++;
                    if (lineIndex >= inputs.Count)
                    {
                        return null;
                    }
                    sNextLine = inputs[lineIndex];
                    if (sNextLine != null)
                    {
                        int iPeriodIndex = sNextLine.IndexOf('.');
                        if (iPeriodIndex == 0)
                        {
                            nextLineIndex = lineIndex + 1;
                            break;
                        }
                        else if (iPeriodIndex < 0)
                        {
                            sb.AppendLine(sNextLine);
                        }
                        else if (iPeriodIndex > 0)
                        {
                            nextLineIndex = lineIndex + 1;
                            sb.AppendLine(sNextLine.Substring(0, iPeriodIndex));
                            break;
                        }
                    }
                }
            }
            return sb.ToString().Replace(Environment.NewLine, " ");
        }

        public static List<string> ParseList(string sFullContent)
        {
            string[] allEntries = sFullContent.Split(new char[] { ',' });
            List<string> ret = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (string next in allEntries)
            {
                string nextEntry = next.Trim();
                if (nextEntry.Contains("  "))
                {
                    sb.Clear();
                    char prevChar = 'X';
                    foreach (char nextChar in nextEntry)
                    {
                        if (nextChar != ' ' || prevChar != ' ')
                        {
                            sb.Append(nextChar);
                        }
                        prevChar = nextChar;
                    }
                    nextEntry = sb.ToString();
                }
                if (string.IsNullOrEmpty(nextEntry))
                {
                    return null;
                }
                ret.Add(nextEntry);
            }
            return ret;
        }

        public static List<string> GetList(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex)
        {
            string sFullContent = GetListAsString(inputs, lineIndex, startsWith, requireExactStartsWith, out nextLineIndex);
            return ParseList(sFullContent);
        }
    }

    public enum InformationalMessages
    {
        DayStart,
        NightStart,
        Broadcast,
        BullroarerInMithlond,
        BullroarerInNindamos,
    }

    public enum SkillWithCooldownType
    {
        PowerAttack,
        Manashield,
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
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
        public int HP { get; set; }
        public int MP { get; set; }
    }

    public abstract class AOutputProcessingSequence
    {
        public abstract void FeedLine(FeedLineParameters Parameters);
    }

    public class FeedLineParameters
    {
        public FeedLineParameters(List<string> Lines)
        {
            this.Lines = Lines;
            this.InfoMessages = new List<InformationalMessages>();
            this.ErrorMessages = new List<string>();
        }
        public List<string> Lines { get; set; }
        public BackgroundCommandType? BackgroundCommandType { get; set; }
        public string CurrentlyFightingMob { get; set; }
        public bool FinishedProcessing { get; set; }
        public bool SuppressEcho { get; set; }
        public CommandResult? CommandResult { get; set; }
        public HashSet<string> PlayerNames { get; set; }
        public int NextLineIndex { get; set; }
        public List<InformationalMessages> InfoMessages { get; set; }
        public List<string> ErrorMessages { get; set; }
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

    public class ConstantOutputSequence : AOutputProcessingSequence
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

        public override void FeedLine(FeedLineParameters flParams)
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
                case ConstantSequenceMatchType.EndsWith:
                    ret = Line.EndsWith(_characters);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }
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
                        if (hp > 0)
                        {
                            int mp = 0;
                            for (int i = 0; i < MPNumbers.Count; i++)
                            {
                                mp = (mp * 10) + MPNumbers[i];
                            }
                            if (mp >= 0)
                            {
                                ret = new OutputItemInfo();
                                ret.SequenceType = OutputItemSequenceType.HPMPStatus;
                                ret.HP = hp;
                                ret.MP = mp;
                            }
                        }
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

    public class TimeOutputSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, int> _onSatisfied;
        private const string PREFIX = "Game-Time: ";

        public TimeOutputSequence(Action<FeedLineParameters, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
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
                int iNight;
                if (dayHalf == DayHalf.AM)
                {
                    if (iNumber == 12)
                        iNight = 0;
                    else
                        iNight = iNumber;
                }
                else //PM
                {
                    if (iNumber == 12)
                        iNight = 12;
                    else
                        iNight = 12 + iNumber;
                }
                _onSatisfied(flParams, iNight);
                flParams.FinishedProcessing = true;
            }
        }

        private enum DayHalf
        {
            AM,
            PM,
        }

        public static bool IsDay(int hour)
        {
            return hour >= 6 && hour < 20;
        }
    }

    public class ScoreOutputSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters, int, int, int, int, int, List<SkillCooldown>, List<string>, bool> _onSatisfied;
        private const string SKILLS_PREFIX = "Skills: ";
        private const string SPELLS_PREFIX = "Spells cast: ";
        private const string GOLD_PREFIX = "Gold: ";
        private const string TO_NEXT_LEVEL_PREFIX = " To Next Level:";

        private string _username;
        public ScoreOutputSequence(string username, Action<FeedLineParameters, int, int, int, int, int, List<SkillCooldown>, List<string>, bool> onSatisfied)
        {
            _username = username;
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int iLevel = 0;
            if (Lines.Count >= 7)
            {
                int iNextLineIndex = 0;
                int iIndex;

                //first line is the player name, title, and level. Parse out the level.
                string sNextLine = Lines[iNextLineIndex++];
                if (sNextLine == null || sNextLine.Length < 17) return;
                if (!sNextLine.StartsWith(_username + " the ", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                if (!sNextLine.EndsWith(")"))
                {
                    return;
                }
                int iSpaceIndex = -1;
                int iPlace = 1;
                for (int i = sNextLine.Length - 2; i >= 0; i--)
                {
                    char cChar = sNextLine[i];
                    if (cChar == ' ')
                    {
                        if (iLevel == 0)
                        {
                            return;
                        }
                        else
                        {
                            iSpaceIndex = i;
                            break;
                        }
                    }
                    else if (!char.IsDigit(cChar))
                    {
                        return;
                    }
                    else
                    {
                        iLevel += int.Parse(cChar.ToString()) * iPlace;
                        iPlace *= 10;
                    }
                }
                if (sNextLine[--iSpaceIndex] != 'l') return;
                if (sNextLine[--iSpaceIndex] != 'v') return;
                if (sNextLine[--iSpaceIndex] != 'l') return;
                if (sNextLine[--iSpaceIndex] != '(') return;
                if (sNextLine[--iSpaceIndex] != ' ') return;

                //second line contains the poisoned indicator
                bool poisoned = false;
                sNextLine = Lines[iNextLineIndex++];
                if (sNextLine != null)
                {
                    if (sNextLine.Contains("*Poisoned*"))
                    {
                        poisoned = true;
                        sNextLine = sNextLine.Replace("*Poisoned*", string.Empty);
                    }
                    if (!string.IsNullOrWhiteSpace(sNextLine))
                    {
                        return;
                    }
                }

                //third line is
                //<space><space>[3 character HP]<slash><3 character max HP> Hit Points<space><space>
                //<space><space>[3 character MP]<slash><3 character max MP> 
                sNextLine = Lines[iNextLineIndex++];
                if (sNextLine == null || sNextLine.Length < 32) return;
                iIndex = 0;
                if (sNextLine[iIndex++] != ' ') return;
                if (sNextLine[iIndex++] != ' ') return;
                string sCurrentHitPoints = sNextLine.Substring(iIndex, 3).Trim();
                iIndex += 3;
                if (!int.TryParse(sCurrentHitPoints, out _)) return;
                if (sNextLine[iIndex++] != '/') return;
                string sTotalHitPoints = sNextLine.Substring(iIndex, 3).Trim();
                iIndex += 3;
                int iTotalHP;
                if (!int.TryParse(sTotalHitPoints, out iTotalHP)) return;
                string sMiddleString = " Hit Points    ";
                int iMiddleStringLen = sMiddleString.Length;
                if (sNextLine.Substring(iIndex, iMiddleStringLen) != sMiddleString) return;
                iIndex += iMiddleStringLen;
                string sCurrentMagicPoints = sNextLine.Substring(iIndex, 3).Trim();
                iIndex += 3;
                if (!int.TryParse(sCurrentMagicPoints, out _)) return;
                if (sNextLine[iIndex++] != '/') return;
                string sTotalMagicPoints = sNextLine.Substring(iIndex, 3).Trim();
                iIndex += 3;
                int iTotalMP;
                if (!int.TryParse(sTotalMagicPoints, out iTotalMP)) return;
                if (sNextLine[iIndex++] != ' ') return;

                sNextLine = Lines[iNextLineIndex++];
                if (sNextLine == null) return;
                int iGoldPrefixIndex = sNextLine.IndexOf(GOLD_PREFIX);
                int iTNLPrefixIndex = sNextLine.IndexOf(TO_NEXT_LEVEL_PREFIX);
                if (iTNLPrefixIndex - iGoldPrefixIndex - GOLD_PREFIX.Length <= 0) return;
                string sGold = sNextLine.Substring(iGoldPrefixIndex + GOLD_PREFIX.Length, iTNLPrefixIndex - iGoldPrefixIndex - GOLD_PREFIX.Length);
                if (!int.TryParse(sGold, out int iGold))
                {
                    return;
                }
                iIndex = sNextLine.IndexOf(TO_NEXT_LEVEL_PREFIX);
                if (iIndex < 0) return;
                iIndex += TO_NEXT_LEVEL_PREFIX.Length;
                if (iIndex + 10 >= sNextLine.Length) return;
                string sTNL = sNextLine.Substring(iIndex, 10).Trim();
                if (!int.TryParse(sTNL, out int iTNL))
                {
                    return;
                }

                List<string> skillsRaw = StringProcessing.GetList(Lines, iNextLineIndex, SKILLS_PREFIX, true, out iNextLineIndex, SPELLS_PREFIX);
                if (skillsRaw == null)
                {
                    return;
                }

                List<string> spellsRaw = StringProcessing.GetList(Lines, iNextLineIndex, SPELLS_PREFIX, false, out iNextLineIndex, null);
                if (spellsRaw == null)
                {
                    return;
                }

                List<SkillCooldown> cooldowns = new List<SkillCooldown>();

                foreach (string sNextSkill in skillsRaw)
                {
                    int iLastBracket = sNextSkill.LastIndexOf(" [");
                    if (iLastBracket <= 0) return;
                    if (!sNextSkill.EndsWith("]")) return;

                    SkillWithCooldownType skillType = SkillWithCooldownType.Unknown;
                    string skill = sNextSkill.Substring(0, iLastBracket).Trim();
                    if (skill == "(power) attack") skillType = SkillWithCooldownType.PowerAttack;
                    else if (skill == "manashield") skillType = SkillWithCooldownType.Manashield;

                    int totalLength = sNextSkill.Length;
                    int timeLength = totalLength - iLastBracket - 3;
                    if (timeLength < 4) return;

                    string sTempTime = sNextSkill.Substring(iLastBracket + 2, timeLength);
                    SkillCooldown cooldown = new SkillCooldown();
                    cooldown.SkillType = skillType;
                    if (sTempTime == "ACTIVE")
                    {
                        cooldown.Status = SkillCooldownStatus.Active;
                    }
                    else if (sTempTime == "0:00")
                    {
                        cooldown.Status = SkillCooldownStatus.Available;
                    }
                    else //parse timing
                    {
                        int? iSeconds = PleaseWaitSequence.ParseMinutesAndSecondsToSeconds(sTempTime);
                        if (!iSeconds.HasValue) return;
                        int iSecondsValue = iSeconds.Value;
                        if (iSecondsValue > 0)
                        {
                            cooldown.NextAvailable = DateTime.UtcNow.AddSeconds(iSecondsValue);
                        }
                        cooldown.Status = SkillCooldownStatus.Waiting;
                    }
                    cooldown.SkillName = skill;
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

                _onSatisfied(flParams, iLevel, iTotalHP, iTotalMP, iGold, iTNL, cooldowns, spells, poisoned);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class EquipmentSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, List<KeyValuePair<string, string>>> _onSatisfied;
        public EquipmentSequence(Action<FeedLineParameters, List<KeyValuePair<string, string>>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0)
            {
                string sFirstLine = Lines[0];
                if (sFirstLine == "You aren't wearing anything.")
                {
                    _onSatisfied(flParams, new List<KeyValuePair<string, string>>());
                    flParams.FinishedProcessing = true;
                    return;
                }
                if (!sFirstLine.EndsWith(". of equipment.") || Lines.Count < 3 || !string.IsNullOrEmpty(Lines[1]))
                {
                    return;
                }
                List<KeyValuePair<string, string>> eq = new List<KeyValuePair<string, string>>();
                bool foundBlankLine = false;
                for (int i = 2; i < Lines.Count; i++)
                {
                    string sNextLine = Lines[i];
                    if (string.IsNullOrEmpty(sNextLine))
                    {
                        foundBlankLine = true;
                    }
                    else
                    {
                        if (foundBlankLine)
                        {
                            return;
                        }
                        if (!sNextLine.EndsWith(")"))
                        {
                            return;
                        }
                        int iIndex = sNextLine.IndexOf(" - ");
                        if (iIndex <= 0)
                        {
                            return;
                        }
                        string sItemName = sNextLine.Substring(0, iIndex);
                        iIndex = sNextLine.LastIndexOf("(");
                        if (iIndex <= 0)
                        {
                            return;
                        }
                        int lineLen = sNextLine.Length;
                        string sSlot = sNextLine.Substring(iIndex + 1, lineLen - iIndex - 2);
                        eq.Add(new KeyValuePair<string, string>(sItemName, sSlot));
                    }
                }
                _onSatisfied(flParams, eq);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class InventorySequence : AOutputProcessingSequence
    {
        private const string YOU_HAVE_PREFIX = "You have: ";
        private Action<FeedLineParameters, List<ItemEntity>> _onSatisfied;
        public InventorySequence(Action<FeedLineParameters, List<ItemEntity>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0 && !string.IsNullOrEmpty(Lines[0]) && Lines[0].StartsWith(YOU_HAVE_PREFIX))
            {
                string sFullContent = StringProcessing.GetListAsString(Lines, 0, YOU_HAVE_PREFIX, true, out _, null);
                int totalInventoryWeightIndex = sFullContent.IndexOf("Inventory weight is ");
                if (totalInventoryWeightIndex > 0)
                {
                    sFullContent = sFullContent.Substring(0, totalInventoryWeightIndex).Trim();
                    if (sFullContent.EndsWith(".") && sFullContent != ".")
                    {
                        sFullContent = sFullContent.Substring(0, sFullContent.Length - 1);
                        List<string> items = StringProcessing.ParseList(sFullContent);
                        if (items != null)
                        {
                            List<ItemEntity> itemList = new List<ItemEntity>();
                            if (items.Count > 1 || items[0] != "nothing")
                            {
                                RoomTransitionSequence.LoadItems(itemList, items, flParams.ErrorMessages, EntityTypeFlags.Item);
                            }
                            _onSatisfied(flParams, itemList);
                            flParams.FinishedProcessing = true;
                        }
                    }
                }
            }
        }
    }

    public class SpellsSequence : AOutputProcessingSequence
    {
        private const string SPELLS_KNOWN_PREFIX = "Spells known: ";
        private const string SPELLS_CAST_PREFIX = "Spells cast: ";
        private Action<FeedLineParameters, List<string>> _onSatisfied;
        public SpellsSequence(Action<FeedLineParameters, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters Parameters)
        {
            List<string> Lines = Parameters.Lines;
            for (int i = 0; i < Lines.Count; i++)
            {
                string sNextLine = Lines[i];
                if (!string.IsNullOrEmpty(sNextLine))
                {
                    if (sNextLine.StartsWith(SPELLS_KNOWN_PREFIX))
                    {
                        string sList = StringProcessing.GetListAsString(Lines, i, SPELLS_KNOWN_PREFIX, true, out _, SPELLS_CAST_PREFIX);
                        if (string.IsNullOrEmpty(sList)) break;
                        List<string> list = StringProcessing.ParseList(sList);
                        if (list.Count == 1 && list[0] == "None")
                        {
                            list.Clear();
                        }
                        _onSatisfied(Parameters, list);
                        Parameters.FinishedProcessing = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public class WhoOutputSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, HashSet<string>> _onSatisfied;
        public WhoOutputSequence(Action<FeedLineParameters, HashSet<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
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

    public class InventoryEquipmentManagementSequence : AOutputProcessingSequence
    {
        private const string YOU_WIELD_PREFIX = "You wield ";
        private const string YOU_GET_A_PREFIX = "You get ";
        private const string YOU_DROP_A_PREFIX = "You drop ";
        private const string YOU_WEAR_PREFIX = "You wear ";
        private const string YOU_HOLD_PREFIX = "You hold ";
        private const string YOU_REMOVE_PREFIX = "You remove ";
        private const string YOU_REMOVED_PREFIX = "You removed ";
        private const string THE_SHOPKEEP_GIVES_YOU_PREFIX = "The shopkeep gives you ";
        private Action<FeedLineParameters, List<ItemTypeEnum>, bool, bool, int?, int, List<string>, bool> _onSatisfied;
        public InventoryEquipmentManagementSequence(Action<FeedLineParameters, List<ItemTypeEnum>, bool, bool, int?, int, List<string>, bool> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flp)
        {
            List<string> Lines = flp.Lines;
            int iSellGold = 0;
            int? iTotalGold = null;
            bool? isAdd = null;
            List<string> activeSpells = null;
            bool isEquipment = false;
            bool potionConsumed = false;
            if (Lines.Count > 0)
            {
                string firstLine = Lines[0];
                if (firstLine == "You aren't wearing anything that can be removed.")
                {
                    _onSatisfied(flp, new List<ItemTypeEnum>(), false, true, null, 0, null, false);
                    flp.FinishedProcessing = true;
                    return;
                }
                List<ItemTypeEnum> itemsManaged = null;
                if (firstLine.StartsWith(YOU_WEAR_PREFIX))
                {
                    List<string> wornObjects = StringProcessing.GetList(Lines, 0, YOU_WEAR_PREFIX, true, out _, null);
                    List<ItemEntity> items = new List<ItemEntity>();
                    RoomTransitionSequence.LoadItems(items, wornObjects, flp.ErrorMessages, EntityTypeFlags.Item);
                    isEquipment = true;
                    isAdd = true;
                    if (items.Count > 0)
                    {
                        itemsManaged = new List<ItemTypeEnum>();
                        foreach (ItemEntity ie in items)
                        {
                            if (ie is UnknownItemEntity)
                            {
                                flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                            }
                            else if (ie.Count != 1)
                            {
                                flp.ErrorMessages.Add("Unexpected item count for worn equipment " + ie.ItemType.Value.ToString() + ": " + ie.Count);
                            }
                            else
                            {
                                itemsManaged.Add(ie.ItemType.Value);
                            }
                        }
                    }
                }
                else if (firstLine.StartsWith(YOU_HOLD_PREFIX))
                {
                    List<string> heldObjects = StringProcessing.GetList(Lines, 0, YOU_HOLD_PREFIX, true, out _, null);
                    List<ItemEntity> items = new List<ItemEntity>();
                    RoomTransitionSequence.LoadItems(items, heldObjects, flp.ErrorMessages, EntityTypeFlags.Item);
                    isEquipment = true;
                    isAdd = true;
                    if (items.Count > 0)
                    {
                        itemsManaged = new List<ItemTypeEnum>();
                        foreach (ItemEntity ie in items)
                        {
                            if (ie is UnknownItemEntity)
                            {
                                flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                            }
                            else if (ie.Count != 1)
                            {
                                flp.ErrorMessages.Add("Unexpected item count for held equipment " + ie.ItemType.Value.ToString() + ": " + ie.Count);
                            }
                            else
                            {
                                itemsManaged.Add(ie.ItemType.Value);
                            }
                        }
                    }
                }
                else if (firstLine.StartsWith(YOU_REMOVE_PREFIX) || firstLine.StartsWith(YOU_REMOVED_PREFIX))
                {
                    string sExpectedPrefix;
                    if (firstLine.StartsWith(YOU_REMOVE_PREFIX))
                    {
                        sExpectedPrefix = YOU_REMOVE_PREFIX;
                    }
                    else
                    {
                        sExpectedPrefix = YOU_REMOVED_PREFIX;
                    }
                    List<string> removedObjects = StringProcessing.GetList(Lines, 0, sExpectedPrefix, true, out _, null);
                    List<ItemEntity> items = new List<ItemEntity>();
                    RoomTransitionSequence.LoadItems(items, removedObjects, flp.ErrorMessages, EntityTypeFlags.Item);
                    isEquipment = true;
                    isAdd = false;
                    if (items.Count > 0)
                    {
                        itemsManaged = new List<ItemTypeEnum>();
                        foreach (ItemEntity ie in items)
                        {
                            if (ie is UnknownItemEntity)
                            {
                                flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                            }
                            else if (ie.Count != 1)
                            {
                                flp.ErrorMessages.Add("Unexpected item count for removed equipment " + ie.ItemType.Value.ToString() + ": " + ie.Count);
                            }
                            else
                            {
                                itemsManaged.Add(ie.ItemType.Value);
                            }
                        }
                    }
                }
                else if (firstLine.StartsWith(YOU_WIELD_PREFIX))
                {
                    List<string> wieldedObjects = StringProcessing.GetList(Lines, 0, YOU_WIELD_PREFIX, true, out _, null);
                    List<ItemEntity> items = new List<ItemEntity>();
                    RoomTransitionSequence.LoadItems(items, wieldedObjects, flp.ErrorMessages, EntityTypeFlags.Item);
                    isEquipment = true;
                    isAdd = true;
                    if (items.Count > 0)
                    {
                        itemsManaged = new List<ItemTypeEnum>();
                        foreach (ItemEntity ie in items)
                        {
                            if (ie is UnknownItemEntity)
                            {
                                flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                            }
                            else if (ie.Count != 1)
                            {
                                flp.ErrorMessages.Add("Unexpected item count for wielded equipment " + ie.ItemType.Value.ToString() + ": " + ie.Count);
                            }
                            else
                            {
                                itemsManaged.Add(ie.ItemType.Value);
                            }
                        }
                    }
                }
                if (!isEquipment)
                {
                    bool expectCapitalized = false;
                    foreach (string nextLine in Lines)
                    {
                        int lineLength = nextLine.Length;
                        string objectText = string.Empty;
                        if (nextLine.StartsWith(YOU_GET_A_PREFIX) && nextLine != YOU_GET_A_PREFIX)
                        {
                            if (isAdd.HasValue && !isAdd.Value)
                            {
                                return;
                            }
                            isAdd = true;
                            objectText = nextLine.Substring(YOU_GET_A_PREFIX.Length).Trim().TrimEnd('.');
                        }
                        else if (nextLine.StartsWith(YOU_DROP_A_PREFIX) && nextLine != YOU_DROP_A_PREFIX)
                        {
                            if (isAdd.HasValue && isAdd.Value)
                            {
                                return;
                            }
                            isAdd = false;
                            objectText = nextLine.Substring(YOU_DROP_A_PREFIX.Length).Trim().TrimEnd('.');
                        }
                        else if (nextLine.StartsWith(THE_SHOPKEEP_GIVES_YOU_PREFIX))
                        {
                            if (isAdd.HasValue && isAdd.Value)
                            {
                                return;
                            }
                            isAdd = false;
                            if (!nextLine.EndsWith("."))
                            {
                                return;
                            }
                            int goldForPrefixIndex = nextLine.IndexOf(" gold for ");
                            int goldLength = goldForPrefixIndex - THE_SHOPKEEP_GIVES_YOU_PREFIX.Length;
                            if (goldLength <= 0) return;
                            string sGold = nextLine.Substring(THE_SHOPKEEP_GIVES_YOU_PREFIX.Length, goldLength);
                            if (!int.TryParse(sGold, out int iNextGold))
                            {
                                return;
                            }
                            iSellGold += iNextGold;
                            int objectLen = lineLength - goldForPrefixIndex - " gold for ".Length - 1;
                            if (objectLen <= 0) return;
                            objectText = nextLine.Substring(goldForPrefixIndex + " gold for ".Length, objectLen);
                        }
                        else if (nextLine.EndsWith(" disintegrates."))
                        {
                            if (isAdd.HasValue && isAdd.Value)
                            {
                                return;
                            }
                            isAdd = false;
                            int suffixIndex = nextLine.IndexOf(" disintegrates.");
                            if (suffixIndex > 0)
                            {
                                objectText = nextLine.Substring(0, lineLength - " disintegrates.".Length);
                            }
                            expectCapitalized = true;
                        }
                        else if (nextLine == "Substance consumed.")
                        {
                            potionConsumed = true;
                        }
                        else if (nextLine == "Thanks for recycling." ||
                                 nextLine == "You feel better." || //vigor/mend
                                 nextLine == "You start to feel real strange, as if connected to another dimension." || //additional message for detect-invisible
                                 nextLine == "Yuck!  Tastes awful!" || //additional message for endure-fire
                                 nextLine == "Yuck! That's terrible!" || //viscous potion
                                 nextLine == "Yuck!" || //viscous potion
                                 nextLine.EndsWith(" hit points removed."))
                        {
                            continue; //skipped
                        }
                        else if (SelfSpellCastSequence.ACTIVE_SPELL_TO_ACTIVE_TEXT.TryGetValue(nextLine, out string activeSpell))
                        {
                            if (activeSpells == null)
                            {
                                activeSpells = new List<string>() { activeSpell };
                            }
                        }
                        else if (nextLine.StartsWith("You have ") && nextLine.EndsWith(" gold."))
                        {
                            if (nextLine.Length == "You have ".Length + " gold.".Length)
                            {
                                return;
                            }
                            else
                            {
                                string sGold = nextLine.Substring("You have ".Length, nextLine.Length - "You have ".Length - " gold.".Length);
                                if (!int.TryParse(sGold, out int iFoundGold))
                                {
                                    return;
                                }
                                iTotalGold = iFoundGold;
                            }
                            continue;
                        }
                        else if (!string.IsNullOrEmpty(nextLine))
                        {
                            return;
                        }
                        if (!string.IsNullOrEmpty(objectText))
                        {
                            ItemEntity ie = Entity.GetEntity(objectText, EntityTypeFlags.Item, flp.ErrorMessages, null, expectCapitalized) as ItemEntity;
                            if (ie != null)
                            {
                                if (ie is UnknownItemEntity)
                                {
                                    flp.ErrorMessages.Add("Unknown item: " + objectText);
                                }
                                else if (ie.Count != 1)
                                {
                                    flp.ErrorMessages.Add("Unexpected item count for " + objectText + ": " + ie.Count);
                                }
                                else
                                {
                                    if (itemsManaged == null)
                                    {
                                        itemsManaged = new List<ItemTypeEnum>();
                                    }
                                    itemsManaged.Add(ie.ItemType.Value);
                                }
                            }
                        }
                    }
                }
                if (itemsManaged != null || potionConsumed)
                {
                    _onSatisfied(flp, itemsManaged, isAdd.Value, isEquipment, iTotalGold, iSellGold, activeSpells, potionConsumed);
                    flp.FinishedProcessing = true;
                }
            }
        }
    }

    public class SkillCooldown
    {
        public SkillCooldown()
        {
        }
        public string SkillName { get; set; }
        public SkillWithCooldownType SkillType { get; set; }
        public SkillCooldownStatus Status { get; set; }
        public DateTime NextAvailable { get; set; }
        public System.Windows.Forms.Label CooldownLabel { get; set; }
        public string RemainingTextUI { get; set; }
        public Color RemainingColorUI { get; set; }
    }

    internal class InitialLoginInfo
    {
        public string RoomName { get; set; }
        public string ObviousExits { get; set; }
        public string List1 { get; set; }
        public string List2 { get; set; }
        public string List3 { get; set; }
    }

    public class RoomTransitionInfo
    {
        public RoomTransitionType TransitionType { get; set; }
        public string RoomName { get; set; }
        public List<string> ObviousExits { get; set; }
        public List<PlayerEntity> Players { get; set; }
        public List<ItemEntity> Items { get; set; }
        public List<MobEntity> Mobs { get; set; }
        public bool DrankHazy { get; set; }
    }

    internal class InitialLoginSequence : AOutputProcessingSequence
    {
        public Action<InitialLoginInfo> _onSatisfied;

        public InitialLoginSequence(Action<InitialLoginInfo> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            bool isLogin = false;
            foreach (var nextMessage in flParams.InfoMessages)
            {
                if (nextMessage.MessageType == InformationalMessageType.InitialLogin)
                {
                    isLogin = true;
                    break;
                }
            }
            if (isLogin)
            {
                InitialLoginInfo ili = RoomTransitionSequence.ProcessRoomForInitialization(flParams.Lines, flParams.NextLineIndex);
                if (ili != null)
                {
                    _onSatisfied(ili);
                    flParams.FinishedProcessing = true;
                }
            }
        }
    }

    public class RoomTransitionSequence : AOutputProcessingSequence
    {
        private const string YOU_SEE_PREFIX = "You see ";
        private Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> _onSatisfied;
        public RoomTransitionSequence(Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            TrapType eTrapType = TrapType.None;
            RoomTransitionType rtType = RoomTransitionType.Move;
            int iDamage = 0;
            foreach (var nextMessage in flParams.InfoMessages)
            {
                InformationalMessageType eType = nextMessage.MessageType;
                if (eType == InformationalMessageType.Flee)
                {
                    rtType = RoomTransitionType.Flee;
                }
                else if (eType == InformationalMessageType.WordOfRecall)
                {
                    rtType = RoomTransitionType.WordOfRecall;
                }
                else if (eType == InformationalMessageType.Death)
                {
                    rtType = RoomTransitionType.Death;
                }
                else if (eType == InformationalMessageType.FallDamage)
                {
                    eTrapType |= TrapType.Fall;
                    int iNextDamage = nextMessage.Damage;
                    if (iNextDamage > 0)
                    {
                        iDamage += iNextDamage;
                    }
                }
            }
            if (ProcessRoom(Lines, flParams.NextLineIndex, rtType, flParams, _onSatisfied, iDamage, ref eTrapType))
            {
                flParams.FinishedProcessing = true;
            }
        }

        internal static InitialLoginInfo ProcessRoomForInitialization(List<string> Lines, int nextLineIndex)
        {
            InitialLoginInfo ili;
            if (ProcessRoomContent(Lines, ref nextLineIndex, out string sRoomName, out string exitsString, out string room1List, out string room2List, out string room3List))
            {
                ili = new InitialLoginInfo();
                ili.RoomName = sRoomName;
                ili.ObviousExits = exitsString;
                ili.List1 = room1List;
                ili.List2 = room2List;
                ili.List3 = room3List;
            }
            else
            {
                ili = null;
            }
            return ili;
        }

        /// <summary>
        /// processes content of a room
        /// </summary>
        /// <param name="Lines">line information</param>
        /// <param name="nextLineIndex">starting index. the room is processed if it starts at a blank line before the room name.</param>
        internal static bool ProcessRoomContent(List<string> Lines, ref int nextLineIndex, out string sRoomName, out string exitsString, out string room1List, out string room2List, out string room3List)
        {
            int lineCount = Lines.Count;
            int tempIndex = nextLineIndex;
            exitsString = room1List = room2List = room3List = null;
            sRoomName = string.Empty;

            //blank line before room name
            if (tempIndex >= lineCount) return false;
            if (!string.IsNullOrEmpty(Lines[tempIndex])) return false;

            //room name
            tempIndex++;
            if (tempIndex >= lineCount) return false;
            sRoomName = (Lines[tempIndex] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(sRoomName)) return false;

            //blank line after room name
            tempIndex++;
            if (tempIndex >= lineCount) return false;
            if (Lines[tempIndex] != string.Empty) return false;

            //get the obvious exits. If long or short descriptions are on, lines may be skipped.
            tempIndex++;
            exitsString = StringProcessing.GetListAsString(Lines, tempIndex, "Obvious exits: ", false, out tempIndex, YOU_SEE_PREFIX);
            if (exitsString == null)
            {
                return false;
            }

            room1List = StringProcessing.GetListAsString(Lines, tempIndex, "You see ", true, out tempIndex, YOU_SEE_PREFIX);
            room2List = null;
            room3List = null;
            if (room1List != null)
            {
                room2List = StringProcessing.GetListAsString(Lines, tempIndex, "You see ", true, out tempIndex, YOU_SEE_PREFIX);
                if (room2List != null)
                {
                    room3List = StringProcessing.GetListAsString(Lines, tempIndex, "You see ", true, out tempIndex, null);
                }
            }
            nextLineIndex = tempIndex;
            return true;
        }

        public static bool ProcessRoom(string sRoomName, string exitsList, string list1, string list2, string list3, Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> onSatisfied, FeedLineParameters flParams, RoomTransitionType rtType, int damage, TrapType trapType, bool drankHazy)
        {
            List<string> exits = StringProcessing.ParseList(exitsList);
            if (exits == null)
            {
                return false;
            }

            List<ItemEntity> items = new List<ItemEntity>();
            List<MobEntity> mobs = new List<MobEntity>();
            List<PlayerEntity> players = new List<PlayerEntity>();
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
                LoadItems(items, roomList3, flParams.ErrorMessages, EntityTypeFlags.Item);
            }
            if (roomList2 != null)
            {
                EntityTypeFlags possibleTypes = roomList3 != null ? EntityTypeFlags.Mob : EntityTypeFlags.Mob | EntityTypeFlags.Item;
                EntityType? foundType = null;
                foreach (string next in roomList2)
                {
                    Entity e = Entity.GetEntity(next, possibleTypes, flParams.ErrorMessages, playerNames, false);
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
                        LoadMobs(mobs, roomList2, flParams.ErrorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList2, flParams.ErrorMessages, EntityTypeFlags.Item);
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
                        flParams.ErrorMessages.Add("Failed to identify " + s);
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
                    Entity e = Entity.GetEntity(next, possibleTypes, flParams.ErrorMessages, playerNames, false);
                    EntityType eType = e.Type;
                    if (eType != EntityType.Unknown)
                    {
                        foundType = eType;
                        break;
                    }
                    if (canBePlayers && !PlayerEntity.IsValidPlayerName(next))
                    {
                        canBePlayers = false;
                    }
                }
                if (foundType.HasValue)
                {
                    EntityType foundTypeValue = foundType.Value;
                    if (foundTypeValue == EntityType.Player)
                    {
                        LoadPlayers(players, roomList1, flParams.ErrorMessages, playerNames, EntityTypeFlags.Player);
                    }
                    else if (foundTypeValue == EntityType.Mob)
                    {
                        LoadMobs(mobs, roomList1, flParams.ErrorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList1, flParams.ErrorMessages, EntityTypeFlags.Item);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else if (canBePlayers) //presumably players
                {
                    LoadPlayers(players, roomList1, flParams.ErrorMessages, playerNames, possibleTypes);
                }
                else
                {
                    possibleTypes &= ~EntityTypeFlags.Player;
                    if (mobs.Count == 0)
                    {
                        LoadMobs(mobs, roomList1, flParams.ErrorMessages, possibleTypes);
                    }
                    else
                    {
                        LoadItems(items, roomList1, flParams.ErrorMessages, possibleTypes);
                    }
                }
            }

            foreach (var nextItem in items)
            {
                UnknownItemEntity uie = nextItem as UnknownItemEntity;
                if (uie != null)
                {
                    flParams.ErrorMessages.Add("Unknown mob/item: " + uie.Name);
                }
            }
            foreach (var nextMob in mobs)
            {
                UnknownMobEntity ume = nextMob as UnknownMobEntity;
                if (ume != null)
                {
                    flParams.ErrorMessages.Add("Unknown mob/item: " + ume.Name);
                }
            }

            RoomTransitionInfo rti = new RoomTransitionInfo();
            rti.TransitionType = rtType;
            rti.RoomName = sRoomName;
            rti.ObviousExits = exits;
            rti.Players = players;
            rti.Mobs = mobs;
            rti.Items = items;
            rti.DrankHazy = drankHazy;
            onSatisfied(flParams, rti, damage, trapType);
            return true;
        }

        internal static bool ProcessRoom(List<string> Lines, int nextLineIndex, RoomTransitionType rtType, FeedLineParameters flParams, Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> onSatisfied, int damage, ref TrapType trapType)
        {
            int lineCount = Lines.Count;

StartProcessRoom:

            if (!ProcessRoomContent(Lines, ref nextLineIndex, out string sRoomName, out string exitsString, out string list1String, out string list2String, out string list3String))
            {
                return false;
            }

            bool drankHazy = false;

            if (nextLineIndex < lineCount)
            {
                string sNextLine = Lines[nextLineIndex];
                if (string.IsNullOrEmpty(sNextLine))
                {
                    nextLineIndex++;
                    for (int i = nextLineIndex; i < lineCount; i++)
                    {
                        sNextLine = Lines[i];
                        if (sNextLine == "You triggered a hidden dart!")
                        {
                            trapType = trapType | TrapType.PoisonDart;
                        }
                        else if (sNextLine == "You fell into a pit trap!")
                        {
                            nextLineIndex = i + 1;
                            goto StartProcessRoom;
                        }
                        else if (sNextLine == "The hazy potion disintegrates.")
                        {
                            drankHazy = true;
                        }
                        else if (sNextLine == "You tingle all over" || //part of word of recall / hazy
                                 sNextLine == "Substance consumed.") //part of drinking hazy
                        {
                            //skipped
                        }
                        else
                        {
                            int trapDamage = Room.ProcessTrapDamage("You lost ", " hit points.", sNextLine);
                            if (trapDamage > 0) damage += trapDamage;
                        }
                    }
                }
            }

            return ProcessRoom(sRoomName, exitsString, list1String, list2String, list3String, onSatisfied, flParams, rtType, damage, trapType, drankHazy);
        }

        public static void LoadItems(List<ItemEntity> items, List<string> itemNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in itemNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null, false);
                if (CheckForValidItem(next, e, errorMessages, possibleEntityTypes))
                {
                    items.Add((ItemEntity)e);
                }
            }
        }
        private static void LoadMobs(List<MobEntity> mobs, List<string> mobNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in mobNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null, false);
                if (CheckForValidMob(next, e, errorMessages, possibleEntityTypes))
                {
                    mobs.Add((MobEntity)e);
                }
            }
        }

        public static bool CheckForValidItem(string next, Entity e, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            bool ret = false;
            if (e != null)
            {
                if (e is UnknownTypeEntity)
                {
                    errorMessages.Add("Unknown type entity (looking for items): " + next);
                }
                else if (e is ItemEntity)
                {
                    if (e is UnknownItemEntity)
                    {
                        errorMessages.Add("Unknown item entity: " + next);
                    }
                    else
                    {
                        ItemEntity ient = (ItemEntity)e;
                        if (ient.ItemType.HasValue)
                        {
                            StaticItemData sid = ItemEntity.StaticItemData[ient.ItemType.Value];
                            if (sid.WeaponType.HasValue && sid.WeaponType.Value == WeaponType.Unknown)
                            {
                                errorMessages.Add("found unknown weapon type: " + ient.ItemType.Value.ToString());
                            }
                            if (sid.EquipmentType == EquipmentType.Unknown)
                            {
                                errorMessages.Add("found unknown equipment type: " + ient.ItemType.Value.ToString());
                            }
                        }
                        ret = true;
                    }
                }
                else
                {
                    if (possibleEntityTypes == EntityTypeFlags.Item)
                    {
                        errorMessages.Add("Nonitem found in item list: " + next);
                    }
                    else
                    {
                        errorMessages.Add("Unexpected " + possibleEntityTypes.ToString() + ": " + next);
                    }
                }
            }
            return ret;
        }

        public static bool CheckForValidMob(string next, Entity e, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            bool ret = false;
            if (e != null)
            {
                if (e is UnknownTypeEntity)
                {
                    UnknownTypeEntity ute = (UnknownTypeEntity)e;
                    if ((possibleEntityTypes & EntityTypeFlags.Player) == EntityTypeFlags.None)
                    {
                        errorMessages.Add("Unknown type entity (looking for mobs): " + next);
                    }
                }
                else if (e is MobEntity)
                {
                    if (e is UnknownMobEntity)
                    {
                        errorMessages.Add("Unknown mob entity: " + next);
                    }
                    else
                    {
                        ret = true;
                    }
                }
                else
                {
                    if (possibleEntityTypes == EntityTypeFlags.Mob)
                    {
                        errorMessages.Add("Nonmob found in mob list: " + next);
                    }
                    else
                    {
                        errorMessages.Add("Unexpected " + possibleEntityTypes.ToString() + ": " + next);
                    }
                }
            }
            return ret;
        }

        private static void LoadPlayers(List<PlayerEntity> players, List<string> currentPlayerNames, List<string> errorMessages, HashSet<string> allPlayerNames, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in currentPlayerNames)
            {
                PlayerEntity pEntity = (PlayerEntity)Entity.GetEntity(next, possibleEntityTypes, errorMessages, allPlayerNames, false);
                if (pEntity != null)
                {
                    players.Add(pEntity);
                }
                else
                {
                    errorMessages.Add("Failed to process player: " + next);
                }
            }
        }
    }

    public class CastOffensiveSpellSequence : AOutputProcessingSequence
    {
        internal static List<string> EARTH_OFFENSIVE_SPELLS = new List<string>() { "rumble", "crush", "shatterstone", "engulf" };
        internal static List<string> FIRE_OFFENSIVE_SPELLS = new List<string>() { "burn", "fireball", "burstflame", "immolate" };
        internal static List<string> WATER_OFFENSIVE_SPELLS = new List<string>() { "blister", "waterbolt", "steamblast", "bloodboil" };
        internal static List<string> WIND_OFFENSIVE_SPELLS = new List<string>() { "hurt", "dustgust", "shockbolt", "lightning" };
        internal static HashSet<string> ALL_OFFENSIVE_SPELLS;

        public static List<string> GetOffensiveSpellsForRealm(RealmType realm)
        {
            List<string> ret;
            switch (realm)
            {
                case RealmType.Earth:
                    ret = EARTH_OFFENSIVE_SPELLS;
                    break;
                case RealmType.Fire:
                    ret = FIRE_OFFENSIVE_SPELLS;
                    break;
                case RealmType.Water:
                    ret = WATER_OFFENSIVE_SPELLS;
                    break;
                case RealmType.Wind:
                    ret = WIND_OFFENSIVE_SPELLS;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return ret;
        }

        private const string YOU_CAST_A_PREFIX = "You cast a ";
        private const string DAMAGE_PREFIX = " for ";
        private const string DAMAGE_SUFFIX = " damage.";

        static CastOffensiveSpellSequence()
        {
            ALL_OFFENSIVE_SPELLS = new HashSet<string>();
            foreach (List<string> nextList in new List<string>[] { EARTH_OFFENSIVE_SPELLS, FIRE_OFFENSIVE_SPELLS, WATER_OFFENSIVE_SPELLS, WIND_OFFENSIVE_SPELLS })
            {
                foreach (string nextSpell in nextList)
                {
                    ALL_OFFENSIVE_SPELLS.Add(nextSpell);
                }
            }
        }

        public Action<int, bool, MobTypeEnum?, int, FeedLineParameters> _onSatisfied;
        public CastOffensiveSpellSequence(Action<int, bool, MobTypeEnum?, int, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            if (lineCount == 0) return;

            //check the first line matches the pattern "You cast a X spell on <something> for Y damage."
            string nextLine = Lines[0];
            if (!nextLine.StartsWith(YOU_CAST_A_PREFIX)) return;
            int iLineLength = nextLine.Length;
            int startLength = YOU_CAST_A_PREFIX.Length;
            int iStartingPartIndex = nextLine.IndexOf(" ", startLength);
            if (iStartingPartIndex == iLineLength) return;
            string spellName = nextLine.Substring(startLength, iStartingPartIndex - startLength);
            if (!ALL_OFFENSIVE_SPELLS.Contains(spellName)) return;
            iStartingPartIndex++;
            bool failed = false;
            foreach (char c in "spell on ")
            {
                if (iStartingPartIndex == iLineLength)
                {
                    failed = true;
                    continue;
                }
                if (nextLine[iStartingPartIndex++] != c)
                {
                    failed = true;
                    continue;
                }
            }
            if (failed) return;
            int damage = AttackSequence.GetDamage(nextLine, DAMAGE_PREFIX, DAMAGE_SUFFIX);
            if (iStartingPartIndex + DAMAGE_PREFIX.Length + damage.ToString().Length + DAMAGE_SUFFIX.Length == iLineLength)
            {
                return;
            }
            if (damage <= 0)
            {
                return;
            }

            int iExperience;
            MobTypeEnum? mobType;
            bool monsterKilled = AttackSequence.ProcessMonsterKilledMessages(flParams, 1, out iExperience, out mobType);
            flParams.FinishedProcessing = true;
            _onSatisfied(damage, monsterKilled, mobType, iExperience, flParams);
        }
    }

    public class AttackSequence : AOutputProcessingSequence
    {
        private const string YOUR_REGULAR_ATTACK_PREFIX = "Your ";
        private const string YOUR_POWER_ATTACK_PREFIX = "Your power attack ";
        private const string BEFORE_DAMAGE = " hits for ";
        private const string AFTER_DAMAGE = " damage.";

        private const string YOU_GAINED_PREFIX = "You gained";
        private const string WAS_CARRYING_MID = " was carrying: ";
        private const string EXPERIENCE_FOR_THE_DEATH_SUFFIX = " experience for the death of ";

        public Action<bool, int, bool, MobTypeEnum?, int, bool, FeedLineParameters> _onSatisfied;
        public AttackSequence(Action<bool, int, bool, MobTypeEnum?, int, bool, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;

            //skip the "You attack the X" message if present.
            int iIndex = 0;
            string nextLine;
            bool foundLineToProcess = false;
            do
            {
                if (iIndex >= lineCount)
                {
                    return;
                }
                nextLine = Lines[iIndex];
                if (string.IsNullOrEmpty(nextLine) || nextLine.StartsWith("You attack the ") || nextLine == "CRITICAL HIT!")
                {
                    iIndex++;
                    continue;
                }
                foundLineToProcess = true;
            }
            while (!foundLineToProcess);

            bool fumbled = false;
            bool satisfied;
            int damage = 0;
            bool powerAttacked = false;
            if (nextLine == "You missed.")
            {
                satisfied = true;
            }
            else if (nextLine.StartsWith("Your attack has no effect on "))
            {
                satisfied = true;
            }
            else if (nextLine.StartsWith("Your power attack has no effect on "))
            {
                powerAttacked = false; //this does not actually trigger the skill
                satisfied = true;
            }
            else if (nextLine == "You FUMBLED your weapon.")
            {
                satisfied = true;
                fumbled = true;
            }
            else if (nextLine == "You STUMBLE and miss your unarmed attack.")
            {
                satisfied = true;
            }
            else if (MatchesPowerAttackMissPattern(nextLine))
            {
                satisfied = true;
                powerAttacked = true;
            }
            else
            {
                damage = MatchesHitPattern(nextLine, out powerAttacked);
                satisfied = damage > 0;
            }
            if (satisfied)
            {
                bool monsterKilled = false;
                int iExperience = 0;
                MobTypeEnum? eMobType = null;
                if (damage > 0)
                {
                    monsterKilled = ProcessMonsterKilledMessages(flParams, iIndex + 1, out iExperience, out eMobType);
                }
                flParams.FinishedProcessing = true;
                _onSatisfied(fumbled, damage, monsterKilled, eMobType, iExperience, powerAttacked, flParams);
            }
        }

        internal static bool ProcessMonsterKilledMessages(FeedLineParameters flParams, int startLineIndex, out int experience, out MobTypeEnum? monsterType)
        {
            List<string> Lines = flParams.Lines;
            experience = 0;
            monsterType = null;
            int lineCount = Lines.Count;
            if (startLineIndex >= lineCount)
            {
                return false;
            }
            bool monsterKilled = false;
            int i = startLineIndex;
            while (true)
            {
                bool skipToNextLine = true;
                string nextLine = Lines[i];
                if (nextLine == null) continue;
                if (nextLine.StartsWith(YOU_GAINED_PREFIX))
                {
                    int iLineLen = nextLine.Length;
                    int iExpStart = YOU_GAINED_PREFIX.Length;
                    if (iLineLen == iExpStart) continue;

                    int iSpaceIndex = nextLine.IndexOf(EXPERIENCE_FOR_THE_DEATH_SUFFIX, iExpStart);
                    if (iSpaceIndex == iExpStart) continue;

                    int iNextExperience;
                    if (!ParseNumber(nextLine, iSpaceIndex - 1, out iNextExperience, out int _))
                    {
                        continue;
                    }

                    int remainingLength = iLineLen - iSpaceIndex - EXPERIENCE_FOR_THE_DEATH_SUFFIX.Length - 1;
                    if (remainingLength <= 0)
                    {
                        continue;
                    }
                    string remaining = nextLine.Substring(iSpaceIndex + EXPERIENCE_FOR_THE_DEATH_SUFFIX.Length, remainingLength);

                    MobEntity ment = Entity.GetEntity(remaining, EntityTypeFlags.Mob | EntityTypeFlags.Player, flParams.ErrorMessages, null, false) as MobEntity;
                    if (ment != null && ment.MobType.HasValue)
                    {
                        monsterType = ment.MobType.Value;
                    }

                    experience += iNextExperience;
                    monsterKilled = true;
                }
                else
                {
                    int iFoundCarrying = nextLine.IndexOf(WAS_CARRYING_MID);
                    if (iFoundCarrying > 0)
                    {
                        string sPrefix = nextLine.Substring(0, iFoundCarrying + WAS_CARRYING_MID.Length);
                        string itemList = StringProcessing.GetListAsString(Lines, i, sPrefix, true, out i, null);
                        skipToNextLine = false;
                        List<string> itemsString = StringProcessing.ParseList(itemList);
                        List<ItemEntity> oItems = new List<ItemEntity>();
                        RoomTransitionSequence.LoadItems(oItems, itemsString, flParams.ErrorMessages, EntityTypeFlags.Item);
                    }
                }
                if (skipToNextLine)
                {
                    i++;
                }
                if (i >= lineCount)
                {
                    break;
                }
            }
            return monsterKilled;
        }

        public bool MatchesPowerAttackMissPattern(string nextLine)
        {
            return nextLine.StartsWith("Your power attack ") && nextLine.EndsWith(" missed.");
        }

        public int MatchesHitPattern(string nextLine, out bool powerAttacked)
        {
            //validate the prefix to determine it is for an attack or power attack
            powerAttacked = nextLine.StartsWith(YOUR_POWER_ATTACK_PREFIX);
            if (!powerAttacked && !nextLine.StartsWith(YOUR_REGULAR_ATTACK_PREFIX)) return 0;

            //retrieve damage from the end of the text
            int iDamage = GetDamage(nextLine, BEFORE_DAMAGE, AFTER_DAMAGE);
            if (iDamage == 0) return 0;
            if (nextLine.Length == YOUR_REGULAR_ATTACK_PREFIX.Length + BEFORE_DAMAGE.Length + iDamage.ToString().Length + AFTER_DAMAGE.Length) return 0;
            return iDamage;
        }

        /// <summary>
        /// retrieves damage from the end of a string
        /// </summary>
        /// <param name="nextLine">text to process</param>
        /// <param name="beforeDamageText">text before the damage number. assumed to end with a space.</param>
        /// <param name="afterDamageText">text after the damage number</param>
        /// <returns>damage count</returns>
        public static int GetDamage(string nextLine, string beforeDamageText, string afterDamageText)
        {
            int iLineLength = nextLine.Length;
            int iAfterDamageLength = afterDamageText.Length;
            int iAfterDamageIndex = nextLine.LastIndexOf(afterDamageText);
            if (iAfterDamageIndex + iAfterDamageLength != iLineLength) return 0;

            if (!ParseNumber(nextLine, iAfterDamageIndex - 1, out int iDamage, out int iCharacters))
            {
                return 0;
            }
            if (iDamage == 0) return 0;

            int iBeforeDamageIndex = nextLine.LastIndexOf(beforeDamageText);
            int iBeforeDamageLength = beforeDamageText.Length;
            if (iBeforeDamageIndex + iBeforeDamageLength + iCharacters + iAfterDamageLength != iLineLength) return 0;

            return iDamage;
        }

        /// <summary>
        /// parses a number out of text. It is assumed before and after the number are spaces.
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="iEndIndex">the index of the last digit of the number</param>
        /// <param name="number">returns the number</param>
        /// <param name="characterCount">returns the number of characters in the number</param>
        /// <returns>true if the number was parsed, false otherwise</returns>
        private static bool ParseNumber(string input, int iEndIndex, out int number, out int characterCount)
        {
            number = 0;
            int iTens = 1;
            characterCount = 0;
            for (int i = iEndIndex; i >= 0; i--)
            {
                char c = input[i];
                if (char.IsDigit(c))
                {
                    number += int.Parse(c.ToString()) * iTens;
                    iTens *= 10;
                    characterCount++;
                }
                else if (c == ' ')
                {
                    break;
                }
                else
                {
                    return false;
                }
            }
            return characterCount > 0;
        }
    }

    internal class MobStatusSequence : AOutputProcessingSequence
    {
        private Action<MonsterStatus, FeedLineParameters> _onSatisfied;

        public MobStatusSequence(Action<MonsterStatus, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (string.IsNullOrEmpty(flParams.CurrentlyFightingMob))
            {
                return;
            }
            bool firstLine = true;
            MonsterStatus status = MonsterStatus.None;
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
                    if (status != MonsterStatus.None)
                    {
                        flParams.FinishedProcessing = true;
                        flParams.SuppressEcho = !string.IsNullOrEmpty(flParams.CurrentlyFightingMob);
                        _onSatisfied(status, flParams);
                        return;
                    }
                }
            }
        }
    }

    public class PleaseWaitSequence : AOutputProcessingSequence
    {
        private const string PLEASE_WAIT_PREFIX = "Please wait ";
        private const string ENDS_WITH_MINUTES_SUFFIX = " minutes.";
        private const string ENDS_WITH_SECONDS_SUFFIX = " seconds.";
        private Action<int, FeedLineParameters> _onSatisfied;
        private int? _lastMeleeWaitSeconds;
        private int? _lastMagicWaitSeconds;
        private int? _lastPotionsWaitSeconds;

        public void ClearLastMeleeWaitSeconds()
        {
            _lastMeleeWaitSeconds = null;
        }

        public void ClearLastMagicWaitSeconds()
        {
            _lastMagicWaitSeconds = null;
        }

        public void ClearLastPotionsWaitSeconds()
        {
            _lastPotionsWaitSeconds = null;
        }

        public PleaseWaitSequence(Action<int, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (backgroundCommandType.HasValue)
            {
                BackgroundCommandType bctValue = backgroundCommandType.Value;
                foreach (InformationalMessages msg in flParams.InfoMessages)
                {
                    if (msg.MessageType == InformationalMessageType.PleaseWait)
                    {
                        int? lastWaitSeconds = null;
                        int newWaitSeconds = msg.WaitSeconds;
                        if (bctValue == BackgroundCommandType.Stun || bctValue == BackgroundCommandType.OffensiveSpell)
                        {
                            lastWaitSeconds = _lastMagicWaitSeconds;
                        }
                        else if (bctValue == BackgroundCommandType.Attack)
                        {
                            lastWaitSeconds = _lastMeleeWaitSeconds;
                        }
                        else if (bctValue == BackgroundCommandType.DrinkHazy || bctValue == BackgroundCommandType.DrinkNonHazyPotion)
                        {
                            lastWaitSeconds = _lastPotionsWaitSeconds;
                        }
                        if (lastWaitSeconds.HasValue && lastWaitSeconds.Value == newWaitSeconds && flParams.InfoMessages.Count == 1)
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
                        else if (bctValue == BackgroundCommandType.DrinkHazy || bctValue == BackgroundCommandType.DrinkNonHazyPotion)
                        {
                            _lastPotionsWaitSeconds = newWaitSeconds;
                        }
                        flParams.FinishedProcessing = true;
                        _onSatisfied(newWaitSeconds, flParams);
                        break;
                    }
                }
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

        public static int? GetPleaseWaitSeconds(string input)
        {
            int? ret = null;
            if (input.StartsWith(PLEASE_WAIT_PREFIX) && input.Length != PLEASE_WAIT_PREFIX.Length)
            {
                string remainder = input.Substring(PLEASE_WAIT_PREFIX.Length);
                if (remainder == "1 more second.")
                {
                    ret = 1;
                }
                else if (remainder.EndsWith(ENDS_WITH_MINUTES_SUFFIX))
                {
                    if (remainder.Length != ENDS_WITH_MINUTES_SUFFIX.Length)
                    {
                        string rest = remainder.Substring(0, remainder.Length - ENDS_WITH_MINUTES_SUFFIX.Length);
                        ret = ParseMinutesAndSecondsToSeconds(rest);
                    }
                }
                else if (remainder.EndsWith(ENDS_WITH_SECONDS_SUFFIX))
                {
                    if (remainder.Length != ENDS_WITH_SECONDS_SUFFIX.Length)
                    {
                        string rest = remainder.Substring(0, remainder.Length - ENDS_WITH_SECONDS_SUFFIX.Length);
                        if (int.TryParse(rest, out int iSeconds))
                        {
                            ret = iSeconds;
                        }
                    }
                }
            }
            return ret;
        }
    }

    public class SearchSequence : AOutputProcessingSequence
    {
        private const string YOU_DIDNT_FIND_ANYTHING = "You didn't find anything.";
        private const string YOU_FIND_A_HIDDEN_EXIT = "You find a hidden exit: ";
        private Action<List<string>, FeedLineParameters> _onSearchSuccessful;
        private Action<FeedLineParameters> _onSearchUnsuccessful;
        public SearchSequence(Action<List<string>, FeedLineParameters> onSearchSuccessful, Action<FeedLineParameters> onSearchUnsuccessful)
        {
            _onSearchSuccessful = onSearchSuccessful;
            _onSearchUnsuccessful = onSearchUnsuccessful;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0 && Lines.Count <= 3)
            {
                string firstLine = Lines[0];
                string secondLine = Lines.Count > 1 ? Lines[1] : string.Empty;
                string thirdLine = Lines.Count > 2 ? Lines[2] : string.Empty;
                if ((firstLine == YOU_DIDNT_FIND_ANYTHING && string.IsNullOrEmpty(secondLine) && string.IsNullOrEmpty(thirdLine)) ||
                    (string.IsNullOrEmpty(firstLine) && secondLine == YOU_DIDNT_FIND_ANYTHING && string.IsNullOrEmpty(thirdLine)) ||
                    (string.IsNullOrEmpty(firstLine) && string.IsNullOrEmpty(secondLine) && thirdLine == YOU_DIDNT_FIND_ANYTHING))
                {
                    _onSearchUnsuccessful(flParams);
                    flParams.FinishedProcessing = true;
                    return;
                }
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
                _onSearchSuccessful(foundExits, flParams);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class FailMovementSequence : AOutputProcessingSequence
    {
        private const string YOU_FELL_AND_HURT_YOURSELF_PREFIX = "You fell and hurt yourself for ";
        private const string YOU_FELL_AND_HURT_YOURSELF_SUFFIX = " damage.";
        public Action<FeedLineParameters, MovementResult, int> _onSatisfied;

        public FailMovementSequence(Action<FeedLineParameters, MovementResult, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters Parameters)
        {
            int iDamage = 0;
            List<string> Lines = Parameters.Lines;
            if (Lines.Count > 0)
            {
                string firstLine = Lines[0];
                MovementResult? result = null;
                if (firstLine == "You can't go that way.")
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine == "That exit is closed for the night.")
                {
                    result = MovementResult.MapFailure;
                }
                else if (firstLine == "You must fly to get there.")
                {
                    result = MovementResult.MapFailure;
                }
                else if (firstLine == "You must stand up before you may move.")
                {
                    result = MovementResult.StandFailure;
                }
                else if (firstLine == "You have to open it first.")
                {
                    result = MovementResult.ClosedDoorFailure;
                }
                else if (firstLine == "It's locked.")
                {
                    result = MovementResult.LockedDoorFailure;
                }
                else if (firstLine.EndsWith(" blocks your exit."))
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine == "Your gender prevents you from using that exit.")
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine.StartsWith("Only players under level ") && firstLine.EndsWith(" may go that way."))
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine.StartsWith("You must be at least level ") && firstLine.EndsWith(" to go that way."))
                {
                    result = MovementResult.TotalFailure;
                }
                else
                {
                    int iFoundDamage = ProcessFallDamage(firstLine);
                    if (iFoundDamage > 0)
                    {
                        result = MovementResult.FallFailure;
                        iDamage = iFoundDamage;
                    }
                }
                if (result.HasValue)
                {
                    Parameters.FinishedProcessing = true;
                    _onSatisfied(Parameters, result.Value, iDamage);
                }
            }
        }

        public static int ProcessFallDamage(string nextLine)
        {
            return Room.ProcessTrapDamage(YOU_FELL_AND_HURT_YOURSELF_PREFIX, YOU_FELL_AND_HURT_YOURSELF_SUFFIX, nextLine);
        }
    }

    public class InformationalMessagesSequence : AOutputProcessingSequence
    {
        public const string CELDUIN_EXPRESS_IN_BREE_MESSAGE = "### The Celduin Express is ready for boarding in Bree.";

        public Action<FeedLineParameters, List<string>, List<string>, List<string>> _onSatisfied;
        private string _userLoginPrefix;
        private string _deathPrefixMessage;

        public InformationalMessagesSequence(string userName, Action<FeedLineParameters, List<string>, List<string>, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
            _userLoginPrefix = "### " + userName + " the ";
            _deathPrefixMessage = "### Sadly, " + userName + " ";
        }

        public override void FeedLine(FeedLineParameters Parameters)
        {
            List<string> broadcastMessages = null;
            List<string> Lines = Parameters.Lines;
            List<string> addedPlayers = null;
            List<string> removedPlayers = null;
            List<int> linesToRemove = null;
            bool haveDataToDisplay = false;

            //process the initial user login. This happenens first because of a blank line before the message, so it needs to be
            //handled beforehand because the later logic stops at a blank line.
            int iStartIndex = 0;
            for (int i = 0; i < Lines.Count; i++)
            {
                string sNextLine = Lines[i];
                if (sNextLine.StartsWith(_userLoginPrefix) && sNextLine.EndsWith(" just logged in."))
                {
                    Parameters.InfoMessages.Add(new InformationalMessages(InformationalMessageType.InitialLogin));
                    iStartIndex = i + 1;
                    haveDataToDisplay = true;
                    break;
                }
            }

            for (int i = iStartIndex; i < Lines.Count; i++)
            {
                bool isMessageToKeep = false;
                InformationalMessageType? im = null;
                InformationalMessages nextMsg = null;
                string sLine = Lines[i];
                int lineLength = sLine.Length;
                bool isBroadcast = false;
                if (string.IsNullOrWhiteSpace(sLine))
                {
                    Parameters.NextLineIndex = i;
                    break;
                }
                else if (sLine == "The sun rises.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.DayStart;
                }
                else if (sLine == "The sun disappears over the horizon.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.NightStart;
                }
                else if (sLine == "You feel less holy.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.BlessOver;
                }
                else if (sLine == "You feel less protected.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.ProtectionOver;
                }
                else if (sLine == "You can no longer fly.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FlyOver;
                }
                else if (sLine == "Your feet hit the ground.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.LevitationOver;
                }
                else if (sLine == "Your invisibility fades.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.InvisibilityOver;
                }
                else if (sLine == "Your detect-invis wears off.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.DetectInvisibleOver;
                }
                else if (sLine == "You no longer endure fire.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.EndureFireOver;
                }
                else if (sLine == "You no longer endure earth.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.EndureEarthOver;
                }
                else if (sLine == "You no longer endure water.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.EndureWaterOver;
                }
                else if (sLine == "You no longer endure cold.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.EndureColdOver;
                }
                else if (sLine == "Your manashield dissipates.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.ManashieldOff;
                }
                else if (sLine == CELDUIN_EXPRESS_IN_BREE_MESSAGE)
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.CelduinExpressInBree;
                }
                else if (sLine == "The Celduin Express has departed for Mithlond.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.CelduinExpressLeftBree;
                }
                else if (sLine == "The Celduin Express has departed for Bree.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.CelduinExpressLeftMithlond;
                }
                else if (sLine == "The Bullroarer has arrived in Mithlond.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.BullroarerInMithlond;
                }
                else if (sLine == "The Bullroarer has arrived in Nindamos.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.BullroarerInNindamos;
                }
                else if (sLine == "The Bullroarer is now ready for boarding.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.BullroarerReadyForBoarding;
                }
                else if (sLine == "The Harbringer has set sail.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.HarbringerSailed;
                }
                else if (sLine == "The Harbringer is ready for boarding.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.HarbringerInPort;
                }
                else if (sLine == "The searing heat burns your flesh.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FireDamage;
                }
                else if (sLine == "Water fills your lungs.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.WaterDamage;
                }
                else if (sLine == "The earth swells up around you and smothers you.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.EarthDamage;
                }
                else if (sLine == "The freezing air chills you to the bone.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.WindDamage;
                }
                else if (sLine == "The toxic air poisoned you.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.RoomPoisoned;
                }
                else if (sLine == "Poison courses through your veins.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.PoisonDamage;
                }
                else if (sLine == "You run like a chicken.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.Flee;
                }
                else if (sLine == "You are thrown back by an invisible force.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FleeFailed;
                }
                else if (sLine == "You phase in and out of existence.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.WordOfRecall;
                }
                else if (sLine == "You receive a vampyric touch!")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.ReceiveVampyricTouch;
                }
                else if (sLine.StartsWith(_deathPrefixMessage))
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.Death;
                }
                else if (sLine == "The air is still and quiet." ||
                         sLine == "Light clouds appear over the mountains." ||
                         sLine == "A light breeze blows from the south." ||
                         sLine == "Clear, blue skies cover the land." ||
                         sLine == "It's a beautiful day today." ||
                         sLine == "The sun shines brightly across the land." ||
                         sLine == "The glaring sun beats down upon the inhabitants of the world." ||
                         sLine == "The heat today is unbearable." ||

                         sLine == "The earth trembles under your feet." ||

                         sLine == "The sky is dark as pitch." ||
                         sLine == "The full moon shines across the land." ||
                         sLine == "The night sky is lit by the waxing moon." ||
                         sLine == "Half a moon lights the evening skies." ||
                         sLine == "A sliver of silver can be seen in the night sky." ||

                         sLine == "Thunderheads roll in from the east." ||
                         sLine == "A heavy fog blankets the earth." ||
                         sLine == "A light rain falls quietly." ||
                         sLine == "Sheets of rain pour down from the skies." ||
                         sLine == "A torrent soaks the ground." ||
                         sLine == "A heavy rain begins to fall." ||

                         sLine == "A strong wind blows across the land." ||
                         sLine == "The wind gusts, blowing debris through the streets." ||
                         sLine == "Gale force winds blow in from the sea." ||

                         sLine == "Player saved.")
                {
                    //These lines are ignored
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
                else if (sLine.EndsWith(" killed you.")) //skip this message and use the broadcast to trigger death logic
                {
                    isMessageToKeep = true;
                    haveDataToDisplay = true;
                }
                else if (sLine.StartsWith("Scared of going "))
                {
                    isMessageToKeep = true;
                    haveDataToDisplay = true;
                }
                else
                {
                    bool isArrived = false;
                    bool isLeft = false;
                    string sWhat = null;
                    bool expectCapitalized = true;
                    if (sLine.EndsWith(" just arrived.") && lineLength != " just arrived.".Length)
                    {
                        isArrived = true;
                        sWhat = sLine.Substring(0, lineLength - " just arrived.".Length);
                    }
                    else if (sLine.EndsWith(" just wandered away.") && lineLength != " just wandered away.".Length)
                    {
                        isLeft = true;
                        sWhat = sLine.Substring(0, lineLength - " just wandered away.".Length);
                    }
                    else
                    {
                        int iJustWanderedIndex = sLine.IndexOf(" just wandered to the ");
                        if (iJustWanderedIndex > 0)
                        {
                            sWhat = sLine.Substring(0, iJustWanderedIndex);
                            isLeft = true;
                        }

                        if (!isLeft)
                        {
                            int iKilledThe = sLine.IndexOf(" killed ");
                            if (iKilledThe > 0)
                            {
                                int targetLength = lineLength - iKilledThe - " killed ".Length - 1;
                                if (targetLength > 0)
                                {
                                    sWhat = sLine.Substring(iKilledThe + " killed ".Length, targetLength);
                                    isLeft = true;
                                    expectCapitalized = false;
                                }
                            }
                        }
                    }
                    if (isArrived || isLeft)
                    {
                        Entity e = Entity.GetEntity(sWhat, EntityTypeFlags.Mob | EntityTypeFlags.Player, Parameters.ErrorMessages, null, expectCapitalized);
                        if (RoomTransitionSequence.CheckForValidMob(sWhat, e, Parameters.ErrorMessages, EntityTypeFlags.Mob | EntityTypeFlags.Player))
                        {
                            MobEntity ment = (MobEntity)e;
                            if (ment.MobType.HasValue)
                            {
                                nextMsg = new InformationalMessages(isArrived ? InformationalMessageType.MobArrived : InformationalMessageType.MobWanderedAway);
                                nextMsg.Mob = ment.MobType.Value;
                                nextMsg.MobCount = ment.Count;
                            }
                        }
                    }

                    if (nextMsg == null)
                    {
                        int iDamage = FailMovementSequence.ProcessFallDamage(sLine);
                        if (iDamage > 0)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.FallDamage);
                            nextMsg.Damage = iDamage;
                        }
                    }

                    if (nextMsg == null)
                    {
                        int? iDamage2 = CheckIfEnemyAttacksYou(sLine);
                        if (iDamage2.HasValue)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.EnemyAttacksYou);
                            nextMsg.Damage = iDamage2.Value;
                        }
                    }

                    if (nextMsg == null)
                    {
                        int? pleaseWaitSeconds = PleaseWaitSequence.GetPleaseWaitSeconds(sLine);
                        if (pleaseWaitSeconds.HasValue)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.PleaseWait);
                            nextMsg.WaitSeconds = pleaseWaitSeconds.Value;
                        }
                    }

                    if (nextMsg == null) //these are valid informational messages that aren't currently processed
                    {
                        if (sLine.Contains(" misses ") ||
                            sLine.Contains(" barely nicks ") ||
                            sLine.Contains(" scratches ") ||
                            sLine.Contains(" bruises ") ||
                            sLine.Contains(" hurts ") ||
                            sLine.Contains(" wounds ") ||
                            sLine.Contains(" smites ") ||
                            sLine.Contains(" maims ") ||
                            sLine.Contains(" pulverizes ") ||
                            sLine.Contains(" devestates ") ||
                            sLine.Contains(" circles "))
                        {
                            haveDataToDisplay = true;
                            continue;
                        }
                    }

                    haveDataToDisplay = true;
                    if (nextMsg == null) //not an informational message
                    {
                        break;
                    }
                }

                bool removeLine = false;
                bool addAsBroadcastMessage = false;
                if (isBroadcast)
                {
                    addAsBroadcastMessage = true;
                    removeLine = true;
                }
                else if (im.HasValue || nextMsg != null)
                {
                    if (nextMsg == null)
                    {
                        nextMsg = new InformationalMessages(im.Value);
                    }
                    Parameters.InfoMessages.Add(nextMsg);
                }
                else if (!isMessageToKeep)
                {
                    removeLine = true;
                }
                if (addAsBroadcastMessage)
                {
                    if (broadcastMessages == null)
                        broadcastMessages = new List<string>();
                    broadcastMessages.Add(sLine);
                }
                if (removeLine)
                {
                    if (linesToRemove == null)
                        linesToRemove = new List<int>();
                    linesToRemove.Add(i);
                }
            }

            //remove from output lines that shouldn't display
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

            if (Parameters.InfoMessages.Count > 0 || broadcastMessages != null)
            {
                _onSatisfied(Parameters, broadcastMessages, addedPlayers, removedPlayers);
            }
        }

        private const string DAMAGE_END_STRING = " damage!";
        private const string YOU_FOR_STRING = " you for ";
        public static int? CheckIfEnemyAttacksYou(string firstLine)
        {
            if (firstLine.EndsWith(" missed you."))
            {
                return 0;
            }
            else if (firstLine.EndsWith(DAMAGE_END_STRING))
            {
                int iDamage = AttackSequence.GetDamage(firstLine, YOU_FOR_STRING, DAMAGE_END_STRING);
                if (iDamage <= 0)
                {
                    return null;
                }

                int len = firstLine.Length;
                int backlen = YOU_FOR_STRING.Length + DAMAGE_END_STRING.Length + iDamage.ToString().Length;
                if (len == backlen) return null;

                string remainder = firstLine.Substring(0, len - backlen);
                bool matches;
                if (remainder.EndsWith(" barely nicks"))
                {
                    matches = iDamage >= 1 && iDamage <= 2;
                }
                else if (remainder.EndsWith(" scratches"))
                {
                    matches = iDamage >= 3 && iDamage <= 5;
                }
                else if (remainder.EndsWith(" bruises"))
                {
                    matches = iDamage >= 6 && iDamage <= 9;
                }
                else if (remainder.EndsWith(" hurts"))
                {
                    matches = iDamage >= 10 && iDamage <= 12;
                }
                else if (remainder.EndsWith(" wounds"))
                {
                    matches = iDamage >= 13 && iDamage <= 15;
                }
                else if (remainder.EndsWith(" smites"))
                {
                    matches = true; //CSRTODO: 16-20
                }
                else if (remainder.EndsWith(" maims"))
                {
                    matches = true; //CSRTODO: 22-23
                }
                else if (remainder.EndsWith(" pulverizes"))
                {
                    matches = true; //CSRTODO: 26-27
                }
                else if (remainder.EndsWith(" devestates"))
                {
                    matches = true; //CSRTODO: 33
                }
                else if (remainder.EndsWith(" spell on"))
                {
                    matches = true;
                }
                else
                {
                    matches = false;
                }
                if (matches)
                {
                    return iDamage;
                }
            }
            return null;
        }
    }

    public class EntityAttacksYouSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters> _onSatisfied;

        public EntityAttacksYouSequence(Action<FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            foreach (InformationalMessages nextMessage in flParams.InfoMessages)
            {
                if (nextMessage.MessageType == InformationalMessageType.EnemyAttacksYou)
                {
                    _onSatisfied(flParams);
                    break;
                }
            }
        }
    }

    public class SelfSpellCastSequence : AOutputProcessingSequence
    {
        public static Dictionary<string, string> ACTIVE_SPELL_TO_ACTIVE_TEXT = new Dictionary<string, string>()
        {
            { "You feel holy.", "bless" },
            { "You feel watched.", "protection" },
            { "You can fly!", "fly"},
            { "Your eyes feel funny.", "detect-magic" },
            { "You turn invisible.", "invisibility" },
            { "You become shielded from the normal fire element.", "endure-fire" },
            { "You become shielded from the normal wind element.", "endure-cold" },
            { "You become shielded from the normal earth element.", "endure-earth" },
            { "You become shielded from the normal water element.", "endure-water" },
            { "You begin to float.", "levitation" },
            { "Your eyes tingle.", "detect-invisible"}, //comes with "You start to feel real strange, as if connected to another dimension."
        };

        public Action<FeedLineParameters, BackgroundCommandType?, string> _onSatisfied;
        public SelfSpellCastSequence(Action<FeedLineParameters, BackgroundCommandType?, string> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            BackgroundCommandType? matchingSpell = null;
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            string activeSpell = null;
            if (lineCount > 0 && lineCount <= 3)
            {
                string firstLine = Lines[0];
                string secondLine = lineCount >= 2 ? Lines[1] : string.Empty;
                string thirdLine = lineCount >= 3 ? Lines[2] : string.Empty;
                if (firstLine == "Vigor spell cast." && string.IsNullOrEmpty(secondLine) && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.Vigor;
                else if (firstLine == "Mend-wounds spell cast." && string.IsNullOrEmpty(secondLine) && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.MendWounds;
                else if (firstLine == "Curepoison spell cast on yourself." && secondLine == "You feel much better." && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.CurePoison;
                else if (firstLine.EndsWith(" spell cast."))
                {
                    if (ACTIVE_SPELL_TO_ACTIVE_TEXT.TryGetValue(secondLine, out activeSpell))
                    {
                        if (activeSpell == "bless")
                        {
                            matchingSpell = BackgroundCommandType.Bless;
                        }
                        else if (activeSpell == "protection")
                        {
                            matchingSpell = BackgroundCommandType.Protection;
                        }
                    }
                }
            }
            if (matchingSpell.HasValue || !string.IsNullOrEmpty(activeSpell))
            {
                _onSatisfied(flParams, matchingSpell.Value, activeSpell);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public static class StringProcessing
    {
        /// <summary>
        /// retrieves a list. The list is assumed to be comma-delimited ending in a period.
        /// </summary>
        /// <param name="inputs">text input information</param>
        /// <param name="lineIndex">line index to start at</param>
        /// <param name="startsWith">beginning text for the list</param>
        /// <param name="requireExactStartsWith">if true it will skip lines that don't match the start index.</param>
        /// <param name="nextLineIndex">returns the line after the final line containing the list.</param>
        /// <returns></returns>
        public static string GetListAsString(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex, string stopAtPrefix)
        {
            nextLineIndex = lineIndex;
            bool foundStartsWith = false;
            bool finished = false;
            StringBuilder sb = null;
            string sNextLine = null;
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
            }
            if (!finished)
            {
                bool isFirstLine = true;
                bool stopProcessing = false;
                string previousLine = null;
                while (true)
                {
                    if (isFirstLine)
                    {
                        sNextLine = inputs[lineIndex];
                        if (sNextLine == startsWith)
                        {
                            return null;
                        }
                        sNextLine = sNextLine.Substring(startsWith.Length);
                        isFirstLine = false;
                    }
                    else
                    {
                        lineIndex++;
                        if (lineIndex >= inputs.Count)
                        {
                            stopProcessing = true;
                        }
                        else
                        {
                            sNextLine = inputs[lineIndex];
                        }
                        if (!stopProcessing)
                        {
                            //line continuations start with two spaces. There is also a check for the next prefix to stop at,
                            //which shouldn't be needed anymore with the line continuation logic, but is left in anyway.
                            stopProcessing = !sNextLine.StartsWith("  ") || (!string.IsNullOrEmpty(stopAtPrefix) && sNextLine.StartsWith(stopAtPrefix));
                        }
                    }
                    if (!stopProcessing)
                    {
                        stopProcessing = string.IsNullOrEmpty(sNextLine);
                    }
                    if (stopProcessing)
                    {
                        nextLineIndex = lineIndex;
                        previousLine = previousLine.Trim();
                        if (!previousLine.EndsWith("."))
                        {
                            return null;
                        }
                        else if (previousLine != ".")
                        {
                            sb.AppendLine(previousLine.Substring(0, previousLine.Length - 1));
                        }
                        break;
                    }
                    else
                    {
                        sb.AppendLine(previousLine);
                    }
                    previousLine = sNextLine;
                }
            }
            string ret = sb.ToString().Replace(Environment.NewLine, " ");
            while (ret.Contains("  "))
            {
                ret = ret.Replace("  ", " ");
            }
            return ret;
        }

        public static List<string> ParseList(string sFullContent)
        {
            List<string> ret = new List<string>();
            if (sFullContent != null)
            {
                string[] allEntries = sFullContent.Split(new char[] { ',' });
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
            }
            return ret;
        }

        public static List<string> GetList(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex, string stopAtPrefix)
        {
            string sFullContent = GetListAsString(inputs, lineIndex, startsWith, requireExactStartsWith, out nextLineIndex, stopAtPrefix);
            return ParseList(sFullContent);
        }

        /// <summary>
        /// picks words contained in a list of words
        /// </summary>
        /// <param name="input">input words</param>
        /// <returns>words, starting with the longest</returns>
        public static IEnumerable<string> PickWords(string input)
        {
            string sBestWord = string.Empty;
            List<string> sWords = new List<string>(input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            sWords.Sort((a, b) =>
            {
                return b.Length.CompareTo(a.Length);
            });
            foreach (string sNextWord in sWords)
            {
                yield return sNextWord;
            }
        }
    }
}

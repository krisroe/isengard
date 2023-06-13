﻿using System;
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
        public virtual bool IsActive()
        {
            return true;
        }

        public abstract void FeedLine(FeedLineParameters Parameters);
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
        public Action<FeedLineParameters, int, int, int, int, List<SkillCooldown>, List<string>, bool> _onSatisfied;
        private const string SKILLS_PREFIX = "Skills: ";
        private const string SPELLS_PREFIX = "Spells cast: ";
        private const string TO_NEXT_LEVEL_PREFIX = "To Next Level:";

        private string _username;
        public ScoreOutputSequence(string username, Action<FeedLineParameters, int, int, int, int, List<SkillCooldown>, List<string>, bool> onSatisfied)
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
                iIndex = sNextLine.IndexOf(TO_NEXT_LEVEL_PREFIX);
                if (iIndex < 0) return;
                iIndex += TO_NEXT_LEVEL_PREFIX.Length;
                if (iIndex + 10 >= sNextLine.Length) return;
                string sTNL = sNextLine.Substring(iIndex, 10).Trim();
                if (!int.TryParse(sTNL, out int iTNL))
                {
                    return;
                }

                List<string> skillsRaw = StringProcessing.GetList(Lines, iNextLineIndex, SKILLS_PREFIX, true, out iNextLineIndex);
                if (skillsRaw == null)
                {
                    return;
                }

                List<string> spellsRaw = StringProcessing.GetList(Lines, iNextLineIndex, SPELLS_PREFIX, false, out iNextLineIndex);
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

                _onSatisfied(flParams, iLevel, iTotalHP, iTotalMP, iTNL, cooldowns, spells, poisoned);
                flParams.FinishedProcessing = true;
            }
        }
    }

    public class RemoveEquipmentSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters> _onSatisfied;
        public RemoveEquipmentSequence(Action<FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
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
        public FeedLineParameters FeedLineParameters { get; set; }
        public List<PlayerEntity> Players { get; set; }
        public List<ItemEntity> Items { get; set; }
        public List<MobEntity> Mobs { get; set; }
        public List<string> ErrorMessages { get; set; }
    }

    internal class InitialLoginSequence : AOutputProcessingSequence
    {
        public Action<InitialLoginInfo> _onSatisfied;
        public bool Active { get; set; }

        public InitialLoginSequence(Action<InitialLoginInfo> onSatisfied)
        {
            _onSatisfied = onSatisfied;
            this.Active = true;
        }

        public override bool IsActive()
        {
            return this.Active;
        }

        public override void FeedLine(FeedLineParameters flParams)
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

    public class RoomTransitionSequence : AOutputProcessingSequence
    {
        private string _userName;
        private Action<RoomTransitionInfo, int, TrapType> _onSatisfied;
        public RoomTransitionSequence(Action<RoomTransitionInfo, int, TrapType> onSatisfied, string userName)
        {
            _onSatisfied = onSatisfied;
            _userName = userName;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            RoomTransitionType rtType = RoomTransitionType.Move;
            int nextLineIndex = 0;
            int iDamage = 0;
            TrapType eTrapType = TrapType.None;
            while (true)
            {
                string nextLine = Lines[nextLineIndex];
                if (string.IsNullOrEmpty(nextLine))
                {
                    break;
                }
                else if (nextLine.StartsWith("Scared of going "))
                {
                    //skipped
                }
                else
                {
                    int iFoundDamage = FailMovementSequence.ProcessFallDamage(nextLine);
                    if (iFoundDamage > 0) //skip but process the damage
                    {
                        eTrapType = eTrapType | TrapType.Fall;
                        iDamage += iFoundDamage;
                    }
                    else //something else, so skip to the following logic
                    {
                        break;
                    }
                }
                nextLineIndex++;
                if (nextLineIndex == lineCount) //reached the end of the input
                {
                    return;
                }
            }

            string sNextLine = Lines[nextLineIndex];
            if (Lines[nextLineIndex] == "You run like a chicken.")
            {
                rtType = RoomTransitionType.Flee;
                nextLineIndex++;

                //check if didn't actually flee
                if (Lines[nextLineIndex] == "You are thrown back by an invisible force.")
                {
                    return;
                }
            }
            else if (sNextLine == "You phase in and out of existence.")
            {
                rtType = RoomTransitionType.Hazy;
                nextLineIndex++;
            }
            else if (sNextLine.StartsWith("### Sadly, " + _userName + " "))
            {
                rtType = RoomTransitionType.Death;
                nextLineIndex++;
            }

            //blank line before room name
            if (!string.IsNullOrEmpty(Lines[nextLineIndex])) return;

            if (ProcessRoom(Lines, nextLineIndex, rtType, flParams, _onSatisfied, iDamage, ref eTrapType))
            {
                flParams.FinishedProcessing = true;
            }
        }

        /// <summary>
        /// processes a room name as
        /// [blank line]
        /// Room name (non blank)
        /// [blank line]
        /// </summary>
        /// <param name="Lines">list of output lines</param>
        /// <param name="nextLineIndex">starting index for the room</param>
        /// <param name="sRoomName">returns the room name</param>
        /// <returns>true if the room name was successfully processed, false otherwise</returns>
        internal static bool ProcessRoomName(List<string> Lines, ref int nextLineIndex, out string sRoomName)
        {
            sRoomName = string.Empty;

            if (!string.IsNullOrEmpty(Lines[nextLineIndex])) return false;
            nextLineIndex++;

            if (nextLineIndex >= Lines.Count) return false;
            sRoomName = (Lines[nextLineIndex] ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(sRoomName)) return false;
            nextLineIndex++;

            //blank line after room name
            if (Lines[nextLineIndex] != string.Empty) return false;
            nextLineIndex++;

            return true;
        }

        internal static InitialLoginInfo ProcessRoomForInitialization(List<string> Lines, int nextLineIndex)
        {
            if (!ProcessRoomName(Lines, ref nextLineIndex, out string sRoomName))
            {
                return null;
            }

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

        public static bool ProcessRoom(string sRoomName, string exitsList, string list1, string list2, string list3, Action<RoomTransitionInfo, int, TrapType> onSatisfied, FeedLineParameters flParams, RoomTransitionType rtType, int damage, TrapType trapType)
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
                LoadItems(items, roomList3, errorMessages, EntityTypeFlags.Item);
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
                        LoadMobs(mobs, roomList2, errorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList2, errorMessages, EntityTypeFlags.Item);
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
                        LoadPlayers(players, roomList1, errorMessages, playerNames, EntityTypeFlags.Player);
                    }
                    else if (foundTypeValue == EntityType.Mob)
                    {
                        LoadMobs(mobs, roomList1, errorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadItems(items, roomList1, errorMessages, EntityTypeFlags.Item);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
                else if (canBePlayers) //presumably players
                {
                    LoadPlayers(players, roomList1, errorMessages, playerNames, possibleTypes);
                }
                else
                {
                    possibleTypes &= ~EntityTypeFlags.Player;
                    if (mobs.Count == 0)
                    {
                        LoadMobs(mobs, roomList1, errorMessages, possibleTypes);
                    }
                    else
                    {
                        LoadItems(items, roomList1, errorMessages, possibleTypes);
                    }
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
            onSatisfied(rti, damage, trapType);
            return true;
        }

        internal static bool ProcessRoom(List<string> Lines, int nextLineIndex, RoomTransitionType rtType, FeedLineParameters flParams, Action<RoomTransitionInfo, int, TrapType> onSatisfied, int damage, ref TrapType trapType)
        {
            int lineCount = Lines.Count;
            if (!ProcessRoomName(Lines, ref nextLineIndex, out string sRoomName))
            {
                return false;
            }

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
                        else
                        {
                            int trapDamage = Room.ProcessTrapDamage("You lost ", " hit points.", sNextLine);
                            if (trapDamage > 0) damage += trapDamage;
                        }
                    }
                }
            }

            return ProcessRoom(sRoomName, exitsString, list1String, list2String, list3String, onSatisfied, flParams, rtType, damage, trapType);
        }

        private static void LoadItems(List<ItemEntity> items, List<string> itemNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in itemNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null);
                if (e != null)
                {
                    if (e is ItemEntity)
                    {
                        items.Add((ItemEntity)e);
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
            }
        }
        private static void LoadMobs(List<MobEntity> mobs, List<string> mobNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in mobNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null);
                if (e != null)
                {
                    if (e is MobEntity)
                    {
                        mobs.Add((MobEntity)e);
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
            }
        }

        private static void LoadPlayers(List<PlayerEntity> players, List<string> currentPlayerNames, List<string> errorMessages, HashSet<string> allPlayerNames, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in currentPlayerNames)
            {
                PlayerEntity pEntity = (PlayerEntity)Entity.GetEntity(next, possibleEntityTypes, errorMessages, allPlayerNames);
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

        public Action<int, bool, int, FeedLineParameters> _onSatisfied;
        public CastOffensiveSpellSequence(Action<int, bool, int, FeedLineParameters> onSatisfied)
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
            bool monsterKilled = AttackSequence.ProcessMonsterKilledMessages(Lines, 1, out iExperience);
            flParams.FinishedProcessing = true;
            _onSatisfied(damage, monsterKilled, iExperience, flParams);
        }
    }

    public class AttackSequence : AOutputProcessingSequence
    {
        private const string YOUR_REGULAR_ATTACK_PREFIX = "Your ";
        private const string YOUR_POWER_ATTACK_PREFIX = "Your power attack ";
        private const string BEFORE_DAMAGE = " hits for ";
        private const string AFTER_DAMAGE = " damage.";

        private const string YOU_GAINED_PREFIX = "You gained";
        private const string EXPERIENCE_FOR_THE_DEATH_SUFFIX = " experience for the death of ";

        public Action<bool, int, bool, int, bool, FeedLineParameters> _onSatisfied;
        public AttackSequence(Action<bool, int, bool, int, bool, FeedLineParameters> onSatisfied)
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
                if (damage > 0)
                {
                    monsterKilled = ProcessMonsterKilledMessages(Lines, iIndex + 1, out iExperience);
                }
                flParams.FinishedProcessing = true;
                _onSatisfied(fumbled, damage, monsterKilled, iExperience, powerAttacked, flParams);
            }
        }

        internal static bool ProcessMonsterKilledMessages(List<string> Lines, int startLineIndex, out int experience)
        {
            experience = 0;
            int lineCount = Lines.Count;
            if (startLineIndex >= lineCount)
            {
                return false;
            }
            bool monsterKilled = false;
            for (int i = startLineIndex; i < lineCount; i++)
            {
                string nextLine = Lines[i];
                if (nextLine == null) continue;
                if (!nextLine.StartsWith(YOU_GAINED_PREFIX)) continue;

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

                experience += iNextExperience;
                monsterKilled = true;
                break;
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

        public override void FeedLine(FeedLineParameters flParams)
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

    public class SuccessfulSearchSequence : AOutputProcessingSequence
    {
        private const string YOU_FIND_A_HIDDEN_EXIT = "You find a hidden exit: ";
        private Action<List<string>, FeedLineParameters> _onSatisfied;
        public SuccessfulSearchSequence(Action<List<string>, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
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
                else if (firstLine.EndsWith(" blocks your exit."))
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine.StartsWith("Only players under level ") && firstLine.EndsWith(" may go that way."))
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
        public Action<List<InformationalMessages>, List<string>, List<string>, List<string>> _onSatisfied;

        public InformationalMessagesSequence(Action<List<InformationalMessages>, List<string>, List<string>, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters Parameters)
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
                bool isMessageToKeep = false;
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
                else if (sLine == "You feel less holy.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.BlessOver;
                }
                else if (sLine == "You feel less protected.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.ProtectionOver;
                }
                else if (sLine == "You can no longer fly.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.FlyOver;
                }
                else if (sLine == "Your manashield dissipates.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessages.ManashieldOff;
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

                         sLine == "Player saved." ||
                         sLine == "### The Celduin Express is ready for boarding in Bree.")
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
                else if (sLine == "The searing heat burns your flesh." ||
                         sLine == "Water fills your lungs." ||
                         sLine == "The earth swells up around you and smothers you." ||
                         sLine == "Poison courses through your veins." ||
                         sLine.EndsWith(" just arrived.") ||
                         sLine.EndsWith(" just wandered away.") ||
                         sLine.EndsWith(" circles you."))
                {
                    isMessageToKeep = true;
                    haveDataToDisplay = true;
                }
                else if (!string.IsNullOrWhiteSpace(sLine)) //not an informational message
                {
                    haveDataToDisplay = true;
                    break;
                }
                else //whitespace
                {
                    whitespaceLine = true;
                }
                if (!whitespaceLine)
                {
                    bool removeLine = false;
                    bool addAsBroadcastMessage = false;
                    if (isBroadcast)
                    {
                        addAsBroadcastMessage = true;
                        removeLine = true;
                    }
                    else if (im.HasValue)
                    {
                        InformationalMessages imVal = im.Value;
                        if (messages == null)
                            messages = new List<InformationalMessages>();
                        messages.Add(imVal);
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

    public class EntityAttacksYouSequence : AOutputProcessingSequence
    {
        private const string DAMAGE_END_STRING = " damage!";
        private const string YOU_FOR_STRING = " you for ";
        public Action<FeedLineParameters> _onSatisfied;

        public EntityAttacksYouSequence(Action<FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        public override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0)
            {
                string firstLine = Lines[0];
                bool matches = false;
                if (firstLine.EndsWith(" missed you."))
                {
                    matches = true;
                }
                else if (firstLine.EndsWith(DAMAGE_END_STRING))
                {
                    int iDamage = AttackSequence.GetDamage(firstLine, YOU_FOR_STRING, DAMAGE_END_STRING);
                    if (iDamage <= 0)
                    {
                        return;
                    }

                    int len = firstLine.Length;
                    int backlen = YOU_FOR_STRING.Length + DAMAGE_END_STRING.Length + iDamage.ToString().Length;
                    if (len == backlen) return;

                    string remainder = firstLine.Substring(0, len - backlen);
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
                        matches = true; //CSRTODO: 10-11
                    }
                    else if (remainder.EndsWith(" wounds"))
                    {
                        matches = true; //CSRTODO: 13-15
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
                        matches = true; //CSRTODO: 26
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
                }
                if (matches)
                {
                    flParams.FinishedProcessing = true;
                    _onSatisfied(flParams);
                }
            }
        }
    }

    public class LifeSpellCastSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters, BackgroundCommandType> _onSatisfied;
        public LifeSpellCastSequence(Action<FeedLineParameters, BackgroundCommandType> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public override void FeedLine(FeedLineParameters flParams)
        {
            BackgroundCommandType? matchingSpell = null;
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            if (lineCount > 0 && lineCount <= 3)
            {
                string firstLine = Lines[0];
                string secondLine = lineCount >= 2 ? Lines[1] : string.Empty;
                string thirdLine = lineCount >= 3 ? Lines[2] : string.Empty;
                if (firstLine == "Vigor spell cast." && string.IsNullOrEmpty(secondLine) && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.Vigor;
                else if (firstLine == "Mend-wounds spell cast." && string.IsNullOrEmpty(secondLine) && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.MendWounds;
                else if (firstLine == "Bless spell cast." && secondLine == "You feel holy." && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.Bless;
                else if (firstLine == "Protection spell cast." && secondLine == "You feel watched." && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.Protection;
                else if (firstLine == "Curepoison spell cast on yourself." && secondLine == "You feel much better." && string.IsNullOrEmpty(thirdLine))
                    matchingSpell = BackgroundCommandType.CurePoison;
            }
            if (matchingSpell.HasValue)
            {
                _onSatisfied(flParams, matchingSpell.Value);
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
        public static string GetListAsString(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex)
        {
            nextLineIndex = lineIndex;
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

        public static List<string> GetList(List<string> inputs, int lineIndex, string startsWith, bool requireExactStartsWith, out int nextLineIndex)
        {
            string sFullContent = GetListAsString(inputs, lineIndex, startsWith, requireExactStartsWith, out nextLineIndex);
            return ParseList(sFullContent);
        }
    }
}

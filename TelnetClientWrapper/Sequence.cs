using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
namespace IsengardClient
{
    internal interface IOutputItemSequence
    {
        OutputItemInfo FeedByte(int nextByte);
    }

    internal class OutputItemInfo
    {
        public OutputItemSequenceType SequenceType { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
    }

    internal abstract class AOutputProcessingSequence
    {
        internal abstract void FeedLine(FeedLineParameters Parameters);
    }

    internal class FeedLineParameters
    {
        public FeedLineParameters(List<string> Lines)
        {
            this.Lines = Lines;
            this.InfoMessages = new List<InformationalMessages>();
            this.ErrorMessages = new List<string>();
        }
        public List<string> Lines { get; set; }
        public BackgroundCommandType? BackgroundCommandType { get; set; }
        public bool IsFightingMob { get; set; }
        public bool FinishedProcessing { get; set; }
        public bool SuppressEcho { get; private set; }
        public void SetSuppressEcho(bool suppressEcho)
        {
            if (ConsoleVerbosity != ConsoleOutputVerbosity.Maximum)
            {
                SuppressEcho = suppressEcho;
            }
        }
        /// <summary>
        /// general result type common to all background commands
        /// </summary>
        public CommandResult? CommandResult { get; set; }
        /// <summary>
        /// result specific to each background command. Positive values are specific to the command.
        /// </summary>
        public int CommandSpecificResult { get; set; }
        public HashSet<string> PlayerNames { get; set; }
        public int NextLineIndex { get; set; }
        public List<InformationalMessages> InfoMessages { get; set; }
        public List<string> ErrorMessages { get; set; }
        public ConsoleOutputVerbosity ConsoleVerbosity { get; set; }
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

    internal class ConstantOutputSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters> _onSatisfied;
        private string _characters1;
        private string _characters2;
        private ConstantSequenceMatchType _matchType;
        private int? _exactLine;
        private List<BackgroundCommandType> _backgroundCommandTypes;

        public ConstantOutputSequence(string prefix, string suffix, Action<FeedLineParameters> onSatisfied, int? exactLine, List<BackgroundCommandType> backgroundCommandTypes)
        {
            _onSatisfied = onSatisfied;
            _characters1 = prefix;
            _characters2 = suffix;
            _matchType = ConstantSequenceMatchType.StartsWithAndEndsWith;
            _exactLine = exactLine;
            _backgroundCommandTypes = backgroundCommandTypes;
        }

        public ConstantOutputSequence(string characters, Action<FeedLineParameters> onSatisfied, ConstantSequenceMatchType MatchType, int? exactLine, List<BackgroundCommandType> backgroundCommandTypes)
        {
            _onSatisfied = onSatisfied;
            _characters1 = characters;
            if (MatchType == ConstantSequenceMatchType.StartsWithAndEndsWith)
            {
                throw new InvalidOperationException();
            }
            _matchType = MatchType;
            _exactLine = exactLine;
            _backgroundCommandTypes = backgroundCommandTypes;
        }

        internal override void FeedLine(FeedLineParameters flParams)
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
                    ret = Line.Equals(_characters1);
                    break;
                case ConstantSequenceMatchType.StartsWith:
                    ret = Line.StartsWith(_characters1);
                    break;
                case ConstantSequenceMatchType.Contains:
                    ret = Line.Contains(_characters1);
                    break;
                case ConstantSequenceMatchType.EndsWith:
                    ret = Line.EndsWith(_characters1);
                    break;
                case ConstantSequenceMatchType.StartsWithAndEndsWith:
                    ret = Line.StartsWith(_characters1) && Line.EndsWith(_characters2);
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

    internal class TimeOutputSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, int> _onSatisfied;
        private const string PREFIX = "Game-Time: ";

        public TimeOutputSequence(Action<FeedLineParameters, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
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

    internal class InformationOutputSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters, int, int, int, int, int, int, int, int, int, int> _onSatisfied;
        public InformationOutputSequence(Action<FeedLineParameters, int, int, int, int, int, int, int, int, int, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters Parameters)
        {
            int iEarth = 0;
            int iWind = 0;
            int iFire = 0;
            int iWater = 0;
            int iDivination = 0;
            int iArcana = 0;
            int iLife = 0;
            int iSorcery = 0;
            int iExperience = 0;
            int iTNL = 0;
            bool foundFirstProficienciesLine = false;
            bool foundSecondProficienciesLine = false;
            bool foundHeader = false;
            bool foundExperience = false;
            List<string> Lines = Parameters.Lines;
            if (Lines.Count > 0)
            {
                foreach (string s in Lines)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (foundHeader)
                        {
                            string[] sSplit = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            int numPieces = sSplit.Length;
                            if (numPieces == 6)
                            {
                                if (sSplit[0] == "Experience:" &&
                                    sSplit[2] == "To" &&
                                    sSplit[3] == "next" &&
                                    sSplit[4] == "level:" &&
                                    int.TryParse(sSplit[1], out iExperience) &&
                                    int.TryParse(sSplit[5], out iTNL))
                                {
                                    foundExperience = true;
                                }
                            }
                            else if (foundExperience && numPieces == 8)
                            {
                                if (sSplit[1].EndsWith("%") && sSplit[3].EndsWith("%") && sSplit[5].EndsWith("%") && sSplit[7].EndsWith("%") &&
                                    sSplit[1].Length > 1 && sSplit[3].Length > 1 && sSplit[5].Length > 1 && sSplit[7].Length > 1)
                                {
                                    string s1 = sSplit[1].Substring(0, sSplit[1].Length - 1);
                                    string s2 = sSplit[3].Substring(0, sSplit[3].Length - 1);
                                    string s3 = sSplit[5].Substring(0, sSplit[5].Length - 1);
                                    string s4 = sSplit[7].Substring(0, sSplit[7].Length - 1);
                                    if (sSplit[0] == "Earth:" && sSplit[2] == "Wind:" && sSplit[4] == "Fire:" && sSplit[6] == "Water:")
                                    {
                                        if (!int.TryParse(s1, out iEarth) ||
                                            !int.TryParse(s2, out iWind) ||
                                            !int.TryParse(s3, out iFire) ||
                                            !int.TryParse(s4, out iWater) ||
                                            iEarth < 0 || iWind < 0 || iFire < 0 || iWater < 0)
                                        {
                                            return;
                                        }
                                        else if (foundFirstProficienciesLine)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            foundFirstProficienciesLine = true;
                                        }
                                    }
                                    else if (sSplit[0] == "Divination:" && sSplit[2] == "Arcana:" && sSplit[4] == "Life:" && sSplit[6] == "Sorcery:")
                                    {
                                        if (!int.TryParse(s1, out iDivination) ||
                                            !int.TryParse(s2, out iArcana) ||
                                            !int.TryParse(s3, out iLife) ||
                                            !int.TryParse(s4, out iSorcery) ||
                                            iDivination < 0 || iArcana < 0 || iLife < 0 || iSorcery < 0)
                                        {
                                            return;
                                        }
                                        else if (foundSecondProficienciesLine)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            foundSecondProficienciesLine = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (s != "------------------------------- Character ------------------------------")
                        {
                            return;
                        }
                        else
                        {
                            foundHeader = true;
                        }
                    }
                }
                if (foundFirstProficienciesLine && foundSecondProficienciesLine && foundExperience)
                {
                    _onSatisfied(Parameters, iEarth, iWind, iFire, iWater, iDivination, iArcana, iLife, iSorcery, iExperience, iTNL);
                }
            }
        }
    }

    internal class ScoreOutputSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters, ClassType, int, int, int, double, string, int, int, List<SkillCooldown>, List<string>, PlayerStatusFlags> _onSatisfied;
        private const string SKILLS_PREFIX = "Skills: ";
        private const string SPELLS_PREFIX = "Spells cast: ";
        private const string GOLD_PREFIX = "Gold: ";
        private const string TO_NEXT_LEVEL_PREFIX = " To Next Level:";

        private string _username;
        public ScoreOutputSequence(string username, Action<FeedLineParameters, ClassType, int, int, int, double, string, int, int, List<SkillCooldown>, List<string>, PlayerStatusFlags> onSatisfied)
        {
            _username = username;
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int iLevel;
            if (Lines.Count >= 7)
            {
                int iNextLineIndex = 0;
                int iIndex;
                ClassType? foundClass = null;

                //first line is the player name, title, and level. Parse out the level and class.
                string sNextLine = Lines[iNextLineIndex++];
                if (sNextLine == null || sNextLine.Length < 17) return;
                string[] words = sNextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length < 4) return;
                if (words[0] != _username) return;
                if (words[1] != "the") return;
                for (int i = 2; i < words.Length; i++)
                {
                    switch (words[i])
                    {
                        case "Mage":
                            foundClass = ClassType.Mage;
                            break;
                        case "Priest":
                            foundClass = ClassType.Priest;
                            break;
                        case "Bard":
                            foundClass = ClassType.Bard;
                            break;
                        case "Monk":
                            foundClass = ClassType.Monk;
                            break;
                        case "Hunter":
                            foundClass = ClassType.Hunter;
                            break;
                        case "Rogue":
                            foundClass = ClassType.Rogue;
                            break;
                        case "Warrior":
                            foundClass = ClassType.Warrior;
                            break;
                    }
                    if (foundClass.HasValue)
                    {
                        break;
                    }
                }
                if (!foundClass.HasValue) return;
                if (words[words.Length - 2] != "(lvl") return;
                string sLastWord = words[words.Length - 1];
                if (!sLastWord.EndsWith(")")) return;
                if (sLastWord == ")") return;
                sLastWord = sLastWord.Substring(0, sLastWord.Length - 1);
                if (!int.TryParse(sLastWord, out iLevel)) return;
                if (iLevel <= 0) return;

                //second line contains the poisoned indicator
                PlayerStatusFlags playerStatusFlags = PlayerStatusFlags.None;
                sNextLine = Lines[iNextLineIndex++];
                if (sNextLine != null)
                {
                    if (sNextLine.Contains("*Poisoned*"))
                    {
                        playerStatusFlags |= PlayerStatusFlags.Poisoned;
                    }
                    if (sNextLine.Contains("*Prone*"))
                    {
                        playerStatusFlags |= PlayerStatusFlags.Prone;
                    }
                }

                //third line contains hit points, magic points, and armor class
                sNextLine = Lines[iNextLineIndex++];
                words = sNextLine.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                string hpText = string.Empty;
                string mpText = string.Empty;
                bool foundHit = false;
                bool foundMagic = false;
                bool foundAC = false;
                double armorClass = 0;
                string armorClassText = string.Empty;
                bool foundArmorClass = false;
                foreach (string nextWord in words)
                {
                    if (!foundHit && nextWord == "Hit")
                    {
                        foundHit = true;
                        hpText = sb.ToString();
                        sb.Clear();
                    }
                    else if (!foundHit)
                    {
                        sb.Append(nextWord);
                    }
                    else if (!foundMagic && nextWord == "Magic")
                    {
                        foundMagic = true;
                        mpText = sb.ToString();
                        sb.Clear();
                    }
                    else if (!foundMagic && nextWord != "Points")
                    {
                        sb.Append(nextWord);
                    }
                    else if (foundAC && !foundArmorClass)
                    {
                        if (!double.TryParse(nextWord, out armorClass))
                        {
                            return;
                        }
                        armorClassText = nextWord;
                        foundArmorClass = true;
                    }
                    else if (!foundAC && nextWord == "AC:")
                    {
                        foundAC = true;
                    }
                    else if (nextWord != "Points")
                    {
                        return;
                    }
                }
                if (!foundHit || !foundMagic || !foundAC || !foundArmorClass)
                {
                    return;
                }
                if (!hpText.Contains("/")) return;
                if (!mpText.Contains("/")) return;
                string[] hpmpValues;
                hpmpValues = hpText.Split(new char[] { '/' });
                if (hpmpValues.Length != 2) return;
                if (!int.TryParse(hpmpValues[0], out _)) return;
                int iTotalHP;
                if (!int.TryParse(hpmpValues[1], out iTotalHP)) return;
                hpmpValues = mpText.Split(new char[] { '/' });
                if (hpmpValues.Length != 2) return;
                if (!int.TryParse(hpmpValues[0], out _)) return;
                int iTotalMP;
                if (!int.TryParse(hpmpValues[1], out iTotalMP)) return;

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
                    else if (skill == "fireshield") skillType = SkillWithCooldownType.Fireshield;

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

                _onSatisfied(flParams, foundClass.Value, iLevel, iTotalHP, iTotalMP, armorClass, armorClassText, iGold, iTNL, cooldowns, spells, playerStatusFlags);
                flParams.FinishedProcessing = true;
            }
        }
    }

    internal class EquipmentSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, List<KeyValuePair<string, string>>, int> _onSatisfied;
        public EquipmentSequence(Action<FeedLineParameters, List<KeyValuePair<string, string>>, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int equipmentWeight;
            if (Lines.Count > 0)
            {
                string sFirstLine = Lines[0];
                if (sFirstLine == "You aren't wearing anything.")
                {
                    _onSatisfied(flParams, new List<KeyValuePair<string, string>>(), 0);
                    flParams.FinishedProcessing = true;
                    return;
                }
                if (!sFirstLine.EndsWith(". of equipment.") || Lines.Count < 3 || !string.IsNullOrEmpty(Lines[1]))
                {
                    return;
                }
                string[] firstLineSplit = sFirstLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (!int.TryParse(firstLineSplit[0], out equipmentWeight))
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
                _onSatisfied(flParams, eq, equipmentWeight);
                flParams.FinishedProcessing = true;
            }
        }
    }

    internal class InventorySequence : AOutputProcessingSequence
    {
        private const string YOU_HAVE_PREFIX = "You have: ";
        private Action<FeedLineParameters, List<ItemEntity>, int> _onSatisfied;
        public InventorySequence(Action<FeedLineParameters, List<ItemEntity>, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (Lines.Count > 0 && !string.IsNullOrEmpty(Lines[0]) && Lines[0].StartsWith(YOU_HAVE_PREFIX))
            {
                string sFullContent = StringProcessing.GetListAsString(Lines, 0, YOU_HAVE_PREFIX, true, out _, null).Trim();
                if (sFullContent == "nothing")
                {
                    _onSatisfied(flParams, new List<ItemEntity>(), 0);
                    flParams.FinishedProcessing = true;
                    return;
                }
                int totalInventoryWeightIndex = sFullContent.IndexOf("Inventory weight is ");
                if (totalInventoryWeightIndex > 0 && totalInventoryWeightIndex + "Inventory weight is ".Length != sFullContent.Length)
                {
                    string totalInventoryText = sFullContent.Substring(totalInventoryWeightIndex + "Inventory weight is ".Length).Trim();
                    if (totalInventoryText.EndsWith(" lbs") && totalInventoryText != " lbs")
                    {
                        string sTotalWeight = totalInventoryText.Substring(0, totalInventoryText.Length - " lbs".Length);
                        if (int.TryParse(sTotalWeight, out int iTotalWeight) && iTotalWeight >= 0)
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
                                        RoomTransitionSequence.LoadMustBeItems(itemList, items, flParams.ErrorMessages);
                                    }
                                    _onSatisfied(flParams, itemList, iTotalWeight);
                                    flParams.FinishedProcessing = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    internal class SpellsSequence : AOutputProcessingSequence
    {
        private const string SPELLS_KNOWN_PREFIX = "Spells known: ";
        private const string SPELLS_CAST_PREFIX = "Spells cast: ";
        private Action<FeedLineParameters, List<string>> _onSatisfied;
        public SpellsSequence(Action<FeedLineParameters, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters Parameters)
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

    internal class WhoOutputSequence : AOutputProcessingSequence
    {
        private Action<FeedLineParameters, HashSet<string>> _onSatisfied;
        public WhoOutputSequence(Action<FeedLineParameters, HashSet<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
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

    internal class InventoryEquipmentManagementSequence : AOutputProcessingSequence
    {
        private const string YOU_WIELD_PREFIX = "You wield ";
        private const string YOU_GET_A_PREFIX = "You get ";
        private const string YOU_DROP_A_PREFIX = "You drop ";
        private const string YOU_WEAR_PREFIX = "You wear ";
        private const string YOU_HOLD_PREFIX = "You hold ";
        private const string YOU_REMOVE_PREFIX = "You remove ";
        private const string YOU_REMOVED_PREFIX = "You removed ";
        private const string THE_SHOPKEEP_GIVES_YOU_PREFIX = "The shopkeep gives you ";
        private const string TRADE_MID_TEXT = " gives you ";
        private Action<FeedLineParameters, List<ItemEntity>, ItemManagementAction, int?, int, List<string>, bool, bool> _onSatisfied;
        public InventoryEquipmentManagementSequence(Action<FeedLineParameters, List<ItemEntity>, ItemManagementAction, int?, int, List<string>, bool, bool> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flp)
        {
            List<string> Lines = flp.Lines;
            int iSellGold = 0;
            int? iTotalGold = null;
            ItemManagementAction eAction = ItemManagementAction.None;
            List<string> activeSpells = null;
            bool potionConsumed = false;
            bool poisonCured = false;
            int iLinesCount = Lines.Count;
            if (iLinesCount > 0)
            {
                List<ItemEntity> itemsManaged = null;
                int iIndex = 0;
                while (true)
                {
                    int iOriginalIndex = iIndex;
                    string nextLine = Lines[iIndex];
                    if (!string.IsNullOrEmpty(nextLine))
                    {
                        int lineLength = nextLine.Length;
                        bool expectCapitalized = false;
                        string objectText = string.Empty;
                        if (nextLine == "You aren't wearing anything that can be removed.")
                        {
                            _onSatisfied(flp, new List<ItemEntity>(), ItemManagementAction.Unequip, null, 0, null, false, false);
                            flp.FinishedProcessing = true;
                            return;
                        }
                        else if (nextLine.StartsWith("You can't trade with the ") || nextLine.EndsWith(" says, \"I don't want that!\""))
                        {
                            _onSatisfied(flp, new List<ItemEntity>(), ItemManagementAction.Trade, null, 0, null, false, false);
                            flp.FinishedProcessing = true;
                            return;
                        }
                        else if (nextLine.StartsWith(YOU_WEAR_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.WearItem) return;
                            eAction = ItemManagementAction.WearItem;
                            List<string> wornObjects = StringProcessing.GetList(Lines, iIndex, YOU_WEAR_PREFIX, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, wornObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
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
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(YOU_HOLD_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.HoldItem) return;
                            eAction = ItemManagementAction.HoldItem;
                            List<string> heldObjects = StringProcessing.GetList(Lines, iIndex, YOU_HOLD_PREFIX, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, heldObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
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
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(YOU_REMOVE_PREFIX) || nextLine.StartsWith(YOU_REMOVED_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.Unequip) return;
                            eAction = ItemManagementAction.Unequip;
                            string sExpectedPrefix;
                            if (nextLine.StartsWith(YOU_REMOVE_PREFIX))
                            {
                                sExpectedPrefix = YOU_REMOVE_PREFIX;
                            }
                            else
                            {
                                sExpectedPrefix = YOU_REMOVED_PREFIX;
                            }
                            List<string> removedObjects = StringProcessing.GetList(Lines, iIndex, sExpectedPrefix, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, removedObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
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
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(YOU_WIELD_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.WieldItem) return;
                            eAction = ItemManagementAction.WieldItem;
                            List<string> wieldedObjects = StringProcessing.GetList(Lines, iIndex, YOU_WIELD_PREFIX, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, wieldedObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
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
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(YOU_GET_A_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.PickUpItem) return;
                            eAction = ItemManagementAction.PickUpItem;
                            List<string> retrievedObjects = StringProcessing.GetList(Lines, iIndex, YOU_GET_A_PREFIX, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, retrievedObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
                                foreach (ItemEntity ie in items)
                                {
                                    if (ie is UnknownItemEntity)
                                    {
                                        flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                                    }
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(YOU_DROP_A_PREFIX) && nextLine != InformationalMessagesSequence.FLEE_WITH_DROP_WEAPON)
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.DropItem) return;
                            eAction = ItemManagementAction.DropItem;
                            List<string> droppedObjects = StringProcessing.GetList(Lines, iIndex, YOU_DROP_A_PREFIX, true, out iIndex, null);
                            List<ItemEntity> items = new List<ItemEntity>();
                            RoomTransitionSequence.LoadMustBeItems(items, droppedObjects, flp.ErrorMessages);
                            if (items.Count > 0)
                            {
                                itemsManaged = new List<ItemEntity>();
                                foreach (ItemEntity ie in items)
                                {
                                    if (ie is UnknownItemEntity)
                                    {
                                        flp.ErrorMessages.Add("Unknown item: " + ((UnknownItemEntity)ie).Name);
                                    }
                                    itemsManaged.Add(ie);
                                }
                            }
                        }
                        else if (nextLine.StartsWith(THE_SHOPKEEP_GIVES_YOU_PREFIX))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.SellItem) return;
                            eAction = ItemManagementAction.SellItem;
                            if (!nextLine.EndsWith(".")) return;
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
                        else if (nextLine.Contains(TRADE_MID_TEXT))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.Trade) return;
                            eAction = ItemManagementAction.Trade;
                            if (!nextLine.EndsWith(".")) return;
                            int iMidIndex = nextLine.IndexOf(TRADE_MID_TEXT);
                            int iItemLength = lineLength - iMidIndex - TRADE_MID_TEXT.Length - 1;
                            if (iItemLength <= 0) return;
                            objectText = nextLine.Substring(iMidIndex + TRADE_MID_TEXT.Length, iItemLength);
                        }
                        else if (GetObjectTextForMessageWithSpecificSuffix(nextLine, " disintegrates.", ref objectText))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.ConsumeItem) return;
                            eAction = ItemManagementAction.ConsumeItem;
                            expectCapitalized = true;
                        }
                        else if (GetObjectTextForMessageWithSpecificSuffix(nextLine, " shocks you and you drop it.", ref objectText))
                        {
                            if (eAction != ItemManagementAction.None && eAction != ItemManagementAction.DropItem) return;
                            eAction = ItemManagementAction.DropItem;
                            expectCapitalized = true;
                        }
                        else if (nextLine == "Substance consumed.")
                        {
                            potionConsumed = true;
                        }
                        else if (nextLine == "You feel the poison subside.")
                        {
                            poisonCured = true;
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
                        }
                        else if (nextLine.StartsWith("You now have ") && nextLine.EndsWith(" gold pieces."))
                        {
                            if (nextLine.Length == "You now have ".Length + " gold pieces.".Length)
                            {
                                return;
                            }
                            else
                            {
                                string sGold = nextLine.Substring("You now have ".Length, nextLine.Length - "You now have ".Length - " gold pieces.".Length);
                                if (!int.TryParse(sGold, out int iFoundGold))
                                {
                                    return;
                                }
                                iTotalGold = iFoundGold;
                            }
                        }
                        if (!string.IsNullOrEmpty(objectText))
                        {
                            ItemEntity.GetItemEntityFromObjectText(objectText, ref itemsManaged, flp, expectCapitalized);
                        }
                    }
                    //if a list was processed the index counter has already been incremented. so increment the index if just a single line
                    //was processed.
                    if (iOriginalIndex == iIndex)
                    {
                        iIndex++;
                    }
                    if (iIndex >= iLinesCount)
                    {
                        break;
                    }
                }
                if (itemsManaged != null || potionConsumed || iTotalGold.HasValue)
                {
                    _onSatisfied(flp, itemsManaged, eAction, iTotalGold, iSellGold, activeSpells, potionConsumed, poisonCured);
                    flp.FinishedProcessing = true;
                }
            }
        }

        private static bool GetObjectTextForMessageWithSpecificSuffix(string nextLine, string suffix, ref string objectText)
        {
            bool ret = false;
            if (nextLine.EndsWith(suffix))
            {
                int lineLength = nextLine.Length;
                int suffixIndex = nextLine.IndexOf(suffix);
                if (suffixIndex > 0)
                {
                    objectText = nextLine.Substring(0, lineLength - suffix.Length);
                    ret = true;
                }
            }
            return ret;
        }
    }

    internal class SkillCooldown
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

    internal class RoomTransitionInfo
    {
        public RoomTransitionType TransitionType { get; set; }
        public string RoomName { get; set; }
        public List<string> ObviousExits { get; set; }
        public List<PlayerEntity> Players { get; set; }
        public List<ItemEntity> Items { get; set; }
        public List<MobEntity> Mobs { get; set; }
        public List<UnknownTypeEntity> UnknownEntities { get; set; }
        public bool DrankHazy { get; set; }
    }

    internal class InitialLoginSequence : AOutputProcessingSequence
    {
        public Action<InitialLoginInfo> _onSatisfied;

        public InitialLoginSequence(Action<InitialLoginInfo> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
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

    internal class RoomTransitionSequence : AOutputProcessingSequence
    {
        private const string YOU_SEE_PREFIX = "You see ";
        private Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> _onSatisfied;
        public RoomTransitionSequence(Action<FeedLineParameters, RoomTransitionInfo, int, TrapType> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            TrapType eTrapType = TrapType.None;
            RoomTransitionType rtType = RoomTransitionType.Move;
            int iDamage = 0;
            foreach (var nextMessage in flParams.InfoMessages)
            {
                InformationalMessageType eType = nextMessage.MessageType;
                if (eType == InformationalMessageType.FleeWithoutDropWeapon)
                {
                    rtType = RoomTransitionType.FleeWithoutDropWeapon;
                }
                else if (eType == InformationalMessageType.FleeWithDropWeapon)
                {
                    rtType = RoomTransitionType.FleeWithDropWeapon;
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
            if (exits.Count == 1 && exits[0] == "none")
            {
                exits.Clear();
            }

            List<UnknownTypeEntity> unknownEntities = new List<UnknownTypeEntity>();
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
                LoadMustBeItems(items, roomList3, flParams.ErrorMessages);
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
                        LoadMobs(mobs, null, roomList2, flParams.ErrorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadMustBeItems(items, roomList2, flParams.ErrorMessages);
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
                        LoadMobs(mobs, null, roomList1, flParams.ErrorMessages, EntityTypeFlags.Mob);
                    }
                    else if (foundTypeValue == EntityType.Item)
                    {
                        LoadMustBeItems(items, roomList1, flParams.ErrorMessages);
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
                        LoadMobs(mobs, unknownEntities, roomList1, flParams.ErrorMessages, possibleTypes);
                    }
                    else
                    {
                        List<Entity> entities = new List<Entity>();
                        LoadItems(entities, roomList1, flParams.ErrorMessages, possibleTypes);
                        foreach (Entity e in entities)
                        {
                            if (e is ItemEntity)
                            {
                                items.Add((ItemEntity)e);
                            }
                            else
                            {
                                unknownEntities.Add((UnknownTypeEntity)e);
                            }
                        }
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
            rti.UnknownEntities = unknownEntities;
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
                            int trapDamage = StringProcessing.PullDamageFromString("You lost ", " hit points.", sNextLine);
                            if (trapDamage > 0) damage += trapDamage;
                        }
                    }
                }
            }

            return ProcessRoom(sRoomName, exitsString, list1String, list2String, list3String, onSatisfied, flParams, rtType, damage, trapType, drankHazy);
        }

        public static void LoadMustBeItems(List<ItemEntity> items, List<string> itemNames, List<string> errorMessages)
        {
            foreach (string next in itemNames)
            {
                Entity e = Entity.GetEntity(next, EntityTypeFlags.Item, errorMessages, null, false);
                CheckForValidItem(next, e, errorMessages, EntityTypeFlags.Item);
                items.Add((ItemEntity)e);
            }
        }

        public static void LoadItems(List<Entity> items, List<string> itemNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in itemNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null, false);
                CheckForValidItem(next, e, errorMessages, possibleEntityTypes);
                items.Add(e);
            }
        }
        private static void LoadMobs(List<MobEntity> mobs, List<UnknownTypeEntity> unknownEntities, List<string> mobNames, List<string> errorMessages, EntityTypeFlags possibleEntityTypes)
        {
            foreach (string next in mobNames)
            {
                Entity e = Entity.GetEntity(next, possibleEntityTypes, errorMessages, null, false);
                if (CheckForValidMob(next, e, errorMessages, possibleEntityTypes))
                {
                    mobs.Add((MobEntity)e);
                }
                else if (e is UnknownTypeEntity && unknownEntities != null)
                {
                    unknownEntities.Add((UnknownTypeEntity)e);
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

    internal class CastOffensiveSpellSequence : AOutputProcessingSequence
    {
        internal static List<string> EARTH_OFFENSIVE_SPELLS = new List<string>() { "rumble", "crush", "shatterstone", "engulf", "tremor" };
        internal static List<string> FIRE_OFFENSIVE_SPELLS = new List<string>() { "burn", "fireball", "burstflame", "immolate", "flamefill" };
        internal static List<string> WATER_OFFENSIVE_SPELLS = new List<string>() { "blister", "waterbolt", "steamblast", "bloodboil", "iceblade" };
        internal static List<string> WIND_OFFENSIVE_SPELLS = new List<string>() { "hurt", "dustgust", "shockbolt", "lightning", "thunderbolt" };
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

        public Action<int, bool, MobTypeEnum?, int, List<ItemEntity>, FeedLineParameters> _onSatisfied;
        public CastOffensiveSpellSequence(Action<int, bool, MobTypeEnum?, int, List<ItemEntity>, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            if (lineCount == 0) return;

            //check the first line matches the pattern "You cast a X spell on <something> for Y damage."
            string nextLine = Lines[0];
            if (!nextLine.StartsWith(YOU_CAST_A_PREFIX)) return;

            int lineIndex = 0;
            nextLine = StringProcessing.PullFullMessageWithLineContinuations(Lines, ref lineIndex);

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

            List<ItemEntity> items = new List<ItemEntity>();
            int iExperience;
            MobTypeEnum? mobType;
            bool monsterKilled = AttackSequence.ProcessMonsterKilledMessages(flParams, lineIndex, out iExperience, out mobType, items);
            flParams.FinishedProcessing = true;
            _onSatisfied(damage, monsterKilled, mobType, iExperience, items, flParams);
        }
    }

    internal class AttackSequence : AOutputProcessingSequence
    {
        private const string YOUR_REGULAR_ATTACK_PREFIX = "Your ";
        private const string YOUR_POWER_ATTACK_PREFIX = "Your power attack ";
        private const string BEFORE_DAMAGE = " hits for ";
        private const string AFTER_DAMAGE = " damage.";

        private const string YOU_GAINED_PREFIX = "You gained";
        private const string WAS_CARRYING_MID = " was carrying: ";
        private const string EXPERIENCE_FOR_THE_DEATH_SUFFIX = " experience for the death of ";

        public Action<bool, int, bool, MobTypeEnum?, int, bool, List<ItemEntity>, FeedLineParameters> _onSatisfied;
        public AttackSequence(Action<bool, int, bool, MobTypeEnum?, int, bool, List<ItemEntity>, FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flParams)
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
                List<ItemEntity> items = new List<ItemEntity>();
                bool monsterKilled = false;
                int iExperience = 0;
                MobTypeEnum? eMobType = null;
                if (damage > 0)
                {
                    monsterKilled = ProcessMonsterKilledMessages(flParams, iIndex + 1, out iExperience, out eMobType, items);
                }
                flParams.FinishedProcessing = true;
                _onSatisfied(fumbled, damage, monsterKilled, eMobType, iExperience, powerAttacked, items, flParams);
            }
        }

        internal static bool ProcessMonsterKilledMessages(FeedLineParameters flParams, int startLineIndex, out int experience, out MobTypeEnum? monsterType, List<ItemEntity> items)
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
                        List<ItemEntity> itemsFirstPass = new List<ItemEntity>();
                        RoomTransitionSequence.LoadMustBeItems(itemsFirstPass, itemsString, flParams.ErrorMessages);
                        foreach (ItemEntity next in itemsFirstPass)
                        {
                            ItemEntity.ProcessAndSplitItemEntity(next, ref items, flParams, false);
                        }
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

        internal override void FeedLine(FeedLineParameters flParams)
        {
            List<string> Lines = flParams.Lines;
            if (!flParams.IsFightingMob) return;
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
                        _onSatisfied(status, flParams);
                        return;
                    }
                }
            }
        }
    }

    internal class PleaseWaitSequence : AOutputProcessingSequence
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

        internal override void FeedLine(FeedLineParameters flParams)
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
                        if (flParams.InfoMessages.Count == 1)
                        {
                            if (flParams.ConsoleVerbosity == ConsoleOutputVerbosity.Minimum) flParams.SetSuppressEcho(true);
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

    internal class SearchSequence : AOutputProcessingSequence
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

        internal override void FeedLine(FeedLineParameters flParams)
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

    internal class FailMovementSequence : AOutputProcessingSequence
    {
        private const string YOU_FELL_AND_HURT_YOURSELF_PREFIX = "You fell and hurt yourself for ";
        private const string YOU_FELL_AND_HURT_YOURSELF_SUFFIX = " damage.";
        public Action<FeedLineParameters, MovementResult, int> _onSatisfied;

        public FailMovementSequence(Action<FeedLineParameters, MovementResult, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters Parameters)
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
                else if (firstLine == "Your class prevents you from using that exit.")
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine == "Your clan allegiance prevents you from using that exit.")
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine == "Your guild allegiance prevents you from using that exit.")
                {
                    result = MovementResult.TotalFailure;
                }
                else if (firstLine == "You can't see in there.")
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
            return StringProcessing.PullDamageFromString(YOU_FELL_AND_HURT_YOURSELF_PREFIX, YOU_FELL_AND_HURT_YOURSELF_SUFFIX, nextLine);
        }
    }

    internal class InformationalMessagesSequence : AOutputProcessingSequence
    {
        public const string CELDUIN_EXPRESS_IN_BREE_MESSAGE = "### The Celduin Express is ready for boarding in Bree.";
        public const string FLEE_WITHOUT_DROP_WEAPON = "You run like a chicken.";
        public const string FLEE_WITH_DROP_WEAPON = "You drop your weapon and run like a chicken.";

        public Action<FeedLineParameters, List<string>, List<string>, List<string>> _onSatisfied;
        private string _userLoginPrefix;
        private string _deathPrefixMessage;

        public InformationalMessagesSequence(string userName, Action<FeedLineParameters, List<string>, List<string>, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
            _userLoginPrefix = "### " + userName + " the ";
            _deathPrefixMessage = "### Sadly, " + userName + " ";
        }

        internal override void FeedLine(FeedLineParameters Parameters)
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
                else if (sLine == "Your perception is diminished.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.KnowAuraOver;
                }
                else if (sLine == "Your detect-magic wears off.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.DetectMagicOver;
                }
                else if (sLine == "Your magical light fades.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.LightOver;
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
                else if (sLine == "Your fireshield dissipates.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FireshieldOff;
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
                //could be of the form "The toxic air poisoned you." (room damage) or "The green slime poisoned you." (monster damage)
                else if (sLine.EndsWith(" poisoned you."))
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.RoomPoisoned;
                }
                else if (sLine == "Poison courses through your veins.")
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.PoisonDamage;
                }
                else if (sLine == FLEE_WITHOUT_DROP_WEAPON)
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FleeWithoutDropWeapon;
                }
                else if (sLine == FLEE_WITH_DROP_WEAPON)
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.FleeWithDropWeapon;
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
                else if (sLine.StartsWith("Stun cast on "))
                {
                    haveDataToDisplay = true;
                    im = InformationalMessageType.StunCastOnEnemy;
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

                    ItemEntity ient;
                    if (nextMsg == null)
                    {
                        ient = ProcessMessageEndingInItem(sLine, " destroys your ", Parameters);
                        if (ient != null)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.EquipmentDestroyed);
                            nextMsg.Item = ient;
                        }
                    }

                    if (nextMsg == null)
                    {
                        ient = ProcessMessageEndingInItem(sLine, " magically sends you ", Parameters);
                        if (ient != null)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.ItemMagicallySentToYou);
                            nextMsg.Item = ient;
                        }
                    }

                    if (nextMsg == null)
                    {
                        ient = ProcessEquipmentFellApartMessage(sLine, "Your ", " fell apart.", Parameters.ErrorMessages);
                        if (ient != null)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.EquipmentFellApart);
                            nextMsg.Item = ient;
                        }
                    }

                    if (nextMsg == null)
                    {
                        ient = ProcessEquipmentFellApartMessage(sLine, "Your ", " is broken.", Parameters.ErrorMessages);
                        if (ient != null)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.WeaponIsBroken);
                            nextMsg.Item = ient;
                        }
                    }

                    if (nextMsg == null)
                    {
                        ient = ProcessMessageEndingInItem(sLine, " picked up ", Parameters);
                        if (ient != null && ient.ItemType.HasValue)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.MobPickedUpItem);
                            nextMsg.Item = ient;
                        }
                    }

                    int iDamage;
                    if (nextMsg == null)
                    {
                        iDamage = FailMovementSequence.ProcessFallDamage(sLine);
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

                    if (nextMsg == null && sLine.EndsWith(" circles you."))
                    {
                        nextMsg = new InformationalMessages(InformationalMessageType.EnemyCirclesYou);
                    }
                    if (nextMsg == null && sLine.EndsWith(" tried to circle you."))
                    {
                        nextMsg = new InformationalMessages(InformationalMessageType.EnemyTriesToCircleYou);
                    }
                    if (nextMsg == null && sLine.EndsWith(" activates sanctuary."))
                    {
                        nextMsg = new InformationalMessages(InformationalMessageType.EnemyActivatesSanctuary);
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

                    if (nextMsg == null)
                    {
                        iDamage = StringProcessing.PullDamageFromString("Your fireshield inflicts ", " damage to your attacker and dissipates!", sLine);
                        if (iDamage > 0)
                        {
                            nextMsg = new InformationalMessages(InformationalMessageType.FireshieldInflictsDamageAndDissipates);
                            nextMsg.Damage = iDamage;
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
                            sLine.Contains(" obliterates ") ||  //CSRTODO: not sure of order
                            sLine.Contains(" annihilates ") ||  //CSRTODO: not sure of order
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
            if (linesToRemove != null & Parameters.ConsoleVerbosity != ConsoleOutputVerbosity.Maximum)
            {
                linesToRemove.Reverse();
                foreach (int i in linesToRemove)
                {
                    Lines.RemoveAt(i);
                }
                if (!haveDataToDisplay)
                {
                    Parameters.SetSuppressEcho(true);
                    Parameters.FinishedProcessing = true;
                }
            }

            if (Parameters.InfoMessages.Count > 0 || broadcastMessages != null)
            {
                _onSatisfied(Parameters, broadcastMessages, addedPlayers, removedPlayers);
            }
        }

        private ItemEntity ProcessEquipmentFellApartMessage(string sLine, string prefix, string suffix, List<string> errorMessages)
        {
            ItemEntity ient = null;
            if (sLine.StartsWith(prefix) && sLine.EndsWith(suffix))
            {
                int lineLength = sLine.Length;
                int objectLen = lineLength - prefix.Length - suffix.Length;
                if (objectLen > 0)
                {
                    string objectText = sLine.Substring(prefix.Length, objectLen);
                    ient = ProcessItemInMessage(objectText, errorMessages);
                }
            }
            return ient;
        }

        private ItemEntity ProcessMessageEndingInItem(string sLine, string midText, FeedLineParameters Parameters)
        {
            ItemEntity ret = null;
            int lineLength = sLine.Length;
            int index = sLine.IndexOf(midText);
            if (index > 0 && sLine.EndsWith("."))
            {
                string sWhat = sLine.Substring(index + midText.Length, lineLength - index - midText.Length - 1);
                ret = ProcessItemInMessage(sWhat, Parameters.ErrorMessages);
            }
            return ret;
        }

        private ItemEntity ProcessItemInMessage(string sWhat, List<string> errorMessages)
        {
            ItemEntity ret = null;
            Entity e = Entity.GetEntity(sWhat, EntityTypeFlags.Item, errorMessages, null, false);
            if (RoomTransitionSequence.CheckForValidItem(sWhat, e, errorMessages, EntityTypeFlags.Item))
            {
                ret = (ItemEntity)e;
            }
            return ret;
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
                //cannot do exact matching on damage counts because there are cases where damage counts
                //don't match up with the text, e.g. when fireshield prevents some of the damage.
                if (remainder.EndsWith(" barely nicks"))
                {
                    matches = true; //typically 1-2
                }
                else if (remainder.EndsWith(" scratches"))
                {
                    matches = true; //typically 3-5
                }
                else if (remainder.EndsWith(" bruises"))
                {
                    matches = true; //typically 6-9
                }
                else if (remainder.EndsWith(" hurts"))
                {
                    matches = true; //typically 10-12
                }
                else if (remainder.EndsWith(" wounds"))
                {
                    matches = true; //typically 13-15
                }
                else if (remainder.EndsWith(" smites"))
                {
                    matches = true; //typically 16-20
                }
                else if (remainder.EndsWith(" maims"))
                {
                    matches = true; //typically 21-24
                }
                else if (remainder.EndsWith(" pulverizes"))
                {
                    matches = true; //CSRTODO: 26-31
                }
                else if (remainder.EndsWith(" devestates"))
                {
                    matches = true; //CSRTODO: 32-33
                }
                else if (remainder.EndsWith(" obliterates"))
                {
                    matches = true; //CSRTODO: ?
                }
                else if (remainder.EndsWith(" annihilates "))
                {
                    matches = true; //CSRTODO: ?
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

    internal class EntityAttacksYouSequence : AOutputProcessingSequence
    {
        public Action<FeedLineParameters> _onSatisfied;

        public EntityAttacksYouSequence(Action<FeedLineParameters> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }
        internal override void FeedLine(FeedLineParameters flParams)
        {
            foreach (InformationalMessages nextMessage in flParams.InfoMessages)
            {
                InformationalMessageType msgType = nextMessage.MessageType;
                if (msgType == InformationalMessageType.EnemyAttacksYou ||
                    msgType == InformationalMessageType.EnemyCirclesYou ||
                    msgType == InformationalMessageType.EnemyTriesToCircleYou ||
                    msgType == InformationalMessageType.EnemyActivatesSanctuary)
                {
                    _onSatisfied(flParams);
                    break;
                }
            }
        }
    }

    internal class SelfSpellCastSequence : AOutputProcessingSequence
    {
        public static Dictionary<string, string> ACTIVE_SPELL_TO_ACTIVE_TEXT = new Dictionary<string, string>()
        {
            { "You feel holy.", "bless" },
            { "You feel watched.", "protection" },
            { "You can fly!", "fly"},
            { "Your eyes feel funny.", "detect-magic" }, //dark green potion
            { "You can count magical bonuses now!", "detect-magic" }, //magical tabulator
            { "You turn invisible.", "invisibility" },
            { "You become shielded from the normal fire element.", "endure-fire" },
            { "You become shielded from the normal wind element.", "endure-cold" },
            { "You become shielded from the normal earth element.", "endure-earth" },
            { "You become shielded from the normal water element.", "endure-water" },
            { "You begin to float.", "levitation" },
            { "Your eyes tingle.", "detect-invisible"},
            { "You become more perceptive.", "know-aura" },
        };

        public Action<FeedLineParameters, BackgroundCommandType?, string, List<ItemEntity>> _onSatisfied;
        public SelfSpellCastSequence(Action<FeedLineParameters, BackgroundCommandType?, string, List<ItemEntity>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        internal override void FeedLine(FeedLineParameters flParams)
        {
            BackgroundCommandType? matchingSpell = null;
            List<string> Lines = flParams.Lines;
            int lineCount = Lines.Count;
            string activeSpell = null;
            List<ItemEntity> consumedItems = null;
            bool hasSpellCast = false;
            if (Lines.Count > 0)
            {
                foreach (string nextLine in Lines)
                {
                    int lineLength = nextLine.Length;
                    if (nextLine == "Vigor spell cast.")
                    {
                        hasSpellCast = true;
                        matchingSpell = BackgroundCommandType.Vigor;
                    }
                    else if (nextLine == "Mend-wounds spell cast.")
                    {
                        hasSpellCast = true;
                        matchingSpell = BackgroundCommandType.MendWounds;
                    }
                    else if (nextLine == "Curepoison spell cast on yourself.")
                    {
                        hasSpellCast = true;
                        matchingSpell = BackgroundCommandType.CurePoison;
                    }
                    else if (nextLine == "Light spell cast.")
                    {
                        hasSpellCast = true;
                        activeSpell = "light";
                    }
                    else if (nextLine.EndsWith(" spell cast."))
                    {
                        hasSpellCast = true;
                        continue;
                    }
                    else if (ACTIVE_SPELL_TO_ACTIVE_TEXT.TryGetValue(nextLine, out string activeSpellTemp))
                    {
                        activeSpell = activeSpellTemp;
                        if (activeSpell == "bless")
                        {
                            matchingSpell = BackgroundCommandType.Bless;
                        }
                        else if (activeSpell == "protection")
                        {
                            matchingSpell = BackgroundCommandType.Protection;
                        }
                    }
                    else if (nextLine.EndsWith(" disintegrates."))
                    {
                        int suffixIndex = nextLine.IndexOf(" disintegrates.");
                        if (suffixIndex > 0)
                        {
                            string objectText = nextLine.Substring(0, lineLength - " disintegrates.".Length);
                            ItemEntity.GetItemEntityFromObjectText(objectText, ref consumedItems, flParams, true);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(nextLine) &&
                             nextLine != "You start to feel real strange, as if connected to another dimension." && //detect invis
                             nextLine != "You feel much better.") //cure-poison
                    {
                        return;
                    }
                }
            }
            if (hasSpellCast)
            {
                _onSatisfied(flParams, matchingSpell.Value, activeSpell, consumedItems);
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

        public static int PullDamageFromString(string prefix, string suffix, string nextLine)
        {
            int ret = 0;
            if (nextLine.StartsWith(prefix) && nextLine.EndsWith(suffix))
            {
                int iPrefixLen = prefix.Length;
                int iSuffixLen = suffix.Length;
                int iTotalLen = nextLine.Length;
                int iDamageLen = iTotalLen - iPrefixLen - iSuffixLen;
                if (iDamageLen > 0)
                {
                    string sDamage = nextLine.Substring(iPrefixLen, iDamageLen);
                    if (int.TryParse(sDamage, out int iThisDamage) && iThisDamage > 0)
                    {
                        ret = iThisDamage;
                    }
                }
            }
            return ret;
        }

        public static string PullFullMessageWithLineContinuations(List<string> Lines, ref int LineIndex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Lines[LineIndex].Trim());
            int LineCount = Lines.Count;
            while (true)
            {
                LineIndex++;
                if (LineIndex == LineCount) break;
                string sNextLine = Lines[LineIndex];
                if (sNextLine.StartsWith("  "))
                {
                    sb.Append(" ");
                    sb.Append(sNextLine.Trim());
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }
    }
}

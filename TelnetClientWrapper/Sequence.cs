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
        public FeedLineParameters(List<string> Lines, BackgroundCommandType? BackgroundCommandType, string CurrentlyFightingMob)
        {
            this.Lines = Lines;
            this.BackgroundCommandType = BackgroundCommandType;
            this.CurrentlyFightingMob = CurrentlyFightingMob;
        }
        public List<string> Lines { get; set; }
        public BackgroundCommandType? BackgroundCommandType { get; set; }
        public string CurrentlyFightingMob { get; set; }
        public bool FinishedProcessing { get; set; }
        public bool SuppressEcho { get; set; }
        public CommandResult? CommandResult { get; set; }
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

        public static List<int> GenerateBytesForPattern(string pattern, Dictionary<char,int> asciiMapping)
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
            bool firstLine = true;
            if (Lines.Count > 0)
            {
                StringBuilder oSkills = null;
                StringBuilder oSpells = null;
                bool foundSkills = false;
                bool finishedSkills = false;
                bool foundSpells = false;
                bool finishedSpells = false;
                foreach (string nextLine in Lines)
                {
                    if (firstLine)
                    {
                        if (!nextLine.StartsWith(_username + " the ", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                        firstLine = false;
                    }
                    string nextLineTemp = nextLine;
                    if (!foundSkills)
                    {
                        if (nextLineTemp.StartsWith(SKILLS_PREFIX))
                        {
                            oSkills = new StringBuilder();
                            foundSkills = true;
                            nextLineTemp = nextLineTemp.Substring(SKILLS_PREFIX.Length);
                        }
                    }
                    if (foundSkills && !finishedSkills)
                    {
                        int iPeriodIndex = nextLineTemp.IndexOf(".");
                        if (iPeriodIndex == 0)
                        {
                            finishedSkills = true;
                            continue; //spells always starts on a new line so skip this line
                        }
                        else if (iPeriodIndex < 0)
                        {
                            oSkills.Append(nextLineTemp);
                        }
                        else
                        {
                            oSkills.Append(nextLineTemp.Substring(0, iPeriodIndex));
                            finishedSkills = true;
                            continue; //spells always starts on a new line so skip this line
                        }
                    }
                    if (finishedSkills && !foundSpells)
                    {
                        if (nextLineTemp.StartsWith(SPELLS_PREFIX))
                        {
                            oSpells = new StringBuilder();
                            foundSpells = true;
                            nextLineTemp = nextLineTemp.Substring(SPELLS_PREFIX.Length);
                        }
                    }
                    if (foundSpells && !finishedSpells)
                    {
                        int iPeriodIndex = nextLineTemp.IndexOf(".");
                        if (iPeriodIndex == 0)
                        {
                            finishedSpells = true;
                            break;
                        }
                        else if (iPeriodIndex < 0)
                        {
                            oSpells.Append(nextLineTemp);
                        }
                        else
                        {
                            oSpells.Append(nextLineTemp.Substring(0, iPeriodIndex));
                            finishedSpells = true;
                            break;
                        }
                    }
                }

                if (!finishedSpells)
                {
                    return;
                }

                List<SkillCooldown> cooldowns = new List<SkillCooldown>();

                string sSkills = oSkills.ToString();
                string[] sSkillList = sSkills.Split(new char[] { ',' });
                foreach (string sNextSkill in sSkillList)
                {
                    string sTempSkill = sNextSkill.Trim();
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
                string sSpells = oSpells.ToString();
                string[] sSpellsList = sSpells.Split(new char[] { ',' }, StringSplitOptions.None);
                foreach (string sNextSpell in sSpellsList)
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

    public class SkillCooldown
    {
        public SkillWithCooldownType SkillType { get; set; }
        public bool Active { get; set; }
        public DateTime? NextAvailable { get; set; }
    }

    internal enum RoomTransitionType
    {
        Move,
        Flee,
        Hazy,
    }

    internal class RoomTransitionSequence : IOutputProcessingSequence
    {
        private Action<RoomTransitionType, string, List<string>, FeedLineParameters> _onSatisfied;
        public RoomTransitionSequence(Action<RoomTransitionType, string, List<string>, FeedLineParameters> onSatisfied)
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
            nextLineIndex++;

            if (nextLineIndex >= Lines.Count) return;
            string sRoomName = Lines[nextLineIndex];
            if (string.IsNullOrEmpty(sRoomName)) return;
            nextLineIndex++;

            //blank line after room name
            if (Lines[nextLineIndex] != string.Empty) return;
            nextLineIndex++;

            if (nextLineIndex < Lines.Count)
            {
                sNextLine = Lines[nextLineIndex];
                if (sNextLine.StartsWith("Obvious exits: "))
                {
                    sNextLine = sNextLine.Substring("Obvious exits: ".Length);
                    StringBuilder sbExits = new StringBuilder();
                    while (true)
                    {
                        if (sNextLine.Contains("."))
                        {
                            sNextLine = sNextLine.Substring(0, sNextLine.IndexOf("."));
                            sbExits.Append(sNextLine);
                            break;
                        }
                        else
                        {
                            sbExits.Append(sNextLine);
                            nextLineIndex++;
                            sNextLine = Lines[nextLineIndex];
                        }
                    }
                    string allExits = sbExits.ToString().Replace(" ", string.Empty);
                    string[] allExitsList = allExits.Split(',');
                    List<string> allExitsList2 = new List<string>();
                    allExitsList2.AddRange(allExitsList);

                    _onSatisfied(rtType, sRoomName, allExitsList2, flParams);
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
        public Action<List<InformationalMessages>, List<string>> _onSatisfied;

        public InformationalMessagesSequence(Action<List<InformationalMessages>, List<string>> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(FeedLineParameters Parameters)
        {
            List<InformationalMessages> messages = null;
            List<string> broadcastMessages = null;
            List<string> Lines = Parameters.Lines;
            List<int> linesToRemove = null;
            bool haveDataToDisplay = false;
            for (int i = 0; i < Lines.Count; i++)
            {
                bool whitespaceLine = false;
                InformationalMessages? im = null;
                string sLine = Lines[i];
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
                         sLine == "The glaring sun beats down upon the inhabitants of the world." ||
                         sLine == "Clear, blue skies cover the land." ||
                         sLine == "The sky is dark as pitch." ||
                         sLine == "The earth trembles under your feet." ||
                         sLine == "Light clouds appear over the mountains." ||
                         sLine == "A light breeze blows from the south." ||
                         sLine == "### The Celduin Express is ready for boarding in Bree.")
                {
                    //These lines will be removed.
                }
                else if (sLine.StartsWith("###"))
                {
                    im = InformationalMessages.Broadcast;
                    if (broadcastMessages == null)
                        broadcastMessages = new List<string>();
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
                _onSatisfied(messages, broadcastMessages);
            }
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

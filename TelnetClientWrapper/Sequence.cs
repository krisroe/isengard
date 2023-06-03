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
        void FeedLine(string[] Lines, FeedLineParameters Parameters);
    }

    public class FeedLineParameters
    {
        public FeedLineParameters(BackgroundCommandType? BackgroundCommandType, string CurrentlyFightingMob)
        {
            this.BackgroundCommandType = BackgroundCommandType;
            this.CurrentlyFightingMob = CurrentlyFightingMob;
        }
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

        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
            BackgroundCommandType? backgroundCommandType = flParams.BackgroundCommandType;
            if (_backgroundCommandTypes != null && (!backgroundCommandType.HasValue || !_backgroundCommandTypes.Contains(backgroundCommandType.Value)))
            {
                return;
            }
            bool match = false;
            if (_exactLine.HasValue)
            {
                int exactLineVal = _exactLine.Value;
                if (Lines.Length < exactLineVal)
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

    public class SkillCooldownSequence : IOutputProcessingSequence
    {
        private enum SkillCooldownStep
        {
            None,
            FinishedStartPattern,
            LeftBracket,
            Minute,
            Colon,
            TensOfSeconds,
            Seconds,
            A,
            C,
            T,
            I,
            V,
            E
        }

        private SkillWithCooldownType _skillWithCooldownType;
        private List<char> _chars;
        private Action<SkillWithCooldownType, bool, DateTime?> _onSatisfied;

        public SkillCooldownSequence(SkillWithCooldownType skillWithCooldownType, Action<SkillWithCooldownType, bool, DateTime?> onSatisfied)
        {
            _skillWithCooldownType = skillWithCooldownType;
            _onSatisfied = onSatisfied;
            string sPattern;
            switch (_skillWithCooldownType)
            {
                case SkillWithCooldownType.PowerAttack:
                    sPattern = "(power) attack";
                    break;
                case SkillWithCooldownType.Manashield:
                    sPattern = "manashield";
                    break;
                default:
                    throw new InvalidOperationException();
            }
            sPattern += " ";
            _chars = new List<char>();
            foreach (char c in sPattern)
            {
                _chars.Add(c);
            }
        }

        private bool CanBeActive()
        {
            return _skillWithCooldownType == SkillWithCooldownType.Manashield;
        }

        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
            foreach (string nextLine in Lines)
            {
                SkillCooldownStep currentStep = SkillCooldownStep.None;
                int currentMatchPoint = -1;
                int minutes = 0;
                int tensOfSeconds = 0;
                int seconds = 0;
                foreach (char nextCharacter in nextLine)
                {
                    bool startedAtNone = false;
                    if (currentStep == SkillCooldownStep.None)
                    {
                        startedAtNone = true;
                        char nextCharToMatch = _chars[currentMatchPoint + 1];
                        if (nextCharToMatch == nextCharacter)
                        {
                            currentMatchPoint++;
                            if (currentMatchPoint == _chars.Count - 1) //finished start pattern
                            {
                                currentStep = SkillCooldownStep.FinishedStartPattern;
                                currentMatchPoint = -1;
                            }
                        }
                        else //start over
                        {
                            currentMatchPoint = -1;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.FinishedStartPattern)
                    {
                        if (nextCharacter == '[')
                        {
                            currentStep = SkillCooldownStep.LeftBracket;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.LeftBracket)
                    {
                        if (nextCharacter == 'A' && CanBeActive())
                        {
                            currentStep = SkillCooldownStep.A;
                        }
                        else if (nextCharacter >= '0' && nextCharacter <= '9')
                        {
                            minutes = nextCharacter - '0';
                            currentStep = SkillCooldownStep.Minute;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.Minute)
                    {
                        if (nextCharacter == ':')
                        {
                            currentStep = SkillCooldownStep.Colon;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.Colon)
                    {
                        if (nextCharacter >= '0' && nextCharacter <= '9')
                        {
                            tensOfSeconds = nextCharacter - '0';
                            currentStep = SkillCooldownStep.TensOfSeconds;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.TensOfSeconds)
                    {
                        if (nextCharacter >= '0' && nextCharacter <= '9')
                        {
                            seconds = nextCharacter - '0';
                            currentStep = SkillCooldownStep.Seconds;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.Seconds || currentStep == SkillCooldownStep.E)
                    {
                        if (nextCharacter == ']')
                        {
                            bool isActive;
                            DateTime? nextAvailableDate;
                            if (currentStep == SkillCooldownStep.Seconds)
                            {
                                isActive = false;
                                nextAvailableDate = DateTime.UtcNow.Add(new TimeSpan(0, minutes, tensOfSeconds * 10 + seconds));
                            }
                            else
                            {
                                isActive = true;
                                nextAvailableDate = null;
                            }
                            currentStep = SkillCooldownStep.None;
                            _onSatisfied(_skillWithCooldownType, isActive, nextAvailableDate);
                            return;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.A)
                    {
                        if (nextCharacter == 'C')
                        {
                            currentStep = SkillCooldownStep.C;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.C)
                    {
                        if (nextCharacter == 'T')
                        {
                            currentStep = SkillCooldownStep.T;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.T)
                    {
                        if (nextCharacter == 'I')
                        {
                            currentStep = SkillCooldownStep.I;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.I)
                    {
                        if (nextCharacter == 'V')
                        {
                            currentStep = SkillCooldownStep.V;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    else if (currentStep == SkillCooldownStep.V)
                    {
                        if (nextCharacter == 'E')
                        {
                            currentStep = SkillCooldownStep.E;
                        }
                        else
                        {
                            currentStep = SkillCooldownStep.None;
                        }
                    }
                    if (!startedAtNone && currentStep == SkillCooldownStep.None)
                    {
                        currentMatchPoint = -1;
                    }
                }
            }
        }
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
        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
            RoomTransitionType rtType = RoomTransitionType.Move;
            int nextLineIndex = 0;

            //skip fleeing messages for scared exits
            while (nextLineIndex < Lines.Length && 
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

            if (nextLineIndex >= Lines.Length) return;
            string sRoomName = Lines[nextLineIndex];
            if (string.IsNullOrEmpty(sRoomName)) return;
            nextLineIndex++;

            //blank line after room name
            if (Lines[nextLineIndex] != string.Empty) return;
            nextLineIndex++;

            if (nextLineIndex < Lines.Length)
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
        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
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
        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
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

        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
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

        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
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
                    if (!rest.Contains(":"))
                    {
                        break;
                    }
                    string[] sPieces = rest.Split(new string[] { ":" }, StringSplitOptions.None);
                    int iMinutes;
                    int iSeconds;
                    if (sPieces.Length == 2 && int.TryParse(sPieces[0], out iMinutes) && int.TryParse(sPieces[1], out iSeconds))
                    {
                        waitSeconds = (iMinutes * 60) + iSeconds;
                    }
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
    }

    internal class SpellsCastSequence : IOutputProcessingSequence
    {
        private List<char> _chars;
        private Action<List<string>> _onSatisfied;
        public SpellsCastSequence(Action<List<string>> onSatisfied)
        {
            _chars = new List<char>();
            foreach (char c in "Spells cast: ")
            {
                _chars.Add(c);
            }
            _onSatisfied = onSatisfied;
        }

        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
            foreach (string nextLine in Lines)
            {
                SpellsCastStep currentStep = SpellsCastStep.None;
                int currentMatchPoint = -1;
                List<char> spellText = new List<char>();
                foreach (char nextChar in nextLine)
                {
                    if (currentStep == SpellsCastStep.None)
                    {
                        char nextCharToMatch = _chars[currentMatchPoint + 1];
                        if (nextCharToMatch == nextChar)
                        {
                            currentMatchPoint++;
                            if (currentMatchPoint == _chars.Count - 1) //finished start pattern
                            {
                                currentStep = SpellsCastStep.PastSpellsCast;
                                currentMatchPoint = -1;
                                spellText = new List<char>();
                            }
                        }
                        else //start over
                        {
                            currentMatchPoint = -1;
                        }
                    }
                    else if (currentStep == SpellsCastStep.PastSpellsCast)
                    {
                        if (nextChar == '.')
                        {
                            currentStep = SpellsCastStep.None;
                            currentMatchPoint = -1;
                            StringBuilder sb = new StringBuilder();
                            foreach (char nextCharacter in spellText)
                            {
                                sb.Append(nextCharacter);
                            }
                            List<string> ret = new List<string>();
                            ret.AddRange(sb.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                            _onSatisfied(ret);
                            return;
                        }
                        else
                        {
                            spellText.Add(nextChar);
                        }
                    }
                }
            }
        }

        private enum SpellsCastStep
        {
            None,
            PastSpellsCast
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
        public void FeedLine(string[] Lines, FeedLineParameters flParams)
        {
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

    public enum SkillWithCooldownType
    {
        PowerAttack,
        Manashield,
    }
}

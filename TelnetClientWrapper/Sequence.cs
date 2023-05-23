using System;
using System.Collections.Generic;
using System.Text;

namespace IsengardClient
{
    public interface ISequence
    {
        void FeedByte(int nextByte);
    }

    internal class ConstantSequence : ISequence
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

        public void FeedByte(int nextByte)
        {
            if (_chars[_currentMatchPoint + 1] == nextByte)
            {
                _currentMatchPoint++;
                if (_currentMatchPoint == _chars.Count - 1) //reached end of sequence
                {
                    _onSatisfied();
                    _currentMatchPoint = -1;
                }
            }
            else
            {
                _currentMatchPoint = -1;
            }
        }
    }

    internal class HPMPSequence : ISequence
    {
        private List<int> HPNumbers = new List<int>();
        private List<int> MPNumbers = new List<int>();
        private HPMPStep CurrentStep = HPMPStep.None;
        private Action<int, int> _onSatisfied;

        public HPMPSequence(Action<int, int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
        }

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
        }

        public void FeedByte(int nextByte)
        {
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
                    if (nextByte == AsciiMapping.ASCII_COLON) //finished
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
                        _onSatisfied(hp, mp);
                    }
                    CurrentStep = HPMPStep.None;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (CurrentStep == HPMPStep.None)
            {
                HPNumbers.Clear();
                MPNumbers.Clear();
            }
        }
    }

    public class SkillCooldownSequence : ISequence
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

        private SkillCooldownStep _currentStep;
        private SkillWithCooldownType _skillWithCooldownType;
        private int _currentMatchPoint = -1;
        private List<int> _chars;
        private int _minutes;
        private int _tensOfSeconds;
        private int _seconds;
        private Action<SkillWithCooldownType, bool, DateTime?> _onSatisfied;

        public SkillCooldownSequence(SkillWithCooldownType skillWithCooldownType, Dictionary<char, int> asciiMapping, Action<SkillWithCooldownType, bool, DateTime?> onSatisfied)
        {
            _skillWithCooldownType = skillWithCooldownType;
            _currentStep = SkillCooldownStep.None;
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
            _chars = ConstantSequence.GenerateBytesForPattern(sPattern, asciiMapping);
            _minutes = 0;
            _tensOfSeconds = 0;
            _seconds = 0;
        }

        private bool CanBeActive()
        {
            return _skillWithCooldownType == SkillWithCooldownType.Manashield;
        }

        public void FeedByte(int nextByte)
        {
            bool startedAtNone = false;
            if (_currentStep == SkillCooldownStep.None)
            {
                startedAtNone = true;
                int nextCharToMatch = _chars[_currentMatchPoint + 1];
                if (nextCharToMatch == nextByte)
                {
                    _currentMatchPoint++;
                    if (_currentMatchPoint == _chars.Count - 1) //finished start pattern
                    {
                        _currentStep = SkillCooldownStep.FinishedStartPattern;
                        _currentMatchPoint = -1;
                    }
                }
                else //start over
                {
                    _currentMatchPoint = -1;
                }
            }
            else if (_currentStep == SkillCooldownStep.FinishedStartPattern)
            {
                if (nextByte == AsciiMapping.ASCII_LEFT_BRACKET)
                {
                    _currentStep = SkillCooldownStep.LeftBracket;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.LeftBracket)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_A && CanBeActive())
                {
                    _currentStep = SkillCooldownStep.A;
                }
                else if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                {
                    _minutes = nextByte - AsciiMapping.ASCII_NUMBER_ZERO;
                    _currentStep = SkillCooldownStep.Minute;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.Minute)
            {
                if (nextByte == AsciiMapping.ASCII_COLON)
                {
                    _currentStep = SkillCooldownStep.Colon;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.Colon)
            {
                if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                {
                    _tensOfSeconds = nextByte - AsciiMapping.ASCII_NUMBER_ZERO;
                    _currentStep = SkillCooldownStep.TensOfSeconds;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.TensOfSeconds)
            {
                if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                {
                    _seconds = nextByte - AsciiMapping.ASCII_NUMBER_ZERO;
                    _currentStep = SkillCooldownStep.Seconds;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.Seconds || _currentStep == SkillCooldownStep.E)
            {
                if (nextByte == AsciiMapping.ASCII_RIGHT_BRACKET)
                {
                    bool isActive;
                    DateTime? nextAvailableDate;
                    if (_currentStep == SkillCooldownStep.Seconds)
                    {
                        isActive = false;
                        nextAvailableDate = DateTime.UtcNow.Add(new TimeSpan(0, _minutes, _tensOfSeconds * 10 + _seconds));
                    }
                    else
                    {
                        isActive = true;
                        nextAvailableDate = null;
                    }
                    _onSatisfied(_skillWithCooldownType, isActive, nextAvailableDate);
                }
                _currentStep = SkillCooldownStep.None;
            }
            else if (_currentStep == SkillCooldownStep.A)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_C)
                {
                    _currentStep = SkillCooldownStep.C;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.C)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_T)
                {
                    _currentStep = SkillCooldownStep.T;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.T)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_I)
                {
                    _currentStep = SkillCooldownStep.I;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.I)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_V)
                {
                    _currentStep = SkillCooldownStep.V;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.V)
            {
                if (nextByte == AsciiMapping.ASCII_UPPERCASE_E)
                {
                    _currentStep = SkillCooldownStep.E;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            if (!startedAtNone && _currentStep == SkillCooldownStep.None)
            {
                _currentMatchPoint = -1;
            }
        }
    }

    public class PleaseWaitXSecondsSequence : ISequence
    {
        private Action<int> _onSatisfied;
        private int _currentMatchPoint = -1;
        private List<int> _firstChars;
        private List<int> _secondCharsGreaterThanOne;
        private List<int> _secondCharsOne;
        private List<int> _secondChars;
        private List<int> _waitNumbers;
        private PleaseWaitXSecondsStep _currentStep;

        private enum PleaseWaitXSecondsStep
        {
            None,
            PastPleaseWait,
            SecondPart,
        }

        public PleaseWaitXSecondsSequence(Dictionary<char, int> asciiMapping, Action<int> onSatisfied)
        {
            _onSatisfied = onSatisfied;
            _firstChars = ConstantSequence.GenerateBytesForPattern("Please wait ", asciiMapping);
            _secondCharsGreaterThanOne = ConstantSequence.GenerateBytesForPattern("seconds.", asciiMapping);
            _secondCharsOne = ConstantSequence.GenerateBytesForPattern("more second.", asciiMapping);
            _waitNumbers = new List<int>();
        }

        public void FeedByte(int nextByte)
        {
            if (_currentStep == PleaseWaitXSecondsStep.None)
            {
                int nextCharToMatch = _firstChars[_currentMatchPoint + 1];
                if (nextCharToMatch == nextByte)
                {
                    _currentMatchPoint++;
                    if (_currentMatchPoint == _firstChars.Count - 1) //finished start pattern
                    {
                        _currentStep = PleaseWaitXSecondsStep.PastPleaseWait;
                        _currentMatchPoint = -1;
                        _waitNumbers.Clear();
                    }
                }
                else //start over
                {
                    _currentMatchPoint = -1;
                }
            }
            else if (_currentStep == PleaseWaitXSecondsStep.PastPleaseWait)
            {
                if (nextByte == AsciiMapping.ASCII_SPACE)
                {
                    if (_waitNumbers.Count == 0)
                    {
                        _currentStep = PleaseWaitXSecondsStep.None;
                    }
                    else
                    {
                        if (_waitNumbers.Count == 1 && _waitNumbers[0] == 1)
                        {
                            _secondChars = _secondCharsOne;
                        }
                        else
                        {
                            _secondChars = _secondCharsGreaterThanOne;
                        }
                        _currentStep = PleaseWaitXSecondsStep.SecondPart;
                        _currentMatchPoint = -1;
                    }
                }
                else if (nextByte >= AsciiMapping.ASCII_NUMBER_ZERO && nextByte <= AsciiMapping.ASCII_NUMBER_NINE)
                {
                    _waitNumbers.Add(nextByte - AsciiMapping.ASCII_NUMBER_ZERO);
                }
            }
            else if (_currentStep == PleaseWaitXSecondsStep.SecondPart)
            {
                int nextCharToMatch = _secondChars[_currentMatchPoint + 1];
                if (nextCharToMatch == nextByte)
                {
                    _currentMatchPoint++;
                    if (_currentMatchPoint == _secondChars.Count - 1) //finished pattern
                    {
                        int waitNumber = 0;
                        foreach (int nextWaitNumber in _waitNumbers)
                        {
                            waitNumber = (waitNumber * 10) + nextWaitNumber;
                        }
                        _onSatisfied(waitNumber);
                        _currentStep = PleaseWaitXSecondsStep.None;
                        _currentMatchPoint = -1;
                    }
                }
                else //start over
                {
                    _currentStep = PleaseWaitXSecondsStep.None;
                    _currentMatchPoint = -1;
                }
            }
        }
    }

    public class SpellsCastSequence : ISequence
    {
        private Dictionary<char, int> _asciiMapping;
        private Dictionary<int, char> _reverseAsciiMapping;
        private List<int> _chars;
        private SpellsCastStep _currentStep;
        private int _currentMatchPoint = -1;
        private List<int> _spellText;
        private Action<List<string>> _onSatisfied;
        public SpellsCastSequence(Dictionary<char, int> asciiMapping, Dictionary<int, char> reverseAsciiMapping, Action<List<string>> onSatisfied)
        {
            _asciiMapping = asciiMapping;
            _reverseAsciiMapping = reverseAsciiMapping;
            _chars = ConstantSequence.GenerateBytesForPattern("Spells cast: ", asciiMapping);
            _currentStep = SpellsCastStep.None;
            _spellText = new List<int>();
            _onSatisfied = onSatisfied;
        }

        public void FeedByte(int nextByte)
        {
            if (_currentStep == SpellsCastStep.None)
            {
                int nextCharToMatch = _chars[_currentMatchPoint + 1];
                if (nextCharToMatch == nextByte)
                {
                    _currentMatchPoint++;
                    if (_currentMatchPoint == _chars.Count - 1) //finished start pattern
                    {
                        _currentStep = SpellsCastStep.PastSpellsCast;
                        _currentMatchPoint = -1;
                        _spellText = new List<int>();
                    }
                }
                else //start over
                {
                    _currentMatchPoint = -1;
                }
            }
            else if (_currentStep == SpellsCastStep.PastSpellsCast)
            {
                if (nextByte == _asciiMapping['.'])
                {
                    _currentStep = SpellsCastStep.None;
                    _currentMatchPoint = -1;
                    StringBuilder sb = new StringBuilder();
                    foreach (int nextcharcter in _spellText)
                    {
                        sb.Append(_reverseAsciiMapping[nextcharcter]);
                    }
                    List<string> ret = new List<string>();
                    ret.AddRange(sb.ToString().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                    _onSatisfied(ret);
                }
                else
                {
                    _spellText.Add(nextByte);
                }
            }
        }

        private enum SpellsCastStep
        {
            None,
            PastSpellsCast
        }
    }

    public enum SkillWithCooldownType
    {
        PowerAttack,
        Manashield,
    }
}

using System;
using System.Collections.Generic;
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
            _chars = new List<int>(characters.Length);
            foreach (char c in characters)
            {
                _chars.Add(asciiMapping[c]);
            }
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
        private Dictionary<char, int> _asciiMapping;
        private Action<int, int> _onSatisfied;

        public HPMPSequence(Dictionary<char, int> asciiMapping, Action<int, int> onSatisfied)
        {
            _asciiMapping = asciiMapping;
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
                    if (nextByte == _asciiMapping['('])
                        CurrentStep = HPMPStep.LeftParen;
                    break;
                case HPMPStep.LeftParen:
                    if (nextByte == _asciiMapping[' '])
                    {
                        if (HPNumbers.Count == 0)
                            CurrentStep = HPMPStep.None;
                        else
                            CurrentStep = HPMPStep.AfterHPNumberSpace;
                    }
                    else if (nextByte >= _asciiMapping['0'] && nextByte <= _asciiMapping['9'])
                    {
                        HPNumbers.Add(nextByte - _asciiMapping['0']);
                    }
                    else
                    {
                        CurrentStep = HPMPStep.None;
                    }
                    break;
                case HPMPStep.AfterHPNumberSpace:
                    if (nextByte == _asciiMapping['H'])
                        CurrentStep = HPMPStep.H;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.H:
                    if (nextByte == _asciiMapping[' '])
                        CurrentStep = HPMPStep.BeforeMPNumberSpace;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.BeforeMPNumberSpace:
                    if (nextByte == _asciiMapping[' '])
                    {
                        if (MPNumbers.Count == 0)
                            CurrentStep = HPMPStep.None;
                        else
                            CurrentStep = HPMPStep.AfterMPNumberSpace;
                    }
                    else if (nextByte >= _asciiMapping['0'] && nextByte <= _asciiMapping['9'])
                    {
                        MPNumbers.Add(nextByte - _asciiMapping['0']);
                    }
                    else
                    {
                        CurrentStep = HPMPStep.None;
                    }
                    break;
                case HPMPStep.AfterMPNumberSpace:
                    if (nextByte == _asciiMapping['M'])
                        CurrentStep = HPMPStep.M;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.M:
                    if (nextByte == _asciiMapping[')'])
                        CurrentStep = HPMPStep.RightParen;
                    else
                        CurrentStep = HPMPStep.None;
                    break;
                case HPMPStep.RightParen:
                    if (nextByte == _asciiMapping[':']) //finished
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
        private Dictionary<char, int> _asciiMapping;
        private Action<SkillWithCooldownType, bool, DateTime?> _onSatisfied;

        public SkillCooldownSequence(SkillWithCooldownType skillWithCooldownType, Dictionary<char, int> asciiMapping, Action<SkillWithCooldownType, bool, DateTime?> onSatisfied)
        {
            _skillWithCooldownType = skillWithCooldownType;
            _currentStep = SkillCooldownStep.None;
            _asciiMapping = asciiMapping;
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
            _chars = new List<int>(sPattern.Length);
            foreach (char c in sPattern)
            {
                _chars.Add(_asciiMapping[c]);
            }
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
                if (nextByte == _asciiMapping['['])
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
                if (nextByte == _asciiMapping['A'] && CanBeActive())
                {
                    _currentStep = SkillCooldownStep.A;
                }
                else if (nextByte >= _asciiMapping['0'] && nextByte <= _asciiMapping['9'])
                {
                    _minutes = nextByte - _asciiMapping['0'];
                    _currentStep = SkillCooldownStep.Minute;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.Minute)
            {
                if (nextByte == _asciiMapping[':'])
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
                if (nextByte >= _asciiMapping['0'] && nextByte <= _asciiMapping['9'])
                {
                    _tensOfSeconds = nextByte - _asciiMapping['0'];
                    _currentStep = SkillCooldownStep.TensOfSeconds;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.TensOfSeconds)
            {
                if (nextByte >= _asciiMapping['0'] && nextByte <= _asciiMapping['9'])
                {
                    _seconds = nextByte - _asciiMapping['0'];
                    _currentStep = SkillCooldownStep.Seconds;
                }
                else
                {
                    _currentStep = SkillCooldownStep.None;
                }
            }
            else if (_currentStep == SkillCooldownStep.Seconds || _currentStep == SkillCooldownStep.E)
            {
                if (nextByte == _asciiMapping[']'])
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
                if (nextByte == _asciiMapping['C'])
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
                if (nextByte == _asciiMapping['T'])
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
                if (nextByte == _asciiMapping['I'])
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
                if (nextByte == _asciiMapping['V'])
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
                if (nextByte == _asciiMapping['E'])
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

    public enum SkillWithCooldownType
    {
        PowerAttack,
        Manashield,
    }
}

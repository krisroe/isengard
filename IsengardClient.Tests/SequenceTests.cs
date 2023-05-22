using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using IsengardClient;
using System;

namespace IsengardClient.Tests
{
    [TestClass]
    public class SequenceTests
    {
        [TestMethod]
        public void TestSkillCooldownSequence()
        {
            Dictionary<int, char> reverseAsciiMapping;
            Dictionary<char, int> asciiMapping = AsciiMapping.GetAsciiMapping(out reverseAsciiMapping);

            bool skillActive;
            DateTime? availableDate;
            Action<SkillWithCooldownType, bool, DateTime?> getcooldown = (type, isActive, date) =>
            {
                skillActive = isActive;
                availableDate = date;
            };

            SkillCooldownSequence scs = new SkillCooldownSequence(SkillWithCooldownType.Manashield, asciiMapping, getcooldown);

            skillActive = false;
            availableDate = DateTime.UtcNow;
            PumpSequence(scs, "manashield [ACTIVE]", asciiMapping);
            Assert.IsTrue(skillActive);
            Assert.IsTrue(!availableDate.HasValue);

            PumpSequence(scs, "manashield [2:34]", asciiMapping);
            Assert.IsFalse(skillActive);
            Assert.IsTrue(availableDate.HasValue);
        }

        [TestMethod]
        public void TestWaitXSecondsSequence()
        {
            Dictionary<int, char> reverseAsciiMapping;
            Dictionary<char, int> asciiMapping = AsciiMapping.GetAsciiMapping(out reverseAsciiMapping);

            int waited = -1;
            Action<int> waitedAction = (seconds) =>
            {
                waited = seconds;
            };

            PleaseWaitXSecondsSequence plxss = new PleaseWaitXSecondsSequence(asciiMapping, waitedAction);

            waited = -1;
            PumpSequence(plxss, "Please wait 2 seconds.", asciiMapping);
            Assert.IsTrue(waited == 2);

            waited = -1;
            PumpSequence(plxss, "Please wait 12 seconds.", asciiMapping);
            Assert.IsTrue(waited == 12);

            waited = -1;
            PumpSequence(plxss, "Please wait 1 more second.", asciiMapping);
            Assert.IsTrue(waited == 1);
        }

        public void PumpSequence(ISequence sequence, string text, Dictionary<char, int> asciiMapping)
        {
            foreach (char nextCharacter in text)
            {
                sequence.FeedByte(asciiMapping[nextCharacter]);
            }
        }
    }
}

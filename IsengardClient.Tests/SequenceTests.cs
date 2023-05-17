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
        public void TestSequences()
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

        public void PumpSequence(ISequence sequence, string text, Dictionary<char, int> asciiMapping)
        {
            foreach (char nextCharacter in text)
            {
                sequence.FeedByte(asciiMapping[nextCharacter]);
            }
        }
    }
}

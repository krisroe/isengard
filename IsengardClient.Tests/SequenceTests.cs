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
            bool skillActive;
            DateTime? availableDate;
            Action<SkillWithCooldownType, bool, DateTime?> getcooldown = (type, isActive, date) =>
            {
                skillActive = isActive;
                availableDate = date;
            };

            SkillCooldownSequence scs = new SkillCooldownSequence(SkillWithCooldownType.Manashield, getcooldown);

            skillActive = false;
            availableDate = DateTime.UtcNow;
            scs.FeedLine(new string[] { "manashield [ACTIVE]" });
            Assert.IsTrue(skillActive);
            Assert.IsTrue(!availableDate.HasValue);

            scs.FeedLine(new string[] { "manashield [2:34]" });
            Assert.IsFalse(skillActive);
            Assert.IsTrue(availableDate.HasValue);
        }

        [TestMethod]
        public void TestWaitXSecondsSequence()
        {
            int waited = -1;
            Action<int> waitedAction = (seconds) =>
            {
                waited = seconds;
            };

            PleaseWaitXSecondsSequence plxss = new PleaseWaitXSecondsSequence(waitedAction);

            waited = -1;
            plxss.FeedLine(new string[] { "Please wait 2 seconds." });
            Assert.IsTrue(waited == 2);

            waited = -1;
            plxss.FeedLine(new string[] { "Please wait 12 seconds." });
            Assert.IsTrue(waited == 12);

            waited = -1;
            plxss.FeedLine(new string[] { "Please wait 1 more second." });
            Assert.IsTrue(waited == 1);
        }
    }
}

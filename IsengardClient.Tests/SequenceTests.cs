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

        [TestMethod]
        public void TestAttackSequence()
        {
            bool success = false;
            bool fumbled = false;
            int damage = 0;
            Action<bool, int> a = (f, d) =>
            {
                success = true;
                fumbled = f;
                damage = d;
            };
            AttackSequence aseq = new AttackSequence(a);

            success = false;
            fumbled = false;
            damage = 0;
            aseq.FeedLine(new string[] { "Your slash attack hits for 10 damage." });
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);

            success = false;
            fumbled = false;
            damage = 10;
            aseq.FeedLine(new string[] { "You FUMBLED your weapon." });
            Assert.IsTrue(success);
            Assert.IsTrue(fumbled);
            Assert.AreEqual(damage, 0);
        }

        [TestMethod]
        public void TestCastOffensiveSpellSequence()
        {
            bool success = false;
            int damage = 0;
            Action<int> a = (d) =>
            {
                success = true;
                damage = d;
            };
            CastOffensiveSpellSequence cseq = new CastOffensiveSpellSequence(a);
            cseq.FeedLine(new string[] { "You cast a rumble spell on the drunk for 10 damage." });
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);
        }
    }
}

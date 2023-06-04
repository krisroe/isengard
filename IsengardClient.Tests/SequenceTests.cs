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
            scs.FeedLine(new FeedLineParameters(new List<string>() { "manashield [ACTIVE]" }, null, null));
            Assert.IsTrue(skillActive);
            Assert.IsTrue(!availableDate.HasValue);

            scs.FeedLine(new FeedLineParameters(new List<string> { "manashield [2:34]" }, null, null));
            Assert.IsFalse(skillActive);
            Assert.IsTrue(availableDate.HasValue);
        }

        [TestMethod]
        public void TestWaitXSecondsSequence()
        {
            int waited = -1;
            Action<int, FeedLineParameters> waitedAction = (seconds, flp) =>
            {
                waited = seconds;
            };

            PleaseWaitSequence plxss = new PleaseWaitSequence(waitedAction);
            FeedLineParameters oFLP = new FeedLineParameters(null, BackgroundCommandType.Quit, null);

            waited = -1;
            oFLP.Lines = new List<string>() { "Please wait 2 seconds." };
            plxss.FeedLine(oFLP);
            Assert.IsTrue(waited == 2);

            waited = -1;
            oFLP.Lines = new List<string>() { "Please wait 12 seconds." };
            plxss.FeedLine(oFLP);
            Assert.IsTrue(waited == 12);

            waited = -1;
            oFLP.Lines = new List<string>() { "Please wait 1 more second." };
            plxss.FeedLine(oFLP);
            Assert.IsTrue(waited == 1);

            waited = -1;
            oFLP.Lines = new List<string>() { "Please wait 1:05 minutes." };
            plxss.FeedLine(oFLP);
            Assert.IsTrue(waited == 65);
        }

        [TestMethod]
        public void TestAttackSequence()
        {
            bool success = false;
            bool fumbled = false;
            int damage = 0;
            Action<bool, int, FeedLineParameters> a = (f, d, flp) =>
            {
                success = true;
                fumbled = f;
                damage = d;
            };
            AttackSequence aseq = new AttackSequence(a);

            success = false;
            fumbled = false;
            damage = 0;
            aseq.FeedLine(new FeedLineParameters(new List<string>() { "Your slash attack hits for 10 damage." }, BackgroundCommandType.Attack, null));
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);

            success = false;
            fumbled = false;
            damage = 10;
            aseq.FeedLine(new FeedLineParameters(new List<string>() { "You FUMBLED your weapon." }, BackgroundCommandType.Attack, null));
            Assert.IsTrue(success);
            Assert.IsTrue(fumbled);
            Assert.AreEqual(damage, 0);
        }

        [TestMethod]
        public void TestCastOffensiveSpellSequence()
        {
            bool success = false;
            int damage = 0;
            Action<int, FeedLineParameters> a = (d, flp) =>
            {
                success = true;
                damage = d;
            };
            CastOffensiveSpellSequence cseq = new CastOffensiveSpellSequence(a);
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "You cast a rumble spell on the drunk for 10 damage." }, BackgroundCommandType.OffensiveSpell, null));
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);
        }

        [TestMethod]
        public void TestSuccessfulSearch()
        {
            List<string> exits;
            Action<List<string>, FeedLineParameters> a = (l, flp) =>
            {
                exits = l;
            };

            SuccessfulSearchSequence sseq = new SuccessfulSearchSequence(a);

            exits = null;
            sseq.FeedLine(new FeedLineParameters(new List<string>() { "You find a hidden exit: test." }, BackgroundCommandType.Search, null));
            Assert.IsTrue(exits != null);
            Assert.IsTrue(exits.Contains("test"));

            exits = null;
            sseq.FeedLine(new FeedLineParameters(new List<string>() { "You find a hidden exit: test.", "You find a hidden exit: test2." }, BackgroundCommandType.Search, null));
            Assert.IsTrue(exits != null);
            Assert.IsTrue(exits.Contains("test"));
            Assert.IsTrue(exits.Contains("test2"));
        }

        [TestMethod]
        public void TestConstantSequences()
        {
            bool satisfied;
            ConstantOutputSequence cseq;
            Action<FeedLineParameters> a = (flp) =>
            {
                satisfied = true;
            };

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.ExactMatch, null);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "test" }, null, null));
            Assert.IsTrue(satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "atest" }, null, null));
            Assert.IsTrue(!satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "testa" }, null, null));
            Assert.IsTrue(!satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.StartsWith, null);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "test" }, null, null));
            Assert.IsTrue(satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "atest" }, null, null));
            Assert.IsTrue(!satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "testa" }, null, null));
            Assert.IsTrue(satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.Contains, null);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "test" }, null, null));
            Assert.IsTrue(satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "atest" }, null, null));
            Assert.IsTrue(satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "testa" }, null, null));
            Assert.IsTrue(satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.ExactMatch, 1);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "not", "test" }, null, null));
            Assert.IsTrue(satisfied);
            satisfied = false;
            cseq.FeedLine(new FeedLineParameters(new List<string>() { "not", "other" }, null, null));
            Assert.IsTrue(!satisfied);
        }
    }
}

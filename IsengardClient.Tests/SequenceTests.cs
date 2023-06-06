using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
namespace IsengardClient.Tests
{
    [TestClass]
    public class SequenceTests
    {
        [TestMethod]
        public void TestStringProcessing()
        {
            List<string> list = new List<string>();
            List<string> output;
            int nextLineIndex;

            list.Clear();
            list.Add("Stuff: a.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex);
            Assert.IsTrue(output.Count == 1);
            Assert.IsTrue(output[0] == "a");

            list.Clear();
            list.Add("Stuff: a,b.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "b");

            list.Clear();
            list.Add("Stuff: a,");
            list.Add("b.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "b");
        }

        [TestMethod]
        public void TestScoreSequence()
        {
            List<SkillCooldown> cooldowns = null;
            List<string> spells = null;
            int iLevel = -1;
            int iMaxHP = -1;
            int iMaxMP = -1;
            Action<FeedLineParameters, int, int, int, List<SkillCooldown>, List<string>> a = (flpparam, l, hp, mp, cs, ss) =>
            {
                iLevel = l;
                iMaxHP = -1;
                iMaxMP = -1;
                cooldowns = cs;
                spells = ss;
            };

            ScoreOutputSequence sos = new ScoreOutputSequence("despug", a);

            List<string> input = new List<string>();
            FeedLineParameters flp = new FeedLineParameters(input);

            iLevel = iMaxHP = iMaxMP = -1;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 12)");
            input.Add(string.Empty);
            input.Add("   59/ 59 Hit Points     39/ 61 Magic Points    AC: 0");
            input.Add(string.Empty);
            input.Add("Skills: (power) attack [2:15], ");
            input.Add("manashield [0:00]");
            input.Add(".");
            input.Add("Spells cast: ");
            input.Add("None");
            input.Add(".");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.IsTrue(iLevel == 12);
            Assert.AreEqual(iMaxHP, 59);
            Assert.AreEqual(iMaxMP, 61);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 1);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(cooldowns[0].NextAvailable.HasValue);
            Assert.IsTrue(!cooldowns[0].Active);
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(!cooldowns[1].NextAvailable.HasValue);
            Assert.IsTrue(!cooldowns[1].Active);
            Assert.IsTrue(spells[0] == "None");

            iLevel = iMaxHP = iMaxMP = -1;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 1)");
            input.Add(string.Empty);
            input.Add("   59/159 Hit Points     39/261 Magic Points    AC: 0");
            input.Add(string.Empty);
            input.Add("Skills: (power) attack [0:00], ");
            input.Add("manashield [0:45]");
            input.Add(".");
            input.Add("Spells cast: ");
            input.Add("bless");
            input.Add(",protection.");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.IsTrue(iLevel == 1);
            Assert.IsTrue(iMaxHP == 159);
            Assert.IsTrue(iMaxMP == 261);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 2);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(!cooldowns[0].NextAvailable.HasValue);
            Assert.IsTrue(!cooldowns[0].Active);
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(cooldowns[1].NextAvailable.HasValue);
            Assert.IsTrue(!cooldowns[1].Active);
            Assert.IsTrue(spells[0] == "bless");
            Assert.IsTrue(spells[1] == "protection");

            iLevel = iMaxHP = iMaxMP = -1;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 62)");
            input.Add(string.Empty);
            input.Add("    9/  9 Hit Points      9/  9 Magic Points    AC: 0");
            input.Add(string.Empty);
            input.Add("Skills: (power) attack [12:13], manashield [ACTIVE].");
            input.Add("Spells cast: ");
            input.Add("bless");
            input.Add(",");
            input.Add("protection");
            input.Add(".");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.IsTrue(iLevel == 62);
            Assert.IsTrue(iMaxHP == 9);
            Assert.IsTrue(iMaxMP == 9);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 2);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(cooldowns[0].NextAvailable.HasValue);
            Assert.IsTrue(!cooldowns[0].Active);
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(!cooldowns[1].NextAvailable.HasValue);
            Assert.IsTrue(cooldowns[1].Active);
            Assert.IsTrue(spells[0] == "bless");
            Assert.IsTrue(spells[1] == "protection");
        }

        [TestMethod]
        public void TestTimeSequence()
        {
            int hour = -1;
            Action<FeedLineParameters, int> a = (flParams, h) =>
            {
                hour = h;
            };

            TimeOutputSequence tos = new TimeOutputSequence(a);
            List<string> info = new List<string>() { string.Empty };
            FeedLineParameters flp = new FeedLineParameters(info);

            hour = -1;
            info[0] = "Game-Time: 12 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 0);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 1 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 1);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 2 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 2);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 3 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 3);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 4 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 4);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 5 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 5);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 6 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 6);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 7 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 7);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 8 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 8);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 9 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 9);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 10 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 10);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 11 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 11);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 12 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 12);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 1 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 13);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 2 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 14);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 3 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 15);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 4 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 16);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 5 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 17);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 6 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 18);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 7 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 19);
            Assert.IsTrue(TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 8 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 20);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 9 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 21);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 10 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 22);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));

            hour = -1;
            info[0] = "Game-Time: 11 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(hour == 23);
            Assert.IsTrue(!TimeOutputSequence.IsDay(hour));
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
            FeedLineParameters oFLP = new FeedLineParameters(null);
            oFLP.BackgroundCommandType = BackgroundCommandType.Quit;

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

            FeedLineParameters flParams = new FeedLineParameters(null);
            flParams.BackgroundCommandType = BackgroundCommandType.Attack;

            success = false;
            fumbled = false;
            damage = 0;
            flParams.Lines = new List<string>() { "Your slash attack hits for 10 damage." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);

            success = false;
            fumbled = false;
            damage = 10;
            flParams.Lines = new List<string>() { "You FUMBLED your weapon." };
            aseq.FeedLine(flParams);
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
            FeedLineParameters flParams = new FeedLineParameters(new List<string>() { "You cast a rumble spell on the drunk for 10 damage." });
            flParams.BackgroundCommandType = BackgroundCommandType.OffensiveSpell;
            cseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage, 10);
        }

        [TestMethod]
        public void TestSuccessfulSearch()
        {
            List<string> exits;
            Action<List<string>, FeedLineParameters> a = (l, flParams) =>
            {
                exits = l;
            };

            SuccessfulSearchSequence sseq = new SuccessfulSearchSequence(a);

            FeedLineParameters flp = new FeedLineParameters(null);
            flp.BackgroundCommandType = BackgroundCommandType.Search;

            exits = null;
            flp.Lines = new List<string>() { "You find a hidden exit: test." };
            sseq.FeedLine(flp);
            Assert.IsTrue(exits != null);
            Assert.IsTrue(exits.Contains("test"));

            exits = null;
            flp.Lines = new List<string>() { "You find a hidden exit: test.", "You find a hidden exit: test2." };
            sseq.FeedLine(flp);
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
            FeedLineParameters flParams = new FeedLineParameters(null);

            satisfied = false;
            flParams.Lines = new List<string>() { "test" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "atest" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(!satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "testa" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(!satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.StartsWith, null);
            satisfied = false;
            flParams.Lines = new List<string>() { "test" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "atest" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(!satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "testa" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.Contains, null);
            satisfied = false;
            flParams.Lines = new List<string>() { "test" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "atest" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "testa" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.ExactMatch, 1);
            satisfied = false;
            flParams.Lines = new List<string>() { "not", "test" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(satisfied);
            satisfied = false;
            flParams.Lines = new List<string>() { "not", "other" };
            cseq.FeedLine(flParams);
            Assert.IsTrue(!satisfied);
        }
    }
}

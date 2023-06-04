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
            Action<FeedLineParameters, List<SkillCooldown>, List<string>> a = (flpparam, cs, ss) =>
            {
                cooldowns = cs;
                spells = ss;
            };

            ScoreOutputSequence sos = new ScoreOutputSequence("despug", a);

            List<string> input = new List<string>();
            FeedLineParameters flp = new FeedLineParameters(input, null, null);

            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 12)");
            input.Add("Skills: (power) attack [2:15], ");
            input.Add("manashield [0:00]");
            input.Add(".");
            input.Add("Spells cast: ");
            input.Add("None");
            input.Add(".");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
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

            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 12)");
            input.Add("Skills: (power) attack [0:00], ");
            input.Add("manashield [0:45]");
            input.Add(".");
            input.Add("Spells cast: ");
            input.Add("bless");
            input.Add(",protection.");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
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

            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 12)");
            input.Add("Skills: (power) attack [12:13], manashield [ACTIVE].");
            input.Add("Spells cast: ");
            input.Add("bless");
            input.Add(",");
            input.Add("protection");
            input.Add(".");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
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
            bool? isNight = null;
            Action<bool> a = (isn) =>
            {
                isNight = isn;
            };

            TimeOutputSequence tos = new TimeOutputSequence(a);
            List<string> info = new List<string>() { string.Empty };
            FeedLineParameters flp = new FeedLineParameters(info, null, null);

            isNight = null;
            info[0] = "Game-Time: 12 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 1 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 2 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 3 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 4 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 5 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 6 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 7 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 8 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 9 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 10 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 11 o'clock AM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 12 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 1 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 2 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 3 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 4 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 5 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 6 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 7 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == false);

            isNight = null;
            info[0] = "Game-Time: 8 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 9 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 10 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);

            isNight = null;
            info[0] = "Game-Time: 11 o'clock PM.";
            tos.FeedLine(flp);
            Assert.IsTrue(isNight == true);
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

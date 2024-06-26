﻿using IsengardClient.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;

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
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex, null);
            Assert.IsTrue(output.Count == 1);
            Assert.IsTrue(output[0] == "a");

            list.Clear();
            list.Add("Stuff: a,b.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex, null);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "b");

            list.Clear();
            list.Add("Stuff: a,");
            list.Add("  b.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex, null);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "b");

            list.Clear();
            list.Add("Stuff: a,");
            list.Add("  Mr. Wartnose.");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex, null);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "Mr. Wartnose");

            list.Clear();
            list.Add("Stuff: a,");
            list.Add("  Mr. Wartnose");
            list.Add("  .");
            output = StringProcessing.GetList(list, 0, "Stuff: ", false, out nextLineIndex, null);
            Assert.IsTrue(output.Count == 2);
            Assert.IsTrue(output[0] == "a");
            Assert.IsTrue(output[1] == "Mr. Wartnose");
        }

        [TestMethod]
        public void TestInformationalMessageProcessing()
        {
            List<string> broadcasts = null;
            List<string> addedPlayers = null;
            List<string> removedPlayers = null;
            Action<FeedLineParameters, List<string>, List<string>, List<string>> a = (flParams, s1, s2, s3) =>
            {
                broadcasts = s1;
                addedPlayers = s2;
                removedPlayers = s3;
            };
            InformationalMessagesSequence seq = new InformationalMessagesSequence("Despug", a);
            FeedLineParameters flp = new FeedLineParameters(null);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The heat today is unbearable.", "### The Celduin Express is ready for boarding in Bree." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.Lines.Count == 1); //celduin express message stays because it might stay or go depending on location

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "A hobbit just arrived." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.Lines.Count == 1);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Scranlin barely nicks you for 1 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Scranlin barely nicks you for 2 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Scranlin scratches you for 3 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Scranlin scratches you for 4 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Scranlin scratches you for 5 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit bruises you for 6 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit hurts you for 11 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit wounds you for 13 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit smites you for 16 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit maims you for 22 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit pulverizes you for 26 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit devestates you for 26 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit missed you." };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The hobbit casts a rumble spell on you for 6 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(InformationalMessages.ContainsType(flp.InfoMessages, InformationalMessageType.EnemyAttacksYou));

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Please wait 2 seconds." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].WaitSeconds == 2);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Please wait 12 seconds." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].WaitSeconds == 12);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Please wait 1 more second." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].WaitSeconds == 1);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Please wait 1:05 minutes." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].WaitSeconds == 65);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "The green slime destroys your silver scimitar." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].MessageType == InformationalMessageType.EquipmentDestroyed);
            Assert.IsTrue(flp.InfoMessages[0].Item.ItemType.Value == ItemTypeEnum.SilverScimitar);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Your gawdy ear hoop fell apart.", "Naugrim wounds you for 15 damage!" };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].MessageType == InformationalMessageType.EquipmentFellApart);
            Assert.IsTrue(flp.InfoMessages[0].Item.ItemType.Value == ItemTypeEnum.GawdyEarHoop);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Your claw gauntlet is broken." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].MessageType == InformationalMessageType.WeaponIsBroken);
            Assert.IsTrue(flp.InfoMessages[0].Item.ItemType.Value == ItemTypeEnum.ClawGauntlet);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Frodo Baggins wounds you for 13 damage!", "### The Celduin Express is ready for boarding in Bree." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].MessageType == InformationalMessageType.EnemyAttacksYou);
            Assert.IsTrue(flp.InfoMessages[1].MessageType == InformationalMessageType.CelduinExpressInBree);
            Assert.IsFalse(flp.SuppressEcho);
            Assert.IsFalse(flp.FinishedProcessing);

            flp.InfoMessages = new List<InformationalMessages>();
            broadcasts = addedPlayers = removedPlayers = null;
            flp.Lines = new List<string>() { "Your fireshield inflicts 36 damage to your attacker and dissipates!", "Dale Purves maims you for 11 damage!", "### Cleetus just logged off." };
            seq.FeedLine(flp);
            Assert.IsTrue(flp.InfoMessages[0].MessageType == InformationalMessageType.FireshieldInflictsDamageAndDissipates);
            Assert.IsTrue(flp.InfoMessages[1].MessageType == InformationalMessageType.EnemyAttacksYou);
            Assert.IsTrue(flp.Lines.Count == 2);
            Assert.IsFalse(flp.SuppressEcho);
            Assert.IsFalse(flp.FinishedProcessing);
        }

        [TestMethod]
        public void TestMobStatusSequence()
        {
            MonsterStatus? monsterStatus = null;
            Action<MonsterStatus, FeedLineParameters> a = (ms, flp) =>
            {
                monsterStatus = ms;
            };

            MobStatusSequence mss = new MobStatusSequence(a);
            FeedLineParameters flParams = new FeedLineParameters(null);
            flParams.ConsoleVerbosity = ConsoleOutputVerbosity.Default;
            flParams.RunningHiddenCommand = true;

            flParams.Lines = new List<string>()
            {
"You see Manager Mulloy.",
"He's big and tanned from working the fields.",
"    He is barely clinging to life.",
"    .. looks very agressive.",
"    .. is attacking you.",
"He is a perfect match for you!",
"Manager Mulloy pulverizes you for 28 damage!",
            };

            mss.FeedLine(flParams);
            Assert.IsTrue(monsterStatus == MonsterStatus.BarelyClingingToLife);
            Assert.IsTrue(flParams.Lines.Count == 1);
        }

        [TestMethod]
        public void TestScoreSequence()
        {
            List<SkillCooldown> cooldowns = null;
            List<SpellsEnum> spells = null;
            ClassType? classType = null;
            int iLevel = -1;
            int iMaxHP = -1;
            int iMaxMP = -1;
            decimal armorClass = -1;
            bool? armorClassIsExact = null;
            int iGold = -1;
            int iTNL = -1;
            PlayerStatusFlags? playerStatusFlags = null;
            Action<FeedLineParameters, ClassType, int, int, int, decimal, bool, int, int, List<SkillCooldown>, List<SpellsEnum>, PlayerStatusFlags> a = (flpparam, ct, l, hp, mp, ac, acexact, g, tnl, cs, ss, psf) =>
            {
                iLevel = l;
                classType = ct;
                iMaxHP = hp;
                iMaxMP = mp;
                armorClass = ac;
                armorClassIsExact = acexact;
                iGold = g;
                iTNL = tnl;
                cooldowns = cs;
                spells = ss;
                playerStatusFlags = psf;
            };

            ScoreOutputSequence sos = new ScoreOutputSequence("Despug", a);

            List<string> input = new List<string>();
            FeedLineParameters flp = new FeedLineParameters(input);

            iLevel = iMaxHP = iMaxMP = iTNL = -1;
            armorClass = -1;
            armorClassIsExact = null;
            classType = null;
            playerStatusFlags = null;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 12)");
            input.Add(" *Poisoned*");
            input.Add("   59/ 59 Hit Points     39/ 61 Magic Points    AC: 2");
            input.Add("    Gold:  376933  To Next Level:    24115 Exp         0 GP");
            input.Add("Skills: (power) attack [2:15], ");
            input.Add("  manashield [0:00]");
            input.Add("  .");
            input.Add("Spells cast: None");
            input.Add("  .");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.AreEqual(classType.Value, ClassType.Mage);
            Assert.IsTrue(iLevel == 12);
            Assert.AreEqual(iMaxHP, 59);
            Assert.AreEqual(iMaxMP, 61);
            Assert.AreEqual(armorClass, 2);
            Assert.AreEqual(armorClassIsExact, false);
            Assert.AreEqual(iGold, 376933);
            Assert.AreEqual(iTNL, 24115);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 0);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(cooldowns[0].NextAvailable != DateTime.MinValue);
            Assert.IsTrue(cooldowns[0].Status == SkillCooldownStatus.Waiting);
            Assert.IsTrue(cooldowns[0].SkillName == "(power) attack");
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(cooldowns[1].NextAvailable == DateTime.MinValue);
            Assert.IsTrue(cooldowns[1].Status == SkillCooldownStatus.Available);
            Assert.IsTrue(cooldowns[1].SkillName == "manashield");
            Assert.IsTrue(spells.Count == 0);
            Assert.IsTrue(playerStatusFlags == PlayerStatusFlags.Poisoned);

            iLevel = iMaxHP = iMaxMP = iTNL = -1;
            armorClass = -1;
            armorClassIsExact = null;
            classType = null;
            playerStatusFlags = null;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 1)");
            input.Add(string.Empty);
            input.Add("   59/159 Hit Points     39/261 Magic Points    AC: 3.3");
            input.Add("    Gold:  376933  To Next Level:        0 Exp         0 GP");
            input.Add("Skills: (power) attack [0:00], ");
            input.Add("  manashield [0:45]");
            input.Add("  .");
            input.Add("Spells cast: bless");
            input.Add("  ,protection.");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.IsTrue(classType.Value == ClassType.Mage);
            Assert.IsTrue(iLevel == 1);
            Assert.IsTrue(iMaxHP == 159);
            Assert.IsTrue(iMaxMP == 261);
            Assert.AreEqual(armorClass, 3.3M);
            Assert.AreEqual(armorClassIsExact, true);
            Assert.IsTrue(iTNL == 0);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 2);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(cooldowns[0].NextAvailable == DateTime.MinValue);
            Assert.IsTrue(cooldowns[0].Status == SkillCooldownStatus.Available);
            Assert.IsTrue(cooldowns[0].SkillName == "(power) attack");
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(cooldowns[1].NextAvailable != DateTime.MinValue);
            Assert.IsTrue(cooldowns[1].Status == SkillCooldownStatus.Waiting);
            Assert.IsTrue(cooldowns[1].SkillName == "manashield");
            Assert.IsTrue(spells.Contains(SpellsEnum.bless));
            Assert.IsTrue(spells.Contains(SpellsEnum.protection));

            iLevel = iMaxHP = iMaxMP = iTNL = -1;
            armorClass = -1;
            armorClassIsExact = null;
            classType = null;
            playerStatusFlags = null;
            input.Clear();
            input.Add("Despug the Mage Occulate (lvl 62)");
            input.Add(string.Empty);
            input.Add("    9/  9 Hit Points      9/  9 Magic Points    AC: 2.0");
            input.Add("    Gold:  376933  To Next Level:       14 Exp         0 GP");
            input.Add("Skills: (power) attack [12:13], manashield [ACTIVE].");
            input.Add("Spells cast: bless");
            input.Add("  ,");
            input.Add("  protection");
            input.Add("  .");
            cooldowns = null;
            spells = null;
            sos.FeedLine(flp);
            Assert.IsTrue(classType.Value == ClassType.Mage);
            Assert.IsTrue(iLevel == 62);
            Assert.IsTrue(iMaxHP == 9);
            Assert.IsTrue(iMaxMP == 9);
            Assert.AreEqual(armorClass, 2);
            Assert.AreEqual(armorClassIsExact, true);
            Assert.IsTrue(iTNL == 14);
            Assert.IsNotNull(cooldowns);
            Assert.IsNotNull(spells);
            Assert.IsTrue(cooldowns.Count == 2);
            Assert.IsTrue(spells.Count == 2);
            Assert.IsTrue(cooldowns[0].SkillType == SkillWithCooldownType.PowerAttack);
            Assert.IsTrue(cooldowns[0].NextAvailable != DateTime.MinValue);
            Assert.IsTrue(cooldowns[0].Status == SkillCooldownStatus.Waiting);
            Assert.IsTrue(cooldowns[0].SkillName == "(power) attack");
            Assert.IsTrue(cooldowns[1].SkillType == SkillWithCooldownType.Manashield);
            Assert.IsTrue(cooldowns[1].NextAvailable == DateTime.MinValue);
            Assert.IsTrue(cooldowns[1].Status == SkillCooldownStatus.Active);
            Assert.IsTrue(cooldowns[1].SkillName == "manashield");
            Assert.IsTrue(spells.Contains(SpellsEnum.bless));
            Assert.IsTrue(spells.Contains(SpellsEnum.protection));
        }

        [TestMethod]
        public void TestAttackSequence()
        {
            bool success = false;
            bool? fumbled = null;
            bool? killed = null;
            MobTypeEnum? mobType = null;
            int? tnl = null;
            int? damage = null;
            bool? powerAttacked = null;
            List<ItemEntity> monsterItems = null;
            Action<bool, int, bool, MobTypeEnum?, int, bool, List<ItemEntity>, FeedLineParameters> a = (f, d, k, mt, t, p, mi, flp) =>
            {
                success = true;
                fumbled = f;
                damage = d;
                killed = k;
                mobType = mt;
                tnl = t;
                powerAttacked = p;
                monsterItems = mi;
            };
            AttackSequence aseq = new AttackSequence(a);

            FeedLineParameters flParams = new FeedLineParameters(null);
            flParams.BackgroundCommandType = BackgroundCommandType.Attack;

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "Your slash attack hits for 3 damage." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 3);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, false);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "You attack the drunk.", "Your slash attack hits for 16 damage." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 16);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, false);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "Your slash attack hits for 10 damage.", "You gained 15 experience for the death of the hobbitish doctor." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 10);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, true);
            Assert.AreEqual(powerAttacked.Value, false);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "You attack the hobbitish doctor", "Your slash attack hits for 17 damage.", "You gained 15 experience for the death of the hobbitish doctor." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 17);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, true);
            Assert.AreEqual(powerAttacked.Value, false);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "Your power attack cleave hits for 10 damage." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 10);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, true);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "Your power attack cleave missed." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 0);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, true);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "Your power attack has no effect on Manager Mulloy." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 0);
            Assert.AreEqual(fumbled.Value, false);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, false);

            success = false;
            fumbled = null;
            killed = null;
            mobType = null;
            damage = null;
            monsterItems = null;
            flParams.Lines = new List<string>() { "You FUMBLED your weapon." };
            aseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.IsTrue(fumbled.Value);
            Assert.AreEqual(damage.Value, 0);
            Assert.AreEqual(killed.Value, false);
            Assert.AreEqual(powerAttacked.Value, false);
        }

        [TestMethod]
        public void TestCastOffensiveSpellSequence()
        {
            bool success = false;
            int? damage = null;
            bool? killed = null;
            MobTypeEnum? mobType = null;
            int? tnl = null;
            Action<int, bool, MobTypeEnum?, int, List<ItemEntity>, FeedLineParameters> a = (d, k, mt, t, mi, flp) =>
            {
                success = true;
                damage = d;
                killed = k;
                mobType = mt;
                tnl = t;
            };
            CastOffensiveSpellSequence cseq = new CastOffensiveSpellSequence(a);
            FeedLineParameters flParams = new FeedLineParameters(null);
            flParams.BackgroundCommandType = BackgroundCommandType.OffensiveSpell;

            success = false;
            damage = null;
            killed = null;
            mobType = null;
            flParams.Lines = new List<string>() { "You cast a rumble spell on the drunk for 10 damage." };
            cseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 10);
            Assert.AreEqual(killed.Value, false);

            success = false;
            damage = null;
            killed = null;
            mobType = null;
            flParams.Lines = new List<string>() { "You cast a rumble spell on Igor the Bouncer for 2 damage.", "You gained 130 experience for the death of Igor the Bouncer." };
            cseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 2);
            Assert.AreEqual(killed.Value, true);

            success = false;
            damage = null;
            killed = null;
            mobType = null;
            flParams.Lines = new List<string>() { "You cast a shatterstone spell on The burned remains of Nimrodel for 40", "  damage." };
            cseq.FeedLine(flParams);
            Assert.IsTrue(success);
            Assert.AreEqual(damage.Value, 40);
            Assert.AreEqual(killed.Value, false);
        }

        [TestMethod]
        public void TestSuccessfulSearch()
        {
            List<string> exits;
            Action<List<string>, FeedLineParameters> a = (l, flParams) =>
            {
                exits = l;
            };
            Action<FeedLineParameters> a2 = (flParams) =>
            {
            };

            SearchSequence sseq = new SearchSequence(a, a2);

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
        public void TestRoomTransition()
        {
            RoomTransitionInfo oRTI = null;
            int? iDamage = null;
            TrapType? trapType = null;
            List<string> broadcastMessages = null;
            List<string> addedPlayers = null;
            List<string> removedPlayers = null;
            Action<FeedLineParameters, RoomTransitionInfo, int, TrapType, List<string>, List<string>, List<string>> a = (flp, rti, d, tt, bm, ap, rp) =>
            {
                oRTI = rti;
                iDamage = d;
                trapType = tt;
                broadcastMessages = bm;
                addedPlayers = ap;
                removedPlayers = rp;
            };
            Action<FeedLineParameters, List<string>, List<string>, List<string>> a2 = (flp, l1, l2, l3) =>
            {
            };

            RoomTransitionSequence seq = new RoomTransitionSequence("Despug", a);
            InformationalMessagesSequence info = new InformationalMessagesSequence("Despug", a2);
            FeedLineParameters flParams;

            flParams = new FeedLineParameters(null);
            flParams.PlayerNames = new HashSet<string>();
            flParams.Lines = new List<string>()
            {
                string.Empty,
                "Torture Room",
                string.Empty,
                "Obvious exits: out.",
                "You see Eugene the Executioner.",
                "You see a carved ivory key.",
                string.Empty,
                "You triggered a hidden dart!",
                "You lost 10 hit points."
            };
            oRTI = null;
            iDamage = null;
            trapType = null;
            seq.FeedLine(flParams);
            Assert.IsTrue(oRTI != null);
            Assert.IsTrue(iDamage == 10);
            Assert.IsTrue(trapType.Value == TrapType.PoisonDart);

            flParams = new FeedLineParameters(null);
            flParams.PlayerNames = new HashSet<string>();
            flParams.Lines = new List<string>()
            {
                string.Empty,
                "Brethil Forest",
                string.Empty,
                "Obvious exits: south, northwest, northeast.",
                string.Empty,
                "The greater spider followed you.",
                "A greater spider just arrived.",
            };
            seq.FeedLine(flParams);
            info.FeedLine(flParams);
            Assert.IsTrue(flParams.InfoMessages.Any((im) => { return im.MessageType == InformationalMessageType.MobArrived; }));

            oRTI = null;
            iDamage = null;
            trapType = null;
            flParams = new FeedLineParameters(null);
            flParams.PlayerNames = new HashSet<string>();
            flParams.Lines = new List<string>()
            {
                "You run like a chicken.",
                string.Empty,
                "Imladris Main Street",
                string.Empty,
                "Obvious exits: east, west, north, south.",
                string.Empty
            };
            seq.FeedLine(flParams);
            Assert.IsTrue(oRTI != null);
            Assert.IsTrue(oRTI.TransitionType == RoomTransitionType.FleeWithoutDropWeapon);
        }

        [TestMethod]
        public void TestInventoryManagementSequence()
        {
            List<ItemEntity> items = null;
            ItemManagementAction? action = null;
            int? totalGold = null;
            int? soldGold = null;
            List<SpellsEnum> spells = null;
            bool? potionConsumed = null;
            PlayerStatusFlags? statusFlagsRemoved = null;
            Action<FeedLineParameters, List<ItemEntity>, ItemManagementAction, int?, int, List<SpellsEnum>, bool, PlayerStatusFlags> a = (flParams, i, act, gt, gs, sp, pot, sfr) =>
            {
                items = i;
                action = act;
                totalGold = gt;
                soldGold = gs;
                spells = sp;
                potionConsumed = pot;
                statusFlagsRemoved = sfr;
            };

            FeedLineParameters flp = new FeedLineParameters(null);
            InventoryEquipmentManagementSequence seq = new InventoryEquipmentManagementSequence(a);

            items = null;
            action = null;
            totalGold = null;
            soldGold = null;
            spells = null;
            potionConsumed = null;
            flp.Lines = new List<string>() { "You can fly!", string.Empty, "Substance consumed.", "The ice blue potion disintegrates." };
            seq.FeedLine(flp);
            Assert.IsTrue(spells != null && spells.Contains(SpellsEnum.fly));
            Assert.IsTrue(potionConsumed.Value);
            Assert.IsTrue(items.Count == 1);
            Assert.IsTrue(items[0].ItemType.Value == ItemTypeEnum.IceBluePotion);
            Assert.IsTrue(action == ItemManagementAction.ConsumeItem);

            items = null;
            action = null;
            totalGold = null;
            soldGold = null;
            spells = null;
            potionConsumed = null;
            flp.Lines = new List<string>() { "You get pot of gold.", "You now have 557420 gold pieces." };
            seq.FeedLine(flp);
            Assert.IsTrue(items.Count == 1);
            Assert.IsTrue(items[0].ItemType.Value == ItemTypeEnum.PotOfGold);
            Assert.IsTrue(action == ItemManagementAction.PickUpItem);
            Assert.IsTrue(totalGold.Value == 557420);

            items = null;
            action = null;
            totalGold = null;
            soldGold = null;
            spells = null;
            potionConsumed = null;
            flp.Lines = new List<string>() { "Oohlgrist gives you the tomb key." };
            seq.FeedLine(flp);
            Assert.IsTrue(items.Count == 1);
            Assert.IsTrue(items[0].ItemType.Value == ItemTypeEnum.TombKey);
            Assert.IsTrue(action == ItemManagementAction.Trade);
        }

        [TestMethod]
        public void TestSelfSpellCastSequence()
        {
            BackgroundCommandType? eBCT = null;
            SpellsEnum? spell = null;
            List<ItemEntity> items = null;
            Action<FeedLineParameters, BackgroundCommandType?, SpellsEnum?, List<ItemEntity>> a = (flp, bct, sp, it) =>
            {
                eBCT = bct;
                spell = sp;
                items = it;
            };

            SelfSpellCastSequence seq = new SelfSpellCastSequence(a);
            FeedLineParameters flParams = new FeedLineParameters(null);

            eBCT = null;
            spell = null;
            items = null;
            flParams.Lines = new List<string>() { "Curepoison spell cast on yourself.", "You feel much better." };
            seq.FeedLine(flParams);
            Assert.IsTrue(eBCT.Value == BackgroundCommandType.CurePoison);

            eBCT = null;
            spell = null;
            items = null;
            flParams.Lines = new List<string>() { "Protection spell cast.", "You feel watched.", string.Empty };
            seq.FeedLine(flParams);
            Assert.IsTrue(eBCT.Value == BackgroundCommandType.Protection);
            Assert.IsTrue(spell.Value == SpellsEnum.protection);
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

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.ExactMatch, null, null);
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

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.StartsWith, null, null);
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

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.Contains, null, null);
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

            cseq = new ConstantOutputSequence("test", a, ConstantSequenceMatchType.ExactMatch, 1, null);
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

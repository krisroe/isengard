using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;

namespace IsengardClient.Tests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void TestCloning()
        {
            IsengardMap gameMap = new IsengardMap(new List<string>());
            IsengardSettingData settings = IsengardSettingData.GetDefaultSettings();
            IsengardSettingData clone = new IsengardSettingData(settings);
            VerifySettingsMatch(settings, clone, false);
            settings = GenerateTestSettings(gameMap);
            clone = new IsengardSettingData(settings);
            VerifySettingsMatch(settings, clone, false);
        }

        [TestMethod]
        public void TestXMLSerialization()
        {
            IsengardMap gameMap = new IsengardMap(new List<string>());
            VerifyXMLSerializationForSettings(IsengardSettingData.GetDefaultSettings(), gameMap);
            VerifyXMLSerializationForSettings(GenerateTestSettings(gameMap), gameMap);
        }

        [TestMethod]
        public void TestDatabaseSerialization()
        {
            IsengardMap gameMap = new IsengardMap(new List<string>());
            VerifySqliteDatabaseSerializationForSettings(IsengardSettingData.GetDefaultSettings(), gameMap);
            VerifySqliteDatabaseSerializationForSettings(GenerateTestSettings(gameMap), gameMap);
        }

        internal IsengardSettingData GenerateTestSettings(IsengardMap gameMap)
        {
            IsengardSettingData settings = new IsengardSettingData();
            settings.Realm = RealmType.Fire;
            settings.PreferredAlignment = AlignmentType.Red;
            settings.QueryMonsterStatus = false;
            settings.GetNewPermRunOnBoatExitMissing = true;
            settings.RemoveAllOnStartup = false;
            settings.ConsoleVerbosity = ConsoleOutputVerbosity.Minimum;
            settings.Weapon = ItemTypeEnum.SilimaBlade;
            settings.HeldItem = ItemTypeEnum.Diamond;
            DynamicItemData did = new DynamicItemData();
            did.OverflowAction = ItemInventoryOverflowAction.Ignore;
            settings.DynamicItemData[ItemTypeEnum.Sextant] = did;
            did = new DynamicItemData();
            did.OverflowAction = ItemInventoryOverflowAction.SellOrJunk;
            settings.DynamicItemData[ItemTypeEnum.Scythe] = did;
            did = new DynamicItemData();
            did.SinkCount = 3;
            settings.DynamicItemData[ItemTypeEnum.Sapphire] = did;
            did = new DynamicItemData();
            did.OverflowAction = ItemInventoryOverflowAction.Ignore;
            settings.DynamicItemClassData[DynamicDataItemClass.Item] = did;
            did = new DynamicItemData();
            did.OverflowAction = ItemInventoryOverflowAction.SellOrJunk;
            settings.DynamicItemClassData[DynamicDataItemClass.Equipment] = did;
            did = new DynamicItemData();
            did.KeepCount = 12;
            settings.DynamicItemClassData[DynamicDataItemClass.Money] = did;

            LocationNode node1 = new LocationNode(null);
            node1.DisplayName = "asdf";
            node1.Expanded = true;
            LocationNode node2 = new LocationNode(node1);
            node2.DisplayName = "whozt";
            node2.Room = "Order of Love";
            node2.RoomObject = gameMap.UnambiguousRoomsByBackendName["Order of Love"];
            node1.Children = new List<LocationNode>() { node2 };
            settings.Locations.Add(node1);

            Area a = new Area();
            a.DisplayName = "test";
            a.PawnShop = PawnShoppe.Esgaroth;
            a.TickRoom = HealingRoom.SpindrilsCastle;
            a.InventorySinkRoomObject = gameMap.HealingRooms[HealingRoom.DeathValley];
            a.InventorySinkRoomIdentifier = a.InventorySinkRoomObject.BackendName;
            settings.Areas.Add(a);
            settings.AreasByName[a.DisplayName] = a;

            Strategy s = new Strategy();
            s.AfterKillMonsterAction = AfterKillMonsterAction.SelectFirstMonsterInRoom;
            s.AutoSpellLevelMin = 2;
            s.AutoSpellLevelMax = 3;
            s.DisplayName = "something";
            s.FinalMagicAction = FinalStepAction.Flee;
            s.FinalMeleeAction = FinalStepAction.Hazy;
            s.FinalPotionsAction = FinalStepAction.FinishCombat;
            s.MagicOnlyWhenStunnedForXMS = 100;
            s.MeleeOnlyWhenStunnedForXMS = 200;
            s.PotionsOnlyWhenStunnedForXMS = 300;
            s.TypesToRunLastCommandIndefinitely = CommandType.Magic | CommandType.Melee;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Potions;
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellLevel1, MagicStrategyStep.OffensiveSpellLevel2 };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.PowerAttack, MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.CurePoison, PotionsStrategyStep.GenericHeal, PotionsStrategyStep.MendWounds, PotionsStrategyStep.Vigor };
            settings.Strategies.Add(s);

            PermRun p = new PermRun();
            p.Area = a;
            p.AfterKillMonsterAction = AfterKillMonsterAction.SelectFirstMonsterInRoomOfSameType;
            p.AutoSpellLevelMin = 3;
            p.AutoSpellLevelMax = 4;
            p.DisplayName = "asfdsdaf";
            p.BeforeFull = FullType.Total;
            p.AfterFull = FullType.Almost;
            p.ItemsToProcessType = ItemsToProcessType.ProcessMonsterDrops;
            p.MobIndex = 2;
            p.MobType = MobTypeEnum.Amlug;
            p.OrderValue = 12;
            p.UseMagicCombat = true;
            p.UseMeleeCombat = false;
            p.UsePotionsCombat = null;
            p.Strategy = s;
            p.TargetRoomObject = gameMap.HealingRooms[HealingRoom.BreeArena];
            p.TargetRoomIdentifier = p.TargetRoomObject.BackendName;
            p.ThresholdRoomObject = gameMap.HealingRooms[HealingRoom.BreeNortheast];
            p.ThresholdRoomIdentifier = p.ThresholdRoomObject.BackendName;
            settings.PermRuns.Add(p);

            return settings;
        }

        internal void VerifyXMLSerializationForSettings(IsengardSettingData settings, IsengardMap gameMap)
        {
            List<string> errorMessages = new List<string>();
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = " ";
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, xmlSettings))
            {
                settings.SaveToXmlWriter(xmlWriter);
            }
            IsengardSettingData sets2 = new IsengardSettingData(sb.ToString(), errorMessages, false, gameMap);
            Assert.AreEqual(errorMessages.Count, 0);
            VerifySettingsMatch(settings, sets2, false);
        }

        internal void VerifySqliteDatabaseSerializationForSettings(IsengardSettingData settings, IsengardMap gameMap)
        {
            List<string> errorMessages = new List<string>();

            //save to SQLite database and back
            string sSQLiteDatabaseFileName = Path.Combine(Path.GetTempPath(), "isengard.sqlite");
            if (File.Exists(sSQLiteDatabaseFileName)) File.Delete(sSQLiteDatabaseFileName);

            IsengardSettingData sets4;
            SQLiteConnection.CreateFile(sSQLiteDatabaseFileName);
            using (var conn = IsengardSettingData.GetSqliteConnection(sSQLiteDatabaseFileName))
            {
                conn.Open();
                IsengardSettingData.CreateNewDatabaseSchema(conn);
                int iUserID = IsengardSettingData.GetUserID(conn, "Despug", true);
                settings.SaveSettings(conn, iUserID);
                sets4 = new IsengardSettingData(conn, iUserID, errorMessages, gameMap);
            }
            Assert.AreEqual(errorMessages.Count, 0);
            VerifySettingsMatch(settings, sets4, true);
        }

        internal void VerifySettingsMatch(IsengardSettingData settings, IsengardSettingData sets2, bool expectIDsPopulated)
        {
            Assert.AreEqual(settings.Realm, sets2.Realm);
            Assert.AreEqual(settings.PreferredAlignment, sets2.PreferredAlignment);
            Assert.AreEqual(settings.QueryMonsterStatus, sets2.QueryMonsterStatus);
            Assert.AreEqual(settings.GetNewPermRunOnBoatExitMissing, sets2.GetNewPermRunOnBoatExitMissing);
            Assert.AreEqual(settings.RemoveAllOnStartup, sets2.RemoveAllOnStartup);
            Assert.AreEqual(settings.ConsoleVerbosity, sets2.ConsoleVerbosity);
            Assert.AreEqual(settings.Weapon, sets2.Weapon);
            Assert.AreEqual(settings.HeldItem, sets2.HeldItem);
            Assert.AreEqual(settings.DynamicItemData.Count, sets2.DynamicItemData.Count);
            foreach (var next in settings.DynamicItemData)
            {
                Assert.AreEqual(settings.DynamicItemData[next.Key].KeepCount, sets2.DynamicItemData[next.Key].KeepCount);
                Assert.AreEqual(settings.DynamicItemData[next.Key].SinkCount, sets2.DynamicItemData[next.Key].SinkCount);
                Assert.AreEqual(settings.DynamicItemData[next.Key].OverflowAction, sets2.DynamicItemData[next.Key].OverflowAction);
            }
            Assert.AreEqual(settings.DynamicItemClassData.Count, sets2.DynamicItemClassData.Count);
            foreach (var next in settings.DynamicItemClassData)
            {
                Assert.AreEqual(settings.DynamicItemClassData[next.Key].KeepCount, sets2.DynamicItemClassData[next.Key].KeepCount);
                Assert.AreEqual(settings.DynamicItemClassData[next.Key].SinkCount, sets2.DynamicItemClassData[next.Key].SinkCount);
                Assert.AreEqual(settings.DynamicItemClassData[next.Key].OverflowAction, sets2.DynamicItemClassData[next.Key].OverflowAction);
            }
            Assert.AreEqual(settings.Areas.Count, sets2.Areas.Count);
            for (int i = 0; i < settings.Areas.Count; i++)
            {
                VerifyAreasMatch(settings.Areas[i], sets2.Areas[i], expectIDsPopulated);
            }
            Assert.AreEqual(settings.Locations.Count, sets2.Locations.Count);
            for (int i = 0; i < settings.Locations.Count; i++)
            {
                VerifyLocationsMatch(settings.Locations[i], sets2.Locations[i], expectIDsPopulated);
            }
            Assert.AreEqual(settings.Strategies.Count, sets2.Strategies.Count);
            for (int i = 0; i < settings.Strategies.Count; i++)
            {
                VerifyStrategiesMatch(settings.Strategies[i], sets2.Strategies[i], expectIDsPopulated);
            }
            Assert.AreEqual(settings.PermRuns.Count, sets2.PermRuns.Count);
            for (int i = 0; i < settings.PermRuns.Count; i++)
            {
                VerifyPermRunsMatch(settings.PermRuns[i], sets2.PermRuns[i], expectIDsPopulated);
            }
        }

        internal void VerifyAreasMatch(Area a1, Area a2, bool expectIDsPopulated)
        {
            Assert.AreEqual(a1.ID, a2.ID);
            if (expectIDsPopulated)
                Assert.AreNotEqual(a1.ID, 0);
            else
                Assert.AreEqual(a1.ID, 0);
            Assert.AreEqual(a1.DisplayName, a2.DisplayName);
            Assert.AreEqual(a1.TickRoom, a2.TickRoom);
            Assert.AreEqual(a1.PawnShop, a2.PawnShop);
            Assert.AreEqual(a1.InventorySinkRoomIdentifier ?? string.Empty, a2.InventorySinkRoomIdentifier ?? string.Empty);
            Assert.AreEqual(a1.InventorySinkRoomObject, a2.InventorySinkRoomObject);
        }

        internal void VerifyPermRunsMatch(PermRun p1, PermRun p2, bool expectIDsPopulated)
        {
            Assert.AreEqual(p1.ID, p2.ID);
            if (expectIDsPopulated)
                Assert.AreNotEqual(p1.ID, 0);
            else
                Assert.AreEqual(p1.ID, 0);
            Assert.AreEqual(p1.DisplayName ?? string.Empty, p2.DisplayName ?? string.Empty);
            VerifyAreasMatch(p1.Area, p2.Area, expectIDsPopulated);
            Assert.AreEqual(p1.BeforeFull, p2.BeforeFull);
            Assert.AreEqual(p1.AfterFull, p2.AfterFull);
            Assert.AreEqual(p1.SpellsToCast, p2.SpellsToCast);
            Assert.AreEqual(p1.SpellsToPotion, p2.SpellsToPotion);
            Assert.AreEqual(p1.SkillsToRun, p2.SkillsToRun);
            Assert.AreEqual(p1.TargetRoomIdentifier, p2.TargetRoomIdentifier);
            Assert.AreEqual(p1.TargetRoomObject, p2.TargetRoomObject);
            Assert.AreEqual(p1.ThresholdRoomIdentifier, p2.ThresholdRoomIdentifier);
            Assert.AreEqual(p1.ThresholdRoomObject, p2.ThresholdRoomObject);
            Assert.AreEqual(p1.MobType, p2.MobType);
            Assert.AreEqual(p1.MobText, p2.MobText);
            Assert.AreEqual(p1.MobIndex, p2.MobIndex);
            Assert.AreEqual(p1.UseMagicCombat, p2.UseMagicCombat);
            Assert.AreEqual(p1.UseMeleeCombat, p2.UseMeleeCombat);
            Assert.AreEqual(p1.UsePotionsCombat, p2.UsePotionsCombat);
            Assert.AreEqual(p1.AfterKillMonsterAction, p2.AfterKillMonsterAction);
            Assert.AreEqual(p1.AutoSpellLevelMin, p2.AutoSpellLevelMin);
            Assert.AreEqual(p1.AutoSpellLevelMax, p2.AutoSpellLevelMax);
            Assert.AreEqual(p1.ItemsToProcessType, p2.ItemsToProcessType);
            VerifyStrategiesMatch(p1.Strategy, p2.Strategy, expectIDsPopulated);
        }

        internal void VerifyStrategiesMatch(Strategy s1, Strategy s2, bool expectIDsPopulated)
        {
            Assert.AreEqual(s1.ID, s2.ID);
            if (expectIDsPopulated)
                Assert.AreNotEqual(s1.ID, 0);
            else
                Assert.AreEqual(s1.ID, 0);
            Assert.AreEqual(s1.DisplayName ?? string.Empty, s2.DisplayName ?? string.Empty);
            Assert.AreEqual(s1.AfterKillMonsterAction, s2.AfterKillMonsterAction);
            Assert.AreEqual(s1.ManaPool, s2.ManaPool);
            Assert.AreEqual(s1.FinalMagicAction, s2.FinalMagicAction);
            Assert.AreEqual(s1.FinalMeleeAction, s2.FinalMeleeAction);
            Assert.AreEqual(s1.FinalPotionsAction, s2.FinalPotionsAction);
            Assert.AreEqual(s1.MagicOnlyWhenStunnedForXMS, s2.MagicOnlyWhenStunnedForXMS);
            Assert.AreEqual(s1.MeleeOnlyWhenStunnedForXMS, s2.MeleeOnlyWhenStunnedForXMS);
            Assert.AreEqual(s1.PotionsOnlyWhenStunnedForXMS, s2.PotionsOnlyWhenStunnedForXMS);
            Assert.AreEqual(s1.TypesToRunLastCommandIndefinitely, s2.TypesToRunLastCommandIndefinitely);
            Assert.AreEqual(s1.TypesWithStepsEnabled, s2.TypesWithStepsEnabled);
            Assert.AreEqual(s1.AutoSpellLevelMin, s2.AutoSpellLevelMin);
            Assert.AreEqual(s1.AutoSpellLevelMax, s2.AutoSpellLevelMax);
            Assert.AreEqual(s1.MagicSteps == null, s2.MagicSteps == null);
            Assert.AreEqual(s1.MeleeSteps == null, s2.MeleeSteps == null);
            Assert.AreEqual(s1.PotionsSteps == null, s2.PotionsSteps == null);
            if (s1.MagicSteps != null)
            {
                Assert.AreEqual(s1.MagicSteps.Count, s2.MagicSteps.Count);
                for (int i = 0; i < s1.MagicSteps.Count; i++)
                {
                    Assert.AreEqual(s1.MagicSteps[i], s2.MagicSteps[i]);
                }
            }
            if (s1.MeleeSteps != null)
            {
                Assert.AreEqual(s1.MeleeSteps.Count, s2.MeleeSteps.Count);
                for (int i = 0; i < s1.MeleeSteps.Count; i++)
                {
                    Assert.AreEqual(s1.MeleeSteps[i], s2.MeleeSteps[i]);
                }
            }
            if (s1.PotionsSteps != null)
            {
                Assert.AreEqual(s1.PotionsSteps.Count, s2.PotionsSteps.Count);
                for (int i = 0; i < s1.PotionsSteps.Count; i++)
                {
                    Assert.AreEqual(s1.PotionsSteps[i], s2.PotionsSteps[i]);
                }
            }
        }

        internal void VerifyLocationsMatch(LocationNode ln1, LocationNode ln2, bool expectIDsPopulated)
        {
            Assert.AreEqual(ln1.ID, ln2.ID);
            if (expectIDsPopulated)
                Assert.AreNotEqual(ln1.ID, 0);
            else
                Assert.AreEqual(ln1.ID, 0);
            Assert.AreEqual(ln1.DisplayName, ln2.DisplayName);
            Assert.AreEqual(ln1.Room ?? string.Empty, ln2.Room ?? string.Empty);
            Assert.AreEqual(ln1.RoomObject, ln2.RoomObject);
            Assert.AreEqual(ln1.Expanded, ln2.Expanded);
            Assert.AreEqual(ln1.Children == null, ln2.Children == null);
            if (ln1.Children != null)
            {
                Assert.AreEqual(ln1.Children.Count, ln2.Children.Count);
                for (int i = 0; i < ln1.Children.Count; i++)
                {
                    VerifyLocationsMatch(ln1.Children[0], ln2.Children[0], expectIDsPopulated);
                }
            }
        }
    }
}

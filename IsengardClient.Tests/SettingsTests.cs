using IsengardClient.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            settings.Realms = RealmTypeFlags.Fire | RealmTypeFlags.Wind;
            settings.PreferredAlignment = AlignmentType.Red;
            settings.QueryMonsterStatus = false;
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

            Area a = new Area(null);
            a.DisplayName = "test";
            a.PawnShop = PawnShoppe.Esgaroth;
            a.TickRoom = HealingRoom.SpindrilsCastle;
            a.InventorySinkRoomObject = gameMap.HealingRooms[HealingRoom.DeathValley];
            a.InventorySinkRoomIdentifier = a.InventorySinkRoomObject.BackendName;
            settings.HomeArea = a;

            Strategy s = new Strategy();
            s.AfterKillMonsterAction = AfterKillMonsterAction.SelectFirstMonsterInRoom;
            s.AutoSpellLevelMin = 2;
            s.AutoSpellLevelMax = 3;
            s.Realms = RealmTypeFlags.Earth | RealmTypeFlags.Wind;
            s.DisplayName = "something";
            s.FinalMagicAction = FinalStepAction.Flee;
            s.FinalMeleeAction = FinalStepAction.Hazy;
            s.FinalPotionsAction = FinalStepAction.FinishCombat;
            s.MagicOnlyWhenStunnedForXMS = 100;
            s.MeleeOnlyWhenStunnedForXMS = 200;
            s.PotionsOnlyWhenStunnedForXMS = 300;
            s.MagicLastCommandsToRunIndefinitely = 2;
            s.MeleeLastCommandsToRunIndefinitely = 1;
            s.TypesWithStepsEnabled = CommandType.Melee | CommandType.Potions;
            s.MagicSteps = new List<MagicStrategyStep>() { MagicStrategyStep.OffensiveSpellLevel1, MagicStrategyStep.OffensiveSpellLevel2 };
            s.MeleeSteps = new List<MeleeStrategyStep>() { MeleeStrategyStep.PowerAttack, MeleeStrategyStep.RegularAttack };
            s.PotionsSteps = new List<PotionsStrategyStep>() { PotionsStrategyStep.CurePoison, PotionsStrategyStep.GenericHeal, PotionsStrategyStep.MendWounds, PotionsStrategyStep.Vigor };
            settings.Strategies.Add(s);

            DynamicMobData dmd = new DynamicMobData();
            dmd.StrategyOverrides.AfterKillMonsterAction = AfterKillMonsterAction.SelectFirstMonsterInRoom;
            dmd.StrategyOverrides.Realms = RealmTypeFlags.Earth | RealmTypeFlags.Wind;
            dmd.StrategyOverrides.UseMagicCombat = true;
            dmd.StrategyOverrides.UseMeleeCombat = true;
            dmd.StrategyOverrides.UsePotionsCombat = false;
            dmd.Strategy = s;
            settings.DynamicMobData[MobTypeEnum.Accuser] = dmd;

            PermRun p = new PermRun();
            p.Rehome = false;
            p.Areas = new HashSet<Area>() { a };
            p.StrategyOverrides.AfterKillMonsterAction = AfterKillMonsterAction.SelectFirstMonsterInRoomOfSameType;
            p.StrategyOverrides.AutoSpellLevelMin = 3;
            p.StrategyOverrides.AutoSpellLevelMax = 4;
            p.StrategyOverrides.Realms = RealmTypeFlags.Fire;
            p.StrategyOverrides.UseMagicCombat = true;
            p.StrategyOverrides.UseMeleeCombat = false;
            p.StrategyOverrides.UsePotionsCombat = null;
            p.Strategy = s;
            p.DisplayName = "asfdsdaf";
            p.BeforeFull = FullType.Total;
            p.AfterFull = FullType.Almost;
            p.ItemsToProcessType = ItemsToProcessType.ProcessMonsterDrops;
            p.SkillsToRun = PromptedSkills.Fireshield | PromptedSkills.Manashield;
            p.RemoveAllEquipment = true;
            p.SupportedKeys = SupportedKeysFlags.BoilerKey | SupportedKeysFlags.BridgeKey;
            p.LastCompleted = DateTime.UtcNow;
            p.MobIndex = 2;
            p.MobType = MobTypeEnum.Amlug;
            p.OrderValue = 12;
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
                int iUserID = IsengardSettingData.GetUserID(conn, "TestUser", true);
                settings.SaveSettings(conn, iUserID);
                sets4 = new IsengardSettingData(conn, iUserID, errorMessages, gameMap);
            }
            Assert.AreEqual(errorMessages.Count, 0);
            VerifySettingsMatch(settings, sets4, true);
        }

        internal void VerifySettingsMatch(IsengardSettingData settings, IsengardSettingData sets2, bool expectIDsPopulated)
        {
            Assert.AreEqual(settings.Realms, sets2.Realms);
            Assert.AreEqual(settings.PreferredAlignment, sets2.PreferredAlignment);
            Assert.AreEqual(settings.QueryMonsterStatus, sets2.QueryMonsterStatus);
            Assert.AreEqual(settings.RemoveAllOnStartup, sets2.RemoveAllOnStartup);
            Assert.AreEqual(settings.ConsoleVerbosity, sets2.ConsoleVerbosity);
            Assert.AreEqual(settings.Weapon, sets2.Weapon);
            Assert.AreEqual(settings.HeldItem, sets2.HeldItem);
            Assert.AreEqual(settings.DisplayStunLength, sets2.DisplayStunLength);
            Assert.AreEqual(settings.CommandTimeoutSeconds, sets2.CommandTimeoutSeconds);
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
            Assert.AreEqual(settings.DynamicMobData.Count, sets2.DynamicMobData.Count);
            foreach (var next in settings.DynamicMobData)
            {
                Assert.AreEqual(settings.DynamicMobData[next.Key].StrategyID, sets2.DynamicMobData[next.Key].StrategyID);
                VerifyStrategiesMatch(settings.DynamicMobData[next.Key].Strategy, sets2.DynamicMobData[next.Key].Strategy, expectIDsPopulated);
                VerifyStrategyOverridesMatch(settings.DynamicMobData[next.Key].StrategyOverrides, sets2.DynamicMobData[next.Key].StrategyOverrides);
            }
            Assert.AreEqual(settings.HomeArea == null, sets2.HomeArea == null);
            if (settings.HomeArea != null)
            {
                VerifyAreasMatch(settings.HomeArea, sets2.HomeArea, expectIDsPopulated);
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
            VerifyAreaPropertiesMatch(a1, a2);
            if (expectIDsPopulated)
                Assert.AreNotEqual(a1.ID, 0);
            else
                Assert.AreEqual(a1.ID, 0);
            Assert.AreEqual(a1.Parent == null, a2.Parent == null);
            Assert.AreEqual(a1.ParentID, a2.ParentID);
            if (expectIDsPopulated && a1.Parent != null)
                Assert.AreNotEqual(a1.ParentID, 0);
            else
                Assert.AreEqual(a1.ParentID, 0);
            if (a1.Parent != null) VerifyAreaPropertiesMatch(a1.Parent, a2.Parent);
            Assert.AreEqual(a1.Children == null, a2.Children == null);
            if (a1.Children != null)
            {
                for (int i = 0; i < a1.Children.Count; i++)
                {
                    VerifyAreasMatch(a1.Children[i], a2.Children[i], expectIDsPopulated);
                }
            }
        }

        internal void VerifyAreaPropertiesMatch(Area a1, Area a2)
        {
            Assert.AreEqual(a1.ID, a2.ID);
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
            Assert.AreEqual(p1.Rehome, p2.Rehome);
            Assert.AreEqual(p1.Areas == null, p2.Areas == null);
            if (p1.Areas != null)
            {
                Assert.AreEqual(p1.Areas.Count, p2.Areas.Count);
                foreach (Area a1 in p1.Areas)
                {
                    bool verified = false;
                    foreach (Area a2 in p2.Areas)
                    {
                        if (a1.DisplayName == a2.DisplayName)
                        {
                            VerifyAreasMatch(a1, a2, expectIDsPopulated);
                            verified = true;
                            break;
                        }
                    }
                    if (!verified)
                    {
                        Assert.Fail("Uncorresponding area found.");
                    }
                }
            }
            Assert.AreEqual(p1.BeforeFull, p2.BeforeFull);
            Assert.AreEqual(p1.AfterFull, p2.AfterFull);
            Assert.AreEqual(p1.SpellsToCast, p2.SpellsToCast);
            Assert.AreEqual(p1.SpellsToPotion, p2.SpellsToPotion);
            Assert.AreEqual(p1.SkillsToRun, p2.SkillsToRun);
            Assert.AreEqual(p1.RemoveAllEquipment, p2.RemoveAllEquipment);
            Assert.AreEqual(p1.SupportedKeys, p2.SupportedKeys);
            Assert.AreEqual(p1.TargetRoomIdentifier, p2.TargetRoomIdentifier);
            Assert.AreEqual(p1.TargetRoomObject, p2.TargetRoomObject);
            Assert.AreEqual(p1.ThresholdRoomIdentifier, p2.ThresholdRoomIdentifier);
            Assert.AreEqual(p1.ThresholdRoomObject, p2.ThresholdRoomObject);
            Assert.AreEqual(p1.MobType, p2.MobType);
            Assert.AreEqual(p1.MobText, p2.MobText);
            Assert.AreEqual(p1.MobIndex, p2.MobIndex);
            VerifyStrategiesMatch(p1.Strategy, p2.Strategy, expectIDsPopulated);
            VerifyStrategyOverridesMatch(p1.StrategyOverrides, p2.StrategyOverrides);
            Assert.AreEqual(p1.ItemsToProcessType, p2.ItemsToProcessType);
            Assert.AreEqual(p1.LastCompleted, p2.LastCompleted);
        }

        internal void VerifyStrategyOverridesMatch(StrategyOverrides s1, StrategyOverrides s2)
        {
            Assert.AreEqual(s1.UseMagicCombat, s2.UseMagicCombat);
            Assert.AreEqual(s1.UseMeleeCombat, s2.UseMeleeCombat);
            Assert.AreEqual(s1.UsePotionsCombat, s2.UsePotionsCombat);
            Assert.AreEqual(s1.AfterKillMonsterAction, s2.AfterKillMonsterAction);
            Assert.AreEqual(s1.AutoSpellLevelMin, s2.AutoSpellLevelMin);
            Assert.AreEqual(s1.AutoSpellLevelMax, s2.AutoSpellLevelMax);
            Assert.AreEqual(s1.Realms, s2.Realms);
        }

        internal void VerifyStrategiesMatch(Strategy s1, Strategy s2, bool expectIDsPopulated)
        {
            Assert.AreEqual(s1 == null, s2 == null);
            if (s1 != null)
            {
                Assert.AreEqual(s1.ID, s2.ID);
                if (expectIDsPopulated)
                    Assert.AreNotEqual(s1.ID, 0);
                else
                    Assert.AreEqual(s1.ID, 0);
                Assert.AreEqual(s1.DisplayName ?? string.Empty, s2.DisplayName ?? string.Empty);
                Assert.AreEqual(s1.AfterKillMonsterAction, s2.AfterKillMonsterAction);
                Assert.AreEqual(s1.ManaPool, s2.ManaPool);
                Assert.AreEqual(s1.IsDefault, s2.IsDefault);
                Assert.AreEqual(s1.FinalMagicAction, s2.FinalMagicAction);
                Assert.AreEqual(s1.FinalMeleeAction, s2.FinalMeleeAction);
                Assert.AreEqual(s1.FinalPotionsAction, s2.FinalPotionsAction);
                Assert.AreEqual(s1.MagicOnlyWhenStunnedForXMS, s2.MagicOnlyWhenStunnedForXMS);
                Assert.AreEqual(s1.MeleeOnlyWhenStunnedForXMS, s2.MeleeOnlyWhenStunnedForXMS);
                Assert.AreEqual(s1.PotionsOnlyWhenStunnedForXMS, s2.PotionsOnlyWhenStunnedForXMS);
                Assert.AreEqual(s1.MagicLastCommandsToRunIndefinitely, s2.MagicLastCommandsToRunIndefinitely);
                Assert.AreEqual(s1.MeleeLastCommandsToRunIndefinitely, s2.MeleeLastCommandsToRunIndefinitely);
                Assert.AreEqual(s1.PotionsLastCommandsToRunIndefinitely, s2.PotionsLastCommandsToRunIndefinitely);
                Assert.AreEqual(s1.TypesWithStepsEnabled, s2.TypesWithStepsEnabled);
                Assert.AreEqual(s1.AutoSpellLevelMin, s2.AutoSpellLevelMin);
                Assert.AreEqual(s1.AutoSpellLevelMax, s2.AutoSpellLevelMax);
                Assert.AreEqual(s1.Realms, s2.Realms);
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

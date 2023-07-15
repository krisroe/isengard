using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Data.SQLite;
using System.Data;
using System.Text;
namespace IsengardClient.Backend
{
    public class IsengardSettingData
    {
        public const int AUTO_SPELL_LEVEL_MINIMUM = 1;
        public const int AUTO_SPELL_LEVEL_MAXIMUM = 5;
        public const int AUTO_SPELL_LEVEL_NOT_SET = 0;

        public ItemTypeEnum? Weapon { get; set; }
        public ItemTypeEnum? HeldItem { get; set; }
        public RealmTypeFlags Realms { get; set; }
        public AlignmentType PreferredAlignment { get; set; }
        public ConsoleOutputVerbosity ConsoleVerbosity { get; set; }
        public bool QueryMonsterStatus { get; set; }
        public Color FullColor { get; set; }
        public Color EmptyColor { get; set; }
        public int AutoSpellLevelMin { get; set; }
        public int AutoSpellLevelMax { get; set; }
        public int AutoEscapeThreshold { get; set; }
        public AutoEscapeType AutoEscapeType { get; set; }
        public bool AutoEscapeActive { get; set; }
        public bool RemoveAllOnStartup { get; set; }
        public bool DisplayStunLength { get; set; }
        public int MagicVigorOnlyWhenDownXHP { get; set; }
        public int MagicMendOnlyWhenDownXHP { get; set; }
        public int PotionsVigorOnlyWhenDownXHP { get; set; }
        public int PotionsMendOnlyWhenDownXHP { get; set; }
        public bool SaveSettingsOnQuit { get; set; }
        public Dictionary<ItemTypeEnum, DynamicItemData> DynamicItemData { get; set; }
        public Dictionary<DynamicDataItemClass, DynamicItemData> DynamicItemClassData { get; set; }
        public List<LocationNode> Locations { get; set; }
        public List<Strategy> Strategies { get; set; }
        public List<PermRun> PermRuns { get; set; }
        public Area HomeArea { get; set; }
        public IsengardSettingData()
        {
            Weapon = null;
            HeldItem = null;
            Realms = RealmTypeFlags.Earth;
            PreferredAlignment = AlignmentType.Blue;
            ConsoleVerbosity = ConsoleOutputVerbosity.Default;
            FullColor = Color.Green;
            EmptyColor = Color.Red;
            QueryMonsterStatus = true;
            AutoSpellLevelMin = AUTO_SPELL_LEVEL_MINIMUM;
            AutoSpellLevelMax = AUTO_SPELL_LEVEL_MAXIMUM;
            RemoveAllOnStartup = true;
            DisplayStunLength = false;
            AutoEscapeThreshold = 0;
            AutoEscapeType = AutoEscapeType.Flee;
            AutoEscapeActive = false;
            MagicVigorOnlyWhenDownXHP = 6;
            MagicMendOnlyWhenDownXHP = 12;
            PotionsVigorOnlyWhenDownXHP = 6;
            PotionsMendOnlyWhenDownXHP = 12;
            SaveSettingsOnQuit = true;
            Strategies = new List<Strategy>();
            PermRuns = new List<PermRun>();
            DynamicItemData = new Dictionary<ItemTypeEnum, DynamicItemData>();
            DynamicItemClassData = new Dictionary<DynamicDataItemClass, DynamicItemData>();
            Locations = new List<LocationNode>();
            HomeArea = null;
        }
        public IsengardSettingData(IsengardSettingData copied)
        {
            Weapon = copied.Weapon;
            HeldItem = copied.HeldItem;
            Realms = copied.Realms;
            PreferredAlignment = copied.PreferredAlignment;
            ConsoleVerbosity = copied.ConsoleVerbosity;
            QueryMonsterStatus = copied.QueryMonsterStatus;
            FullColor = copied.FullColor;
            EmptyColor = copied.EmptyColor;
            AutoSpellLevelMin = copied.AutoSpellLevelMin;
            AutoSpellLevelMax = copied.AutoSpellLevelMax;
            AutoEscapeThreshold = copied.AutoEscapeThreshold;
            AutoEscapeType = copied.AutoEscapeType;
            AutoEscapeActive = copied.AutoEscapeActive;
            RemoveAllOnStartup = copied.RemoveAllOnStartup;
            DisplayStunLength = copied.DisplayStunLength;
            MagicVigorOnlyWhenDownXHP = copied.MagicVigorOnlyWhenDownXHP;
            MagicMendOnlyWhenDownXHP = copied.MagicMendOnlyWhenDownXHP;
            PotionsVigorOnlyWhenDownXHP = copied.PotionsVigorOnlyWhenDownXHP;
            PotionsMendOnlyWhenDownXHP = copied.PotionsMendOnlyWhenDownXHP;
            SaveSettingsOnQuit = copied.SaveSettingsOnQuit;
            DynamicItemData = new Dictionary<ItemTypeEnum, DynamicItemData>();
            foreach (var next in copied.DynamicItemData)
            {
                DynamicItemData[next.Key] = new DynamicItemData(next.Value);
            }
            DynamicItemClassData = new Dictionary<DynamicDataItemClass, DynamicItemData>();
            foreach (var next in copied.DynamicItemClassData)
            {
                DynamicItemClassData[next.Key] = new DynamicItemData(next.Value);
            }
            Dictionary<Area, Area> areaMapping = new Dictionary<Area, Area>();
            HomeArea = new Area(copied.HomeArea, null, areaMapping);
            Locations = new List<LocationNode>();
            foreach (LocationNode next in copied.Locations)
            {
                Locations.Add(new LocationNode(next, null));
            }
            Dictionary<Strategy, Strategy> oStrategyMapping = new Dictionary<Strategy, Strategy>();
            Strategies = new List<Strategy>();
            foreach (Strategy s in copied.Strategies)
            {
                Strategy copyStrategy = new Strategy(s);
                oStrategyMapping[s] = copyStrategy;
                Strategies.Add(copyStrategy);
            }
            PermRuns = new List<PermRun>();
            foreach (PermRun p in copied.PermRuns)
            {
                PermRun copyPermRun = new PermRun(p);
                copyPermRun.Strategy = oStrategyMapping[copyPermRun.Strategy];
                if (copyPermRun.Areas != null)
                {
                    copyPermRun.Areas.Clear();
                    foreach (Area a in p.Areas)
                    {
                        copyPermRun.Areas.Add(areaMapping[a]);
                    }
                }
                PermRuns.Add(copyPermRun);
            }
        }
        public IsengardSettingData(SQLiteConnection conn, int UserID, List<string> errorMessages, IsengardMap gameMap) : this()
        {
            object oData;
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.CommandText = "SELECT SettingName,SettingValue FROM Settings WHERE UserID = @UserID";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HandleSetting(reader["SettingName"].ToString(), reader["SettingValue"].ToString(), errorMessages);
                    }
                }

                cmd.CommandText = "SELECT Key,KeepCount,SinkCount,OverflowAction FROM DynamicItemData WHERE UserID = @UserID";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string key = reader["Key"].ToString();
                        ItemTypeEnum itemType;
                        DynamicDataItemClass itemClass = DynamicDataItemClass.Item;
                        bool isItemType = false;
                        bool isItemClass = false;
                        if (Enum.TryParse(key, out itemType))
                        {
                            isItemType = true;
                        }
                        else if (Enum.TryParse(key, out itemClass))
                        {
                            isItemClass = true;
                        }
                        else
                        {
                            errorMessages.Add("Invalid dynamic data key for dynamic data: " + key);
                            continue;
                        }

                        DynamicItemData did = new DynamicItemData();
                        oData = reader["KeepCount"];
                        did.KeepCount = oData == DBNull.Value ? -1 : Convert.ToInt32(oData);
                        oData = reader["SinkCount"];
                        did.SinkCount = oData == DBNull.Value ? -1 : Convert.ToInt32(oData);
                        oData = reader["OverflowAction"];
                        did.OverflowAction = oData == DBNull.Value ? ItemInventoryOverflowAction.None : (ItemInventoryOverflowAction)Convert.ToInt32(oData);

                        if (isItemType)
                        {
                            DynamicItemData[itemType] = did;
                        }
                        else if (isItemClass)
                        {
                            DynamicItemClassData[itemClass] = did;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }

                List<Area> areaFlatList = new List<Area>();
                Dictionary<int, Area> areaMapping = new Dictionary<int, Area>();
                Dictionary<string, Area> areasByName = new Dictionary<string, Area>();

                cmd.CommandText = "SELECT ID,ParentID,DisplayName,TickRoom,PawnShop,InventorySinkRoom FROM Areas WHERE UserID = @UserID ORDER BY OrderValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Area a = new Area(null);
                        bool isValid = true;

                        int iID = Convert.ToInt32(reader["ID"]);
                        a.ID = iID;

                        oData = reader["ParentID"];
                        if (oData != DBNull.Value) a.ParentID = Convert.ToInt32(oData);

                        string sDisplayName = reader["DisplayName"].ToString();
                        a.DisplayName = sDisplayName;
                        if (string.IsNullOrEmpty(sDisplayName))
                        {
                            isValid = false;
                            errorMessages.Add("Area found with no display name.");
                        }
                        else if (areasByName.ContainsKey(sDisplayName))
                        {
                            isValid = false;
                            errorMessages.Add("Duplicate area display name found.");
                        }
                        else
                        {
                            areasByName[sDisplayName] = a;
                        }

                        oData = reader["TickRoom"];
                        if (oData != DBNull.Value) a.TickRoom = (HealingRoom)Convert.ToInt32(oData);
                        oData = reader["PawnShop"];
                        if (oData != DBNull.Value) a.PawnShop = (PawnShoppe)Convert.ToInt32(oData);

                        Room rTemp;
                        oData = reader["InventorySinkRoom"];
                        if (oData != DBNull.Value)
                        {
                            a.InventorySinkRoomIdentifier = oData.ToString();
                            isValid &= ValidateRoomFromIdentifier(a.InventorySinkRoomIdentifier, errorMessages, gameMap, out rTemp, "inventory sink", "area");
                            a.InventorySinkRoomObject = rTemp;
                        }

                        a.IsValid = isValid;
                        areaMapping[iID] = a;
                        areaFlatList.Add(a);
                    }
                }
                foreach (Area a in areaFlatList)
                {
                    if (a.ParentID == 0)
                    {
                        if (HomeArea != null)
                        {
                            errorMessages.Add("Duplicate home area found");
                            a.IsValid = false;
                        }
                        HomeArea = a;
                    }
                    else if (areaMapping.TryGetValue(a.ParentID, out Area parent))
                    {
                        if (parent.Children == null) parent.Children = new List<Area>();
                        parent.Children.Add(a);
                        a.Parent = parent;
                    }
                    else
                    {
                        errorMessages.Add("Location parent ID not found for " + a.ToString());
                        a.IsValid = false;
                    }
                }
                areaMapping.Clear();
                if (HomeArea != null)
                {
                    if (HomeArea.IsValid)
                    {
                        RemoveInvalidAreas(HomeArea);
                        PopulateAreaMappings(HomeArea, areaMapping);
                    }
                    else
                    {
                        HomeArea = null;
                    }
                }

                List<LocationNode> locationFlatList = new List<LocationNode>();
                Dictionary<int, LocationNode> locationMapping = new Dictionary<int, LocationNode>();
                cmd.CommandText = "SELECT ID,ParentID,DisplayName,Room,Expanded FROM LocationNodes WHERE UserID = @UserID ORDER BY OrderValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LocationNode ln = new LocationNode(null);
                        bool isValid = true;

                        int iID = Convert.ToInt32(reader["ID"]);
                        ln.ID = iID;

                        oData = reader["DisplayName"];
                        string sDisplayName = oData == DBNull.Value ? string.Empty : oData.ToString();
                        ln.DisplayName = sDisplayName;

                        oData = reader["Room"];
                        string sRoom = oData == DBNull.Value ? string.Empty : oData.ToString();
                        ln.Room = sRoom;
                        Room r = null;
                        if (!string.IsNullOrEmpty(sRoom))
                        {
                            r = gameMap.GetRoomFromTextIdentifier(sRoom);
                            if (r == null)
                            {
                                isValid = false;
                                errorMessages.Add("Unable to determine location room: " + sRoom);
                            }
                        }
                        ln.RoomObject = r;

                        ln.Expanded = Convert.ToInt32(reader["Expanded"]) != 0;

                        oData = reader["ParentID"];
                        int iParentID = oData == DBNull.Value ? 0 : Convert.ToInt32(oData);
                        ln.ParentID = iParentID;

                        ln.IsValid = isValid;
                        locationMapping[iID] = ln;
                        locationFlatList.Add(ln);
                    }
                }
                foreach (LocationNode ln in locationFlatList)
                {
                    if (ln.ParentID == 0)
                    {
                        Locations.Add(ln);
                    }
                    else if (locationMapping.TryGetValue(ln.ParentID, out LocationNode parent))
                    {
                        if (parent.Children == null) parent.Children = new List<LocationNode>();
                        parent.Children.Add(ln);
                        ln.Parent = parent;
                    }
                    else
                    {
                        errorMessages.Add("Location parent ID not found for " + ln.ToString());
                        ln.IsValid = false;
                    }
                }
                RemoveInvalidLocations(Locations);

                List<Strategy> strategiesTemp = new List<Strategy>();
                Dictionary<int, Strategy> strats = new Dictionary<int, Strategy>();
                cmd.CommandText = "SELECT ID,DisplayName,AfterKillMonsterAction,ManaPool,FinalMagicAction,FinalMeleeAction,FinalPotionsAction,MagicOnlyWhenStunnedForXMS,MeleeOnlyWhenStunnedForXMS,PotionsOnlyWhenStunnedForXMS,TypesToRunLastCommandIndefinitely,TypesWithStepsEnabled,AutoSpellLevelMin,AutoSpellLevelMax,Realms FROM Strategies WHERE UserID = @UserID ORDER BY OrderValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Strategy s = new Strategy();
                        s.IsValid = true;
                        s.ID = Convert.ToInt32(reader["ID"]);
                        oData = reader["DisplayName"];
                        s.DisplayName = oData == DBNull.Value ? string.Empty : oData.ToString();
                        s.AfterKillMonsterAction = (AfterKillMonsterAction)Convert.ToInt32(reader["AfterKillMonsterAction"]);
                        oData = reader["ManaPool"];
                        if (oData != DBNull.Value) s.ManaPool = Convert.ToInt32(oData);
                        s.FinalMagicAction = (FinalStepAction)Convert.ToInt32(reader["FinalMagicAction"]);
                        s.FinalMeleeAction = (FinalStepAction)Convert.ToInt32(reader["FinalMeleeAction"]);
                        s.FinalPotionsAction = (FinalStepAction)Convert.ToInt32(reader["FinalPotionsAction"]);
                        oData = reader["MagicOnlyWhenStunnedForXMS"];
                        if (oData != DBNull.Value) s.MagicOnlyWhenStunnedForXMS = Convert.ToInt32(oData);
                        oData = reader["MeleeOnlyWhenStunnedForXMS"];
                        if (oData != DBNull.Value) s.MeleeOnlyWhenStunnedForXMS = Convert.ToInt32(oData);
                        oData = reader["PotionsOnlyWhenStunnedForXMS"];
                        if (oData != DBNull.Value) s.PotionsOnlyWhenStunnedForXMS = Convert.ToInt32(oData);
                        s.TypesToRunLastCommandIndefinitely = (CommandType)Convert.ToInt32(reader["TypesToRunLastCommandIndefinitely"]);
                        s.TypesWithStepsEnabled = (CommandType)Convert.ToInt32(reader["TypesWithStepsEnabled"]);
                        oData = reader["AutoSpellLevelMin"];
                        if (oData != DBNull.Value) s.AutoSpellLevelMin = Convert.ToInt32(oData);
                        oData = reader["AutoSpellLevelMax"];
                        if (oData != DBNull.Value) s.AutoSpellLevelMax = Convert.ToInt32(oData);
                        oData = reader["Realms"];
                        if (oData != DBNull.Value) s.Realms = (RealmTypeFlags)Convert.ToInt32(oData);

                        if (!ValidateStrategy(s, errorMessages))
                        {
                            s.IsValid = false;
                        }

                        strategiesTemp.Add(s);
                        strats[s.ID] = s;
                    }
                }
                cmd.CommandText = "SELECT ss.StrategyID,ss.CombatType,ss.StepType FROM StrategySteps ss INNER JOIN Strategies s ON ss.StrategyID = s.ID WHERE s.UserID = @UserID ORDER BY ss.IndexValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int strategyID = Convert.ToInt32(reader["StrategyID"]);
                        CommandType combatType = (CommandType)Convert.ToInt32(reader["CombatType"]);
                        int stepType = Convert.ToInt32(reader["StepType"]);
                        Strategy s = strats[strategyID];
                        if (combatType == CommandType.Magic)
                        {
                            MagicStrategyStep magicss = (MagicStrategyStep)stepType;
                            if (int.TryParse(magicss.ToString(), out _))
                            {
                                s.IsValid = false;
                                errorMessages.Add("Invalid magic strategy step found: " + magicss);
                                continue;
                            }
                            else
                            {
                                if (s.MagicSteps == null) s.MagicSteps = new List<MagicStrategyStep>();
                                s.MagicSteps.Add(magicss);
                            }
                        }
                        else if (combatType == CommandType.Melee)
                        {
                            MeleeStrategyStep meleess = (MeleeStrategyStep)stepType;
                            if (int.TryParse(meleess.ToString(), out _))
                            {
                                s.IsValid = false;
                                errorMessages.Add("Invalid melee strategy step found: " + meleess);
                                continue;
                            }
                            else
                            {
                                if (s.MeleeSteps == null) s.MeleeSteps = new List<MeleeStrategyStep>();
                                s.MeleeSteps.Add(meleess);
                            }
                        }
                        else if (combatType == CommandType.Potions)
                        {
                            PotionsStrategyStep potionsss = (PotionsStrategyStep)stepType;
                            if (int.TryParse(potionsss.ToString(), out _))
                            {
                                s.IsValid = false;
                                errorMessages.Add("Invalid potions strategy step found: " + potionsss);
                                continue;
                            }
                            else
                            {
                                if (s.PotionsSteps == null) s.PotionsSteps = new List<PotionsStrategyStep>();
                                s.PotionsSteps.Add(potionsss);
                            }
                        }
                        else
                        {
                            errorMessages.Add("Invalid combat type found for strategy step: " + combatType);
                            continue;
                        }
                    }
                }
                foreach (Strategy s in strategiesTemp)
                {
                    if (s.IsValid)
                        Strategies.Add(s);
                    else
                        strats.Remove(s.ID);
                }
                Dictionary<int, PermRun> permRunMapping = new Dictionary<int, PermRun>();
                cmd.CommandText = "SELECT p.ID,p.DisplayName,p.Rehome,p.BeforeFull,p.AfterFull,p.SpellsToCast,p.SpellsToPotion,p.SkillsToRun,p.SupportedKeys,p.TargetRoom,p.ThresholdRoom,p.MobText,p.MobIndex,p.StrategyID,p.UseMagicCombat,p.UseMeleeCombat,p.UsePotionsCombat,p.AfterKillMonsterAction,p.AutoSpellLevelMin,p.AutoSpellLevelMax,p.Realms,p.ItemsToProcessType,p.LastCompleted FROM PermRuns p INNER JOIN Strategies s ON p.StrategyID = s.ID WHERE p.UserID = @UserID AND s.UserID = @UserID ORDER BY p.OrderValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PermRun permRun = new PermRun();
                        permRun.IsValid = true;
                        permRun.ID = Convert.ToInt32(reader["ID"]);
                        oData = reader["DisplayName"];
                        permRun.DisplayName = oData == DBNull.Value ? string.Empty : oData.ToString();

                        permRun.Rehome = Convert.ToInt32(reader["Rehome"]) != 0;

                        oData = reader["BeforeFull"];
                        if (oData != DBNull.Value) permRun.BeforeFull = (FullType)Convert.ToInt32(oData);
                        oData = reader["AfterFull"];
                        if (oData != DBNull.Value) permRun.AfterFull = (FullType)Convert.ToInt32(oData);
                        permRun.SpellsToCast = (WorkflowSpells)Convert.ToInt32(reader["SpellsToCast"]);
                        permRun.SpellsToPotion = (WorkflowSpells)Convert.ToInt32(reader["SpellsToPotion"]);
                        permRun.SkillsToRun = (PromptedSkills)Convert.ToInt32(reader["SkillsToRun"]);
                        permRun.SupportedKeys = (SupportedKeysFlags)Convert.ToInt32(reader["SupportedKeys"]);

                        Room rTemp;

                        permRun.TargetRoomIdentifier = reader["TargetRoom"].ToString();
                        permRun.IsValid &= ValidateRoomFromIdentifier(permRun.TargetRoomIdentifier, errorMessages, gameMap, out rTemp, "target", "perm run");
                        permRun.TargetRoomObject = rTemp;

                        oData = reader["ThresholdRoom"];
                        if (oData != DBNull.Value)
                        {
                            permRun.ThresholdRoomIdentifier = oData.ToString();
                            permRun.IsValid &= ValidateRoomFromIdentifier(permRun.ThresholdRoomIdentifier, errorMessages, gameMap, out rTemp, "threshold", "perm run");
                            permRun.ThresholdRoomObject = rTemp;
                        }

                        oData = reader["MobText"];
                        string sMobText = oData == DBNull.Value ? string.Empty : oData.ToString();
                        bool hasMob = sMobText.Length > 0;
                        if (hasMob)
                        {
                            char cFirstChar = sMobText[0];
                            if (char.IsUpper(cFirstChar))
                            {
                                if (Enum.TryParse(sMobText, out MobTypeEnum mobType))
                                {
                                    permRun.MobType = mobType;
                                }
                                else
                                {
                                    permRun.IsValid = false;
                                    errorMessages.Add("Invalid perm run mob type: " + sMobText);
                                }
                            }
                            else
                            {
                                permRun.MobText = sMobText;
                            }
                        }

                        oData = reader["MobIndex"];
                        if (oData == DBNull.Value)
                        {
                            if (hasMob)
                            {
                                permRun.MobIndex = 1;
                            }
                        }
                        else //have a mob index in the database
                        {
                            int iMobIndex = Convert.ToInt32(oData);
                            if (iMobIndex < 1)
                            {
                                permRun.IsValid = false;
                                errorMessages.Add("Invalid perm run mob index: " + iMobIndex);
                            }
                            permRun.MobIndex = iMobIndex;
                        }

                        int iStrategyID = Convert.ToInt32(reader["StrategyID"]);
                        if (strats.TryGetValue(iStrategyID, out Strategy s))
                        {
                            permRun.Strategy = s;
                        }
                        else
                        {
                            permRun.IsValid = false;
                            errorMessages.Add("Perm run uses invalid strategy " + iStrategyID);
                        }
                        oData = reader["UseMagicCombat"];
                        if (oData != DBNull.Value) permRun.UseMagicCombat = Convert.ToInt32(oData) != 0;
                        oData = reader["UseMeleeCombat"];
                        if (oData != DBNull.Value) permRun.UseMeleeCombat = Convert.ToInt32(oData) != 0;
                        oData = reader["UsePotionsCombat"];
                        if (oData != DBNull.Value) permRun.UsePotionsCombat = Convert.ToInt32(oData) != 0;
                        oData = reader["AfterKillMonsterAction"];
                        if (oData != DBNull.Value) permRun.AfterKillMonsterAction = (AfterKillMonsterAction)Convert.ToInt32(oData);
                        oData = reader["AutoSpellLevelMin"];
                        if (oData != DBNull.Value) permRun.AutoSpellLevelMin = Convert.ToInt32(oData);
                        oData = reader["AutoSpellLevelMax"];
                        if (oData != DBNull.Value) permRun.AutoSpellLevelMax = Convert.ToInt32(oData);
                        oData = reader["Realms"];
                        if (oData != DBNull.Value) permRun.Realms = (RealmTypeFlags)Convert.ToInt32(oData);
                        permRun.ItemsToProcessType = (ItemsToProcessType)Convert.ToInt32(reader["ItemsToProcessType"]);
                        oData = reader["LastCompleted"];
                        if (oData != DBNull.Value) permRun.LastCompleted = new DateTime(Convert.ToInt64(oData), DateTimeKind.Utc);

                        if (permRun.IsValid)
                        {
                            PermRuns.Add(permRun);
                            permRunMapping[permRun.ID] = permRun;
                        }
                    }
                }
                cmd.CommandText = "SELECT p2a.PermRunID,p2a.AreaID FROM PermRunToAreas p2a INNER JOIN PermRuns p ON p2a.PermRunID = p.ID INNER JOIN Areas a ON p2a.AreaID = a.ID WHERE p.UserID = @UserID AND a.UserID = @UserID";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int iPermRunID = Convert.ToInt32(reader["PermRunID"]);
                        int iAreaID = Convert.ToInt32(reader["AreaID"]);
                        if (areaMapping.TryGetValue(iAreaID, out Area a) && permRunMapping.TryGetValue(iPermRunID, out PermRun p))
                        {
                            if (p.Areas == null) p.Areas = new HashSet<Area>();
                            p.Areas.Add(a);
                        }
                    }
                }
                ValidateSettings(errorMessages);
            }
        }

        /// <summary>
        /// removes invalid areas. The specified area is assumed to be valid
        /// </summary>
        /// <param name="a">area to check</param>
        public void RemoveInvalidAreas(Area a)
        {
            if (a.Children != null)
            {
                for (int i = a.Children.Count - 1; i >= 0; i--)
                {
                    Area aChild = a.Children[i];
                    if (aChild.IsValid)
                    {
                        RemoveInvalidAreas(aChild);
                    }
                    else
                    {
                        a.Children.RemoveAt(i);
                    }
                }
                if (a.Children.Count == 0)
                {
                    a.Children = null;
                }
            }
        }

        private void PopulateAreaMappings(Area a, Dictionary<int, Area> AreasByID)
        {
            AreasByID[a.ID] = a;
            if (a.Children != null)
            {
                foreach (Area aChild in a.Children)
                {
                    PopulateAreaMappings(aChild, AreasByID);
                }
            }
        }

        private void RemoveInvalidLocations(List<LocationNode> locations)
        {
            for (int i = locations.Count - 1; i >= 0; i--)
            {
                LocationNode ln = locations[i];
                if (ln.IsValid)
                {
                    if (ln.Children != null)
                    {
                        RemoveInvalidLocations(ln.Children);
                        if (ln.Children.Count == 0)
                        {
                            ln.Children = null;
                        }
                    }
                }
                else
                {
                    locations.RemoveAt(i);
                }
            }
        }

        private bool ValidateRoomFromIdentifier(string identifier, List<string> errorMessages, IsengardMap gameMap, out Room r, string roomType, string objectType)
        {
            bool ret = true;
            r = null;
            if (string.IsNullOrEmpty(identifier))
            {
                ret = false;
                errorMessages.Add($"Invalid {objectType} {roomType} room: {identifier}");
            }
            else
            {
                r = gameMap.GetRoomFromTextIdentifier(identifier);
                if (r == null)
                {
                    ret = false;
                    errorMessages.Add($"Unable to find {objectType} {roomType} room: {identifier}");
                }
            }
            return ret;
        }

        public IsengardSettingData(string Input, List<string> errorMessages, bool IsFile, IsengardMap gameMap) : this()
        {
            XmlDocument doc = new XmlDocument();
            if (IsFile)
            {
                doc.Load(Input);
            }
            else
            {
                doc.LoadXml(Input);
            }
            XmlElement docElement = doc.DocumentElement;
            Dictionary<string, XmlElement> topLevelElements = new Dictionary<string, XmlElement>();
            XmlElement elem;
            foreach (XmlNode nextNode in docElement.ChildNodes)
            {
                elem = nextNode as XmlElement;
                if (elem == null) continue;
                string sElemName = elem.Name;
                if (topLevelElements.ContainsKey(sElemName))
                    errorMessages.Add($"Duplicate {sElemName} element found.");
                else
                    topLevelElements[sElemName] = elem;
            }
            List<string> standardElements = new List<string>()
            {
                "Settings",
                "DynamicItemData",
                "Area",
                "Locations",
                "Strategies"
            };
            Dictionary<string, Area> areasByName = new Dictionary<string, Area>();
            foreach (string nextStandardElement in standardElements)
            {
                if (topLevelElements.TryGetValue(nextStandardElement, out elem))
                {
                    if (nextStandardElement == "Settings")
                        HandleSettings(elem, errorMessages);
                    else if (nextStandardElement == "DynamicItemData")
                        HandleDynamicItemData(elem, errorMessages);
                    else if (nextStandardElement == "Area")
                        HomeArea = HandleArea(null, elem, errorMessages, gameMap, areasByName);
                    else if (nextStandardElement == "Locations")
                        HandleLocations(elem, errorMessages, null, Locations, gameMap);
                    else if (nextStandardElement == "Strategies")
                        HandleStrategies(elem, errorMessages, gameMap, areasByName);
                    else
                        throw new InvalidOperationException();
                    topLevelElements.Remove(nextStandardElement);
                }
            }
            foreach (var next in topLevelElements)
            {
                errorMessages.Add("Unexpected element found in XML: " + next.Key);
            }
            ValidateSettings(errorMessages);
        }
        public static IsengardSettingData GetDefaultSettings()
        {
            IsengardSettingData ret = new IsengardSettingData();
            ret.Strategies.AddRange(Strategy.GetDefaultStrategies());
            ret.HomeArea = Area.GetDefaultHomeArea();
            return ret;
        }

        private Area HandleArea(Area parentArea, XmlElement elem, List<string> errorMessages, IsengardMap gameMap, Dictionary<string, Area> areasByName)
        {
            HashSet<string> attributes = new HashSet<string>()
            {
                "DisplayName",
                "TickRoom",
                "PawnShop",
                "InventorySinkRoom",
            };

            Area a = new Area(parentArea);

            bool isValid;
            var attributeMapping = GetAttributeMapping(elem, attributes, errorMessages, out isValid);
            string sValue;

            sValue = GetAttributeValueByName(attributeMapping, "DisplayName");
            if (string.IsNullOrEmpty(sValue))
            {
                errorMessages.Add("No display name specified for area.");
                isValid = false;
            }
            else if (areasByName.ContainsKey(sValue))
            {
                errorMessages.Add($"Duplicate area found: {sValue}");
                isValid = false;
            }
            else
            {
                areasByName[sValue] = a;
            }
            a.DisplayName = sValue;

            sValue = GetAttributeValueByName(attributeMapping, "TickRoom");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (Enum.TryParse(sValue, out HealingRoom eTickRoom))
                {
                    a.TickRoom = eTickRoom;
                }
                else
                {
                    errorMessages.Add("Invalid area tick room: " + sValue);
                    isValid = false;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "PawnShop");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (Enum.TryParse(sValue, out PawnShoppe ePawnShop))
                {
                    a.PawnShop = ePawnShop;
                }
                else
                {
                    errorMessages.Add("Invalid area pawn shop: " + sValue);
                    isValid = false;
                }
            }

            Room rTemp;
            sValue = GetAttributeValueByName(attributeMapping, "InventorySinkRoom");
            if (!string.IsNullOrEmpty(sValue))
            {
                isValid &= ValidateRoomFromIdentifier(sValue, errorMessages, gameMap, out rTemp, "inventory sink", "area");
                a.InventorySinkRoomIdentifier = sValue;
                a.InventorySinkRoomObject = rTemp;
            }

            foreach (XmlNode nextAreaNode in elem.ChildNodes)
            {
                XmlElement nextAreaElem = nextAreaNode as XmlElement;
                if (nextAreaElem == null) continue;
                string sAreaElementName = nextAreaElem.Name;
                if (sAreaElementName != "Area")
                {
                    errorMessages.Add("Invalid area element: " + sAreaElementName);
                    continue;
                }
                Area aChild = HandleArea(a, nextAreaElem, errorMessages, gameMap, areasByName);
                if (aChild == null)
                {
                    isValid = false;
                }
                else
                {
                    if (a.Children == null)
                    {
                        a.Children = new List<Area>();
                    }
                    a.Children.Add(aChild);
                }
            }

            Area aRet = null;
            if (isValid)
            {
                aRet = a;
            }
            return aRet;
        }

        private void HandleStrategies(XmlElement elem, List<string> errorMessages, IsengardMap gameMap, Dictionary<string, Area> areasByName)
        {
            HashSet<string> strategyAttributes = new HashSet<string>()
            {
                "DisplayName",
                "AfterKillMonsterAction",
                "ManaPool",
                "FinalMagicAction",
                "FinalMeleeAction",
                "FinalPotionsAction",
                "MagicOnlyWhenStunnedForXMS",
                "MeleeOnlyWhenStunnedForXMS",
                "PotionsOnlyWhenStunnedForXMS",
                "TypesToRunLastCommandIndefinitely",
                "TypesWithStepsEnabled",
                "AutoSpellLevelMin",
                "AutoSpellLevelMax",
                "Realms",
            };
            HashSet<string> permRunAttributes = new HashSet<string>()
            {
                "Order",
                "DisplayName",
                "Rehome",
                "Areas",
                "BeforeFull",
                "AfterFull",
                "SpellsToCast",
                "SpellsToPotion",
                "SkillsToRun",
                "SupportedKeys",
                "TargetRoom",
                "ThresholdRoom",
                "Mob",
                "MobIndex",
                "UseMagicCombat",
                "UseMeleeCombat",
                "UsePotionsCombat",
                "AfterKillMonsterAction",
                "AutoSpellLevelMin",
                "AutoSpellLevelMax",
                "Realms",
                "ItemsToProcessType",
                "LastCompleted",
            };
            string sValue;
            int iValue;
            int? iValueNullable;
            foreach (XmlNode nextStrategyNode in elem.ChildNodes)
            {
                XmlElement nextStrategyElem = nextStrategyNode as XmlElement;
                if (nextStrategyElem == null) continue;
                string sStrategyElementName = nextStrategyElem.Name;
                if (sStrategyElementName != "Strategy")
                {
                    errorMessages.Add("Invalid strategy element: " + sStrategyElementName);
                    continue;
                }
                Strategy s = new Strategy();
                List<PermRun> permRunsForStrategy = new List<PermRun>();
                s.IsValid = true;

                var attributeMapping = GetAttributeMapping(nextStrategyElem, strategyAttributes, errorMessages, out bool isValid);
                if (!isValid)
                {
                    s.IsValid = false;
                }

                s.DisplayName = GetAttributeValueByName(attributeMapping, "DisplayName");

                sValue = GetAttributeValueByName(attributeMapping, "ManaPool");
                iValue = 0;
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (!int.TryParse(sValue, out iValue) || iValue <= 0)
                    {
                        s.IsValid = false;
                        errorMessages.Add("Invalid strategy mana pool: " + sValue);
                    }
                }
                s.ManaPool = iValue;

                sValue = GetAttributeValueByName(attributeMapping, "AfterKillMonsterAction");
                AfterKillMonsterAction eAfterKillMonsterAction;
                if (!Enum.TryParse(sValue, out eAfterKillMonsterAction))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy after kill monster action: " + sValue);
                }
                s.AfterKillMonsterAction = eAfterKillMonsterAction;

                sValue = GetAttributeValueByName(attributeMapping, "FinalMagicAction");
                FinalStepAction action;
                if (!Enum.TryParse(sValue, out action))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy final magic action: " + sValue);
                }
                s.FinalMagicAction = action;

                sValue = GetAttributeValueByName(attributeMapping, "FinalMeleeAction");
                if (!Enum.TryParse(sValue, out action))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy final melee action: " + sValue);
                }
                s.FinalMeleeAction = action;

                sValue = GetAttributeValueByName(attributeMapping, "FinalPotionsAction");
                if (!Enum.TryParse(sValue, out action))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy final potions action: " + sValue);
                }
                s.FinalPotionsAction = action;

                sValue = GetAttributeValueByName(attributeMapping, "MagicOnlyWhenStunnedForXMS");
                iValueNullable = null;
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (int.TryParse(sValue, out iValue) && iValue >= 0)
                    {
                        iValueNullable = iValue;
                    }
                    else
                    {
                        s.IsValid = false;
                        errorMessages.Add("Invalid strategy magic only when stunned for x ms: " + sValue);
                    }
                }
                s.MagicOnlyWhenStunnedForXMS = iValueNullable;

                sValue = GetAttributeValueByName(attributeMapping, "MeleeOnlyWhenStunnedForXMS");
                iValueNullable = null;
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (int.TryParse(sValue, out iValue) && iValue >= 0)
                    {
                        iValueNullable = iValue;
                    }
                    else
                    {
                        s.IsValid = false;
                        errorMessages.Add("Invalid strategy melee only when stunned for x ms: " + sValue);
                    }
                }
                s.MeleeOnlyWhenStunnedForXMS = iValueNullable;

                sValue = GetAttributeValueByName(attributeMapping, "PotionsOnlyWhenStunnedForXMS");
                iValueNullable = null;
                if (!string.IsNullOrEmpty(sValue))
                {
                    if (int.TryParse(sValue, out iValue) && iValue >= 0)
                    {
                        iValueNullable = iValue;
                    }
                    else
                    {
                        s.IsValid = false;
                        errorMessages.Add("Invalid strategy potions only when stunned for x ms: " + sValue);
                    }
                }
                s.PotionsOnlyWhenStunnedForXMS = iValueNullable;

                sValue = GetAttributeValueByName(attributeMapping, "TypesToRunLastCommandIndefinitely");
                CommandType commandTypes;
                if (!Enum.TryParse(sValue, out commandTypes))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy command types to run last step indefinitely: " + sValue);
                }
                s.TypesToRunLastCommandIndefinitely = commandTypes;

                sValue = GetAttributeValueByName(attributeMapping, "TypesWithStepsEnabled");
                if (!Enum.TryParse(sValue, out commandTypes))
                {
                    s.IsValid = false;
                    errorMessages.Add("Invalid strategy command types enabled: " + sValue);
                }
                s.TypesWithStepsEnabled = commandTypes;

                int iAutoSpellLevelMin, iAutoSpellLevelMax;
                if (!ProcessNonRequiredAutoSpellLevelMinMaxFromText(GetAttributeValueByName(attributeMapping, "AutoSpellLevelMin"), GetAttributeValueByName(attributeMapping, "AutoSpellLevelMax"), out iAutoSpellLevelMin, out iAutoSpellLevelMax, errorMessages, "strategy"))
                {
                    s.IsValid = false;
                }
                s.AutoSpellLevelMin = iAutoSpellLevelMin;
                s.AutoSpellLevelMax = iAutoSpellLevelMax;

                if (!ProcessNonRequiredNonBlankRealmsText(GetAttributeValueByName(attributeMapping, "Realms"), out RealmTypeFlags? realms, errorMessages, "strategy"))
                {
                    s.IsValid = false;
                }
                s.Realms = realms;

                if (!ValidateStrategy(s, errorMessages))
                {
                    s.IsValid = false;
                }

                foreach (XmlNode nextNode in nextStrategyElem.ChildNodes)
                {
                    XmlElement nextSubElement = nextNode as XmlElement;
                    if (nextSubElement == null) continue;
                    string sSubElementName = nextSubElement.Name;
                    if (sSubElementName == "PermRun")
                    {
                        PermRun p = HandlePermRun(nextSubElement, errorMessages, permRunAttributes, gameMap, areasByName);
                        p.Strategy = s;
                        if (p.IsValid)
                        {
                            permRunsForStrategy.Add(p);
                        }
                    }
                    else if (sSubElementName == "Step")
                    {
                        string sCombat = nextSubElement.Attributes["combat"]?.Value;
                        string sStep = nextSubElement.Attributes["step"]?.Value;
                        if (Enum.TryParse(sCombat, out CommandType combatType) && (combatType == CommandType.Magic || combatType == CommandType.Melee || combatType == CommandType.Potions))
                        {
                            if (combatType == CommandType.Magic)
                            {
                                if (Enum.TryParse(sStep, out MagicStrategyStep magicStep))
                                {
                                    if (s.MagicSteps == null) s.MagicSteps = new List<MagicStrategyStep>();
                                    s.MagicSteps.Add(magicStep);
                                }
                                else
                                {
                                    s.IsValid = false;
                                    errorMessages.Add("Invalid magic strategy step element: " + sStep);
                                }
                            }
                            else if (combatType == CommandType.Melee)
                            {
                                if (Enum.TryParse(sStep, out MeleeStrategyStep meleeStep))
                                {
                                    if (s.MeleeSteps == null) s.MeleeSteps = new List<MeleeStrategyStep>();
                                    s.MeleeSteps.Add(meleeStep);
                                }
                                else
                                {
                                    s.IsValid = false;
                                    errorMessages.Add("Invalid melee strategy step element: " + sStep);
                                }
                            }
                            else if (combatType == CommandType.Potions)
                            {
                                if (Enum.TryParse(sStep, out PotionsStrategyStep potionsStep))
                                {
                                    if (s.PotionsSteps == null) s.PotionsSteps = new List<PotionsStrategyStep>();
                                    s.PotionsSteps.Add(potionsStep);
                                }
                                else
                                {
                                    s.IsValid = false;
                                    errorMessages.Add("Invalid potions strategy step element: " + sStep);
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            s.IsValid = false;
                            errorMessages.Add("Invalid strategy step combat type: " + sCombat);
                        }
                    }
                    else
                    {
                        s.IsValid = false;
                        errorMessages.Add("Invalid strategy subelement: " + sSubElementName);
                        continue;
                    }
                }

                if (s.IsValid)
                {
                    Strategies.Add(s);
                    PermRuns.AddRange(permRunsForStrategy);
                }
            }
            PermRuns.Sort((a, b) => { return a.OrderValue.CompareTo(b.OrderValue); });
        }

        private bool ProcessNonRequiredNonBlankRealmsText(string sRealms, out RealmTypeFlags? realms, List<string> errorMessages, string objectType)
        {
            bool bRet = true;
            if (string.IsNullOrEmpty(sRealms))
            {
                realms = null;
            }
            else
            {
                bool success = false;
                if (Enum.TryParse(sRealms, out RealmTypeFlags realmsValue))
                {
                    success = realmsValue != RealmTypeFlags.None;
                }
                if (!success)
                {
                    errorMessages.Add($"Invalid {objectType} realms: {StringProcessing.TrimFlagsEnumToString(realmsValue)}");
                    bRet = false;
                }
                realms = realmsValue;
            }
            return bRet;
        }

        private bool ProcessNonRequiredAutoSpellLevelMinMaxFromText(string sMin, string sMax, out int autoSpellLevelMin, out int autoSpellLevelMax, List<string> errorMessages, string objectType)
        {
            bool ret = true;
            autoSpellLevelMin = AUTO_SPELL_LEVEL_NOT_SET;
            if (!string.IsNullOrEmpty(sMin))
            {
                if (!int.TryParse(sMin, out autoSpellLevelMin) || autoSpellLevelMin <= 0)
                {
                    ret = false;
                    errorMessages.Add($"Invalid {objectType} auto spell level min: {sMin}");
                }
            }
            autoSpellLevelMax = AUTO_SPELL_LEVEL_NOT_SET;
            if (!string.IsNullOrEmpty(sMax))
            {
                if (!int.TryParse(sMax, out autoSpellLevelMax) || autoSpellLevelMax <= 0)
                {
                    ret = false;
                    errorMessages.Add($"Invalid {objectType} auto spell level max: {sMax}");
                }
            }
            return ret;
        }

        private string GetAttributeValueByName(Dictionary<string, string> input, string attributeName)
        {
            string ret;
            input.TryGetValue(attributeName, out ret);
            return ret;
        }

        private Dictionary<string, string> GetAttributeMapping(XmlElement elem, HashSet<string> expectedAttributes, List<string> errorMessages, out bool isValid)
        {
            isValid = true;
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (XmlAttribute nextAttr in elem.Attributes)
            {
                string sAttrName = nextAttr.Name;
                string sAttrValue = nextAttr.Value;
                if (expectedAttributes.Contains(sAttrName))
                {
                    ret[sAttrName] = sAttrValue;
                }
                else
                {
                    isValid = false;
                    errorMessages.Add($"Unexpected {elem.Name} attribute found: {sAttrName}");
                }
            }
            return ret;
        }

        private PermRun HandlePermRun(XmlElement elem, List<string> errorMessages, HashSet<string> permRunAttributes, IsengardMap gameMap, Dictionary<string, Area> areasByName)
        {
            PermRun p = new PermRun();
            var attributeMapping = GetAttributeMapping(elem, permRunAttributes, errorMessages, out bool isValid);

            string sValue;
            int iValue;
            bool bValue;

            sValue = GetAttributeValueByName(attributeMapping, "Order");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (int.TryParse(sValue, out iValue))
                {
                    p.OrderValue = iValue;
                }
                else
                {
                    errorMessages.Add("Invalid perm run order: " + sValue);
                    isValid = false;
                }
            }

            p.DisplayName = GetAttributeValueByName(attributeMapping, "DisplayName");

            sValue = GetAttributeValueByName(attributeMapping, "Rehome");
            if (string.IsNullOrEmpty(sValue))
            {
                bValue = false;
            }
            else if (!bool.TryParse(sValue, out bValue))
            {
                errorMessages.Add("Invalid perm run rehome: " + sValue);
                isValid = false;
            }
            p.Rehome = bValue;

            sValue = GetAttributeValueByName(attributeMapping, "Areas");
            if (!string.IsNullOrEmpty(sValue))
            {
                string[] areaDisplayNames = sValue.Split(new char[] { ',' });
                foreach (string nextDisplayName in areaDisplayNames)
                {
                    if (!areasByName.TryGetValue(nextDisplayName, out Area foundArea))
                    {
                        errorMessages.Add($"Perm run area not found: {nextDisplayName}");
                        isValid = false;
                    }
                    else if (p.Areas != null && p.Areas.Contains(foundArea))
                    {
                        errorMessages.Add($"Duplicate perm run area found: {sValue}");
                        isValid = false;
                    }
                    else
                    {
                        if (p.Areas == null) p.Areas = new HashSet<Area>();
                        p.Areas.Add(foundArea);
                    }
                }
            }

            if (p.Rehome && p.Areas == null)
            {
                errorMessages.Add("Perm run set to rehome without an area.");
                isValid = false;
            }

            FullType fullType;

            sValue = GetAttributeValueByName(attributeMapping, "BeforeFull");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (Enum.TryParse(sValue, out fullType))
                {
                    p.BeforeFull = fullType;
                }
                else
                {
                    errorMessages.Add("Invalid perm run before full: " + sValue);
                    isValid = false;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "AfterFull");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (Enum.TryParse(sValue, out fullType))
                {
                    p.AfterFull = fullType;
                }
                else
                {
                    errorMessages.Add("Invalid perm run full after finishing: " + sValue);
                    isValid = false;
                }
            }

            WorkflowSpells eSpells;

            sValue = GetAttributeValueByName(attributeMapping, "SpellsToCast");
            if (string.IsNullOrEmpty(sValue))
            {
                p.SpellsToCast = WorkflowSpells.None;
            }
            else if (Enum.TryParse(sValue, out eSpells))
            {
                p.SpellsToCast = eSpells;
            }
            else
            {
                errorMessages.Add("Invalid perm run spells to cast: " + sValue);
                isValid = false;
            }

            sValue = GetAttributeValueByName(attributeMapping, "SpellsToPotion");
            if (string.IsNullOrEmpty(sValue))
            {
                p.SpellsToPotion = WorkflowSpells.None;
            }
            else if (Enum.TryParse(sValue, out eSpells))
            {
                p.SpellsToPotion = eSpells;
            }
            else
            {
                errorMessages.Add("Invalid perm run spells to potion: " + sValue);
                isValid = false;
            }

            sValue = GetAttributeValueByName(attributeMapping, "SkillsToRun");
            if (string.IsNullOrEmpty(sValue))
            {
                p.SkillsToRun = PromptedSkills.None;
            }
            else if (Enum.TryParse(sValue, out PromptedSkills eSkills))
            {
                p.SkillsToRun = eSkills;
            }
            else
            {
                errorMessages.Add("Invalid perm run skills to run: " + sValue);
                isValid = false;
            }

            sValue = GetAttributeValueByName(attributeMapping, "SupportedKeys");
            if (string.IsNullOrEmpty(sValue))
            {
                p.SupportedKeys = SupportedKeysFlags.None;
            }
            else if (Enum.TryParse(sValue, out SupportedKeysFlags eKeys))
            {
                p.SupportedKeys = eKeys;
            }
            else
            {
                errorMessages.Add("Invalid perm run supported keys: " + sValue);
                isValid = false;
            }

            Room rTemp;

            sValue = GetAttributeValueByName(attributeMapping, "TargetRoom");
            isValid &= ValidateRoomFromIdentifier(sValue, errorMessages, gameMap, out rTemp, "target", "perm run");
            p.TargetRoomIdentifier = sValue;
            p.TargetRoomObject = rTemp;

            sValue = GetAttributeValueByName(attributeMapping, "ThresholdRoom");
            if (!string.IsNullOrEmpty(sValue))
            {
                isValid &= ValidateRoomFromIdentifier(sValue, errorMessages, gameMap, out rTemp, "threshold", "perm run");
                p.ThresholdRoomIdentifier = sValue;
                p.ThresholdRoomObject = rTemp;
            }

            sValue = GetAttributeValueByName(attributeMapping, "Mob");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (char.IsUpper(sValue[0]))
                {
                    if (Enum.TryParse(sValue, out MobTypeEnum eMobType))
                    {
                        p.MobType = eMobType;
                    }
                    else
                    {
                        errorMessages.Add("Invalid perm run mob type: " + sValue);
                        isValid = false;
                    }
                }
                else
                {
                    p.MobText = sValue;
                }
            }

            bool hasMob = p.MobType.HasValue || !string.IsNullOrEmpty(p.MobText);
            sValue = GetAttributeValueByName(attributeMapping, "MobIndex");
            if (string.IsNullOrEmpty(sValue))
            {
                if (hasMob)
                {
                    p.MobIndex = 1;
                }
            }
            else
            {
                if (!int.TryParse(sValue, out iValue))
                {
                    errorMessages.Add("Invalid perm run mob index: " + sValue);
                    isValid = false;
                }
                else if (iValue < 1)
                {
                    errorMessages.Add("Invalid perm run mob index: " + sValue);
                    isValid = false;
                }
                else
                {
                    p.MobIndex = iValue;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "UseMagicCombat");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (bool.TryParse(sValue, out bValue))
                {
                    p.UseMagicCombat = bValue;
                }
                else
                {
                    errorMessages.Add("Invalid perm run use magic combat: " + sValue);
                    isValid = false;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "UseMeleeCombat");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (bool.TryParse(sValue, out bValue))
                {
                    p.UseMeleeCombat = bValue;
                }
                else
                {
                    errorMessages.Add("Invalid perm run use melee combat: " + sValue);
                    isValid = false;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "UsePotionsCombat");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (bool.TryParse(sValue, out bValue))
                {
                    p.UsePotionsCombat = bValue;
                }
                else
                {
                    errorMessages.Add("Invalid perm run use potions combat: " + sValue);
                    isValid = false;
                }
            }

            sValue = GetAttributeValueByName(attributeMapping, "AfterKillMonsterAction");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (Enum.TryParse(sValue, out AfterKillMonsterAction eAction))
                {
                    p.AfterKillMonsterAction = eAction;
                }
                else
                {
                    errorMessages.Add("Invalid perm run after kill monster action: " + sValue);
                    isValid = false;
                }
            }

            if (!ProcessNonRequiredAutoSpellLevelMinMaxFromText(GetAttributeValueByName(attributeMapping, "AutoSpellLevelMin"), GetAttributeValueByName(attributeMapping, "AutoSpellLevelMax"), out int autoSpellLevelMin, out int autoSpellLevelMax, errorMessages, "perm run"))
            {
                isValid = false;
            }
            p.AutoSpellLevelMin = autoSpellLevelMin;
            p.AutoSpellLevelMax = autoSpellLevelMax;

            if (!ProcessNonRequiredNonBlankRealmsText(GetAttributeValueByName(attributeMapping, "Realms"), out RealmTypeFlags? realms, errorMessages, "perm run"))
            {
                isValid = false;
            }
            p.Realms = realms;

            sValue = GetAttributeValueByName(attributeMapping, "ItemsToProcessType");
            if (string.IsNullOrEmpty(sValue))
            {
                p.ItemsToProcessType = ItemsToProcessType.NoProcessing;
            }
            else if (Enum.TryParse(sValue, out ItemsToProcessType eValue))
            {
                p.ItemsToProcessType = eValue;
            }
            else
            {
                errorMessages.Add("Invalid perm run items to process type: " + sValue);
                isValid = false;
            }

            sValue = GetAttributeValueByName(attributeMapping, "LastCompleted");
            if (!string.IsNullOrEmpty(sValue))
            {
                if (long.TryParse(sValue, out long lValue))
                {
                    p.LastCompleted = new DateTime(lValue, DateTimeKind.Utc);
                }
                else
                {
                    errorMessages.Add("Invalid perm run last completed: " + sValue);
                    isValid = false;
                }
            }

            p.IsValid = isValid;
            return p;
        }

        private void HandleLocations(XmlElement elem, List<string> errorMessages, LocationNode parent, List<LocationNode> locations, IsengardMap gameMap)
        {
            foreach (XmlNode nextLocationNode in elem.ChildNodes)
            {
                XmlElement nextLocationElem = nextLocationNode as XmlElement;
                if (nextLocationElem != null)
                {
                    if (nextLocationElem.Name == "Location")
                    {
                        bool isValid = true;
                        string sDisplayName, sRoom, sExpanded;
                        sDisplayName = sRoom = sExpanded = null;
                        foreach (XmlAttribute nextAttr in nextLocationElem.Attributes)
                        {
                            string sAttrName = nextAttr.Name;
                            string sAttrValue = nextAttr.Value;
                            switch (sAttrName)
                            {
                                case "displayname":
                                    sDisplayName = sAttrValue;
                                    break;
                                case "room":
                                    sRoom = sAttrValue;
                                    break;
                                case "expanded":
                                    sExpanded = sAttrValue;
                                    break;
                                default:
                                    isValid = false;
                                    errorMessages.Add("Unexpected location attribute found: " + sAttrName);
                                    break;
                            }
                        }
                        bool bExpanded = false;
                        if (!string.IsNullOrEmpty(sExpanded))
                        {
                            if (!bool.TryParse(sExpanded, out bExpanded))
                            {
                                isValid = false;
                                errorMessages.Add("Invalid location expanded: " + sExpanded);
                            }
                        }
                        if (string.IsNullOrEmpty(sDisplayName) && string.IsNullOrEmpty(sRoom))
                        {
                            isValid = false;
                            errorMessages.Add("No room or display name specified for location");
                        }

                        Room r = null;
                        if (!string.IsNullOrEmpty(sRoom))
                        {
                            r = gameMap.GetRoomFromTextIdentifier(sRoom);
                            if (r == null)
                            {
                                isValid = false;
                                errorMessages.Add("Unable to find room from identifier: " + sRoom);
                            }
                        }

                        if (isValid)
                        {
                            LocationNode node = new LocationNode(parent);
                            node.DisplayName = sDisplayName;
                            node.Room = sRoom;
                            node.RoomObject = r;
                            locations.Add(node);
                            List<LocationNode> subNodes = new List<LocationNode>();
                            HandleLocations(nextLocationElem, errorMessages, node, subNodes, gameMap);
                            if (subNodes.Count > 0)
                            {
                                node.Children = subNodes;
                                node.Expanded = bExpanded;
                                foreach (LocationNode nextSubNode in subNodes)
                                {
                                    nextSubNode.Parent = node;
                                }
                            }
                        }
                    }
                    else
                    {
                        errorMessages.Add("Invalid location element: " + nextLocationElem.Name);
                    }
                }
            }
        }

        public void SaveSettings(SQLiteConnection conn, int userID)
        {
            Dictionary<string, string> existingSettings = new Dictionary<string, string>();
            Dictionary<string, string> newSettings = new Dictionary<string, string>();
            foreach (var next in GetSettingsToSave())
            {
                newSettings[next.Key] = next.Value;
            }
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT SettingName,SettingValue FROM Settings WHERE UserID = @UserID";
                cmd.Parameters.AddWithValue("@UserID", userID);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingSettings[reader["SettingName"].ToString()] = reader["SettingValue"].ToString();
                    }
                }
                List<string> keysToRemove = new List<string>();
                foreach (var next in newSettings)
                {
                    string sKey = next.Key;
                    if (existingSettings.TryGetValue(sKey, out string sValue))
                    {
                        if (sValue == next.Value)
                        {
                            keysToRemove.Add(sKey);
                        }
                    }
                }
                foreach (string nextKey in keysToRemove)
                {
                    newSettings.Remove(nextKey);
                    existingSettings.Remove(nextKey);
                }
                cmd.CommandText = "DELETE FROM Settings WHERE UserID = @UserID AND SettingName = @SettingName";
                SQLiteParameter settingName = cmd.Parameters.Add("@SettingName", DbType.String);
                foreach (var next in existingSettings)
                {
                    string sKey = next.Key;
                    if (!newSettings.ContainsKey(sKey))
                    {
                        settingName.Value = sKey;
                        cmd.ExecuteNonQuery();
                    }
                }
                SQLiteParameter settingValue = cmd.Parameters.Add("@SettingValue", DbType.String);
                foreach (var next in newSettings)
                {
                    string sKey = next.Key;
                    settingName.Value = sKey;
                    settingValue.Value = next.Value;
                    if (existingSettings.ContainsKey(sKey))
                        cmd.CommandText = "UPDATE Settings SET SettingValue = @SettingValue WHERE SettingName = @SettingName AND UserID = @UserID";
                    else
                        cmd.CommandText = "INSERT INTO Settings (UserID, SettingName, SettingValue) VALUES (@UserID, @SettingName, @SettingValue)";
                    cmd.ExecuteNonQuery();
                }

                HashSet<string> existingKeys = new HashSet<string>();
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = "SELECT Key FROM DynamicItemData WHERE UserID = @UserID";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingKeys.Add(reader["Key"].ToString());
                    }
                }
                SQLiteParameter keyParameter = cmd.Parameters.Add("@Key", DbType.String);
                foreach (KeyValuePair<ItemTypeEnum, DynamicItemData> nextDID in DynamicItemData)
                {
                    SaveDynamicItemDataRow(nextDID.Key.ToString(), nextDID.Value, cmd, keyParameter, existingKeys);
                }
                foreach (KeyValuePair<DynamicDataItemClass, DynamicItemData> nextDID in DynamicItemClassData)
                {
                    SaveDynamicItemDataRow(nextDID.Key.ToString(), nextDID.Value, cmd, keyParameter, existingKeys);
                }
                foreach (string nextItemName in existingKeys)
                {
                    keyParameter.Value = nextItemName;
                    cmd.CommandText = "DELETE FROM DynamicItemData WHERE UserID = @UserID AND Key = @Key";
                    cmd.ExecuteNonQuery();
                }

                SaveAreasToDatabase(conn, userID);
                SaveLocationsToDatabase(conn, userID);
                SaveStrategiesToDatabase(conn, userID);
                SavePermRunsToDatabase(conn, userID);
            }
        }

        private void SaveAreasToDatabase(SQLiteConnection conn, int userID)
        {
            List<string> baseRecordColumns = new List<string>()
            {
                "ParentID",
                "OrderValue",
                "DisplayName",
                "TickRoom",
                "PawnShop",
                "InventorySinkRoom",
            };
            string sInsertBaseRecordCommand = GetInsertCommand("Areas", baseRecordColumns);
            string sUpdateBaseRecordCommand = GetUpdateCommand("Areas", baseRecordColumns, "ID");

            HashSet<int> existingIDs = GetExistingIDs(conn, userID, "Areas", "ID");
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", userID);

                SQLiteParameter idParameter = cmd.Parameters.Add("@ID", DbType.Int32);
                SQLiteParameter parentIDParameter = cmd.Parameters.Add("@ParentID", DbType.Int32);
                SQLiteParameter orderParam = cmd.Parameters.Add("@OrderValue", DbType.Int32);
                SQLiteParameter displayNameParam = cmd.Parameters.Add("@DisplayName", DbType.String);
                SQLiteParameter tickRoomParam = cmd.Parameters.Add("@TickRoom", DbType.Int32);
                SQLiteParameter pawnShopParam = cmd.Parameters.Add("@PawnShop", DbType.Int32);
                SQLiteParameter inventorySinkRoomParam = cmd.Parameters.Add("@InventorySinkRoom", DbType.String);

                if (HomeArea != null)
                {
                    int iOrder = 0;
                    foreach (Area nextRecord in EnumerateAreas(HomeArea))
                    {
                        parentIDParameter.Value = nextRecord.Parent == null ? (object)DBNull.Value : nextRecord.Parent.ID;
                        orderParam.Value = ++iOrder;
                        displayNameParam.Value = nextRecord.DisplayName;
                        tickRoomParam.Value = nextRecord.TickRoom.HasValue ? (object)Convert.ToInt32(nextRecord.TickRoom.Value) : DBNull.Value;
                        pawnShopParam.Value = nextRecord.PawnShop.HasValue ? (object)Convert.ToInt32(nextRecord.PawnShop.Value) : DBNull.Value;
                        inventorySinkRoomParam.Value = string.IsNullOrEmpty(nextRecord.InventorySinkRoomIdentifier) ? (object)DBNull.Value : nextRecord.InventorySinkRoomIdentifier;

                        int iID = nextRecord.ID;
                        bool isNew = iID == 0;
                        iID = SaveRecord(cmd, nextRecord.ID, sInsertBaseRecordCommand, sUpdateBaseRecordCommand, idParameter, existingIDs);
                        nextRecord.ID = iID;
                        if (nextRecord.Children != null)
                        {
                            foreach (Area aChild in nextRecord.Children)
                            {
                                aChild.ParentID = iID;
                            }
                        }
                    }
                }
                DeleteUnprocessedIDs(existingIDs, conn, "Areas", "ID");
            }
        }

        public IEnumerable<Area> EnumerateAreas()
        {
            if (HomeArea != null)
            {
                foreach (Area a in EnumerateAreas(HomeArea))
                {
                    yield return a;
                }
            }
        }

        private IEnumerable<Area> EnumerateAreas(Area a)
        {
            yield return a;
            if (a.Children != null)
            {
                foreach (Area aChild in a.Children)
                {
                    foreach (Area aEnumerated in EnumerateAreas(aChild))
                    {
                        yield return aEnumerated;
                    }
                }
            }
        }

        private void SavePermRunsToDatabase(SQLiteConnection conn, int userID)
        {
            List<string> baseRecordColumns = new List<string>()
            {
                "OrderValue",
                "DisplayName",
                "Rehome",
                "BeforeFull",
                "AfterFull",
                "SpellsToCast",
                "SpellsToPotion",
                "SkillsToRun",
                "SupportedKeys",
                "TargetRoom",
                "ThresholdRoom",
                "MobText",
                "MobIndex",
                "StrategyID",
                "UseMagicCombat",
                "UseMeleeCombat",
                "UsePotionsCombat",
                "AfterKillMonsterAction",
                "AutoSpellLevelMin",
                "AutoSpellLevelMax",
                "Realms",
                "ItemsToProcessType",
                "LastCompleted",
            };
            string sInsertBaseRecordCommand = GetInsertCommand("PermRuns", baseRecordColumns);
            string sUpdateBaseRecordCommand = GetUpdateCommand("PermRuns", baseRecordColumns, "ID");

            HashSet<string> existingPermRunToAreaRecords = GetExistingIDsForCompoundKey(conn, userID, "SELECT p2a.PermRunID,p2a.AreaID FROM PermRunToAreas p2a INNER JOIN PermRuns p ON p.ID = p2a.PermRunID INNER JOIN Areas a ON a.ID = p2a.AreaID WHERE p.UserID = @UserID AND a.UserID = @UserID", 2);

            HashSet<int> existingIDs = GetExistingIDs(conn, userID, "PermRuns", "ID");
            using (SQLiteCommand cmdSavePermRun = conn.CreateCommand())
            using (SQLiteCommand cmdSavePermRunToArea = conn.CreateCommand())
            {
                cmdSavePermRun.Parameters.AddWithValue("@UserID", userID);

                SQLiteParameter idParameter = cmdSavePermRun.Parameters.Add("@ID", DbType.Int32);
                SQLiteParameter orderParam = cmdSavePermRun.Parameters.Add("@OrderValue", DbType.Int32);
                SQLiteParameter displayNameParam = cmdSavePermRun.Parameters.Add("@DisplayName", DbType.String);
                SQLiteParameter rehomeParam = cmdSavePermRun.Parameters.Add("@Rehome", DbType.Int32);
                SQLiteParameter beforeFullParam = cmdSavePermRun.Parameters.Add("@BeforeFull", DbType.Int32);
                SQLiteParameter afterFullParam = cmdSavePermRun.Parameters.Add("@AfterFull", DbType.Int32);
                SQLiteParameter spellsToCastParam = cmdSavePermRun.Parameters.Add("@SpellsToCast", DbType.Int32);
                SQLiteParameter spellsToPotionParam = cmdSavePermRun.Parameters.Add("@SpellsToPotion", DbType.Int32);
                SQLiteParameter skillsToRunParam = cmdSavePermRun.Parameters.Add("@SkillsToRun", DbType.Int32);
                SQLiteParameter supportedKeysParam = cmdSavePermRun.Parameters.Add("@SupportedKeys", DbType.Int32);
                SQLiteParameter targetRoomParam = cmdSavePermRun.Parameters.Add("@TargetRoom", DbType.String);
                SQLiteParameter thresholdRoomParam = cmdSavePermRun.Parameters.Add("@ThresholdRoom", DbType.String);
                SQLiteParameter mobTextParam = cmdSavePermRun.Parameters.Add("@MobText", DbType.String);
                SQLiteParameter mobIndexParam = cmdSavePermRun.Parameters.Add("@MobIndex", DbType.Int32);
                SQLiteParameter strategyIDParam = cmdSavePermRun.Parameters.Add("@StrategyID", DbType.Int32);
                SQLiteParameter useMagicCombatParam = cmdSavePermRun.Parameters.Add("@UseMagicCombat", DbType.Int32);
                SQLiteParameter useMeleeCombatParam = cmdSavePermRun.Parameters.Add("@UseMeleeCombat", DbType.Int32);
                SQLiteParameter usePotionsCombatParam = cmdSavePermRun.Parameters.Add("@UsePotionsCombat", DbType.Int32);
                SQLiteParameter afterKillMonsterActionParam = cmdSavePermRun.Parameters.Add("@AfterKillMonsterAction", DbType.Int32);
                SQLiteParameter autoSpellLevelMinParam = cmdSavePermRun.Parameters.Add("@AutoSpellLevelMin", DbType.Int32);
                SQLiteParameter autoSpellLevelMaxParam = cmdSavePermRun.Parameters.Add("@AutoSpellLevelMax", DbType.Int32);
                SQLiteParameter realmsParam = cmdSavePermRun.Parameters.Add("@Realms", DbType.Int32);
                SQLiteParameter itemsToProcessTypeParam = cmdSavePermRun.Parameters.Add("@ItemsToProcessType", DbType.Int32);
                SQLiteParameter lastCompletedParam = cmdSavePermRun.Parameters.Add("@LastCompleted", DbType.Int64);

                SQLiteParameter saveP2APermRunID = cmdSavePermRunToArea.Parameters.Add("@PermRunID", DbType.Int32);
                SQLiteParameter saveP2AAreaID = cmdSavePermRunToArea.Parameters.Add("@AreaID", DbType.Int32);
                cmdSavePermRunToArea.CommandText = "INSERT INTO PermRunToAreas (PermRunID, AreaID) VALUES (@PermRunID, @AreaID)";

                int iOrder = 0;
                foreach (PermRun nextRecord in PermRuns)
                {
                    orderParam.Value = ++iOrder;
                    displayNameParam.Value = string.IsNullOrEmpty(nextRecord.DisplayName) ? (object)DBNull.Value : nextRecord.DisplayName;
                    rehomeParam.Value = nextRecord.Rehome;
                    beforeFullParam.Value = nextRecord.BeforeFull == FullType.None ? (object)DBNull.Value : Convert.ToInt32(nextRecord.BeforeFull);
                    afterFullParam.Value = nextRecord.AfterFull == FullType.None ? (object)DBNull.Value : Convert.ToInt32(nextRecord.AfterFull);
                    spellsToCastParam.Value = Convert.ToInt32(nextRecord.SpellsToCast);
                    spellsToPotionParam.Value = Convert.ToInt32(nextRecord.SpellsToPotion);
                    skillsToRunParam.Value = Convert.ToInt32(nextRecord.SkillsToRun);
                    supportedKeysParam.Value = Convert.ToInt32(nextRecord.SupportedKeys);
                    targetRoomParam.Value = nextRecord.TargetRoomIdentifier;
                    thresholdRoomParam.Value = string.IsNullOrEmpty(nextRecord.ThresholdRoomIdentifier) ? (object)DBNull.Value : nextRecord.ThresholdRoomIdentifier;

                    string sMobText;
                    if (nextRecord.MobType.HasValue)
                        sMobText = nextRecord.MobType.Value.ToString();
                    else
                        sMobText = nextRecord.MobText;
                    mobTextParam.Value = string.IsNullOrEmpty(sMobText) ? (object)DBNull.Value : sMobText;

                    int iMobIndexThreshold;
                    int iMobIndex = nextRecord.MobIndex;
                    if (string.IsNullOrEmpty(sMobText))
                        iMobIndexThreshold = 1;
                    else
                        iMobIndexThreshold = 2;
                    mobIndexParam.Value = iMobIndex >= iMobIndexThreshold ? (object)iMobIndex : DBNull.Value;

                    strategyIDParam.Value = nextRecord.Strategy.ID;
                    useMagicCombatParam.Value = nextRecord.UseMagicCombat.HasValue ? (object)(nextRecord.UseMagicCombat.Value ? 1 : 0) : DBNull.Value;
                    useMeleeCombatParam.Value = nextRecord.UseMeleeCombat.HasValue ? (object)(nextRecord.UseMeleeCombat.Value ? 1 : 0) : DBNull.Value;
                    usePotionsCombatParam.Value = nextRecord.UsePotionsCombat.HasValue ? (object)(nextRecord.UsePotionsCombat.Value ? 1 : 0) : DBNull.Value;
                    afterKillMonsterActionParam.Value = nextRecord.AfterKillMonsterAction.HasValue ? (object)Convert.ToInt32(nextRecord.AfterKillMonsterAction.Value) : DBNull.Value;
                    autoSpellLevelMinParam.Value = nextRecord.AutoSpellLevelMin != AUTO_SPELL_LEVEL_NOT_SET ? (object)nextRecord.AutoSpellLevelMin : DBNull.Value;
                    autoSpellLevelMaxParam.Value = nextRecord.AutoSpellLevelMax != AUTO_SPELL_LEVEL_NOT_SET ? (object)nextRecord.AutoSpellLevelMax : DBNull.Value;
                    realmsParam.Value = nextRecord.Realms.HasValue ? (object)Convert.ToInt32(nextRecord.Realms.Value) : DBNull.Value;
                    itemsToProcessTypeParam.Value = Convert.ToInt32(nextRecord.ItemsToProcessType);
                    lastCompletedParam.Value = nextRecord.LastCompleted == DateTime.MinValue ? (object)DBNull.Value : nextRecord.LastCompleted.Ticks;

                    int iID = nextRecord.ID;
                    bool isNew = iID == 0;
                    iID = SaveRecord(cmdSavePermRun, nextRecord.ID, sInsertBaseRecordCommand, sUpdateBaseRecordCommand, idParameter, existingIDs);
                    nextRecord.ID = iID;

                    if (nextRecord.Areas != null)
                    {
                        saveP2APermRunID.Value = iID;
                        foreach (Area a in nextRecord.Areas)
                        {
                            bool saveRecord;
                            if (isNew)
                            {
                                saveRecord = true;
                            }
                            else
                            {
                                string sKey = iID + "," + a.ID;
                                saveRecord = !existingPermRunToAreaRecords.Contains(sKey);
                                if (!saveRecord) existingPermRunToAreaRecords.Remove(sKey);
                            }
                            if (saveRecord)
                            {
                                saveP2AAreaID.Value = a.ID;
                                cmdSavePermRunToArea.ExecuteNonQuery();
                            }
                        }
                    }
                }

                DeleteUnprocessedIDs(existingIDs, conn, "PermRuns", "ID");
                DeleteExistingIDsForCompoundKey(conn, existingPermRunToAreaRecords, "DELETE FROM PermRunToAreas WHERE PermRunID = @ID1 AND AreaID = @ID2", 2);
            }
        }

        private void SaveStrategiesToDatabase(SQLiteConnection conn, int userID)
        {
            List<string> baseRecordColumns = new List<string>()
            {
                    "OrderValue",
                    "DisplayName",
                    "AfterKillMonsterAction",
                    "ManaPool",
                    "FinalMagicAction",
                    "FinalMeleeAction",
                    "FinalPotionsAction",
                    "MagicOnlyWhenStunnedForXMS",
                    "MeleeOnlyWhenStunnedForXMS",
                    "PotionsOnlyWhenStunnedForXMS",
                    "TypesToRunLastCommandIndefinitely",
                    "TypesWithStepsEnabled",
                    "AutoSpellLevelMin",
                    "AutoSpellLevelMax",
                    "Realms",
            };
            string sInsertBaseRecordCommand = GetInsertCommand("Strategies", baseRecordColumns);
            string sUpdateBaseRecordCommand = GetUpdateCommand("Strategies", baseRecordColumns, "ID");

            HashSet<string> existingStrategySteps = GetExistingIDsForCompoundKey(conn, userID, "SELECT ss.StrategyID,ss.CombatType,ss.IndexValue FROM StrategySteps ss INNER JOIN Strategies s ON s.ID = ss.StrategyID WHERE s.UserID = @UserID", 3);

            HashSet<int> existingIDs = GetExistingIDs(conn, userID, "Strategies", "ID");
            using (SQLiteCommand cmd = conn.CreateCommand())
            using (SQLiteCommand strategyStepCommand = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", userID);

                SQLiteParameter idParameter = cmd.Parameters.Add("@ID", DbType.Int32);
                SQLiteParameter orderParam = cmd.Parameters.Add("@OrderValue", DbType.Int32);
                SQLiteParameter displayNameParam = cmd.Parameters.Add("@DisplayName", DbType.String);
                SQLiteParameter afterKillMonsterActionParam = cmd.Parameters.Add("@AfterKillMonsterAction", DbType.Int32);
                SQLiteParameter manaPoolParam = cmd.Parameters.Add("@ManaPool", DbType.Int32);
                SQLiteParameter finalMagicActionParam = cmd.Parameters.Add("@FinalMagicAction", DbType.Int32);
                SQLiteParameter finalMeleeActionParam = cmd.Parameters.Add("@FinalMeleeAction", DbType.Int32);
                SQLiteParameter finalPotionsActionParam = cmd.Parameters.Add("@FinalPotionsAction", DbType.Int32);
                SQLiteParameter magicOnlyWhenStunnedForXMSParam = cmd.Parameters.Add("@MagicOnlyWhenStunnedForXMS", DbType.Int32);
                SQLiteParameter meleeOnlyWhenStunnedForXMSParam = cmd.Parameters.Add("@MeleeOnlyWhenStunnedForXMS", DbType.Int32);
                SQLiteParameter potionsOnlyWhenStunnedForXMSParam = cmd.Parameters.Add("@PotionsOnlyWhenStunnedForXMS", DbType.Int32);
                SQLiteParameter typesToRunLastCommandIndefinitelyParam = cmd.Parameters.Add("@TypesToRunLastCommandIndefinitely", DbType.Int32);
                SQLiteParameter typesWithStepsEnabledParam = cmd.Parameters.Add("@TypesWithStepsEnabled", DbType.Int32);
                SQLiteParameter autoSpellLevelMinParam = cmd.Parameters.Add("@AutoSpellLevelMin", DbType.Int32);
                SQLiteParameter autoSpellLevelMaxParam = cmd.Parameters.Add("@AutoSpellLevelMax", DbType.Int32);
                SQLiteParameter realmsParam = cmd.Parameters.Add("@Realms", DbType.Int32);

                string sInsertStrategyStepCommand = "INSERT INTO StrategySteps (StrategyID,CombatType,IndexValue,StepType) VALUES (@StrategyID,@CombatType,@IndexValue,@StepType)";
                string sUpdateStrategyStepCommand = "UPDATE StrategySteps SET StepType = @StepType WHERE StrategyID = @StrategyID AND CombatType = @CombatType AND IndexValue = @IndexValue";

                SQLiteParameter strategyStepStrategyID = strategyStepCommand.Parameters.Add("@StrategyID", DbType.Int32);
                SQLiteParameter strategyStepCombatType = strategyStepCommand.Parameters.Add("@CombatType", DbType.Int32);
                SQLiteParameter strategyStepIndex = strategyStepCommand.Parameters.Add("@IndexValue", DbType.Int32);
                SQLiteParameter strategyStepStepType = strategyStepCommand.Parameters.Add("@StepType", DbType.Int32);

                int iOrder = 0;
                foreach (Strategy nextRecord in Strategies)
                {
                    orderParam.Value = ++iOrder;
                    displayNameParam.Value = string.IsNullOrEmpty(nextRecord.DisplayName) ? (object)DBNull.Value : nextRecord.DisplayName;
                    afterKillMonsterActionParam.Value = Convert.ToInt32(nextRecord.AfterKillMonsterAction);
                    manaPoolParam.Value = nextRecord.ManaPool > 0 ? (object)nextRecord.ManaPool : DBNull.Value;
                    finalMagicActionParam.Value = Convert.ToInt32(nextRecord.FinalMagicAction);
                    finalMeleeActionParam.Value = Convert.ToInt32(nextRecord.FinalMeleeAction);
                    finalPotionsActionParam.Value = Convert.ToInt32(nextRecord.FinalPotionsAction);
                    magicOnlyWhenStunnedForXMSParam.Value = nextRecord.MagicOnlyWhenStunnedForXMS.HasValue ? (object)nextRecord.MagicOnlyWhenStunnedForXMS.Value : DBNull.Value;
                    meleeOnlyWhenStunnedForXMSParam.Value = nextRecord.MeleeOnlyWhenStunnedForXMS.HasValue ? (object)nextRecord.MeleeOnlyWhenStunnedForXMS.Value : DBNull.Value;
                    potionsOnlyWhenStunnedForXMSParam.Value = nextRecord.PotionsOnlyWhenStunnedForXMS.HasValue ? (object)nextRecord.PotionsOnlyWhenStunnedForXMS.Value : DBNull.Value;
                    typesToRunLastCommandIndefinitelyParam.Value = Convert.ToInt32(nextRecord.TypesToRunLastCommandIndefinitely);
                    typesWithStepsEnabledParam.Value = Convert.ToInt32(nextRecord.TypesWithStepsEnabled);
                    autoSpellLevelMinParam.Value = nextRecord.AutoSpellLevelMin > 0 ? (object)nextRecord.AutoSpellLevelMin : DBNull.Value;
                    autoSpellLevelMaxParam.Value = nextRecord.AutoSpellLevelMax > 0 ? (object)nextRecord.AutoSpellLevelMax : DBNull.Value;
                    realmsParam.Value = nextRecord.Realms.HasValue ? (object)Convert.ToInt32(nextRecord.Realms.Value) : DBNull.Value;

                    int iID = nextRecord.ID;
                    bool isNew = iID == 0;
                    iID = SaveRecord(cmd, nextRecord.ID, sInsertBaseRecordCommand, sUpdateBaseRecordCommand, idParameter, existingIDs);
                    nextRecord.ID = iID;

                    strategyStepStrategyID.Value = iID;

                    int iCombatType;
                    List<int> steps;

                    iCombatType = (int)CommandType.Magic;
                    strategyStepCombatType.Value = iCombatType;
                    steps = new List<int>();
                    if (nextRecord.MagicSteps != null)
                    {
                        foreach (var next in nextRecord.MagicSteps)
                        {
                            steps.Add(Convert.ToInt32(next));
                        }
                    }
                    ProcessStrategyStepsForCombatType(existingStrategySteps, iCombatType, steps, isNew, iID, strategyStepCommand, sInsertStrategyStepCommand, sUpdateStrategyStepCommand, strategyStepIndex, strategyStepStepType);

                    iCombatType = (int)CommandType.Melee;
                    strategyStepCombatType.Value = iCombatType;
                    steps = new List<int>();
                    if (nextRecord.MeleeSteps != null)
                    {
                        foreach (var next in nextRecord.MeleeSteps)
                        {
                            steps.Add(Convert.ToInt32(next));
                        }
                    }
                    ProcessStrategyStepsForCombatType(existingStrategySteps, iCombatType, steps, isNew, iID, strategyStepCommand, sInsertStrategyStepCommand, sUpdateStrategyStepCommand, strategyStepIndex, strategyStepStepType);

                    iCombatType = (int)CommandType.Potions;
                    strategyStepCombatType.Value = iCombatType;
                    steps = new List<int>();
                    if (nextRecord.PotionsSteps != null)
                    {
                        foreach (var next in nextRecord.PotionsSteps)
                        {
                            steps.Add(Convert.ToInt32(next));
                        }
                    }
                    ProcessStrategyStepsForCombatType(existingStrategySteps, iCombatType, steps, isNew, iID, strategyStepCommand, sInsertStrategyStepCommand, sUpdateStrategyStepCommand, strategyStepIndex, strategyStepStepType);
                }

                DeleteUnprocessedIDs(existingIDs, conn, "Strategies", "ID");
                DeleteExistingIDsForCompoundKey(conn, existingStrategySteps, "DELETE FROM StrategySteps WHERE StrategyID = @ID1 AND CombatType = @ID2 AND IndexValue = @ID3", 3);
            }
        }

        private int SaveRecord(SQLiteCommand cmd, int id, string insertCommand, string updateCommand, SQLiteParameter idParameter, HashSet<int> existingIDs)
        {
            bool isNew = id == 0;
            string sCommand;
            if (isNew)
            {
                sCommand = insertCommand;
            }
            else
            {
                idParameter.Value = id;
                sCommand = updateCommand;
                existingIDs.Remove(id);
            }
            cmd.CommandText = sCommand;
            cmd.ExecuteNonQuery();
            if (isNew)
            {
                cmd.CommandText = "SELECT last_insert_rowid()";
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return id;
        }

        private void ProcessStrategyStepsForCombatType(HashSet<string> existingKeys, int combatType, List<int> steps, bool isNewStrategy, int strategyID, SQLiteCommand strategyStepCommand, string insertCommand, string updateCommand, SQLiteParameter indexParameter, SQLiteParameter stepTypeParameter)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                bool insert;
                if (isNewStrategy)
                {
                    insert = true;
                }
                else
                {
                    string key = strategyID + "," + combatType.ToString() + "," + i;
                    if (existingKeys.Contains(key))
                    {
                        insert = false;
                        existingKeys.Remove(key);
                    }
                    else
                    {
                        insert = true;
                    }
                }
                strategyStepCommand.CommandText = insert ? insertCommand : updateCommand;
                indexParameter.Value = i;
                stepTypeParameter.Value = Convert.ToInt32(steps[i]);
                strategyStepCommand.ExecuteNonQuery();
            }
        }

        private void SaveLocationsToDatabase(SQLiteConnection conn, int userID)
        {
            HashSet<int> existingIDs = GetExistingIDs(conn, userID, "LocationNodes", "ID");
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                SQLiteParameter orderParam = cmd.Parameters.Add("@OrderValue", DbType.Int32);
                SQLiteParameter displayNameParam = cmd.Parameters.Add("@DisplayName", DbType.String);
                SQLiteParameter roomParameter = cmd.Parameters.Add("@Room", DbType.String);
                SQLiteParameter expandedParameter = cmd.Parameters.Add("@Expanded", DbType.Int32);
                SQLiteParameter parentIDParameter = cmd.Parameters.Add("@ParentID", DbType.Int32);
                SQLiteParameter idParameter = cmd.Parameters.Add("@ID", DbType.Int32);

                List<string> columns = new List<string>()
                {
                    "OrderValue",
                    "DisplayName",
                    "Room",
                    "Expanded",
                    "ParentID",
                };
                string sInsertCommand = GetInsertCommand("LocationNodes", columns);
                string sUpdateCommand = GetUpdateCommand("LocationNodes", columns, "ID");

                int iOrder = 0;
                foreach (LocationNode nextLoc in EnumerateLocations())
                {
                    orderParam.Value = ++iOrder;
                    displayNameParam.Value = string.IsNullOrEmpty(nextLoc.DisplayName) ? (object)DBNull.Value : nextLoc.DisplayName;
                    roomParameter.Value = string.IsNullOrEmpty(nextLoc.Room) ? (object)DBNull.Value : nextLoc.Room;
                    expandedParameter.Value = nextLoc.Expanded ? 1 : 0;
                    parentIDParameter.Value = nextLoc.Parent == null ? (object)DBNull.Value : nextLoc.Parent.ID;
                    int iID = nextLoc.ID;
                    bool isNew = iID == 0;
                    string sCommand;
                    if (isNew)
                    {
                        sCommand = sInsertCommand;
                    }
                    else
                    {
                        idParameter.Value = iID;
                        sCommand = sUpdateCommand;
                        existingIDs.Remove(iID);
                    }
                    cmd.CommandText = sCommand;
                    cmd.ExecuteNonQuery();
                    if (isNew)
                    {
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        iID = Convert.ToInt32(cmd.ExecuteScalar());
                        nextLoc.ID = iID;
                        if (nextLoc.Children != null)
                        {
                            foreach (LocationNode nextChild in nextLoc.Children)
                            {
                                nextChild.ParentID = iID;
                            }
                        }
                    }
                }
            }
            DeleteUnprocessedIDs(existingIDs, conn, "LocationNodes", "ID");
        }

        private string GetInsertCommand(string tableName, List<string> columns)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(tableName);
            sb.Append("(UserID");
            foreach (string sNext in columns)
            {
                sb.Append(",");
                sb.Append(sNext);
            }
            sb.Append(") VALUES (@UserID");
            foreach (string sNext in columns)
            {
                sb.Append(",@");
                sb.Append(sNext);
            }
            sb.Append(")");
            return sb.ToString();
        }

        private string GetUpdateCommand(string tableName, List<string> columns, string idColumnName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE ");
            sb.Append(tableName);
            sb.Append(" SET ");
            bool first = true;
            foreach (string column in columns)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(column);
                sb.Append("=@");
                sb.Append(column);
            }
            sb.Append(" WHERE ");
            sb.Append(idColumnName);
            sb.Append(" = @");
            sb.Append(idColumnName);
            return sb.ToString();
        }

        private void DeleteUnprocessedIDs(HashSet<int> ids, SQLiteConnection conn, string tableName, string idColumnName)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = $"DELETE FROM {tableName} WHERE {idColumnName} = @{idColumnName}";
                SQLiteParameter idParameter = cmd.Parameters.Add($"@{idColumnName}", DbType.Int32);
                foreach (int iID in ids)
                {
                    idParameter.Value = iID;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private HashSet<int> GetExistingIDs(SQLiteConnection conn, int userID, string tableName, string idColumnName)
        {
            HashSet<int> ret = new HashSet<int>();
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = $"SELECT {idColumnName} FROM {tableName} WHERE UserID = @UserID";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(Convert.ToInt32(reader[idColumnName]));
                    }
                }
            }
            return ret;
        }

        private void DeleteExistingIDsForCompoundKey(SQLiteConnection conn, HashSet<string> keys, string sql, int numberOfKeys)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                List<SQLiteParameter> idParams = new List<SQLiteParameter>();
                for (int i = 1; i <= numberOfKeys; i++)
                {
                    idParams.Add(cmd.Parameters.Add("@ID" + i, DbType.Int32));
                }
                foreach (string sKey in keys)
                {
                    string[] split = sKey.Split(new char[] { ',' });
                    for (int i = 0; i < numberOfKeys; i++)
                    {
                        idParams[i].Value = int.Parse(split[i]);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private HashSet<string> GetExistingIDsForCompoundKey(SQLiteConnection conn, int userID, string command, int numberOfKeys)
        {
            HashSet<string> existingKeys = new HashSet<string>();
            StringBuilder sb = new StringBuilder();
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = command;
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sb.Clear();
                        for (int i = 0; i < numberOfKeys; i++)
                        {
                            if (i != 0) sb.Append(",");
                            sb.Append(reader[i].ToString());
                        }
                        existingKeys.Add(sb.ToString());
                    }
                }
            }
            return existingKeys;
        }

        private IEnumerable<LocationNode> EnumerateLocations()
        {
            foreach (LocationNode nextLoc in Locations)
            {
                yield return nextLoc;
                foreach (LocationNode nextSub in nextLoc.GetChildNodes())
                {
                    yield return nextSub;
                }
            }
        }

        private void SaveDynamicItemDataRow(string key, DynamicItemData did, SQLiteCommand cmd, SQLiteParameter keyParameter, HashSet<string> existingKeys)
        {
            keyParameter.Value = key;
            string sql;
            string sKeepCount = did.KeepCount >= 0 ? did.KeepCount.ToString() : "NULL";
            string sSinkCount = did.SinkCount >= 0 ? did.SinkCount.ToString() : "NULL";
            string sOverflowAction = did.OverflowAction == ItemInventoryOverflowAction.None ? "NULL" : Convert.ToInt32(did.OverflowAction).ToString();
            if (existingKeys.Contains(key))
                sql = $"UPDATE DynamicItemData SET KeepCount = {sKeepCount}, SinkCount = {sSinkCount}, OverflowAction = {sOverflowAction} WHERE UserID = @UserID AND Key = @Key";
            else
                sql = $"INSERT INTO DynamicItemData (UserID, Key, KeepCount, SinkCount, OverflowAction) VALUES (@UserID, @Key, {sKeepCount}, {sSinkCount}, {sOverflowAction})";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            existingKeys.Remove(key);
        }

        private bool ValidateStrategy(Strategy s, List<string> errorMessages)
        {
            bool ret = true;
            if (!ValidateNonRequiredAutoSpellMinMax(s.AutoSpellLevelMin, s.AutoSpellLevelMax, errorMessages, "strategy"))
            {
                ret = false;
            }
            if (s.Realms.HasValue && s.Realms.Value == RealmTypeFlags.None)
            {
                errorMessages.Add($"Invalid realms: {s.Realms.Value}");
                ret = false;
            }
            return ret;
        }

        private void ValidateSettings(List<string> errorMessages)
        {
            if (AutoEscapeType != AutoEscapeType.Flee && AutoEscapeType != AutoEscapeType.Hazy)
            {
                errorMessages.Add($"Invalid auto escape type found ({AutoEscapeType}), using Flee");
                AutoEscapeType = AutoEscapeType.Flee;
            }
            if (AutoEscapeThreshold < 0)
            {
                errorMessages.Add($"Invalid auto escape threshold found ({AutoEscapeThreshold}), using no threshold.");
                AutoEscapeThreshold = 0;
            }
            if (AutoEscapeThreshold == 0 && AutoEscapeActive)
            {
                errorMessages.Add($"Auto escape cannot be active without a threshold, using inactive.");
                AutoEscapeActive = false;
            }
            if (AutoSpellLevelMin > AutoSpellLevelMax || AutoSpellLevelMax < AUTO_SPELL_LEVEL_MINIMUM || AutoSpellLevelMax > AUTO_SPELL_LEVEL_MAXIMUM || AutoSpellLevelMin < AUTO_SPELL_LEVEL_MINIMUM || AutoSpellLevelMin > AUTO_SPELL_LEVEL_MAXIMUM)
            {
                errorMessages.Add($"Invalid auto spell min/max found: {AutoSpellLevelMin}-{AutoSpellLevelMax}, using {AUTO_SPELL_LEVEL_MINIMUM}-{AUTO_SPELL_LEVEL_MAXIMUM}");
                AutoSpellLevelMin = AUTO_SPELL_LEVEL_MINIMUM;
                AutoSpellLevelMax = AUTO_SPELL_LEVEL_MAXIMUM;
            }
            for (int i = PermRuns.Count - 1; i >= 0; i--)
            {
                PermRun p = PermRuns[i];
                bool isValid = true;
                if (!ValidateNonRequiredAutoSpellMinMax(p.AutoSpellLevelMin, p.AutoSpellLevelMax, errorMessages, "perm run"))
                {
                    isValid = false;
                }
                if (p.Realms.HasValue && p.Realms.Value == RealmTypeFlags.None)
                {
                    errorMessages.Add($"Invalid realms: {p.Realms.Value}");
                    isValid = false;
                }
                if (p.Rehome && p.Areas == null)
                {
                    isValid = false;
                    errorMessages.Add("Perm run set as rehome without an area.");
                }
                if (!isValid)
                {
                    PermRuns.RemoveAt(i);
                }
            }
        }

        private bool ValidateNonRequiredAutoSpellMinMax(int iMin, int iMax, List<string> errorMessages, string objectType)
        {
            bool hasMin = iMin > 0;
            bool hasMax = iMax > 0;
            bool isValid = true;
            bool ret = true;
            if (!hasMin && !hasMax)
                ret = true;
            else if (hasMin && hasMax)
                isValid = iMin <= iMax && iMax >= AUTO_SPELL_LEVEL_MINIMUM && iMax <= AUTO_SPELL_LEVEL_MAXIMUM && iMin >= AUTO_SPELL_LEVEL_MINIMUM && iMin <= AUTO_SPELL_LEVEL_MAXIMUM;
            else
                isValid = false;
            if (!isValid)
            {
                errorMessages.Add($"Invalid {objectType} auto spell min/max found: {iMin}-{iMax}");
                ret = false;
            }
            return ret;
        }

        private void HandleDynamicItemData(XmlElement dynamicItemData, List<string> errorMessages)
        {
            foreach (XmlNode nextNode in dynamicItemData.ChildNodes)
            {
                XmlElement elem = nextNode as XmlElement;
                if (elem == null) continue;
                if (elem.Name == "Info")
                {
                    string sName = elem.Attributes["key"]?.Value;
                    if (sName == null)
                    {
                        errorMessages.Add("Item dynamic data element missing key");
                    }
                    else
                    {
                        DynamicItemData did = new DynamicItemData();

                        did.KeepCount = ProcessNonNegativeIntAttributeWithAll(elem, "keep", -1, errorMessages);
                        did.SinkCount = ProcessNonNegativeIntAttributeWithAll(elem, "sink", -1, errorMessages);

                        string sValue = elem.Attributes["overflow"]?.Value;
                        int iValue = 0;
                        if (sValue != null)
                        {
                            if (Enum.TryParse(sValue, out ItemInventoryOverflowAction action))
                            {
                                if (action != ItemInventoryOverflowAction.Ignore && action != ItemInventoryOverflowAction.SellOrJunk)
                                {
                                    errorMessages.Add("Item dynamic data element with invalid overflow action: " + sValue);
                                }
                                else
                                {
                                    iValue = (int)action;
                                }
                            }
                            else
                            {
                                errorMessages.Add("Item dynamic data element with invalid overflow action: " + sValue);
                            }
                        }
                        did.OverflowAction = (ItemInventoryOverflowAction)iValue;

                        if (Enum.TryParse(sName, out ItemTypeEnum itemType))
                        {
                            if (DynamicItemData.ContainsKey(itemType))
                                errorMessages.Add("Duplicate item dynamic item element for " + sName);
                            else
                                DynamicItemData[itemType] = did;
                        }
                        else if (Enum.TryParse(sName, out DynamicDataItemClass itemClass))
                        {
                            if (DynamicItemClassData.ContainsKey(itemClass))
                                errorMessages.Add("Duplicate item dynamic class element for " + sName);
                            else
                                DynamicItemClassData[itemClass] = did;
                        }
                        else
                        {
                            errorMessages.Add("Item dynamic data element with invalid name: " + sName);
                        }
                    }
                }
            }
        }

        private int ProcessNonNegativeIntAttributeWithAll(XmlElement elem, string attributeName, int defaultValue, List<string> errorMessages)
        {
            string sValue = elem.Attributes[attributeName]?.Value;
            int iValue = defaultValue;
            if (sValue != null)
            {
                if (sValue.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    iValue = int.MaxValue;
                }
                else if (!int.TryParse(sValue, out iValue))
                {
                    errorMessages.Add($"Item dynamic data element with invalid {attributeName}: " + sValue);
                }
                if (iValue < 0) iValue = defaultValue;
            }
            return iValue;
        }

        private void HandleSettings(XmlElement settings, List<string> errorMessages)
        {
            foreach (XmlNode nextNode in settings.ChildNodes)
            {
                XmlElement elem = nextNode as XmlElement;
                if (elem == null) continue;
                if (elem.Name == "Setting")
                {
                    string sName = elem.Attributes["name"]?.Value;
                    if (sName == null)
                    {
                        errorMessages.Add("Setting element missing name");
                    }
                    else
                    {
                        string sValue = elem.Attributes["value"]?.Value;
                        HandleSetting(sName, sValue, errorMessages);
                    }
                }
                else
                {
                    errorMessages.Add("Invalid setting element: " + elem.Name);
                }
            }
        }

        private void HandleSetting(string sName, string sValue, List<string> errorMessages)
        {
            bool bValue;
            int iValue;
            ItemTypeEnum item;
            switch (sName)
            {
                case "Weapon":
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        if (Enum.TryParse(sValue, out item))
                            Weapon = item;
                        else
                            errorMessages.Add("Invalid weapon: " + item);
                    }
                    break;
                case "HeldItem":
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        if (Enum.TryParse(sValue, out item))
                            HeldItem = item;
                        else
                            errorMessages.Add("Invalid held item: " + item);
                    }
                    break;
                case "Realms":
                    if (!Enum.TryParse(sValue, out RealmTypeFlags realms))
                        errorMessages.Add("Invalid realms: " + sValue);
                    else if (realms == RealmTypeFlags.None)
                        errorMessages.Add("Cannot specify no realms: " + sValue);
                    else
                        Realms = realms;
                    break;
                case "PreferredAlignment":
                    if (Enum.TryParse(sValue, out AlignmentType alignment))
                        PreferredAlignment = alignment;
                    else
                        errorMessages.Add("Invalid preferred alignment: " + sValue);
                    break;
                case "ConsoleVerbosity":
                    if (Enum.TryParse(sValue, out ConsoleOutputVerbosity verbosity))
                        ConsoleVerbosity = verbosity;
                    else
                        errorMessages.Add("Invalid ConsoleVerbosity: " + sValue);
                    break;
                case "FullColor":
                    if (int.TryParse(sValue, out iValue))
                        FullColor = Color.FromArgb(iValue);
                    else
                        errorMessages.Add("Invalid FullColor: " + sValue);
                    break;
                case "EmptyColor":
                    if (int.TryParse(sValue, out iValue))
                        EmptyColor = Color.FromArgb(iValue);
                    else
                        errorMessages.Add("Invalid EmptyColor: " + sValue);
                    break;
                case "QueryMonsterStatus":
                    if (bool.TryParse(sValue, out bValue))
                        QueryMonsterStatus = bValue;
                    else
                        errorMessages.Add("Invalid QueryMonsterStatus: " + sValue);
                    break;
                case "AutoSpellLevelMin":
                    if (int.TryParse(sValue, out iValue))
                        AutoSpellLevelMin = iValue;
                    else
                        errorMessages.Add("Invalid AutoSpellLevelMin: " + sValue);
                    break;
                case "AutoSpellLevelMax":
                    if (int.TryParse(sValue, out iValue))
                        AutoSpellLevelMax = iValue;
                    else
                        errorMessages.Add("Invalid AutoSpellLevelMax: " + sValue);
                    break;
                case "RemoveAllOnStartup":
                    if (bool.TryParse(sValue, out bValue))
                        RemoveAllOnStartup = bValue;
                    else
                        errorMessages.Add("Invalid RemoveAllOnStartup: " + sValue);
                    break;
                case "DisplayStunLength":
                    if (bool.TryParse(sValue, out bValue))
                        DisplayStunLength = bValue;
                    else
                        errorMessages.Add("Invalid DisplayStunLength: " + sValue);
                    break;
                case "SaveSettingsOnQuit":
                    if (bool.TryParse(sValue, out bValue))
                        SaveSettingsOnQuit = bValue;
                    else
                        errorMessages.Add("Invalid SaveSettingsOnQuit: " + sValue);
                    break;
                case "AutoEscapeThreshold":
                    if (int.TryParse(sValue, out iValue))
                        AutoEscapeThreshold = iValue;
                    else
                        errorMessages.Add("Invalid AutoEscapeThreshold: " + sValue);
                    break;
                case "AutoEscapeType":
                    if (Enum.TryParse(sValue, out AutoEscapeType autoEscapeType))
                        AutoEscapeType = autoEscapeType;
                    else
                        errorMessages.Add("Invalid AutoEscapeType: " + sValue);
                    break;
                case "AutoEscapeActive":
                    if (bool.TryParse(sValue, out bValue))
                        AutoEscapeActive = bValue;
                    else
                        errorMessages.Add("Invalid AutoEscapeActive: " + sValue);
                    break;
                case "MagicVigorOnlyWhenDownXHP":
                    if (int.TryParse(sValue, out iValue))
                        MagicVigorOnlyWhenDownXHP = Math.Max(iValue, 0);
                    else
                        errorMessages.Add("Invalid MagicVigorOnlyWhenDownXHP: " + sValue);
                    break;
                case "MagicMendOnlyWhenDownXHP":
                    if (int.TryParse(sValue, out iValue))
                        MagicMendOnlyWhenDownXHP = Math.Max(iValue, 0);
                    else
                        errorMessages.Add("Invalid MagicMendOnlyWhenDownXHP: " + sValue);
                    break;
                case "PotionsVigorOnlyWhenDownXHP":
                    if (int.TryParse(sValue, out iValue))
                        PotionsVigorOnlyWhenDownXHP = Math.Max(iValue, 0);
                    else
                        errorMessages.Add("Invalid PotionsVigorOnlyWhenDownXHP: " + sValue);
                    break;
                case "PotionsMendOnlyWhenDownXHP":
                    if (int.TryParse(sValue, out iValue))
                        PotionsMendOnlyWhenDownXHP = Math.Max(iValue, 0);
                    else
                        errorMessages.Add("Invalid PotionsMendOnlyWhenDownXHP: " + sValue);
                    break;
                default:
                    errorMessages.Add("Invalid setting name: " + sName);
                    break;
            }
        }

        public void SaveToXmlWriter(XmlWriter writer)
        {
            int iPermRunOrder = 0;
            foreach (PermRun next in PermRuns)
            {
                next.OrderValue = ++iPermRunOrder;
            }
            writer.WriteStartElement("DynamicData");
            WriteSettingsXML(writer);
            WriteDynamicItemDataXML(writer);
            if (HomeArea != null) WriteAreaXML(writer, HomeArea);
            WriteLocationsXML(writer);
            WriteStrategiesXML(writer);
            writer.WriteEndElement();
        }

        private void WriteAreaXML(XmlWriter writer, Area a)
        {
            writer.WriteStartElement("Area");
            writer.WriteAttributeString("DisplayName", a.DisplayName);
            if (a.TickRoom.HasValue)
            {
                writer.WriteAttributeString("TickRoom", a.TickRoom.Value.ToString());
            }
            if (a.PawnShop.HasValue)
            {
                writer.WriteAttributeString("PawnShop", a.PawnShop.Value.ToString());
            }
            string sRoomIdentifier = a.InventorySinkRoomIdentifier;
            if (!string.IsNullOrEmpty(sRoomIdentifier))
            {
                writer.WriteAttributeString("InventorySinkRoom", sRoomIdentifier);
            }
            if (a.Children != null)
            {
                foreach (Area aChild in a.Children)
                {
                    WriteAreaXML(writer, aChild);
                }
            }
            writer.WriteEndElement();
        }

        private void WriteStrategiesXML(XmlWriter writer)
        {
            if (Strategies.Count > 0)
            {
                writer.WriteStartElement("Strategies");
                foreach (Strategy s in Strategies)
                {
                    writer.WriteStartElement("Strategy");
                    if (!string.IsNullOrEmpty(s.DisplayName)) writer.WriteAttributeString("DisplayName", s.DisplayName);
                    writer.WriteAttributeString("AfterKillMonsterAction", s.AfterKillMonsterAction.ToString());
                    if (s.ManaPool > 0) writer.WriteAttributeString("ManaPool", s.ManaPool.ToString());
                    writer.WriteAttributeString("FinalMagicAction", s.FinalMagicAction.ToString());
                    writer.WriteAttributeString("FinalMeleeAction", s.FinalMeleeAction.ToString());
                    writer.WriteAttributeString("FinalPotionsAction", s.FinalPotionsAction.ToString());
                    if (s.MagicOnlyWhenStunnedForXMS.HasValue) writer.WriteAttributeString("MagicOnlyWhenStunnedForXMS", s.MagicOnlyWhenStunnedForXMS.Value.ToString());
                    if (s.MeleeOnlyWhenStunnedForXMS.HasValue) writer.WriteAttributeString("MeleeOnlyWhenStunnedForXMS", s.MeleeOnlyWhenStunnedForXMS.Value.ToString());
                    if (s.PotionsOnlyWhenStunnedForXMS.HasValue) writer.WriteAttributeString("PotionsOnlyWhenStunnedForXMS", s.PotionsOnlyWhenStunnedForXMS.Value.ToString());
                    writer.WriteAttributeString("TypesToRunLastCommandIndefinitely", StringProcessing.TrimFlagsEnumToString(s.TypesToRunLastCommandIndefinitely));
                    writer.WriteAttributeString("TypesWithStepsEnabled", StringProcessing.TrimFlagsEnumToString(s.TypesWithStepsEnabled));
                    if (s.AutoSpellLevelMin != AUTO_SPELL_LEVEL_NOT_SET) writer.WriteAttributeString("AutoSpellLevelMin", s.AutoSpellLevelMin.ToString());
                    if (s.AutoSpellLevelMax != AUTO_SPELL_LEVEL_NOT_SET) writer.WriteAttributeString("AutoSpellLevelMax", s.AutoSpellLevelMax.ToString());
                    if (s.Realms.HasValue) writer.WriteAttributeString("Realms", StringProcessing.TrimFlagsEnumToString(s.Realms.Value));
                    if (s.MagicSteps != null)
                    {
                        foreach (MagicStrategyStep step in s.MagicSteps)
                        {
                            writer.WriteStartElement("Step");
                            writer.WriteAttributeString("combat", "Magic");
                            writer.WriteAttributeString("step", step.ToString());
                            writer.WriteEndElement();
                        }
                    }
                    if (s.MeleeSteps != null)
                    {
                        foreach (MeleeStrategyStep step in s.MeleeSteps)
                        {
                            writer.WriteStartElement("Step");
                            writer.WriteAttributeString("combat", "Melee");
                            writer.WriteAttributeString("step", step.ToString());
                            writer.WriteEndElement();
                        }
                    }
                    if (s.PotionsSteps != null)
                    {
                        foreach (PotionsStrategyStep step in s.PotionsSteps)
                        {
                            writer.WriteStartElement("Step");
                            writer.WriteAttributeString("combat", "Potions");
                            writer.WriteAttributeString("step", step.ToString());
                            writer.WriteEndElement();
                        }
                    }

                    List<PermRun> permRunsForStrategy = new List<PermRun>();
                    foreach (PermRun p in PermRuns)
                    {
                        if (p.Strategy == s)
                        {
                            permRunsForStrategy.Add(p);
                        }
                    }
                    if (permRunsForStrategy.Count > 0)
                    {
                        foreach (PermRun p in permRunsForStrategy)
                        {
                            writer.WriteStartElement("PermRun");
                            writer.WriteAttributeString("Order", p.OrderValue.ToString());
                            if (!string.IsNullOrEmpty(p.DisplayName))
                            {
                                writer.WriteAttributeString("DisplayName", p.DisplayName);
                            }
                            if (p.Areas != null)
                            {
                                writer.WriteAttributeString("Rehome", p.Rehome.ToString());
                                StringBuilder sbAreas = new StringBuilder();
                                bool first = true;
                                foreach (Area a in p.Areas)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        sbAreas.Append(",");
                                    sbAreas.Append(a.DisplayName);
                                }
                                writer.WriteAttributeString("Areas", sbAreas.ToString());
                            }
                            if (p.BeforeFull != FullType.None)
                            {
                                writer.WriteAttributeString("BeforeFull", p.BeforeFull.ToString());
                            }
                            if (p.AfterFull != FullType.None)
                            {
                                writer.WriteAttributeString("AfterFull", p.AfterFull.ToString());
                            }
                            if (p.SpellsToCast != WorkflowSpells.None)
                            {
                                writer.WriteAttributeString("SpellsToCast", StringProcessing.TrimFlagsEnumToString(p.SpellsToCast));
                            }
                            if (p.SpellsToPotion != WorkflowSpells.None)
                            {
                                writer.WriteAttributeString("SpellsToPotion", StringProcessing.TrimFlagsEnumToString(p.SpellsToPotion));
                            }
                            if (p.SkillsToRun != PromptedSkills.None)
                            {
                                writer.WriteAttributeString("SkillsToRun", StringProcessing.TrimFlagsEnumToString(p.SkillsToRun));
                            }
                            if (p.SupportedKeys != SupportedKeysFlags.None)
                            {
                                writer.WriteAttributeString("SupportedKeys", StringProcessing.TrimFlagsEnumToString(p.SupportedKeys));
                            }
                            writer.WriteAttributeString("TargetRoom", p.TargetRoomIdentifier);
                            if (!string.IsNullOrEmpty(p.ThresholdRoomIdentifier))
                            {
                                writer.WriteAttributeString("ThresholdRoom", p.ThresholdRoomIdentifier);
                            }

                            string sMobValue;
                            if (p.MobType.HasValue)
                                sMobValue = p.MobType.Value.ToString();
                            else
                                sMobValue = p.MobText;
                            if (!string.IsNullOrEmpty(sMobValue)) writer.WriteAttributeString("Mob", sMobValue);

                            int iMobIndexThreshold;
                            int iMobIndex = p.MobIndex;
                            if (string.IsNullOrEmpty(sMobValue))
                                iMobIndexThreshold = 1;
                            else
                                iMobIndexThreshold = 2;
                            if (iMobIndex >= iMobIndexThreshold) writer.WriteAttributeString("MobIndex", iMobIndex.ToString());

                            if (p.UseMagicCombat.HasValue)
                            {
                                writer.WriteAttributeString("UseMagicCombat", p.UseMagicCombat.Value.ToString());
                            }
                            if (p.UseMeleeCombat.HasValue)
                            {
                                writer.WriteAttributeString("UseMeleeCombat", p.UseMeleeCombat.Value.ToString());
                            }
                            if (p.UsePotionsCombat.HasValue)
                            {
                                writer.WriteAttributeString("UsePotionsCombat", p.UsePotionsCombat.Value.ToString());
                            }
                            if (p.AfterKillMonsterAction.HasValue)
                            {
                                writer.WriteAttributeString("AfterKillMonsterAction", p.AfterKillMonsterAction.Value.ToString());
                            }
                            if (p.AutoSpellLevelMin != AUTO_SPELL_LEVEL_NOT_SET && p.AutoSpellLevelMax != AUTO_SPELL_LEVEL_NOT_SET)
                            {
                                writer.WriteAttributeString("AutoSpellLevelMin", p.AutoSpellLevelMin.ToString());
                                writer.WriteAttributeString("AutoSpellLevelMax", p.AutoSpellLevelMax.ToString());
                            }
                            if (p.Realms.HasValue)
                            {
                                writer.WriteAttributeString("Realms", StringProcessing.TrimFlagsEnumToString(p.Realms.Value));
                            }
                            writer.WriteAttributeString("ItemsToProcessType", p.ItemsToProcessType.ToString());
                            if (p.LastCompleted != DateTime.MinValue)
                            {
                                writer.WriteAttributeString("LastCompleted", p.LastCompleted.Ticks.ToString());
                            }
                            writer.WriteEndElement(); //end the perm run
                        }
                    }

                    writer.WriteEndElement(); //end the strategy
                }
                writer.WriteEndElement(); //end the strategy list
            }
        }
        
        private IEnumerable<KeyValuePair<string, string>> GetSettingsToSave()
        {
            if (Weapon.HasValue) yield return new KeyValuePair<string, string>("Weapon", Weapon.Value.ToString());
            if (HeldItem.HasValue) yield return new KeyValuePair<string, string>("HeldItem", HeldItem.Value.ToString());
            yield return new KeyValuePair<string, string>("Realms", StringProcessing.TrimFlagsEnumToString(Realms));
            yield return new KeyValuePair<string, string>("PreferredAlignment", PreferredAlignment.ToString());
            yield return new KeyValuePair<string, string>("ConsoleVerbosity", ConsoleVerbosity.ToString());
            yield return new KeyValuePair<string, string>("FullColor", FullColor.ToArgb().ToString());
            yield return new KeyValuePair<string, string>("EmptyColor", EmptyColor.ToArgb().ToString());
            yield return new KeyValuePair<string, string>("QueryMonsterStatus", QueryMonsterStatus.ToString());
            yield return new KeyValuePair<string, string>("AutoSpellLevelMin", AutoSpellLevelMin.ToString());
            yield return new KeyValuePair<string, string>("AutoSpellLevelMax", AutoSpellLevelMax.ToString());
            yield return new KeyValuePair<string, string>("RemoveAllOnStartup", RemoveAllOnStartup.ToString());
            yield return new KeyValuePair<string, string>("SaveSettingsOnQuit", SaveSettingsOnQuit.ToString());
            yield return new KeyValuePair<string, string>("AutoEscapeThreshold", AutoEscapeThreshold.ToString());
            yield return new KeyValuePair<string, string>("AutoEscapeType", AutoEscapeType.ToString());
            yield return new KeyValuePair<string, string>("AutoEscapeActive", AutoEscapeActive.ToString());
            yield return new KeyValuePair<string, string>("MagicVigorOnlyWhenDownXHP", MagicVigorOnlyWhenDownXHP.ToString());
            yield return new KeyValuePair<string, string>("MagicMendOnlyWhenDownXHP", MagicMendOnlyWhenDownXHP.ToString());
            yield return new KeyValuePair<string, string>("PotionsVigorOnlyWhenDownXHP", PotionsVigorOnlyWhenDownXHP.ToString());
            yield return new KeyValuePair<string, string>("PotionsMendOnlyWhenDownXHP", PotionsMendOnlyWhenDownXHP.ToString());
        }

        private void WriteSettingsXML(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            foreach (var next in GetSettingsToSave())
            {
                writer.WriteStartElement("Setting");
                writer.WriteAttributeString("name", next.Key);
                writer.WriteAttributeString("value", next.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteDynamicItemDataXML(XmlWriter writer)
        {
            List<KeyValuePair<ItemTypeEnum, DynamicItemData>> didItemList = new List<KeyValuePair<ItemTypeEnum, DynamicItemData>>();
            List<KeyValuePair<DynamicDataItemClass, DynamicItemData>> didItemClassList = new List<KeyValuePair<DynamicDataItemClass, DynamicItemData>>();
            foreach (DynamicDataItemClass nextItemClass in Enum.GetValues(typeof(DynamicDataItemClass)))
            {
                if (DynamicItemClassData.TryGetValue(nextItemClass, out DynamicItemData did))
                {
                    didItemClassList.Add(new KeyValuePair<DynamicDataItemClass, DynamicItemData>(nextItemClass, did));
                }
            }
            foreach (ItemTypeEnum nextItemType in Enum.GetValues(typeof(ItemTypeEnum)))
            {
                if (DynamicItemData.TryGetValue(nextItemType, out DynamicItemData did))
                {
                    didItemList.Add(new KeyValuePair<ItemTypeEnum, DynamicItemData>(nextItemType, did));
                }
            }
            writer.WriteStartElement("DynamicItemData");
            foreach (KeyValuePair<DynamicDataItemClass, DynamicItemData> did in didItemClassList)
            {
                WriteDynamicItemDataAttributes(did.Key.ToString(), writer, did.Value);
            }
            foreach (KeyValuePair<ItemTypeEnum, DynamicItemData> did in didItemList)
            {
                WriteDynamicItemDataAttributes(did.Key.ToString(), writer, did.Value);
            }
            writer.WriteEndElement();
        }

        private void WriteLocationsXML(XmlWriter writer)
        {
            if (Locations.Count > 0)
            {
                writer.WriteStartElement("Locations");
                foreach (var next in Locations)
                {
                    WriteLocation(next, writer);
                }
                writer.WriteEndElement();
            }
        }

        private void WriteDynamicItemDataAttributes(string key, XmlWriter writer, DynamicItemData didValue)
        {
            writer.WriteStartElement("Info");
            writer.WriteAttributeString("key", key.ToString());

            string sValue;
            int iCount;

            sValue = null;
            iCount = didValue.KeepCount;
            if (iCount == int.MaxValue)
                sValue = "All";
            else if (iCount >= 0)
                sValue = iCount.ToString();
            if (sValue != null) writer.WriteAttributeString("keep", sValue);

            sValue = null;
            iCount = didValue.SinkCount;
            if (iCount == int.MaxValue)
                sValue = "All";
            else if (iCount >= 0)
                sValue = iCount.ToString();
            if (sValue != null) writer.WriteAttributeString("sink", sValue);

            if (didValue.OverflowAction != ItemInventoryOverflowAction.None)
            {
                writer.WriteAttributeString("overflow", didValue.OverflowAction.ToString());
            }

            writer.WriteEndElement();
        }

        private void WriteLocation(LocationNode node, XmlWriter writer)
        {
            writer.WriteStartElement("Location");
            if (!string.IsNullOrEmpty(node.DisplayName))
            {
                writer.WriteAttributeString("displayname", node.DisplayName);
            }
            if (!string.IsNullOrEmpty(node.Room))
            {
                writer.WriteAttributeString("room", node.Room);
            }
            if (node.Children != null && node.Children.Count > 0)
            {
                if (node.Expanded)
                {
                    writer.WriteAttributeString("expanded", node.Expanded.ToString());
                }
                foreach (LocationNode childNode in node.Children)
                {
                    WriteLocation(childNode, writer);
                }
            }
            writer.WriteEndElement();
        }

        public static void CreateNewDatabaseSchema(SQLiteConnection conn)
        {
            List<string> tableCreations = new List<string>()
            {
                "CREATE TABLE Users (ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, UserName TEXT UNIQUE NOT NULL)",
                "CREATE TABLE Settings (UserID INTEGER NOT NULL, SettingName TEXT NOT NULL, SettingValue TEXT NOT NULL, PRIMARY KEY (UserID, SettingName), FOREIGN KEY(UserID) REFERENCES Users(UserID))",
                "CREATE TABLE DynamicItemData (UserID INTEGER NOT NULL, Key TEXT NOT NULL, KeepCount INTEGER NULL, SinkCount INTEGER NULL, OverflowAction INTEGER NULL, PRIMARY KEY (UserID, Key), FOREIGN KEY(UserID) REFERENCES Users(UserID))",
                "CREATE TABLE Areas (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, OrderValue INTEGER NOT NULL, ParentID INTEGER NULL, DisplayName TEXT NOT NULL, TickRoom INTEGER NULL, PawnShop INTEGER NULL, InventorySinkRoom TEXT NULL, FOREIGN KEY(UserID) REFERENCES Users(UserID), FOREIGN KEY(ParentID) REFERENCES Areas(ID))",
                "CREATE TABLE LocationNodes (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, OrderValue INTEGER NOT NULL, ParentID INTEGER NULL, DisplayName TEXT NULL, Room TEXT NULL, Expanded INTEGER NOT NULL, FOREIGN KEY(UserID) REFERENCES Users(UserID), FOREIGN KEY(ParentID) REFERENCES LocationNodes(ID))",
                "CREATE TABLE Strategies (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, OrderValue INTEGER NOT NULL, DisplayName TEXT NULL, AfterKillMonsterAction INTEGER NOT NULL, ManaPool INTEGER NULL, FinalMagicAction INTEGER NOT NULL, FinalMeleeAction INTEGER NOT NULL, FinalPotionsAction INTEGER NOT NULL, MagicOnlyWhenStunnedForXMS INTEGER NULL, MeleeOnlyWhenStunnedForXMS INTEGER NULL, PotionsOnlyWhenStunnedForXMS INTEGER NULL, TypesToRunLastCommandIndefinitely INTEGER NOT NULL, TypesWithStepsEnabled INTEGER NOT NULL, AutoSpellLevelMin INTEGER NULL, AutoSpellLevelMax INTEGER NULL, Realms INTEGER NULL, FOREIGN KEY(UserID) REFERENCES Users(UserID))",
                "CREATE TABLE StrategySteps (StrategyID INTEGER NOT NULL, CombatType INTEGER NOT NULL, IndexValue INTEGER NOT NULL, StepType INTEGER NOT NULL, PRIMARY KEY (StrategyID, CombatType, IndexValue), FOREIGN KEY(StrategyID) REFERENCES Strategies(ID) ON DELETE CASCADE)",
                "CREATE TABLE PermRuns (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, OrderValue INTEGER NOT NULL, DisplayName TEXT NULL, Rehome INTEGER NOT NULL, BeforeFull INTEGER NULL, AfterFull INTEGER NULL, SpellsToCast INTEGER NOT NULL, SpellsToPotion INTEGER NOT NULL, SkillsToRun INTEGER NOT NULL, SupportedKeys INTEGER NOT NULL, TargetRoom TEXT NOT NULL, ThresholdRoom TEXT NULL, MobText TEXT NULL, MobIndex INTEGER NULL, StrategyID INTEGER NOT NULL, UseMagicCombat INTEGER NULL, UseMeleeCombat INTEGER NULL, UsePotionsCombat INTEGER NULL, AfterKillMonsterAction INTEGER NULL, AutoSpellLevelMin INTEGER NULL, AutoSpellLevelMax INTEGER NULL, Realms INTEGER NULL, ItemsToProcessType INTEGER NOT NULL, LastCompleted INTEGER NULL, FOREIGN KEY(UserID) REFERENCES Users(UserID), FOREIGN KEY(StrategyID) REFERENCES Strategies(ID) ON DELETE CASCADE)",
                "CREATE TABLE PermRunToAreas (PermRunID INTEGER NOT NULL, AreaID INTEGER NOT NULL, PRIMARY KEY (PermRunID, AreaID), FOREIGN KEY(AreaID) REFERENCES Areas(ID) ON DELETE CASCADE, FOREIGN KEY(PermRunID) REFERENCES PermRuns(ID) ON DELETE CASCADE)",
            };
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                foreach (string next in tableCreations)
                {
                    cmd.CommandText = next;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to execute " + next, ex);
                    }
                }
            }
        }

        public static SQLiteConnection GetSqliteConnection(string databasePath)
        {
            SQLiteConnectionStringBuilder connsb = new SQLiteConnectionStringBuilder()
            {
                DataSource = databasePath,
                Version = 3
            };
            return new SQLiteConnection(connsb.ToString());
        }

        public static int GetUserID(SQLiteConnection conn, string userName, bool autoCreate)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT ID FROM Users WHERE UserName = @UserName";
                cmd.Parameters.AddWithValue("@UserName", userName);
                object oResult = cmd.ExecuteScalar();
                int iUserID = 0;
                if (oResult == null || oResult == DBNull.Value)
                {
                    if (autoCreate)
                    {
                        cmd.CommandText = "INSERT INTO Users (UserName) VALUES (@UserName)";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        iUserID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                else
                {
                    iUserID = Convert.ToInt32(oResult);
                }
                return iUserID;
            }
        }

        public void ScrubIDs()
        {
            foreach (LocationNode nextLocation in EnumerateLocations())
            {
                nextLocation.ID = 0;
                nextLocation.ParentID = 0;
            }
            foreach (Strategy s in Strategies)
            {
                s.ID = 0;
            }
        }

        public WorkflowSpells AlwaysOnSpells
        {
            get
            {
                return WorkflowSpells.Bless | WorkflowSpells.Protection;
            }
        }

        public static RealmTypeFlags GetNextRealmFromStartingPoint(RealmTypeFlags startingPoint, RealmTypeFlags availableRealms)
        {
            foreach (RealmTypeFlags nextRealm in GetAvailableRealmsFromStartingPoint(startingPoint, availableRealms))
            {
                if (nextRealm != startingPoint)
                {
                    return nextRealm;
                }
            }
            return startingPoint;
        }

        public static IEnumerable<RealmTypeFlags> GetAvailableRealmsFromStartingPoint(RealmTypeFlags startingPoint, RealmTypeFlags availableRealms)
        {
            foreach (RealmTypeFlags nextRealm in EnumerateRealmsFromStartingPoint(startingPoint))
            {
                if ((nextRealm & availableRealms) != RealmTypeFlags.None)
                {
                    yield return nextRealm;
                }
            }
        }

        public static IEnumerable<RealmTypeFlags> EnumerateRealmsFromStartingPoint(RealmTypeFlags startingPoint)
        {
            if (startingPoint == RealmTypeFlags.None || startingPoint == RealmTypeFlags.Earth)
            {
                yield return RealmTypeFlags.Earth;
                yield return RealmTypeFlags.Wind;
                yield return RealmTypeFlags.Fire;
                yield return RealmTypeFlags.Water;
            }
            else if (startingPoint == RealmTypeFlags.Wind)
            {
                yield return RealmTypeFlags.Wind;
                yield return RealmTypeFlags.Fire;
                yield return RealmTypeFlags.Water;
                yield return RealmTypeFlags.Earth;
            }
            else if (startingPoint == RealmTypeFlags.Fire)
            {
                yield return RealmTypeFlags.Fire;
                yield return RealmTypeFlags.Water;
                yield return RealmTypeFlags.Earth;
                yield return RealmTypeFlags.Wind;
            }
            else if (startingPoint == RealmTypeFlags.Water)
            {
                yield return RealmTypeFlags.Water;
                yield return RealmTypeFlags.Earth;
                yield return RealmTypeFlags.Wind;
                yield return RealmTypeFlags.Fire;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}

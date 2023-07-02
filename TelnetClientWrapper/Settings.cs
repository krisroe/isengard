using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Data.SQLite;
using System.Data;
namespace IsengardClient
{
    internal class IsengardSettingData
    {
        public ItemTypeEnum? Weapon { get; set; }
        public RealmType Realm { get; set; }
        public AlignmentType PreferredAlignment { get; set; }
        public bool VerboseMode { get; set; }
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
        public Dictionary<ItemTypeEnum, DynamicItemData> DynamicItemData { get; set; }
        public Dictionary<DynamicDataItemClass, DynamicItemData> DynamicItemClassData { get; set; }
        public List<LocationNode> Locations { get; set; }
        public IsengardSettingData()
        {
            Weapon = null;
            Realm = RealmType.Earth;
            PreferredAlignment = AlignmentType.Blue;
            VerboseMode = false;
            FullColor = Color.Green;
            EmptyColor = Color.Red;
            QueryMonsterStatus = true;
            AutoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
            AutoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
            RemoveAllOnStartup = true;
            DisplayStunLength = false;
            AutoEscapeThreshold = 0;
            AutoEscapeType = AutoEscapeType.Flee;
            AutoEscapeActive = false;
            DynamicItemData = new Dictionary<ItemTypeEnum, DynamicItemData>();
            DynamicItemClassData = new Dictionary<DynamicDataItemClass, DynamicItemData>();
            Locations = new List<LocationNode>();
        }
        public IsengardSettingData(IsengardSettingData copied)
        {
            Weapon = copied.Weapon;
            Realm = copied.Realm;
            PreferredAlignment = copied.PreferredAlignment;
            VerboseMode = copied.VerboseMode;
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
            Locations = new List<LocationNode>();
            foreach (LocationNode next in copied.Locations)
            {
                Locations.Add(new LocationNode(next, null));
            }
        }
        public IsengardSettingData(SQLiteConnection conn, int UserID, List<string> errorMessages) : this()
        {
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
                ValidateSettings();

                cmd.CommandText = "SELECT Key,KeepCount,TickCount,OverflowAction FROM DynamicItemData WHERE UserID = @UserID";
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
                        object oData = reader["KeepCount"];
                        did.KeepCount = oData == DBNull.Value ? -1 : Convert.ToInt32(oData);
                        oData = reader["TickCount"];
                        did.TickCount = oData == DBNull.Value ? -1 : Convert.ToInt32(oData);
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

                List<LocationNode> locationFlatList = new List<LocationNode>();
                Dictionary<int, LocationNode> locationMapping = new Dictionary<int, LocationNode>();
                cmd.CommandText = "SELECT ID,DisplayName,Room,Expanded,ParentID FROM LocationNodes WHERE UserID = @UserID ORDER BY OrderValue";
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int iID = Convert.ToInt32(reader["ID"]);
                        object oData = reader["DisplayName"];
                        string sDisplayName = oData == DBNull.Value ? string.Empty : oData.ToString();
                        oData = reader["Room"];
                        string sRoom = oData == DBNull.Value ? string.Empty : oData.ToString();
                        bool expanded = Convert.ToInt32(reader["Expanded"]) != 0;
                        oData = reader["ParentID"];
                        int iParentID = oData == DBNull.Value ? 0 : Convert.ToInt32(oData);
                        LocationNode ln = new LocationNode();
                        ln.ID = iID;
                        ln.DisplayName = sDisplayName;
                        ln.Room = sRoom;
                        ln.Expanded = expanded;
                        ln.ParentID = iParentID;
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
                    }
                }
            }
        }

        public IsengardSettingData(string Input, List<string> errorMessages, bool IsFile) : this()
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
            bool foundSettings = false;
            bool foundDynamicItemData = false;
            bool foundLocations = false;
            foreach (XmlNode nextNode in docElement.ChildNodes)
            {
                if (nextNode is XmlElement)
                {
                    XmlElement elem = (XmlElement)nextNode;
                    string sElemName = elem.Name;
                    switch (sElemName)
                    {
                        case "Settings":
                            if (foundSettings) errorMessages.Add("Duplicate settings element found.");
                            HandleSettings(elem, errorMessages);
                            foundSettings = true;
                            break;
                        case "DynamicItemData":
                            if (foundDynamicItemData) errorMessages.Add("Duplicate dynamic item data element found.");
                            HandleDynamicItemData(elem, errorMessages);
                            foundDynamicItemData = true;
                            break;
                        case "Locations":
                            if (foundLocations) errorMessages.Add("Duplicate locations data element found.");
                            HandleLocations(elem, errorMessages, Locations);
                            foundLocations = true;
                            break;
                        default:
                            errorMessages.Add("Unexpected element found: " + sElemName);
                            break;
                    }
                }
            }
            if (!foundSettings)
            {
                errorMessages.Add("No settings element found.");
            }
            ValidateSettings();
        }

        private void HandleLocations(XmlElement elem, List<string> errorMessages, List<LocationNode> locations)
        {
            foreach (XmlNode nextLocationNode in elem.ChildNodes)
            {
                XmlElement nextLocationElem = nextLocationNode as XmlElement;
                if (nextLocationElem != null)
                {
                    if (nextLocationElem.Name == "Location")
                    {
                        string sDisplayName = nextLocationElem.GetAttribute("displayname");
                        string sRoom = nextLocationElem.GetAttribute("room");
                        string sExpanded = nextLocationElem.GetAttribute("expanded");
                        bool bExpanded = false;
                        if (!string.IsNullOrEmpty(sExpanded))
                        {
                            if (!bool.TryParse(sExpanded, out bExpanded))
                            {
                                errorMessages.Add("Invalid location expanded: " + sExpanded);
                            }
                        }
                        if (string.IsNullOrEmpty(sDisplayName) && string.IsNullOrEmpty(sRoom))
                        {
                            errorMessages.Add("No room or display name specified for location");
                        }
                        else
                        {
                            LocationNode node = new LocationNode();
                            node.DisplayName = sDisplayName;
                            node.Room = sRoom;
                            locations.Add(node);
                            List<LocationNode> subNodes = new List<LocationNode>();
                            HandleLocations(nextLocationElem, errorMessages, subNodes);
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
            newSettings["Weapon"] = Weapon.HasValue ? Weapon.Value.ToString() : string.Empty;
            newSettings["Realm"] = Realm.ToString();
            newSettings["PreferredAlignment"] = PreferredAlignment.ToString();
            newSettings["VerboseMode"] = VerboseMode.ToString();
            newSettings["QueryMonsterStatus"] = QueryMonsterStatus.ToString();
            newSettings["RemoveAllOnStartup"] = RemoveAllOnStartup.ToString();
            newSettings["DisplayStunLength"] = DisplayStunLength.ToString();
            newSettings["FullColor"] = FullColor.ToArgb().ToString();
            newSettings["EmptyColor"] = EmptyColor.ToArgb().ToString();
            newSettings["AutoSpellLevelMin"] = AutoSpellLevelMin.ToString();
            newSettings["AutoSpellLevelMax"] = AutoSpellLevelMax.ToString();
            newSettings["AutoEscapeThreshold"] = AutoEscapeThreshold.ToString();
            newSettings["AutoEscapeType"] = AutoEscapeType.ToString();
            newSettings["AutoEscapeActive"] = AutoEscapeActive.ToString();
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

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.CommandText = "SELECT ID FROM LocationNodes WHERE UserID = @UserID";
                HashSet<int> existingIDs = new HashSet<int>();
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        existingIDs.Add(Convert.ToInt32(reader["ID"]));
                    }
                }
                SQLiteParameter orderParam = cmd.Parameters.Add("@OrderValue", DbType.Int32);
                SQLiteParameter displayNameParam = cmd.Parameters.Add("@DisplayName", DbType.String);
                SQLiteParameter roomParameter = cmd.Parameters.Add("@Room", DbType.String);
                SQLiteParameter expandedParameter = cmd.Parameters.Add("@Expanded", DbType.Int32);
                SQLiteParameter parentIDParameter = cmd.Parameters.Add("@ParentID", DbType.Int32);
                SQLiteParameter idParameter = cmd.Parameters.Add("@ID", DbType.Int32);

                int iOrder = 0;
                foreach (LocationNode nextLoc in EnumerateLocations())
                {
                    iOrder++;
                    orderParam.Value = iOrder;
                    displayNameParam.Value = string.IsNullOrEmpty(nextLoc.DisplayName) ? (object)DBNull.Value : nextLoc.DisplayName;
                    roomParameter.Value = string.IsNullOrEmpty(nextLoc.Room) ? (object)DBNull.Value : nextLoc.Room;
                    expandedParameter.Value = nextLoc.Expanded ? 1 : 0;
                    parentIDParameter.Value = nextLoc.Parent == null ? (object)DBNull.Value : nextLoc.Parent.ID;
                    int iID = nextLoc.ID;
                    bool isNew = iID == 0;
                    if (isNew)
                    {
                        cmd.CommandText = "INSERT INTO LocationNodes (UserID, OrderValue, DisplayName, Room, Expanded, ParentID) VALUES (@UserID, @OrderValue, @DisplayName, @Room, @Expanded, @ParentID)";
                    }
                    else
                    {
                        idParameter.Value = iID;
                        cmd.CommandText = "UPDATE LocationNodes SET OrderValue = @OrderValue, DisplayName = @DisplayName, Room = @Room, Expanded = @Expanded WHERE ID = @ID";
                        existingIDs.Remove(iID);
                    }
                    cmd.ExecuteNonQuery();
                    if (isNew)
                    {
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        int iSavedID = Convert.ToInt32(cmd.ExecuteScalar());
                        nextLoc.ID = iSavedID;
                        if (nextLoc.Children != null)
                        {
                            foreach (LocationNode nextChild in nextLoc.Children)
                            {
                                nextChild.ParentID = iSavedID;
                            }
                        }
                    }
                }
                cmd.Parameters.Clear();
                idParameter = cmd.Parameters.Add("@ID", DbType.Int32);
                cmd.CommandText = "DELETE FROM LocationNodes WHERE ID = @ID";
                foreach (int iID in existingIDs)
                {
                    idParameter.Value = iID;
                    cmd.ExecuteNonQuery();
                }
            }
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
            string sTickCount = did.TickCount >= 0 ? did.TickCount.ToString() : "NULL";
            string sOverflowAction = did.OverflowAction == ItemInventoryOverflowAction.None ? "NULL" : Convert.ToInt32(did.OverflowAction).ToString();
            if (existingKeys.Contains(key))
                sql = $"UPDATE DynamicItemData SET KeepCount = {sKeepCount}, TickCount = {sTickCount}, OverflowAction = {sOverflowAction} WHERE UserID = @UserID AND Key = @Key";
            else
                sql = $"INSERT INTO DynamicItemData (UserID, Key, KeepCount, TickCount, OverflowAction) VALUES (@UserID, @Key, {sKeepCount}, {sTickCount}, {sOverflowAction})";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            existingKeys.Remove(key);
        }

        private void ValidateSettings()
        {
            if (AutoEscapeType != AutoEscapeType.Flee && AutoEscapeType != AutoEscapeType.Hazy)
            {
                AutoEscapeType = AutoEscapeType.Flee;
            }
            if (AutoEscapeThreshold < 0)
            {
                AutoEscapeThreshold = 0;
            }
            if (AutoEscapeThreshold == 0)
            {
                AutoEscapeActive = false;
            }
            if (AutoSpellLevelMin > AutoSpellLevelMax || AutoSpellLevelMax < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || AutoSpellLevelMax > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM || AutoSpellLevelMin < frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM || AutoSpellLevelMin > frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM)
            {
                AutoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
                AutoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
            }
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
                        did.TickCount = ProcessNonNegativeIntAttributeWithAll(elem, "tick", -1, errorMessages);

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
            switch (sName)
            {
                case "Weapon":
                    if (!string.IsNullOrEmpty(sValue))
                    {
                        if (Enum.TryParse(sValue, out ItemTypeEnum weapon))
                            Weapon = weapon;
                        else
                            errorMessages.Add("Invalid weapon: " + sValue);
                    }
                    break;
                case "Realm":
                    if (Enum.TryParse(sValue, out RealmType realm))
                        Realm = realm;
                    else
                        errorMessages.Add("Invalid realm: " + sValue);
                    break;
                case "PreferredAlignment":
                    if (Enum.TryParse(sValue, out AlignmentType alignment))
                        PreferredAlignment = alignment;
                    else
                        errorMessages.Add("Invalid preferred alignment: " + sValue);
                    break;
                case "VerboseMode":
                    if (bool.TryParse(sValue, out bValue))
                        VerboseMode = bValue;
                    else
                        errorMessages.Add("Invalid VerboseMode: " + sValue);
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
                default:
                    errorMessages.Add("Invalid setting name: " + sName);
                    break;
            }
        }

        public void SaveToXmlWriter(XmlWriter writer)
        {
            writer.WriteStartElement("DynamicData");
            writer.WriteStartElement("Settings");
            WriteSetting(writer, "Weapon", Weapon.HasValue ? Weapon.Value.ToString() : string.Empty);
            WriteSetting(writer, "Realm", Realm.ToString());
            WriteSetting(writer, "PreferredAlignment", PreferredAlignment.ToString());
            WriteSetting(writer, "VerboseMode", VerboseMode.ToString());
            WriteSetting(writer, "FullColor", FullColor.ToArgb().ToString());
            WriteSetting(writer, "EmptyColor", EmptyColor.ToArgb().ToString());
            WriteSetting(writer, "QueryMonsterStatus", QueryMonsterStatus.ToString());
            WriteSetting(writer, "AutoSpellLevelMin", AutoSpellLevelMin.ToString());
            WriteSetting(writer, "AutoSpellLevelMax", AutoSpellLevelMax.ToString());
            WriteSetting(writer, "RemoveAllOnStartup", RemoveAllOnStartup.ToString());
            WriteSetting(writer, "AutoEscapeThreshold", AutoEscapeThreshold.ToString());
            WriteSetting(writer, "AutoEscapeType", AutoEscapeType.ToString());
            WriteSetting(writer, "AutoEscapeActive", AutoEscapeActive.ToString());
            writer.WriteEndElement();

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

            if (Locations.Count > 0)
            {
                writer.WriteStartElement("Locations");
                foreach (var next in Locations)
                {
                    WriteLocation(next, writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
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
            iCount = didValue.TickCount;
            if (iCount == int.MaxValue)
                sValue = "All";
            else if (iCount >= 0)
                sValue = iCount.ToString();
            if (sValue != null) writer.WriteAttributeString("tick", sValue);

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

        public void WriteSetting(XmlWriter writer, string Name, string Value)
        {
            writer.WriteStartElement("Setting");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value);
            writer.WriteEndElement();
        }

        public static void CreateNewDatabaseSchema(SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "CREATE TABLE Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, UserName TEXT UNIQUE NOT NULL)";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE Settings (UserID INTEGER NOT NULL, SettingName TEXT NOT NULL, SettingValue TEXT NOT NULL, PRIMARY KEY (UserID, SettingName), FOREIGN KEY(UserID) REFERENCES Users(UserID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE DynamicItemData (UserID INTEGER NOT NULL, Key TEXT NOT NULL, KeepCount INTEGER NULL, TickCount INTEGER NULL, OverflowAction INTEGER NULL, PRIMARY KEY (UserID, Key), FOREIGN KEY(UserID) REFERENCES Users(UserID))";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "CREATE TABLE LocationNodes (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, OrderValue INTEGER NOT NULL, DisplayName TEXT NULL, Room TEXT NULL, Expanded INTEGER NOT NULL, ParentID INTEGER NULL, FOREIGN KEY(UserID) REFERENCES Users(UserID), FOREIGN KEY(ParentID) REFERENCES LocationNodes(ID))";
                cmd.ExecuteNonQuery();
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

        public static int GetUserID(SQLiteConnection conn, string userName)
        {
            using (SQLiteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT UserID FROM Users WHERE UserName = @UserName";
                cmd.Parameters.AddWithValue("@UserName", userName);
                object oResult = cmd.ExecuteScalar();
                int iUserID;
                if (oResult == null || oResult == DBNull.Value)
                {
                    cmd.CommandText = "INSERT INTO Users (UserName) VALUES (@UserName)";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT last_insert_rowid()";
                    iUserID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    iUserID = Convert.ToInt32(oResult);
                }
                return iUserID;
            }
        }
    }
}

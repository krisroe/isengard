﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Data.SQLite;

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
        private IsengardSettingData()
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
            AutoEscapeThreshold = 0;
            AutoEscapeType = AutoEscapeType.Flee;
            AutoEscapeActive = false;
        }
        public IsengardSettingData(SQLiteCommand cmd, int UserID, List<string> errorMessages) : this()
        {
            cmd.Parameters.Clear();
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
        }

        public IsengardSettingData(string FileName, List<string> errorMessages) : this()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);

            XmlElement docElement = doc.DocumentElement;

            bool foundSettings = false;
            foreach (XmlNode nextNode in docElement.ChildNodes)
            {
                if (nextNode is XmlElement)
                {
                    XmlElement elem = (XmlElement)nextNode;
                    switch (elem.Name)
                    {
                        case "Settings":
                            if (foundSettings)
                            {
                                errorMessages.Add("Duplicate settings element found.");
                            }
                            HandleSettings(elem, errorMessages);
                            foundSettings = true;
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

        private void HandleSettings(XmlElement settings, List<string> errorMessages)
        {
            foreach (XmlNode nextNode in settings.ChildNodes)
            {
                if (nextNode is XmlElement)
                {
                    XmlElement elem = (XmlElement)nextNode;
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

            writer.WriteEndElement();
        }
        public void WriteSetting(XmlWriter writer, string Name, string Value)
        {
            writer.WriteStartElement("Setting");
            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("value", Value);
            writer.WriteEndElement();
        }
    }
}

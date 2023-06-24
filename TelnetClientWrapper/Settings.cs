using System.Drawing;
namespace IsengardClient
{
    internal class IsengardSettingData
    {
        public IsengardSettingData()
        {
            FullColor = Color.Green;
            EmptyColor = Color.Red;
            QueryMonsterStatus = true;
            AutoSpellLevelMin = frmConfiguration.AUTO_SPELL_LEVEL_MINIMUM;
            AutoSpellLevelMax = frmConfiguration.AUTO_SPELL_LEVEL_MAXIMUM;
            RemoveAllOnStartup = true;
        }
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
    }
}

using QuickGraph;
namespace IsengardClient
{
    internal class Exit : Edge<Room>
    {
        public override string ToString()
        {
            return Source.ToString() + "--" + ExitText + " -->" + Target.ToString();
        }
        /// <summary>
        /// text for the exit
        /// </summary>
        public string ExitText { get; set; }
        /// <summary>
        /// command to run before using the exit
        /// </summary>
        public string PreCommand { get; set; }
        /// <summary>
        /// whether the exit is hidden
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// whether flee is permitted
        /// </summary>
        public bool NoFlee { get; set; }
        /// <summary>
        /// set to indicate exits that are not always present
        /// </summary>
        public ExitPresenceType PresenceType { get; set; }
        /// <summary>
        /// wait for a specific message before leaving
        /// </summary>
        public string WaitForMessage { get; set; }
        /// <summary>
        /// what type of key is needed to use the exit
        /// </summary>
        public KeyType KeyType { get; set; }

        /// <summary>
        /// whether the key is required to use the exit (returns false when knockable)
        /// </summary>
        /// <returns>true if the key is required, false otherwise</returns>
        public bool RequiresKey()
        {
            return this.KeyType == KeyType.GateKey;
        }

        public Exit(Room source, Room target, string exitText) : base(source, target)
        {
            this.ExitText = exitText;
        }
    }

    internal enum ExitPresenceType
    {
        Always,
        Periodic,
        RequiresSearch,
    }

    internal enum KeyType
    {
        None,
        GateKey,
        KasnarsRedKey,
        SilverKey,
    }
}

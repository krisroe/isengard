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
}

using Priority_Queue;
using QuickGraph;
namespace IsengardClient.Backend
{
    public class Exit : Edge<Room>, GraphSharp.Controls.IDeletableEdge
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
        /// whether the exit must be opened before it can be used
        /// </summary>
        public bool MustOpen { get; set; }
        /// <summary>
        /// whether the exit is hidden
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// set to indicate exits that are not always present
        /// </summary>
        public ExitPresenceType PresenceType { get; set; }
        /// <summary>
        /// wait for a specific message before leaving
        /// </summary>
        public InformationalMessageType? WaitForMessage { get; set; }
        /// <summary>
        /// what type of key is needed to use the exit
        /// </summary>
        public SupportedKeysFlags KeyType { get; set; }
        /// <summary>
        /// if true, the exit is locked and knockable but the key type is unknown
        /// </summary>
        public bool IsUnknownKnockableKeyType { get; set; }
        /// <summary>
        /// whether the exit requires floating (fly/levitation)
        /// </summary>
        public FloatRequirement FloatRequirement { get; set; }
        /// <summary>
        /// whether the exit requires day
        /// </summary>
        public bool RequiresDay { get; set; }
        /// <summary>
        /// maximum level
        /// </summary>
        public int? MaximumLevel { get; set; }
        /// <summary>
        /// minimum level
        /// </summary>
        public int? MinimumLevel { get; set; }
        /// <summary>
        /// whether the exit is currently deleted for graphing purposes
        /// </summary>
        public bool ShowAsRedOnGraph { get; set; }
        /// <summary>
        /// whether the exit is a trap exit
        /// </summary>
        public bool IsTrapExit { get; set; }
        /// <summary>
        /// required class
        /// </summary>
        public ClassType? RequiredClass { get; set; }
        /// <summary>
        /// requires no items
        /// </summary>
        public bool RequiresNoItems { get; set; }

        /// <summary>
        /// whether go should be forced
        /// </summary>
        public bool ForceGo { get; set; }

        /// <summary>
        /// whether the key is required to use the exit (returns false when knockable)
        /// </summary>
        /// <returns>true if the key is required, false otherwise</returns>
        public bool RequiresKey()
        {
            return KeyType == SupportedKeysFlags.GateKey || KeyType == SupportedKeysFlags.TombKey;
        }

        public int GetCost(GraphInputs graphInputs)
        {
            int ret;
            int level = graphInputs.Level;
            bool levitating = graphInputs.Levitating;
            bool isKeyExit = KeyType != SupportedKeysFlags.None;
            bool hasNeededKey = isKeyExit ? (graphInputs.Keys & KeyType) == KeyType : false;
            bool requiresKey = RequiresKey();
            if (RequiresDay && !graphInputs.IsDay)
                ret = int.MaxValue;
            else if (MaximumLevel.HasValue && level > MaximumLevel.Value)
                ret = int.MaxValue;
            else if (MinimumLevel.HasValue && level < MinimumLevel.Value)
                ret = int.MaxValue;
            else if (FloatRequirement == FloatRequirement.Fly && !graphInputs.Flying)
                ret = int.MaxValue;
            else if (FloatRequirement == FloatRequirement.Levitation && !levitating)
                ret = int.MaxValue;
            else if (FloatRequirement == FloatRequirement.NoLevitation && levitating)
                ret = int.MaxValue;
            else if (isKeyExit && requiresKey && !hasNeededKey)
                ret = int.MaxValue;
            else if (Target.BackendName == Room.UNKNOWN_ROOM)
                ret = int.MaxValue;
            else if (RequiredClass.HasValue && graphInputs.Class != RequiredClass.Value)
                ret = int.MaxValue;
            else if (PresenceType == ExitPresenceType.Periodic) //embark/disembark ship exits
                ret = 10000;
            else if (PresenceType == ExitPresenceType.RequiresSearch)
                ret = 2000;
            else if (IsTrapExit)
                ret = 2000;
            else if (Target.IsTrapRoom)
                ret = 2000;
            else if (isKeyExit && !requiresKey && !hasNeededKey)
                ret = 2000;
            else
                ret = 1;
            return ret;
        }

        public bool IsDeleted()
        {
            return ShowAsRedOnGraph;
        }

        public Exit(Room source, Room target, string exitText) : base(source, target)
        {
            this.ExitText = exitText;
        }
    }

    public class ExitPriorityNode : GenericPriorityQueueNode<int>
    {
        public Exit Exit { get; set; }
        public ExitPriorityNode(Exit e)
        {
            Exit = e;
        }
    }
}

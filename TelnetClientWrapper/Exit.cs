using Priority_Queue;
using QuickGraph;
namespace IsengardClient
{
    internal class Exit : Edge<Room>, GraphSharp.Controls.IDeletableEdge
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
        public InformationalMessageType? WaitForMessage { get; set; }
        /// <summary>
        /// what type of key is needed to use the exit
        /// </summary>
        public KeyType KeyType { get; set; }
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

        public bool ExitIsUsable(GraphInputs graphInputs)
        {
            int level = graphInputs.Level;
            bool levitating = graphInputs.Levitating;
            bool ret;
            if (RequiresDay && !graphInputs.IsDay)
                ret = false;
            else if (MaximumLevel.HasValue && level > MaximumLevel.Value)
                ret = false;
            else if (MinimumLevel.HasValue && level < MinimumLevel.Value)
                ret = false;
            else if (FloatRequirement == FloatRequirement.Fly && !graphInputs.Flying)
                ret = false;
            else if (FloatRequirement == FloatRequirement.Levitation && !levitating)
                ret = false;
            else if (FloatRequirement == FloatRequirement.NoLevitation && levitating)
                ret = false;
            else
                ret = true;
            return ret;
        }

        /// <summary>
        /// whether the key is required to use the exit (returns false when knockable)
        /// </summary>
        /// <returns>true if the key is required, false otherwise</returns>
        public bool RequiresKey()
        {
            return this.KeyType == KeyType.GateKey;
        }

        public int GetCost()
        {
            int ret;
            if (PresenceType == ExitPresenceType.Periodic) //embark/disembark ship exits
            {
                ret = 10000;
            }
            else if (PresenceType == ExitPresenceType.RequiresSearch)
            {
                ret = 1000;
            }
            else if (KeyType != KeyType.None && !RequiresKey())
            {
                ret = 1000;
            }
            else
            {
                ret = 1;
            }
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

    internal class ExitPriorityNode : GenericPriorityQueueNode<int>
    {
        public Exit Exit { get; set; }
        public ExitPriorityNode(Exit e)
        {
            Exit = e;
        }
    }
}

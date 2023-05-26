using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// whether to omit go for the exit
        /// </summary>
        public bool OmitGo { get; set; }
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
        public Exit(Room source, Room target, string exitText) : base(source, target)
        {
            this.ExitText = exitText;
        }
    }
}

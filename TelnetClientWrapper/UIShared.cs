using System.Drawing;
using System.Windows.Forms;
namespace IsengardClient
{
    internal class UIShared
    {
        internal static Color GetColorForRealm(string realm)
        {
            Color ret = Color.Transparent;
            switch (realm)
            {
                case "earth":
                    ret = Color.Tan;
                    break;
                case "fire":
                    ret = Color.LightSalmon;
                    break;
                case "water":
                    ret = Color.LightBlue;
                    break;
                case "wind":
                    ret = Color.LightGray;
                    break;
            }
            return ret;
        }

        internal static void UpdateRealmMenu(ContextMenuStrip cms, string realm)
        {
            foreach (var next in cms.Items)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)next;
                tsmi.Checked = realm == tsmi.Text;
            }
        }

        internal static void RefreshAutoHazyUI(int autoHazyThreshold, Label lblAutoHazy, ToolStripMenuItem tsmiClearAutoHazy, int previousThreshold)
        {
            Color autoHazyBackColor;
            string autoHazyText;
            if (autoHazyThreshold > 0)
            {
                autoHazyBackColor = Color.DarkBlue;
                autoHazyText = "Auto Hazy at " + autoHazyThreshold;
            }
            else
            {
                autoHazyBackColor = Color.Black;
                autoHazyText = "No Auto Hazy";
                if (previousThreshold > 0)
                {
                    autoHazyText += " (" + previousThreshold + ")";
                }
            }
            lblAutoHazy.Text = autoHazyText;
            lblAutoHazy.BackColor = autoHazyBackColor;
            tsmiClearAutoHazy.Visible = true;
        }
    }
}

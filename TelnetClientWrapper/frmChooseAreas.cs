using IsengardClient.Backend;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    internal partial class frmChooseAreas : Form
    {
        public frmChooseAreas(IsengardSettingData settings, HashSet<Area> areas)
        {
            InitializeComponent();

            List<Area> aList = new List<Area>();
            foreach (Area a in settings.EnumerateAreas())
            {
                aList.Add(a);
                chklst.Items.Add(a);
            }
            if (areas != null)
            {
                foreach (Area a in areas)
                {
                    chklst.SetItemChecked(aList.IndexOf(a), true);
                }
            }
        }

        public HashSet<Area> SelectedAreas
        {
            get
            {
                HashSet<Area> ret = null;
                if (chklst.CheckedItems.Count > 0)
                {
                    ret = new HashSet<Area>();
                    foreach (Area a in chklst.CheckedItems)
                    {
                        ret.Add(a);
                    }
                }
                return ret;
            }
        }
    }
}

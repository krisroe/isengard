using IsengardClient.Backend;
using System.Collections.Generic;
using System.Windows.Forms;
namespace IsengardClient
{
    public partial class frmMobDynamicData : Form
    {
        private DynamicMobData _dmd;
        private bool _initialized;

        public frmMobDynamicData(DynamicMobData dmd, List<Strategy> Strategies, IsengardSettingData settings)
        {
            InitializeComponent();

            _dmd = dmd;

            ucStrategyModifications1.Initialize(dmd.StrategyOverrides, dmd.Strategy, true, settings, PermRunEditFlow.Edit, getSelectedStrategy, RefreshUIFromEffectiveStrategy);

            cboStrategy.Items.Add(string.Empty);
            foreach (Strategy s in Strategies)
            {
                cboStrategy.Items.Add(s);
            }
            if (dmd.Strategy != null)
            {
                cboStrategy.SelectedItem = dmd.Strategy;
            }
            else
            {
                cboStrategy.SelectedIndex = 0;
            }

            _initialized = true;
        }

        private Strategy getSelectedStrategy()
        {
            return cboStrategy.SelectedItem as Strategy;
        }

        public void SaveMobDynamicData()
        {
            Strategy s = getSelectedStrategy();
            _dmd.Strategy = s;
            _dmd.StrategyID = s == null ? 0 : s.ID;
            ucStrategyModifications1.SaveChanges();
        }

        private void cboStrategy_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Strategy s = cboStrategy.SelectedItem as Strategy;
            if (_initialized)
            {
                RefreshUIFromStrategy();
            }
        }

        /// <summary>
        /// refreshes the UI after the strategy dropdown changes
        /// </summary>
        private void RefreshUIFromStrategy()
        {
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            ucStrategyModifications1.RefreshUIFromStrategy(Strategy);
            RefreshUIFromEffectiveStrategy();
        }

        private void RefreshUIFromEffectiveStrategy()
        {
            CommandType eCombatTypes = ucStrategyModifications1.GetSelectedCombatTypes();
            Strategy Strategy = (Strategy)cboStrategy.SelectedItem;
            string grpStrategyText;
            if (Strategy == null)
                grpStrategyText = "No Strategy Selected";
            else
                grpStrategyText = "Strategy (" + Strategy.GetToStringForCommandTypes(eCombatTypes) + ")";
            ucStrategyModifications1.SetGroupBoxText(grpStrategyText);
        }
    }
}

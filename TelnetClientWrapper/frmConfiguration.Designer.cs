namespace IsengardClient
{
    partial class frmConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ctxRealm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCurrentRealmEarth = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmFire = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmWater = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentRealmWind = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPreferredAlignment = new System.Windows.Forms.Label();
            this.ctxAutoEscape = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSetCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearCurrentAutoEscapeThreshold = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoEscape1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCurrentAutoEscapeFlee = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeHazy = new System.Windows.Forms.ToolStripMenuItem();
            this.sepAutoEscape2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiCurrentAutoEscapeActive = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCurrentAutoEscapeInactive = new System.Windows.Forms.ToolStripMenuItem();
            this.chkQueryMonsterStatus = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPreferredAlignmentValue = new System.Windows.Forms.Label();
            this.ctxPreferredAlignment = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiPreferredAlignmentGood = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPreferredAlignmentEvil = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentAutoSpellLevelsValue = new System.Windows.Forms.Label();
            this.lblCurrentRealmValue = new System.Windows.Forms.Label();
            this.lblCurrentAutoEscapeValue = new System.Windows.Forms.Label();
            this.lblEmptyColorValue = new System.Windows.Forms.Label();
            this.lblEmptyColor = new System.Windows.Forms.Label();
            this.lblFullColorValue = new System.Windows.Forms.Label();
            this.lblFullColor = new System.Windows.Forms.Label();
            this.lstStrategies = new System.Windows.Forms.ListBox();
            this.ctxListModification = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemoveEntry = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveEntryUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveEntryDown = new System.Windows.Forms.ToolStripMenuItem();
            this.chkRemoveAllOnStartup = new System.Windows.Forms.CheckBox();
            this.tcConfiguration = new System.Windows.Forms.TabControl();
            this.tabConfiguration = new System.Windows.Forms.TabPage();
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.cboConsoleVerbosity = new System.Windows.Forms.ComboBox();
            this.lblConsoleVerbosity = new System.Windows.Forms.Label();
            this.chkGetNewPermRunOnBoatExitMissing = new System.Windows.Forms.CheckBox();
            this.chkSaveSettingsOnQuit = new System.Windows.Forms.CheckBox();
            this.txtMagicMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.chkDisplayStunLength = new System.Windows.Forms.CheckBox();
            this.lblMagicMendWhenDownXHP = new System.Windows.Forms.Label();
            this.btnSelectEmptyColor = new System.Windows.Forms.Button();
            this.txtMagicVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblMagicVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.btnSelectFullColor = new System.Windows.Forms.Button();
            this.txtPotionsMendWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsMendWhenDownXHP = new System.Windows.Forms.Label();
            this.txtPotionsVigorWhenDownXHP = new System.Windows.Forms.TextBox();
            this.lblPotionsVigorWhenDownXHP = new System.Windows.Forms.Label();
            this.tabStrategies = new System.Windows.Forms.TabPage();
            this.tabDynamicItemData = new System.Windows.Forms.TabPage();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.lvItems = new System.Windows.Forms.ListView();
            this.colItemName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colKeepCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSinkCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOverflowAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pnlItemsTop = new System.Windows.Forms.Panel();
            this.btnClearHeldItem = new System.Windows.Forms.Button();
            this.btnClearWeapon = new System.Windows.Forms.Button();
            this.txtHeldItem = new System.Windows.Forms.TextBox();
            this.lblHeldItem = new System.Windows.Forms.Label();
            this.txtCurrentWeaponValue = new System.Windows.Forms.TextBox();
            this.lblWeapon = new System.Windows.Forms.Label();
            this.btnSink = new System.Windows.Forms.Button();
            this.btnSellOrJunk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnKeep = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.tabAreas = new System.Windows.Forms.TabPage();
            this.treeAreas = new System.Windows.Forms.TreeView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.ctxAreas = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRealm.SuspendLayout();
            this.ctxAutoEscape.SuspendLayout();
            this.ctxPreferredAlignment.SuspendLayout();
            this.ctxListModification.SuspendLayout();
            this.tcConfiguration.SuspendLayout();
            this.tabConfiguration.SuspendLayout();
            this.pnlSettings.SuspendLayout();
            this.tabStrategies.SuspendLayout();
            this.tabDynamicItemData.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlItemsTop.SuspendLayout();
            this.tabAreas.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.ctxAreas.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxRealm
            // 
            this.ctxRealm.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRealm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCurrentRealmEarth,
            this.tsmiCurrentRealmFire,
            this.tsmiCurrentRealmWater,
            this.tsmiCurrentRealmWind});
            this.ctxRealm.Name = "ctxRealm";
            this.ctxRealm.Size = new System.Drawing.Size(104, 92);
            this.ctxRealm.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRealm_Opening);
            // 
            // tsmiCurrentRealmEarth
            // 
            this.tsmiCurrentRealmEarth.Name = "tsmiCurrentRealmEarth";
            this.tsmiCurrentRealmEarth.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmEarth.Text = "earth";
            this.tsmiCurrentRealmEarth.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmFire
            // 
            this.tsmiCurrentRealmFire.Name = "tsmiCurrentRealmFire";
            this.tsmiCurrentRealmFire.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmFire.Text = "fire";
            this.tsmiCurrentRealmFire.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWater
            // 
            this.tsmiCurrentRealmWater.Name = "tsmiCurrentRealmWater";
            this.tsmiCurrentRealmWater.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmWater.Text = "water";
            this.tsmiCurrentRealmWater.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // tsmiCurrentRealmWind
            // 
            this.tsmiCurrentRealmWind.Name = "tsmiCurrentRealmWind";
            this.tsmiCurrentRealmWind.Size = new System.Drawing.Size(103, 22);
            this.tsmiCurrentRealmWind.Text = "wind";
            this.tsmiCurrentRealmWind.Click += new System.EventHandler(this.tsmiCurrentRealm_Click);
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(17, 133);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(101, 13);
            this.lblPreferredAlignment.TabIndex = 128;
            this.lblPreferredAlignment.Text = "Preferred alignment:";
            // 
            // ctxAutoEscape
            // 
            this.ctxAutoEscape.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAutoEscape.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSetCurrentAutoEscapeThreshold,
            this.tsmiClearCurrentAutoEscapeThreshold,
            this.sepAutoEscape1,
            this.tsmiCurrentAutoEscapeFlee,
            this.tsmiCurrentAutoEscapeHazy,
            this.sepAutoEscape2,
            this.tsmiCurrentAutoEscapeActive,
            this.tsmiCurrentAutoEscapeInactive});
            this.ctxAutoEscape.Name = "ctxAutoEscape";
            this.ctxAutoEscape.Size = new System.Drawing.Size(200, 148);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiSetCurrentAutoEscapeThreshold
            // 
            this.tsmiSetCurrentAutoEscapeThreshold.Name = "tsmiSetCurrentAutoEscapeThreshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(199, 22);
            this.tsmiSetCurrentAutoEscapeThreshold.Text = "Set Current Threshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetCurrentAutoEscapeThreshold_Click);
            // 
            // tsmiClearCurrentAutoEscapeThreshold
            // 
            this.tsmiClearCurrentAutoEscapeThreshold.Name = "tsmiClearCurrentAutoEscapeThreshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(199, 22);
            this.tsmiClearCurrentAutoEscapeThreshold.Text = "Clear Current Threshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearCurrentAutoEscapeThreshold_Click);
            // 
            // sepAutoEscape1
            // 
            this.sepAutoEscape1.Name = "sepAutoEscape1";
            this.sepAutoEscape1.Size = new System.Drawing.Size(196, 6);
            // 
            // tsmiCurrentAutoEscapeFlee
            // 
            this.tsmiCurrentAutoEscapeFlee.Name = "tsmiCurrentAutoEscapeFlee";
            this.tsmiCurrentAutoEscapeFlee.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeFlee.Text = "Flee";
            this.tsmiCurrentAutoEscapeFlee.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeFlee_Click);
            // 
            // tsmiCurrentAutoEscapeHazy
            // 
            this.tsmiCurrentAutoEscapeHazy.Name = "tsmiCurrentAutoEscapeHazy";
            this.tsmiCurrentAutoEscapeHazy.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeHazy.Text = "Hazy";
            this.tsmiCurrentAutoEscapeHazy.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeHazy_Click);
            // 
            // sepAutoEscape2
            // 
            this.sepAutoEscape2.Name = "sepAutoEscape2";
            this.sepAutoEscape2.Size = new System.Drawing.Size(196, 6);
            // 
            // tsmiCurrentAutoEscapeActive
            // 
            this.tsmiCurrentAutoEscapeActive.Name = "tsmiCurrentAutoEscapeActive";
            this.tsmiCurrentAutoEscapeActive.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeActive.Text = "Active";
            this.tsmiCurrentAutoEscapeActive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeActive_Click);
            // 
            // tsmiCurrentAutoEscapeInactive
            // 
            this.tsmiCurrentAutoEscapeInactive.Name = "tsmiCurrentAutoEscapeInactive";
            this.tsmiCurrentAutoEscapeInactive.Size = new System.Drawing.Size(199, 22);
            this.tsmiCurrentAutoEscapeInactive.Text = "Inactive";
            this.tsmiCurrentAutoEscapeInactive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeInactive_Click);
            // 
            // chkQueryMonsterStatus
            // 
            this.chkQueryMonsterStatus.AutoSize = true;
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(358, 28);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(131, 17);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(654, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(73, 22);
            this.btnOK.TabIndex = 137;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(733, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 22);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblPreferredAlignmentValue
            // 
            this.lblPreferredAlignmentValue.BackColor = System.Drawing.Color.Blue;
            this.lblPreferredAlignmentValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblPreferredAlignmentValue.ForeColor = System.Drawing.Color.White;
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(142, 131);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(141, 15);
            this.lblPreferredAlignmentValue.TabIndex = 139;
            this.lblPreferredAlignmentValue.Text = "Good";
            this.lblPreferredAlignmentValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctxPreferredAlignment
            // 
            this.ctxPreferredAlignment.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxPreferredAlignment.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiPreferredAlignmentGood,
            this.tsmiPreferredAlignmentEvil});
            this.ctxPreferredAlignment.Name = "ctxPreferredAlignment";
            this.ctxPreferredAlignment.Size = new System.Drawing.Size(104, 48);
            this.ctxPreferredAlignment.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPreferredAlignment_Opening);
            // 
            // tsmiPreferredAlignmentGood
            // 
            this.tsmiPreferredAlignmentGood.Name = "tsmiPreferredAlignmentGood";
            this.tsmiPreferredAlignmentGood.Size = new System.Drawing.Size(103, 22);
            this.tsmiPreferredAlignmentGood.Text = "Good";
            this.tsmiPreferredAlignmentGood.Click += new System.EventHandler(this.tsmiPreferredAlignmentGood_Click);
            // 
            // tsmiPreferredAlignmentEvil
            // 
            this.tsmiPreferredAlignmentEvil.Name = "tsmiPreferredAlignmentEvil";
            this.tsmiPreferredAlignmentEvil.Size = new System.Drawing.Size(103, 22);
            this.tsmiPreferredAlignmentEvil.Text = "Evil";
            this.tsmiPreferredAlignmentEvil.Click += new System.EventHandler(this.tsmiPreferredAlignmentEvil_Click);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(117, 53);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 144;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.ContextMenuStrip = this.ctxRealm;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(111, 8);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentRealmValue.TabIndex = 141;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentAutoEscapeValue
            // 
            this.lblCurrentAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblCurrentAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblCurrentAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblCurrentAutoEscapeValue.Location = new System.Drawing.Point(117, 30);
            this.lblCurrentAutoEscapeValue.Name = "lblCurrentAutoEscapeValue";
            this.lblCurrentAutoEscapeValue.Size = new System.Drawing.Size(166, 15);
            this.lblCurrentAutoEscapeValue.TabIndex = 143;
            this.lblCurrentAutoEscapeValue.Text = "Auto Escape";
            this.lblCurrentAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColorValue
            // 
            this.lblEmptyColorValue.BackColor = System.Drawing.Color.Red;
            this.lblEmptyColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblEmptyColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblEmptyColorValue.Location = new System.Drawing.Point(117, 105);
            this.lblEmptyColorValue.Name = "lblEmptyColorValue";
            this.lblEmptyColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblEmptyColorValue.TabIndex = 143;
            this.lblEmptyColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColor
            // 
            this.lblEmptyColor.AutoSize = true;
            this.lblEmptyColor.Location = new System.Drawing.Point(17, 105);
            this.lblEmptyColor.Name = "lblEmptyColor";
            this.lblEmptyColor.Size = new System.Drawing.Size(65, 13);
            this.lblEmptyColor.TabIndex = 142;
            this.lblEmptyColor.Text = "Empty color:";
            // 
            // lblFullColorValue
            // 
            this.lblFullColorValue.BackColor = System.Drawing.Color.Green;
            this.lblFullColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblFullColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblFullColorValue.Location = new System.Drawing.Point(117, 79);
            this.lblFullColorValue.Name = "lblFullColorValue";
            this.lblFullColorValue.Size = new System.Drawing.Size(166, 15);
            this.lblFullColorValue.TabIndex = 141;
            this.lblFullColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullColor
            // 
            this.lblFullColor.AutoSize = true;
            this.lblFullColor.Location = new System.Drawing.Point(17, 81);
            this.lblFullColor.Name = "lblFullColor";
            this.lblFullColor.Size = new System.Drawing.Size(52, 13);
            this.lblFullColor.TabIndex = 140;
            this.lblFullColor.Text = "Full color:";
            // 
            // lstStrategies
            // 
            this.lstStrategies.ContextMenuStrip = this.ctxListModification;
            this.lstStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStrategies.FormattingEnabled = true;
            this.lstStrategies.Location = new System.Drawing.Point(2, 2);
            this.lstStrategies.Name = "lstStrategies";
            this.lstStrategies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstStrategies.Size = new System.Drawing.Size(812, 546);
            this.lstStrategies.TabIndex = 0;
            // 
            // ctxListModification
            // 
            this.ctxListModification.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxListModification.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddEntry,
            this.tsmiEditEntry,
            this.tsmiRemoveEntry,
            this.tsmiMoveEntryUp,
            this.tsmiMoveEntryDown});
            this.ctxListModification.Name = "ctxStrategies";
            this.ctxListModification.Size = new System.Drawing.Size(139, 114);
            this.ctxListModification.Opening += new System.ComponentModel.CancelEventHandler(this.ctxListModification_Opening);
            // 
            // tsmiAddEntry
            // 
            this.tsmiAddEntry.Name = "tsmiAddEntry";
            this.tsmiAddEntry.Size = new System.Drawing.Size(138, 22);
            this.tsmiAddEntry.Text = "Add";
            this.tsmiAddEntry.Click += new System.EventHandler(this.tsmiAddEntry_Click);
            // 
            // tsmiEditEntry
            // 
            this.tsmiEditEntry.Name = "tsmiEditEntry";
            this.tsmiEditEntry.Size = new System.Drawing.Size(138, 22);
            this.tsmiEditEntry.Text = "Edit";
            this.tsmiEditEntry.Click += new System.EventHandler(this.tsmiEditEntry_Click);
            // 
            // tsmiRemoveEntry
            // 
            this.tsmiRemoveEntry.Name = "tsmiRemoveEntry";
            this.tsmiRemoveEntry.Size = new System.Drawing.Size(138, 22);
            this.tsmiRemoveEntry.Text = "Remove";
            this.tsmiRemoveEntry.Click += new System.EventHandler(this.tsmiRemoveEntry_Click);
            // 
            // tsmiMoveEntryUp
            // 
            this.tsmiMoveEntryUp.Name = "tsmiMoveEntryUp";
            this.tsmiMoveEntryUp.Size = new System.Drawing.Size(138, 22);
            this.tsmiMoveEntryUp.Text = "Move Up";
            this.tsmiMoveEntryUp.Click += new System.EventHandler(this.tsmiMoveEntryUp_Click);
            // 
            // tsmiMoveEntryDown
            // 
            this.tsmiMoveEntryDown.Name = "tsmiMoveEntryDown";
            this.tsmiMoveEntryDown.Size = new System.Drawing.Size(138, 22);
            this.tsmiMoveEntryDown.Text = "Move Down";
            this.tsmiMoveEntryDown.Click += new System.EventHandler(this.tsmiMoveEntryDown_Click);
            // 
            // chkRemoveAllOnStartup
            // 
            this.chkRemoveAllOnStartup.AutoSize = true;
            this.chkRemoveAllOnStartup.Location = new System.Drawing.Point(358, 53);
            this.chkRemoveAllOnStartup.Name = "chkRemoveAllOnStartup";
            this.chkRemoveAllOnStartup.Size = new System.Drawing.Size(135, 17);
            this.chkRemoveAllOnStartup.TabIndex = 146;
            this.chkRemoveAllOnStartup.Text = "Remove all on startup?";
            this.chkRemoveAllOnStartup.UseVisualStyleBackColor = true;
            // 
            // tcConfiguration
            // 
            this.tcConfiguration.Controls.Add(this.tabConfiguration);
            this.tcConfiguration.Controls.Add(this.tabStrategies);
            this.tcConfiguration.Controls.Add(this.tabDynamicItemData);
            this.tcConfiguration.Controls.Add(this.tabAreas);
            this.tcConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcConfiguration.Location = new System.Drawing.Point(0, 0);
            this.tcConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.tcConfiguration.Name = "tcConfiguration";
            this.tcConfiguration.SelectedIndex = 0;
            this.tcConfiguration.Size = new System.Drawing.Size(824, 576);
            this.tcConfiguration.TabIndex = 147;
            // 
            // tabConfiguration
            // 
            this.tabConfiguration.Controls.Add(this.pnlSettings);
            this.tabConfiguration.Location = new System.Drawing.Point(4, 22);
            this.tabConfiguration.Margin = new System.Windows.Forms.Padding(2);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.Padding = new System.Windows.Forms.Padding(2);
            this.tabConfiguration.Size = new System.Drawing.Size(816, 550);
            this.tabConfiguration.TabIndex = 0;
            this.tabConfiguration.Text = "Settings";
            this.tabConfiguration.UseVisualStyleBackColor = true;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.cboConsoleVerbosity);
            this.pnlSettings.Controls.Add(this.lblConsoleVerbosity);
            this.pnlSettings.Controls.Add(this.chkGetNewPermRunOnBoatExitMissing);
            this.pnlSettings.Controls.Add(this.chkSaveSettingsOnQuit);
            this.pnlSettings.Controls.Add(this.txtMagicMendWhenDownXHP);
            this.pnlSettings.Controls.Add(this.chkDisplayStunLength);
            this.pnlSettings.Controls.Add(this.lblMagicMendWhenDownXHP);
            this.pnlSettings.Controls.Add(this.btnSelectEmptyColor);
            this.pnlSettings.Controls.Add(this.txtMagicVigorWhenDownXHP);
            this.pnlSettings.Controls.Add(this.lblFullColorValue);
            this.pnlSettings.Controls.Add(this.lblMagicVigorWhenDownXHP);
            this.pnlSettings.Controls.Add(this.btnSelectFullColor);
            this.pnlSettings.Controls.Add(this.txtPotionsMendWhenDownXHP);
            this.pnlSettings.Controls.Add(this.lblPotionsMendWhenDownXHP);
            this.pnlSettings.Controls.Add(this.lblCurrentRealmValue);
            this.pnlSettings.Controls.Add(this.txtPotionsVigorWhenDownXHP);
            this.pnlSettings.Controls.Add(this.lblPotionsVigorWhenDownXHP);
            this.pnlSettings.Controls.Add(this.lblCurrentAutoEscapeValue);
            this.pnlSettings.Controls.Add(this.chkQueryMonsterStatus);
            this.pnlSettings.Controls.Add(this.lblCurrentAutoSpellLevelsValue);
            this.pnlSettings.Controls.Add(this.lblFullColor);
            this.pnlSettings.Controls.Add(this.lblPreferredAlignment);
            this.pnlSettings.Controls.Add(this.lblEmptyColor);
            this.pnlSettings.Controls.Add(this.lblEmptyColorValue);
            this.pnlSettings.Controls.Add(this.lblPreferredAlignmentValue);
            this.pnlSettings.Controls.Add(this.chkRemoveAllOnStartup);
            this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSettings.Location = new System.Drawing.Point(2, 2);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(2);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(812, 546);
            this.pnlSettings.TabIndex = 150;
            // 
            // cboConsoleVerbosity
            // 
            this.cboConsoleVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsoleVerbosity.FormattingEnabled = true;
            this.cboConsoleVerbosity.Items.AddRange(new object[] {
            "Minimum",
            "Default",
            "Maximum"});
            this.cboConsoleVerbosity.Location = new System.Drawing.Point(196, 266);
            this.cboConsoleVerbosity.Name = "cboConsoleVerbosity";
            this.cboConsoleVerbosity.Size = new System.Drawing.Size(87, 21);
            this.cboConsoleVerbosity.TabIndex = 160;
            // 
            // lblConsoleVerbosity
            // 
            this.lblConsoleVerbosity.AutoSize = true;
            this.lblConsoleVerbosity.Location = new System.Drawing.Point(19, 269);
            this.lblConsoleVerbosity.Name = "lblConsoleVerbosity";
            this.lblConsoleVerbosity.Size = new System.Drawing.Size(93, 13);
            this.lblConsoleVerbosity.TabIndex = 159;
            this.lblConsoleVerbosity.Text = "Console verbosity:";
            // 
            // chkGetNewPermRunOnBoatExitMissing
            // 
            this.chkGetNewPermRunOnBoatExitMissing.AutoSize = true;
            this.chkGetNewPermRunOnBoatExitMissing.Location = new System.Drawing.Point(358, 126);
            this.chkGetNewPermRunOnBoatExitMissing.Name = "chkGetNewPermRunOnBoatExitMissing";
            this.chkGetNewPermRunOnBoatExitMissing.Size = new System.Drawing.Size(211, 17);
            this.chkGetNewPermRunOnBoatExitMissing.TabIndex = 158;
            this.chkGetNewPermRunOnBoatExitMissing.Text = "Get new perm run on boat exit missing?";
            this.chkGetNewPermRunOnBoatExitMissing.UseVisualStyleBackColor = true;
            // 
            // chkSaveSettingsOnQuit
            // 
            this.chkSaveSettingsOnQuit.AutoSize = true;
            this.chkSaveSettingsOnQuit.Location = new System.Drawing.Point(358, 101);
            this.chkSaveSettingsOnQuit.Name = "chkSaveSettingsOnQuit";
            this.chkSaveSettingsOnQuit.Size = new System.Drawing.Size(131, 17);
            this.chkSaveSettingsOnQuit.TabIndex = 157;
            this.chkSaveSettingsOnQuit.Text = "Save settings on quit?";
            this.chkSaveSettingsOnQuit.UseVisualStyleBackColor = true;
            // 
            // txtMagicMendWhenDownXHP
            // 
            this.txtMagicMendWhenDownXHP.Location = new System.Drawing.Point(196, 187);
            this.txtMagicMendWhenDownXHP.Name = "txtMagicMendWhenDownXHP";
            this.txtMagicMendWhenDownXHP.Size = new System.Drawing.Size(87, 20);
            this.txtMagicMendWhenDownXHP.TabIndex = 152;
            // 
            // chkDisplayStunLength
            // 
            this.chkDisplayStunLength.AutoSize = true;
            this.chkDisplayStunLength.Location = new System.Drawing.Point(358, 76);
            this.chkDisplayStunLength.Name = "chkDisplayStunLength";
            this.chkDisplayStunLength.Size = new System.Drawing.Size(121, 17);
            this.chkDisplayStunLength.TabIndex = 149;
            this.chkDisplayStunLength.Text = "Display stun length?";
            this.chkDisplayStunLength.UseVisualStyleBackColor = true;
            // 
            // lblMagicMendWhenDownXHP
            // 
            this.lblMagicMendWhenDownXHP.AutoSize = true;
            this.lblMagicMendWhenDownXHP.Location = new System.Drawing.Point(19, 190);
            this.lblMagicMendWhenDownXHP.Name = "lblMagicMendWhenDownXHP";
            this.lblMagicMendWhenDownXHP.Size = new System.Drawing.Size(158, 13);
            this.lblMagicMendWhenDownXHP.TabIndex = 151;
            this.lblMagicMendWhenDownXHP.Text = "Magic: Mend when down X HP:";
            // 
            // btnSelectEmptyColor
            // 
            this.btnSelectEmptyColor.Location = new System.Drawing.Point(289, 100);
            this.btnSelectEmptyColor.Name = "btnSelectEmptyColor";
            this.btnSelectEmptyColor.Size = new System.Drawing.Size(55, 23);
            this.btnSelectEmptyColor.TabIndex = 148;
            this.btnSelectEmptyColor.Text = "Select";
            this.btnSelectEmptyColor.UseVisualStyleBackColor = true;
            this.btnSelectEmptyColor.Click += new System.EventHandler(this.btnSelectEmptyColor_Click);
            // 
            // txtMagicVigorWhenDownXHP
            // 
            this.txtMagicVigorWhenDownXHP.Location = new System.Drawing.Point(196, 162);
            this.txtMagicVigorWhenDownXHP.Name = "txtMagicVigorWhenDownXHP";
            this.txtMagicVigorWhenDownXHP.Size = new System.Drawing.Size(87, 20);
            this.txtMagicVigorWhenDownXHP.TabIndex = 150;
            // 
            // lblMagicVigorWhenDownXHP
            // 
            this.lblMagicVigorWhenDownXHP.AutoSize = true;
            this.lblMagicVigorWhenDownXHP.Location = new System.Drawing.Point(19, 166);
            this.lblMagicVigorWhenDownXHP.Name = "lblMagicVigorWhenDownXHP";
            this.lblMagicVigorWhenDownXHP.Size = new System.Drawing.Size(155, 13);
            this.lblMagicVigorWhenDownXHP.TabIndex = 149;
            this.lblMagicVigorWhenDownXHP.Text = "Magic: Vigor when down X HP:";
            // 
            // btnSelectFullColor
            // 
            this.btnSelectFullColor.Location = new System.Drawing.Point(289, 76);
            this.btnSelectFullColor.Name = "btnSelectFullColor";
            this.btnSelectFullColor.Size = new System.Drawing.Size(55, 23);
            this.btnSelectFullColor.TabIndex = 147;
            this.btnSelectFullColor.Text = "Select";
            this.btnSelectFullColor.UseVisualStyleBackColor = true;
            this.btnSelectFullColor.Click += new System.EventHandler(this.btnSelectFullColor_Click);
            // 
            // txtPotionsMendWhenDownXHP
            // 
            this.txtPotionsMendWhenDownXHP.Location = new System.Drawing.Point(196, 239);
            this.txtPotionsMendWhenDownXHP.Name = "txtPotionsMendWhenDownXHP";
            this.txtPotionsMendWhenDownXHP.Size = new System.Drawing.Size(87, 20);
            this.txtPotionsMendWhenDownXHP.TabIndex = 156;
            // 
            // lblPotionsMendWhenDownXHP
            // 
            this.lblPotionsMendWhenDownXHP.AutoSize = true;
            this.lblPotionsMendWhenDownXHP.Location = new System.Drawing.Point(19, 242);
            this.lblPotionsMendWhenDownXHP.Name = "lblPotionsMendWhenDownXHP";
            this.lblPotionsMendWhenDownXHP.Size = new System.Drawing.Size(164, 13);
            this.lblPotionsMendWhenDownXHP.TabIndex = 155;
            this.lblPotionsMendWhenDownXHP.Text = "Potions: Mend when down X HP:";
            // 
            // txtPotionsVigorWhenDownXHP
            // 
            this.txtPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(196, 214);
            this.txtPotionsVigorWhenDownXHP.Name = "txtPotionsVigorWhenDownXHP";
            this.txtPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(87, 20);
            this.txtPotionsVigorWhenDownXHP.TabIndex = 154;
            // 
            // lblPotionsVigorWhenDownXHP
            // 
            this.lblPotionsVigorWhenDownXHP.AutoSize = true;
            this.lblPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(19, 217);
            this.lblPotionsVigorWhenDownXHP.Name = "lblPotionsVigorWhenDownXHP";
            this.lblPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(161, 13);
            this.lblPotionsVigorWhenDownXHP.TabIndex = 153;
            this.lblPotionsVigorWhenDownXHP.Text = "Potions: Vigor when down X HP:";
            // 
            // tabStrategies
            // 
            this.tabStrategies.Controls.Add(this.lstStrategies);
            this.tabStrategies.Location = new System.Drawing.Point(4, 22);
            this.tabStrategies.Margin = new System.Windows.Forms.Padding(2);
            this.tabStrategies.Name = "tabStrategies";
            this.tabStrategies.Padding = new System.Windows.Forms.Padding(2);
            this.tabStrategies.Size = new System.Drawing.Size(816, 550);
            this.tabStrategies.TabIndex = 1;
            this.tabStrategies.Text = "Strategies";
            this.tabStrategies.UseVisualStyleBackColor = true;
            // 
            // tabDynamicItemData
            // 
            this.tabDynamicItemData.Controls.Add(this.pnlMiddle);
            this.tabDynamicItemData.Controls.Add(this.pnlItemsTop);
            this.tabDynamicItemData.Location = new System.Drawing.Point(4, 22);
            this.tabDynamicItemData.Margin = new System.Windows.Forms.Padding(2);
            this.tabDynamicItemData.Name = "tabDynamicItemData";
            this.tabDynamicItemData.Size = new System.Drawing.Size(816, 550);
            this.tabDynamicItemData.TabIndex = 2;
            this.tabDynamicItemData.Text = "Items";
            this.tabDynamicItemData.UseVisualStyleBackColor = true;
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMiddle.Controls.Add(this.lvItems);
            this.pnlMiddle.Location = new System.Drawing.Point(-4, 76);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(820, 429);
            this.pnlMiddle.TabIndex = 2;
            // 
            // lvItems
            // 
            this.lvItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItemName,
            this.colKeepCount,
            this.colSinkCount,
            this.colOverflowAction});
            this.lvItems.ContextMenuStrip = this.ctxItems;
            this.lvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvItems.FullRowSelect = true;
            this.lvItems.HideSelection = false;
            this.lvItems.Location = new System.Drawing.Point(0, 0);
            this.lvItems.Margin = new System.Windows.Forms.Padding(2);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(820, 429);
            this.lvItems.TabIndex = 1;
            this.lvItems.UseCompatibleStateImageBehavior = false;
            this.lvItems.View = System.Windows.Forms.View.Details;
            this.lvItems.SelectedIndexChanged += new System.EventHandler(this.lvItems_SelectedIndexChanged);
            // 
            // colItemName
            // 
            this.colItemName.Text = "Item";
            this.colItemName.Width = 280;
            // 
            // colKeepCount
            // 
            this.colKeepCount.Text = "Keep #";
            this.colKeepCount.Width = 77;
            // 
            // colSinkCount
            // 
            this.colSinkCount.Text = "Tick #";
            this.colSinkCount.Width = 84;
            // 
            // colOverflowAction
            // 
            this.colOverflowAction.Text = "Overflow Action";
            this.colOverflowAction.Width = 97;
            // 
            // ctxItems
            // 
            this.ctxItems.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxItems.Name = "ctxItems";
            this.ctxItems.Size = new System.Drawing.Size(61, 4);
            this.ctxItems.Opening += new System.ComponentModel.CancelEventHandler(this.ctxItems_Opening);
            this.ctxItems.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxItems_ItemClicked);
            // 
            // pnlItemsTop
            // 
            this.pnlItemsTop.Controls.Add(this.btnClearHeldItem);
            this.pnlItemsTop.Controls.Add(this.btnClearWeapon);
            this.pnlItemsTop.Controls.Add(this.txtHeldItem);
            this.pnlItemsTop.Controls.Add(this.lblHeldItem);
            this.pnlItemsTop.Controls.Add(this.txtCurrentWeaponValue);
            this.pnlItemsTop.Controls.Add(this.lblWeapon);
            this.pnlItemsTop.Controls.Add(this.btnSink);
            this.pnlItemsTop.Controls.Add(this.btnSellOrJunk);
            this.pnlItemsTop.Controls.Add(this.btnClear);
            this.pnlItemsTop.Controls.Add(this.btnKeep);
            this.pnlItemsTop.Controls.Add(this.btnIgnore);
            this.pnlItemsTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlItemsTop.Location = new System.Drawing.Point(0, 0);
            this.pnlItemsTop.Margin = new System.Windows.Forms.Padding(2);
            this.pnlItemsTop.Name = "pnlItemsTop";
            this.pnlItemsTop.Size = new System.Drawing.Size(816, 71);
            this.pnlItemsTop.TabIndex = 0;
            // 
            // btnClearHeldItem
            // 
            this.btnClearHeldItem.Location = new System.Drawing.Point(245, 36);
            this.btnClearHeldItem.Name = "btnClearHeldItem";
            this.btnClearHeldItem.Size = new System.Drawing.Size(24, 20);
            this.btnClearHeldItem.TabIndex = 148;
            this.btnClearHeldItem.Text = "X";
            this.btnClearHeldItem.UseVisualStyleBackColor = true;
            this.btnClearHeldItem.Click += new System.EventHandler(this.btnClearHeldItem_Click);
            // 
            // btnClearWeapon
            // 
            this.btnClearWeapon.Location = new System.Drawing.Point(245, 12);
            this.btnClearWeapon.Name = "btnClearWeapon";
            this.btnClearWeapon.Size = new System.Drawing.Size(24, 20);
            this.btnClearWeapon.TabIndex = 147;
            this.btnClearWeapon.Text = "X";
            this.btnClearWeapon.UseVisualStyleBackColor = true;
            this.btnClearWeapon.Click += new System.EventHandler(this.btnClearWeapon_Click);
            // 
            // txtHeldItem
            // 
            this.txtHeldItem.Enabled = false;
            this.txtHeldItem.Location = new System.Drawing.Point(72, 36);
            this.txtHeldItem.Name = "txtHeldItem";
            this.txtHeldItem.Size = new System.Drawing.Size(167, 20);
            this.txtHeldItem.TabIndex = 146;
            // 
            // lblHeldItem
            // 
            this.lblHeldItem.AutoSize = true;
            this.lblHeldItem.Location = new System.Drawing.Point(10, 39);
            this.lblHeldItem.Name = "lblHeldItem";
            this.lblHeldItem.Size = new System.Drawing.Size(32, 13);
            this.lblHeldItem.TabIndex = 145;
            this.lblHeldItem.Text = "Held:";
            // 
            // txtCurrentWeaponValue
            // 
            this.txtCurrentWeaponValue.Enabled = false;
            this.txtCurrentWeaponValue.Location = new System.Drawing.Point(72, 12);
            this.txtCurrentWeaponValue.Name = "txtCurrentWeaponValue";
            this.txtCurrentWeaponValue.Size = new System.Drawing.Size(167, 20);
            this.txtCurrentWeaponValue.TabIndex = 144;
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(10, 15);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(51, 13);
            this.lblWeapon.TabIndex = 143;
            this.lblWeapon.Text = "Weapon:";
            // 
            // btnSink
            // 
            this.btnSink.Location = new System.Drawing.Point(364, 10);
            this.btnSink.Margin = new System.Windows.Forms.Padding(2);
            this.btnSink.Name = "btnSink";
            this.btnSink.Size = new System.Drawing.Size(56, 21);
            this.btnSink.TabIndex = 4;
            this.btnSink.Text = "Sink";
            this.btnSink.UseVisualStyleBackColor = true;
            this.btnSink.Click += new System.EventHandler(this.btnSink_Click);
            // 
            // btnSellOrJunk
            // 
            this.btnSellOrJunk.Location = new System.Drawing.Point(365, 35);
            this.btnSellOrJunk.Margin = new System.Windows.Forms.Padding(2);
            this.btnSellOrJunk.Name = "btnSellOrJunk";
            this.btnSellOrJunk.Size = new System.Drawing.Size(71, 21);
            this.btnSellOrJunk.TabIndex = 3;
            this.btnSellOrJunk.Text = "Sell/Junk";
            this.btnSellOrJunk.UseVisualStyleBackColor = true;
            this.btnSellOrJunk.Click += new System.EventHandler(this.btnSellOrJunk_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(305, 35);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(56, 21);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnKeep
            // 
            this.btnKeep.Location = new System.Drawing.Point(304, 10);
            this.btnKeep.Margin = new System.Windows.Forms.Padding(2);
            this.btnKeep.Name = "btnKeep";
            this.btnKeep.Size = new System.Drawing.Size(56, 21);
            this.btnKeep.TabIndex = 1;
            this.btnKeep.Text = "Keep";
            this.btnKeep.UseVisualStyleBackColor = true;
            this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(440, 35);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(2);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(56, 21);
            this.btnIgnore.TabIndex = 0;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // tabAreas
            // 
            this.tabAreas.Controls.Add(this.treeAreas);
            this.tabAreas.Location = new System.Drawing.Point(4, 22);
            this.tabAreas.Name = "tabAreas";
            this.tabAreas.Size = new System.Drawing.Size(816, 550);
            this.tabAreas.TabIndex = 3;
            this.tabAreas.Text = "Areas";
            this.tabAreas.UseVisualStyleBackColor = true;
            // 
            // treeAreas
            // 
            this.treeAreas.ContextMenuStrip = this.ctxAreas;
            this.treeAreas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeAreas.Location = new System.Drawing.Point(0, 0);
            this.treeAreas.Name = "treeAreas";
            this.treeAreas.Size = new System.Drawing.Size(816, 550);
            this.treeAreas.TabIndex = 0;
            this.treeAreas.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeAreas_BeforeCollapse);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 532);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(824, 44);
            this.pnlBottom.TabIndex = 148;
            // 
            // ctxAreas
            // 
            this.ctxAreas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddChild,
            this.tsmiAddSiblingBefore,
            this.tsmiAddSiblingAfter,
            this.tsmiEdit,
            this.tsmiRemove,
            this.tsmiMoveUp,
            this.tsmiMoveDown});
            this.ctxAreas.Name = "ctxAreas";
            this.ctxAreas.Size = new System.Drawing.Size(173, 158);
            this.ctxAreas.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAreas_Opening);
            this.ctxAreas.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAreas_ItemClicked);
            // 
            // tsmiAddChild
            // 
            this.tsmiAddChild.Name = "tsmiAddChild";
            this.tsmiAddChild.Size = new System.Drawing.Size(172, 22);
            this.tsmiAddChild.Text = "Add Child";
            // 
            // tsmiAddSiblingBefore
            // 
            this.tsmiAddSiblingBefore.Name = "tsmiAddSiblingBefore";
            this.tsmiAddSiblingBefore.Size = new System.Drawing.Size(172, 22);
            this.tsmiAddSiblingBefore.Text = "Add Sibling Before";
            // 
            // tsmiAddSiblingAfter
            // 
            this.tsmiAddSiblingAfter.Name = "tsmiAddSiblingAfter";
            this.tsmiAddSiblingAfter.Size = new System.Drawing.Size(172, 22);
            this.tsmiAddSiblingAfter.Text = "Add Sibling After";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(172, 22);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiRemove
            // 
            this.tsmiRemove.Name = "tsmiRemove";
            this.tsmiRemove.Size = new System.Drawing.Size(172, 22);
            this.tsmiRemove.Text = "Remove";
            // 
            // tsmiMoveUp
            // 
            this.tsmiMoveUp.Name = "tsmiMoveUp";
            this.tsmiMoveUp.Size = new System.Drawing.Size(172, 22);
            this.tsmiMoveUp.Text = "Move Up";
            // 
            // tsmiMoveDown
            // 
            this.tsmiMoveDown.Name = "tsmiMoveDown";
            this.tsmiMoveDown.Size = new System.Drawing.Size(172, 22);
            this.tsmiMoveDown.Text = "Move Down";
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 576);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.tcConfiguration);
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.ctxRealm.ResumeLayout(false);
            this.ctxAutoEscape.ResumeLayout(false);
            this.ctxPreferredAlignment.ResumeLayout(false);
            this.ctxListModification.ResumeLayout(false);
            this.tcConfiguration.ResumeLayout(false);
            this.tabConfiguration.ResumeLayout(false);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.tabStrategies.ResumeLayout(false);
            this.tabDynamicItemData.ResumeLayout(false);
            this.pnlMiddle.ResumeLayout(false);
            this.pnlItemsTop.ResumeLayout(false);
            this.pnlItemsTop.PerformLayout();
            this.tabAreas.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ctxAreas.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblPreferredAlignment;
        private System.Windows.Forms.CheckBox chkQueryMonsterStatus;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip ctxAutoEscape;
        private System.Windows.Forms.Label lblPreferredAlignmentValue;
        private System.Windows.Forms.ContextMenuStrip ctxPreferredAlignment;
        private System.Windows.Forms.Label lblFullColorValue;
        private System.Windows.Forms.Label lblFullColor;
        private System.Windows.Forms.Label lblEmptyColorValue;
        private System.Windows.Forms.Label lblEmptyColor;
        private System.Windows.Forms.ListBox lstStrategies;
        private System.Windows.Forms.ContextMenuStrip ctxListModification;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddEntry;
        private System.Windows.Forms.ToolStripMenuItem tsmiEditEntry;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveEntry;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveEntryUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveEntryDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentGood;
        private System.Windows.Forms.ToolStripMenuItem tsmiPreferredAlignmentEvil;
        private System.Windows.Forms.Label lblCurrentAutoSpellLevelsValue;
        private System.Windows.Forms.Label lblCurrentRealmValue;
        private System.Windows.Forms.Label lblCurrentAutoEscapeValue;
        private System.Windows.Forms.ContextMenuStrip ctxRealm;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmEarth;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmFire;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWater;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentRealmWind;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearCurrentAutoEscapeThreshold;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeFlee;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeHazy;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeActive;
        private System.Windows.Forms.ToolStripMenuItem tsmiCurrentAutoEscapeInactive;
        private System.Windows.Forms.CheckBox chkRemoveAllOnStartup;
        private System.Windows.Forms.ToolStripSeparator sepAutoEscape1;
        private System.Windows.Forms.ToolStripSeparator sepAutoEscape2;
        private System.Windows.Forms.TabControl tcConfiguration;
        private System.Windows.Forms.TabPage tabConfiguration;
        private System.Windows.Forms.Button btnSelectEmptyColor;
        private System.Windows.Forms.Button btnSelectFullColor;
        private System.Windows.Forms.TabPage tabStrategies;
        private System.Windows.Forms.TabPage tabDynamicItemData;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlItemsTop;
        private System.Windows.Forms.ListView lvItems;
        private System.Windows.Forms.ColumnHeader colItemName;
        private System.Windows.Forms.Button btnKeep;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSellOrJunk;
        private System.Windows.Forms.Button btnSink;
        private System.Windows.Forms.CheckBox chkDisplayStunLength;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.ColumnHeader colKeepCount;
        private System.Windows.Forms.ColumnHeader colSinkCount;
        private System.Windows.Forms.ColumnHeader colOverflowAction;
        private System.Windows.Forms.TextBox txtMagicMendWhenDownXHP;
        private System.Windows.Forms.Label lblMagicMendWhenDownXHP;
        private System.Windows.Forms.TextBox txtMagicVigorWhenDownXHP;
        private System.Windows.Forms.Label lblMagicVigorWhenDownXHP;
        private System.Windows.Forms.TextBox txtPotionsMendWhenDownXHP;
        private System.Windows.Forms.Label lblPotionsMendWhenDownXHP;
        private System.Windows.Forms.TextBox txtPotionsVigorWhenDownXHP;
        private System.Windows.Forms.Label lblPotionsVigorWhenDownXHP;
        private System.Windows.Forms.CheckBox chkSaveSettingsOnQuit;
        private System.Windows.Forms.TabPage tabAreas;
        private System.Windows.Forms.TextBox txtCurrentWeaponValue;
        private System.Windows.Forms.Label lblWeapon;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.TextBox txtHeldItem;
        private System.Windows.Forms.Label lblHeldItem;
        private System.Windows.Forms.Button btnClearWeapon;
        private System.Windows.Forms.Button btnClearHeldItem;
        private System.Windows.Forms.ContextMenuStrip ctxItems;
        private System.Windows.Forms.CheckBox chkGetNewPermRunOnBoatExitMissing;
        private System.Windows.Forms.ComboBox cboConsoleVerbosity;
        private System.Windows.Forms.Label lblConsoleVerbosity;
        private System.Windows.Forms.TreeView treeAreas;
        private System.Windows.Forms.ContextMenuStrip ctxAreas;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddChild;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingBefore;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingAfter;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveDown;
    }
}
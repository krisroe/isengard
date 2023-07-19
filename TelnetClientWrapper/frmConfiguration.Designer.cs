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
            this.lblAutoSpellLevels = new System.Windows.Forms.Label();
            this.lblAutoEscape = new System.Windows.Forms.Label();
            this.lblRealm = new System.Windows.Forms.Label();
            this.cboConsoleVerbosity = new System.Windows.Forms.ComboBox();
            this.lblConsoleVerbosity = new System.Windows.Forms.Label();
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
            this.ctxAreas = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.txtCommandTimeoutSeconds = new System.Windows.Forms.TextBox();
            this.lblCommandTimeoutSeconds = new System.Windows.Forms.Label();
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
            this.ctxAreas.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPreferredAlignment
            // 
            this.lblPreferredAlignment.AutoSize = true;
            this.lblPreferredAlignment.Location = new System.Drawing.Point(23, 164);
            this.lblPreferredAlignment.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreferredAlignment.Name = "lblPreferredAlignment";
            this.lblPreferredAlignment.Size = new System.Drawing.Size(127, 16);
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
            this.ctxAutoEscape.Size = new System.Drawing.Size(234, 160);
            this.ctxAutoEscape.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAutoEscape_Opening);
            // 
            // tsmiSetCurrentAutoEscapeThreshold
            // 
            this.tsmiSetCurrentAutoEscapeThreshold.Name = "tsmiSetCurrentAutoEscapeThreshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(233, 24);
            this.tsmiSetCurrentAutoEscapeThreshold.Text = "Set Current Threshold";
            this.tsmiSetCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiSetCurrentAutoEscapeThreshold_Click);
            // 
            // tsmiClearCurrentAutoEscapeThreshold
            // 
            this.tsmiClearCurrentAutoEscapeThreshold.Name = "tsmiClearCurrentAutoEscapeThreshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Size = new System.Drawing.Size(233, 24);
            this.tsmiClearCurrentAutoEscapeThreshold.Text = "Clear Current Threshold";
            this.tsmiClearCurrentAutoEscapeThreshold.Click += new System.EventHandler(this.tsmiClearCurrentAutoEscapeThreshold_Click);
            // 
            // sepAutoEscape1
            // 
            this.sepAutoEscape1.Name = "sepAutoEscape1";
            this.sepAutoEscape1.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCurrentAutoEscapeFlee
            // 
            this.tsmiCurrentAutoEscapeFlee.Name = "tsmiCurrentAutoEscapeFlee";
            this.tsmiCurrentAutoEscapeFlee.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeFlee.Text = "Flee";
            this.tsmiCurrentAutoEscapeFlee.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeFlee_Click);
            // 
            // tsmiCurrentAutoEscapeHazy
            // 
            this.tsmiCurrentAutoEscapeHazy.Name = "tsmiCurrentAutoEscapeHazy";
            this.tsmiCurrentAutoEscapeHazy.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeHazy.Text = "Hazy";
            this.tsmiCurrentAutoEscapeHazy.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeHazy_Click);
            // 
            // sepAutoEscape2
            // 
            this.sepAutoEscape2.Name = "sepAutoEscape2";
            this.sepAutoEscape2.Size = new System.Drawing.Size(230, 6);
            // 
            // tsmiCurrentAutoEscapeActive
            // 
            this.tsmiCurrentAutoEscapeActive.Name = "tsmiCurrentAutoEscapeActive";
            this.tsmiCurrentAutoEscapeActive.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeActive.Text = "Active";
            this.tsmiCurrentAutoEscapeActive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeActive_Click);
            // 
            // tsmiCurrentAutoEscapeInactive
            // 
            this.tsmiCurrentAutoEscapeInactive.Name = "tsmiCurrentAutoEscapeInactive";
            this.tsmiCurrentAutoEscapeInactive.Size = new System.Drawing.Size(233, 24);
            this.tsmiCurrentAutoEscapeInactive.Text = "Inactive";
            this.tsmiCurrentAutoEscapeInactive.Click += new System.EventHandler(this.tsmiCurrentAutoEscapeInactive_Click);
            // 
            // chkQueryMonsterStatus
            // 
            this.chkQueryMonsterStatus.AutoSize = true;
            this.chkQueryMonsterStatus.Location = new System.Drawing.Point(477, 34);
            this.chkQueryMonsterStatus.Margin = new System.Windows.Forms.Padding(4);
            this.chkQueryMonsterStatus.Name = "chkQueryMonsterStatus";
            this.chkQueryMonsterStatus.Size = new System.Drawing.Size(161, 20);
            this.chkQueryMonsterStatus.TabIndex = 135;
            this.chkQueryMonsterStatus.Text = "Query monster status?";
            this.chkQueryMonsterStatus.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(872, 14);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(97, 27);
            this.btnOK.TabIndex = 137;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(977, 14);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(97, 27);
            this.btnCancel.TabIndex = 138;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblPreferredAlignmentValue
            // 
            this.lblPreferredAlignmentValue.BackColor = System.Drawing.Color.Blue;
            this.lblPreferredAlignmentValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblPreferredAlignmentValue.ForeColor = System.Drawing.Color.White;
            this.lblPreferredAlignmentValue.Location = new System.Drawing.Point(189, 161);
            this.lblPreferredAlignmentValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreferredAlignmentValue.Name = "lblPreferredAlignmentValue";
            this.lblPreferredAlignmentValue.Size = new System.Drawing.Size(188, 18);
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
            this.ctxPreferredAlignment.Size = new System.Drawing.Size(116, 52);
            this.ctxPreferredAlignment.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPreferredAlignment_Opening);
            // 
            // tsmiPreferredAlignmentGood
            // 
            this.tsmiPreferredAlignmentGood.Name = "tsmiPreferredAlignmentGood";
            this.tsmiPreferredAlignmentGood.Size = new System.Drawing.Size(115, 24);
            this.tsmiPreferredAlignmentGood.Text = "Good";
            this.tsmiPreferredAlignmentGood.Click += new System.EventHandler(this.tsmiPreferredAlignmentGood_Click);
            // 
            // tsmiPreferredAlignmentEvil
            // 
            this.tsmiPreferredAlignmentEvil.Name = "tsmiPreferredAlignmentEvil";
            this.tsmiPreferredAlignmentEvil.Size = new System.Drawing.Size(115, 24);
            this.tsmiPreferredAlignmentEvil.Text = "Evil";
            this.tsmiPreferredAlignmentEvil.Click += new System.EventHandler(this.tsmiPreferredAlignmentEvil_Click);
            // 
            // lblCurrentAutoSpellLevelsValue
            // 
            this.lblCurrentAutoSpellLevelsValue.BackColor = System.Drawing.Color.Silver;
            this.lblCurrentAutoSpellLevelsValue.ForeColor = System.Drawing.Color.Black;
            this.lblCurrentAutoSpellLevelsValue.Location = new System.Drawing.Point(156, 65);
            this.lblCurrentAutoSpellLevelsValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentAutoSpellLevelsValue.Name = "lblCurrentAutoSpellLevelsValue";
            this.lblCurrentAutoSpellLevelsValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentAutoSpellLevelsValue.TabIndex = 144;
            this.lblCurrentAutoSpellLevelsValue.Text = "Min:Max";
            this.lblCurrentAutoSpellLevelsValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentRealmValue
            // 
            this.lblCurrentRealmValue.BackColor = System.Drawing.Color.White;
            this.lblCurrentRealmValue.Location = new System.Drawing.Point(156, 10);
            this.lblCurrentRealmValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentRealmValue.Name = "lblCurrentRealmValue";
            this.lblCurrentRealmValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentRealmValue.TabIndex = 141;
            this.lblCurrentRealmValue.Text = "Realm";
            this.lblCurrentRealmValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCurrentAutoEscapeValue
            // 
            this.lblCurrentAutoEscapeValue.BackColor = System.Drawing.Color.Black;
            this.lblCurrentAutoEscapeValue.ContextMenuStrip = this.ctxAutoEscape;
            this.lblCurrentAutoEscapeValue.ForeColor = System.Drawing.Color.White;
            this.lblCurrentAutoEscapeValue.Location = new System.Drawing.Point(156, 37);
            this.lblCurrentAutoEscapeValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentAutoEscapeValue.Name = "lblCurrentAutoEscapeValue";
            this.lblCurrentAutoEscapeValue.Size = new System.Drawing.Size(221, 18);
            this.lblCurrentAutoEscapeValue.TabIndex = 143;
            this.lblCurrentAutoEscapeValue.Text = "Auto Escape";
            this.lblCurrentAutoEscapeValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColorValue
            // 
            this.lblEmptyColorValue.BackColor = System.Drawing.Color.Red;
            this.lblEmptyColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblEmptyColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblEmptyColorValue.Location = new System.Drawing.Point(156, 129);
            this.lblEmptyColorValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmptyColorValue.Name = "lblEmptyColorValue";
            this.lblEmptyColorValue.Size = new System.Drawing.Size(221, 18);
            this.lblEmptyColorValue.TabIndex = 143;
            this.lblEmptyColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyColor
            // 
            this.lblEmptyColor.AutoSize = true;
            this.lblEmptyColor.Location = new System.Drawing.Point(23, 129);
            this.lblEmptyColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmptyColor.Name = "lblEmptyColor";
            this.lblEmptyColor.Size = new System.Drawing.Size(81, 16);
            this.lblEmptyColor.TabIndex = 142;
            this.lblEmptyColor.Text = "Empty color:";
            // 
            // lblFullColorValue
            // 
            this.lblFullColorValue.BackColor = System.Drawing.Color.Green;
            this.lblFullColorValue.ContextMenuStrip = this.ctxPreferredAlignment;
            this.lblFullColorValue.ForeColor = System.Drawing.Color.Black;
            this.lblFullColorValue.Location = new System.Drawing.Point(156, 97);
            this.lblFullColorValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFullColorValue.Name = "lblFullColorValue";
            this.lblFullColorValue.Size = new System.Drawing.Size(221, 18);
            this.lblFullColorValue.TabIndex = 141;
            this.lblFullColorValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFullColor
            // 
            this.lblFullColor.AutoSize = true;
            this.lblFullColor.Location = new System.Drawing.Point(23, 100);
            this.lblFullColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFullColor.Name = "lblFullColor";
            this.lblFullColor.Size = new System.Drawing.Size(64, 16);
            this.lblFullColor.TabIndex = 140;
            this.lblFullColor.Text = "Full color:";
            // 
            // lstStrategies
            // 
            this.lstStrategies.ContextMenuStrip = this.ctxListModification;
            this.lstStrategies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstStrategies.FormattingEnabled = true;
            this.lstStrategies.ItemHeight = 16;
            this.lstStrategies.Location = new System.Drawing.Point(3, 2);
            this.lstStrategies.Margin = new System.Windows.Forms.Padding(4);
            this.lstStrategies.Name = "lstStrategies";
            this.lstStrategies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstStrategies.Size = new System.Drawing.Size(1085, 676);
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
            this.ctxListModification.Size = new System.Drawing.Size(159, 124);
            this.ctxListModification.Opening += new System.ComponentModel.CancelEventHandler(this.ctxListModification_Opening);
            // 
            // tsmiAddEntry
            // 
            this.tsmiAddEntry.Name = "tsmiAddEntry";
            this.tsmiAddEntry.Size = new System.Drawing.Size(158, 24);
            this.tsmiAddEntry.Text = "Add";
            this.tsmiAddEntry.Click += new System.EventHandler(this.tsmiAddEntry_Click);
            // 
            // tsmiEditEntry
            // 
            this.tsmiEditEntry.Name = "tsmiEditEntry";
            this.tsmiEditEntry.Size = new System.Drawing.Size(158, 24);
            this.tsmiEditEntry.Text = "Edit";
            this.tsmiEditEntry.Click += new System.EventHandler(this.tsmiEditEntry_Click);
            // 
            // tsmiRemoveEntry
            // 
            this.tsmiRemoveEntry.Name = "tsmiRemoveEntry";
            this.tsmiRemoveEntry.Size = new System.Drawing.Size(158, 24);
            this.tsmiRemoveEntry.Text = "Remove";
            this.tsmiRemoveEntry.Click += new System.EventHandler(this.tsmiRemoveEntry_Click);
            // 
            // tsmiMoveEntryUp
            // 
            this.tsmiMoveEntryUp.Name = "tsmiMoveEntryUp";
            this.tsmiMoveEntryUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveEntryUp.Text = "Move Up";
            this.tsmiMoveEntryUp.Click += new System.EventHandler(this.tsmiMoveEntryUp_Click);
            // 
            // tsmiMoveEntryDown
            // 
            this.tsmiMoveEntryDown.Name = "tsmiMoveEntryDown";
            this.tsmiMoveEntryDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveEntryDown.Text = "Move Down";
            this.tsmiMoveEntryDown.Click += new System.EventHandler(this.tsmiMoveEntryDown_Click);
            // 
            // chkRemoveAllOnStartup
            // 
            this.chkRemoveAllOnStartup.AutoSize = true;
            this.chkRemoveAllOnStartup.Location = new System.Drawing.Point(477, 65);
            this.chkRemoveAllOnStartup.Margin = new System.Windows.Forms.Padding(4);
            this.chkRemoveAllOnStartup.Name = "chkRemoveAllOnStartup";
            this.chkRemoveAllOnStartup.Size = new System.Drawing.Size(166, 20);
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
            this.tcConfiguration.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tcConfiguration.Name = "tcConfiguration";
            this.tcConfiguration.SelectedIndex = 0;
            this.tcConfiguration.Size = new System.Drawing.Size(1099, 709);
            this.tcConfiguration.TabIndex = 147;
            // 
            // tabConfiguration
            // 
            this.tabConfiguration.Controls.Add(this.pnlSettings);
            this.tabConfiguration.Location = new System.Drawing.Point(4, 25);
            this.tabConfiguration.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabConfiguration.Size = new System.Drawing.Size(1091, 680);
            this.tabConfiguration.TabIndex = 0;
            this.tabConfiguration.Text = "Settings";
            this.tabConfiguration.UseVisualStyleBackColor = true;
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.txtCommandTimeoutSeconds);
            this.pnlSettings.Controls.Add(this.lblCommandTimeoutSeconds);
            this.pnlSettings.Controls.Add(this.lblAutoSpellLevels);
            this.pnlSettings.Controls.Add(this.lblAutoEscape);
            this.pnlSettings.Controls.Add(this.lblRealm);
            this.pnlSettings.Controls.Add(this.cboConsoleVerbosity);
            this.pnlSettings.Controls.Add(this.lblConsoleVerbosity);
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
            this.pnlSettings.Location = new System.Drawing.Point(3, 2);
            this.pnlSettings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(1085, 676);
            this.pnlSettings.TabIndex = 150;
            // 
            // lblAutoSpellLevels
            // 
            this.lblAutoSpellLevels.AutoSize = true;
            this.lblAutoSpellLevels.Location = new System.Drawing.Point(23, 67);
            this.lblAutoSpellLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoSpellLevels.Name = "lblAutoSpellLevels";
            this.lblAutoSpellLevels.Size = new System.Drawing.Size(108, 16);
            this.lblAutoSpellLevels.TabIndex = 163;
            this.lblAutoSpellLevels.Text = "Auto spell levels:";
            // 
            // lblAutoEscape
            // 
            this.lblAutoEscape.AutoSize = true;
            this.lblAutoEscape.Location = new System.Drawing.Point(25, 39);
            this.lblAutoEscape.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAutoEscape.Name = "lblAutoEscape";
            this.lblAutoEscape.Size = new System.Drawing.Size(86, 16);
            this.lblAutoEscape.TabIndex = 162;
            this.lblAutoEscape.Text = "Auto escape:";
            // 
            // lblRealm
            // 
            this.lblRealm.AutoSize = true;
            this.lblRealm.Location = new System.Drawing.Point(25, 12);
            this.lblRealm.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRealm.Name = "lblRealm";
            this.lblRealm.Size = new System.Drawing.Size(50, 16);
            this.lblRealm.TabIndex = 161;
            this.lblRealm.Text = "Realm:";
            // 
            // cboConsoleVerbosity
            // 
            this.cboConsoleVerbosity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConsoleVerbosity.FormattingEnabled = true;
            this.cboConsoleVerbosity.Items.AddRange(new object[] {
            "Minimum",
            "Default",
            "Maximum"});
            this.cboConsoleVerbosity.Location = new System.Drawing.Point(262, 354);
            this.cboConsoleVerbosity.Margin = new System.Windows.Forms.Padding(4);
            this.cboConsoleVerbosity.Name = "cboConsoleVerbosity";
            this.cboConsoleVerbosity.Size = new System.Drawing.Size(115, 24);
            this.cboConsoleVerbosity.TabIndex = 160;
            // 
            // lblConsoleVerbosity
            // 
            this.lblConsoleVerbosity.AutoSize = true;
            this.lblConsoleVerbosity.Location = new System.Drawing.Point(26, 358);
            this.lblConsoleVerbosity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConsoleVerbosity.Name = "lblConsoleVerbosity";
            this.lblConsoleVerbosity.Size = new System.Drawing.Size(118, 16);
            this.lblConsoleVerbosity.TabIndex = 159;
            this.lblConsoleVerbosity.Text = "Console verbosity:";
            // 
            // chkSaveSettingsOnQuit
            // 
            this.chkSaveSettingsOnQuit.AutoSize = true;
            this.chkSaveSettingsOnQuit.Location = new System.Drawing.Point(477, 124);
            this.chkSaveSettingsOnQuit.Margin = new System.Windows.Forms.Padding(4);
            this.chkSaveSettingsOnQuit.Name = "chkSaveSettingsOnQuit";
            this.chkSaveSettingsOnQuit.Size = new System.Drawing.Size(159, 20);
            this.chkSaveSettingsOnQuit.TabIndex = 157;
            this.chkSaveSettingsOnQuit.Text = "Save settings on quit?";
            this.chkSaveSettingsOnQuit.UseVisualStyleBackColor = true;
            // 
            // txtMagicMendWhenDownXHP
            // 
            this.txtMagicMendWhenDownXHP.Location = new System.Drawing.Point(261, 230);
            this.txtMagicMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicMendWhenDownXHP.Name = "txtMagicMendWhenDownXHP";
            this.txtMagicMendWhenDownXHP.Size = new System.Drawing.Size(115, 22);
            this.txtMagicMendWhenDownXHP.TabIndex = 152;
            // 
            // chkDisplayStunLength
            // 
            this.chkDisplayStunLength.AutoSize = true;
            this.chkDisplayStunLength.Location = new System.Drawing.Point(477, 94);
            this.chkDisplayStunLength.Margin = new System.Windows.Forms.Padding(4);
            this.chkDisplayStunLength.Name = "chkDisplayStunLength";
            this.chkDisplayStunLength.Size = new System.Drawing.Size(148, 20);
            this.chkDisplayStunLength.TabIndex = 149;
            this.chkDisplayStunLength.Text = "Display stun length?";
            this.chkDisplayStunLength.UseVisualStyleBackColor = true;
            // 
            // lblMagicMendWhenDownXHP
            // 
            this.lblMagicMendWhenDownXHP.AutoSize = true;
            this.lblMagicMendWhenDownXHP.Location = new System.Drawing.Point(25, 234);
            this.lblMagicMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicMendWhenDownXHP.Name = "lblMagicMendWhenDownXHP";
            this.lblMagicMendWhenDownXHP.Size = new System.Drawing.Size(189, 16);
            this.lblMagicMendWhenDownXHP.TabIndex = 151;
            this.lblMagicMendWhenDownXHP.Text = "Magic: Mend when down X HP:";
            // 
            // btnSelectEmptyColor
            // 
            this.btnSelectEmptyColor.Location = new System.Drawing.Point(385, 123);
            this.btnSelectEmptyColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectEmptyColor.Name = "btnSelectEmptyColor";
            this.btnSelectEmptyColor.Size = new System.Drawing.Size(73, 28);
            this.btnSelectEmptyColor.TabIndex = 148;
            this.btnSelectEmptyColor.Text = "Select";
            this.btnSelectEmptyColor.UseVisualStyleBackColor = true;
            this.btnSelectEmptyColor.Click += new System.EventHandler(this.btnSelectEmptyColor_Click);
            // 
            // txtMagicVigorWhenDownXHP
            // 
            this.txtMagicVigorWhenDownXHP.Location = new System.Drawing.Point(261, 199);
            this.txtMagicVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtMagicVigorWhenDownXHP.Name = "txtMagicVigorWhenDownXHP";
            this.txtMagicVigorWhenDownXHP.Size = new System.Drawing.Size(115, 22);
            this.txtMagicVigorWhenDownXHP.TabIndex = 150;
            // 
            // lblMagicVigorWhenDownXHP
            // 
            this.lblMagicVigorWhenDownXHP.AutoSize = true;
            this.lblMagicVigorWhenDownXHP.Location = new System.Drawing.Point(25, 204);
            this.lblMagicVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMagicVigorWhenDownXHP.Name = "lblMagicVigorWhenDownXHP";
            this.lblMagicVigorWhenDownXHP.Size = new System.Drawing.Size(187, 16);
            this.lblMagicVigorWhenDownXHP.TabIndex = 149;
            this.lblMagicVigorWhenDownXHP.Text = "Magic: Vigor when down X HP:";
            // 
            // btnSelectFullColor
            // 
            this.btnSelectFullColor.Location = new System.Drawing.Point(385, 94);
            this.btnSelectFullColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectFullColor.Name = "btnSelectFullColor";
            this.btnSelectFullColor.Size = new System.Drawing.Size(73, 28);
            this.btnSelectFullColor.TabIndex = 147;
            this.btnSelectFullColor.Text = "Select";
            this.btnSelectFullColor.UseVisualStyleBackColor = true;
            this.btnSelectFullColor.Click += new System.EventHandler(this.btnSelectFullColor_Click);
            // 
            // txtPotionsMendWhenDownXHP
            // 
            this.txtPotionsMendWhenDownXHP.Location = new System.Drawing.Point(261, 294);
            this.txtPotionsMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsMendWhenDownXHP.Name = "txtPotionsMendWhenDownXHP";
            this.txtPotionsMendWhenDownXHP.Size = new System.Drawing.Size(115, 22);
            this.txtPotionsMendWhenDownXHP.TabIndex = 156;
            // 
            // lblPotionsMendWhenDownXHP
            // 
            this.lblPotionsMendWhenDownXHP.AutoSize = true;
            this.lblPotionsMendWhenDownXHP.Location = new System.Drawing.Point(25, 298);
            this.lblPotionsMendWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsMendWhenDownXHP.Name = "lblPotionsMendWhenDownXHP";
            this.lblPotionsMendWhenDownXHP.Size = new System.Drawing.Size(197, 16);
            this.lblPotionsMendWhenDownXHP.TabIndex = 155;
            this.lblPotionsMendWhenDownXHP.Text = "Potions: Mend when down X HP:";
            // 
            // txtPotionsVigorWhenDownXHP
            // 
            this.txtPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(261, 263);
            this.txtPotionsVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4);
            this.txtPotionsVigorWhenDownXHP.Name = "txtPotionsVigorWhenDownXHP";
            this.txtPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(115, 22);
            this.txtPotionsVigorWhenDownXHP.TabIndex = 154;
            // 
            // lblPotionsVigorWhenDownXHP
            // 
            this.lblPotionsVigorWhenDownXHP.AutoSize = true;
            this.lblPotionsVigorWhenDownXHP.Location = new System.Drawing.Point(25, 267);
            this.lblPotionsVigorWhenDownXHP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPotionsVigorWhenDownXHP.Name = "lblPotionsVigorWhenDownXHP";
            this.lblPotionsVigorWhenDownXHP.Size = new System.Drawing.Size(195, 16);
            this.lblPotionsVigorWhenDownXHP.TabIndex = 153;
            this.lblPotionsVigorWhenDownXHP.Text = "Potions: Vigor when down X HP:";
            // 
            // tabStrategies
            // 
            this.tabStrategies.Controls.Add(this.lstStrategies);
            this.tabStrategies.Location = new System.Drawing.Point(4, 25);
            this.tabStrategies.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabStrategies.Name = "tabStrategies";
            this.tabStrategies.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabStrategies.Size = new System.Drawing.Size(1091, 680);
            this.tabStrategies.TabIndex = 1;
            this.tabStrategies.Text = "Strategies";
            this.tabStrategies.UseVisualStyleBackColor = true;
            // 
            // tabDynamicItemData
            // 
            this.tabDynamicItemData.Controls.Add(this.pnlMiddle);
            this.tabDynamicItemData.Controls.Add(this.pnlItemsTop);
            this.tabDynamicItemData.Location = new System.Drawing.Point(4, 25);
            this.tabDynamicItemData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabDynamicItemData.Name = "tabDynamicItemData";
            this.tabDynamicItemData.Size = new System.Drawing.Size(1091, 680);
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
            this.pnlMiddle.Location = new System.Drawing.Point(-5, 94);
            this.pnlMiddle.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(1093, 528);
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
            this.lvItems.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lvItems.Name = "lvItems";
            this.lvItems.Size = new System.Drawing.Size(1093, 528);
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
            this.colSinkCount.Text = "Sink #";
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
            this.pnlItemsTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlItemsTop.Name = "pnlItemsTop";
            this.pnlItemsTop.Size = new System.Drawing.Size(1091, 87);
            this.pnlItemsTop.TabIndex = 0;
            // 
            // btnClearHeldItem
            // 
            this.btnClearHeldItem.Location = new System.Drawing.Point(327, 44);
            this.btnClearHeldItem.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearHeldItem.Name = "btnClearHeldItem";
            this.btnClearHeldItem.Size = new System.Drawing.Size(32, 25);
            this.btnClearHeldItem.TabIndex = 148;
            this.btnClearHeldItem.Text = "X";
            this.btnClearHeldItem.UseVisualStyleBackColor = true;
            this.btnClearHeldItem.Click += new System.EventHandler(this.btnClearHeldItem_Click);
            // 
            // btnClearWeapon
            // 
            this.btnClearWeapon.Location = new System.Drawing.Point(327, 15);
            this.btnClearWeapon.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearWeapon.Name = "btnClearWeapon";
            this.btnClearWeapon.Size = new System.Drawing.Size(32, 25);
            this.btnClearWeapon.TabIndex = 147;
            this.btnClearWeapon.Text = "X";
            this.btnClearWeapon.UseVisualStyleBackColor = true;
            this.btnClearWeapon.Click += new System.EventHandler(this.btnClearWeapon_Click);
            // 
            // txtHeldItem
            // 
            this.txtHeldItem.Enabled = false;
            this.txtHeldItem.Location = new System.Drawing.Point(96, 44);
            this.txtHeldItem.Margin = new System.Windows.Forms.Padding(4);
            this.txtHeldItem.Name = "txtHeldItem";
            this.txtHeldItem.Size = new System.Drawing.Size(221, 22);
            this.txtHeldItem.TabIndex = 146;
            // 
            // lblHeldItem
            // 
            this.lblHeldItem.AutoSize = true;
            this.lblHeldItem.Location = new System.Drawing.Point(13, 48);
            this.lblHeldItem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeldItem.Name = "lblHeldItem";
            this.lblHeldItem.Size = new System.Drawing.Size(39, 16);
            this.lblHeldItem.TabIndex = 145;
            this.lblHeldItem.Text = "Held:";
            // 
            // txtCurrentWeaponValue
            // 
            this.txtCurrentWeaponValue.Enabled = false;
            this.txtCurrentWeaponValue.Location = new System.Drawing.Point(96, 15);
            this.txtCurrentWeaponValue.Margin = new System.Windows.Forms.Padding(4);
            this.txtCurrentWeaponValue.Name = "txtCurrentWeaponValue";
            this.txtCurrentWeaponValue.Size = new System.Drawing.Size(221, 22);
            this.txtCurrentWeaponValue.TabIndex = 144;
            // 
            // lblWeapon
            // 
            this.lblWeapon.AutoSize = true;
            this.lblWeapon.Location = new System.Drawing.Point(13, 18);
            this.lblWeapon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeapon.Name = "lblWeapon";
            this.lblWeapon.Size = new System.Drawing.Size(62, 16);
            this.lblWeapon.TabIndex = 143;
            this.lblWeapon.Text = "Weapon:";
            // 
            // btnSink
            // 
            this.btnSink.Location = new System.Drawing.Point(485, 12);
            this.btnSink.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSink.Name = "btnSink";
            this.btnSink.Size = new System.Drawing.Size(75, 26);
            this.btnSink.TabIndex = 4;
            this.btnSink.Text = "Sink";
            this.btnSink.UseVisualStyleBackColor = true;
            this.btnSink.Click += new System.EventHandler(this.btnSink_Click);
            // 
            // btnSellOrJunk
            // 
            this.btnSellOrJunk.Location = new System.Drawing.Point(487, 43);
            this.btnSellOrJunk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSellOrJunk.Name = "btnSellOrJunk";
            this.btnSellOrJunk.Size = new System.Drawing.Size(95, 26);
            this.btnSellOrJunk.TabIndex = 3;
            this.btnSellOrJunk.Text = "Sell/Junk";
            this.btnSellOrJunk.UseVisualStyleBackColor = true;
            this.btnSellOrJunk.Click += new System.EventHandler(this.btnSellOrJunk_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(407, 43);
            this.btnClear.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 26);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnKeep
            // 
            this.btnKeep.Location = new System.Drawing.Point(405, 12);
            this.btnKeep.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnKeep.Name = "btnKeep";
            this.btnKeep.Size = new System.Drawing.Size(75, 26);
            this.btnKeep.TabIndex = 1;
            this.btnKeep.Text = "Keep";
            this.btnKeep.UseVisualStyleBackColor = true;
            this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(587, 43);
            this.btnIgnore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 26);
            this.btnIgnore.TabIndex = 0;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // tabAreas
            // 
            this.tabAreas.Controls.Add(this.treeAreas);
            this.tabAreas.Location = new System.Drawing.Point(4, 25);
            this.tabAreas.Margin = new System.Windows.Forms.Padding(4);
            this.tabAreas.Name = "tabAreas";
            this.tabAreas.Size = new System.Drawing.Size(1091, 680);
            this.tabAreas.TabIndex = 3;
            this.tabAreas.Text = "Areas";
            this.tabAreas.UseVisualStyleBackColor = true;
            // 
            // treeAreas
            // 
            this.treeAreas.ContextMenuStrip = this.ctxAreas;
            this.treeAreas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeAreas.Location = new System.Drawing.Point(0, 0);
            this.treeAreas.Margin = new System.Windows.Forms.Padding(4);
            this.treeAreas.Name = "treeAreas";
            this.treeAreas.Size = new System.Drawing.Size(1091, 680);
            this.treeAreas.TabIndex = 0;
            this.treeAreas.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeAreas_BeforeCollapse);
            // 
            // ctxAreas
            // 
            this.ctxAreas.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxAreas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddChild,
            this.tsmiAddSiblingBefore,
            this.tsmiAddSiblingAfter,
            this.tsmiEdit,
            this.tsmiRemove,
            this.tsmiMoveUp,
            this.tsmiMoveDown});
            this.ctxAreas.Name = "ctxAreas";
            this.ctxAreas.Size = new System.Drawing.Size(205, 172);
            this.ctxAreas.Opening += new System.ComponentModel.CancelEventHandler(this.ctxAreas_Opening);
            this.ctxAreas.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxAreas_ItemClicked);
            // 
            // tsmiAddChild
            // 
            this.tsmiAddChild.Name = "tsmiAddChild";
            this.tsmiAddChild.Size = new System.Drawing.Size(204, 24);
            this.tsmiAddChild.Text = "Add Child";
            // 
            // tsmiAddSiblingBefore
            // 
            this.tsmiAddSiblingBefore.Name = "tsmiAddSiblingBefore";
            this.tsmiAddSiblingBefore.Size = new System.Drawing.Size(204, 24);
            this.tsmiAddSiblingBefore.Text = "Add Sibling Before";
            // 
            // tsmiAddSiblingAfter
            // 
            this.tsmiAddSiblingAfter.Name = "tsmiAddSiblingAfter";
            this.tsmiAddSiblingAfter.Size = new System.Drawing.Size(204, 24);
            this.tsmiAddSiblingAfter.Text = "Add Sibling After";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(204, 24);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiRemove
            // 
            this.tsmiRemove.Name = "tsmiRemove";
            this.tsmiRemove.Size = new System.Drawing.Size(204, 24);
            this.tsmiRemove.Text = "Remove";
            // 
            // tsmiMoveUp
            // 
            this.tsmiMoveUp.Name = "tsmiMoveUp";
            this.tsmiMoveUp.Size = new System.Drawing.Size(204, 24);
            this.tsmiMoveUp.Text = "Move Up";
            // 
            // tsmiMoveDown
            // 
            this.tsmiMoveDown.Name = "tsmiMoveDown";
            this.tsmiMoveDown.Size = new System.Drawing.Size(204, 24);
            this.tsmiMoveDown.Text = "Move Down";
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnOK);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 655);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1099, 54);
            this.pnlBottom.TabIndex = 148;
            // 
            // txtCommandTimeoutSeconds
            // 
            this.txtCommandTimeoutSeconds.Location = new System.Drawing.Point(261, 324);
            this.txtCommandTimeoutSeconds.Margin = new System.Windows.Forms.Padding(4);
            this.txtCommandTimeoutSeconds.Name = "txtCommandTimeoutSeconds";
            this.txtCommandTimeoutSeconds.Size = new System.Drawing.Size(115, 22);
            this.txtCommandTimeoutSeconds.TabIndex = 165;
            // 
            // lblCommandTimeoutSeconds
            // 
            this.lblCommandTimeoutSeconds.AutoSize = true;
            this.lblCommandTimeoutSeconds.Location = new System.Drawing.Point(25, 328);
            this.lblCommandTimeoutSeconds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCommandTimeoutSeconds.Name = "lblCommandTimeoutSeconds";
            this.lblCommandTimeoutSeconds.Size = new System.Drawing.Size(216, 20);
            this.lblCommandTimeoutSeconds.TabIndex = 164;
            this.lblCommandTimeoutSeconds.Text = "Command timeout seconds:";
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 709);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.tcConfiguration);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
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
            this.ctxAreas.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblAutoSpellLevels;
        private System.Windows.Forms.Label lblAutoEscape;
        private System.Windows.Forms.Label lblRealm;
        private System.Windows.Forms.TextBox txtCommandTimeoutSeconds;
        private System.Windows.Forms.Label lblCommandTimeoutSeconds;
    }
}
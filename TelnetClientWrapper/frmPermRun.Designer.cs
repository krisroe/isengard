namespace IsengardClient
{
    partial class frmPermRun
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flpSkills = new System.Windows.Forms.FlowLayoutPanel();
            this.lblMob = new System.Windows.Forms.Label();
            this.cboMob = new System.Windows.Forms.ComboBox();
            this.cboTargetRoom = new System.Windows.Forms.ComboBox();
            this.lblTargetRoom = new System.Windows.Forms.Label();
            this.btnTargetGraph = new System.Windows.Forms.Button();
            this.btnTargetLocations = new System.Windows.Forms.Button();
            this.cboItemsToProcessType = new System.Windows.Forms.ComboBox();
            this.lblItemsToProcessType = new System.Windows.Forms.Label();
            this.lblSpellsCast = new System.Windows.Forms.Label();
            this.lblSkills = new System.Windows.Forms.Label();
            this.flpSpellsCast = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStrategy = new System.Windows.Forms.Label();
            this.cboStrategy = new System.Windows.Forms.ComboBox();
            this.flpSpellsPotions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSpellsPotions = new System.Windows.Forms.Label();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.btnThresholdLocations = new System.Windows.Forms.Button();
            this.btnThresholdGraph = new System.Windows.Forms.Button();
            this.cboThresholdRoom = new System.Windows.Forms.ComboBox();
            this.lblThresholdRoom = new System.Windows.Forms.Label();
            this.cboBeforeFull = new System.Windows.Forms.ComboBox();
            this.lblBeforeFull = new System.Windows.Forms.Label();
            this.cboAfterFull = new System.Windows.Forms.ComboBox();
            this.lblAfterFull = new System.Windows.Forms.Label();
            this.btnThresholdClear = new System.Windows.Forms.Button();
            this.lblArea = new System.Windows.Forms.Label();
            this.cboArea = new System.Windows.Forms.ComboBox();
            this.chkRehome = new System.Windows.Forms.CheckBox();
            this.btnEditAreas = new System.Windows.Forms.Button();
            this.txtArea = new System.Windows.Forms.TextBox();
            this.flpKeys = new System.Windows.Forms.FlowLayoutPanel();
            this.lblKeys = new System.Windows.Forms.Label();
            this.chkRemoveAllEquipment = new System.Windows.Forms.CheckBox();
            this.ucStrategyModifications1 = new IsengardClient.ucStrategyModifications();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(677, 686);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(140, 43);
            this.btnOK.TabIndex = 30;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(828, 686);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(140, 43);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // flpSkills
            // 
            this.flpSkills.Location = new System.Drawing.Point(121, 349);
            this.flpSkills.Margin = new System.Windows.Forms.Padding(4);
            this.flpSkills.Name = "flpSkills";
            this.flpSkills.Size = new System.Drawing.Size(846, 31);
            this.flpSkills.TabIndex = 11;
            // 
            // lblMob
            // 
            this.lblMob.AutoSize = true;
            this.lblMob.Location = new System.Drawing.Point(12, 492);
            this.lblMob.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMob.Name = "lblMob";
            this.lblMob.Size = new System.Drawing.Size(45, 19);
            this.lblMob.TabIndex = 21;
            this.lblMob.Text = "Mob:";
            // 
            // cboMob
            // 
            this.cboMob.FormattingEnabled = true;
            this.cboMob.Location = new System.Drawing.Point(121, 489);
            this.cboMob.Margin = new System.Windows.Forms.Padding(4);
            this.cboMob.Name = "cboMob";
            this.cboMob.Size = new System.Drawing.Size(528, 27);
            this.cboMob.TabIndex = 22;
            // 
            // cboTargetRoom
            // 
            this.cboTargetRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetRoom.FormattingEnabled = true;
            this.cboTargetRoom.Location = new System.Drawing.Point(121, 454);
            this.cboTargetRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboTargetRoom.Name = "cboTargetRoom";
            this.cboTargetRoom.Size = new System.Drawing.Size(528, 27);
            this.cboTargetRoom.TabIndex = 18;
            this.cboTargetRoom.SelectedIndexChanged += new System.EventHandler(this.cboRoom_SelectedIndexChanged);
            // 
            // lblTargetRoom
            // 
            this.lblTargetRoom.AutoSize = true;
            this.lblTargetRoom.Location = new System.Drawing.Point(12, 457);
            this.lblTargetRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTargetRoom.Name = "lblTargetRoom";
            this.lblTargetRoom.Size = new System.Drawing.Size(58, 19);
            this.lblTargetRoom.TabIndex = 17;
            this.lblTargetRoom.Text = "Target:";
            // 
            // btnTargetGraph
            // 
            this.btnTargetGraph.Location = new System.Drawing.Point(769, 454);
            this.btnTargetGraph.Name = "btnTargetGraph";
            this.btnTargetGraph.Size = new System.Drawing.Size(94, 27);
            this.btnTargetGraph.TabIndex = 20;
            this.btnTargetGraph.Text = "Graph";
            this.btnTargetGraph.UseVisualStyleBackColor = true;
            this.btnTargetGraph.Click += new System.EventHandler(this.btnTargetGraph_Click);
            // 
            // btnTargetLocations
            // 
            this.btnTargetLocations.Location = new System.Drawing.Point(669, 454);
            this.btnTargetLocations.Name = "btnTargetLocations";
            this.btnTargetLocations.Size = new System.Drawing.Size(94, 27);
            this.btnTargetLocations.TabIndex = 19;
            this.btnTargetLocations.Text = "Locations";
            this.btnTargetLocations.UseVisualStyleBackColor = true;
            this.btnTargetLocations.Click += new System.EventHandler(this.btnTargetLocations_Click);
            // 
            // cboItemsToProcessType
            // 
            this.cboItemsToProcessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboItemsToProcessType.FormattingEnabled = true;
            this.cboItemsToProcessType.Location = new System.Drawing.Point(121, 666);
            this.cboItemsToProcessType.Margin = new System.Windows.Forms.Padding(4);
            this.cboItemsToProcessType.Name = "cboItemsToProcessType";
            this.cboItemsToProcessType.Size = new System.Drawing.Size(528, 27);
            this.cboItemsToProcessType.TabIndex = 27;
            // 
            // lblItemsToProcessType
            // 
            this.lblItemsToProcessType.AutoSize = true;
            this.lblItemsToProcessType.Location = new System.Drawing.Point(2, 669);
            this.lblItemsToProcessType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblItemsToProcessType.Name = "lblItemsToProcessType";
            this.lblItemsToProcessType.Size = new System.Drawing.Size(108, 19);
            this.lblItemsToProcessType.TabIndex = 26;
            this.lblItemsToProcessType.Text = "Items source:";
            // 
            // lblSpellsCast
            // 
            this.lblSpellsCast.AutoSize = true;
            this.lblSpellsCast.Location = new System.Drawing.Point(13, 203);
            this.lblSpellsCast.Name = "lblSpellsCast";
            this.lblSpellsCast.Size = new System.Drawing.Size(104, 19);
            this.lblSpellsCast.TabIndex = 6;
            this.lblSpellsCast.Text = "Spells (cast):";
            // 
            // lblSkills
            // 
            this.lblSkills.AutoSize = true;
            this.lblSkills.Location = new System.Drawing.Point(16, 349);
            this.lblSkills.Name = "lblSkills";
            this.lblSkills.Size = new System.Drawing.Size(51, 19);
            this.lblSkills.TabIndex = 10;
            this.lblSkills.Text = "Skills:";
            // 
            // flpSpellsCast
            // 
            this.flpSpellsCast.Location = new System.Drawing.Point(122, 203);
            this.flpSpellsCast.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsCast.Name = "flpSpellsCast";
            this.flpSpellsCast.Size = new System.Drawing.Size(846, 65);
            this.flpSpellsCast.TabIndex = 7;
            // 
            // lblStrategy
            // 
            this.lblStrategy.AutoSize = true;
            this.lblStrategy.Location = new System.Drawing.Point(9, 526);
            this.lblStrategy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStrategy.Name = "lblStrategy";
            this.lblStrategy.Size = new System.Drawing.Size(75, 19);
            this.lblStrategy.TabIndex = 23;
            this.lblStrategy.Text = "Strategy:";
            // 
            // cboStrategy
            // 
            this.cboStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStrategy.FormattingEnabled = true;
            this.cboStrategy.Location = new System.Drawing.Point(121, 523);
            this.cboStrategy.Name = "cboStrategy";
            this.cboStrategy.Size = new System.Drawing.Size(528, 27);
            this.cboStrategy.TabIndex = 24;
            this.cboStrategy.SelectedIndexChanged += new System.EventHandler(this.cboStrategy_SelectedIndexChanged);
            // 
            // flpSpellsPotions
            // 
            this.flpSpellsPotions.Location = new System.Drawing.Point(122, 276);
            this.flpSpellsPotions.Margin = new System.Windows.Forms.Padding(4);
            this.flpSpellsPotions.Name = "flpSpellsPotions";
            this.flpSpellsPotions.Size = new System.Drawing.Size(846, 65);
            this.flpSpellsPotions.TabIndex = 9;
            // 
            // lblSpellsPotions
            // 
            this.lblSpellsPotions.AutoSize = true;
            this.lblSpellsPotions.Location = new System.Drawing.Point(12, 276);
            this.lblSpellsPotions.Name = "lblSpellsPotions";
            this.lblSpellsPotions.Size = new System.Drawing.Size(96, 19);
            this.lblSpellsPotions.TabIndex = 8;
            this.lblSpellsPotions.Text = "Spells (pot):";
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Location = new System.Drawing.Point(14, 12);
            this.lblDisplayName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(113, 19);
            this.lblDisplayName.TabIndex = 0;
            this.lblDisplayName.Text = "Display name:";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Location = new System.Drawing.Point(129, 9);
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(528, 27);
            this.txtDisplayName.TabIndex = 1;
            // 
            // btnThresholdLocations
            // 
            this.btnThresholdLocations.Location = new System.Drawing.Point(668, 419);
            this.btnThresholdLocations.Name = "btnThresholdLocations";
            this.btnThresholdLocations.Size = new System.Drawing.Size(95, 27);
            this.btnThresholdLocations.TabIndex = 14;
            this.btnThresholdLocations.Text = "Locations";
            this.btnThresholdLocations.UseVisualStyleBackColor = true;
            this.btnThresholdLocations.Click += new System.EventHandler(this.btnThresholdLocations_Click);
            // 
            // btnThresholdGraph
            // 
            this.btnThresholdGraph.Location = new System.Drawing.Point(769, 419);
            this.btnThresholdGraph.Name = "btnThresholdGraph";
            this.btnThresholdGraph.Size = new System.Drawing.Size(95, 27);
            this.btnThresholdGraph.TabIndex = 15;
            this.btnThresholdGraph.Text = "Graph";
            this.btnThresholdGraph.UseVisualStyleBackColor = true;
            this.btnThresholdGraph.Click += new System.EventHandler(this.btnThresholdGraph_Click);
            // 
            // cboThresholdRoom
            // 
            this.cboThresholdRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboThresholdRoom.FormattingEnabled = true;
            this.cboThresholdRoom.Location = new System.Drawing.Point(120, 419);
            this.cboThresholdRoom.Margin = new System.Windows.Forms.Padding(4);
            this.cboThresholdRoom.Name = "cboThresholdRoom";
            this.cboThresholdRoom.Size = new System.Drawing.Size(528, 27);
            this.cboThresholdRoom.TabIndex = 13;
            // 
            // lblThresholdRoom
            // 
            this.lblThresholdRoom.AutoSize = true;
            this.lblThresholdRoom.Location = new System.Drawing.Point(11, 422);
            this.lblThresholdRoom.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblThresholdRoom.Name = "lblThresholdRoom";
            this.lblThresholdRoom.Size = new System.Drawing.Size(85, 19);
            this.lblThresholdRoom.TabIndex = 12;
            this.lblThresholdRoom.Text = "Threshold:";
            // 
            // cboBeforeFull
            // 
            this.cboBeforeFull.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBeforeFull.FormattingEnabled = true;
            this.cboBeforeFull.Location = new System.Drawing.Point(129, 79);
            this.cboBeforeFull.Margin = new System.Windows.Forms.Padding(4);
            this.cboBeforeFull.Name = "cboBeforeFull";
            this.cboBeforeFull.Size = new System.Drawing.Size(528, 27);
            this.cboBeforeFull.TabIndex = 5;
            // 
            // lblBeforeFull
            // 
            this.lblBeforeFull.AutoSize = true;
            this.lblBeforeFull.Location = new System.Drawing.Point(13, 82);
            this.lblBeforeFull.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBeforeFull.Name = "lblBeforeFull";
            this.lblBeforeFull.Size = new System.Drawing.Size(88, 19);
            this.lblBeforeFull.TabIndex = 4;
            this.lblBeforeFull.Text = "Before full:";
            // 
            // cboAfterFull
            // 
            this.cboAfterFull.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAfterFull.FormattingEnabled = true;
            this.cboAfterFull.Location = new System.Drawing.Point(121, 701);
            this.cboAfterFull.Margin = new System.Windows.Forms.Padding(4);
            this.cboAfterFull.Name = "cboAfterFull";
            this.cboAfterFull.Size = new System.Drawing.Size(528, 27);
            this.cboAfterFull.TabIndex = 29;
            // 
            // lblAfterFull
            // 
            this.lblAfterFull.AutoSize = true;
            this.lblAfterFull.Location = new System.Drawing.Point(3, 704);
            this.lblAfterFull.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAfterFull.Name = "lblAfterFull";
            this.lblAfterFull.Size = new System.Drawing.Size(74, 19);
            this.lblAfterFull.TabIndex = 28;
            this.lblAfterFull.Text = "After full:";
            // 
            // btnThresholdClear
            // 
            this.btnThresholdClear.Location = new System.Drawing.Point(870, 419);
            this.btnThresholdClear.Name = "btnThresholdClear";
            this.btnThresholdClear.Size = new System.Drawing.Size(95, 27);
            this.btnThresholdClear.TabIndex = 16;
            this.btnThresholdClear.Text = "Clear";
            this.btnThresholdClear.UseVisualStyleBackColor = true;
            this.btnThresholdClear.Click += new System.EventHandler(this.btnThresholdClear_Click);
            // 
            // lblArea
            // 
            this.lblArea.AutoSize = true;
            this.lblArea.Location = new System.Drawing.Point(13, 47);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(49, 19);
            this.lblArea.TabIndex = 2;
            this.lblArea.Text = "Area:";
            // 
            // cboArea
            // 
            this.cboArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboArea.FormattingEnabled = true;
            this.cboArea.Location = new System.Drawing.Point(129, 44);
            this.cboArea.Name = "cboArea";
            this.cboArea.Size = new System.Drawing.Size(528, 27);
            this.cboArea.TabIndex = 3;
            this.cboArea.SelectedIndexChanged += new System.EventHandler(this.cboArea_SelectedIndexChanged);
            // 
            // chkRehome
            // 
            this.chkRehome.AutoSize = true;
            this.chkRehome.Location = new System.Drawing.Point(774, 47);
            this.chkRehome.Name = "chkRehome";
            this.chkRehome.Size = new System.Drawing.Size(91, 23);
            this.chkRehome.TabIndex = 32;
            this.chkRehome.Text = "Rehome";
            this.chkRehome.UseVisualStyleBackColor = true;
            // 
            // btnEditAreas
            // 
            this.btnEditAreas.Location = new System.Drawing.Point(670, 44);
            this.btnEditAreas.Name = "btnEditAreas";
            this.btnEditAreas.Size = new System.Drawing.Size(96, 27);
            this.btnEditAreas.TabIndex = 33;
            this.btnEditAreas.Text = "Edit";
            this.btnEditAreas.UseVisualStyleBackColor = true;
            this.btnEditAreas.Click += new System.EventHandler(this.btnEditAreas_Click);
            // 
            // txtArea
            // 
            this.txtArea.Enabled = false;
            this.txtArea.Location = new System.Drawing.Point(129, 44);
            this.txtArea.Name = "txtArea";
            this.txtArea.Size = new System.Drawing.Size(528, 27);
            this.txtArea.TabIndex = 34;
            // 
            // flpKeys
            // 
            this.flpKeys.Location = new System.Drawing.Point(121, 114);
            this.flpKeys.Margin = new System.Windows.Forms.Padding(4);
            this.flpKeys.Name = "flpKeys";
            this.flpKeys.Size = new System.Drawing.Size(847, 81);
            this.flpKeys.TabIndex = 36;
            // 
            // lblKeys
            // 
            this.lblKeys.AutoSize = true;
            this.lblKeys.Location = new System.Drawing.Point(14, 114);
            this.lblKeys.Name = "lblKeys";
            this.lblKeys.Size = new System.Drawing.Size(51, 19);
            this.lblKeys.TabIndex = 35;
            this.lblKeys.Text = "Keys:";
            // 
            // chkRemoveAllEquipment
            // 
            this.chkRemoveAllEquipment.AutoSize = true;
            this.chkRemoveAllEquipment.Location = new System.Drawing.Point(122, 387);
            this.chkRemoveAllEquipment.Name = "chkRemoveAllEquipment";
            this.chkRemoveAllEquipment.Size = new System.Drawing.Size(198, 23);
            this.chkRemoveAllEquipment.TabIndex = 37;
            this.chkRemoveAllEquipment.Text = "Remove all equipment?";
            this.chkRemoveAllEquipment.UseVisualStyleBackColor = true;
            // 
            // ucStrategyModifications1
            // 
            this.ucStrategyModifications1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucStrategyModifications1.Location = new System.Drawing.Point(6, 557);
            this.ucStrategyModifications1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucStrategyModifications1.Name = "ucStrategyModifications1";
            this.ucStrategyModifications1.Size = new System.Drawing.Size(973, 108);
            this.ucStrategyModifications1.TabIndex = 38;
            // 
            // frmPermRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 737);
            this.ControlBox = false;
            this.Controls.Add(this.ucStrategyModifications1);
            this.Controls.Add(this.chkRemoveAllEquipment);
            this.Controls.Add(this.flpKeys);
            this.Controls.Add(this.lblKeys);
            this.Controls.Add(this.txtArea);
            this.Controls.Add(this.btnEditAreas);
            this.Controls.Add(this.chkRehome);
            this.Controls.Add(this.cboArea);
            this.Controls.Add(this.lblArea);
            this.Controls.Add(this.btnThresholdClear);
            this.Controls.Add(this.cboAfterFull);
            this.Controls.Add(this.lblAfterFull);
            this.Controls.Add(this.cboBeforeFull);
            this.Controls.Add(this.lblBeforeFull);
            this.Controls.Add(this.btnThresholdLocations);
            this.Controls.Add(this.btnThresholdGraph);
            this.Controls.Add(this.cboThresholdRoom);
            this.Controls.Add(this.lblThresholdRoom);
            this.Controls.Add(this.flpSpellsPotions);
            this.Controls.Add(this.txtDisplayName);
            this.Controls.Add(this.lblDisplayName);
            this.Controls.Add(this.lblSpellsPotions);
            this.Controls.Add(this.cboStrategy);
            this.Controls.Add(this.lblStrategy);
            this.Controls.Add(this.flpSpellsCast);
            this.Controls.Add(this.lblSkills);
            this.Controls.Add(this.flpSkills);
            this.Controls.Add(this.lblSpellsCast);
            this.Controls.Add(this.cboItemsToProcessType);
            this.Controls.Add(this.lblItemsToProcessType);
            this.Controls.Add(this.btnTargetLocations);
            this.Controls.Add(this.btnTargetGraph);
            this.Controls.Add(this.cboTargetRoom);
            this.Controls.Add(this.lblTargetRoom);
            this.Controls.Add(this.cboMob);
            this.Controls.Add(this.lblMob);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPermRun";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Perm Run";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flpSkills;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblMob;
        private System.Windows.Forms.ComboBox cboMob;
        private System.Windows.Forms.ComboBox cboTargetRoom;
        private System.Windows.Forms.Label lblTargetRoom;
        private System.Windows.Forms.Button btnTargetGraph;
        private System.Windows.Forms.Button btnTargetLocations;
        private System.Windows.Forms.ComboBox cboItemsToProcessType;
        private System.Windows.Forms.Label lblItemsToProcessType;
        private System.Windows.Forms.Label lblSpellsCast;
        private System.Windows.Forms.Label lblSkills;
        private System.Windows.Forms.FlowLayoutPanel flpSpellsCast;
        private System.Windows.Forms.Label lblStrategy;
        private System.Windows.Forms.ComboBox cboStrategy;
        private System.Windows.Forms.FlowLayoutPanel flpSpellsPotions;
        private System.Windows.Forms.Label lblSpellsPotions;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Button btnThresholdLocations;
        private System.Windows.Forms.Button btnThresholdGraph;
        private System.Windows.Forms.ComboBox cboThresholdRoom;
        private System.Windows.Forms.Label lblThresholdRoom;
        private System.Windows.Forms.ComboBox cboBeforeFull;
        private System.Windows.Forms.Label lblBeforeFull;
        private System.Windows.Forms.ComboBox cboAfterFull;
        private System.Windows.Forms.Label lblAfterFull;
        private System.Windows.Forms.Button btnThresholdClear;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.ComboBox cboArea;
        private System.Windows.Forms.CheckBox chkRehome;
        private System.Windows.Forms.Button btnEditAreas;
        private System.Windows.Forms.TextBox txtArea;
        private System.Windows.Forms.FlowLayoutPanel flpKeys;
        private System.Windows.Forms.Label lblKeys;
        private System.Windows.Forms.CheckBox chkRemoveAllEquipment;
        private ucStrategyModifications ucStrategyModifications1;
    }
}
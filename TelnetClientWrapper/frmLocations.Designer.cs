namespace IsengardClient
{
    partial class frmLocations
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
            this.btnSet = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.treeLocations = new System.Windows.Forms.TreeView();
            this.ctxTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAdd = new System.Windows.Forms.Button();
            this.ctxTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSet
            // 
            this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSet.Enabled = false;
            this.btnSet.Location = new System.Drawing.Point(204, 680);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(75, 23);
            this.btnSet.TabIndex = 1;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(285, 680);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // treeLocations
            // 
            this.treeLocations.ContextMenuStrip = this.ctxTree;
            this.treeLocations.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeLocations.HideSelection = false;
            this.treeLocations.Location = new System.Drawing.Point(0, 0);
            this.treeLocations.Name = "treeLocations";
            this.treeLocations.Size = new System.Drawing.Size(372, 674);
            this.treeLocations.TabIndex = 64;
            this.treeLocations.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeLocations_AfterSelect);
            this.treeLocations.DoubleClick += new System.EventHandler(this.treeLocations_DoubleClick);
            // 
            // ctxTree
            // 
            this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddChild,
            this.tsmiAddSiblingBefore,
            this.tsmiAddSiblingAfter,
            this.tsmiEdit,
            this.tsmiRemove,
            this.tsmiMoveUp,
            this.tsmiMoveDown});
            this.ctxTree.Name = "ctxTree";
            this.ctxTree.Size = new System.Drawing.Size(173, 158);
            this.ctxTree.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTree_Opening);
            this.ctxTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxTree_ItemClicked);
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
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(12, 680);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 65;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // frmLocations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 715);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.treeLocations);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLocations";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Locations";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLocations_FormClosed);
            this.ctxTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TreeView treeLocations;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ContextMenuStrip ctxTree;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddChild;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingBefore;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingAfter;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveDown;
    }
}
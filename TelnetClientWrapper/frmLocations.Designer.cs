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
            this.treeLocations = new System.Windows.Forms.TreeView();
            this.ctxTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingBefore = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddSiblingAfter = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetAsCurrentLocation = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeLocations
            // 
            this.treeLocations.ContextMenuStrip = this.ctxTree;
            this.treeLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeLocations.HideSelection = false;
            this.treeLocations.Location = new System.Drawing.Point(0, 0);
            this.treeLocations.Name = "treeLocations";
            this.treeLocations.Size = new System.Drawing.Size(372, 715);
            this.treeLocations.TabIndex = 64;
            this.treeLocations.DoubleClick += new System.EventHandler(this.treeLocations_DoubleClick);
            // 
            // ctxTree
            // 
            this.ctxTree.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddChild,
            this.tsmiAddSiblingBefore,
            this.tsmiAddSiblingAfter,
            this.tsmiEdit,
            this.tsmiRemove,
            this.tsmiMoveUp,
            this.tsmiMoveDown,
            this.tsmiSetAsCurrentLocation});
            this.ctxTree.Name = "ctxTree";
            this.ctxTree.Size = new System.Drawing.Size(231, 196);
            this.ctxTree.Opening += new System.ComponentModel.CancelEventHandler(this.ctxTree_Opening);
            this.ctxTree.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctxTree_ItemClicked);
            // 
            // tsmiAddChild
            // 
            this.tsmiAddChild.Name = "tsmiAddChild";
            this.tsmiAddChild.Size = new System.Drawing.Size(230, 24);
            this.tsmiAddChild.Text = "Add Child";
            // 
            // tsmiAddSiblingBefore
            // 
            this.tsmiAddSiblingBefore.Name = "tsmiAddSiblingBefore";
            this.tsmiAddSiblingBefore.Size = new System.Drawing.Size(230, 24);
            this.tsmiAddSiblingBefore.Text = "Add Sibling Before";
            // 
            // tsmiAddSiblingAfter
            // 
            this.tsmiAddSiblingAfter.Name = "tsmiAddSiblingAfter";
            this.tsmiAddSiblingAfter.Size = new System.Drawing.Size(230, 24);
            this.tsmiAddSiblingAfter.Text = "Add Sibling After";
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(230, 24);
            this.tsmiEdit.Text = "Edit";
            // 
            // tsmiRemove
            // 
            this.tsmiRemove.Name = "tsmiRemove";
            this.tsmiRemove.Size = new System.Drawing.Size(230, 24);
            this.tsmiRemove.Text = "Remove";
            // 
            // tsmiMoveUp
            // 
            this.tsmiMoveUp.Name = "tsmiMoveUp";
            this.tsmiMoveUp.Size = new System.Drawing.Size(230, 24);
            this.tsmiMoveUp.Text = "Move Up";
            // 
            // tsmiMoveDown
            // 
            this.tsmiMoveDown.Name = "tsmiMoveDown";
            this.tsmiMoveDown.Size = new System.Drawing.Size(230, 24);
            this.tsmiMoveDown.Text = "Move Down";
            // 
            // tsmiSetAsCurrentLocation
            // 
            this.tsmiSetAsCurrentLocation.Name = "tsmiSetAsCurrentLocation";
            this.tsmiSetAsCurrentLocation.Size = new System.Drawing.Size(230, 24);
            this.tsmiSetAsCurrentLocation.Text = "Set as Current Location";
            // 
            // frmLocations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 715);
            this.Controls.Add(this.treeLocations);
            this.MinimizeBox = false;
            this.Name = "frmLocations";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Locations";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLocations_FormClosed);
            this.ctxTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TreeView treeLocations;
        private System.Windows.Forms.ContextMenuStrip ctxTree;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddChild;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingBefore;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddSiblingAfter;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetAsCurrentLocation;
    }
}
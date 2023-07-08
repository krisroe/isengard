namespace IsengardClient
{
    partial class frmPermRuns
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
            this.dgvPermRuns = new System.Windows.Forms.DataGridView();
            this.ctxPermRuns = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermRuns)).BeginInit();
            this.ctxPermRuns.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPermRuns
            // 
            this.dgvPermRuns.AllowUserToAddRows = false;
            this.dgvPermRuns.AllowUserToDeleteRows = false;
            this.dgvPermRuns.AllowUserToResizeRows = false;
            this.dgvPermRuns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPermRuns.ContextMenuStrip = this.ctxPermRuns;
            this.dgvPermRuns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPermRuns.Location = new System.Drawing.Point(0, 0);
            this.dgvPermRuns.Name = "dgvPermRuns";
            this.dgvPermRuns.ReadOnly = true;
            this.dgvPermRuns.RowHeadersVisible = false;
            this.dgvPermRuns.RowHeadersWidth = 51;
            this.dgvPermRuns.RowTemplate.Height = 25;
            this.dgvPermRuns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPermRuns.Size = new System.Drawing.Size(1505, 834);
            this.dgvPermRuns.TabIndex = 0;
            this.dgvPermRuns.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPermRuns_CellContentClick);
            // 
            // ctxPermRuns
            // 
            this.ctxPermRuns.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxPermRuns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdd,
            this.tsmiRemove,
            this.tsmiMoveUp,
            this.tsmiMoveDown});
            this.ctxPermRuns.Name = "ctxPermRuns";
            this.ctxPermRuns.Size = new System.Drawing.Size(159, 100);
            this.ctxPermRuns.Opening += new System.ComponentModel.CancelEventHandler(this.ctxPermRuns_Opening);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(158, 24);
            this.tsmiAdd.Text = "Add";
            this.tsmiAdd.Click += new System.EventHandler(this.tsmiAdd_Click);
            // 
            // tsmiRemove
            // 
            this.tsmiRemove.Name = "tsmiRemove";
            this.tsmiRemove.Size = new System.Drawing.Size(158, 24);
            this.tsmiRemove.Text = "Remove";
            this.tsmiRemove.Click += new System.EventHandler(this.tsmiRemove_Click);
            // 
            // tsmiMoveUp
            // 
            this.tsmiMoveUp.Name = "tsmiMoveUp";
            this.tsmiMoveUp.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveUp.Text = "Move Up";
            this.tsmiMoveUp.Click += new System.EventHandler(this.tsmiMoveUp_Click);
            // 
            // tsmiMoveDown
            // 
            this.tsmiMoveDown.Name = "tsmiMoveDown";
            this.tsmiMoveDown.Size = new System.Drawing.Size(158, 24);
            this.tsmiMoveDown.Text = "Move Down";
            this.tsmiMoveDown.Click += new System.EventHandler(this.tsmiMoveDown_Click);
            // 
            // frmPermRuns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1505, 834);
            this.Controls.Add(this.dgvPermRuns);
            this.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmPermRuns";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Perm Runs";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmPermRuns_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPermRuns)).EndInit();
            this.ctxPermRuns.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPermRuns;
        private System.Windows.Forms.ContextMenuStrip ctxPermRuns;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdd;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveUp;
        private System.Windows.Forms.ToolStripMenuItem tsmiMoveDown;
    }
}
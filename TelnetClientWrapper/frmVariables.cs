using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IsengardClient
{
    public partial class frmVariables : Form
    {
        private List<Variable> _variables;
        private Control[] _controls;

        public frmVariables(List<Variable> variables)
        {
            InitializeComponent();

            _variables = variables;
            _controls = new Control[variables.Count];

            int rowHeight = 30;
            int controlHeight = 20;
            int topBottomPadding = (rowHeight - controlHeight) / 2;
            this.tlpVariables.RowCount = variables.Count;
            for (int i = 0; i < variables.Count; i++)
            {
                Variable v = variables[i];
                tlpVariables.RowStyles.Add(new RowStyle(SizeType.Absolute, (float)rowHeight));
                if (v.Type == VariableType.Bool)
                {
                    CheckBox chk = new CheckBox();
                    chk.AutoSize = true;
                    chk.Text = v.Name;
                    chk.Checked = ((BooleanVariable)v).Value;
                    tlpVariables.Controls.Add(chk, 1, i);
                    _controls[i] = chk;
                }
                else
                {
                    Label lbl = new Label();
                    lbl.Dock = DockStyle.Fill;
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.AutoSize = true;
                    lbl.Text = v.Name;
                    tlpVariables.Controls.Add(lbl, 0, i);
                }
                if (v.Type == VariableType.Int)
                {
                    IntegerVariable iv = (IntegerVariable)v;
                    NumericUpDown num = new NumericUpDown();
                    num.Minimum = iv.Min.GetValueOrDefault(int.MinValue);
                    num.Maximum = iv.Max.GetValueOrDefault(int.MaxValue);
                    num.Height = controlHeight;
                    num.Margin = new Padding(0, topBottomPadding, 0, topBottomPadding);
                    num.Width = 50;
                    num.Value = ((IntegerVariable)v).Value;
                    tlpVariables.Controls.Add(num, 1, i);
                    _controls[i] = num;
                }
                else if (v.Type == VariableType.String)
                {
                    TextBox txt = new TextBox();
                    txt.Height = controlHeight;
                    txt.Margin = new Padding(0, topBottomPadding, 0, topBottomPadding);
                    txt.Width = 200;
                    txt.Text = ((StringVariable)v).Value;
                    tlpVariables.Controls.Add(txt, 1, i);
                    _controls[i] = txt;
                }
            }
            tlpVariables.RowStyles.Add(new RowStyle(SizeType.Absolute, 1F));
            Label remaining = new Label();
            remaining.Dock = DockStyle.Fill;
            tlpVariables.Controls.Add(remaining, 0, variables.Count);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _variables.Count; i++)
            {
                Variable v = _variables[i];
                Control ctl = _controls[i];
                if (v.Type == VariableType.Bool)
                {
                    ((BooleanVariable)v).Value = ((CheckBox)ctl).Checked;
                }
                else if (v.Type == VariableType.Int)
                {
                    ((IntegerVariable)v).Value = Convert.ToInt32(((NumericUpDown)ctl).Value);
                }
                else if (v.Type == VariableType.String)
                {
                    ((StringVariable)v).Value = ((TextBox)ctl).Text;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

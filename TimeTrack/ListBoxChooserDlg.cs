using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeTrack
{
    public partial class ListBoxChooserDlg : Form
    {
        public string SelectedItem { get; set; }

        public string InstructionText
        {
            set { this.InstructionsLabel.Text = value; }
        }
        public void LoadListboxItems(List<string> items)
        {
            foreach (var i in items) listBox1.Items.Add(i);
            listBox1.Items.Add("All");

        }
        public ListBoxChooserDlg()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            SelectedItem = (string) listBox1.SelectedItem;
            this.Close();
        }
    }
}

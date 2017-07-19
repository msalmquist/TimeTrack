using System;
using System.Windows.Forms;

namespace TimeTrack
{
    public partial class DlgGenericTextEntry : Form
    {
        public string TextEntry = "";
        public string Title = "Generic Entry Form";
        public DlgGenericTextEntry()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxEntry.Text))
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                TextEntry = textBoxEntry.Text;
                DialogResult = DialogResult.OK;
            }
            
            Close();
        }

        private void DlgGenericTextEntry_Load(object sender, EventArgs e)
        {
            this.Text = Title;
        }
    }
}

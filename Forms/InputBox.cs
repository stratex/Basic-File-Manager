using System.Windows.Forms;

namespace Simple_Filemanager
{
    public partial class frmInput : Form
    {
        private frmMain _obj;
        private FileAction _action;
        public frmInput(frmMain frmObject, string title, string msg, FileAction action)
        {
            InitializeComponent();
            _obj = frmObject;
            this.Text = title;
            lblInfo.Text = msg;
            _action = action;
            this.Show();
            this.Focus();
        }

        private void cmdSubmit_Click(object sender, System.EventArgs e)
        {
            _obj.HandleAction(txtInput.Text, _action);
            this.Close();
        }

        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}

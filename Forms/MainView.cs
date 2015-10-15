using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Simple_Filemanager
{
    public enum FileAction
    {
        NewFolder,
        NewFile,
        Delete
    }
    public partial class frmMain : Form
    {
        private frmInput _InputBox;
        private string _initialDirectory = @"C:";
        public frmMain()
        {
            InitializeComponent();
            listDir.View = View.Details;
            listDir.Columns.Add("Date Modified");
            listDir.Columns.Add("Filesize");
            ProcessLogicalDrives();
            comboDrive.SelectedItem = _initialDirectory;
            ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
        }

        private void ProcessDirectories(DirectoryInfo dir)
        {
            listDir.Items.Clear();
            try
            {
                foreach (DirectoryInfo d in dir.GetDirectories())
                    listDir.Items.Add(new ListViewItem(new string[] { d.Name, d.LastWriteTimeUtc.ToShortDateString(), "Folder" }));
            }
            catch { return; }
            try
            {
                foreach (FileInfo f in dir.GetFiles())
                    listDir.Items.Add(new ListViewItem(new string[] { f.Name, f.LastWriteTimeUtc.ToShortDateString(), (f.Length / 1000).ToString() + "kb" }));
            }
            catch { return; }
            if (listDir.Items.Count > 0)
                listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            else
                listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void ProcessLogicalDrives()
        {
            try
            {
                string[] drives = Directory.GetLogicalDrives();

                foreach (string drive in drives)
                    comboDrive.Items.Add(drive.Replace(@"\", null));
            }
            catch {}
        }

        private void listDir_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    txtAddress.Text += listDir.SelectedItems[0].Text + @"\";
                    ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
                }
                else
                    Process.Start(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
            }
            catch { return; }
        }
        

        private void cmdGo_Click(object sender, EventArgs e)
        {
            ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
        }

        private void comboDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAddress.Focus();
            txtAddress.SelectAll();
            ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
        }

        private void cmdBack_Click(object sender, EventArgs e)
        {
            if(txtAddress.TextLength > 1)
                txtAddress.Text = txtAddress.Text.Remove(txtAddress.Text.Length - 1, 1);
            if (txtAddress.Text.LastIndexOf(@"\") + 1 > 0)
            {
                txtAddress.Text = txtAddress.Text.Substring(0, txtAddress.Text.LastIndexOf(@"\") + 1);
                ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
            }
        }
        public void HandleAction(string input, FileAction action)
        {
            switch (action)
            {
                case FileAction.NewFolder:
                    {
                        Directory.CreateDirectory(comboDrive.Text + txtAddress.Text + input);
                        ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
                        break;
                    }
                case FileAction.NewFile:
                    {
                        File.Create(comboDrive.Text + txtAddress.Text + input);
                        ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
                        break;
                    }
                case FileAction.Delete:
                    {
                        if (input.ToLower() == listDir.SelectedItems[0].Text.ToLower())
                        {
                            if (Directory.Exists(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text))
                                Directory.Delete(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
                            else if (File.Exists(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text))
                                File.Delete(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
                        }
                        else
                            MessageBox.Show("Error: File/Directory does not exsist.");
                        ProcessDirectories(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
                        break;
                    }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _InputBox = new frmInput(this, "Add Folder", "Type the name of the folder:", FileAction.NewFolder);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _InputBox = new frmInput(this, String.Format("Delete {0}?", listDir.SelectedItems[0].Text), "Please type the name of the folder as confirmation. This action cannot be undone.", FileAction.Delete);
        }

        private void addFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _InputBox = new frmInput(this, "Add File", "Type the name of the file (with ext):", FileAction.NewFile);
        }
    }
}

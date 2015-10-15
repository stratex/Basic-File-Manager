using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_Filemanager
{
    public partial class frmMain : Form
    {
        string dir = @"C:\";
        public frmMain()
        {
            InitializeComponent();
            listDir.View = View.Details;
            listDir.Columns.Add("Date Modified");
            listDir.Columns.Add("Filesize");
            foreach (string item in GetLogicalDrives())
            {
                comboDrive.Items.Add(item);
            }
            comboDrive.SelectedItem = dir;
            FullDirList(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
        }

        private void FullDirList(DirectoryInfo dir)
        {
            listDir.Items.Clear();
            try
            {
                foreach (FileInfo f in dir.GetFiles())
                    listDir.Items.Add(new ListViewItem(new string[] { f.Name, f.LastWriteTimeUtc.ToShortDateString(), (f.Length / 1000).ToString() + "kb" }));
            }
            catch { return; }

            try {
                foreach (DirectoryInfo d in dir.GetDirectories())
                    listDir.Items.Add(new ListViewItem(new string[] { d.Name, d.LastWriteTimeUtc.ToShortDateString(), "Folder" }));
            }
            catch { return; }
            listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        private List<string> GetLogicalDrives()
        {
            List<string> DriveList = new List<string>();
            try
            {
                string[] drives = Directory.GetLogicalDrives();

                foreach (string drive in drives)
                {
                    DriveList.Add(drive);
                }
            }
            catch
            { }
            return DriveList;
        }

        private void listDir_DoubleClick(object sender, EventArgs e)
        {
            FileAttributes attr = File.GetAttributes(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
            if (attr.HasFlag(FileAttributes.Directory))
            {
                txtAddress.Text += listDir.SelectedItems[0].Text + @"\";
                FullDirList(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
            }
            else
                Process.Start(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
        }
        

        private void cmdGo_Click(object sender, EventArgs e)
        {
            FullDirList(new DirectoryInfo(comboDrive.Text + txtAddress.Text));
        }

        private void comboDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAddress.Focus();
            txtAddress.SelectAll();
        }
    }
}

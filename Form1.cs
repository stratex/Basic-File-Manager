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

            DirectoryInfo di = new DirectoryInfo(dir);
            FullDirList(di);
            foreach (DirectoryInfo folder in folders)
                listDir.Items.Add(new ListViewItem(new string[] { folder.Name, folder.LastWriteTimeUtc.ToShortDateString(), "Folder" }));
            foreach (FileInfo file in files)
                listDir.Items.Add(new ListViewItem(new string[] { file.Name, file.LastWriteTimeUtc.ToShortDateString(), (file.Length / 1000).ToString() + "kb" }));
            listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        static List<FileInfo> files = new List<FileInfo>();  // List that will hold the files and subfiles in path
        static List<DirectoryInfo> folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed
        static void FullDirList(DirectoryInfo dir)
        {
            files.Clear();
            folders.Clear();
            try
            {
                foreach (FileInfo f in dir.GetFiles())
                {
                    files.Add(f);
                }
            }
            catch
            {
                return;
            }

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
            }

        }
        public List<string> GetLogicalDrives()
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
                DirectoryInfo dir = new DirectoryInfo(comboDrive.Text + txtAddress.Text);
                FullDirList(dir);
                listDir.Items.Clear();
                foreach (DirectoryInfo folder in folders)
                    listDir.Items.Add(new ListViewItem(new string[] { folder.Name, folder.LastWriteTimeUtc.ToShortDateString(), "Folder" }));
                foreach (FileInfo file in files)
                    listDir.Items.Add(new ListViewItem(new string[] { file.Name, file.LastWriteTimeUtc.ToShortDateString(), (file.Length / 1000).ToString() + "kb" }));
                listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
                Process.Start(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text);
            //File.Open(comboDrive.Text + txtAddress.Text + listDir.SelectedItems[0].Text, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        

        private void cmdGo_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(comboDrive.Text + txtAddress.Text);
            FullDirList(dir);
            listDir.Items.Clear();
            foreach (DirectoryInfo folder in folders)
                listDir.Items.Add(new ListViewItem(new string[] { folder.Name, folder.LastWriteTimeUtc.ToShortDateString(), "Folder"}));
            foreach (FileInfo file in files)
                listDir.Items.Add(new ListViewItem(new string[] { file.Name, file.LastWriteTimeUtc.ToShortDateString(), (file.Length / 1000).ToString() + "kb" }));
            listDir.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void comboDrive_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtAddress.Focus();
            txtAddress.SelectAll();
        }
    }
}

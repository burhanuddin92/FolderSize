using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace InSoft
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            treeView1.BackColor = Color.Black;
        }

        private TreeNode CreateFolderNode(IO.FolderInfo folderInfo, long minSize)
        {
            TreeNode treeNode = new TreeNode();

            string sizeText = ShowSize(folderInfo);

            treeNode.Text = sizeText + " => " + Path.GetFileName(folderInfo.Path);

            if (sizeText.EndsWith("GB"))
            {
                treeNode.ForeColor = Color.OrangeRed;
            }
            else if (sizeText.EndsWith("MB"))
            {
                treeNode.ForeColor = Color.Yellow;
            }
            else if (sizeText.EndsWith("kB"))
            {
                treeNode.ForeColor = Color.LightGreen;
            }
            else
            {
                treeNode.ForeColor = Color.White;
            }

            foreach (var item in folderInfo.Folders)
            {
                if (item.Bytes >= minSize)
                {
                    treeNode.Nodes.Add(CreateFolderNode(item, minSize));
                }
            }

            foreach (var item in folderInfo.Files)
            {
                if (item.Length >= minSize)
                {
                    treeNode.Nodes.Add(CreateFileNode(item));
                }
            }

            return treeNode;
        }

        private TreeNode CreateFileNode(FileInfo fileInfo)
        {
            TreeNode treeNode = new TreeNode();

            string sizeText = ShowSize(fileInfo.Length);

            treeNode.Text = sizeText + " => " + fileInfo.Name;

            if (sizeText.EndsWith("GB"))
            {
                treeNode.ForeColor = Color.OrangeRed;
            }
            else if (sizeText.EndsWith("MB"))
            {
                treeNode.ForeColor = Color.Yellow;
            }
            else if (sizeText.EndsWith("kB"))
            {
                treeNode.ForeColor = Color.LightGreen;
            }
            else
            {
                treeNode.ForeColor = Color.White;
            }

            return treeNode;
        }

        private string ShowSize(IO.FolderInfo folderInfo)
        {
            return ShowSize(folderInfo.Bytes);
        }

        private string ShowSize(long Length)
        {
            if (Length > 1024 * 1024 * 1024)
            {
                return (Length / 1024 / 1024 / 1024).ToString("f2") + " GB";
            }

            else if (Length > 1024 * 1024)
            {
                return (Length / 1024 / 1024).ToString("f2") + " MB";
            }

            else if (Length > 1024)
            {
                return (Length / 1024).ToString("f2") + " kB";
            }

            else
            {
                return Length.ToString("f2") + " B";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            long filterSize = 0;

            if (radioButton1.Checked)
            {
                filterSize = 0;
            }
            else if (radioButton2.Checked)
            {
                filterSize = 1024 * 1024 * 1024;
            }
            else if (radioButton3.Checked)
            {
                filterSize = 100 * 1024 * 1024;
            }
            else if (radioButton4.Checked)
            {
                filterSize = 1024;
            }

            if (textBox1.Text != string.Empty)
            {
                if (Directory.Exists(textBox1.Text))
                {
                    TreeNode treeNode = CreateFolderNode(IO.FolderInfo.GetInfo(textBox1.Text), filterSize);

                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(treeNode);
                }
            }
        }
    }
}

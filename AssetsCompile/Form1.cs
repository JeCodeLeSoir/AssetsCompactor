using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AssetsCompile
{
    public partial class Form1 : Form
    {
        AssetsCompile _AssetsCompile;

        TreeNode root;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public Form1()
        {
            AllocConsole();
            InitializeComponent();

           _AssetsCompile = new AssetsCompile();

            //byte[] data = Encoding.UTF8.GetBytes("Aurélien");
            //byte [] test_key =  _AssetsCompile.EncryptKey(data, "123");
            //Console.WriteLine(Encoding.UTF8.GetString(test_key));
            //bool isValide;
            //Console.WriteLine(Encoding.UTF8.GetString(_AssetsCompile.DecryptKey(test_key, "123", out isValide)));
            //Console.WriteLine(isValide);

            Console.WriteLine("\n");
            Console.WriteLine("================");

            root = new TreeNode("Assets");
            root.Tag = "dir";
             
            root.ImageIndex = 0;
            root.SelectedImageIndex = 0;

            treeView1.ContextMenuStrip = contextMenuStrip_TreeNode;

            toolStripMenuItem_Remove.Click += (object sender, EventArgs e) =>
            {
                List<sFile> fileheaders = _AssetsCompile.getfileheaders();
                TreeNode SelectedNode = GetSelect(true);

                if (SelectedNode.Text != "Assets")
                {

                    Console.WriteLine(" ? =================== ? ");
                    Console.WriteLine(" Dir ? : " + ((string)SelectedNode.Tag == "dir" ? "yes" : "non"));

                    string deletepath = GetPathString(SelectedNode);

                    if ((string)SelectedNode.Tag == "dir")
                    {
                        RecursiveRemove(SelectedNode);
                    }
                    else
                    {
                        _AssetsCompile.removeFile(deletepath);
                        SelectedNode.Remove();
                    }

                    Console.WriteLine(" ? =================== ? ");

                }

            };

            treeView1.Nodes.Add(root);
        }

        void RecursiveRemove(TreeNode SelectedNode)
        {
            for (int i = 0; i < SelectedNode.Nodes.Count; i++)
            {
                TreeNode pvalue = SelectedNode.Nodes[i];
                if ((string)pvalue.Tag != "dir")
                {
                    _AssetsCompile.removeFile(GetPathString(pvalue));
                    pvalue.Remove();
                }
                else
                {
                    RecursiveRemove(pvalue);
                }
            }

            SelectedNode.Remove();
        }
        

        private void ButtonAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,


                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileInfo info = new FileInfo(openFileDialog1.FileName);

                TreeNode SelectedNode = GetSelect();

                string path_file = SelectedNode.FullPath + @"\" + info.Name;

                Console.WriteLine(path_file);

                _AssetsCompile.AddFile(path_file, openFileDialog1.FileName);
                var nodeFile = new TreeNode(info.Name);
                nodeFile.ImageIndex = 2;
                nodeFile.SelectedImageIndex = 2;
                SelectedNode.Nodes.Add(nodeFile);
            }
        }

        bool Contain(TreeNode node, string name)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text.ToUpper() == name.ToUpper())
                    return true;
            }

            return false;
        }

        TreeNode GetSelect(bool isFile = false)
        {
            TreeNode SelectedNode = treeView1.SelectedNode;

            if (SelectedNode == null)
                SelectedNode = root;

            if(!isFile)
                if ((string)SelectedNode.Tag != "dir")
                {
                    SelectedNode = SelectedNode.Parent;
                }

            return SelectedNode;
        }

        private void buttonCompile_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                DefaultExt = ".asset",
                Filter = "asset files (*.asset)|",
                CheckPathExists = true,
            };

            if (save.ShowDialog() == DialogResult.OK)
            {
                //Directory.CreateDirectory(Path.GetFullPath(save.FileName));
                _AssetsCompile.Compile(save.FileName, textBox_password.Text);
            }
        }

        private void buttonAddFolder_Click(object sender, EventArgs e)
        {
        ici:
            string input = Microsoft.VisualBasic.Interaction.InputBox("Name Folder",
                       "Create Folder",
                       "Name",
                       0,
                       0);

            if (input.Length > 0)
            {
                var newdir = new TreeNode(input);
                newdir.ImageIndex = 0;
                newdir.SelectedImageIndex = 0;
                newdir.StateImageIndex = 1;
                newdir.Tag = "dir";

                TreeNode SelectedNode = GetSelect();

                if (Contain(SelectedNode, input))
                {
                    Microsoft.VisualBasic.Interaction.MsgBox("Error dir exist !");
                    goto ici;
                }

                SelectedNode.Nodes.Add(newdir);
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,

                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                root.Nodes.Clear();
                _AssetsCompile = new AssetsCompile();
                bool isValide;

                _AssetsCompile.Read(File.ReadAllBytes(openFileDialog1.FileName), textBox_password.Text, out isValide);

                if (isValide)
                {
                    List<sFile> fileheaders = _AssetsCompile.getfileheaders();
                    for (int i = 0; i < fileheaders.Count; i++)
                    {
                        sFile _fileheader = fileheaders[i];

                        var nodeFile = new TreeNode(_fileheader.name);
                        nodeFile.ImageIndex = 2;
                        nodeFile.SelectedImageIndex = 2;
                        CreateOrGetPath(_fileheader.path).Nodes.Add(nodeFile);
                    }
                }
                else
                {
                    Microsoft.VisualBasic.Interaction.MsgBox("impossible open file !");
                }
               
            }
        }

        string GetPathString(TreeNode node)
        {

            List<string> paths = new List<string>();

        Exec:
            paths.Add(node.Text);

            if (node.Text.ToUpper() != "Assets".ToUpper())
            {
                node = node.Parent;
                goto Exec;
            }
            string path = "";
            paths.Reverse();
            for (int i = 0; i < paths.Count; i++)
            {
                path += (i >= 1 ? @"\" : "") + paths[i];
            }

            return path;
        }

        TreeNode CreateOrGetPath(string path)
        {
            string[] paths = path.Split('\\');
            string[] names = new string[paths.Length - 1];
            Array.Copy(paths, names, paths.Length - 1);

            if (names.Length > 0)
            {
                TreeNode current = root;

                Dictionary<string, TreeNode> finds = new Dictionary<string, TreeNode>();

                for (int i = 0; i < names.Length; i++)
                {
                    Console.WriteLine(names[i]);

                    if (i == 0 && names[i].ToUpper() == "Assets".ToUpper())
                    {
                        finds.Add(names[i], root);
                    }
                    else
                        for (int y = 0; y < current.Nodes.Count; y++)
                        {
                            if (current.Nodes[y].Text == names[i])
                            {
                                current = current.Nodes[y];
                                finds.Add(names[i], current);
                            }
                        }
                }

                for (int i = 0; i < names.Length; i++)
                {
                    if (!finds.ContainsKey(names[i]))
                    {
                        var newdir = new TreeNode(names[i]);
                        newdir.Tag = "dir";
                        newdir.ImageIndex = 0;
                        newdir.SelectedImageIndex = 0;
                        newdir.StateImageIndex = 1;
                        current.Nodes.Add(newdir);
                        current = newdir;
                        finds.Add(names[i], current);
                    }
                    else
                    {
                        current = finds[names[i]];
                    }
                }

                return finds[names[names.Length - 1]];

            }
            else
            {
                return root;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode SelectedNode = treeView1.SelectedNode;
            if (SelectedNode != null)
                if ((string)SelectedNode.Tag != "dir") {
                    List<sFile> fileheaders = _AssetsCompile.getfileheaders();
                    for (int i = 0; i < fileheaders.Count; i++)
                    {
                        sFile _fileheader = fileheaders[i];
                        if (_fileheader.name.ToUpper() == SelectedNode.Text.ToUpper())
                        {
                            Console.WriteLine(_fileheader.type);

                            _fileheader.type = _fileheader.type.ToUpper();
                            textBox_preview.Visible = false;
                            pictureBox_preview.Visible = false;
                            if (
                                _fileheader.type == ".PNG"
                                || _fileheader.type == ".JPG"
                                || _fileheader.type == ".GIF"
                            )
                            {
                                pictureBox_preview.Visible = true;
                                pictureBox_preview.Image = Image.FromStream(
                                new MemoryStream(_fileheader.content));
                            }
                            else
                            {
                                textBox_preview.Visible = true;
                                textBox_preview.Text = Encoding.UTF8.GetString(_fileheader.content);
                            }
                        }
                    }
                }
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {

        }


    }
}

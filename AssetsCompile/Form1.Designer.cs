
namespace AssetsCompile
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ButtonAddFile = new System.Windows.Forms.Button();
            this.buttonCompile = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonAddFolder = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.pictureBox_preview = new System.Windows.Forms.PictureBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.contextMenuStrip_TreeNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).BeginInit();
            this.contextMenuStrip_TreeNode.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonAddFile
            // 
            this.ButtonAddFile.Location = new System.Drawing.Point(517, 28);
            this.ButtonAddFile.Name = "ButtonAddFile";
            this.ButtonAddFile.Size = new System.Drawing.Size(141, 42);
            this.ButtonAddFile.TabIndex = 0;
            this.ButtonAddFile.Text = "Add File";
            this.ButtonAddFile.UseVisualStyleBackColor = true;
            this.ButtonAddFile.Click += new System.EventHandler(this.ButtonAddFile_Click);
            // 
            // buttonCompile
            // 
            this.buttonCompile.Location = new System.Drawing.Point(12, 421);
            this.buttonCompile.Name = "buttonCompile";
            this.buttonCompile.Size = new System.Drawing.Size(169, 43);
            this.buttonCompile.TabIndex = 1;
            this.buttonCompile.Text = "Compile";
            this.buttonCompile.UseVisualStyleBackColor = true;
            this.buttonCompile.Click += new System.EventHandler(this.buttonCompile_Click);
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(480, 396);
            this.treeView1.TabIndex = 2;
            this.treeView1.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterExpand);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "FolderClosed_16x.png");
            this.imageList1.Images.SetKeyName(1, "FolderOpened_16x.png");
            this.imageList1.Images.SetKeyName(2, "Image_16x.png");
            this.imageList1.Images.SetKeyName(3, "Audio_16x.png");
            // 
            // buttonAddFolder
            // 
            this.buttonAddFolder.Location = new System.Drawing.Point(516, 86);
            this.buttonAddFolder.Name = "buttonAddFolder";
            this.buttonAddFolder.Size = new System.Drawing.Size(141, 30);
            this.buttonAddFolder.TabIndex = 3;
            this.buttonAddFolder.Text = "Add Folder";
            this.buttonAddFolder.UseVisualStyleBackColor = true;
            this.buttonAddFolder.Click += new System.EventHandler(this.buttonAddFolder_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(197, 423);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(151, 40);
            this.buttonOpen.TabIndex = 4;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // pictureBox_preview
            // 
            this.pictureBox_preview.Location = new System.Drawing.Point(517, 241);
            this.pictureBox_preview.Name = "pictureBox_preview";
            this.pictureBox_preview.Size = new System.Drawing.Size(141, 139);
            this.pictureBox_preview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_preview.TabIndex = 5;
            this.pictureBox_preview.TabStop = false;
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(388, 433);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(245, 23);
            this.textBox_password.TabIndex = 6;
            // 
            // contextMenuStrip_TreeNode
            // 
            this.contextMenuStrip_TreeNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Remove});
            this.contextMenuStrip_TreeNode.Name = "contextMenuStrip_TreeNode";
            this.contextMenuStrip_TreeNode.Size = new System.Drawing.Size(118, 26);
            // 
            // toolStripMenuItem_Remove
            // 
            this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
            this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem_Remove.Text = "Remove";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(669, 494);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.pictureBox_preview);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.buttonAddFolder);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.buttonCompile);
            this.Controls.Add(this.ButtonAddFile);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_preview)).EndInit();
            this.contextMenuStrip_TreeNode.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Button ButtonAddFile;
        private System.Windows.Forms.Button buttonCompile;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button buttonAddFolder;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.PictureBox pictureBox_preview;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_TreeNode;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Remove;
    }
}


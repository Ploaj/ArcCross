namespace CrossArc.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateHashesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.searchLabel = new System.Windows.Forms.Label();
            this.exportFileSystemToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFileSystemToTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.updateHashesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(704, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openARCToolStripMenuItem,
            this.exportFileSystemToXMLToolStripMenuItem,
            this.exportFileSystemToTXTToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openARCToolStripMenuItem
            // 
            this.openARCToolStripMenuItem.Name = "openARCToolStripMenuItem";
            this.openARCToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openARCToolStripMenuItem.Text = "Open ARC";
            this.openARCToolStripMenuItem.Click += new System.EventHandler(this.openARCToolStripMenuItem_Click);
            // 
            // updateHashesToolStripMenuItem
            // 
            this.updateHashesToolStripMenuItem.Name = "updateHashesToolStripMenuItem";
            this.updateHashesToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.updateHashesToolStripMenuItem.Text = "Update Hashes";
            this.updateHashesToolStripMenuItem.Click += new System.EventHandler(this.updateHashesToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Indent = 18;
            this.treeView1.ItemHeight = 24;
            this.treeView1.Location = new System.Drawing.Point(12, 76);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(421, 353);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.Location = new System.Drawing.Point(439, 50);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(265, 379);
            this.propertyGrid1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Japanese",
            "American English",
            "Canadian French",
            "Latin American Spanish",
            "British English",
            "European French",
            "Castilian Spanish",
            "German",
            "Italian",
            "Dutch",
            "Russian",
            "Korean",
            "Simplified Chinese",
            "Traditional Chinese",
            "All"});
            this.comboBox1.Location = new System.Drawing.Point(497, 27);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(195, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Arc Version: ";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(63, 50);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(370, 20);
            this.searchBox.TabIndex = 5;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(436, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Language:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Search:";
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(368, 34);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(62, 13);
            this.searchLabel.TabIndex = 8;
            this.searchLabel.Text = "searching...";
            this.searchLabel.Visible = false;
            // 
            // exportFileSystemToXMLToolStripMenuItem
            // 
            this.exportFileSystemToXMLToolStripMenuItem.Name = "exportFileSystemToXMLToolStripMenuItem";
            this.exportFileSystemToXMLToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.exportFileSystemToXMLToolStripMenuItem.Text = "Export FileSystem To XML";
            this.exportFileSystemToXMLToolStripMenuItem.Click += new System.EventHandler(this.exportFileSystemToXMLToolStripMenuItem_Click);
            // 
            // exportFileSystemToTXTToolStripMenuItem
            // 
            this.exportFileSystemToTXTToolStripMenuItem.Name = "exportFileSystemToTXTToolStripMenuItem";
            this.exportFileSystemToTXTToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.exportFileSystemToTXTToolStripMenuItem.Text = "Export FileSystem To TXT";
            this.exportFileSystemToTXTToolStripMenuItem.Click += new System.EventHandler(this.exportFileSystemToTXTToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 441);
            this.Controls.Add(this.searchLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "CrossArc GUI";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openARCToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem updateHashesToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ToolStripMenuItem exportFileSystemToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportFileSystemToTXTToolStripMenuItem;
    }
}
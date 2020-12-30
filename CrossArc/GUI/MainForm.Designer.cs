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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openARCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFileSystemToXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportFileSystemToTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateHashesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.arcFilePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.searchLabel = new System.Windows.Forms.Label();
            this.searchOffsetCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutSearch = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutLeft = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutRight = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutLanguage = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutSearch.SuspendLayout();
            this.tableLayoutLeft.SuspendLayout();
            this.tableLayoutRight.SuspendLayout();
            this.tableLayoutLanguage.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.updateHashesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1076, 24);
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
            this.openARCToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openARCToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.openARCToolStripMenuItem.Text = "Open ARC";
            this.openARCToolStripMenuItem.Click += new System.EventHandler(this.openARCToolStripMenuItem_Click);
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
            this.exportFileSystemToTXTToolStripMenuItem.Text = "Export FileSystem To CSV";
            this.exportFileSystemToTXTToolStripMenuItem.Click += new System.EventHandler(this.exportFileSystemToCsvToolStripMenuItem_Click);
            // 
            // updateHashesToolStripMenuItem
            // 
            this.updateHashesToolStripMenuItem.Name = "updateHashesToolStripMenuItem";
            this.updateHashesToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.updateHashesToolStripMenuItem.Text = "Update Hashes";
            this.updateHashesToolStripMenuItem.Click += new System.EventHandler(this.updateHashesToolStripMenuItem_Click);
            // 
            // fileTreeView
            // 
            this.fileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTreeView.Indent = 18;
            this.fileTreeView.ItemHeight = 24;
            this.fileTreeView.Location = new System.Drawing.Point(3, 53);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.Size = new System.Drawing.Size(694, 548);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // arcFilePropertyGrid
            // 
            this.arcFilePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arcFilePropertyGrid.Location = new System.Drawing.Point(3, 33);
            this.arcFilePropertyGrid.Name = "arcFilePropertyGrid";
            this.arcFilePropertyGrid.Size = new System.Drawing.Size(338, 568);
            this.arcFilePropertyGrid.TabIndex = 2;
            this.arcFilePropertyGrid.ToolbarVisible = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
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
            "Dutch",
            "Italian",
            "Russian",
            "Korean",
            "Simplified Chinese",
            "Traditional Chinese",
            "All"});
            this.comboBox1.Location = new System.Drawing.Point(67, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(268, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Arc Version: ";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Location = new System.Drawing.Point(130, 3);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(418, 20);
            this.searchBox.TabIndex = 5;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Language:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Search:";
            // 
            // searchLabel
            // 
            this.searchLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(624, 5);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(62, 13);
            this.searchLabel.TabIndex = 8;
            this.searchLabel.Text = "searching...";
            this.searchLabel.Visible = false;
            // 
            // searchOffsetCheckBox
            // 
            this.searchOffsetCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.searchOffsetCheckBox.AutoSize = true;
            this.searchOffsetCheckBox.Location = new System.Drawing.Point(556, 3);
            this.searchOffsetCheckBox.Name = "searchOffsetCheckBox";
            this.searchOffsetCheckBox.Size = new System.Drawing.Size(54, 17);
            this.searchOffsetCheckBox.TabIndex = 9;
            this.searchOffsetCheckBox.Text = "Offset";
            this.searchOffsetCheckBox.UseVisualStyleBackColor = true;
            this.searchOffsetCheckBox.CheckedChanged += new System.EventHandler(this.searchOffsetCheckBox_CheckedChanged);
            // 
            // tableLayoutSearch
            // 
            this.tableLayoutSearch.ColumnCount = 4;
            this.tableLayoutSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 23.15789F));
            this.tableLayoutSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76.8421F));
            this.tableLayoutSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutSearch.Controls.Add(this.label3, 0, 0);
            this.tableLayoutSearch.Controls.Add(this.searchLabel, 3, 0);
            this.tableLayoutSearch.Controls.Add(this.searchOffsetCheckBox, 2, 0);
            this.tableLayoutSearch.Controls.Add(this.searchBox, 1, 0);
            this.tableLayoutSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutSearch.Location = new System.Drawing.Point(3, 23);
            this.tableLayoutSearch.Name = "tableLayoutSearch";
            this.tableLayoutSearch.RowCount = 1;
            this.tableLayoutSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutSearch.Size = new System.Drawing.Size(694, 24);
            this.tableLayoutSearch.TabIndex = 10;
            // 
            // tableLayoutLeft
            // 
            this.tableLayoutLeft.ColumnCount = 1;
            this.tableLayoutLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutLeft.Controls.Add(this.label1, 0, 0);
            this.tableLayoutLeft.Controls.Add(this.tableLayoutSearch, 0, 1);
            this.tableLayoutLeft.Controls.Add(this.fileTreeView, 0, 2);
            this.tableLayoutLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutLeft.Location = new System.Drawing.Point(13, 3);
            this.tableLayoutLeft.Name = "tableLayoutLeft";
            this.tableLayoutLeft.RowCount = 3;
            this.tableLayoutLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutLeft.Size = new System.Drawing.Size(700, 604);
            this.tableLayoutLeft.TabIndex = 13;
            // 
            // tableLayoutRight
            // 
            this.tableLayoutRight.ColumnCount = 1;
            this.tableLayoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutRight.Controls.Add(this.arcFilePropertyGrid, 0, 1);
            this.tableLayoutRight.Controls.Add(this.tableLayoutLanguage, 0, 0);
            this.tableLayoutRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutRight.Location = new System.Drawing.Point(719, 3);
            this.tableLayoutRight.Name = "tableLayoutRight";
            this.tableLayoutRight.RowCount = 2;
            this.tableLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutRight.Size = new System.Drawing.Size(344, 604);
            this.tableLayoutRight.TabIndex = 14;
            // 
            // tableLayoutLanguage
            // 
            this.tableLayoutLanguage.ColumnCount = 2;
            this.tableLayoutLanguage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutLanguage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutLanguage.Controls.Add(this.comboBox1, 1, 0);
            this.tableLayoutLanguage.Controls.Add(this.label2, 0, 0);
            this.tableLayoutLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutLanguage.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutLanguage.Name = "tableLayoutLanguage";
            this.tableLayoutLanguage.RowCount = 1;
            this.tableLayoutLanguage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutLanguage.Size = new System.Drawing.Size(338, 24);
            this.tableLayoutLanguage.TabIndex = 3;
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutMain.Controls.Add(this.tableLayoutLeft, 0, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutRight, 1, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.Padding = new System.Windows.Forms.Padding(10, 0, 10, 20);
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1076, 630);
            this.tableLayoutMain.TabIndex = 15;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 654);
            this.Controls.Add(this.tableLayoutMain);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 300);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cross Arc";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutSearch.ResumeLayout(false);
            this.tableLayoutSearch.PerformLayout();
            this.tableLayoutLeft.ResumeLayout(false);
            this.tableLayoutLeft.PerformLayout();
            this.tableLayoutRight.ResumeLayout(false);
            this.tableLayoutLanguage.ResumeLayout(false);
            this.tableLayoutLanguage.PerformLayout();
            this.tableLayoutMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openARCToolStripMenuItem;
        private System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.PropertyGrid arcFilePropertyGrid;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem updateHashesToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.ToolStripMenuItem exportFileSystemToXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportFileSystemToTXTToolStripMenuItem;
        private System.Windows.Forms.CheckBox searchRegexCheckBox;
        private System.Windows.Forms.CheckBox searchOffsetCheckBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutLeft;
        private System.Windows.Forms.TableLayoutPanel tableLayoutRight;
        private System.Windows.Forms.TableLayoutPanel tableLayoutLanguage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
    }
}

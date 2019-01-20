namespace CrossArc
{
    partial class Form1
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
            fileTree = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.offLabel = new System.Windows.Forms.Label();
            this.compLabel = new System.Windows.Forms.Label();
            this.decompLabel = new System.Windows.Forms.Label();
            this.flagLabel = new System.Windows.Forms.Label();
            this.regionCB = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileTree
            // 
            fileTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            fileTree.ItemHeight = 24;
            fileTree.Location = new System.Drawing.Point(0, 0);
            fileTree.Name = "fileTree";
            fileTree.Size = new System.Drawing.Size(431, 288);
            fileTree.TabIndex = 0;
            fileTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.fileTree_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Offset";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(437, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(193, 149);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File Properties";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.03743F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.96257F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.offLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.compLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.decompLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.flagLabel, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(187, 130);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "CompSize";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "DecompSize";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Flags";
            // 
            // offLabel
            // 
            this.offLabel.AutoSize = true;
            this.offLabel.Location = new System.Drawing.Point(80, 0);
            this.offLabel.Name = "offLabel";
            this.offLabel.Size = new System.Drawing.Size(0, 13);
            this.offLabel.TabIndex = 5;
            // 
            // compLabel
            // 
            this.compLabel.AutoSize = true;
            this.compLabel.Location = new System.Drawing.Point(80, 32);
            this.compLabel.Name = "compLabel";
            this.compLabel.Size = new System.Drawing.Size(0, 13);
            this.compLabel.TabIndex = 6;
            // 
            // decompLabel
            // 
            this.decompLabel.AutoSize = true;
            this.decompLabel.Location = new System.Drawing.Point(80, 64);
            this.decompLabel.Name = "decompLabel";
            this.decompLabel.Size = new System.Drawing.Size(0, 13);
            this.decompLabel.TabIndex = 7;
            // 
            // flagLabel
            // 
            this.flagLabel.AutoSize = true;
            this.flagLabel.Location = new System.Drawing.Point(80, 96);
            this.flagLabel.Name = "flagLabel";
            this.flagLabel.Size = new System.Drawing.Size(0, 13);
            this.flagLabel.TabIndex = 8;
            // 
            // regionCB
            // 
            this.regionCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.regionCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.regionCB.FormattingEnabled = true;
            this.regionCB.Items.AddRange(new object[] {
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
            this.regionCB.Location = new System.Drawing.Point(440, 174);
            this.regionCB.Name = "regionCB";
            this.regionCB.Size = new System.Drawing.Size(187, 21);
            this.regionCB.TabIndex = 3;
            this.regionCB.SelectedIndexChanged += new System.EventHandler(this.regionCB_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(440, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Region to extract:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 288);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.regionCB);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(fileTree);
            this.Name = "Form1";
            this.Text = "ARCross";
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label offLabel;
        private System.Windows.Forms.Label compLabel;
        private System.Windows.Forms.Label decompLabel;
        private System.Windows.Forms.Label flagLabel;
        private System.Windows.Forms.ComboBox regionCB;
        private System.Windows.Forms.Label label5;
        public static System.Windows.Forms.TreeView fileTree;
    }
}
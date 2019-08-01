using ArcCross;
using CrossArc.GUI.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossArc.GUI
{
    public partial class MainForm : Form
    {
        public string FilePath;
        public int Version;
        public static int SelectedRegion
        {
            get => int.Parse(ConfigurationManager.AppSettings["Region"]);
            set
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Region"].Value = value.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public static Arc ArcFile;

        public static ContextMenu NodeContextMenu;

        private GuiNode rootNode;

        private BackgroundWorker searchWorker;

        public Dictionary<string, FileInformation> pathToFileInfomation = new Dictionary<string, FileInformation>();

        public MainForm()
        {
            InitializeComponent();

            treeView1.BeforeExpand += folderTree_BeforeExpand;

            treeView1.NodeMouseClick += (sender, args) => treeView1.SelectedNode = args.Node;

            treeView1.HideSelection = false;

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("folder", Properties.Resources.folder);
            treeView1.ImageList.Images.Add("file", Properties.Resources.file);

            exportFileSystemToXMLToolStripMenuItem.Enabled = false;
            exportFileSystemToTXTToolStripMenuItem.Enabled = false;

            comboBox1.SelectedIndex = SelectedRegion;

            comboBox1.SelectedIndexChanged += (sender, args) =>
            {
                SelectedRegion = comboBox1.SelectedIndex;
                treeView1_AfterSelect(null, null);
            };

            NodeContextMenu = new ContextMenu();

            {
                MenuItem item = new MenuItem("Extract file(s)");
                item.Click += ExtractFile;
                NodeContextMenu.MenuItems.Add(item);
            }
            {
                MenuItem item = new MenuItem("Extract file(s) (Compressed)");
                item.Click += ExtractFileComp;
                NodeContextMenu.MenuItems.Add(item);
            }
            {
                MenuItem item = new MenuItem("Extract file(s) (With Offset)");
                item.Click += ExtractFileOffset;
                NodeContextMenu.MenuItems.Add(item);
            }
            {
                MenuItem item = new MenuItem("Extract file(s) (Compressed, With Offset)");
                item.Click += ExtractFileCompOffset;
                NodeContextMenu.MenuItems.Add(item);
            }
        }

        private void ExtractFile(object sender, EventArgs args)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n && n.Base is FileNode file)
            {
                ProgressBar bar = new ProgressBar();
                bar.Show();
                bar.Extract(new[] { file });
            }
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n2 && n2.Base is FolderNode folder)
            {
                ProgressBar bar = new ProgressBar();
                bar.Show();
                bar.Extract(folder.GetAllFiles());
            }
        }

        private void ExtractFileComp(object sender, EventArgs args)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n && n.Base is FileNode file)
            {
                ProgressBar bar = new ProgressBar {DecompressFiles = false};
                bar.Show();
                bar.Extract(new[] { file });
            }
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n2 && n2.Base is FolderNode folder)
            {
                ProgressBar bar = new ProgressBar {DecompressFiles = false};
                bar.Show();
                bar.Extract(folder.GetAllFiles());
            }
        }

        private void ExtractFileOffset(object sender, EventArgs args)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n && n.Base is FileNode file)
            {
                ProgressBar bar = new ProgressBar();
                bar.UseOffsetName = true;
                bar.Show();
                bar.Extract(new[] { file });
            }
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n2 && n2.Base is FolderNode folder)
            {
                ProgressBar bar = new ProgressBar();
                bar.UseOffsetName = true;
                bar.Show();
                bar.Extract(folder.GetAllFiles());
            }
        }

        private void ExtractFileCompOffset(object sender, EventArgs args)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n && n.Base is FileNode file)
            {
                ProgressBar bar = new ProgressBar();
                bar.UseOffsetName = true;
                bar.DecompressFiles = false;
                bar.Show();
                bar.Extract(new[] { file });
            }
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n2 && n2.Base is FolderNode folder)
            {
                ProgressBar bar = new ProgressBar();
                bar.UseOffsetName = true;
                bar.DecompressFiles = false;
                bar.Show();
                bar.Extract(folder.GetAllFiles());
            }
        }

        private void folderTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is GuiNode n)
                n.BeforeExpand();
        }

        private void openARCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.FileName = "data.arc";
                d.Filter += "Smash Ultimate ARC|*.arc";
                if (d.ShowDialog() == DialogResult.OK)
                {
                    // loading the arc freezes everything for now
                    // hashes can probably be loaded asynchronously
                    Cursor.Current = Cursors.WaitCursor;

                    var s = System.Diagnostics.Stopwatch.StartNew();

                    ArcFile = new Arc(d.FileName);

                    s.Restart();

                    InitFileSystem();
                    System.Diagnostics.Debug.WriteLine("init nodes: " + s.Elapsed.Milliseconds);

                    Cursor.Current = Cursors.Arrow;

                    // update arc version label
                    label1.Text = "Arc Version: " + ArcFile.Version.ToString("X");

                    // enable controls that can only be accessed when the arc is loaded
                    updateHashesToolStripMenuItem.Enabled = false;
                    exportFileSystemToXMLToolStripMenuItem.Enabled = true;
                    exportFileSystemToTXTToolStripMenuItem.Enabled = true;

                    Version = ArcFile.Version;
                    FilePath = d.FileName;

                    // explicitly unload hashe dictionary as it is no longer needed
                    HashDict.Unload();
                }
            }
        }

        private void InitFileSystem()
        {
            treeView1.Nodes.Clear();
            FolderNode root = new FolderNode("root");
            foreach (var file in ArcFile.GetFileList())
            {
                string[] path = file.Split('/');
                ProcessFile(root, path, 0);

            }
            foreach (var file in ArcFile.GetStreamFileList())
            {
                string[] path = file.Split('/');
                ProcessFile(root, path, 0);
            }
            rootNode = new GuiNode(root);
            treeView1.Nodes.Add(rootNode);
        }

        private void ProcessFile(FolderNode parent, string[] path, int index)
        {
            if (path.Length - 1 == index)
            {
                var FileNode = new FileNode(path[index]);
                parent.AddChild(FileNode);
                return;
            }

            FolderNode node = null;
            string folderName = path[index];
            foreach (var f in parent.Children)
            {
                if (f.Text.Equals(folderName))
                {
                    node = (FolderNode)f;
                    break;
                }
            }

            if (node == null)
            {
                node = new FolderNode(folderName);
                parent.AddChild(node);
            }

            ProcessFile(node, path, index + 1);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid1.SelectedObject = null;
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is GuiNode n && n.Base is FileNode file)
            {
                propertyGrid1.SelectedObject = file.FileInformation;
            }
        }

        private async void updateHashesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dl = MessageBox.Show("Download the latest archive hashes from github?", "Update Hashes.txt", MessageBoxButtons.YesNo);

            // Disable the tool strip to prevent opening another arc or hashes before the file has finished downloading.
            if (dl == DialogResult.Yes)
            {
                menuStrip1.Enabled = false;

                await DownloadHashesAsync();

                menuStrip1.Enabled = true;
            }
        }

        private async Task DownloadHashesAsync()
        {
            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync("https://github.com/ultimate-research/archive-hashes/raw/master/Hashes", "Hashes.txt");
            }
        }


        private readonly object lockTree = new object();

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (!ArcFile.Initialized || rootNode == null)
                return;
            lock (lockTree)
            {
                if (searchWorker != null)
                {
                    searchWorker.CancelAsync();
                    searchWorker.Dispose();
                    searchWorker = null;
                }

                treeView1.Nodes.Clear();

                if (searchBox.Text == "")
                {
                    treeView1.Nodes.Add(rootNode);
                    searchLabel.Visible = false;
                }
                else
                {
                    searchWorker = new BackgroundWorker();
                    searchWorker.DoWork += Search;
                    searchWorker.ProgressChanged += AddNode;
                    searchWorker.WorkerSupportsCancellation = true;
                    searchWorker.WorkerReportsProgress = true;
                    searchWorker.RunWorkerAsync();
                    searchLabel.Visible = true;
                }
            }
        }

        private void AddNode(object sender, ProgressChangedEventArgs args)
        {
            lock (lockTree)
            {
                if (searchWorker != null)
                {
                    if (args.ProgressPercentage == 100)
                    {
                        System.Diagnostics.Debug.WriteLine("Done Searching");
                        searchLabel.Visible = false;
                    }
                    else
                        treeView1.Nodes.Add(new GuiNode((BaseNode)args.UserState));
                }
            }
        }

        private void Search(object sender, DoWorkEventArgs e)
        {
            Queue<BaseNode> toSearch = new Queue<BaseNode>();
            toSearch.Enqueue(rootNode.Base);

            bool interrupted = false;

            var key = searchBox.Text;
            if (key == "0")
                return;

            while (toSearch.Count > 0)
            {
                lock (lockTree)
                {
                    if (searchBox != null && key != searchBox.Text || searchWorker == null || searchWorker.CancellationPending)
                    {
                        interrupted = true;
                        break;
                    }

                    var s = toSearch.Dequeue();

                    if (s.Text.Contains(key))
                    {
                        searchWorker.ReportProgress(0, s);
                    }

                    if (s is FileNode file &&
                        key.Length >= 3 &&
                        key.StartsWith("0x") &&
                        long.TryParse(key.Substring(2, key.Length - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value) &&
                        file.FileInformation.Offset == value)
                    {
                        searchWorker.ReportProgress(0, s);
                    }

                    foreach (var b in s.SubNodes)
                    {
                        toSearch.Enqueue(b);
                    }
                }
            }

            if (!interrupted)
                searchWorker.ReportProgress(100, null);
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // add other code here
            /*if (e.Cancelled && restartWorker)
            {
                restartWorker = false;
                searchWorker.RunWorkerAsync();
            }*/
        }

        private void ExportAll(string key)
        {
            /*Queue<BaseNode> toSearch = new Queue<BaseNode>();
            toSearch.Enqueue(Root);
            List<FileNode> toExport = new List<FileNode>();

            while (toSearch.Count > 0)
            {

                var s = toSearch.Dequeue();

                if (s.Text.Contains(key))
                {
                    if (s is FileNode fn)
                        toExport.Add(fn);
                }

                foreach (var b in s.BaseNodes)
                {
                    toSearch.Enqueue(b);
                }
            }

            ProgressBar bar = new ProgressBar();
            bar.Show();
            bar.Extract(toExport.ToArray());*/
        }

        private void exportFileSystemToXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFS(true);
        }

        private void exportFileSystemToTXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportFS(false);
        }

        private void ExportFS(bool xml)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                if (xml)
                    d.Filter = "XML (*.xml)|*.xml";
                else
                    d.Filter = "TXT (*.txt)|*.txt";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    if (xml)
                        rootNode.Base.WriteToFileXML(d.FileName);
                    else
                        rootNode.Base.WriteToFileTXT(d.FileName);
                }
            }
        }
    }
}

using CrossArc.GUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CrossArc
{
    public partial class Form1 : Form
    {
        private FolderNode Root = new FolderNode("root");
        private FolderNode Stream = new FolderNode("stream");

        public static int SelectedRegion = 0;

        public static ContextMenu NodeContextMenu
        {
            get
            {
                if (_nodeContextMenu == null)
                {
                    _nodeContextMenu = new ContextMenu();

                    MenuItem Ex = new MenuItem("Extract");
                    Ex.Click += Extract;
                    _nodeContextMenu.MenuItems.Add(Ex);

                    MenuItem Exc = new MenuItem("Extract Compressed");
                    Exc.Click += ExtractCompressed;
                    _nodeContextMenu.MenuItems.Add(Exc);
                }
                return _nodeContextMenu;
            }
        }
        public static ContextMenu _nodeContextMenu;

        public static void Extract(object sender, EventArgs args)
        {
            TreeNode node = Form1.fileTree.SelectedNode;
            if (node == null)
            {
                return;
            }
            if (node is FolderNode foldernode)
            {
                var info = foldernode.GetExtractInformation();

                ExtractProgressBar pb = new ExtractProgressBar();
                pb.Show();
                pb.Extract(info);
            }
            if (node is FileNode filenode)
            {
                var info = filenode.GetExtractInformation(filenode.FullFilePath);

                ExtractProgressBar pb = new ExtractProgressBar();
                pb.Show();
                pb.Extract(info);
            }
        }


        public static void ExtractFolder(object sender, EventArgs args)
        {
            TreeNode node = Form1.fileTree.SelectedNode;
            if (node == null)
            {
                return;
            }
            if (node is FolderNode)
            {
                ((FolderNode)node).ExtractFolder();
            }
            if (node is FileNode)
            {
                ((FileNode)node).ExtractFolder();
            }
        }

        public static void ExtractCompressed(object sender, EventArgs args)
        {
            TreeNode node = Form1.fileTree.SelectedNode;
            if (node == null)
            {
                return;
            }
            if (node is FolderNode foldernode)
            {
                var info = foldernode.GetExtractInformation(true);

                ExtractProgressBar pb = new ExtractProgressBar();
                pb.Show();
                pb.Extract(info);
            }
            if (node is FileNode filenode)
            {
                var info = filenode.GetExtractInformation(filenode.FullFilePath, true);

                ExtractProgressBar pb = new ExtractProgressBar();
                pb.Show();
                pb.Extract(info);
            }
        }

        public static void ExtractCompressedFolder(object sender, EventArgs args)
        {
            TreeNode node = Form1.fileTree.SelectedNode;
            if (node == null)
            {
                return;
            }
            if (node is FolderNode)
            {
                ((FolderNode)node).ExtractFolder();
            }
            if (node is FileNode)
            {
                ((FileNode)node).ExtractFolder();
            }
        }

        public Form1()
        {
            InitializeComponent();

            fileTree.BeforeExpand += folderTree_BeforeExpand;

            fileTree.NodeMouseClick += (sender, args) => fileTree.SelectedNode = args.Node;

            fileTree.HideSelection = false;

            fileTree.ImageList = new ImageList();
            fileTree.ImageList.Images.Add("folder", Properties.Resources.folder);
            fileTree.ImageList.Images.Add("file", Properties.Resources.file);

            regionCB.SelectedIndex = 0;

            // Files
            long TotalSize = 0;
            var timer = new Stopwatch();
            timer.Start();
            if (ARC.FileInformation != null || ARC.FileInformationV2 != null)
            {
                foreach (FileOffsetGroup g in ARC.GetFiles())
                {
                    FolderNode Folder = (FolderNode)GetFolderFromPath(g.Path, Root);
                    if (g.CompSize == null)
                        continue;

                    FileNode fNode = CreateFileNode(g);

                    TotalSize += fNode.DecompSize;

                    if (!Folder.NodesByName.ContainsKey(fNode.Text))
                        Folder.NodesByName.Add(fNode.Text, new List<TreeNode>());
                    Folder.NodesByName[fNode.Text].Add(fNode);
                }
            }

            foreach (var g in ARC.GetStreamFiles())
            {
                string FPath = g.FileName.Replace("stream:/", "");
                FolderNode Folder = (FolderNode)GetFolderFromPath(Path.GetDirectoryName(FPath).Replace("\\", "/"), Stream);

                FileNode fNode = CreateFileNodeFromStreamFile(g, FPath);

                if (!Folder.NodesByName.ContainsKey(fNode.Text))
                    Folder.NodesByName.Add(fNode.Text, new List<TreeNode>());
                Folder.NodesByName[fNode.Text].Add(fNode);
            }

            timer.Stop();
            Debug.WriteLine("To load files into tree: " + timer.ElapsedMilliseconds);
            Debug.WriteLine("Total File Size: " + FormatBytes(TotalSize));

            fileTree.BeginUpdate();
            fileTree.Nodes.Add(Root);
            fileTree.Nodes.Add(Stream);
            fileTree.EndUpdate();
        }

        private static FileNode CreateFileNodeFromStreamFile(FileOffsetGroup g, string FPath)
        {
            FileNode fNode = new FileNode(Path.GetFileName(FPath))
            {
                ArcOffset = g.ArcOffset[0],
                CompSize = (uint)g.CompSize[0],
                DecompSize = (uint)g.DecompSize[0],
                Flags = g.Flags[0],
                FullFilePath = FPath
            };

            if ((g.ArcOffset.Length > 1))
            {
                fNode.IsRegional = true;
                fNode._rArcOffset = g.ArcOffset;
                fNode._rCompSize = g.CompSize;
                fNode._rDecompSize = g.DecompSize;
                fNode._rFlags = g.Flags;
                fNode.FullFilePath = FPath;
            }

            return fNode;
        }

        private static FileNode CreateFileNode(FileOffsetGroup g)
        {
            FileNode fNode = new FileNode(g.FileName)
            {
                ArcOffset = g.ArcOffset[0],
                CompSize = (uint)g.CompSize[0],
                DecompSize = (uint)g.DecompSize[0],
                Flags = g.Flags[0],
                FullFilePath = g.Path + g.FileName
            };

            if ((g.ArcOffset.Length > 1))
            {

                fNode.IsRegional = true;
                fNode._rArcOffset = g.ArcOffset;
                fNode._rCompSize = g.CompSize;
                fNode._rDecompSize = g.DecompSize;
                fNode._rFlags = g.Flags;
                fNode.FullFilePath = g.Path + g.FileName;
            }

            return fNode;
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private void folderTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ((FolderNode)e.Node).BeforeExpand();
        }

        public void DumpWithExtension(string extension)
        {
            var folderNodes = new Queue<FolderNode>();
            foreach (var node in fileTree.Nodes)
            {
                if (node is FolderNode fn)
                    folderNodes.Enqueue(fn);
                if (node is FileNode file)
                {
                    if (file.Text.EndsWith(extension))
                    {
                        file.Extract(false);
                    }
                }
            }

            while (folderNodes.Count > 0)
            {
                var FolderNode = folderNodes.Dequeue();

                foreach (var pair in FolderNode.NodesByName)
                {
                    foreach (var node in pair.Value)
                    {
                        if (node is FolderNode fn)
                            folderNodes.Enqueue(fn);
                        if (node is FileNode file)
                        {
                            if (file.Text.EndsWith(extension))
                            {
                                file.Extract(false);
                            }
                        }
                    }
                }
            }
        }

        public void MKDir(string path)
        {
            string[] levels = path.Split('/');
            var subnodes = Root.NodesByName;
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals(""))
                    continue;
                FolderNode folder = (FolderNode)FindFolder(levels[i], subnodes);
                if (folder == null)
                {
                    folder = new FolderNode(levels[i]);
                    subnodes[folder.Text].Add(folder);
                }
                subnodes = folder.NodesByName;
            }
        }

        public TreeNode GetFolderFromPath(string path, FolderNode root)
        {
            string[] levels = path.Split('/');
            var subnodes = root.NodesByName;

            FolderNode folderNode = null;
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals(""))
                    continue;
                folderNode = (FolderNode)FindFolder(levels[i], subnodes);
                if (folderNode == null)
                {
                    FolderNode newFolder = new FolderNode(levels[i]);
                    if (!subnodes.ContainsKey(newFolder.Text))
                        subnodes.Add(newFolder.Text, new List<TreeNode>());
                    subnodes[newFolder.Text].Add(newFolder);
                    folderNode = newFolder;
                }
                subnodes = folderNode.NodesByName;
            }

            if (folderNode == null)
                folderNode = new FolderNode(path);

            return folderNode;
        }

        public TreeNode FindFolder(string name, Dictionary<string, List<TreeNode>> nodes)
        {
            // Ignore any duplicate names.
            // This gets called a lot, so we'll take advantage of the dictionary lookup speed.
            if (nodes.TryGetValue(name, out List<TreeNode> values) && values[0] is FolderNode)
                return values[0];
            else
                return null;
        }

        private void fileTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            offLabel.Text = "";
            compLabel.Text = "";
            decompLabel.Text = "";
            flagLabel.Text = "";
            if (fileTree.SelectedNode != null && fileTree.SelectedNode is FileNode n)
            {
                offLabel.Text = "0x" + n.ArcOffset.ToString("X");
                compLabel.Text = "0x" + n.CompSize.ToString("X");
                decompLabel.Text = "0x" + n.DecompSize.ToString("X");
                flagLabel.Text = "0x" + n.Flags.ToString("X");

                if (n.IsRegional)
                {
                    if (regionCB.SelectedIndex == 14)
                    {
                        offLabel.Text = "0x0";
                        compLabel.Text = "0x0";
                        decompLabel.Text = "0x0";
                        flagLabel.Text = "0x0";
                        return;
                    }
                    if (regionCB.SelectedIndex == -1)
                        regionCB.SelectedIndex = 0;

                    offLabel.Text = "0x" + n._rArcOffset[regionCB.SelectedIndex].ToString("X");
                    compLabel.Text = "0x" + n._rCompSize[regionCB.SelectedIndex].ToString("X");
                    decompLabel.Text = "0x" + n._rDecompSize[regionCB.SelectedIndex].ToString("X");
                    flagLabel.Text = "0x" + n._rFlags[regionCB.SelectedIndex].ToString("X");
                }
            }
        }

        private void regionCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedRegion = regionCB.SelectedIndex;
            if (fileTree.SelectedNode != null && fileTree.SelectedNode is FileNode n)
            {
                fileTree_AfterSelect(null, null);
            }
        }
    }
}

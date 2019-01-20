using System;
using System.Windows.Forms;
using CrossArc.GUI;
using CrossArc.Structs;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CrossArc
{
    public partial class Form1 : Form
    {
        FolderNode Root = new FolderNode("root");
        FolderNode BGM = new FolderNode("music");

        public static int SelectedRegion = 0;

        public static ContextMenu NodeContextMenu
        {
            get
            {
                if(_nodeContextMenu == null)
                {
                    _nodeContextMenu = new ContextMenu();

                    MenuItem Ex = new MenuItem("Extract");
                    Ex.Click += Extract;
                    _nodeContextMenu.MenuItems.Add(Ex);

                    /*MenuItem ExF = new MenuItem("Extract To Folder");
                    ExF.Click += ExtractFolder;
                    _nodeContextMenu.MenuItems.Add(ExF);*/

                    MenuItem Exc = new MenuItem("Extract Compressed");
                    Exc.Click += ExtractCompressed;
                    _nodeContextMenu.MenuItems.Add(Exc);

                    /*MenuItem ExcF = new MenuItem("Extract Compressed To Folder");
                    ExcF.Click += ExtractCompressedFolder;
                    _nodeContextMenu.MenuItems.Add(ExcF);*/
                }
                return _nodeContextMenu;
            }
        }
        public static ContextMenu _nodeContextMenu;

        public static void Extract(object sender, EventArgs args)
        {
            TreeNode node = Form1.fileTree.SelectedNode;
            if(node == null)
            {
                return;
            }
            if(node is FolderNode foldernode)
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

            //initialize arc

            // BGM
            /*foreach(_sBGMOffset off in ARC.BGMOffsets)
            {
                BGM.Nodes.Add(new FileNode(off.Offset.ToString("X"))
                {
                    ArcOffset = off.Offset,
                    DecompSize = (uint)off.Size,
                    CompSize = (uint)off.Size
                });
            }*/

            // Files
            var timer = new Stopwatch();
            timer.Start();
            if (ARC.FileInformation != null)
                foreach (FileOffsetGroup g in ARC.GetFiles())
                {
                    FolderNode Folder = (FolderNode)GetFolderFromPath(g.Path);

                    FileNode fNode = new FileNode(g.FileName)
                    {
                        ArcOffset = g.ArcOffset[0],
                        CompSize = (uint)g.CompSize[0],
                        DecompSize = (uint)g.DecompSize[0],
                        Flags = g.Flags[0],//(uint)g.SubFileInformationOffset
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
                    
                    Folder.SubNodes.Add(fNode);
                }
            timer.Stop();
            Debug.WriteLine("To load files into tree: " + timer.ElapsedMilliseconds);

            /*string[] folders = ARC.GetPaths();
            foreach(string s in folders)
            {
                //fileTree.Nodes.Add(new TreeNode(s));
                MKDir(s);
            }*/

            //update
            /*foreach (_sDirectoryList h in ARC.DirectoryLists)
            {
                _sFileInformation[] fi = ARC.GetFileInformationFromFolder(h);
                string path = ARC.GetPathString(ARC.FolderHashDict, h);
                foreach (_sFileInformation i in fi)
                {
                    //MKDir(path + "/" + ARC.GetName(i.Unk4));
                    TreeNode Folder = GetFolderFromPath(path + "/" + ARC.GetName(i.Parent));
                    //_SubFileInfo sub = ARC.SubFileInformation_1[i.SubFile1_Index];
                    ARC.FileOffsetGroup group = ARC.GetOffsetFromSubFile(i);
                    
                    Folder.Nodes.Add(new FileNode(ARC.GetName(i.Hash2))
                    {
                        ArcOffset = group.ArcOffset,
                        CompSize = (uint)group.CompSize,
                        DecompSize = (uint)group.DecompSize,
                        Flags = group.Flags
                    });
                }
            }*/
            
            fileTree.BeginUpdate();
            fileTree.Nodes.Add(Root);
            fileTree.Nodes.Add(BGM);
            fileTree.EndUpdate();

            //DumpWithExtension(".nuanmb");
        }

        private void folderTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ((FolderNode)e.Node).BeforeExpand();
        }

        public void DumpWithExtension(string Extension)
        {
            Queue<FolderNode> FolderNodes = new Queue<FolderNode>();
            foreach(var node in fileTree.Nodes)
            {
                if (node is FolderNode fn)
                    FolderNodes.Enqueue(fn);
                if (node is FileNode file)
                    if (file.Text.EndsWith(Extension))
                    {
                        file.Extract(false);
                    }
            }

            while(FolderNodes.Count > 0)
            {
                var FolderNode = FolderNodes.Dequeue();

                foreach(var node in FolderNode.SubNodes)
                {
                    if (node is FolderNode fn)
                        FolderNodes.Enqueue(fn);
                    if (node is FileNode file)
                        if (file.Text.EndsWith(Extension))
                        {
                            file.Extract(false);
                        }
                }
            }
        }
        
        public void MKDir(string path)
        {
            string[] levels = path.Split('/');
            var Level = Root.SubNodes;
            for(int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals("")) continue;
                FolderNode folder = (FolderNode)FindFolder(levels[i], Level);
                if(folder == null)
                {
                    folder = new FolderNode(levels[i]);
                    Level.Add(folder);
                }
                Level = folder.SubNodes;
            }
        }

        public TreeNode GetFolderFromPath(string path)
        {
            string[] levels = path.Split('/');
            var Level = Root.SubNodes;
            FolderNode Node = null;
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals("")) continue;
                Node = (FolderNode)FindFolder(levels[i], Level);
                if (Node == null)
                {
                    FolderNode newFolder = new FolderNode(levels[i]);
                    Level.Add(newFolder);
                    Node = newFolder;
                }
                Level = Node.SubNodes;
            }
            return Node;
        }

        public TreeNode FindFolder(string name, List<TreeNode> Nodes)
        {
            foreach(TreeNode tn in Nodes)
            {
                if (tn.Text.Equals(name))
                    return tn;
            }
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
                //regionCB.Visible = false;
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
                    //regionCB.Visible = true;
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

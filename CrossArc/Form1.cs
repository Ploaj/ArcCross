using System;
using System.Windows.Forms;
using CrossArc.GUI;
using CrossArc.Structs;

namespace CrossArc
{
    public partial class Form1 : Form
    {
        FolderNode Root = new FolderNode("root");
        FolderNode BGM = new FolderNode("music");

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

                    MenuItem ExF = new MenuItem("Extract To Folder");
                    ExF.Click += ExtractFolder;
                    _nodeContextMenu.MenuItems.Add(ExF);
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
            if(node is FolderNode)
            {
                ((FolderNode)node).Extract();
            }
            if (node is FileNode)
            {
                ((FileNode)node).Extract();
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

        public Form1()
        {
            InitializeComponent();

            fileTree.NodeMouseClick += (sender, args) => fileTree.SelectedNode = args.Node;

            fileTree.ImageList = new ImageList();
            fileTree.ImageList.Images.Add("folder", Properties.Resources.folder);
            fileTree.ImageList.Images.Add("file", Properties.Resources.file);

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
            if (ARC.FileInformation != null)
                foreach (ARC.FileOffsetGroup g in ARC.GetFiles())
                {
                    TreeNode Folder = GetFolderFromPath(g.Path);

                    FileNode fNode = new FileNode(g.FileName)
                    {
                        ArcOffset = g.ArcOffset[0],
                        CompSize = (uint)g.CompSize[0],
                        DecompSize = (uint)g.DecompSize[0],
                        Flags = g.Flags[0]
                    };

                    if ((g.ArcOffset.Length > 1))
                    {

                        fNode.IsRegional = true;
                        fNode._rArcOffset = g.ArcOffset;
                        fNode._rCompSize = g.CompSize;
                        fNode._rDecompSize = g.DecompSize;
                        fNode._rFlags = g.Flags;
                    }
                    
                    Folder.Nodes.Add(fNode);
                }


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
        }
        
        public void MKDir(string path)
        {
            string[] levels = path.Split('/');
            TreeNodeCollection Level = Root.Nodes;
            for(int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals("")) continue;
                TreeNode folder = FindFolder(levels[i], Level);
                if(folder == null)
                {
                    folder = new FolderNode(levels[i]);
                    Level.Add(folder);
                }
                Level = folder.Nodes;
            }
        }

        public TreeNode GetFolderFromPath(string path)
        {
            string[] levels = path.Split('/');
            TreeNodeCollection Level = Root.Nodes;
            TreeNode Node = null;
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].Equals("")) continue;
                Node = FindFolder(levels[i], Level);
                if (Node == null)
                {
                    FolderNode newFolder = new FolderNode(levels[i]);
                    Level.Add(newFolder);
                    Node = newFolder;
                }
                Level = Node.Nodes;
            }
            return Node;
        }

        public TreeNode FindFolder(string name, TreeNodeCollection Nodes)
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
                regionCB.Visible = false;
                offLabel.Text = "0x" + n.ArcOffset.ToString("X");
                compLabel.Text = "0x" + n.CompSize.ToString("X");
                decompLabel.Text = "0x" + n.DecompSize.ToString("X");
                flagLabel.Text = "0x" + n.Flags.ToString("X");

                if (n.IsRegional)
                {
                    if (regionCB.SelectedIndex == -1)
                        regionCB.SelectedIndex = 0;
                    regionCB.Visible = true;
                    offLabel.Text = "0x" + n._rArcOffset[regionCB.SelectedIndex].ToString("X");
                    compLabel.Text = "0x" + n._rCompSize[regionCB.SelectedIndex].ToString("X");
                    decompLabel.Text = "0x" + n._rDecompSize[regionCB.SelectedIndex].ToString("X");
                    flagLabel.Text = "0x" + n._rFlags[regionCB.SelectedIndex].ToString("X");
                }
            }
        }

        public static int SelectedRegion = -1;

        private void regionCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileTree.SelectedNode != null && fileTree.SelectedNode is FileNode n)
            {
                if(SelectedRegion != regionCB.SelectedIndex)
                    fileTree_AfterSelect(null, null);
                SelectedRegion = regionCB.SelectedIndex;
            }
        }
    }
}

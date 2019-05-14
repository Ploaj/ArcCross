using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace CrossArc.GUI
{

    public class FolderMenuItem : MenuItem
    {
        public FolderNode Node;
    }

    public class FolderNode : TreeNode
    {
        // Some files may have the same name, so we need to store a list of nodes.
        public Dictionary<string, List<TreeNode>> NodesByName { get; } = new Dictionary<string, List<TreeNode>>();

        public FolderNode(string text)
        {
            Text = text;
            ContextMenu = Form1.NodeContextMenu;
            AfterCollapse();
        }


        public void BeforeExpand()
        {
            if (IsExpanded) return;
            Nodes.Clear();
            var sortedNodes = new List<TreeNode>();
            foreach (var pair in NodesByName)
            {
                sortedNodes.AddRange(pair.Value.ToArray());
            }
            Nodes.AddRange(sortedNodes.OrderBy(f => f.Text).ToArray());
        }

        public void AfterCollapse()
        {
            Nodes.Clear();
            Nodes.Add(new TreeNode("Dummy"));
        }

        public ArcExtractInformation[] GetExtractInformation(bool compressed = false)
        {
            Queue<TreeNode> NodeList = new Queue<TreeNode>();
            List<ArcExtractInformation> info = new List<ArcExtractInformation>();

            foreach (var pair in NodesByName)
            {
                foreach (TreeNode n in pair.Value)
                {
                    NodeList.Enqueue(n);
                }
            }

            while (NodeList.Count > 0)
            {
                TreeNode n = NodeList.Dequeue();
                if (n is FileNode fileNode)
                {
                    info.AddRange(fileNode.GetExtractInformation(fileNode.FullFilePath, compressed));
                }
                else
                {
                    foreach (var pair in ((FolderNode)n).NodesByName)
                    {
                        foreach (TreeNode child in pair.Value)
                        {
                            NodeList.Enqueue(child);
                        }
                    }

                }
            }
            return info.ToArray();
        }

        public void Extract(bool compressed = false)
        {
            Queue<TreeNode> NodeList = new Queue<TreeNode>();

            foreach (TreeNode n in Nodes)
            {
                NodeList.Enqueue(n);
            }
            while (NodeList.Count > 0)
            {
                TreeNode n = NodeList.Dequeue();
                if (n is FileNode)
                {
                    ((FileNode)n).Extract(compressed);
                }
                else
                {
                    foreach (TreeNode child in n.Nodes)
                        NodeList.Enqueue(child);
                }
            }
        }
        public void ExtractFolder(bool compressed = false)
        {
            using (SaveFileDialog d = new SaveFileDialog())
            {
                d.FileName = "choose location to save to";

                if (d.ShowDialog() == DialogResult.OK)
                {
                    foreach(TreeNode f in Nodes)
                    {
                        if(f is FileNode)
                        {
                            ((FileNode)f).SaveFile(Path.GetDirectoryName(d.FileName) + "/" + f.Text, compressed);
                        }
                    }
                }
            }
        }
    }
}

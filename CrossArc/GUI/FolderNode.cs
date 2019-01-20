using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<TreeNode> SubNodes = new List<TreeNode>();

        public FolderNode(string text)
        {
            this.Text = text;
            ContextMenu = Form1.NodeContextMenu;
            AfterCollapse();
        }


        public void BeforeExpand()
        {
            if (IsExpanded) return;
            Nodes.Clear();
            Nodes.AddRange(SubNodes.OrderBy(f => f.Text).ToArray());
        }

        public void AfterCollapse()
        {
            Nodes.Clear();
            Nodes.Add(new TreeNode("Dummy"));
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

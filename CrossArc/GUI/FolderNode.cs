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
        public FolderNode(string text)
        {
            this.Text = text;
            ContextMenu = Form1.NodeContextMenu;
        }

        public void Extract()
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
                    ((FileNode)n).Extract();
                }
                else
                {
                    foreach (TreeNode child in n.Nodes)
                        NodeList.Enqueue(child);
                }
            }
        }
        public void ExtractFolder()
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
                            ((FileNode)f).SaveFile(Path.GetDirectoryName(d.FileName) + "/" + f.Text);
                        }
                    }
                }
            }
        }
    }
}

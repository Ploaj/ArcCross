using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CrossArc.GUI.Nodes
{
    public class FolderNode : BaseNode
    {
        public BaseNode[] Children
        {
            get
            {
                return SubNodes.ToArray();
            }
        }

        public FolderNode(string Text)
        {
            this.Text = Text;
        }

        public void AddChild(BaseNode node)
        {
            if(node is BaseNode file)
            {
                file.ParentFolder = this;
            }
        }

        public void RemoveChild(BaseNode node)
        {
            if (SubNodes.Contains(node))
                SubNodes.Remove(node);
        }

        public FileNode[] GetAllFiles()
        {
            List<FileNode> files = new List<FileNode>();
            GetChildren(files);
            return files.ToArray();
        }

        private void GetChildren(List<FileNode> nodes)
        {
            foreach(var node in SubNodes)
            {
                if(node is FileNode file)
                    nodes.Add(file);
                if (node is FolderNode folder)
                    folder.GetChildren(nodes);
            }
        }

        
    }
}

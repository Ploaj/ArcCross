using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CrossArc.GUI.Nodes
{
    public class BaseNode
    {
        public string Text { get; set; }

        public string FullPath
        {
            get
            {
                if(ParentFolder != null)
                    return ParentFolder.FullPath + "/" + Text;

                return Text;
            }
        }

        public List<BaseNode> SubNodes = new List<BaseNode>();

        public FolderNode ParentFolder
        {
            get
            {
                return _parentFolder;
            }
            set
            {
                if (_parentFolder != null)
                    _parentFolder.SubNodes.Remove(this);
                _parentFolder = value;
                if (_parentFolder != null)
                    _parentFolder.SubNodes.Add(this);
            }
        }
        private FolderNode _parentFolder;
    }
}

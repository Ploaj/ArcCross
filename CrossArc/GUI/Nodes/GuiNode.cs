using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CrossArc.GUI.Nodes
{
    public class GuiNode : TreeNode
    {
        public BaseNode Base { get; set; }

        private bool Reordered = false;

        public GuiNode(BaseNode baseNode)
        {
            Base = baseNode;
            Text = Base.Text;
            if (baseNode is FileNode file)
            {
                ImageKey = "file";
                SelectedImageKey = "file";
                if (MainForm.ArcFile.IsRegional(file.ArcPath))
                    ForeColor = Color.DarkOliveGreen;
                if (MainForm.ArcFile.IsRedirected(file.ArcPath))
                    ForeColor = Color.DarkBlue;
                if(MainForm.ArcFile.IsRegional(file.ArcPath) && MainForm.ArcFile.IsRedirected(file.ArcPath))
                    ForeColor = Color.Purple;

            }
            if (baseNode is FolderNode folder)
            {
                ImageKey = "folder";
                SelectedImageKey = "folder";
                AfterCollapse();
            }
            ContextMenu = MainForm.NodeContextMenu;
        }

        public void BeforeExpand()
        {
            if (IsExpanded) return;
            if (!Reordered)
            {
                Base.SubNodes = Base.SubNodes.GroupBy(x => x.Text).Select(x => x.First()).ToList();
                Base.SubNodes = Base.SubNodes.OrderBy(f => f.Text).ToList();
                Reordered = true;
            }
            Nodes.Clear();
            foreach(var v in Base.SubNodes)
            {
                Nodes.Add(new GuiNode(v));
            }
        }

        public void AfterCollapse()
        {
            Nodes.Clear();
            Nodes.Add(new TreeNode("Dummy"));
        }
    }
}

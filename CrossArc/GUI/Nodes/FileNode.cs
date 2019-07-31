namespace CrossArc.GUI.Nodes
{
    public class FileNode : BaseNode
    {
        public string ArcPath
        {
            get
            {
                return FullPath.Replace("\\", "/").Replace("root/", "");
            }
        }
        
        public FileInformation FileInformation { get { return new FileInformation(ArcPath); } }

        public FileNode(string name)
        {
            Text = name;
        }
    }
}

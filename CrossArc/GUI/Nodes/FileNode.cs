namespace CrossArc.GUI.Nodes
{
    public class FileNode : BaseNode
    {
        public string ArcPath
        {
            get
            {
                var path = FullPath.Replace("\\", "/");

                if(path.StartsWith("root/"))
                    path = path.Substring(path.IndexOf("root/") + "root/".Length);

                return path;
            }
        }
        
        public FileInformation FileInformation { get { return new FileInformation(ArcPath); } }

        public FileNode(string name)
        {
            Text = name;
        }
    }
}

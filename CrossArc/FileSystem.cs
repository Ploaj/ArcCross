using CrossArc.GUI.Nodes;
using System.Collections.Generic;

namespace CrossArc
{
    public static class FileSystem
    {
        public static FolderNode CreateFileTreeGetRoot(IEnumerable<string> filePaths, IEnumerable<string> streamFilePaths)
        {
            FolderNode root = new FolderNode("root");

            foreach (var file in filePaths)
            {
                string[] path = file.Split('/');
                ProcessFile(root, path, 0);
            }

            foreach (var file in streamFilePaths)
            {
                string[] path = file.Split('/');
                ProcessFile(root, path, 0);
            }

            return root;
        }

        private static void ProcessFile(FolderNode parent, string[] path, int index)
        {
            string currentPath = path[index];

            // The last part of the path should be the filename.
            if (path.Length - 1 == index)
            {
                var fileNode = new FileNode(currentPath);
                parent.AddChild(fileNode);
                return;
            }

            // Check if the current folder exists to prevent duplicates.
            var node = FindFolderNode(parent, currentPath);
            if (node == null)
            {
                node = new FolderNode(currentPath);
                parent.AddChild(node);
            }

            ProcessFile(node, path, index + 1);
        }

        private static FolderNode FindFolderNode(FolderNode parent, string path)
        {
            for (int i = 0; i < parent.SubNodes.Count; i++)
            {
                var child = parent.SubNodes[i];
                if (child.Text.Equals(path))
                {
                    return (FolderNode)child;
                }
            }

            return null;
        }
    }
}

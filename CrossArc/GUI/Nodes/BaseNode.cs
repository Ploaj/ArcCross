using System.Collections.Generic;
using System.IO;
using System.Xml;

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
        
        public List<BaseNode> SubNodes { get; set; } = new List<BaseNode>();
        
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

        public void WriteToFileXML(string filePath)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(new FileStream(filePath, FileMode.Create), settings))
            {
                writer.WriteStartDocument();
                WriteToFileXML(writer);
                writer.WriteEndDocument();
            }
        }

        private void WriteToFileXML(XmlWriter writer)
        {
            if (this is FileNode file)
            {
                var info = file.FileInformation;

                writer.WriteStartElement("file");
                writer.WriteAttributeString("name", Text);
                writer.WriteAttributeString("offset", info.ArcOffset);

                if(info.CompressedSize == info.DecompressedSize)
                {
                    writer.WriteAttributeString("size", info.CompressedSize.ToString("X"));
                }
                else
                {
                    writer.WriteAttributeString("compsize", info.CompressedSize.ToString("X"));
                    writer.WriteAttributeString("decompsize", info.DecompressedSize.ToString("X"));
                }

                if (info.region)
                {
                    for(int i = 0; i < 14; i++)
                    {
                        info = new FileInformation(file.ArcPath, i);

                        writer.WriteStartElement("regional_offset");
                        writer.WriteAttributeString("tag", ProgressBar.RegionTags[i]);
                        writer.WriteAttributeString("offset", info.ArcOffset);

                        if (info.CompressedSize == info.DecompressedSize)
                        {
                            writer.WriteAttributeString("size", info.CompressedSize.ToString("X"));
                        }
                        else
                        {
                            writer.WriteAttributeString("compsize", info.CompressedSize.ToString("X"));
                            writer.WriteAttributeString("decompsize", info.DecompressedSize.ToString("X"));
                        }

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
            if (GetType() == typeof(FolderNode))
            {
                writer.WriteStartElement("folder");
                writer.WriteAttributeString("name", Text);

                foreach (var s in SubNodes)
                    s.WriteToFileXML(writer);

                writer.WriteEndElement();
            }
        }

        public void WriteToFileTXT(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                WriteToFileTXT(writer);
            }
        }

        private void WriteToFileTXT(StreamWriter writer)
        {
            if (this is FileNode file)
            {
                var info = file.FileInformation;
                if(info.region)
                {
                    for(int i = 0; i < 14; i++)
                    {
                        info = new FileInformation(file.ArcPath, i);

                        if (info.CompressedSize == info.DecompressedSize)
                            writer.WriteLine($"{ProgressBar.GetRegionalPath(FullPath)} | Offset: {info.ArcOffset} Size: {info.CompressedSize.ToString("X")}");
                        else
                            writer.WriteLine($"{ProgressBar.GetRegionalPath(FullPath)} | Offset: {info.ArcOffset} CompSize: {info.CompressedSize.ToString("X")} DecompSize: {info.DecompressedSize.ToString("X")}");

                    }
                }
                else
                {
                    if (info.CompressedSize == info.DecompressedSize)
                        writer.WriteLine($"{FullPath} | Offset: {info.ArcOffset} Size: {info.CompressedSize.ToString("X")}");
                    else
                        writer.WriteLine($"{FullPath} | Offset: {info.ArcOffset} CompSize: 0x{info.CompressedSize.ToString("X")} DecompSize: 0x{info.DecompressedSize.ToString("X")}");

                }
            }
            if (GetType() == typeof(FolderNode))
            {
                foreach (var s in SubNodes)
                    s.WriteToFileTXT(writer);
            }
        }
    }
}

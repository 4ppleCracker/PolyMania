using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public class FileObjectStorage<T>
{
    struct FileObject
    {
        public FileInfo path;
        public string name;
    }

    List<FileObject> Files = new List<FileObject>();
    public string ObjectPath;

    public FileObjectStorage(string objectPath)
    {
        ObjectPath = objectPath;
    }

    public T GetObject(string name)
    {
        foreach(FileObject obj in Files)
        {
            if(obj.name == name)
            {
                if (!obj.path.Exists)
                    break;

                XmlSerializer writer = new XmlSerializer(typeof(T));

                using (FileStream file = File.Open(obj.path.FullName, FileMode.Open))
                {
                    return (T)writer.Deserialize(file);
                }
            }
        }

        throw new FileNotFoundException("Couldn't find a file of name " + name);
    }
    public void AddObject(string name, T obj)
    {
        XmlSerializer writer = new XmlSerializer(typeof(T));

        var path = Path.Combine(ObjectPath, name);
        using (FileStream file = File.Create(path))
        {
            writer.Serialize(file, obj);

            Files.Add(new FileObject() { path = new FileInfo(path), name = name });
        }
    }
    public void DeleteObject(string name)
    {
        foreach (FileObject obj in Files)
        {
            if (obj.name == name)
            {
                if (!obj.path.Exists)
                    break;

                File.Delete(obj.path.FullName);
            }
        }
    }
}

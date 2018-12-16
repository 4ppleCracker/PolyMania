using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class FileObjectStorageTest {
    [Test]
    public void FileObjectStorageStoreGetDeleteTest() {
        string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "FileObjectStorageTest");
        System.IO.Directory.CreateDirectory(tempPath);

        FileObjectStorage<int> storage = new FileObjectStorage<int>(tempPath);

        storage.AddObject("one", 1);
        storage.AddObject("two", 2);

        Assert.AreEqual(1, storage.GetObject("one"));
        Assert.AreEqual(2, storage.GetObject("two"));

        storage.DeleteObject("one");
        storage.DeleteObject("two");

        bool deleteSuccess = false;
        try
        {
            storage.GetObject("one");
            storage.GetObject("two");
        }
        catch
        {
            deleteSuccess = true;
        }
        Assert.AreEqual(true, deleteSuccess);

        System.IO.Directory.Delete(tempPath);
    }
}

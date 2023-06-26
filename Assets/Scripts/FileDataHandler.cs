using System;
using System.IO;
using UnityEngine;

public static class FileDataHandler
{
    public static Levels Load(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName; 
        Levels loadedData = new Levels();
        if(File.Exists(filePath))
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<Levels>(dataToLoad);

            }catch (Exception e)
            {
                Debug.LogError("Error  occured when trying to load data from file:" + filePath + "\n" + e);
            }
        }
        return loadedData;
    }
}

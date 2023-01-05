using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem
{
    public static void SavePlayer(Player_System playerSystem)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/templar.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        DataToSave data = new DataToSave(playerSystem);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static DataToSave LoadPlayer()
    {
        string path = Application.persistentDataPath + "/templar.save";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DataToSave data = formatter.Deserialize(stream) as DataToSave;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file doens't exist in " + path);
            return null;
        }
    }
}

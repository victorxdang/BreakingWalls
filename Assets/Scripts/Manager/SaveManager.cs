
/*****************************************************************************************************************
 - SaveManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Local saving and loading of the player's data files happens here. This class will persist and there will
     only be one instance of this class per game.
*****************************************************************************************************************/

using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Constants

    /// <summary>
    /// The name of the save file. Can be changed as needed, but only change the
    /// name of the file (anything before the ".json").
    /// </summary>
    const string SAVE_FILE = "wbSave.json";

    #endregion


    [SerializeField] soGameSettings soGameSettings;


    /// <summary>
    /// The path to where the save file will be saved to. This particular save file will be a json file.
    /// </summary>
    public static string SavePath
    {
        get { return Application.persistentDataPath + "/" + SAVE_FILE; }
    }

    /// <summary>
    /// The highest score the player has achieved that was stored on the cloud.
    /// </summary>
    public static int CloudRecordSave { get; set; }


    static soGameSettings gameSettings;


    
    void Awake()
    {
        if (FindObjectsOfType<SaveManager>().Length == 1)
        {
            gameSettings = soGameSettings;

            GooglePlayGamesService.OnCloudDataLoaded += (cloudScore) =>
            {
                CloudRecordSave = cloudScore;

                // check to make sure that both the local high score is equal to the cloud's high score, if not
                // then set which ever is the lower score to the higher score
                if (CloudRecordSave < gameSettings.wallsDestroyedRecord)
                    CloudRecordSave = gameSettings.wallsDestroyedRecord;
                else
                    gameSettings.wallsDestroyedRecord = CloudRecordSave;
            };

            Load();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #region Local Save/Load

    public static bool SaveCloud()
    {
        return GooglePlayGamesService.CloudSave(CloudRecordSave);
    }

    public static bool SaveLocal()
    {
        return Save(gameSettings.SerializeSaveData(), SavePath);
    }

    public static bool Load()
    {
        bool successful = Load(out SaveInfo data, SavePath);
        gameSettings.DeserializeSaveData(successful ? data : new SaveInfo());

        return successful;
    }


    /// <summary>
    /// Saves the json file to a particular path. Returns true if successful,
    /// false if an exception occured.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="save"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Save<T>(T save, string path)
    {
        try
        {
            File.WriteAllText(path, JsonUtility.ToJson(save));
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Loads the json file from a particular path. Returns true if successful,
    /// false if an exception occured or the save file doesn't exist. On false,
    /// the method will return a default value for T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="save"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Load<T>(out T save, string path)
    {
        try
        {
            if (File.Exists(path))
            {
                save = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return true;
            }

            save = default;
            return false;
        }
        catch
        {
            save = default;
            return false;
        }
    }

    #endregion
}

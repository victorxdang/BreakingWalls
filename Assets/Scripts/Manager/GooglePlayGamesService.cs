
/*****************************************************************************************************************
 - GooglePlayGameServices.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class handles all work that will be required to have a Google Play Games Service (GPGS) leaderboard, 
     achievements and save games to the cloud.
*****************************************************************************************************************/

using System;
using System.Text;
using UnityEngine;

// Google API's
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class GooglePlayGamesService : MonoBehaviour
{
    public static Action<bool> OnSignedIn;
    public static Action<int> OnCloudDataLoaded;
    public static Action OnCloudDataSaved;


    [SerializeField] soGPGSSettings gpgsSettings;

    static soGPGSSettings gpgs;
    static ISavedGameMetadata CurrentGameMetaData;



    /// <summary>
    /// Sets up the class.
    /// </summary>
    void Start()
    {
        if (FindObjectsOfType<GooglePlayGamesService>().Length == 1)
        {
            gpgs = gpgsSettings;

            // setup buttons and configure Google Play Games Service if enabled, else
            // deactivate all buttons related to Google Play Games
            if (gpgs.EnablePlayServices && GameUtilities.IsAndroid)
            {
                GPGSConfiguration();
                CloudLoad(gpgs.CloudSaveFilename); // attempt to load the data from the cloud
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Removes all method pointers.
    /// </summary>
    void OnDestroy()
    {
        OnSignedIn = null;
        OnCloudDataSaved = null;
        OnCloudDataLoaded = null;
    }


    #region Sign-In/Setup For Google Play Games Service

    /// <summary>
    /// Activate Google Play Games Service to allow signing into Google Play to access achievements, leaderboards
    /// and to save the player's progress.
    /// </summary>
    static void GPGSConfiguration()
    {
        // GPGS configurations
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        // attempt silent sign-in
        PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
    }

    /// <summary>
    /// Called whenever the users presses the sign-in button from either the start menu or the settings menu.
    /// </summary>
    public static void SignIn()
    {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            PlayGamesPlatform.Instance.SignOut();
            OnSignedIn?.Invoke(false);
        }
    }

    /// <summary>
    /// Callback function for when the sign-in button is clicked, or when the game automatically
    /// signs in the player.
    /// </summary>
    /// <param name="success"></param>
    static void SignInCallback(bool success)
    {
        // unlock achievement if the player signs in for the first time
        if (success)
            UnlockAchievement(GPGSIds.achievement_welcome);

        OnSignedIn?.Invoke(success);
    }

    #endregion


    #region Achievement

    /// <summary>
    /// Unlocks an achievement with the given id. Do not use this method for incremental achievements.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool UnlockAchievement(string id)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && gpgs.EnablePlayServices)
        {
            bool unlocked = false;
            PlayGamesPlatform.Instance.ReportProgress(id, 100, success => { unlocked = success; });
            return unlocked;
        }

        return false;
    }

    /// <summary>
    /// Increments an achievement with the given id by a certain number of steps. Exclusive to GPGS.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="steps"></param>
    /// <returns></returns>
    public static bool IncrementAchivement(string id, int steps)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && gpgs.EnablePlayServices)
        {
            bool incremented = false;
            PlayGamesPlatform.Instance.IncrementAchievement(id, steps, success => { incremented = success; });
            return incremented;
        }

        return false;
    }

    /// <summary>
    /// Displays Google Play Platform achievement UI.
    /// </summary>
    public static void ShowAchievementsUI()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        } 
    }

    #endregion


    #region Leaderboard

    /// <summary>
    /// Reports a score to the Google Play Platform leaderboard with the given id leaderboardID.
    /// </summary>
    /// <param name="leaderboardID"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    public static bool AddScoreToLeaderboard(string leaderboardID, long score)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated && gpgs.EnablePlayServices)
        {
            bool added = false;
            PlayGamesPlatform.Instance.ReportScore(score, leaderboardID, success => { added = success; });
            return added;
        }

        return false;
    }

    /// <summary>
    /// Displays Google Play Platform leaderboard UI.
    /// </summary>
    public static void ShowLeaderboardUI()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowLeaderboardUI();
        } 
    }

    #endregion


    #region Cloud Save/Load

    /// <summary>
    /// Saves the game to the Google Play cloud.
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static bool CloudLoad(string filename)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // if there are conflicts with saving the game (i.e. two devices trying to save at the same time), then 
            // the save file with the longest playing time will be the one that is saved to the cloud.
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, HandleCloudLoad);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Saves the reported score to the cloud. This is a helper function for CloudSave() below which will actually
    /// save the data. This just takes the integer and converts it into an array of bytes.
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static bool CloudSave(int score)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            return CloudSave(CurrentGameMetaData, Encoding.UTF8.GetBytes(score.ToString()));
        }

        return false;
    }

    /// <summary>
    /// Saves the data to the Google Play cloud. Keeps record of the time and date of when the data was saved.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="savedData"></param>
    /// <returns></returns>
    public static bool CloudSave(ISavedGameMetadata game, byte[] savedData)
    {
        try
        {
            // update the file to include the date and time that it was saved
            SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
                .WithUpdatedDescription("Saved at: " + DateTime.Now);

            SavedGameMetadataUpdate updatedMetadata = builder.Build();
            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(game, updatedMetadata, savedData, HandleCloudSave);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    /// <summary>
    /// Callback function for when the data has been loaded from the cloud.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="game"></param>
    static void HandleCloudLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            CurrentGameMetaData = game;
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, HandleCloudBinaryLoad);
        }
    }

    /// <summary>
    /// This function will take the data from the cloud and convert it into the correct data type suported
    /// by the game. Which in this case will be an integer becaue the only data saved onto the cloud is
    /// the player's highest score obtained.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="data"></param>
    static void HandleCloudBinaryLoad(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            try
            {
                int cloudScore = Convert.ToInt32(Encoding.UTF8.GetString(data));

                AddScoreToLeaderboard(GPGSIds.leaderboard_walls_destroyed, cloudScore);
                OnCloudDataLoaded?.Invoke(cloudScore);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    /// <summary>
    /// Call back function for whenever the game has saved the data. Nothing happens here but it is here for
    /// future implementations.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="game"></param>
    static void HandleCloudSave(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        OnCloudDataSaved?.Invoke();
    }

    #endregion
}

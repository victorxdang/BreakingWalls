
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/SO Game Data")]
public class soGameSettings : ScriptableObject
{
    // Make sure that platforms does not go faster than this hardcap because 
    // the platforms would then move faster than it can spawn in effectively
    // cause the fall to fall off since there are no platforms for the ball
    // to stay on top of so it is recommended to cap the speed at 50 units/second 
    //
    // so in short: DO NOT MAKE THE PLATFORMS GO FASTER THAN 50 UNITS/SECOND! 
    public const float PLATFORM_SPEED_HARDCAP = 50;


    [Header("Starting Values:")]
    [SerializeField] float startingPlatformSpeed = 15;

    /// <summary>
    /// The name of the main scene.
    /// </summary>
    public string mainSceneName = "MainScene";


    [Header("Persistent Data:")]

    /// <summary>
    /// Has the game started yet?
    /// </summary>
    public bool gameStarted = false;

    /// <summary>
    /// Is the game currently paused?
    /// </summary>
    public bool gamePaused = false;

    /// <summary>
    /// Is the game currently over and on the game over screen?
    /// </summary>
    public bool gameOver = false;

    /// <summary>
    /// Determines if the player has watched a rewarded video ad.
    /// </summary>
    public bool adWatched = false;

    /// <summary>
    /// Determines if the the player restarted the game, either through
    /// the pause menu or game over menu.
    /// </summary>
    public bool gameRestarted = false;

    /// <summary>
    /// Used to keep track of how many consecutive games the player has 
    /// played. Display an interstital ad after five (5) games.
    /// </summary>
    public int gamesUntilAd = 5;

    /// <summary>
    /// Keep track of how many walls the player has destroyed in this
    /// specific playthrough.
    /// </summary>
    public int wallsDestroyed = 0;

    /// <summary>
    /// The starting speed of the platforms.
    /// </summary>
    public float platformSpeed = 15;


    [Header("Save Data:")]

    /// <summary>
    /// Is the player playing for the first time?
    /// This is used to show the tutorial screen at the
    /// beginning of the game.
    /// </summary>
    public bool firstTime = true;

    /// <summary>
    /// If swipe = true, then swipe is enabled, 
    /// otherwise phone tilting is enabled
    /// tilt will enabled by default.
    /// </summary>
    public bool swipe = false;

    /// <summary>
    /// Keeps a record of the player's highest score.
    /// </summary>
    public int wallsDestroyedRecord = 0;


    SaveInfo saveData;



    public bool GameInProgress
    {
        get { return gameStarted && !gamePaused && !gameOver; }
    }



    public SaveInfo SerializeSaveData()
    {
        saveData.firstTime = firstTime;
        saveData.swipe = swipe;
        saveData.wallsDestroyedRecord = wallsDestroyedRecord;

        return saveData;
    }

    public void DeserializeSaveData(SaveInfo data)
    {
        saveData = data;

        firstTime = data.firstTime;
        swipe = data.swipe;
        wallsDestroyedRecord = data.wallsDestroyedRecord;

        RestartGame(true);
        RestartStats();
    }

    public void RestartGame(bool intialize = false)
    {
        gameRestarted = !intialize;
        adWatched = false;
        platformSpeed = startingPlatformSpeed;
        wallsDestroyed = 0;

        gameStarted = false;
        gamePaused = false;
        gameOver = false;
    }

    public void RestartGameFromWatchingAd()
    {
        gameRestarted = false;
        platformSpeed = startingPlatformSpeed;

        gameStarted = false;
        gamePaused = false;
        gameOver = false;
    }

    public void RestartStats()
    {
        gamesUntilAd = 5;
        wallsDestroyed = 0;
    }
}

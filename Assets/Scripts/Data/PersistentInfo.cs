
/*****************************************************************************************************************
 - PersistentInfo.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class holds the variables needed to persist between transitions in games. Data in this class will 
     not be saved, but will be handled by the SaveManager class.
*****************************************************************************************************************/

public class PersistentInfo
{
    /// <summary>
    /// Determines whether or not the game is currently on the loading screen.
    /// </summary>
    public bool onLoadingScreen = true;

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
}

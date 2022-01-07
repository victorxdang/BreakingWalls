
/*****************************************************************************************************************
 - SaveInfo.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class will hold the settings of the user. This data will be saved to the local drive of the device.
     This class will only hold just purely data and nothing else. Add more variables/data as needed. Handled
     by the SaveManagerclass.
*****************************************************************************************************************/

public class SaveInfo
{
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
}

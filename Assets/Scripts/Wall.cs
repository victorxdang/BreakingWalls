
/*****************************************************************************************************************
 - Wall.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class contains the wall. Note that this script is attached to the parent object that then
     handles the set of children walls attached to it.
*****************************************************************************************************************/

using System.Collections;
using UnityEngine;

public class Wall : ColorEntity
{
    [SerializeField] DestructibleBlock destrucibleBlock;


    /// <summary>
    /// The time at which the wall changed colors. Used to determined if
    /// the achievement "LOL" should be unlocked.
    /// </summary>
    public float TimeOfColorChange { get; private set; }

    Platform parentPlatform;
    MeshRenderer meshRenderer;


    #region Wall Mechanics

    /// <summary>
    /// Called when the game object is created.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        parentPlatform = transform.parent.GetComponent<Platform>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Stops the color cycle coroutine if this game object is running it.
    /// </summary>
    void OnDisable()
    {
        StopCoroutine(ColorCycle(0));
    }

    /// <summary>
    /// Changes the colors of the wall however many seconds specified by TIME_INTERVAL constant 
    /// (default 5 seconds). It will iterate continously until the wall is destroyed.
    /// </summary>
    /// <returns></returns>
    IEnumerator ColorCycle(float waitTime)
    {
        while (!parentPlatform.GameSettings.gameOver)
        {
            TimeOfColorChange = Time.time;
            yield return GameUtilities.Wait(waitTime, () => parentPlatform.GameSettings.gameOver);
        }
    }

    /// <summary>
    /// Destroys the wall and spawn a destrutible wall in its place. The wall itself is not destructible, rather
    /// it is a trigger to allow the ball to go through. But this is done this way so that the ball is not physically
    /// affected (i.e. the ball slows down) when it encounters a wall. The destructible wall also has a negligible mass
    /// in order to not have the ball be physically affected by the destructible wall.
    /// </summary>
    public void DestroyWall()
    {
        meshRenderer.enabled = false;
        destrucibleBlock.gameObject.SetActive(true);
        destrucibleBlock.OnWallDestroyed(parentPlatform.DestrucibleWallDespawnTime, CurrentColor);
    }

    /// <summary>
    /// Randomly selects the wall to either be colored, white (impassable) or hidden. If the wall is colored, then is
    /// there is also a chance that the wall could change colors after a certain time interval.
    /// </summary>
    /// <param name="forceColor"></param>
    public void SetupWall(bool forceColor = false)
    {
        TimeOfColorChange = 0;
        Hidden = false;
        meshRenderer.enabled = true;

        if (!forceColor)
        {
            // 90% chance for a colored block
            // 10% chance for white colored block (impassable)
            bool color = Random.Range(0, 100) < 90;

            // 25% chance for the wall to be hidden
            Hidden = Random.Range(0, 100) < 25;

            if (Hidden)
                gameObject.SetActive(false);
            else if (color)
                ChangeRandomColor();
            else
                SetBlockColor(DefaultColor);  
        }
        else
        {
            ChangeRandomColor();
        }

        // the percentage chance that a colored wall will cycle through the three colors
        float randNum = (parentPlatform.GameSettings.platformSpeed < soGameSettings.PLATFORM_SPEED_HARDCAP / 2.0f) ? 5 : 15;

        // (randNum) chance for wall to change colors every 5 seconds
        // can only cycle through colors if the wall is not hidden;
        // change the color of the wall after an amount of seconds determined
        // by TIME_INTERVAL (defaulted to 5 seconds)
        bool cycleColors = Random.Range(0, 100) < randNum && !Hidden && CurrentColor != DefaultColor;

        if (cycleColors)
        {
            gameObject.name = "Wall Color Changing (Clone)";

            float waitTime = (parentPlatform.GameSettings.platformSpeed < soGameSettings.PLATFORM_SPEED_HARDCAP / 2.0f) 
                ? parentPlatform.ColorCycleInterval 
                : parentPlatform.ColorCycleInterval - 2;

            StartCoroutine(ColorCycle(waitTime));
        }
        else
        {
            gameObject.name = "Wall (Clone)";
        }
    }

    #endregion
}

/*****************************************************************************************************************
 - Ball.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class contains the ball, or the "main character" of the game. The player will be controlling this 
     entity throughout the game. For debugging purposes, use:
        - A to move left
        - D to move right
        - F to change colors
*****************************************************************************************************************/

using System;
using UnityEngine;

public class Ball : ColorEntity, IUpdatable
{
    #region Editable Region

    /// <summary>
    /// The distance where the ball will be considered "killed", effectively
    /// ending the game. Can be adjusted as desired, but do not set it to 0.5f
    /// or greater.
    /// </summary>
    const float KILL_Y_POS = 0;

    /// <summary>
    /// How fast the ball should move side-to-side. Increase the number to 
    /// increase the speed and vice versa.
    /// </summary>
    const float TRANSLATE_SPEED = 10;

    /// <summary>
    /// This is the how far (in pixels) the player must swipe either left or 
    /// right before registering it as a swipe and not a touch.
    /// </summary>
    const float SWIPE_THRESHOLD = 25;

    /// <summary>
    /// How fast the ball should roll from left to right when using the tilt
    /// functionality.
    /// </summary>
    const float ROLLING_SPEED = 30;

    /// <summary>
    /// The tag of the colored walls. Change this if/when the tag of the wall is
    /// going to be changed.
    /// </summary>
    const string WALL_TAG = "Wall";

    /// <summary>
    /// The tag of the trigger that will start the ckeck to see if the player is
    /// attempting parkour.
    /// </summary>
    const string ACTIVE_TAG = "ParkourActive";

    /// <summary>
    /// The tag of the trigger that will verify if the player has successfully
    /// compeleted their parkouring or not.
    /// </summary>
    const string DEACTIVE_TAG = "ParkourDeactive";

    /// <summary>
    /// The tag of the trigger that despawns the platform. This is primarily used
    /// to reset the parkour booleans so that it doesn't carry over to the next 
    /// platform. 
    /// </summary>
    const string DESAPWNER_TAG = "Despawner";

    #endregion


    // the distance in which the ball should move side-to-side, meaning that
    // the ball can only move 2.1 units either to the left or the right,
    // depending on the player input
    //
    // DO NOT CHANGE THIS NUMBER! The only reason to change it is because
    // of an extension of the platform on the x-axis
    const float TRANSLATE_DISTANCE = 2.1f;


    public static Action OnRequestingEndGame;
    public static Action<int> OnRequestingUpdateScore;


    [SerializeField] soGameSettings gameSettings;
    [SerializeField] soDebugSettings debugSettings;
    [SerializeField] soAudioClips soundEffects;


    /// <summary>
    /// Should this game object be updated by the update manager?
    /// </summary>
    public bool ShouldUpdate { get { return gameObject.activeSelf; } }


    // colorIndex is used to determine the current color of the ball (0 = red, 1 = green, 2 = blue, -1 = white).
    // Note that -1 is not a valid index and this is okay since the ChangeColor function will increment it by one
    // and will start the ball to red after the player switches colors. -1 is just a placeholder for a white ball.
    int colorIndex = -1;

    bool ballInPlace;
    bool movingLeft;
    bool movingRight; 
    bool setPosition;
    bool parkourActive;
    bool parkourDeactive;

    /*bool gameStarted;
    bool gamePaused;
    bool gameOver;*/

    float targetX;
    Rigidbody rbBall;
    AudioSource source;

    Vector2 touchStart;
    Vector2 touchEnd;

    Vector3 tempPos;



    /// <summary>
    /// Called when the game object is created.
    /// </summary>
    protected override void Awake()
    {
        if (FindObjectsOfType<Ball>().Length == 1)
        {
            base.Awake();
            source = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    void Start()
    {
        CurrentColor = DefaultColor;
        rbBall = GetComponent<Rigidbody>();
        rbBall.constraints = RigidbodyConstraints.FreezeAll;

        // if the player has watched an ad and has not played the game again by watching the ad, then
        // retain the same score from the previous game;
        //
        // otherwise, set the score to 0.
        if (gameSettings.adWatched || gameSettings.gameRestarted)
        {
            transform.position = new Vector3(0, 0.5f, 0);

            if (!debugSettings.InvincibleBall)
                rbBall.constraints &= ~RigidbodyConstraints.FreezePositionY;

            ballInPlace = true;

            if (gameSettings.gameRestarted)
                gameSettings.wallsDestroyed = 0;
        }
    }

    /// <summary>
    /// Called everytime the game object is enabled/re-enabled.
    /// </summary>
    void OnEnable()
    {
        UpdateManager.Register(this);
    }

    /// <summary>
    /// Called everytime the game object is disabled.
    /// </summary>
    void OnDisable()
    {
        UpdateManager.Unregister(this);
    }

    /// <summary>
    /// Clear all method pointers.
    /// </summary>
    void OnDestroy()
    {
        OnRequestingUpdateScore = null;
        OnRequestingEndGame = null;
    }

    /// <summary>
    /// Trigger whenever the ball hits a wall. If the ball hits a wall with the same color, then
    /// the wall will be destroyed. Otherwise, the ball will bounce back, signifying the end of the game.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        WallHit(other);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    public void OnUpdate()
    {
        // have the platform move forward until the Start Game button on the start menu is pressed
        if (!gameSettings.gameOver && gameSettings.gameStarted)
        {
            // move the ball into place if the ball is not at 0 on the x-axis
            if (!ballInPlace)
            {
                BallMoveIntoPosition();
            }
            else if (!gameSettings.gamePaused)
            {
                // keyboard input, for debugging purposes
                ProcessDebuggingInput();

                // tilt functionality
                if (!gameSettings.swipe)
                    ProcessTiltInput();

                // touch input, registers only only one finger touching the screen
                if (Input.touchCount > 0)
                    ProcessTouchInput();

                // move the ball left or right when appropriate
                if (movingLeft || movingRight)
                    MoveSide();
            }

            // end game when ball is below map boundaries (the ball fell off of the platform)
            if (transform.position.y < KILL_Y_POS)
            {
                // unlock achievement if the player fell off of the platform and didn't destroy
                // any walls
                if (gameSettings.wallsDestroyed == 0)
                    GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_air_ball);

                BallEndGame(0, 0, gameSettings.platformSpeed * 20);
            }
        }
    }


    #region Inputs

    /// <summary>
    /// Handles input from the keyboard for simple debugging:
    ///     - A-key to move left
    ///     - D-key to move right
    ///     - F-key to change colors
    /// </summary>
    void ProcessDebuggingInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -TRANSLATE_DISTANCE)
        {
            movingLeft = true;
            movingRight = false;
            setPosition = false;
        }
        else if (Input.GetKeyDown(KeyCode.D) && transform.position.x < TRANSLATE_DISTANCE)
        {
            movingRight = true;
            movingLeft = false;
            setPosition = false;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            BallChangeColor();
        }
    }

    /// <summary>
    /// Handles using tilting of the phone to move the ball left or right. Can be activated or deactivated
    /// by the user in the settings.
    /// </summary>
    void ProcessTiltInput()
    {
        // move the ball based on the accelerometer input of the phone
        transform.Translate(Input.acceleration.x * ROLLING_SPEED * Time.deltaTime, 0, 0);

        // set the ball to the correct position and do not allow the ball to roll
        // off of either side of the platform whenever the ball is still on the main platform,
        // after that, the ball can roll off the sides
        /*if (gameSettings.onInitialPlatform)
        {
            tempPos = transform.position;
            tempPos.x = Mathf.Clamp(tempPos.x, -TRANSLATE_DISTANCE, TRANSLATE_DISTANCE);
            transform.position = tempPos;
        }*/
    }

    /// <summary>
    /// Function to handle touch inputs (when swiping/touching the screen) when the user 
    /// enables swipe in the settings.
    /// </summary>
    void ProcessTouchInput()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStart = touch.position;
                break;
            case TouchPhase.Ended:
                touchEnd = touch.position;

                // swiped left, move left if swipe is enabled
                if (touchEnd.x - touchStart.x < -SWIPE_THRESHOLD && gameSettings.swipe && transform.position.x > -TRANSLATE_DISTANCE)
                {
                    movingLeft = true;
                    movingRight = false;
                    setPosition = false;
                }
                // swiped right, move right, move right if swipe is enabled
                else if (touchEnd.x - touchStart.x > SWIPE_THRESHOLD && gameSettings.swipe && transform.position.x < TRANSLATE_DISTANCE)
                {
                    movingRight = true;
                    movingLeft = false;
                    setPosition = false;
                }
                // touch to change colors, will not be affected by swipe/tilt setting
                else
                {
                    BallChangeColor();
                }

                break;
        }
    }

    #endregion


    #region Ball Mechanics

    /// <summary>
    /// Smoothly transitions the ball's position to either left or right,
    /// depending on the button pressed/touch input. This method is being 
    /// updated from the Update() function.
    /// </summary>
    void MoveSide()
    {
        if (!setPosition)
        {
            // unfreeze the x-axis to allow the ball to move from side-to-side
            rbBall.constraints &= ~RigidbodyConstraints.FreezePositionX;

            if (movingLeft)
                targetX += (transform.position.x <= 0) ? -2.1f : 0;
            else if (movingRight)
                targetX += (transform.position.x >= 0) ? 2.1f : 0;

            tempPos = transform.position;
            tempPos.x = targetX;
            setPosition = true;
        }

        if (transform.position.x != targetX)
        {
            transform.Translate(TRANSLATE_SPEED * Time.deltaTime * ((transform.position.x > targetX) ? -1 : 1), 0, 0);

            if ((transform.position.x > targetX && movingRight) || (transform.position.x < targetX && movingLeft))
            {
                transform.position = tempPos;
                LockBallPosition();
            }
        }
        else
        {
            LockBallPosition();
        }
    }

    /// <summary>
    /// Locks the x-axis of the ball.
    /// </summary>
    void LockBallPosition()
    {
        rbBall.constraints |= RigidbodyConstraints.FreezePositionX;

        movingLeft = false;
        movingRight = false;
        setPosition = false;
    }

    /// <summary>
    /// Moves the ball into camera view at the beginning of the game.
    /// </summary>
    void BallMoveIntoPosition()
    {
        if (transform.position.z < 0)
        {
            transform.Translate(0, 0, Time.deltaTime * 2);
        }
        else
        {
            // unfreeze the y-constraint to have the ball be able to fall
            if (!debugSettings.InvincibleBall)
                rbBall.constraints &= ~RigidbodyConstraints.FreezePositionY;

            ballInPlace = true;
        }
    }

    /// <summary>
    /// Changes the balls color and plays a sound effect.
    /// </summary>
    void BallChangeColor()
    {
        PlayBallColorSwitch();
        colorIndex = ChangeColor(colorIndex);
    }

    /// <summary>
    /// Called when the ball encounters a situation where the game must end. (ball is not the right color when it 
    /// hits a wall, falling off the map, etc.)
    /// The force paramaters is used to add force to the ball when the game ends since all of the axes will be 
    /// unfrozen allowing the ball to do some sort of free falling.
    /// </summary>
    /// <param name="xForce"></param>
    /// <param name="yForce"></param>
    /// <param name="zForce"></param>
    void BallEndGame(float xForce, float yForce, float zForce)
    {
        rbBall.constraints = RigidbodyConstraints.None;

        // push the ball forward a little to make it look like it was rolling
        // down the platforms
        rbBall.AddForce(xForce, yForce, zForce);

        OnRequestingEndGame?.Invoke();
        Destroy(gameObject, 3);
    }

    /// <summary>
    /// Trigger whenever the ball hits a wall. If the ball hits a wall with the same color, then
    /// the wall will be destroyed. Otherwise, the ball will bounce back, signifying the end of the game.
    /// </summary>
    /// <param name="other"></param>
    void WallHit(Collider other)
    {
        if (other.CompareTag(WALL_TAG) && !gameSettings.gameOver)
        {
            Wall collidingWall = other.GetComponent<Wall>();

            // only allow the ball to destroy a wall if:
            //  - the ball's current color is not white and 
            //  - the colliding wall's current color is not white and 
            //  - the colliding wall's and the ball's curent color is the same as one another 
            //    OR
            //  - the ball is in invincible mode
            //
            // otherwise:
            //  - knock back the ball, destroy it after 3 seconds and end the game.
            if ((CurrentColor != DefaultColor && collidingWall.CurrentColor != DefaultColor && collidingWall.CurrentColor == CurrentColor) || debugSettings.InvincibleBall)
            {
                PlayWallDestroyedSE(); // play the wall destroyed audio
                collidingWall.DestroyWall(); // this is where the wall is actually destroyed, look in "Wall" script

                // update the number of walls brokens,
                // glass walls does not count and not in invincible mode
                if (!debugSettings.InvincibleBall)
                    gameSettings.wallsDestroyed++;

                // called to update the number displayed on the UI
                OnRequestingUpdateScore?.Invoke(gameSettings.wallsDestroyed);


                // - below this point in the if statement is Google Play achievement stuff -


                // unlock an achievement when the player destroys their first wall (lol)
                GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_wall_smash);

                // increment achivement based on the current setting (swipe/tilt)
                GooglePlayGamesService.IncrementAchivement(gameSettings.swipe ? GPGSIds.achievement_proswiper : GPGSIds.achievement_protilter, 1);

                // unlocked when the player get 100 or more walls destroyed in one game and without watching an ad to keep
                // their score
                if (!gameSettings.adWatched && gameSettings.wallsDestroyed >= 100)
                    GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_quality_esports_gameplay);

                // make sure the player can't get the parkour achievement if they destroyed a wall on the platform
                ResetParkourBooleans();
            }
            else
            {
                // unlocked when the player slams into a wall that just changed colors
                if (collidingWall.TimeOfColorChange > 0 && collidingWall.TimeOfColorChange < Time.time + 0.5f)
                    GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_lol);

                PlayBallWrongColor();
                BallEndGame(0, 300, -75 * gameSettings.platformSpeed);
            }
        }
        else if (other.CompareTag(ACTIVE_TAG)) // check if the player is currently parkouring
        {
            parkourActive = !parkourActive;
            CheckForParkourAchievement();
        }
        else if (other.CompareTag(DEACTIVE_TAG)) // check if the player has successfully completed parkouring
        {
            parkourDeactive = !parkourDeactive;
            CheckForParkourAchievement();
        }
        else if (other.CompareTag(DESAPWNER_TAG)) // reset parkour booleans if the player has exited a platform
        {
            ResetParkourBooleans();
        }
    }

    /// <summary>
    /// Check if the player has successfully compeleted parkouring with the ball. To be successful, the ball must enter the active
    /// trigger, located on the backside of the platform, first and then hit the deactive trigger that is on the forward side of 
    /// the platform. If the player goes through the same trigger twice, the boolean keeping track will revert to back to false.
    /// So the player must hit both triggers, both must be true in order to get the parkour achievement.
    /// </summary>
    void CheckForParkourAchievement()
    {
        if (parkourActive && parkourDeactive && (transform.position.x > 2.1f || transform.position.x < -2.1f))
            GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_whoa_holy);
    }

    /// <summary>
    /// Resets the parkour booleans if either the player has exited the platform (entered the despawning trigger) or destroyed
    /// a wall.
    /// </summary>
    void ResetParkourBooleans()
    {
        parkourActive = false;
        parkourDeactive = false;
    }

    #endregion


    #region Sound Effects

    /// <summary>
    /// Plays a sound effect based on the current color of the wall.
    /// </summary>
    /// <param name="currentColor"></param>
    /// <param name="volume"></param>
    public void PlayWallDestroyedSE(float volume = soAudioClips.SE_VOLUME)
    {
        source.PlayOneShot(soundEffects.WallDestroyedSE, volume);
    }

    /// <summary>
    /// Play a sound effect when the ball switches color.
    /// </summary>
    /// <param name="volume"></param>
    public void PlayBallColorSwitch(float volume = soAudioClips.SE_VOLUME)
    {
        source.PlayOneShot(soundEffects.BallColorSwitchSE, volume);
    }

    /// <summary>
    /// Plays a sound effect whenever the ball is switching lanes.
    /// </summary>
    /// <param name="volume"></param>
    public void PlayBallMovingSideSE(float volume = soAudioClips.SE_VOLUME)
    {
        source.PlayOneShot(soundEffects.BallMovingSideSE, volume);
    }

    /// <summary>
    /// Played when the game ends (whenever the ball hits a wall of the wrong color).
    /// </summary>
    /// <param name="volume"></param>
    public void PlayBallWrongColor(float volume = soAudioClips.SE_VOLUME)
    {
        source.PlayOneShot(soundEffects.BallWrongColorSE, volume);
    }

    #endregion
}
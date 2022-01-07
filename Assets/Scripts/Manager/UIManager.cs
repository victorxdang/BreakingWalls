
/*****************************************************************************************************************
 - GameManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     The UI logic is handled within this class. Menus, skyboxes and such are within here.
*****************************************************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Editable Region

    /// <summary>
    /// Allow how ever many seconds for the player to get ready to play again
    /// (defaulted to three).
    /// </summary>
    const int RESUME_DELAY = 3;

    /// <summary>
    /// The distance the player must travel without hitting any walls, to unlock
    /// the achievement "No Walls But Balls".
    /// </summary>
    const int DISTANCE_WITHOUT_HITTING_WALLS = 300;

    /// <summary>
    /// The unit of distance at which the platform will increase its speed.
    /// Can be changed but if so, also change the PLATFORM_SPEED_INCREASE so that
    /// it matches the target ration between this constant and PLATFORM_SPEED_INCREASE, unless
    /// the platform speed is going to be greater than 3.
    /// </summary>
    const float DISTANCE_TO_INCREASE = 50;

    /// <summary>
    /// The amount to increase the speed of the platform by (not recommended to be greater than 2 or 3).
    /// Also, recommendation applies to this constant is the same as DISTANCE_TO_INCREASE 
    /// when changing this value.
    /// </summary>
    const float PLATFORM_SPEED_INCREASE = 0.5f;


    /// <summary>
    /// How far up the camera should be to provide optimal view.
    /// </summary>
    const float CAMERA_HEIGHT = 5;

    /// <summary>
    /// The following position of the ball is how far ball it should be to provide optimal view of the 
    /// scenery, the platforms and walls.
    /// </summary>
    const float CAMERA_START_Z_POSITION = -8;

    /// <summary>
    /// How far up the camera should be to provide a cinematic overview of the map.
    /// </summary>
    const float CAMERA_RESTART_Z_POSITION = 75;


    /// <summary>
    /// The Euler's angles of the rotation of the camera. This angle makes the camera
    /// angle down by 10 degrees to get a better view of the map, scenery and the ball.
    /// Can be changed if desired.
    /// </summary>
    readonly Vector3 FOLLOW_CAM_ROTATION = new Vector3(10, 0, 0);

    /// <summary>
    /// The Euler's angle for the rotation of the camera when the game is over. This is to have
    /// the camera tilt upwards a bit for better visibility of the buttons on the game over 
    /// menu.
    /// </summary>
    readonly Vector3 FOLLOW_CAM_END_ROTATION = new Vector3(-5, 0, 0);

    #endregion


    #region Fields


    [Header("Settings:")]
    [Range(0, 10)]
    [SerializeField] float cameraTransitionTime = 1;
    [SerializeField] float timeToLoadIntoGame = 10; // amount of time in seconds to load into game if banner ad cannot load

    [Header("UI Components:")]
    [SerializeField] UILoading loadingUI;
    [SerializeField] UIStart startUI;
    [SerializeField] UITutorial tutorialUI;
    [SerializeField] UIGameplay gameplayUI;
    [SerializeField] UIPause pauseUI;
    [SerializeField] UIGameOver gameOverUI;
    [SerializeField] UISettings settingsUI;
    [SerializeField] UICredits creditsUI;

    [Header("Scriptable Objects:")]
    [SerializeField] soGPGSSettings gpgsSettings;
    [SerializeField] soGameSettings gameSettings;
    [SerializeField] soDebugSettings debugSettings;


    bool subscribedToBannerEvents = false;
    bool tutorialCalledFromSettings = false;
    bool settingsCalledFromStart = false;
    bool canSwitchMenus = true;
    bool resume = false;
    bool showToastPressedOnce = false;

    int timeToResume = 0;

    float distanceToSpeedInc = DISTANCE_TO_INCREASE;
    float distanceTravelled = 0;
    float timeToNextResumeCheck = 0;
    float timeOfShowToastClick = 0;

    Camera mainCamera;

    #endregion



    #region Built-In Methods

    /// <summary>
    /// Awake is called before start, whenever the object is created.
    /// </summary>
    void Awake()
    {
        Time.timeScale = 1;
        mainCamera = FindObjectOfType<Camera>();

        startUI.SetStartGameCallback(StartGame);
        startUI.SetSettingsCallback(ShowSettingsMenu);

        tutorialUI.SetExitTutorialCallback(ExitTutorialScreen);
        gameplayUI.SetPauseButtonCallback(PauseGame);

        pauseUI.SetResumeButtonCallback(ResumeGame);
        pauseUI.SetRestartButtonCallback(RestartGame);
        pauseUI.SetSettingsButtonCallback(ShowSettingsMenu);
        pauseUI.SetQuitButtonCallback(QuitGame);

        gameOverUI.SetWatchAdCallback(ShowRewardedVideoAd);
        gameOverUI.SetRestartCallback(RestartGame);
        gameOverUI.SetQuitCallback(QuitGame);

        settingsUI.SetCreditsCallback(ShowCredits);
        settingsUI.SetTutorialCallback(ShowTutorialMenu);
        settingsUI.SetReturnCallback(ExitSettingsMenu);

        creditsUI.SetExitCreditsCallback(ExitCreditsMenu);

        
        AdManager.OnRewardedVideoComplete += RestartGameFromWatchingAd;
        Ball.OnRequestingEndGame += EndGame;
        Ball.OnRequestingUpdateScore += UpdateScoreText;

        SetDebuggingValues();

        if (gameSettings.gameRestarted || gameSettings.adWatched)
        {
            loadingUI.DisableCanvasHolder();
        }
        else
        {
            loadingUI.EnableCanvasHolder(); // <- only enabled loading screen UI initially

            subscribedToBannerEvents = true;
            AdManager.OnBannerLoaded += ShowStartMenu;
            AdManager.OnBannerFailedToLoad += ShowStartMenu;
        }

        startUI.DisableCanvasHolder();
        tutorialUI.DisableCanvasHolder();
        gameplayUI.DisableCanvasHolder();
        pauseUI.DisableCanvasHolder();
        gameOverUI.DisableCanvasHolder();
        settingsUI.DisableCanvasHolder();
        creditsUI.DisableCanvasHolder();

        if (GameUtilities.IsAndroid)
            StartCoroutine(GameUtilities.Wait(timeToLoadIntoGame, ShowStartMenu, () => startUI.CanvasHolderActive));
    }

    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    void Start()
    {
        UpdateScoreText(gameSettings.wallsDestroyed);

        // have it where the camera does not start off by looking at the start menu, but instead
        // go straight into the game if the game was restarted or the player watched an ad
        if (gameSettings.gameRestarted || gameSettings.adWatched)
        {
            AdManager.DestroyBanner();
            GC.Collect(); // call the garbage man

            // start camera a bit further up the map, will move into place when the game start to have 
            // a cinematic-like effect
            mainCamera.transform.position = new Vector3(0, CAMERA_HEIGHT, CAMERA_START_Z_POSITION);
            mainCamera.transform.rotation = Quaternion.Euler(FOLLOW_CAM_END_ROTATION);

            StartGame();
        }
        else
        {
            // initialize and show a banner ad
            AdManager.ShowBannerAd();

            // this is just for use in the editor
            if (debugSettings.SkipLoadingScreen || !GameUtilities.IsAndroid)
            {
                ShowStartMenu();
            }
        }
    }

    /// <summary>
    /// Remove all pointers when this objects is destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (subscribedToBannerEvents)
        {
            subscribedToBannerEvents = false;
            AdManager.OnBannerLoaded -= ShowStartMenu;
            AdManager.OnBannerFailedToLoad -= ShowStartMenu;
        }   
        
        AdManager.OnRewardedVideoComplete -= RestartGameFromWatchingAd;
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    void Update()
    {
        if (gameSettings.gameStarted && !gameSettings.gameOver && !gameSettings.gamePaused)
        {
            // keeps track of the distanceTravelled for the purpose of increase difficulty once the
            // player has acheived a certain distance
            distanceTravelled += gameSettings.platformSpeed * Time.deltaTime;

            // increase the speed of the platform once the play does achieve the target distance
            if (distanceTravelled >= distanceToSpeedInc && gameSettings.platformSpeed < soGameSettings.PLATFORM_SPEED_HARDCAP)
            {
                distanceToSpeedInc += DISTANCE_TO_INCREASE;
                gameSettings.platformSpeed += PLATFORM_SPEED_INCREASE;
            }
        }

        // display the resume timer when the game is resuming from pause
        if (resume && Time.unscaledTime > timeToNextResumeCheck)
        {
            timeToNextResumeCheck = Time.unscaledTime + 1;
            ResumeTimer();
        }

        // unlocked when the player goes for a certain distance without hitting a wall
        if (distanceTravelled >= DISTANCE_WITHOUT_HITTING_WALLS && gameSettings.wallsDestroyed == 0)
        {
            GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_no_walls_but_balls);
        }

        // if the player enters the back key on the phone, then return to the previous menu, otherwise if on the start menu, have the player
        // press the back button twice to confirm that that want to exit the game
        if (Input.GetKeyDown(KeyCode.Escape) && (canSwitchMenus && !resume))
        {
            ReturnToPreviousMenu();
        }
    }

    #endregion


    #region Start Menu

    void ShowStartMenu()
    {
        loadingUI.FadeOut();
        startUI.FadeIn();
    }

    /// <summary>
    /// Does a smooth transition from the game menu to in game.
    /// </summary>
    void StartGame()
    {
        gameSettings.gamesUntilAd--;

        // if the start menu is currently active, make it fade out
        if (startUI.CanvasHolderActive)
        {
            startUI.FadeOut();
        }

        // if the player is playing for the first time
        if (gameSettings.firstTime)
        {
            gameSettings.firstTime = false;
            ShowTutorialMenu();
        }
        else
        {
            AdManager.DestroyBanner();
            gameSettings.gameStarted = true;

            if (gameSettings.gameRestarted || gameSettings.adWatched)
            {
                gameSettings.gameRestarted = false;
                LeanTween.moveZ(mainCamera.gameObject, CAMERA_RESTART_Z_POSITION, cameraTransitionTime);
                LeanTween.rotate(mainCamera.gameObject, FOLLOW_CAM_ROTATION, cameraTransitionTime);
            }

            gameplayUI.FadeIn();
        }
    }

    /// <summary>
    /// Used to update the text that outputs the amount of walls that have been broken
    /// by the player.
    /// </summary>
    void UpdateScoreText(int score)
    {
        gameplayUI.ScoreText = score.ToString();
    }

    #endregion


    #region Settings Menu

    /// <summary>
    /// Change the current menu to the settings menu. Settings menu should only be available in the start and pause menus only.
    /// </summary>
    /// <param name="currentMenu"></param>
    void ShowSettingsMenu()
    {
        if (startUI.CanvasHolderActive)
        {
            settingsCalledFromStart = true;
            startUI.FadeOut();
        }
        else if (pauseUI.CanvasHolderActive)
        {
            settingsCalledFromStart = false;
            pauseUI.FadeOut();
        }

        settingsUI.FadeIn();
    }

    /// <summary>
    /// Exits the settings menu to either the start menu or the pause menu, depending on where it was called from.
    /// </summary>
    void ExitSettingsMenu()
    {
        if (settingsCalledFromStart)
            startUI.FadeIn();
        else
            pauseUI.FadeIn();

        settingsCalledFromStart = false;
        settingsUI.FadeOut();
    }

    /// <summary>
    /// Display the credits page.
    /// </summary>
    void ShowCredits()
    {
        settingsUI.FadeOut();
        creditsUI.FadeIn();
    }

    /// <summary>
    /// Exits the credits menu and returns to the settings menu.
    /// </summary>
    void ExitCreditsMenu()
    {
        settingsUI.FadeIn();
        creditsUI.FadeOut();
    }

    /// <summary>
    /// Displays the tutorial menu. Will only display once when the user plays this game for the first time
    /// or can be displayed by pressing the help button in the settings menu.
    /// </summary>
    void ShowTutorialMenu()
    {
        if (settingsUI.CanvasHolderActive)
        {
            tutorialCalledFromSettings = true;
            settingsUI.FadeOut();
        }
        else if (startUI.CanvasHolderActive)
        {
            tutorialCalledFromSettings = false;
            startUI.FadeOut();
        }

        tutorialUI.FadeIn();
    }

    /// <summary>
    /// Exits the tutorial screen and starts the game, or returns to the settings menu.
    /// </summary>
    void ExitTutorialScreen()
    {
        if (tutorialCalledFromSettings)
        {
            settingsUI.FadeIn();
        }
        else
        {
            AdManager.DestroyBanner();
            gameSettings.gameStarted = true;
            gameplayUI.FadeIn();
        }

        tutorialCalledFromSettings = false;
        tutorialUI.FadeOut();
    }

    /// <summary>
    /// Returns to the previous menu from the respective menu.
    /// </summary>
    /// <param name="currentMenu"></param>
    void ReturnToPreviousMenu()
    {
        if (startUI.CanvasHolderActive)
        {
            if (showToastPressedOnce && Time.time > timeOfShowToastClick)
                showToastPressedOnce = false;

            if (!showToastPressedOnce)
            {
                showToastPressedOnce = true;
                timeOfShowToastClick = Time.time + 2; // reset exiting if player has not pressed back button for more than two second
                Toast.Show("Press back again to exit");
            }
            else
            {
                QuitGame();
            }
        }
        else if (tutorialUI.CanvasHolderActive)
        {
            ExitTutorialScreen();
        }
        else if (gameplayUI.CanvasHolderActive)
        {
            PauseGame();
        }
        else if (pauseUI.CanvasHolderActive)
        {
            ResumeGame();
        }
        else if (settingsUI.CanvasHolderActive)
        {
            ExitSettingsMenu();
        }
        else if (creditsUI.CanvasHolderActive)
        {
            ExitCreditsMenu();
        }
        /*else if (gameOverUI.CanvasHolderActive)
        {
            // no effect
        }*/
    }

    #endregion


    #region Pause Menu

    /// <summary>
    /// Pauses the game and brings up the pause menu. Also displays a banner ad at the bottom of the screen.
    /// </summary>
    void PauseGame()
    {
        Time.timeScale = 0;
        AdManager.ShowBannerAd();
        gameplayUI.FadeOut();
        pauseUI.FadeIn();

        gameSettings.gamePaused = true;
    }

    /// <summary>
    /// Resumes the game and switches the menu back to the in-game menu.
    /// </summary>
    void ResumeGame()
    {
        pauseUI.FadeOut();
        gameplayUI.FadeIn(false);
        gameplayUI.ScoreText = string.Empty;

        timeToResume = RESUME_DELAY;
        timeToNextResumeCheck = Time.unscaledTime + 1;
        resume = true;
    }

    /// <summary>
    /// Countdown timer until the game resume again, this will be three seconds. This function is updated by the
    /// Update() method.
    /// </summary>
    void ResumeTimer()
    {
        gameplayUI.ResumeText = "Resuming in\n" + timeToResume;
        timeToResume--;

        if (timeToResume < 0)
        {
            Time.timeScale = 1;
            gameplayUI.ResumeText = "";
            gameplayUI.ScoreText = gameSettings.wallsDestroyed.ToString();
            gameplayUI.SetButtonsInteractable(true);
            gameSettings.gamePaused = false;
            resume = false;

            AdManager.DestroyBanner();
            GC.Collect();
        }
    }

    /// <summary>
    /// Restarts the game. When this occurs, if the player has watched an ad previously, then
    /// the player can watch an ad again to maintain his/her current progress after the end of
    /// the next game. 
    /// </summary>
    void RestartGame()
    {
        gameSettings.RestartGame();
        loadingUI.LoadScene(gameSettings.mainSceneName);
    }

    /// <summary>
    /// Restarts the game but keeps the scores and such after watching an ad.
    /// </summary>
    void RestartGameFromWatchingAd()
    {
        gameSettings.RestartGameFromWatchingAd();
        loadingUI.LoadScene(gameSettings.mainSceneName);
    }

    #endregion


    #region Game End

    /// <summary>
    /// Exits the app.
    /// </summary>
    void QuitGame()
    {
        AdManager.CleanUp();
        GC.Collect();
        GameUtilities.QuitGame();
    }

    /// <summary>
    /// Helper method to call the coroutine to end the game.
    /// </summary>
    public void EndGame()
    {
        gameSettings.gameOver = true;
        AdManager.ShowBannerAd();
        gameplayUI.SetButtonsInteractable(false);
        StartCoroutine(GameUtilities.Wait(2, FinalizeEndGame));
    }

    /// <summary>
    /// Ends the game and saves the player's data.
    /// </summary>
    /// <returns></returns>
    void FinalizeEndGame()
    {
        // show (after playing 5 consecutive games) an interstitial ad before making the menus fade out and in
        TryShowInterstitialAd();

        // deactivate the pause and settings menu when the game is over, then fade in/out the game over menu and the in-game
        // menu respectively
        if (pauseUI.CanvasHolderActive)
            pauseUI.CanvasHolderActive = false;

        if (settingsUI.CanvasHolderActive)
            settingsUI.CanvasHolderActive = false;

        gameplayUI.FadeOut();
        gameOverUI.FadeIn();

        // disable the ability for the player to watch ad again if they already watched one
        gameOverUI.SetWatchAdButtonInteractable(!gameSettings.adWatched);

        // determine if the player has set a new personal record
        bool newRecord = false;
        if (gameSettings.wallsDestroyed > gameSettings.wallsDestroyedRecord)
        {
            newRecord = true;

            if (gameSettings.wallsDestroyedRecord > 0)
                GooglePlayGamesService.UnlockAchievement(GPGSIds.achievement_record_breaker);

            gameSettings.wallsDestroyedRecord = gameSettings.wallsDestroyed;
            SaveManager.CloudRecordSave = gameSettings.wallsDestroyed;
            GooglePlayGamesService.AddScoreToLeaderboard(GPGSIds.leaderboard_walls_destroyed, gameSettings.wallsDestroyed);

            SaveManager.SaveCloud();
        }

        gameOverUI.SetFinalScore(gameSettings.wallsDestroyed, gameSettings.wallsDestroyedRecord, newRecord);
        SaveManager.SaveLocal();
    }

    #endregion


    #region Ads

    /// <summary>
    /// Determines whether to show an ad if it is appropriate.
    /// </summary>
    void TryShowInterstitialAd()
    {
        // show an interstitial ad if the player has played 5 games consecutively in one sitting
        if (gameSettings.gamesUntilAd <= 0)
        {
            gameSettings.gamesUntilAd = 5;
            AdManager.ShowInterstitialAd();
        }
    }

    /// <summary>
    /// Show reward video ads.
    /// </summary>
    void ShowRewardedVideoAd()
    {
        gameSettings.adWatched = true;
        AdManager.ShowRewardedVideoAd();
    }

    #endregion


    #region Utility Methods

    /// <summary>
    /// Sets the correct boolean values for the debugging variables so that there are no "going back to the editor
    /// to change this value" kind of deal.
    /// </summary>
    void SetDebuggingValues()
    {
        #if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        debugSettings.MovePlatform = true;
        debugSettings.InvincibleBall = false;
        debugSettings.SkipLoadingScreen = false;
        #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
        gpgsSettings.EnablePlayServices = true;
        #endif
    }

    #endregion
}
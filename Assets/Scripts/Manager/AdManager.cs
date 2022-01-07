
/*****************************************************************************************************************
 - AdManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Ads are handled this script. Currently, Unity's built-in ads and Google AdMob are supported. To call ads
     from this script, call the ShowInterstitialAd() or ShowRewardedVideoAd() methods appropriately. To 
     de-reference variables that point to certain ads, use the CleanUp() method. The ad loading methods in this 
     script will also determine whether or not to use live or test ads. However, it determines this by seeing 
     if Unity Ads (assuming itis being used as part of the ads system) has "Test Mode" enabled. If so, then
     AdMob ads will also be test ads.

     NOTE: There will be many blocks of code that are commented out. This is for use with Unity Monetization 
     plug-in. There was an attempt to try and implement this but it did not work out really well. The code,
     however is still there if there is a desire to try and implement Unity Monetization in the future again.
*****************************************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static Action OnBannerLoaded;
    public static Action OnBannerFailedToLoad;
    public static Action OnIntersititialLoaded;
    public static Action OnIntersittialFailedToLoad;
    public static Action OnRewardedVideoComplete;
    public static Action OnRewardedVideoFailedToLoad;



    [SerializeField] soAdMobSettings soAdMobSettings;
    [SerializeField] soUnityAdsSettings soUnityAdsSettings;
    [SerializeField] soGameSettings soGameSettings;


    static BannerView adMobBanner;
    static InterstitialAd adMobInterstitial;
    static RewardBasedVideoAd adMobRewardVideo;

    static soAdMobSettings adMobSettings;
    static soUnityAdsSettings unityAdsSettings;
    static soGameSettings gameSettings;



    #region Built-In Methods

    void Awake()
    {
        if (FindObjectsOfType<AdManager>().Length == 1)
        {
            adMobSettings = soAdMobSettings;
            unityAdsSettings = soUnityAdsSettings;
            gameSettings = soGameSettings;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes this class.
    /// </summary>
    void Start()
    {
        //Monetization.Initialize(unityAppID, testMode); // initialize Unity ads
        MobileAds.Initialize(adMobSettings.AppID); // initialize AdMob ads

        // initialize and load Unity ads
        //RequestUnityBannerAd(bannerString);
        //RequestUnityInterstitialAd(interstitialString);
        //RequestUnityRewardedVideoAd(rewardedVideoString);

        // initialize and load the various ad types, banners show up immediately after
        // being called so call banners whereever appropriate
        RequestAdMobBannerAd();
        RequestAdMobInterstitialAd();
        RequestAdMobRewardedVideoAd();
    }

    /// <summary>
    /// Removes all of the method pointers.
    /// </summary>
    void OnDestroy()
    {
        OnBannerLoaded = null;
        OnBannerFailedToLoad = null;
        OnIntersititialLoaded = null;
        OnIntersittialFailedToLoad = null;
        OnRewardedVideoComplete = null;
        OnRewardedVideoFailedToLoad = null;
    }

    #endregion


    #region Show Ads

    /// <summary>
    /// Helper method to display a banner ad on the screen. showAd is only used when initializing the ad to determine
    /// whether or not to show the ad when it is finished loading.
    /// </summary>
    /// <param name="showAd"></param>
    public static void ShowBannerAd()
    {
        /*if (Advertisement.IsReady(bannerString))
        {
            Advertisement.Banner.Show(bannerString);
        }*/

        if (adMobBanner != null)
        {
            adMobBanner.Show();
        }
    }
    
    /// <summary>
    /// Shows a full screen ad, either interactable or static, or a skippable video.
    /// </summary>
    public static void ShowInterstitialAd()
    {
        // AdMob
        if (adMobInterstitial.IsLoaded())
        {
            adMobInterstitial.Show();
        }
        // Unity
        else if (Advertisement.IsReady(unityAdsSettings.InterstitialType))
        {
            RequestUnityInterstitialAd(unityAdsSettings.InterstitialType);
        }
    }

    /// <summary>
    /// Call to play a rewarded video ad (unskippable). Prioritizes AdMob over Unity ads.
    /// </summary>
    public static void ShowRewardedVideoAd()
    {
        // AdMob
        if (adMobRewardVideo.IsLoaded())
        {
            adMobRewardVideo.Show();
        }
        // Unity
        else if (Advertisement.IsReady(unityAdsSettings.RewardedVideoType))
        {
            RequestUnityRewardedVideoAd(unityAdsSettings.RewardedVideoType);
        }
    }

    #endregion


    #region Unity Ads

    /*/// <summary>
    /// Displays a banner ad if one is ready and available.
    /// </summary>
    static void RequestUnityBannerAd(string id)
    {
        BannerLoadOptions options = new BannerLoadOptions { loadCallback = HandleUnityBannerAdLoadCallback, errorCallback = HandleUnityBannerAdErrorCallback };
        Advertisement.Banner.Load(id, options);
    }*/

    /// <summary>
    /// Displays an interstitial ad if one is ready and available.
    /// </summary>
    /// <param name="id"></param>
    static void RequestUnityInterstitialAd(string id)
    {
        ShowOptions options = new ShowOptions { resultCallback = HandleUnityIntersitialCallback };
        Advertisement.Show(id, options);

        /*ShowAdPlacementContent ad = Monetization.GetPlacementContent(id) as ShowAdPlacementContent;

        if (ad != null)
            ad.Show(HandleUnityIntersitialCallback);*/
    }

    /// <summary>
    /// Displays a rewarded video ad if one is ready and available.
    /// </summary>
    /// <param name="id"></param>
    static void RequestUnityRewardedVideoAd(string id)
    {
        ShowOptions options = new ShowOptions { resultCallback = HandleUnityRewardedVideoCallback };
        Advertisement.Show(id, options);

        /*ShowAdPlacementContent ad = Monetization.GetPlacementContent(id) as ShowAdPlacementContent;

        if (ad != null)
            ad.Show(HandleUnityRewardedVideoCallback);*/
    }

    /*/// <summary>
    /// Start to display the start menu when the ad is successfully loaded.
    /// </summary>
    static void HandleUnityBannerAdLoadCallback()
    {
        if (SaveManager.Instance.PersistentData.onLoadingScreen)
            GameManager.Instance.SimulateLoading(Random.Range(1, 3));

        unityBannerFailToLoad = false;
    }

    /// <summary>
    /// Set the boolean to true when the ad is not loaded.
    /// </summary>
    /// <param name="msg"></param>
    static void HandleUnityBannerAdErrorCallback(string msg)
    {
        unityBannerFailToLoad = true;
    }*/

    /// <summary>
    /// Callback function whenever the player finished watching an intersitial/skippable video ad.
    /// </summary>
    /// <param name="result"></param>
    static void HandleUnityIntersitialCallback(ShowResult result)
    {
        // nothing happens here, yet
    }

    /// <summary>
    /// Callback function whenever the player finishes watching a rewarded video ad.
    /// </summary>
    /// <param name="result"></param>
    static void HandleUnityRewardedVideoCallback(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            DestroyBanner();
            gameSettings.RestartGameFromWatchingAd();
            SceneManager.LoadScene(gameSettings.mainSceneName);

            //OnRewardedVideoComplete?.Invoke();
        }
    }

    #endregion


    #region AdMob Ads

    /// <summary>
    /// Creates a new banner ad that will be displayed on the bottom of screen.
    /// There is no Show() method call for banner so after it is finished creating, then
    /// the banner ad will be displayed on the screen.
    /// 
    /// This method should only be called once.
    /// </summary>
    static void RequestAdMobBannerAd()
    {
        // destroy the previous banner ad
        DestroyBanner(true);

        // create the banner ad
        adMobBanner = new BannerView(adMobSettings.BannerID, AdSize.SmartBanner, AdPosition.Bottom);

        adMobBanner.OnAdLoaded += HandleAdMobBannerAdLoaded;
        adMobBanner.OnAdFailedToLoad += HandleAdMobBannerAdFailedToLoad;

        // load the banner ad to display
        adMobBanner.LoadAd(new AdRequest.Builder().Build());
    }

    /// <summary>
    /// Creates a new full screen interstitial ad. To show an interstital, the
    /// ShowInterstitialAd() method needs to be called, this method only loads the
    /// interstital ad.
    /// </summary>
    static void RequestAdMobInterstitialAd()
    {
        // destroy the previous interstital (if one exists) before creating a new one
        DestroyInterstitial();

        // create a new interstitial ad
        adMobInterstitial = new InterstitialAd(adMobSettings.InterstitialID);

        adMobInterstitial.OnAdLoaded += HandleAdMobInterstitialAdLoaded;
        adMobInterstitial.OnAdFailedToLoad += HandleAdMobInterstitialAdFailedToLoad;

        // load the interstital ad
        adMobInterstitial.LoadAd(new AdRequest.Builder().Build());
    }

    /// <summary>
    /// Loads a rewarded video ad. To show the rewarded video ad, the 
    /// ShowRewardedVideoAd() needs to be called, like the interstitial
    /// method, this method will only load the rewarded video ad.
    /// </summary>
    static void RequestAdMobRewardedVideoAd()
    {
        // get the singleton of rewarded video ads
        adMobRewardVideo = RewardBasedVideoAd.Instance;

        adMobRewardVideo.OnAdRewarded += HandleAdMobRewardedVideoComplete;
        adMobRewardVideo.OnAdFailedToLoad += HandleAdMobRewardedVideoFailedToLoad;

        // load the rewarded video ad
        adMobRewardVideo.LoadAd(new AdRequest.Builder().Build(), adMobSettings.RewardedVideoID);
    }

    /// <summary>
    /// Only switch to the game whenever the ad is loaded to avoid any strange bugs
    /// from happening. Primarily the banner ad popping up in the middle of a game
    /// and not being able to destroy it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobBannerAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Banner Loaded");
        OnBannerLoaded?.Invoke();
    }

    /// <summary>
    /// Starts the game after 5 - 7 seconds if the banner ad cannot be loaded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Banner Failed to Load");
        OnBannerFailedToLoad?.Invoke();
    }

    /// <summary>
    /// This method handles the events that should occur after an interstitial ad is loaded.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobInterstitialAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Interstitial Loaded");
        OnIntersititialLoaded?.Invoke();
    }

    /// <summary>
    /// Handle for whenever an interstitial fails to load (i.e. no internet connection).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobInterstitialAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Interstitial Failed to Load");
        OnIntersittialFailedToLoad?.Invoke();
    }

    /// <summary>
    /// This handles the events that should occur after the rewarded video ad is completed and 
    /// closed by the user.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobRewardedVideoComplete(object sender, Reward args)
    {
        DestroyBanner();
        gameSettings.RestartGameFromWatchingAd();
        SceneManager.LoadScene(gameSettings.mainSceneName);

        /*.Log("Rewarded Video Loaded");
        OnRewardedVideoComplete?.Invoke();*/
    }

    /// <summary>
    /// Handle for whenever an rewarded video fails to load (i.e. no internet connection).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    static void HandleAdMobRewardedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Rewarded Video Failed to Load");
        OnRewardedVideoFailedToLoad?.Invoke();
    }

    #endregion


    #region Ad Utilites

    /// <summary>
    /// Should be called whenever the game exits.
    /// </summary>
    public static void CleanUp()
    {
        DestroyBanner(true); // actually destroy the banner
        DestroyInterstitial();
    }

    /// <summary>
    /// Destroys any banner ads on the screen. However, under normal circumstances, the banner will only be
    /// hidden and not actually destroyed. If the banner is force destroyed, then the banner will then be actually
    /// destroyed.
    /// </summary>
    /// <param name="forceDestroy"></param>
    public static void DestroyBanner(bool forceDestroy = false)
    {
        // destroy Unity banner ad (if specified), else hide it
        /*if (Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.Hide(forceDestroy);
        }*/

        // destroy AdMob banner ad (if specified), else hide it
        if (adMobBanner != null)
        {
            if (forceDestroy)
                adMobBanner.Destroy();
            else
                adMobBanner.Hide();
        }
    }

    /// <summary>
    /// Destroys any interstitial ads on the screen.
    /// </summary>
    public static void DestroyInterstitial()
    {
        if (adMobInterstitial != null)
            adMobInterstitial.Destroy();
    }

    #endregion
}

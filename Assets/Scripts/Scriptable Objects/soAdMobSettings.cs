
using UnityEngine;

[CreateAssetMenu(fileName = "AdMobSettings", menuName = "Settings/SO AdMob")]
public class soAdMobSettings : ScriptableObject
{
    [Header("Settings:")]
    [SerializeField] bool testMode;

    [Header("AdMob ID:")]
    [SerializeField] string androidAppID;
    [SerializeField] string iosAppID;

    [Header("Android Test Ads:")]
    [SerializeField] string androidTestBanner;
    [SerializeField] string androidTestInterstitial;
    [SerializeField] string androidTestRewardedVideo;

    [Header("iOS Test Ads:")]
    [SerializeField] string iosTestBanner;
    [SerializeField] string iosTestInterstitial;
    [SerializeField] string iosTestRewardedVideo;


    [Header("Android Live Ads:")]
    [SerializeField] string androidLiveBanner;
    [SerializeField] string androidLiveInterstitial;
    [SerializeField] string androidLiveRewardedVideo;

    [Header("iOS Live Ads:")]
    [SerializeField] string iosLiveBanner;
    [SerializeField] string iosLiveInterstitial;
    [SerializeField] string iosLiveRewardedVideo;


    /// <summary>
    /// Gets the respective app ID based on the current platform.
    /// </summary>
    public string AppID
    {
        get { return GameUtilities.IsAndroid ? androidAppID : iosAppID; }
    }

    /// <summary>
    /// Gets the respective banner ID based on the current platform.
    /// </summary>
    public string BannerID
    {
        get
        {
            if (testMode)
                return GameUtilities.IsAndroid ? androidTestBanner : iosTestBanner;
            else
                return GameUtilities.IsAndroid ? androidLiveBanner : iosLiveBanner;
        }
    }

    /// <summary>
    /// Gets the respective interstitial ID based on the current platform.
    /// </summary>
    public string InterstitialID
    {
        get
        {
            if (testMode)
                return GameUtilities.IsAndroid ? androidTestInterstitial : iosTestInterstitial;
            else
                return GameUtilities.IsAndroid ? androidLiveInterstitial : iosLiveInterstitial;
        }
    }

    /// <summary>
    /// Gets the respective banner ID based on the current platform.
    /// </summary>
    public string RewardedVideoID
    {
        get
        {
            if (testMode)
                return GameUtilities.IsAndroid ? androidTestRewardedVideo : iosTestRewardedVideo;
            else
                return GameUtilities.IsAndroid ? androidLiveRewardedVideo : iosLiveRewardedVideo;
        }
    }
}

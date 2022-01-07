
using UnityEngine;

[CreateAssetMenu(fileName = "UnityAdsSettings", menuName = "Settings/SO Unity Ads")]
public class soUnityAdsSettings : ScriptableObject
{
    [Header("Unity App ID:")]
    [SerializeField] string androidAppID;
    [SerializeField] string iosAppID;

    [Header("Unity Ad Types:")]
    [SerializeField] string banner;
    [SerializeField] string interstitial;
    [SerializeField] string rewardedVideo;


    /// <summary>
    /// Gets the unity app ID.
    /// </summary>
    public string AppID
    {
        get { return GameUtilities.IsAndroid ? androidAppID : iosAppID; }
    }

    /// <summary>
    /// Gets the unity banner string.
    /// </summary>
    public string BannerType
    {
        get { return banner; }
    }

    /// <summary>
    /// Gets the unity intersitital string.
    /// </summary>
    public string InterstitialType
    {
        get { return interstitial; }
    }

    /// <summary>
    /// Gets the unity rewarded video string.
    /// </summary>
    public string RewardedVideoType
    {
        get { return rewardedVideo; }
    }
}
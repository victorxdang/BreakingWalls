
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISettings : UIFading
{
    [SerializeField] UISwitch swipeTiltSwitch; 
    [SerializeField] Button privacyPolicyButton;
    [SerializeField] Button creditsButton;
    [SerializeField] Button returnButton;
    [SerializeField] Button tutorialButton;
    [SerializeField] soURLs URLs;
    [SerializeField] soGameSettings gameSettings;

    [Header("Google Play Games Service:")]
    [SerializeField] GameObject gpgsButtonHolder;
    [SerializeField] Button achievementButton;
    [SerializeField] Button signInButton;
    [SerializeField] Button leaderboardButton;


    public bool GPGSButtonActive
    {
        get { return gpgsButtonHolder.activeSelf; }
        set { gpgsButtonHolder.SetActive(value); }
    }


    Text signInButtonText;


    protected override void Awake()
    {
        base.Awake();

        swipeTiltSwitch.onClick.AddListener((status) => gameSettings.swipe = status == SwitchStatus.Left); // left = swipe, right = tilt
        privacyPolicyButton.onClick.AddListener(() => Application.OpenURL(URLs.PrivacyPolicyURL));

        signInButton.onClick.AddListener(GooglePlayGamesService.SignIn);
        achievementButton.onClick.AddListener(GooglePlayGamesService.ShowAchievementsUI);
        leaderboardButton.onClick.AddListener(GooglePlayGamesService.ShowLeaderboardUI);
        signInButtonText = signInButton.transform.GetChild(0).GetComponent<Text>();


        SetupGPGS(false);

        if (GameUtilities.IsAndroid)
            GooglePlayGamesService.OnSignedIn += SetupGPGS;
    }

    void Start()
    {
        swipeTiltSwitch.OverrideSwitchStatus(gameSettings.swipe ? SwitchStatus.Left : SwitchStatus.Right);
    }

    void OnDestroy()
    {
        GooglePlayGamesService.OnSignedIn -= SetupGPGS;
    }

    public override void SetButtonsInteractable(bool interactable)
    {
        privacyPolicyButton.interactable = interactable;
        creditsButton.interactable = interactable;
        returnButton.interactable = interactable;
        tutorialButton.interactable = interactable;
    }

    public void SetCreditsCallback(UnityAction callback)
    {
        creditsButton.onClick.AddListener(callback);
    }

    public void SetReturnCallback(UnityAction callback)
    {
        returnButton.onClick.AddListener(callback);
    }

    public void SetTutorialCallback(UnityAction callback)
    {
        tutorialButton.onClick.AddListener(callback);
    }

    void SetupGPGS(bool signedIn)
    {
        achievementButton.gameObject.SetActive(signedIn);
        leaderboardButton.gameObject.SetActive(signedIn);
        signInButtonText.text = signedIn ? "Sign In" : "Sign Out";
    }
}

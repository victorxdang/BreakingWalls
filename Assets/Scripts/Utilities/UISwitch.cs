
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISwitch : MonoBehaviour
{
    public SwitchEvent onClick = new SwitchEvent();


    [Header("Status:")]
    [SerializeField] bool interactable = true;
    [SerializeField] SwitchStatus status = SwitchStatus.Left;
    [Range(0f, 1f)]
    [SerializeField] float switchSpeed = 0.25f; // in seconds

    [Header("Settings:")]
    [SerializeField] string switchLeftText;
    [SerializeField] string switchRightText;
    [SerializeField] Color backgroundColor = Color.white;
    [SerializeField] Color handleColor = Color.white;


    [Header("Components:")]
    [SerializeField] Button backgroundButton;
    [SerializeField] Image handleImage;
    [SerializeField] Text leftText;
    [SerializeField] Text rightText;


    public bool Interactable
    {
        get 
        { 
            return backgroundButton.interactable;
        }
        set 
        {
            interactable = value;
            backgroundButton.interactable = value; 
        }
    }

    public SwitchStatus Status
    {
        get { return status; }
    }


    bool movingSwitch;
    Vector2 leftPosition;
    Vector2 rightPosition;
    RectTransform buttonRectTransform;



    void Awake()
    {
        buttonRectTransform = backgroundButton.GetComponent<RectTransform>();
        backgroundButton.onClick.AddListener(UpdateSwitchStatus);

        leftPosition = new Vector2(-buttonRectTransform.rect.width / 4.0f, 0);
        rightPosition = new Vector2(buttonRectTransform.rect.width / 4.0f, 0);

        backgroundButton.interactable = interactable;
    }

    void OnValidate()
    {
        if (backgroundButton != null)
        {
            ColorBlock cb = backgroundButton.colors;
            cb.normalColor = backgroundColor;
            backgroundButton.colors = cb;
        }

        if (handleImage != null)
            handleImage.color = handleColor;

        if (leftText != null)
            leftText.text = switchLeftText;

        if (rightText != null)
            rightText.text = switchRightText;

        if (buttonRectTransform == null)
            buttonRectTransform = backgroundButton.GetComponent<RectTransform>();

        leftPosition = new Vector2(-buttonRectTransform.rect.width / 4.0f, 0);
        rightPosition = new Vector2(buttonRectTransform.rect.width / 4.0f, 0);

        handleImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        handleImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        if (status == SwitchStatus.Left)
            handleImage.rectTransform.anchoredPosition = leftPosition;
        else if (status == SwitchStatus.Right)
            handleImage.rectTransform.anchoredPosition = rightPosition;

        backgroundButton.interactable = interactable;
    }

    public void OverrideSwitchStatus(SwitchStatus switchStatus)
    {
        if (movingSwitch)
            LeanTween.cancel(gameObject, true);

        if (switchStatus == SwitchStatus.Left)
            MoveHandleLeft(false);
        else if (switchStatus == SwitchStatus.Right)
            MoveHandleRight(false);
    }

    void UpdateSwitchStatus()
    {
        if (movingSwitch)
            return;

        if (status == SwitchStatus.Left)
            MoveHandleRight();
        else if (status == SwitchStatus.Right)
            MoveHandleLeft();

        onClick?.Invoke(status);
    }

    void MoveHandleLeft(bool animate = true)
    {
        status = SwitchStatus.Left;

        if (animate && switchSpeed > 0)
            AnimateHandleTo(leftPosition);
        else
            handleImage.rectTransform.anchoredPosition = leftPosition;
    }

    void MoveHandleRight(bool animate = true)
    {
        status = SwitchStatus.Right;

        if (animate && switchSpeed > 0)
            AnimateHandleTo(rightPosition);
        else
            handleImage.rectTransform.anchoredPosition = rightPosition;
    }

    void AnimateHandleTo(Vector2 newHandlePosition)
    {
        if (!movingSwitch)
        {
            movingSwitch = true;
            LeanTween.move(handleImage.rectTransform, newHandlePosition, switchSpeed)
                .setIgnoreTimeScale(true)
                .setOnComplete(() => movingSwitch = false);
        }
    }
}

public class SwitchEvent : UnityEvent<SwitchStatus>
{
    // empty class
}
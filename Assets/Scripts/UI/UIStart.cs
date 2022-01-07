
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIStart : UIFading
{
    [SerializeField] Button startButton;
    [SerializeField] Button settingsButton;



    public override void SetButtonsInteractable(bool interactable)
    {
        startButton.interactable = interactable;
        settingsButton.interactable = interactable;
    }

    public void SetStartGameCallback(UnityAction callback)
    {
        startButton.onClick.AddListener(callback);
    }

    public void SetSettingsCallback(UnityAction callback)
    {
        settingsButton.onClick.AddListener(callback);
    }
}

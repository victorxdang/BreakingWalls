
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIPause : UIFading
{
    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button settingsButton;



    public void SetResumeButtonCallback(UnityAction callback)
    {
        resumeButton.onClick.AddListener(callback);
    }

    public void SetRestartButtonCallback(UnityAction callback)
    {
        restartButton.onClick.AddListener(callback);
    }

    public void SetQuitButtonCallback(UnityAction callback)
    {
        quitButton.onClick.AddListener(callback);
    }

    public void SetSettingsButtonCallback(UnityAction callback)
    {
        settingsButton.onClick.AddListener(callback);
    }

    public override void SetButtonsInteractable(bool interactable)
    {
        resumeButton.interactable = interactable;
        restartButton.interactable = interactable;
        quitButton.interactable = interactable;
        settingsButton.interactable = interactable;
    }
}

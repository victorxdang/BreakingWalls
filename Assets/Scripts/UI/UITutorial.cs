
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITutorial : UIFading
{
    [SerializeField] Button exitTutorialButton;



    public override void SetButtonsInteractable(bool interactable)
    {
        exitTutorialButton.interactable = interactable;
    }

    public void SetExitTutorialCallback(UnityAction callback)
    {
        exitTutorialButton.onClick.AddListener(callback);
    }
}

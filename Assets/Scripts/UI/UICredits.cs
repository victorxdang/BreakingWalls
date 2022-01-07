
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UICredits : UIFading
{
    [SerializeField] Button zapSplatButton;
    [SerializeField] Button sappheirosButton;
    [SerializeField] Button exitCreditsButton;
    [SerializeField] soURLs URLs;


    protected override void Awake()
    {
        base.Awake();
        zapSplatButton.onClick.AddListener(() => Application.OpenURL(URLs.ZapSplatURL));
        sappheirosButton.onClick.AddListener(() => Application.OpenURL(URLs.SappheirosURL));
    }

    public void SetExitCreditsCallback(UnityAction callback)
    {
        exitCreditsButton.onClick.AddListener(callback);
    }

    public override void SetButtonsInteractable(bool interactable)
    {
        zapSplatButton.interactable = interactable;
        sappheirosButton.interactable = interactable;
        exitCreditsButton.interactable = interactable;
    }
}

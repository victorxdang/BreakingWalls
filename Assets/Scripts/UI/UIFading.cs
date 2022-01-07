
using System;
using UnityEngine;

public class UIFading : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] protected bool shouldFade = true;
    [Range(0, 10)]
    [SerializeField] protected float fadeSpeed = 1;

    [Header("Components:")]
    [SerializeField] protected GameObject canvasHolder;


    public Action OnFadeStart;
    public Action OnFadeEnd;


    public bool CanvasHolderActive
    {
        get 
        {
            GetCanvasGroup();
            return canvasHolder.gameObject.activeSelf && canvasGroup.alpha == 1;
        }
        set 
        { 
            canvasHolder.gameObject.SetActive(value); 
        }
    }

    public float Alpha
    {
        get 
        {
            GetCanvasGroup();
            return canvasGroup.alpha; 
        }
        set
        {
            GetCanvasGroup();
            canvasGroup.alpha = Mathf.Clamp01(value);
        }
    }


    protected bool fadingIn;
    protected bool fadingOut;
    protected CanvasGroup canvasGroup;



    protected virtual void Awake()
    {
        GetCanvasGroup();
    }

    void OnDestroy()
    {
        OnFadeStart = null;
        OnFadeEnd = null;
    }

    void OnValidate()
    {
        if (canvasHolder == null)
            return;

        if (canvasHolder.GetComponent<CanvasGroup>() == null)
            canvasHolder.AddComponent<CanvasGroup>();
    }

    void GetCanvasGroup()
    {
        if (canvasGroup == null)
            canvasGroup = canvasHolder.GetComponent<CanvasGroup>();
    }

    public void EnableCanvasHolder()
    {
        Alpha = 1;
        CanvasHolderActive = true;
    }

    public void DisableCanvasHolder()
    {
        Alpha = 0;
        CanvasHolderActive = false;
    }

    public virtual void SetButtonsInteractable(bool interactable)
    {
        // to be overridden by inherited class
    }

    public void FadeIn(bool setButtonsActive = true)
    {
        if ((fadeSpeed == 0) || fadingIn || fadingOut)
        {
            FadeInComplete();
            return;
        }


        fadingIn = true;
        fadingOut = false;
        canvasGroup.gameObject.SetActive(true);
        SetButtonsInteractable(false);

        OnFadeStart?.Invoke();
        LeanTween.alphaCanvas(canvasGroup, 1, fadeSpeed)
            .setFrom(0)
            .setIgnoreTimeScale(true)
            .setOnComplete(FadeInComplete);


        void FadeInComplete()
        {
            fadingIn = false;
            canvasGroup.alpha = 1;
            SetButtonsInteractable(setButtonsActive);
            OnFadeEnd?.Invoke();
        }
    }

    public void FadeOut(bool setButtonsActive = false)
    {
        if (fadeSpeed == 0 || fadingIn || fadingOut)
        {
            FadeOutComplete();
            return;
        }


        fadingIn = false;
        fadingOut = true;
        SetButtonsInteractable(false);

        OnFadeStart?.Invoke();
        LeanTween.alphaCanvas(canvasGroup, 0, fadeSpeed)
            .setFrom(1)
            .setIgnoreTimeScale(true)
            .setOnComplete(FadeOutComplete);


        void FadeOutComplete()
        {
            fadingOut = false;
            canvasGroup.alpha = 0;
            canvasGroup.gameObject.SetActive(setButtonsActive);
            OnFadeEnd?.Invoke();
        }
    }
}

/*****************************************************************************************************************
 - GifAnimator.cs -
-----------------------------------------------------------------------------------------------------------------
 Script Type:       Utility
 Author(s):         Victor Dang
 Game Name:         Journey To Ascension
 Engine Version:    Unity 2019.2.16f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Animates a GIF that has been split into multiple frames. This class will only work with UI objects!
*****************************************************************************************************************/

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class GifAnimator : MonoBehaviour
{
    [SerializeField] float gifSpeed = 25;
    [SerializeField] Sprite[] gifSprites;

    RectTransform thisRectTransform;


    /// <summary>
    /// Starts the animation of the GIF using a tweening system.
    /// </summary>
    void Start()
    {
        thisRectTransform = GetComponent<RectTransform>();
        LeanTween.play(thisRectTransform, gifSprites).setSpeed(gifSpeed).setRepeat(-1).setIgnoreTimeScale(true);
    }

    /// <summary>
    /// Resumes tweening on this game object when it is enabled.
    /// </summary>
    void OnEnable()
    {
        LeanTween.resume(gameObject);
    }

    /// <summary>
    /// Stops tweening on this game object when it is disabled.
    /// </summary>
    void OnDisable()
    {
        LeanTween.pause(gameObject);
    }
}
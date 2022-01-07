
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/SO Audio")]
public class soAudioClips : ScriptableObject
{
    #region Constants

    /// <summary>
    /// The volume of the background music.
    /// </summary>
    public const float BGM_VOLUME = 1;

    /// <summary>
    /// This is how loud the sound effect will play when triggered.
    /// </summary>
    public const float SE_VOLUME = 1;

    #endregion


    [Header("BGM:")]
    [SerializeField] AudioClip[] backgroundMusic;

    [Header("SE:")]
    [SerializeField] AudioClip wallDestroyClip;
    [SerializeField] AudioClip ballColorSwitchClip;
    [SerializeField] AudioClip ballMovingSideClip;
    [SerializeField] AudioClip ballWrongColorClip;


    public AudioClip[] BGM { get { return backgroundMusic; } }

    public AudioClip WallDestroyedSE { get { return wallDestroyClip; } }

    public AudioClip BallColorSwitchSE { get { return ballColorSwitchClip; } }

    public AudioClip BallMovingSideSE { get { return ballMovingSideClip; } }

    public AudioClip BallWrongColorSE { get { return ballWrongColorClip; } }
}
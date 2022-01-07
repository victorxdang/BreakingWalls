
/*****************************************************************************************************************
 - Audio.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This class handles audio playing and such. This class will persist and there will only be one instance 
     of this class per game.
*****************************************************************************************************************/

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] soAudioClips backgroundMusic;


    AudioClip nextClip; // the next bgm clip to be played
    AudioSource source; // audio plays from here


    /// <summary>
    /// Awake is called before start, whenever the object is created.
    /// </summary>
    void Awake()
    {
        if (FindObjectsOfType<AudioManager>().Length == 1)
        {
            source = GetComponent<AudioSource>();
            nextClip = GetRandomNextBGM();

            StartCoroutine(IEnumPlayBGM());
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Stops the background music coroutine.
    /// </summary>
    void OnDestroy()
    {
        StopAllCoroutines();
    }


    #region Background Music

    /// <summary>
    /// Automatically plays audio clips by randomly selecting a clip to play.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumPlayBGM()
    {
        while (true)
        {
            PlayClip(nextClip, soAudioClips.BGM_VOLUME);
            nextClip = GetRandomNextBGM();
            yield return GameUtilities.Wait(source.clip.length);
        }
    }

    /// <summary>
    /// Plays the specified clip.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    void PlayClip(AudioClip clip, float volume = 1)
    {
        source.volume = volume;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Selects a random bgm audio clip to play. This also makes sure that the same clip isn't
    /// played over again.
    /// </summary>
    AudioClip GetRandomNextBGM()
    {
        int randomClip;

        do
        {
            randomClip = Random.Range(0, backgroundMusic.BGM.Length);
        } while (backgroundMusic.BGM[randomClip] == nextClip);

        return backgroundMusic.BGM[randomClip];
    }

    #endregion
}

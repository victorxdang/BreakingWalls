using System;
using System.Collections;
using UnityEngine;

public static class GameUtilities
{
    public static bool IsAndroid
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return true;
            #else
            return false;
            #endif
        }
    }

    public static bool IsIOS
    {
        get
        {
            #if UNITY_IOS && !UNITY_EDITOR
            return true;
            #else
            return false;
            #endif
        }
    }



    public static void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    /// <summary>
    /// Returns a random element from the params array. Useful if elements are implemented as fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <returns></returns>
    public static T GetRandomElementFrom<T>(params T[] elements)
    {
        return elements[UnityEngine.Random.Range(0, elements.Length)];
    }

    /// <summary>
    /// Waits for the specified amount of time.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator Wait(float seconds)
    {
        float time = 0;

        while (time < seconds)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Waits for the specified amount of time the executes callback.
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static IEnumerator Wait(float seconds, Action callback)
    {
        float time = 0;

        while (time < seconds)
        {
            time += Time.deltaTime;
            yield return null;
        }

        callback?.Invoke();
    }

    /// <summary>
    /// Waits for the specified amount of time. Will exit the coroutine if cancelCondition() is true.
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="cancelCondition"></param>
    /// <returns></returns>
    public static IEnumerator Wait(float seconds, Func<bool> cancelCondition)
    {
        float time = 0;

        while (time < seconds && !cancelCondition())
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Waits for the specified amount of time. Will exit the coroutine if cancelCondition() is true and will NOT execute the callback.
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="callback"></param>
    /// <param name="cancelCondition"></param>
    /// <returns></returns>
    public static IEnumerator Wait(float seconds, Action callback, Func<bool> cancelCondition)
    {
        float time = 0;

        while (time < seconds && !cancelCondition())
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (!cancelCondition())
            callback?.Invoke();
    }

    /// <summary>
    /// Will continuously execute until condition is false.
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static IEnumerator WaitWhile(Func<bool> condition)
    {
        while (condition())
        {
            yield return null;
        }
    }
}

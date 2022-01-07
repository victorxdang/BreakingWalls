
/*****************************************************************************************************************
 - UILoading.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Handles all of the scene loading and loading UI components.
*****************************************************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILoading : UIFading
{
    [SerializeField] Slider progressSlider; 


    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LoadScene(string sceneName, LoadSceneMode mode)
    {
        StartCoroutine(IEnumLoadScene(sceneName, mode));
    }

    IEnumerator IEnumLoadScene(string sceneName, LoadSceneMode mode)
    {
        if (shouldFade)
        {
            CanvasHolderActive = true;
            FadeIn();
            yield return GameUtilities.WaitWhile(() => fadingIn);
        }

        
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!loadingScene.isDone)
        {
            // progress bar can be added here if desired
            yield return null;
        }


        if (shouldFade)
        {
            FadeOut();
            yield return GameUtilities.WaitWhile(() => fadingOut);
            CanvasHolderActive = false;
        }
    }
}

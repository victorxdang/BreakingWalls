
/*****************************************************************************************************************
 - BlockDespawner.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Platform despawning is handled here. Despawns the platform and its attached path a few seconds after
     the ball has rolled through the trigger.
*****************************************************************************************************************/

using System;
using UnityEngine;

public class BlockDespawner : MonoBehaviour
{
    #region Constants

    /// <summary>
    /// The tag of the initial platform (just the initial platform).
    /// </summary>
    const string INITIAL_PLATFORM_TAG = "InitialPlatform";

    /// <summary>
    /// The tag of the ball.
    /// </summary>
    const string BALL_TAG = "Ball";

    #endregion


    public static Action<Platform> OnPlatformDespawn;

    Platform parentPlatform;



    /// <summary>
    /// Grabs the parent's platform reference.
    /// </summary>
    void Awake()
    {
        parentPlatform = transform.parent.GetComponent<Platform>();
    }

    /// <summary>
    /// Destroys this game object and spawns in a new platform at the next attach point once 
    /// the ball reaches this point.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(BALL_TAG))
        {
            float delay = (parentPlatform.GameSettings.platformSpeed < soGameSettings.PLATFORM_SPEED_HARDCAP / 2.0f) 
                ? parentPlatform.DestrucibleWallDespawnTime 
                : parentPlatform.DestrucibleWallDespawnTime / 2.0f;

            LeanTween.delayedCall(delay, () =>
            {
                if (parentPlatform.GameSettings.gameOver)
                    return;

                if (transform.parent.CompareTag(INITIAL_PLATFORM_TAG))
                    Destroy(transform.parent.gameObject);
                else
                    OnPlatformDespawn?.Invoke(parentPlatform);
            });
        }
    }
}

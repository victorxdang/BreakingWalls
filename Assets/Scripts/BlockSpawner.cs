
/*****************************************************************************************************************
 - BlockSpawner.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Platform/path spawning is handled here. 
*****************************************************************************************************************/

using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] int maxNumberOfPlatforms = 5;

    [Header("Components:")]
    [SerializeField] Transform platformHolder;
    [SerializeField] Platform initialPlatform;
    [SerializeField] Platform prefabPlatform;


    Transform nextPlatformAttachPoint;


    /// <summary>
    /// Called when the game object is enabled.
    /// </summary>
    void Start()
    {
        BlockDespawner.OnPlatformDespawn += RespawnPlatform;
        nextPlatformAttachPoint = initialPlatform.NextPlatformAttachPoint;

        for (int i = 0; i < maxNumberOfPlatforms; ++i)
        {
            Platform platform = Instantiate(prefabPlatform, platformHolder);
            platform.RespawnPlatform(nextPlatformAttachPoint);
            nextPlatformAttachPoint = platform.NextPlatformAttachPoint;
        }
    }

    /// <summary>
    /// Removes all action pointers.
    /// </summary>
    void OnDisable()
    {
        BlockDespawner.OnPlatformDespawn -= RespawnPlatform;
    }

    /// <summary>
    /// Respawns the platform and sets the new platform attach point.
    /// </summary>
    /// <param name="platform"></param>
    void RespawnPlatform(Platform platform)
    {
        platform.RespawnPlatform(nextPlatformAttachPoint);
        nextPlatformAttachPoint = platform.NextPlatformAttachPoint;
    }
}

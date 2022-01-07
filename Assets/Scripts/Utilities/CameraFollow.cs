
/*****************************************************************************************************************
 - CameraFollow.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Follows the specified transform.
*****************************************************************************************************************/

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform follow;

    Vector3 followPosition;


    /// <summary>
    /// Grabs the camera's initial position.
    /// </summary>
    void Awake()
    {
        followPosition = transform.position;
    }

    /// <summary>
    /// Updates the camera position so that it follows the specified transform.
    /// </summary>
    void LateUpdate()
    {
        if (follow != null)
        {
            followPosition.x = follow.position.x;
            transform.position = followPosition;
        }
    }
}

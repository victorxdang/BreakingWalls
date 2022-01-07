
/*****************************************************************************************************************
 - DestructibleBlock.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Contains all of the references for the individual pieces of this block.
*****************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    const float MAX_FORCE = 10.0f;

    List<Transform> children = new List<Transform>();
    List<Vector3> startingLocalPositions = new List<Vector3>();
    List<Renderer> renderers = new List<Renderer>();
    List<Rigidbody> rigidbodies = new List<Rigidbody>();


    /// <summary>
    /// Grabs all of the children of this game object.
    /// </summary>
    void Awake()
    {
        foreach (Transform child in transform)
        {
            children.Add(child);
            startingLocalPositions.Add(child.localPosition);
            renderers.Add(child.GetComponent<Renderer>());
            rigidbodies.Add(child.GetComponent<Rigidbody>());
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Return all of the pieces back to the original position.
    /// </summary>
    void OnDisable()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            children[i].localPosition = startingLocalPositions[i];
        }
    }

    /// <summary>
    /// Called when the ball successfully smashes into this wall.
    /// </summary>
    /// <param name="despawnTime"></param>
    /// <param name="newColor"></param>
    public void OnWallDestroyed(float despawnTime, Color newColor)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            renderers[i].material.color = newColor;
            rigidbodies[i].AddForce(Random.Range(-MAX_FORCE, MAX_FORCE), Random.Range(-MAX_FORCE, MAX_FORCE), 0);
        }

        LeanTween.delayedCall(despawnTime, () => gameObject.SetActive(false));
    }
}

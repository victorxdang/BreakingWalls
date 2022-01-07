
/*****************************************************************************************************************
 - UpdateManager.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Consolidates all updating game objects to one class and one update loop.
*****************************************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public sealed class UpdateManager : MonoBehaviour
{
    static List<IUpdatable> list_updatables = new List<IUpdatable>();


    void Update()
    {
        for (int i = 0; i < list_updatables.Count; i++)
        {
            list_updatables[i].OnUpdate();
        }
    }

    public static bool Register(IUpdatable item)
    {
        if (list_updatables.Contains(item))
            return false;

        list_updatables.Add(item);
        return true;
    }

    public static bool Unregister(IUpdatable item)
    {
        return list_updatables.Remove(item);
    }
}

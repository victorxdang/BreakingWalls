using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugSettings", menuName = "Settings/SO Debug")]
public class soDebugSettings : ScriptableObject
{
    [SerializeField] bool movePlatform = true;
    [SerializeField] bool invincibleBall = false;
    [SerializeField] bool skipLoadingScreen = false;


    public bool MovePlatform
    {
        get { return movePlatform; }
        set { movePlatform = value; }
    }

    public bool InvincibleBall
    {
        get { return invincibleBall; }
        set { invincibleBall = value; }
    }

    public bool SkipLoadingScreen
    {
        get { return skipLoadingScreen; }
        set { skipLoadingScreen = value; }
    }
}

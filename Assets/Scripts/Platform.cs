
/*****************************************************************************************************************
 - Platform.cs -
-----------------------------------------------------------------------------------------------------------------
 Author:             Victor Dang
 Game/Program Name:  Breaking Walls
 Engine Version:     Unity 2018.2.10f1
-----------------------------------------------------------------------------------------------------------------
 Description: 
     This script handles platform moving (as opposed to the ball moving in previous versions of the game).
*****************************************************************************************************************/

using UnityEngine;

public class Platform : MonoBehaviour, IUpdatable
{
    #region Constants

    /// <summary>
    /// The number of walls per platform.
    /// </summary>
    const int NUMBER_OF_LANES = 3;

    /// <summary>
    /// The x-position of where the platform will initially spawn.
    /// </summary>
    const float SPAWN_POS_X = 75;

    /// <summary>
    /// The y-position of where the platform will initially spawn.
    /// </summary>
    const float SPAWN_POS_Y = 75;

    #endregion


    [Header("Settings:")]
    [SerializeField] bool initialPlatform = false;
    [SerializeField] float transitionTime = 1;
    [SerializeField] float destrucibleWallDespawnTime = 3;
    [SerializeField] float colorCycleInterval = 5;
    [SerializeField] soGameSettings gameSettings;

    [Header("Walls:")]
    [SerializeField] Wall leftWall;
    [SerializeField] Wall middleWall;
    [SerializeField] Wall rightWall;

    [Header("Attach Points:")]
    [SerializeField] Transform path;
    [SerializeField] Transform leftAttachPoint;
    [SerializeField] Transform middleAttachPoint;
    [SerializeField] Transform rightAttachPoint;


    /// <summary>
    /// Should this game object be updated by the update manager?
    /// </summary>
    public bool ShouldUpdate 
    { 
        get { return gameObject.activeSelf; } 
    }

    /// <summary>
    /// Returns the amount of time before the destrucible wall should despawn after a wall is destroyed.
    /// </summary>
    public float DestrucibleWallDespawnTime 
    { 
        get { return destrucibleWallDespawnTime; } 
    }

    /// <summary>
    /// The time between color changes for color changing walls.
    /// </summary>
    public float ColorCycleInterval
    {
        get { return colorCycleInterval; }
    }

    /// <summary>
    /// Return the attach point that was set by this platform.
    /// </summary>
    public Transform NextPlatformAttachPoint 
    { 
        get
        {
            if (attachPoint == null)
                attachPoint = path.GetChild(0);

            return attachPoint;
        }
    }

    /// <summary>
    /// Gets the game settings data.
    /// </summary>
    public soGameSettings GameSettings
    {
        get { return gameSettings; }
    }


    Transform attachPoint;

    

    /// <summary>
    /// Called when the game object is created.
    /// </summary>
    void Awake()
    {
        if (initialPlatform)
            SetPathToRandomPoint();
    }

    /// <summary>
    /// Register this game object from the update manager.
    /// </summary>
    void OnEnable()
    {
        UpdateManager.Register(this);
    }

    /// <summary>
    /// Unregister this game object from the update manager.
    /// </summary>
    void OnDisable()
    {
        UpdateManager.Unregister(this);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
	public void OnUpdate()
    {
        // move the platform backwards as the game progresses
        if (gameSettings.GameInProgress)
            transform.Translate(0, 0, -gameSettings.platformSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Moves the platform the spepcified position.
    /// </summary>
    /// <param name="attachPoint"></param>
    public void SetNewPosition(Transform attachPoint)
    {
        LeanTween.move(gameObject, attachPoint, transitionTime);
    }

    /// <summary>
    /// Reloads the platform onto the scene with a new speed. Leave as 0 to maintain the current speed. 
    /// Returns the next platform attach point to move to.
    /// </summary>
    /// <param name="newSpeed"></param>
    /// <returns></returns>
    public void RespawnPlatform(Transform platformAttachPoint)
    {
        SetupPath();
        SetupPlatform(platformAttachPoint);
        SetupWalls();
    }

    void SetupPath()
    {
        SetPathToRandomPoint();

        // scale the path's length with the current platform speed
        Vector3 pathScale = path.transform.localScale;
        pathScale.z = 1 + (gameSettings.platformSpeed / 100.0f);
        path.localScale = pathScale;
    }

    void SetupPlatform(Transform platformAttachPoint)
    {
        float zCoord;
        Vector3 platformScale;
        Vector3 worldAttachPoint = transform.TransformPoint(platformAttachPoint.position);

        // get the world z-coordinate of the attach point to spawn the platform on
        zCoord = worldAttachPoint.z;
        transform.position = GetRandomSpawnPoint(zCoord);

        // scale the platform based on the platform's speed
        platformScale = transform.localScale;
        platformScale.z = 1 + (gameSettings.platformSpeed / 100.0f);
        transform.localScale = platformScale;

        SetNewPosition(platformAttachPoint);
    }

    void SetupWalls()
    {
        int whiteWallCount = 0;

        // setup all three walls here while keeping count of how many white walls have been spawned
        for (int i = 0; i < NUMBER_OF_LANES; i++)
        {
            Wall wall;

            if (i == 0) { wall = leftWall; }
            else if (i == 1) { wall = middleWall; }
            else { wall = rightWall; }

            if (!wall.gameObject.activeSelf)
                wall.gameObject.SetActive(true);

            wall.SetupWall();

            // keep count of the amount of white walls
            if (wall.CurrentColor == wall.DefaultColor)
                ++whiteWallCount;
        }

        // if there are the same amount of white walls as there are number of walls on the platform,
        // then randomly get one of the walls and change its color
        if (whiteWallCount > NUMBER_OF_LANES - 1)
        {
            GameUtilities.GetRandomElementFrom(leftWall, middleWall, rightWall).SetupWall(true);
        }
    }

    /// <summary>
    /// Returns a random positional vector in world coordinates with certain constraints.
    /// </summary>
    /// <param name="zCoord"></param>
    /// <returns></returns>
    Vector3 GetRandomSpawnPoint(float zCoord)
    {
        return new Vector3(Random.Range((int) -SPAWN_POS_X, (int) SPAWN_POS_X),
                           Random.Range((int) -SPAWN_POS_Y, (int) SPAWN_POS_Y),
                           zCoord);
    }

    /// <summary>
    /// Sets the path to one of the three attach points on the platform.
    /// </summary>
    /// <returns></returns>
    void SetPathToRandomPoint()
    {
        Transform attachPoint = GameUtilities.GetRandomElementFrom(leftAttachPoint, middleAttachPoint, rightAttachPoint);
        Vector3 newPosition = attachPoint.position;
        newPosition.z += 2.5f;

        path.position = newPosition;
        path.parent = attachPoint;
    }
}


using UnityEngine;

[CreateAssetMenu(fileName = "Google Play Games Serivce Settings", menuName = "Settings/SO GPGS")]
public class soGPGSSettings : ScriptableObject
{
    [Header("Activate:")]
    [SerializeField] bool enableGPGS = true;

    [Header("Components:")]
    [SerializeField] string cloudSaveFilename;


    public bool EnablePlayServices
    {
        get { return enableGPGS; }
        set { enableGPGS = value; }
    }

    public string CloudSaveFilename
    {
        get { return cloudSaveFilename; }
    }
}

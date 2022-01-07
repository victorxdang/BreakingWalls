
using UnityEngine;

[CreateAssetMenu(fileName = "CreditsURL", menuName = "Settings/SO Credits")]
public class soURLs : ScriptableObject
{
    [SerializeField] string privacyPolicyURL;
    [SerializeField] string zapSplatURL;
    [SerializeField] string sappheirosURL;



    public string PrivacyPolicyURL
    {
        get { return privacyPolicyURL; }
    }

    public string ZapSplatURL
    {
        get { return zapSplatURL; }
    }

    public string SappheirosURL
    {
        get { return sappheirosURL; }
    }
}

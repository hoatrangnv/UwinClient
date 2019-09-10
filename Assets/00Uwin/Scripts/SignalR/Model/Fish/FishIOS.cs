using System.Runtime.InteropServices;
using UnityEngine;

public class FishIOS : MonoBehaviour
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern int isYelpInstalled();

    public static int appFishAlreadyInstalled()
    {
        int isAlready = isYelpInstalled();
        return isAlready;
    }
#endif
}

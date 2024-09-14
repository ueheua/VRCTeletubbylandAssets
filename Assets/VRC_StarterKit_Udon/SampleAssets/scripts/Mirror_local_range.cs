
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mirror_local_range : UdonSharpBehaviour
{
    public GameObject local_tmp_mirror3;

    private void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        //トリガーに侵入したのが自分だった場合
        if (player == Networking.LocalPlayer)
        {
            //鏡をOnにする
            local_tmp_mirror3.SetActive(true);
        }
    }

    private void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        //トリガーに侵入したのが自分だった場合
        if (player == Networking.LocalPlayer)
        {
            //鏡をOffにする
            local_tmp_mirror3.SetActive(false);
        }
    }
}

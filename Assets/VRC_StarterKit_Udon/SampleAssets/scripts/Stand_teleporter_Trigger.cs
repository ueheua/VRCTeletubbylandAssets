using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Stand_teleporter_Trigger : UdonSharpBehaviour
{
    public GameObject teleportPoint;

    public GameObject Teleport_AudioSource;

    private VRCPlayerApi playerlocal;

    private bool infinity_protect = false;

    private AudioSource teleport_SE1;

    void Start()
    {
        playerlocal = Networking.LocalPlayer;
        teleport_SE1 = Teleport_AudioSource.GetComponent<AudioSource>();
    }

    private void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        
        //トリガーに侵入したのが自分だった場合
        if (player == Networking.LocalPlayer && infinity_protect == false )
        {
            playerlocal.TeleportTo(teleportPoint.transform.position, teleportPoint.transform.localRotation);
            infinity_protect = true;

            teleport_SE1.PlayOneShot(teleport_SE1.clip);
        }
    }

    private void OnPlayerTriggerExit(VRCPlayerApi player)
    {

        if (player == Networking.LocalPlayer && infinity_protect == true)
        {
            infinity_protect = false;
        }
    }

}

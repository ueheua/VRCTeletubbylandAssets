
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EntranceSound : UdonSharpBehaviour
{
    public GameObject Entrance_SE_object;

    private AudioSource Join_SE1;

    void Start()
    {
        Join_SE1 = Entrance_SE_object.GetComponent<AudioSource>();
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
 
            Join_SE1.PlayOneShot(Join_SE1.clip);
    }

}

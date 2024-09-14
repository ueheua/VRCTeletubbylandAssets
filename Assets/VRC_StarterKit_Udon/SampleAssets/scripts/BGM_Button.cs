
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BGM_Button : UdonSharpBehaviour
{
    public GameObject BGM_Button1;

    public GameObject BGM_Audio_object;

    private Animator animator1 = null;

    private bool button_state = false;


    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            animator1 = BGM_Button1.GetComponent<Animator>();
        }
    }
    public override void Interact()
    {
        //ローカル(押した人だけ)に「public void mirror_onoff3()」を実行させる命令
        BGM_onoff1();
    }

    public void BGM_onoff1()
    {
        if (button_state == false)
        {
            animator1.SetBool("Bool", true);

            BGM_Button1.GetComponent<Animator>().enabled = true;

            BGM_Audio_object.SetActive(true);

            button_state = true;
        }
        else if (button_state == true)
        {

            animator1.SetBool("Bool", false);

            BGM_Button1.GetComponent<Animator>().enabled = true;

            BGM_Audio_object.SetActive(false);

            button_state = false;
        }
    }


}


using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mirror_local_Button2 : UdonSharpBehaviour
{
    public GameObject[] local_tmp_animeobject1;

    private Animator animator1 = null;

    private Animator animator2 = null;

    private bool button_state = false;

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            animator1 = local_tmp_animeobject1[0].GetComponent<Animator>();
            animator2 = local_tmp_animeobject1[1].GetComponent<Animator>();
        }
    }

    public override void Interact()
    {
        //ローカル(押した人だけ)に「public void screen_onoff()」を実行させる命令
        mirror_onoff2();
    }

    public void mirror_onoff2()
    {
        if (button_state == false)
        {
            animator1.SetBool("Bool", true);
            animator2.SetBool("Bool", true);

            local_tmp_animeobject1[0].GetComponent<Animator>().enabled = true;
            local_tmp_animeobject1[1].GetComponent<Animator>().enabled = true;

            button_state = true;
        }
        else if (button_state == true)
        {
            animator1.SetBool("Bool", false);
            animator2.SetBool("Bool", false);

            local_tmp_animeobject1[0].GetComponent<Animator>().enabled = true;
            local_tmp_animeobject1[1].GetComponent<Animator>().enabled = true;

            button_state = false;
        }
    }
}

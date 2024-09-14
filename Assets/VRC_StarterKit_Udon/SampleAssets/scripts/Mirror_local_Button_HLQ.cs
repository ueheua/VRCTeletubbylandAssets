
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mirror_local_Button_HLQ : UdonSharpBehaviour
{
    public GameObject[] local_tmp_object1;

    public GameObject[] local_tmp_eventobject1;

    private Animator animator1 = null;

    private Animator animator2 = null;

    private bool button_state = false;


    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            animator1 = local_tmp_object1[0].GetComponent<Animator>();
            animator2 = local_tmp_object1[1].GetComponent<Animator>();
        }
    }
    public override void Interact()
    {
        //ローカル(押した人だけ)に「public void mirror_onoff3()」を実行させる命令
        mirror_onoff3();
    }

    public void mirror_onoff3()
    {
        if (button_state == false)
        {
            animator1.SetBool("Bool", true);
            animator2.SetBool("Bool", false);

            for(int i=0; i < local_tmp_object1.Length; i++)
            {
                local_tmp_object1[i].GetComponent<Animator>().enabled = true;
            }

            local_tmp_eventobject1[0].SetActive(true);
            local_tmp_eventobject1[1].SetActive(false);


            button_state = true;
        }
        else if (button_state == true)
        {
            animator1.SetBool("Bool", false);
            animator2.SetBool("Bool", false);

            for (int i = 0; i < local_tmp_object1.Length; i++)
            {
                local_tmp_object1[i].GetComponent<Animator>().enabled = true;
            }

            local_tmp_eventobject1[0].SetActive(true);
            local_tmp_eventobject1[1].SetActive(false);

            button_state = false;
        }
    }
}

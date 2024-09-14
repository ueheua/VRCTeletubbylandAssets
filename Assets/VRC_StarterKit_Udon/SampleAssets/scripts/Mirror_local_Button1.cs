
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Mirror_local_Button1 : UdonSharpBehaviour
{
    public GameObject[] local_tmp_mirror1;

    public GameObject[] local_tmp_icon1;

    public AudioSource button_SE1;

    private bool button_state = false;

    public override void Interact()
    {
        //ローカル(押した人だけ)に「public void mirror_onoff()」を実行させる命令
        mirror_onoff();
    }

    public void mirror_onoff()
    {
        if (button_state == false)
        {
            local_tmp_mirror1[0].SetActive(true);
            local_tmp_mirror1[1].SetActive(false);

            local_tmp_icon1[0].GetComponent<Image>().enabled = false;
            local_tmp_icon1[1].GetComponent<Image>().enabled = true;

            button_SE1.PlayOneShot(button_SE1.clip);

            button_state = true;
        }
        else if (button_state == true)
        {
            local_tmp_mirror1[0].SetActive(false);
            local_tmp_mirror1[1].SetActive(true);

            local_tmp_icon1[0].GetComponent<Image>().enabled = true;
            local_tmp_icon1[1].GetComponent<Image>().enabled = false;

            button_SE1.PlayOneShot(button_SE1.clip);

            button_state = false;
        }

    }

}

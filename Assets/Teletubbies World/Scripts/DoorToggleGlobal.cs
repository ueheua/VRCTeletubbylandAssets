using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class DoorToggleGlobal : UdonSharpBehaviour
{
    
    bool activated = false;

    [Header("初期状態をactiveにする場合チェック")]
    [Header("Check to turn active the initial state.")]
    public bool init = false;

    [Header("")]
    [Header("切り替えたいGameObjectのリスト")]
    [Header("List of GameObjects to switch")]
    public GameObject[] actObjList;

    [Header("")]
    [Header("上のGameObjectとは逆のタイミングで切り替わるGameObjectのリスト")]
    [Header("List of GameObjects that switch at the opposite time from the GameObjects above.")]
    public GameObject[] inverse_actObjList;

    void Start()
    {
        if(init)ActivateObject();
        else InActivateObject();
    }

    public override void Interact()//これをOnPlayerTriggerEnterに変更してもよい
    {
        SetActEvent();
    }
    public void SetActEvent()//これを別のUdonBehaviourからSendCustomEventで呼んだりしてもいい
    {

        if(!Networking.IsOwner(gameObject)){
            activated = !activated;

            if(activated)ActivateObject();
            else InActivateObject();//ラグ感を減らすため、まずローカルで動かします
        }
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner,"RequestToggleObject");
    }

    public void ActivateObject()
    {
        activated = true;
        if(actObjList != null){
            for(int i = 0; i < actObjList.Length; i++)
            {
                actObjList[i].SetActive(true);
            }
        }
        if(inverse_actObjList != null){
            for(int i = 0; i < inverse_actObjList.Length; i++)
            {
                inverse_actObjList[i].SetActive(false);
            }
        }
    }
    public void InActivateObject()
    {
        activated = false;
        if(actObjList != null){
            for(int i = 0; i < actObjList.Length; i++)
            {
                actObjList[i].SetActive(false);
            }
        }
        if(inverse_actObjList != null){
            for(int i = 0; i < inverse_actObjList.Length; i++)
            {
                inverse_actObjList[i].SetActive(true);
            }
        }
        
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(Networking.IsOwner(gameObject))
        {
            NoticeToggleValue();
        }

    }
    private void NoticeToggleValue(){
        if(activated)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"ActivateObject");
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"InActivateObject");
        }
    }

    public void RequestToggleObject()
    {
        activated = !activated;
        
        NoticeToggleValue();
    }



}

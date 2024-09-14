
using UdonSharp;
using UnityEngine;

namespace PurabeWorks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ResetAll : UdonSharpBehaviour
    {
        //ワールド全域を覆うReseterオブジェクト
        [SerializeField] private ReturnObject reseter;
        private GameObject reseterObject;

        private const int waitFlames = 10;

        private void Start()
        {
            if (reseter == null)
            {
                Debug.LogError("[purabe]Reseterが設定されていません。");
            }

            reseterObject = reseter.gameObject;
            //デフォルトでは無効化しておく
            reseterObject.SetActive(false);
        }

        public override void Interact()
        {
            //全リセットを発火
            SendCustomNetworkEvent(
                VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                "TriggerAllReset");
        }

        public void TriggerAllReset()
        {
            //Reseterオブジェクトを有効化
            reseterObject.SetActive(true);
            //指定フレーム後に無効化する
            SendCustomEventDelayedFrames("DisableReseter", waitFlames);
        }

        public void DisableReseter()
        {
            reseterObject.SetActive(false);
        }

        public void Yodo_HapticSwitchTriggered()
        {
            this.Interact();
        }
    }
}

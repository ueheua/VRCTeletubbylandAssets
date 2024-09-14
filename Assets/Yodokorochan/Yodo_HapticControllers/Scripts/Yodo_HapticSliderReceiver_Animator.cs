
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Yodokorochan
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Yodo_HapticSliderReceiver_Animator : UdonSharpBehaviour
    {
        [Header("対象Animator")]
        [SerializeField]
        private Animator Yodo_TargetAnimator;

        [Header("対象Floatパラメタ名")]
        [SerializeField]
        private string Yodo_TargetParameterName;

        [Header("スライダーの値受信用変数")]
        // [HideInInspector]
        // を付けるとInspectorから消えるのでヨシ
        public float Yodo_CurrentSliderValue = 0.0f;

        void Start()
        {
            if(!Yodo_TargetAnimator)
            {
                Debug.LogError($"[Yodo]対象Animatorが空です。[{this.name}]");
            }
            if(Yodo_TargetParameterName == "")
            {
                Debug.LogError($"[Yodo]対象Floatパラメタ名が空です。[{this.name}]");
            }
        }

        public void Yodo_OnSliderValueChanged()
        {
            Yodo_TargetAnimator.SetFloat(Yodo_TargetParameterName, Yodo_CurrentSliderValue);
        }
    }
}

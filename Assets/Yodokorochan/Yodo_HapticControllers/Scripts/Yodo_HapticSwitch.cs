using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Yodokorochan
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Yodo_HapticSwitch : UdonSharpBehaviour
    {
        [UdonSynced]
        [HideInInspector]
        public bool currentStatus = false;

        [Header("振動の強さ")]
        [Range(0.0f, 1.0f)] public float Yodo_HapticStrength = 1.0f;

        [Header("振動の持続時間[s]")]
        [Range(0.0f, 1.0f)] public float Yodo_HapticDuration = 0.2f;

        [Header("OFFを表すオブジェクト")]
        public GameObject Yodo_SwitchOffObject = null;

        [Header("ONを表すオブジェクト")]
        public GameObject Yodo_SwitchOnObject = null;

        [Header("Toggleするオブジェクト")]
        public GameObject[] Yodo_ToggleTargetObject = null;

        [Header("スイッチの初期状態")]
        public bool Yodo_DefaultStatus = false;

        [Header("グローバル同期する")]
        [SerializeField]
        private bool Yodo_IsGlobal = false;

        [Header("VRでも普通のInteractを有効にする")]
        [SerializeField]
        private bool Yodo_InteractiveInVR = false;

        [Header("外部Udon連携(Advanced)")]
        public GameObject[] Yodo_SendCustomEventTarget;

        [Header("外部メソッド(コピペ用)")]
        [SerializeField][TextArea]
        public string CustomEventNames = "public void Yodo_HapticSwitchTriggered(){}\npublic void Yodo_HapticSwitchOn(){}\npublic void Yodo_HapticSwitchOff(){}";

        private bool[] _defaultStatuses;
        private VRCPlayerApi _localPlayer;
        private AudioSource _audio;
        private const float vib_amplitude_coefficient = 0.0636f;    // 0～1じゃないので補正　そのうちVRCのアプデで変わるかも

        void Start()
        {
            currentStatus = Yodo_DefaultStatus;
            _defaultStatuses = new bool[Yodo_ToggleTargetObject.Length];
            for(int cur = 0; cur < Yodo_ToggleTargetObject.Length;cur++)
            {
                if(Yodo_ToggleTargetObject[cur] != null)
                {
                    _defaultStatuses[cur] = Yodo_ToggleTargetObject[cur].activeSelf;
                }
            }
            _audio = GetComponent<AudioSource>();
            _localPlayer = Networking.LocalPlayer;
            UpdateObjects();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player == null) { return; }
            if (!player.IsValid()) { return; }

            if(player.isLocal)
            {
                if(player.IsUserInVR())
                {
                    if(!Yodo_InteractiveInVR)
                    {
                        this.DisableInteractive = true;
                    }
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other == null) { return; }
            UdonBehaviour ub = (UdonBehaviour)other.GetComponent(typeof(UdonBehaviour));
            if (ub == null) { return; }

            if (ub.GetProgramVariableType("Yodo_isHapticCollider") == typeof(bool))
            {
                if ((bool)ub.GetProgramVariable("Yodo_isHapticCollider"))
                {
                    // スイッチをToggle(デスクトップモードと共通)
                    ToggleSwitch();

                    // 振動フィードバック
                    if ((bool)ub.GetProgramVariable("Yodo_VibrateLeftHand"))
                    {
                        _localPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Left, Yodo_HapticDuration, Yodo_HapticStrength * vib_amplitude_coefficient, 320.0f);
                    }
                    if ((bool)ub.GetProgramVariable("Yodo_VibrateRightHand"))
                    {
                        _localPlayer.PlayHapticEventInHand(VRC_Pickup.PickupHand.Right, Yodo_HapticDuration, Yodo_HapticStrength * vib_amplitude_coefficient, 320.0f);
                    }
                }
            }
        }

        public override void Interact()
        {
            ToggleSwitch();
        }

        public override void OnDeserialization()
        {
            if (!Yodo_IsGlobal) { return; }
            UpdateObjects();
        }

        public void ToggleSwitch()
        {
            if(Yodo_IsGlobal)
            {
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
                currentStatus = !currentStatus;
                RequestSerialization();
            }
            else
            {
                currentStatus = !currentStatus;
            }
            UpdateObjects();

            // 効果音があれば再生
            if (_audio != null)
            {
                _audio.Play();
            }

            // 外部Udon連携
            if(Yodo_SendCustomEventTarget != null)
            {
                foreach(GameObject go in Yodo_SendCustomEventTarget)
                {
                    if(go == null) { continue; }
                    UdonBehaviour ub = (UdonBehaviour)go.GetComponent(typeof(UdonBehaviour));
                    if(ub == null) { continue; }
                    if (Yodo_IsGlobal)
                    {
                        ub.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"Yodo_HapticSwitchTriggered");
                        if (currentStatus)
                        {
                            ub.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,"Yodo_HapticSwitchOn");
                        }
                        else
                        {
                            ub.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Yodo_HapticSwitchOff");
                        }
                    }
                    else
                    {
                        ub.SendCustomEvent("Yodo_HapticSwitchTriggered");
                        if (currentStatus)
                        {
                            ub.SendCustomEvent("Yodo_HapticSwitchOn");
                        }
                        else
                        {
                            ub.SendCustomEvent("Yodo_HapticSwitchOff");
                        }
                    }
                }
            }
        }
        private void UpdateObjects() //オブジェクトの状態を設定
        {
            // オブジェクトのActiveを設定
            for (int cur = 0; cur < Yodo_ToggleTargetObject.Length; cur++)
            {
                GameObject go = Yodo_ToggleTargetObject[cur];
                if (go != null)
                {
                    Debug.Log($"def[{Yodo_DefaultStatus}] cur[{currentStatus}] ini [{_defaultStatuses[cur]}] res{(Yodo_DefaultStatus ^ currentStatus) ^ _defaultStatuses[cur]}");
                    go.SetActive((Yodo_DefaultStatus ^ currentStatus) ^ _defaultStatuses[cur]);
                }
            }
            if (Yodo_SwitchOffObject != null)
            {
                Yodo_SwitchOffObject.SetActive(!currentStatus);
            }
            if (Yodo_SwitchOnObject != null)
            {
                Yodo_SwitchOnObject.SetActive(currentStatus);
            }
        }
    }
}

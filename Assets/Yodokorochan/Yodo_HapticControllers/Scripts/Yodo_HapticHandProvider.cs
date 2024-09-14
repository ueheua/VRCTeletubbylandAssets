
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Yodokorochan
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Yodo_HapticHandProvider : UdonSharpBehaviour
    {
        [Header("手でスイッチできるようにする")]
        [SerializeField]
        private bool SwitchWithHands = true;

        [Header("足でスイッチできるようにする")]
        [SerializeField]
        private bool SwitchWithFoots = false;

        [Header("頭突きでスイッチできるようにする")]
        [SerializeField]
        private bool SwitchWithHead = false;

        [Header("左手コライダー")]
        [SerializeField]
        private GameObject LeftHandObject;

        [Header("右手コライダー")]
        [SerializeField]
        private GameObject RightHandObject;

        [Header("左足コライダー")]
        [SerializeField]
        private GameObject LeftFootObject;

        [Header("右手コライダー")]
        [SerializeField]
        private GameObject RightFootObject;

        [Header("頭突きコライダー")]
        [SerializeField]
        private GameObject HeadObject;

        private HumanBodyBones targetBoneLeftHand = HumanBodyBones.LeftIndexDistal;
        private HumanBodyBones targetBoneRightHand = HumanBodyBones.RightIndexDistal;
        private HumanBodyBones targetBoneLeftFoot = HumanBodyBones.LeftFoot;
        private HumanBodyBones targetBoneRightFoot = HumanBodyBones.RightFoot;
        private float boneResetInterval = 5.0f;
        private float boneResetCounter = 5.0f;
        private VRCPlayerApi _localPlayer;
        void Start()
        {
            if (SwitchWithHands)
            {
                if (!LeftHandObject) { Debug.LogError($"[Yodo]ハプティックコントローラーに左手用オブジェクトがありません [{this.name}]"); }
                if (!RightHandObject) { Debug.LogError($"[Yodo]ハプティックコントローラーに右手用オブジェクトがありません [{this.name}]"); }
            }
            if (SwitchWithFoots)
            {
                if (!LeftFootObject) { Debug.LogError($"[Yodo]ハプティックコントローラーに左足用オブジェクトがありません [{this.name}]"); }
                if (!RightFootObject) { Debug.LogError($"[Yodo]ハプティックコントローラーに右足用オブジェクトがありません [{this.name}]"); }
            }
            if(SwitchWithHead)
            {
                if (!HeadObject) { Debug.LogError($"[Yodo]ハプティックコントローラーに頭突き用オブジェクトがありません [{this.name}]"); }
            }

            _localPlayer = Networking.LocalPlayer;
        }

        private void FixedUpdate()
        {
            boneResetCounter -= Time.deltaTime; // アバターの読み込み完了は検知できないので数秒に一回ずつリセットしまくる(無駄だけどしゃーなし、どうせそんな重くない)
            if (boneResetCounter < 0)
            {
                SetupLocalHandBones();
            }

            UpdateColliderPositions();
        }

        // AvatarによってBoneがあったりなかったりするので近い指を検索する。Handもなければ諦める。
        private void SetupLocalHandBones()
        {
            Vector3 noBone = new Vector3(0, 0, 0);  // ボーンがないと原点が取れるので原点だったらボーンがないことにする
            Vector3 newPos;
            HumanBodyBones newBone;

            if (SwitchWithHands)
            {
                // 左手
                newBone = HumanBodyBones.RightIndexDistal;
                newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                if (newPos == noBone)
                {
                    newBone = HumanBodyBones.RightIndexIntermediate;
                    newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                    if (newPos == noBone)
                    {
                        newBone = HumanBodyBones.RightIndexProximal;
                        newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                        if (newPos == noBone)
                        {
                            newBone = HumanBodyBones.RightHand;
                        }
                    }
                }
                targetBoneRightHand = newBone;

                // 右手
                newBone = HumanBodyBones.LeftIndexDistal;
                newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                if (newPos == noBone)
                {
                    newBone = HumanBodyBones.LeftIndexIntermediate;
                    newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                    if (newPos == noBone)
                    {
                        newBone = HumanBodyBones.LeftIndexProximal;
                        newPos = Networking.LocalPlayer.GetBonePosition(newBone);
                        if (newPos == noBone)
                        {
                            newBone = HumanBodyBones.LeftHand;
                        }
                    }
                }
                targetBoneLeftHand = newBone;
            }
        }

        private void UpdateColliderPositions()
        {
            if(_localPlayer == null) { return; }
            if (!_localPlayer.IsValid()) { return; }    // ワールド退出時のエラー回避

            if(SwitchWithHands)
            {
                if(LeftHandObject)
                {
                    LeftHandObject.transform.position = _localPlayer.GetBonePosition(targetBoneLeftHand);
                }
                if(RightHandObject)
                {
                    RightHandObject.transform.position = _localPlayer.GetBonePosition(targetBoneRightHand);
                }
            }
            if(SwitchWithFoots)
            {
                if(LeftFootObject)
                {
                    LeftFootObject.transform.position = _localPlayer.GetBonePosition(targetBoneLeftFoot);
                }
                if(RightFootObject)
                {
                    RightFootObject.transform.position = _localPlayer.GetBonePosition(targetBoneRightFoot);
                }
            }
            if(SwitchWithHead)
            {
                if(HeadObject)
                {
                    HeadObject.transform.position = _localPlayer.GetBonePosition(HumanBodyBones.Head);
                }
            }
        }
    }
}

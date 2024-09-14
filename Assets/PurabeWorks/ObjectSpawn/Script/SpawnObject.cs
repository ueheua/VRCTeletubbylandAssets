using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Core;
using VRC.Udon;

namespace PurabeWorks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SpawnObject : UdonSharpBehaviour
    {
        [Header("ランダムスポーンをするかどうか")]
        [SerializeField] private bool _randomSpawn = false;
        [Header("スポーン対象のVRC Object Pool")]
        [SerializeField] private VRCObjectPool _vRCObjectPool;
        [Header("スポーンアイテムを手元に移動するか")]
        [SerializeField] private bool _moveItemToHand = true;
        [Header("スポーン時に再生するオーディオ")]
        [SerializeField] private AudioSource _audioSource = null;
        [Header("カスタムメソッドを実行するか")]
        [SerializeField] private bool _executeCustomEvent = false;
        [Header("スポーン対象以外のカスタムメソッド実行先")]
        [SerializeField] private GameObject[] _externalObjects;
        [Header("カスタムメソッド(コピペ用)")]
        [SerializeField]
        [TextArea]
        public string CustomEventNames = "public void Pura_OnSpawn(){}";


        private VRCPlayerApi localPlayer;

        private void Start()
        {
            if (_vRCObjectPool == null)
            {
                Debug.Log("[purabe]VRC Object Poolを登録してください。");
            }

            if (_randomSpawn)
            {
                //スポーン順序をシャッフル
                _vRCObjectPool.Shuffle();
            }

            if (Networking.LocalPlayer != null)
            {
                localPlayer = Networking.LocalPlayer;
            }
        }

        public override void Interact()
        {
            // このスクリプトを実行しているプレイヤーが「オーナ」でなければ「オーナ」にする
            _SetOwner(_vRCObjectPool.gameObject);
            // オブジェクトプールの配列頭のオブジェクトをスポーン
            GameObject spawnedObject = _vRCObjectPool.TryToSpawn();
            // オーナ権限取得
            _SetOwner(spawnedObject);

            // 手元に移動させる
            if (_moveItemToHand)
            {
                if (_IsNearToRightHand())
                {
                    spawnedObject.transform.position = localPlayer.GetBonePosition(HumanBodyBones.RightHand);
                } else
                {
                    spawnedObject.transform.position = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);
                }
            }

            // カスタムメソッド実行
            if (_executeCustomEvent)
            {
                _ExecuteCustomEvent(spawnedObject);
            }

            // SE再生
            if (_audioSource != null && _audioSource.clip != null)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayAudio));
            }
        }

        private void _SetOwner(GameObject obj)
        {
            if (!Networking.IsOwner(obj))
            {
                Networking.SetOwner(Networking.LocalPlayer, obj);
            }
        }

        private bool _IsNearToRightHand()
        {
            Vector3 rightHandPos = localPlayer.GetBonePosition(HumanBodyBones.RightHand);
            Vector3 leftHandPos = localPlayer.GetBonePosition(HumanBodyBones.LeftHand);

            return Vector3.Distance(transform.position, rightHandPos) <= Vector3.Distance(transform.position, leftHandPos);
        }

        private void _ExecuteCustomEvent(GameObject spawnedObject)
        {
            //スポーンされたオブジェクトでの実行
            Component[] udons = spawnedObject.GetComponents(typeof(UdonBehaviour));
            foreach (Component c in udons)
            {
                _ExecuteCustomEventSub((UdonBehaviour)c);
            }
            udons = spawnedObject.GetComponentsInChildren(typeof(UdonBehaviour));
            foreach (Component c in udons)
            {
                _ExecuteCustomEventSub((UdonBehaviour)c);
            }

            //外部オブジェクトでの実行
            if (_externalObjects.Length > 0)
            {
                foreach (GameObject e in _externalObjects)
                {
                    if (e == null)
                    {
                        continue;
                    }

                    udons = e.GetComponents(typeof(UdonBehaviour));
                    foreach (Component c in udons)
                    {
                        _ExecuteCustomEventSub((UdonBehaviour)c);
                    }
                    udons = e.GetComponentsInChildren(typeof(UdonBehaviour));
                    foreach (Component c in udons)
                    {
                        _ExecuteCustomEventSub((UdonBehaviour)c);
                    }
                }
            }
        }

        private void _ExecuteCustomEventSub(UdonBehaviour udon)
        {
            udon.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Pura_OnSpawn");
        }

        public void PlayAudio()
        {
            _audioSource.PlayOneShot(_audioSource.clip);
        }
    }
}

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;
using VRC.Udon;

namespace PurabeWorks
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ReturnObject : UdonSharpBehaviour
    {
        [Header("VRC Object Poolオブジェクトまたは親")]
        public GameObject[] pools;
        [Header("VRC Object Poolオブジェクトまたは親の参照先")]
        [SerializeField] private ReturnObject _reference;
        [Header("リターン対象レイヤー")]
        public int layer = 13;
        [Header("リターン時に再生するオーディオ")]
        [SerializeField] private AudioSource _audioSource = null;
        [Header("TriggerStayで処理するか")]
        [SerializeField] private bool _onTriggerStay = false;
        [Header("カスタムメソッドを実行するか")]
        [SerializeField] private bool _executeCustomEvent = false;
        [Header("リターン対象以外のカスタムメソッド実行先")]
        [SerializeField] private GameObject[] _externalObjects;
        [Header("カスタムメソッド(コピペ用)")]
        [SerializeField]
        [TextArea]
        public string CustomEventNames = "public void Pura_OnReturn(){}";

        private GameObject[] poolsRef;
        private void Start()
        {
            if (pools.Length <= 0 && _reference == null)
            {
                Debug.Log("[purabe]poolsを定義しない場合はreferenceを登録してください");
            }

            if (_reference != null)
            {
                poolsRef = _reference.pools;
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!_onTriggerStay)
            {
                _ReturnProcess(other.gameObject);
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (_onTriggerStay)
            {
                _ReturnProcess(other.gameObject);
            }
        }

        private void _ReturnProcess(GameObject target)
        {
            if (target.layer == layer && (!_onTriggerStay || Networking.IsOwner(target)))
            {
                // オーナ権限取得
                _SetOwner(target);
                // Drop処理
                VRCPickup pickup = (VRCPickup)target.GetComponent(typeof(VRCPickup));
                if (pickup != null)
                {
                    pickup.Drop();
                }
                // すべてのVRC Object Poolに対してアイテムReturnを実行
                foreach (GameObject p in pools)
                {
                    _ReturnProcessSub(target, p);
                    if (!target.activeInHierarchy)
                    {
                        return;
                    }
                }
                foreach (GameObject p in poolsRef)
                {
                    _ReturnProcessSub(target, p);
                    if (!target.activeInHierarchy)
                    {
                        return;
                    }
                }
            }
        }

        private void _ReturnProcessSub(GameObject target, GameObject g)
        {
            VRCObjectPool pool = (VRCObjectPool)g.GetComponent(typeof(VRCObjectPool));
            if (pool != null)
            {
                pool.Return(target);
                if (!target.activeInHierarchy)
                {
                    _DoWhenReturned(target);
                    return;
                }
            }

            Component[] pools = g.GetComponentsInChildren(typeof(VRCObjectPool));
            foreach (Component x in pools)
            {
                VRCObjectPool p2 = (VRCObjectPool)x;
                p2.Return(target);
                if (!target.activeInHierarchy)
                {
                    _DoWhenReturned(target);
                    return;
                }
            }
        }

        private void _DoWhenReturned(GameObject obj)
        {
            if (_executeCustomEvent)
            {
                //カスタムメソッドを全Udon Behaviourで発火する
                Component[] udons = obj.GetComponents(typeof(UdonBehaviour));
                foreach (Component c in udons)
                {
                    _DoWhenReturnedSub((UdonBehaviour)c);
                }
                udons = obj.GetComponentsInChildren(typeof(UdonBehaviour));
                foreach (Component c in udons)
                {
                    _DoWhenReturnedSub((UdonBehaviour)c);
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
                        foreach(Component c in udons)
                        {
                            _DoWhenReturnedSub((UdonBehaviour)c);
                        }
                        udons = e.GetComponentsInChildren(typeof(UdonBehaviour));
                        foreach (Component c in udons)
                        {
                            _DoWhenReturnedSub((UdonBehaviour)c);
                        }
                    }
                }
            }

            //リターンSE再生
            if (_audioSource != null && _audioSource.clip != null)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayAudio));
            }
        }

        private void _DoWhenReturnedSub(UdonBehaviour udon)
        {
            udon.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Pura_OnReturn");
        }

        private void _SetOwner(GameObject obj)
        {
            if (!Networking.IsOwner(obj))
            {
                Networking.SetOwner(Networking.LocalPlayer, obj);
            }
        }

        public void PlayAudio()
        {
            _audioSource.PlayOneShot(_audioSource.clip);
        }
    }
}
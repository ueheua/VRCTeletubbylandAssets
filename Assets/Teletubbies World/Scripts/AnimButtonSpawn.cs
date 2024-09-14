using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AnimButtonSpawn : UdonSharpBehaviour
{

    public Animator animator;

    public string animationName;

    public float animationDelaySeconds;

    public VRCObjectPool vrcObjectPool;

    public Transform spawnPosition;

    private void Start()
    {

    }

    public override void Interact()
    {

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");

    }
    public void PlayAnimation()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("pause"))
        {
            animator.Play(animationName);
            SendCustomEventDelayedSeconds("SpawnObject", animationDelaySeconds);
        }
    }

    public void SpawnObject()
    {
        if (Networking.IsMaster)
        {
            SetOwner(vrcObjectPool.gameObject);
            SetOwner(this.gameObject);

            foreach (GameObject gameObject in vrcObjectPool.Pool)
            {
                if(!gameObject.activeInHierarchy)
                SetOwner(gameObject);
            }

            GameObject spawnObject = vrcObjectPool.TryToSpawn();

            if (null != spawnObject)
            {

                spawnObject.transform.position = spawnPosition.position;
                spawnObject.transform.rotation = spawnPosition.rotation;

            }

        }
    }

    private void SetOwner(GameObject obj)
    {
        if (!Networking.IsOwner(obj))
        {
            Networking.SetOwner(Networking.LocalPlayer, obj);
        }
    }
}

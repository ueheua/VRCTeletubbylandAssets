using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;


[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AnimToggleSpawn : UdonSharpBehaviour
{

    public Animator animator;

    public string paramName;

    public float animationDelaySeconds;

    public VRCObjectPool vrcObjectPool;

    public Transform spawnPosition;

    private void Start()
    {

    }

    public override void Interact()
    {

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleAnimation");

    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsMaster)
        {
            if (animator.GetBool(paramName))
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleAnimationTrue");
            }
            else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ToggleAnimationFalse");
            }

        }

    }

    public void ToggleAnimation()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("pause"))
        {
            bool animationToggle = animator.GetBool(paramName);

            animator.SetBool(paramName, !animationToggle);

            if (!animationToggle)
            {
                SendCustomEventDelayedSeconds("SpawnObject", animationDelaySeconds);
            }

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
                if (!gameObject.activeInHierarchy)
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

    public void ToggleAnimationTrue()
    {
        animator.SetBool(paramName, true);
    }

    public void ToggleAnimationFalse()
    {
        animator.SetBool(paramName, false);
    }

}

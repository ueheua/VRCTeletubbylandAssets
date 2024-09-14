
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ReturnSpawnObject : UdonSharpBehaviour
{
    public VRCObjectPool[] pools;
    
    public int objectLayer = 13; // Pickup layer

    public void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.layer == objectLayer)
        {            
            SetOwner(this.gameObject);
            SetOwner(other.gameObject);
         
            foreach(VRCObjectPool vrcObjectPool in pools)
            {
                SetOwner(vrcObjectPool.gameObject);
                
                VRCPickup pickup = (VRCPickup)other.gameObject.GetComponent(typeof(VRCPickup));
                if (pickup != null)
                {
                    pickup.Drop();
                }

                vrcObjectPool.Return(other.gameObject);

                if (!other.gameObject.activeInHierarchy)
                {
                  /*
                    VRC_Pickup leftPickup = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Left);
                    VRC_Pickup rightPickup = Networking.LocalPlayer.GetPickupInHand(VRC_Pickup.PickupHand.Right);

                    if (leftPickup != null 
                        && leftPickup.gameObject != null
                        && leftPickup.gameObject.Equals(other.gameObject))
                    {
                        leftPickup.Drop();
                    } 
                    else if (rightPickup != null
                        && rightPickup.gameObject != null
                        && rightPickup.gameObject.Equals(other.gameObject))
                    {
                        rightPickup.Drop();
                    }
                  */
                    return;
                }
             
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        this.OnTriggerEnter(other);
    }

    private void SetOwner(GameObject obj)
    {
        if (!Networking.IsOwner(obj))
        {
            Networking.SetOwner(Networking.LocalPlayer, obj);
        }
    }
}

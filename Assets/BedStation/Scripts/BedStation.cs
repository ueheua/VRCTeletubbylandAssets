using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace nekobako {
	[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
	public class BedStation : UdonSharpBehaviour {
		[SerializeField]
		private VRCStation Station = null;

		[SerializeField]
		private Transform Head = null;
		
		private VRCPlayerApi Player = null;

		public override void Interact() {
			this.Station.UseStation(Networking.LocalPlayer);
		}

		public override void OnStationEntered(VRCPlayerApi player) {
			this.Player = player;
			this.Station.stationEnterPlayerLocation.localRotation = Quaternion.AngleAxis(-90, Vector3.right);
		}

		public override void OnStationExited(VRCPlayerApi player) {
			this.Player = null;
			this.Station.stationEnterPlayerLocation.localRotation = Quaternion.identity;
		}

		private void Update() {
			if(this.Player != null && this.Player.IsValid()) {
				this.Station.stationEnterPlayerLocation.position += this.Head.position - this.Player.GetBonePosition(HumanBodyBones.Head);
			}
		}
	}
}

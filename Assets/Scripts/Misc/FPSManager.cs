using UnityEngine;

namespace EmptyGame.Misc {

	public class FPSManager : MonoBehaviour {

		private void Awake() {
			Application.targetFrameRate = 60;
		}
	}

}
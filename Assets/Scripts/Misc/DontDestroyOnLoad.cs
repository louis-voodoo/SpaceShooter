using UnityEngine;

namespace EmptyGame.Misc {

	public class DontDestroyOnLoad : MonoBehaviour {

		private void Awake() {
			DontDestroyOnLoad(this.gameObject);
		}

	}

}
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmptyGame.Misc {

	public class ForcePreload : MonoBehaviour {

		private void Awake() {
			SceneManager.LoadScene(0);
		}

	}

}

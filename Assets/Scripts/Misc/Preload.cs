using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmptyGame.Misc {

	public class Preload : MonoBehaviour {

		private void Awake() {
			SceneManager.LoadScene(1);
		}

	}

}
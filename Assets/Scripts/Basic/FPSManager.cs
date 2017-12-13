using UnityEngine;

public class FramerateSetter : MonoBehaviour {

	private void Awake() {
		Application.targetFrameRate = 60;
	}
}

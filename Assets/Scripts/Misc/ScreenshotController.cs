using UnityEngine;

namespace EmptyGame.Misc {

	public class ScreenshotController : MonoBehaviour {

#if UNITY_EDITOR
		private int count = 1;

		void Update() {
			if (Input.GetMouseButtonDown(1)) {
				string screenshotName = "Screenshot_" + count++ + ".png";
				Application.CaptureScreenshot(screenshotName);
				Debug.Log("Saved screenshot : \"" + screenshotName + "\"");
			}
		}
#endif
	}

}
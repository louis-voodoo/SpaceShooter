using System.IO;
using UnityEngine;

namespace Misc {

	public class ScreenshotController : MonoBehaviour {
#if UNITY_EDITOR
		public string ScreenshotFolder = "Screenshots";
		private int count = 1;

		void Start() {
			if (!Directory.Exists(ScreenshotFolder)) {
				Debug.Log("Created directory for screenshots: " + ScreenshotFolder);
				Directory.CreateDirectory(ScreenshotFolder);
			}
			count = Directory.GetFiles(ScreenshotFolder).Length;
		}

		void Update() {
			if (Input.GetMouseButtonDown(1)) {
				string screenshotName = ScreenshotFolder + "/Screenshot_" + count++ + ".png";
				ScreenCapture.CaptureScreenshot(screenshotName);
				Debug.Log("Saved screenshot : \"" + screenshotName + "\"");
			}
		}
#endif
	}
}
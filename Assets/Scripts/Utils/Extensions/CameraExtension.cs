using UnityEngine;
public static class CameraExtension {
	
	public static Bounds GetBounds(this Camera cam) {		
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = cam.orthographicSize * 2;
		Bounds bounds = new Bounds(
			cam.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
		return bounds;
	}
}
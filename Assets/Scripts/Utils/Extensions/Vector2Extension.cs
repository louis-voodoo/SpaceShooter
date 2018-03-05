using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension {

	public static float ToAngle(this Vector2 v) {
        float angle = Mathf.Atan2(v.y,v.x);
        return (angle * Mathf.Rad2Deg - 90 + 360) % 360;
    }

	 public static Vector2 FromFloat(this Vector2 v, float angle) {
        return (Vector2) (Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up).normalized;
    }

    public static Vector2 AdjustPosition(this Vector2 position, int decimals = 0)
	{
        float precision = Mathf.Pow(10, decimals);
		Vector2 newPosition = position;
		newPosition.x = Mathf.Round(newPosition.x * precision) / precision;
		newPosition.y = Mathf.Round(newPosition.y * precision) / precision;
		return newPosition;
	}
}

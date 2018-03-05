using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension {

	public static Color HexToColor(this Color c, uint HexVal, float alpha = 1)
	{
		float R = (float)((HexVal >> 16) & 0xFF);
		float G = (float)((HexVal >> 8) & 0xFF);
		float B = (float)((HexVal) & 0xFF);
		return new Color(R/255, G/255, B/255, alpha);
	}

	public static Color SetAlpha(this Color color, float alpha = 1){
		return new Color (color.r,color.g,color.b,alpha); 
	}

	public static Color SetAlpha(this Color color, int alpha = 255){
		return new Color (color.r,color.g,color.b,alpha/255f); 
	}
}

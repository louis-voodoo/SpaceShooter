using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension {

	public static void ChangeLayersRecursively(this Transform trans , string name)
	{
		trans.gameObject.layer = LayerMask.NameToLayer (name);
		for (int i  = 0; i < trans.childCount; i++)
		{
			ChangeLayersRecursively(trans.GetChild(i), name);
		}
	}

	public static void DeleteAllChildren (this Transform trans){
		for (int i = trans.childCount - 1; i >= 0; i--) {
			GameObject.Destroy (trans.GetChild (i).gameObject);
		}
	}
}

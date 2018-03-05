#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResetSave {

	[MenuItem("Voodoo/ResetSave")]
	public static void DoReset (){
		PlayerPrefs.DeleteAll ();
	}
}
#endif

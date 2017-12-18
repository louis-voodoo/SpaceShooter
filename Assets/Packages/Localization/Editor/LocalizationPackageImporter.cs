using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationPackageImporter : Editor {



	[MenuItem("Voodoo/Released games tools/Import localization package")] 
	public static void ImportPackage()
	{
		

		string path = Application.dataPath+"/Packages/Localization/Localization_withLean.unitypackage";
		if (LeanLocalizationAlreadyImported ()) {
			path =  Application.dataPath+"/Packages/Localization/Localization_withoutLean.unitypackage";
		}

		AssetDatabase.ImportPackage (path, true);
		
	}

	[MenuItem("Voodoo/Released games tools/Import localization package",true)] 
	public static bool ShouldShowItem(){
		return !PackageAlreadyImported();
	}

	static bool PackageAlreadyImported(){
		string className="LeanLocalizationImporter";
		System.Type type = System.Type.GetType (className);
		if (type != null) {
			return true;
		} else {
			return false;
		}
	}

	static bool LeanLocalizationAlreadyImported(){
		string className="Lean.Localization.LeanLocalization_Editor";
		System.Type type = System.Type.GetType (className);
		if (type != null) {
			return true;
		} else {
			return false;
		}
	}
}

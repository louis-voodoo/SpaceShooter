using UnityEngine;

namespace VoodooSauceInternal {
	
	[CreateAssetMenu(fileName = "Assets/Resources/VoodooSettings", menuName = "VoodooSauce/Settings file", order = 1000)]
	internal class VoodooSettings : ScriptableObject {
		[Header("Voodoo Sauce version " + VoodooSauce.VERSION)]
		
		[Header("GameAnalytics")]

        [Tooltip("Your GameAnalytics Ios Game Key - copy/paste from the GA website")]
        public string GameAnalyticsIosGameKey;

        [Tooltip("Your GameAnalytics Ios Secret Key - copy/paste from the GA website")]
        public string GameAnalyticsIosSecretKey;

        [Tooltip("Your GameAnalytics Android Game Key - copy/paste from the GA website")]
        public string GameAnalyticsAndroidGameKey;

        [Tooltip("Your GameAnalytics Android Secret Key - copy/paste from the GA website")]
        public string GameAnalyticsAndroidSecretKey;

		[Header("Amplitude")]
		
		[Tooltip("Your Amplitude API key for your project")]
		public string AmplitudeApiKey;
	}
}

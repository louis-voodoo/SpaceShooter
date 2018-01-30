using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

namespace VoodooSauceInternal {

	internal static class VoodooAnalytics {
		private const string PrefsLaunchCount = "VoodooSauce_AppLaunchCount";
		private const string PrefsGameCount = "VoodooSauce_GameCount";

		
		internal static void OnApplicationStarted() {
			int launchCount = PlayerPrefs.GetInt(PrefsLaunchCount, 0) + 1;
			PlayerPrefs.SetInt(PrefsLaunchCount, launchCount);
		}

		internal static void OnGameStarted() {
			PlayerPrefs.SetInt(PrefsGameCount, PlayerPrefs.GetInt(PrefsGameCount, 0) + 1);
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
		}

		internal static void OnGameFinished(bool levelComplete, float score, Dictionary<string, object> eventProperties) {
			Debug.Log("Sending progression event \"Complete\" to Game Analytics.");
			GameAnalytics.NewProgressionEvent(
				levelComplete ? GAProgressionStatus.Complete : GAProgressionStatus.Fail,
				"game",
				(int) score
			);
		}

		internal static void TrackCustomEvent(string eventName, Dictionary<string, object> eventProperties) {
			Debug.Log("Sending custom event to GameAnalytics : " + eventName);
			GameAnalytics.NewDesignEvent(eventName);
		}
	}
}
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

namespace VoodooSauceInternal {

	internal static class VoodooAnalytics {
		private const string PrefsLaunchCount = "VoodooSauce_AppLaunchCount";
		private const string PrefsGameCount = "VoodooSauce_GameCount";

		private const string AppLaunchedFirstTimeEventName = "App Launched First Time";
		private const string AppLaunchedEventName = "App Launched";
		
		private const string GamePlayedEventName = "Game Played";
		
		internal static void OnApplicationStarted() {
			int launchCount = PlayerPrefs.GetInt(PrefsLaunchCount, 0) + 1;
			PlayerPrefs.SetInt(PrefsLaunchCount, launchCount);

			if (launchCount == 1) {
				Debug.Log("Sending \"" + AppLaunchedFirstTimeEventName + "\" event to Amplitude.");
				Amplitude.Instance.logEvent(AppLaunchedFirstTimeEventName);
			}
			else {
				Debug.Log("Sending \"" + AppLaunchedEventName + "\" event to Amplitude.");
				Amplitude.Instance.logEvent(AppLaunchedEventName);
			}
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
			
			Debug.Log("Sending \"" + GamePlayedEventName + "\" event to Amplitude.");
			Dictionary<string, object> properties = eventProperties ?? new Dictionary<string, object>();
			properties["Game Number"] = PlayerPrefs.GetInt(PrefsGameCount, 1);
			properties["Win"] = levelComplete;
			properties["Score"] = score;
			Amplitude.Instance.logEvent(GamePlayedEventName, properties);
		}
		
		internal static void TrackCustomEvent(string eventName, Dictionary<string, object> eventProperties) {
			Debug.Log("Sending custom event to GameAnalytics : " + eventName);
			GameAnalytics.NewDesignEvent(eventName);
			
			Debug.Log("Sending custom event to Amplitude : " + eventName);
			Amplitude.Instance.logEvent(eventName, eventProperties);
		}
	}
}
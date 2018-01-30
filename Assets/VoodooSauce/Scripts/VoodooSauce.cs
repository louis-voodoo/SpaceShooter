using System.Collections.Generic;
using VoodooSauceInternal;

public static class VoodooSauce {
	public const string VERSION = "2.5";
	
	/// <summary>
	/// Method to call whenever the user starts a game.
	/// </summary>
	public static void OnGameStarted() {
		VoodooAnalytics.OnGameStarted();
	}

	/// <summary>
	/// Method to call whenever the user completes a game.
	/// </summary>
	/// <param name="score">The score of the game</param>
	public static void OnGameFinished(float score) {
		OnGameFinished(true, score, null);
	}

	/// <summary>
	/// Method to call whenever the user finishes a game, even when leaving a game.
	/// </summary>
	/// <param name="levelComplete">Whether the user finished the game</param>
	/// <param name="score">The score of the game</param>
	public static void OnGameFinished(bool levelComplete, float score) {
		OnGameFinished(levelComplete, score, null);
	}

	/// <summary>
	/// Method to call whenever the user finishes a game, even when leaving a game.
	/// </summary>
	/// <param name="levelComplete">Whether the user finished the game</param>
	/// <param name="score">The score of the game</param>
	/// <param name="eventProperties">An optional list of mixpanel properties to send along with the event</param>
	public static void OnGameFinished(bool levelComplete, float score, Dictionary<string, object> eventProperties) {
		VoodooAnalytics.OnGameFinished(levelComplete, score, eventProperties);
	}

	/// <summary>
	/// Call this method to track any custom event you want.
	/// </summary>
	/// <param name="eventName">The name of the event to track</param>
	/// <param name="eventProperties">A list of key/values pairs you want as event properties</param>
	public static void TrackCustomEvent(string eventName, Dictionary<string, object> eventProperties) {
		VoodooAnalytics.TrackCustomEvent(eventName, eventProperties);
	}
	
	/// <summary>
	/// Call this method to track any custom event you want.
	/// </summary>
	/// <param name="eventName">The name of the event to track</param>
	public static void TrackCustomEvent(string eventName) {
		TrackCustomEvent(eventName, null);
    }
}
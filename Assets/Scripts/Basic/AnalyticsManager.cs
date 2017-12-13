using GameAnalyticsSDK;

namespace App {
	public class AnalyticsManager {

		public static void TrackGameStarted() {
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");
		}

		public static void TrackGameCompleted(int score) {
			GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", score);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores score and maxscore. Use ScoreManager.Instance.AddScore(value) to add score
/// Uses Events OnResetGameEvent and OnPlayerDeathEvent
/// </summary>
public class ScoreManager : MonoSingleton<ScoreManager> {

	const string MAX_SCORE_KEY = "max";
	public double CurrentScore { get; private set; }
	public PersistentVariable<double> BestScorePersistentVariable = new PersistentVariable<double>(MAX_SCORE_KEY, 0);

	public double BestScore {
		get {
			return (BestScorePersistentVariable.Value);
		}
		set {
			BestScorePersistentVariable.Value = value;
		}
	}

	void OnEnable()
	{
		Application.targetFrameRate = 60;
		Events.Instance.AddListener<OnResetGameEvent>(HandleOnResetGame);
		Events.Instance.AddListener<OnModifyScoreEvent>(HandleOnModifyScore);
		Events.Instance.AddListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
	}

	void OnDisable()
	{
		Events.Instance.RemoveListener<OnResetGameEvent>(HandleOnResetGame);
		Events.Instance.RemoveListener<OnModifyScoreEvent>(HandleOnModifyScore);
		Events.Instance.RemoveListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
	}

	void HandleOnPlayerDeath(OnPlayerDeathEvent ev) {
		if (BestScore < CurrentScore)
			BestScore = CurrentScore;
	}

	void HandleOnModifyScore(OnModifyScoreEvent ev) {
		CurrentScore += ev.Value;
		Events.Instance.Raise(new OnScoreModifiedEvent());
	}

	void HandleOnResetGame(OnResetGameEvent ev) {
		CurrentScore = 0;
	}

	public void AddScore(double value) {
		Events.Instance.Raise(new OnModifyScoreEvent() { Value = value });
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnNewBestScoreEvent : GameEvent {} // Thrown when you get a new best score
public class OnModifyScoreEvent : GameEvent { // Thrown by ScoreManager when you change score
    public double Value;
}
public class OnScoreModifiedEvent : GameEvent {}

/// <summary>
/// The ScoreDisplayer is to put as a UI Element and controls a CurrentScore and a BestScore
/// Please make sure you use the OnLaunchGameEvent and OnPlayerDeathEvent to display/hide the values at the right time
/// </summary>
public class ScoreDisplayer : UIElement {
    [SerializeField]
    TextMeshProUGUI _currentScoreText; // Drop the Text component displaying Current score
    [SerializeField]
    UIElement _currentScore; // Drop the UIElement (canvas group) of the CurrentScore gameobject
    [SerializeField]
    TextMeshProUGUI _bestScoreText; // Drop the Text component displaying Best score
    [SerializeField]
    UIElement _bestScore; // Drop the UIElement (canvas group) of the BestScore gameobject


    void OnEnable()
    {
        if (_currentScore != null)
            _currentScore.Hide();
        Events.Instance.AddListener<OnLaunchGameEvent>(HandleOnGameReady);
        Events.Instance.AddListener<OnScoreModifiedEvent>(HandleOnScoreModified);
        Events.Instance.AddListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
    }

    void OnDisable()
    {
        Events.Instance.RemoveListener<OnLaunchGameEvent>(HandleOnGameReady);
        Events.Instance.RemoveListener<OnScoreModifiedEvent>(HandleOnScoreModified);
        Events.Instance.RemoveListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
    }

    void Start() {
        if (_currentScoreText != null)
            _currentScoreText.text = StyleScore(ScoreManager.Instance.CurrentScore);
        if (_bestScoreText != null)
            _bestScoreText.text = StyleScore(ScoreManager.Instance.BestScore);
    }

    void UpdateBestScore() {
        if (_bestScoreText != null)
            _bestScoreText.text = StyleScore(ScoreManager.Instance.BestScore);
    }

    void HandleOnPlayerDeath(OnPlayerDeathEvent ev) {
        if (ScoreManager.Instance.CurrentScore >= ScoreManager.Instance.BestScore) {
            Events.Instance.Raise(new OnNewBestScoreEvent());
        }
        UpdateBestScore();
        if (_bestScore != null)
            _bestScore.Show(0.5f);
    }

    /// <summary>
    /// Change this function to display the score and best score (like adding units, dots, whatever)
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    string StyleScore(double score) {
        return (System.Math.Floor(score).ToString());
    }

    void HandleOnGameReady(OnLaunchGameEvent ev) {
        if (_currentScore != null)
            _currentScore.Show();
        if (_bestScore != null)
            _bestScore.Hide(0.5f);
        if (_currentScoreText != null)
            _currentScoreText.text = StyleScore(ScoreManager.Instance.CurrentScore);
    }

    void HandleOnScoreModified(OnScoreModifiedEvent ev) {
        if (_currentScoreText != null)
            _currentScoreText.text = StyleScore(ScoreManager.Instance.CurrentScore);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils
{
	public enum EMenu {
		MainMenu,
		Gameplay,
		DeathMenu,
	}
	public class MenuManager : MonoSingleton<MenuManager> {

		[SerializeField]UIElement MainMenuUI;
		[SerializeField]UIElement GameplayUI;
		[SerializeField]UIElement DeathMenuUI;

		public float DeathWaitingTime = 0.5f;
		public float DeathTransitionTime = 0.2f;

		public bool PlayOnTouch = true;

		public EMenu CurrentMenu;

		Coroutine _deathCoroutine;

		void OnEnable() {
			Events.Instance.AddListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
			Events.Instance.AddListener<OnLaunchGameEvent>(HandleOnLaunchGame);
			CurrentMenu = EMenu.MainMenu;
			MainMenuUI.Show();
			GameplayUI.Hide();
			DeathMenuUI.Hide();
		}

		void OnDisable()
		{
			Events.Instance.RemoveListener<OnPlayerDeathEvent>(HandleOnPlayerDeath);
			Events.Instance.RemoveListener<OnLaunchGameEvent>(HandleOnLaunchGame);
		}

		/// <summary>
		/// Calls the event of player death
		/// </summary>
		public void PlayerDeath() {
			Events.Instance.Raise(new OnPlayerDeathEvent());
		}

		/// <summary>
		/// Calls the event of reset game and reloads the scene with animation
		/// </summary>
		public void ResetGame() {
			Events.Instance.Raise(new OnResetGameEvent());
			Utils.LoadingScreenManager.Instance.ReloadScene();
		}

		void HandleOnLaunchGame(OnLaunchGameEvent ev) {
			MainMenuUI.Hide();
			GameplayUI.Show();
			CurrentMenu = EMenu.Gameplay;
		}

		void HandleOnPlayerDeath(OnPlayerDeathEvent ev) {
			GameplayUI.Hide();
			if (_deathCoroutine != null)
				StopCoroutine(_deathCoroutine);
			_deathCoroutine = StartCoroutine(WaitToDisplayResetButton());
		}

		IEnumerator WaitToDisplayResetButton() {
			yield return (new WaitForSeconds(DeathWaitingTime));
			DeathMenuUI.Show(DeathTransitionTime);
			CurrentMenu = EMenu.DeathMenu;
		}

		/// <summary>
		/// Start the game if touching the screen and in the current menu
		/// </summary>
		void Update()
		{
			if (Input.GetMouseButtonDown(0) && CurrentMenu == EMenu.MainMenu && IsPointerOverUIObject() == false && PlayOnTouch == true) {
				Events.Instance.Raise(new OnLaunchGameEvent());
			}
		}

		bool IsPointerOverUIObject() 
		{
    		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    		List<RaycastResult> results = new List<RaycastResult>();
			if (EventSystem.current != null)
    			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			else {
				Debug.LogError("No EventSystem in the scene. Please add one.");
			}

    		return (results.Count > 0);
		}
	}
}

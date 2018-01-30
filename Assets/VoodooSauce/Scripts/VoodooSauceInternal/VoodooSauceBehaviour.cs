using System;
using System.Collections;
using Facebook.Unity;
using Facebook.Unity.Settings;
using GameAnalyticsSDK;
using UnityEngine;

namespace VoodooSauceInternal {
	internal class VoodooSauceBehaviour : MonoBehaviour {
		private static VoodooSauceBehaviour _instance;

		private const int _gameAnalyticsIosIndex = 0;
		private const int _gameAnalyticsAndroidIndex = 1;

		[SerializeField] private GameAnalytics _gameAnalyticsPrefab;

		private VoodooSettings _settings;

		private static bool _isInitialized;

		private void Awake() {
			if (transform != transform.root)
				throw new Exception("VoodooSauce prefab HAS to be at the ROOT level!");

			if (_isInitialized) {
				Debug.LogError("VoodooSauce is already initialized! Please avoid creating multiple instance of VoodooSauce. This object will be destroyed...");
				Destroy(gameObject);
				return;
			}

			DontDestroyOnLoad(this);
			_instance = this;

			_settings = Resources.Load<VoodooSettings>("VoodooSettings");

			if (_settings == null)
				throw new Exception(
					"Can't find VoodooSauce settings file. Please check you have created the file using Assets/Create/VoodooSauce/Settings File");

			InitGameAnalytics();
			InitFacebook();

			VoodooAnalytics.OnApplicationStarted();
		}

		private void InitGameAnalytics() {
			if (_settings.GameAnalyticsIosGameKey.Equals("") || _settings.GameAnalyticsIosSecretKey.Equals("")) {
				Debug.Log("VoodooSauce Settings is missing iOS GameAnalytics keys! Go to Resources/VoodooSettings and set it.");
				return;
			}

			GameAnalytics gameAnalyticsInstance = FindObjectOfType<GameAnalytics>();
			if (gameAnalyticsInstance == null) {
				gameAnalyticsInstance = Instantiate(_gameAnalyticsPrefab);

				AddOrUpdatePlatform(RuntimePlatform.IPhonePlayer, _settings.GameAnalyticsIosGameKey, _settings.GameAnalyticsIosSecretKey);
				
				GameAnalytics.SettingsGA.InfoLogBuild = false;
				GameAnalytics.SettingsGA.InfoLogEditor = false;

				gameAnalyticsInstance.gameObject.SetActive(true);
			} else
			{
				throw new Exception("Looks like GameAnalytics has been added manually to the scene. please remove your GameAnalytics object and let VoodooSauce handle it.");
			}
		}

		private void AddOrUpdatePlatform(RuntimePlatform platform, string gameKey, string secretKey) {
			if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
				GameAnalytics.SettingsGA.AddPlatform(platform);

			int platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);

			GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
			GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
			GameAnalytics.SettingsGA.Build[platformIndex] = Application.version;
		}

		private void RemovePlatform(RuntimePlatform platform) {
			if (GameAnalytics.SettingsGA.Platforms.Contains(platform))
			{
				int platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
				GameAnalytics.SettingsGA.RemovePlatformAtIndex(platformIndex);
			}
		}

		private void InitFacebook() {
			string appId = FacebookSettings.AppId;

			if (string.IsNullOrEmpty(appId)) {
				Debug.LogWarning("FacebookSettings AppId is empty. Please fill it in menu Facebook > Edit Settings > AppId");
				return;
			}
			if (appId.Equals("0") || appId.Equals("157578437735213") ||
			    appId.Equals("146810152723342") && !Application.productName.Equals("VoodooSauce")) {
				Debug.LogWarning("FacebookSettings AppId has not been changed. Please change it in menu Facebook > Edit Settings > AppId");
				return;
			}

			Debug.Log("Initializing Facebook...");
			FB.Init(() => Debug.Log("Facebook Initialized"));
		}

		private void Start()
		{
			if (_isInitialized) return;
			
			Debug.Log("VoodooSauce initialized. Version: " + VoodooSauce.VERSION);
			_isInitialized = true;
		}
		
		internal static void InvokeAfter(Action methodToCall, float duration) {
			_instance.StartCoroutine(_instance.InvokeAfterCoroutine(methodToCall, duration));
		}

		private IEnumerator InvokeAfterCoroutine(Action methodToCall, float duration) {
			yield return new WaitForSeconds(duration);
			methodToCall();
		}
	}
}
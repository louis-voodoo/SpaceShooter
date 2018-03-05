using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
	/// <summary>
	/// Loads asynchronously a scene with some waiting visual, either simple screen fade or animation-based
	/// </summary>
	/// To use it, you have to include the Prefab and call anytime : LoadingScreenManager.Instance.LoadScene("my_scene_name");
	public class LoadingScreenManager : MonoSingleton<LoadingScreenManager> {
    	public UIElement TransitionElement;
    	[SerializeField]
    	private Animator Animation;
		public float DefaultTimer;
    	public Scene CurrentScene { get; private set; }
    	public bool Loading { get; private set; }

    	protected override void Init()
    	{
    	    base.Init();
    	    Loading = false;
			transform.parent = null;
			DontDestroyOnLoad(gameObject);
		}
		public void ReloadScene(float timer = -1)
    	{
    	    LoadScene(SceneManager.GetActiveScene().name, (timer == -1) ? DefaultTimer : timer);
    	}

    	public void LoadScene(string sceneName, float timer = -1)
    	{
    	    if (Loading == false)
    	    {
    	        Loading = true;
    	        StartCoroutine(Load(sceneName, (timer == -1) ? DefaultTimer : timer));
    	    }
    	    else
    	        Debug.LogError("Tried to load scene " + sceneName);
    	}

    	IEnumerator Load(string sceneName, float timer)
    	{
			if (Animation != null) {
				Animation.SetBool("Fade", true);
				yield return (new WaitForEndOfFrame());
				yield return new WaitForSeconds(Animation.GetCurrentAnimatorStateInfo(0).length);
			}
			else {
				TransitionElement.Show(timer / 2f);
    	   		yield return new WaitForSeconds(timer / 2f);
			}

    	    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

    	    while (!async.isDone)
    	        yield return null;

			if (Animation != null) {
				Animation.SetBool("Fade", false);
				yield return (new WaitForEndOfFrame());
				yield return new WaitForSeconds(Animation.GetCurrentAnimatorStateInfo(0).length);
			}
			else {
				TransitionElement.Hide(timer / 2f);
				yield return new WaitForSeconds(timer / 2f);
			}

    	    Loading = false;
    	}
	}

}
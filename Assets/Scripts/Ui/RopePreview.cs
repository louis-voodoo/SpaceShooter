using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePreview : MonoBehaviour {

    public static RopePreview Instance { get; private set; }

    public float PreviewDistance;
    public float FadingTime;
    public float PreviewZPos;
    public float PreviewMinScale;
    public UIElement PreviewPrefab;

    List<Rope> registeredRopes = new List<Rope>();
    List<UIElement> previews = new List<UIElement>();
    Dictionary<Rope, UIElement> activePreviews = new Dictionary<Rope, UIElement>();
    Character player;
    Camera cam;
    Vector3 ropePos;
    Vector3 previewPos;

    void Awake()
    {
        if (!Instance)
            Instance = this;
        else if (Instance != this)
            Destroy(this.gameObject);
    }

	// Use this for initialization
	void Start () {

        player = GameObject.FindObjectOfType<Character>();
        cam = Camera.main;

	}

    public void RegisterRope(Rope r)
    {
        if (!registeredRopes.Contains(r))
            registeredRopes.Add(r);
    }
	
	// Update is called once per frame
	void LateUpdate () {

        if (!player || !cam)
            return;

        for (int i = registeredRopes.Count - 1; i >= 0; i--)
        {
            if (!registeredRopes[i])
            {
                registeredRopes.RemoveAt(i);
                continue;
            }

            if (registeredRopes[i].transform.position.x < player.transform.position.x)
                continue;

            if (activePreviews.ContainsKey(registeredRopes[i]))
            {
                ropePos = cam.WorldToViewportPoint(registeredRopes[i].transform.position);
                previewPos = cam.ViewportToWorldPoint(new Vector3(1, ropePos.y, PreviewZPos));
                activePreviews[registeredRopes[i]].transform.position = previewPos;
                activePreviews[registeredRopes[i]].transform.localScale = PreviewPrefab.transform.localScale * Mathf.Lerp(PreviewMinScale, 1f, 1 - ((registeredRopes[i].transform.position.x - player.transform.position.x) / PreviewDistance));

                if (cam.WorldToViewportPoint(registeredRopes[i].transform.position).x < 1)
                {
                    if (!activePreviews[registeredRopes[i]].IsVisible())
                        activePreviews.Remove(registeredRopes[i]);
                    else if (!activePreviews[registeredRopes[i]].Hiding)
                        activePreviews[registeredRopes[i]].Hide(FadingTime);
                }
                continue;
            }

            if(registeredRopes[i].transform.position.x - player.transform.position.x < PreviewDistance && cam.WorldToViewportPoint(registeredRopes[i].transform.position).x > 1)
            {
                activePreviews.Add(registeredRopes[i], GetNextElement());
                activePreviews[registeredRopes[i]].Show();
                ropePos = cam.WorldToViewportPoint(registeredRopes[i].transform.position);
                previewPos = cam.ViewportToWorldPoint(new Vector3(1, ropePos.y, PreviewZPos));
                activePreviews[registeredRopes[i]].transform.position = previewPos;
                activePreviews[registeredRopes[i]].transform.localScale = PreviewPrefab.transform.localScale * Mathf.Lerp(PreviewMinScale, 1f, 1 - ((registeredRopes[i].transform.position.x - player.transform.position.x) / PreviewDistance));
            }
        }

	}

    UIElement GetNextElement()
    {
        if(previews.Count == 0)
        {
            previews.Add(Instantiate(PreviewPrefab, new Vector3(-200,0,0),Quaternion.identity));
            return previews[0];
        }

        for (int i = 0; i < previews.Count;i++)
        {
            if (!activePreviews.ContainsValue(previews[i]))
                return previews[i];
        }

        previews.Add(Instantiate(PreviewPrefab, new Vector3(-200, 0, 0), Quaternion.identity));
        return previews[previews.Count - 1];
    }
}

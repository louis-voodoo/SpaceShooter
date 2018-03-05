using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamScript : MonoBehaviour {

    public float LerpSpeed;
    public Vector2 Offset;
    public Vector2 LandscapeOffset;
    public bool FollowY;
    public Vector2 MinMaxZPos;
    public Vector2 LandscapeMinMaxZPos;
    public AnimationCurve ZPosInterpolation;

    Character playerObject;
    Vector3 wantedPos;

	// Use this for initialization
	void Start () {

        playerObject = GameObject.FindObjectOfType<Character>();

	}
	
	// Update is called once per frame
	void LateUpdate () {

        if (!playerObject)
            return;

        if (playerObject.MaxVelocity > 0)
        {
            if (Screen.width < Screen.height)
                wantedPos = new Vector3(playerObject.transform.position.x + Offset.x, FollowY ? playerObject.transform.position.y + Offset.y : this.transform.position.y, Mathf.Lerp(MinMaxZPos.x, MinMaxZPos.y, ZPosInterpolation.Evaluate(playerObject.Velocity.x / playerObject.MaxVelocity)));
            else
                wantedPos = new Vector3(playerObject.transform.position.x + LandscapeOffset.x, FollowY ? playerObject.transform.position.y + LandscapeOffset.y : this.transform.position.y, Mathf.Lerp(LandscapeMinMaxZPos.x, LandscapeMinMaxZPos.y, ZPosInterpolation.Evaluate(playerObject.Velocity.x / playerObject.MaxVelocity)));
        }

        this.transform.position = Vector3.Lerp(this.transform.position, wantedPos, LerpSpeed * Time.deltaTime);
	}
}

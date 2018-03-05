using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    
	public Transform target;
	public Vector3 positionOffset;
	public Vector3 rotationOffset;
	public bool startOffset;
	public bool followRotation;

	public enum E_FollowMode{
		Lerp,
		Speed
	}

	public E_FollowMode followMode;
	public float speedValue = 10;
	public float speedRotationValue = 10;
	[Range(0,1)]
	public float lerpValue = 0.1f;
	[Range(0,1)]
	public float lerpRotationValue = 0.1f;
	private Transform mTransform;


	void OnEnable (){
		mTransform = GetComponent<Transform>();
		if(startOffset && target != null)
		{
			positionOffset = mTransform.position - target.position;
			rotationOffset = mTransform.rotation.eulerAngles -target.rotation.eulerAngles;
		}
	}

	void FixedUpdate()
	{
		Vector3 targetPos = target.position + positionOffset;
		Vector3 targetRotation = target.rotation.eulerAngles + rotationOffset;

		if(followMode == E_FollowMode.Lerp)
		{
			mTransform.position = Vector3.Lerp(mTransform.position, targetPos, lerpValue);
			if(followRotation)
				mTransform.rotation = Quaternion.Lerp(mTransform.rotation, Quaternion.Euler(targetRotation), lerpRotationValue);
		}
		else{
			mTransform.position = Vector3.MoveTowards(mTransform.position, targetPos, speedValue * Time.deltaTime);
			if(followRotation)
				mTransform.rotation = Quaternion.RotateTowards(mTransform.rotation, Quaternion.Euler(targetRotation), speedRotationValue * Time.deltaTime);
		}
	}
}
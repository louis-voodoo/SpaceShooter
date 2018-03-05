using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public float speed;
	public Vector3 axis = Vector3.forward;
	private Transform mTransform;

	void Awake()
	{
		mTransform = GetComponent<Transform>();
	}

	void Update()
	{
		mTransform.Rotate(axis * speed * Time.deltaTime);
	}
}

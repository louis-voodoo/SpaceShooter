using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestsRopes : MonoBehaviour {

    Rigidbody2D RB;
    HingeJoint2D joint;
    public float ForceAngle = 0;
    public float ForceAmplitude = 100;

	// Use this for initialization
	void Start () {
        RB = this.GetComponent<Rigidbody2D>();
        joint = this.GetComponent<HingeJoint2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            RB.AddForce(new Vector2(Mathf.Cos(Mathf.Deg2Rad * ForceAngle), Mathf.Sin(Mathf.Deg2Rad * ForceAngle)) * ForceAmplitude, ForceMode2D.Impulse);
    }
}

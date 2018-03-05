using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardColliders : MonoBehaviour {

	public GameObject target;

	void OnEnable (){
		if (target == null)
			target = transform.parent.gameObject;
	}

	void OnTriggerEnter(Collider other){
		target.SendMessage ("OnForwardedTriggerEnter", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerEnter2D(Collider2D other){
		target.SendMessage ("OnForwardedTriggerEnter2D", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerExit2D(Collider2D other){
		target.SendMessage ("OnForwardedTriggerExit2D", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerExit(Collider other){
		target.SendMessage ("OnForwardedTriggerExit", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerStay(Collider other){
		target.SendMessage ("OnForwardedTriggerStay", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnTriggerStay2D(Collider2D other){
		target.SendMessage ("OnForwardedTriggerStay2D", new ForwardEvent (this.gameObject, other.gameObject, null, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionEnter(Collision col){
		target.SendMessage ("OnForwardedCollisionEnter", new ForwardEvent (this.gameObject, col.collider.gameObject, col, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionEnter2D(Collision2D col){
		target.SendMessage ("OnForwardedCollisionEnter2D", new ForwardEvent (this.gameObject, col.collider.gameObject, null, col), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionExit2D(Collision2D col){
		target.SendMessage ("OnForwardedCollisionExit2D", new ForwardEvent (this.gameObject, col.collider.gameObject, null, col), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionExit(Collision col){
		target.SendMessage ("OnForwardedCollisionExit", new ForwardEvent (this.gameObject, col.collider.gameObject, col, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionStay(Collision col){
		target.SendMessage ("OnForwardedCollisionStay", new ForwardEvent (this.gameObject, col.collider.gameObject, col, null), SendMessageOptions.DontRequireReceiver);
	}

	void OnCollisionStay2D(Collision2D col){
		target.SendMessage ("OnForwardedCollisionStay2D", new ForwardEvent (this.gameObject, col.collider.gameObject, null, col), SendMessageOptions.DontRequireReceiver);
	}
}

public class ForwardEvent {

	public GameObject sender;
	public GameObject other;
	public Collision collision;
	public Collision2D collision2D;

	public ForwardEvent (GameObject _sender, GameObject _other, Collision _col, Collision2D _col2D)
	{
		this.sender = _sender;
		this.other = _other;
		this.collision = _col;
		this.collision2D = _col2D;
	}
}

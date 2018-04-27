using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltMoverRight : MonoBehaviour {
	Rigidbody2D rigidBody;
	public float speed;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
		Vector2 movement = new Vector2 (0.5f, 0.5f);
		rigidBody.AddForce (movement * speed);
	}
}

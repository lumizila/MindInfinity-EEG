using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Rigidbody2D r2d = this.GetComponent<Rigidbody2D> ();
		r2d.AddTorque (Random.Range(-50.0f, 50.0f));
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTrigger : MonoBehaviour {
	public GameObject explosion;
	public Transform parent;
	public GameObject nextStar;
	public GameObject nextTilt;
	public GameObject currentTilt;

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			Instantiate (explosion, transform.position, transform.rotation, parent);
			Destroy (gameObject);
			currentTilt.SetActive (false);
			nextStar.SetActive (true);
			nextTilt.SetActive (true);
		}
	}
}

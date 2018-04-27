using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosion : MonoBehaviour {

	// Use this for initialization
	void Update () {
		if (this.GetComponent<ParticleSystem> ().isStopped == true) {
			Destroy (this.gameObject);
		}
	}

}

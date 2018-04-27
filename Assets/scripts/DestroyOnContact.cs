using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour {
	public GameObject explosion;
	public GameObject playerExplosion;
	public Transform parent;
	public GameObject gameController;


	void OnTriggerEnter2D(Collider2D other){
		gameController = GameObject.Find ("Canvas");
		parent = gameController.transform;
		if (other.tag == "Boundary") {
			return;
		}
		Instantiate (explosion, transform.position, transform.rotation, parent);

		if (other.tag == "Player") {
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation, parent);
			other.GetComponent<Player> ().closeWebsocketAndSubscriptions ();
			Destroy (other.gameObject);
			Destroy (gameObject);
			gameController.GetComponent<GameController>().GameOver ();
		}
		Destroy (other.gameObject);
		Destroy (gameObject);
		return;
	}
}

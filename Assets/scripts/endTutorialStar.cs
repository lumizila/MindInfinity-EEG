using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endTutorialStar : MonoBehaviour {
	public PlayerTraining player;
	public GameObject explosion;
	public Transform parent;
	public string targetScene;

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			Instantiate (explosion, transform.position, transform.rotation, parent);
			player.closeWebsocketAndSubscriptions ();
			Destroy (gameObject);
			SceneManager.LoadScene (this.targetScene);
		}
	}
}

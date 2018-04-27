using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnTriggerClick : MonoBehaviour {
	
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			this.gameObject.GetComponent<Button> ().interactable = true;
			this.gameObject.GetComponent<Button> ().onClick.Invoke ();
		}
	}

}

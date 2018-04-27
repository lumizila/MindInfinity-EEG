using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollStars : MonoBehaviour {

	void ChangeWhite(){
		this.GetComponent<MeshRenderer> ().material.color = Color.white;
	}
	void ChangeYellow(){
		this.GetComponent<MeshRenderer> ().material.color = Color.yellow;
	}
	void ChangeRed(){
		this.GetComponent<MeshRenderer> ().material.color = Color.red;
	}

	// Update is called once per frame
	void Update () {
		MeshRenderer mr = GetComponent<MeshRenderer>();
		Vector2 offset = mr.material.mainTextureOffset;
		offset.y += Time.deltaTime / 14;
		mr.material.mainTextureOffset = offset;
	}
}

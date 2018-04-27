using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CloseParentOnClick : MonoBehaviour {
	public GameObject parent;
	public GameObject gameObjects;

	public void DisactivateParent(){
		parent.SetActive (false);
	}

	public void ActivateObjects(){
		gameObjects.SetActive (true);
	}

}

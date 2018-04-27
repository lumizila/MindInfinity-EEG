using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetObjects : MonoBehaviour {
	public GameObject global;
	ClassesJSON globalscripts;
	public GameObject unityDispatcherPrefab;

	void Awake(){
		if(GameObject.Find("GlobalScripts(Clone)") == null){
			Instantiate(global);
		}
	}
	// Use this for initialization
	void Start () {
		globalscripts = global.GetComponent<ClassesJSON>();
		if (globalscripts != null) {
			globalscripts.authToken = "";
			globalscripts.currentHeadset = null;
			globalscripts.session = null;
			globalscripts.headsetModel = "";
			globalscripts.language = "";
			globalscripts.firstX = 0f;
			globalscripts.firstY = 0f;
			globalscripts.firstZ = 0f;
			globalscripts.firstUpdate = false;
		}
	}

}

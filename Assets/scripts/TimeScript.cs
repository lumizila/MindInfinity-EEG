using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeScript : MonoBehaviour {
	// Update is called once per frame
	public bool stopTime;
	public int time;
	ClassesJSON globalscripts;
	GameController controller;
	int counter;
	int firstTime;

	void Start(){
		globalscripts = GameObject.Find("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		controller = GameObject.Find("Canvas").GetComponent<GameController>();
		counter = 0;
		stopTime = false;
		firstTime = (int)Time.timeSinceLevelLoad;
	}

	void Update () {
		if (stopTime == false) {
			time = (int)Time.timeSinceLevelLoad - firstTime;

			if (time % 15 == 0 && time > counter) {
				//passed level
				controller.changeLevel((int)(time / 15));
				counter = counter + 15;
			}

			if (globalscripts.language == "pt"){
				this.GetComponent<TextMeshProUGUI> ().text = "Tempo: " + time.ToString ();
			}
			else{
				this.GetComponent<TextMeshProUGUI> ().text = "Time: " + time.ToString ();
			}
		}
	}
}

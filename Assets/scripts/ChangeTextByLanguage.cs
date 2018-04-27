using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeTextByLanguage : MonoBehaviour {
	ClassesJSON globalscripts;
	public string entext;
	public string pttext;
	// Use this for initialization
	void Start () {
		globalscripts = GameObject.Find("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		if (globalscripts.language == "en") {
			this.GetComponent<TextMeshProUGUI> ().text = entext;
		} else if (globalscripts.language == "pt") {
			this.GetComponent<TextMeshProUGUI> ().text = pttext;
		} else {
			this.GetComponent<TextMeshProUGUI> ().text = entext;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonClicked : MonoBehaviour {
	public bool connected;
	public HeadsetClass headset;
	public GameObject nextBtn;
	// Use this for initialization
	void Start () {
		nextBtn = GameObject.Find ("Next");
		connected = false;		
		ClassesJSON globalscripts = GameObject.Find("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		GameObject grid = GameObject.Find ("Grid");

		//adding action to onclick 
		this.GetComponent<Button> ().onClick.AddListener (delegate { 
			//if the headset of this button is not yet connected
			if(connected == false){
				nextBtn.GetComponent <Image> ().color = new Color32(71,23,246,255);
				nextBtn.GetComponent<Button>().interactable = true;
				//connect it by creating new session
				//TODO

				//set this headset on global
				globalscripts.selectHeadset (headset);
				this.GetComponentInChildren<TextMeshProUGUI> ().text += " --> Connected"; 
				connected = true;

				//disconnect other headsets
				//TODO do I have to send a disconnection request ?

				foreach (Transform button in grid.transform){
					//If it actually has this script
					if(button.GetComponent<ButtonClicked>() != null){
						ButtonClicked bc = button.GetComponent<ButtonClicked>();
						if(bc.headset.id != this.headset.id){
							bc.connected = false;
							button.GetComponentInChildren<TextMeshProUGUI>().text = button.GetComponent<ButtonClicked>().headset.id;
						}
					}
				}
			}
		});

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

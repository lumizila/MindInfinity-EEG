using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using UnityEngine;
using TMPro;

public class Select: MonoBehaviour{
	public GameObject headsetButton;
	public List<string> devices;
	WebSocket ws;
	public GameObject parent;
	HeadsetResultClass headsets;

	/*public static GameObject FindObject(GameObject parent, string name)
	{
	//TODO as vezes ta dando null reference exception na linha de baixo
		Transform[] trs= parent.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in trs){
			if(t.name == name){	
				return t.gameObject;
			}
		}
		return null;
	}*/

	public IEnumerator populateButtons(){
		
		//TODO check for headsets not here anymore and delete the button related to them.
		if (headsets.result != null) {
			foreach (HeadsetClass headset in headsets.result) {
				if (!devices.Contains (headset.id)) {
					devices.Add (headset.id);
					GameObject newButton = Instantiate (headsetButton, parent.transform);
					newButton.GetComponentInChildren<TextMeshProUGUI> ().text = headset.id; 
					newButton.GetComponent<ButtonClicked> ().headset = headset;
					newButton.SetActive (true);
				}
			}
		} else {
			Debug.Log ("headsets.result eh null");
		}
		headsets = null;
		yield return 0;
	}

	public void treatMessage(object s, MessageEventArgs e){
		var generalResult= JsonUtility.FromJson<ResultClass>(e.Data);

		//if message is headsets data
		if (generalResult.id == 1) {
			HeadsetResultClass res = JsonUtility.FromJson<HeadsetResultClass> (e.Data);
			headsets = res;
			UnityMainThreadDispatcher.Instance ().Enqueue (populateButtons ());
		}

		return;
	}

	void Start()
	{
		headsets = null;
		string URL = "wss://emotivcortex.com:54321";
		ws = new WebSocket(URL);

		ws.OnMessage += new EventHandler<MessageEventArgs> (treatMessage);

		ws.OnOpen += (sender, e) => 
		{
			Debug.Log ("Connected to socket");
		};

		ws.OnError += (sender, e) => {
			Debug.Log("Error on the socket: "+ e.Message);
		};

		ws.Connect ();

		InvokeRepeating("checkDevices", 1.0f, 0.5f);

	}

	public void checkDevices(){
		if (ws.IsAlive == true) {
			ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"queryHeadsets\",\"params\": {},\"id\": 1}");
		}
	}
		
	public void closeWebsocketAndSubscriptions(){
		ws.Close ();
	}
}
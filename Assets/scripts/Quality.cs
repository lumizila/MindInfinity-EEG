using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Linq;

public class Quality : MonoBehaviour {
	WebSocket ws;
	ClassesJSON globalscripts;
	GameObject qualityText;

	public void parseDev(string data, DevClass dev){
		devEventClass res = JsonUtility.FromJson<devEventClass> (data);
		dev.battery = res.dev[0];
		dev.signal = res.dev[1];

		//parse of signals
		//delete the first `"dev":[' chars
		//TODO checar se substring[8 ou 7]
		string chans = data.Substring(8);
		int indexofList = chans.IndexOf ("[");
		chans = chans.Substring (indexofList + 1, (chans.LastIndexOf ("]") - indexofList - 2));
		dev.channels = chans.Split(',').Select(float.Parse).ToList();
	}

	public IEnumerator populateQuality(string data){
		DevClass dev = new DevClass();
		parseDev(data, dev);
		if (globalscripts.language == "pt") {
			qualityText.GetComponent<TextMeshProUGUI> ().text = "Bateria do dispositivo: " + dev.battery;
			int i = 1;
			foreach (float channel in dev.channels){
				qualityText.GetComponent<TextMeshProUGUI> ().text += "\n Canal " + i + ": " + channel.ToString ();
				i++;
			}
		} else {
			qualityText.GetComponent<TextMeshProUGUI> ().text = "Device battery: " + dev.battery;
			int i = 1;
			foreach (float channel in dev.channels){
				qualityText.GetComponent<TextMeshProUGUI> ().text += "\n Channel " + i + ": " + channel.ToString ();
				i++;
			}
		}
		yield return 0;
	}

	public void treatMessage(object s, MessageEventArgs e){
		//TODO dando erro embaixo aqui
		var generalResult= JsonUtility.FromJson<ResultClass>(e.Data);
		//if message is new session
		if (generalResult.id == 1) {
			var res = JsonUtility.FromJson<NewSessionResultClass> (e.Data);
			Debug.Log ("SessionID: " + res.result.id + "\n headset: " + res.result.headset.id);
			globalscripts.session = res.result;
		} else if (generalResult.id == 2) { //if this is connection data
			Debug.Log ("Received answer for creating  new subscription...");
		} else if (e.Data.IndexOf("\"jsonrpc\":\"2.0\"") == -1) { //dev information data
			UnityMainThreadDispatcher.Instance ().Enqueue (populateQuality(e.Data));
		}
	}

	// Use this for initialization
	void Start () {
		globalscripts = GameObject.Find ("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		qualityText = GameObject.Find ("Quality");
		Debug.Log (globalscripts.currentHeadset.id);

		string URL = "wss://emotivcortex.com:54321";

		ws = new WebSocket (URL);

		ws.OnMessage += new EventHandler<MessageEventArgs> (treatMessage);

		ws.OnOpen += (sender, e) => 
		{
			Debug.Log ("Connected to socket");
		};

		ws.OnError += (sender, e) => {
			Debug.Log("Error on the socket at Quality script:"+ e.Message);
		};

		ws.Connect ();

		//creating a new session with the chosen headset
		if (globalscripts.currentHeadset.id != "") {
			ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"createSession\",\"params\": {" +
				"\"_auth\": \""+globalscripts.authToken+"\", \"headset\": \"" + globalscripts.currentHeadset.id + "\", \"status\": \"open\"},\"id\": 1}");
		} else {
			Debug.Log ("Erro: o headset nao foi escolhido.");
			Application.Quit ();
		}

		//subscribing to dev data 
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"subscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"dev\"]},\"id\": 2}");

	}

	public void Next(){
		//unsubscribing to dev data
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"unsubscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"dev\"]},\"id\": 4}");
		ws.Close ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}
		
} 

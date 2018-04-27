using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using TMPro;
using System;
using UnityEngine.UI;

public class FrownTrain : MonoBehaviour {
	WebSocket ws;
	ClassesJSON globalscripts;
	public GameObject defaultText;
	public GameObject saving;
	public GameObject tryAgain;
	public GameObject save;
	public GameObject failed;
	public string status;

	void Start(){
		status = "none";
		globalscripts = GameObject.Find ("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		//create socket to communicate
		string URL = "wss://emotivcortex.com:54321";
		ws = new WebSocket (URL);

		ws.OnMessage += new EventHandler<MessageEventArgs> (treatMessage);

		ws.OnOpen += (sender, e) => 
		{
			Debug.Log ("Connected to socket in training");
		};

		ws.OnError += (sender, e) => {
			Debug.Log("Error on the socket:"+ e.Message);
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
		//send message to connect to sys stream
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"subscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"sys\"]},\"id\": 3}");
	}

	public void UnsubscribeSys(){
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"unsubscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"sys\"]},\"id\": 4}");
		ws.Close();
	}

	public IEnumerator Loading(){
		defaultText.SetActive (false);
		saving.SetActive (true);
		yield return 0;
	}

	public IEnumerator Succeeded(){
		saving.SetActive (false);
		tryAgain.SetActive (true);
		save.GetComponent <Image> ().color = new Color32 (71, 23, 243, 255);
		save.GetComponent <Button> ().interactable = true;
		ws.Send ("{\"jsonrpc\": \"2.0\", \"method\": \"training\", \"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"detection\": "+
			"\"facialExpression\", \"session\": \""+globalscripts.session.id+"\", \"action\":" +
			" \"frown\", \"status\": \"accept\"}, \"id\": 5}");
		yield return 0;
	}

	public IEnumerator Failed(){
		saving.SetActive (false);
		failed.SetActive (true);
		yield return 0;
	}

	public void treatMessage(object s, MessageEventArgs e){
		ResultClass generalResult = new ResultClass();
		generalResult.id = 0;
		generalResult = JsonUtility.FromJson<ResultClass>(e.Data);

		//if message is new session
		if (generalResult.id == 1) {
			var res = JsonUtility.FromJson<NewSessionResultClass> (e.Data);
			Debug.Log ("SessionID: " + res.result.id + "\n headset: " + res.result.headset.id);
			globalscripts.session = res.result;
		}
		//se for resposta 
		else if (generalResult.id == 2) {
			//if start succeeded
			if (e.Data.IndexOf ("Set up training successfully for action frown with status start") != -1) {
				UnityMainThreadDispatcher.Instance ().Enqueue (Loading ());
				status = "setup";
			}
		} else if (generalResult.id == 5) {
			//if accept succeeded
			if (e.Data.IndexOf ("Set up training successfully for action frown with status accept") != -1){
				status = "succeded";
			}
		} else {
			if (e.Data.IndexOf ("FE_Succeeded") != -1) {
				UnityMainThreadDispatcher.Instance ().Enqueue (Succeeded ());
			} else if (e.Data.IndexOf ("FE_Failed") != -1) {
				Debug.Log ("treinamento falhou" + e.Data);
				UnityMainThreadDispatcher.Instance ().Enqueue (Failed ());
				status = "none";
			} else {
				Debug.Log (e.Data);
			}
		}
	}

	public void Select(){
		//send message to start train frown
		ws.Send ("{\"jsonrpc\": \"2.0\", \"method\": \"training\", \"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"detection\": "+
			"\"facialExpression\", \"session\": \""+globalscripts.session.id+"\", \"action\":" +
			" \"frown\", \"status\": \"start\"}, \"id\": 2}");
		//TODO testar se a pessoa clica no botao pra tentar de novo

	}
}

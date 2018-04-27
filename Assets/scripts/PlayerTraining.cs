using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

public class PlayerTraining : MonoBehaviour {
	private Rigidbody2D rigidBody;
	public float speed;
	public Transform parent;
	WebSocket ws;
	ClassesJSON globalscripts;
	float xAxis;
	float yAxis;
	float zAxis;
	bool first;
	float firstX;
	float firstY;
	float firstZ;
	GameController controller;
	bool firstUpdate;
	public float maxHeadsetResponseTime;
	float lastResponse;

	public IEnumerator GetTime(){
		lastResponse = Time.time + maxHeadsetResponseTime;
		yield return 0;
	}

	public void treatMessage(object s, MessageEventArgs e){
		var generalResult= JsonUtility.FromJson<ResultClass>(e.Data);
		if (controller.gamePaused == false) {
			if (generalResult.id == 1) {
				var res = JsonUtility.FromJson<NewSessionResultClass> (e.Data);
				globalscripts.session = res.result;
			}		
			//receiveing confirmation of accelerometer stream
			else if (generalResult.id == 2) { //if this is connection data
				Debug.Log ("Received answer for creating  new subscription to accelerometer stream...");
			} else if (e.Data.IndexOf ("\"jsonrpc\":\"2.0\"") == -1 && e.Data.IndexOf ("\"mot\"") != -1) { //mot information data
				UnityMainThreadDispatcher.Instance ().Enqueue (GetTime ());

				motEventClass motResult = JsonUtility.FromJson<motEventClass> (e.Data);
				//x axis = 4
				xAxis = motResult.mot [4];
				//y axis
				yAxis = motResult.mot [5];
				//z axis = 6 
				zAxis = motResult.mot [6];
				if (globalscripts.firstUpdate == false) {
					globalscripts.updateFirstShipPosition (xAxis,yAxis,zAxis);
					firstX = globalscripts.firstX;
					firstY = globalscripts.firstY;
					firstZ = globalscripts.firstZ;
				}
			} 
		}
	}

	void Start(){
		lastResponse = 0;
		globalscripts = GameObject.Find ("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		firstX = globalscripts.firstX;
		firstY = globalscripts.firstY;
		firstZ = globalscripts.firstZ;

		controller = GameObject.Find ("Canvas").GetComponent<GameController>();
		rigidBody = GetComponent<Rigidbody2D> ();

		//create socket to communicate
		string URL = "wss://emotivcortex.com:54321";
		ws = new WebSocket (URL);

		ws.OnMessage += new EventHandler<MessageEventArgs> (treatMessage);

		ws.OnOpen += (sender, e) => 
		{
			Debug.Log ("Connected to socket");
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
		//send message to connect to accelerometer stream
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"subscribe\",\"params\": {" +
			"\"_auth\": \"" + globalscripts.authToken + "\", \"streams\": [\"mot\"]},\"id\": 2}");
		
	}

	public void UnsubscribeMot(){
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"unsubscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"mot\"]},\"id\": 4}");
		ws.Close();
	}

	void FixedUpdate(){
		//code to move ship with EEG headset

		//headset has not responded for too long, try to reconnect
		if (lastResponse < Time.time) {
			//websocket is dead
			if (ws.IsAlive == false) {
				Debug.Log ("Erro: websocket morreu");
				ws.Connect ();
			} 
			Debug.Log ("Erro: sessao morreu");

			//creating a new session with the chosen headset
			if (globalscripts.currentHeadset.id != "") {
				ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"createSession\",\"params\": {" +
				"\"_auth\": \"" + globalscripts.authToken + "\", \"headset\": \"" + globalscripts.currentHeadset.id + "\", \"status\": \"open\"},\"id\": 1}");
			} else {
				Debug.Log ("Erro: o headset nao foi escolhido.");
				Application.Quit ();
			}
			//send message to connect to accelerometer stream
			ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"subscribe\",\"params\": {" +
			"\"_auth\": \"" + globalscripts.authToken + "\", \"streams\": [\"mot\"]},\"id\": 2}");
			GetTime ();
			
		} else {
			float moveHorizontal = (xAxis - firstX + (yAxis - firstY)) / 2000;
			float moveVertical = ((zAxis - firstZ) * (-1)) / 2000;
			if (moveVertical > 1) {
				moveVertical = 1;
			}
			if (moveHorizontal > 1) {
				moveHorizontal = 1;
			}
			if (moveVertical < -1) {
				moveVertical = -1;
			}
			if (moveHorizontal < -1) {
				moveHorizontal = -1;
			}
			Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
			rigidBody.AddForce (movement * speed);
		}
	}

	public void closeWebsocketAndSubscriptions(){
		UnsubscribeMot ();
		ws.Close ();
	}
}

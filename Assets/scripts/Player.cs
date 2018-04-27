using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	private Rigidbody2D rigidBody;
	public float speed;
	public GameObject shot;
	public GameObject shotRight;
	public GameObject shotLeft;
	public Transform parent;
	WebSocket ws;
	ClassesJSON globalscripts;
	public float xAxis;
	public float yAxis;
	public float zAxis;
	public bool first;
	public float firstX;
	public float firstY;
	public float firstZ;
	public GameObject background;
	float originalRed;
	float originalGreen;
	Color bcolor;
	MeshRenderer brenderer;
	float frownValue;
	GameController controller;
	public float fireRate;
	private float nextFire;
	public float maxHeadsetResponseTime;
	float lastResponse;

	void Shoot(){
		GameObject shotClone = Instantiate (shot, parent);
		Vector3 pos = new Vector3();
		//pos = this.gameObject.transform.position;
		pos.Set (this.gameObject.transform.position.x, (this.gameObject.transform.position.y + 0.5f), this.gameObject.transform.position.z);
		shotClone.transform.position = pos;
		shotClone.SetActive (true);
		this.GetComponent<AudioSource> ().Play ();
	}

	public IEnumerator changeBackground(){
		brenderer.material.color = new Vector4 (bcolor.r, bcolor.g, bcolor.b, bcolor.a);
		yield return 0;
	}

	public IEnumerator GetFrownValue(string data){
		//{"fac":["neutral","frown",0.81891393661499,"neutral",0.0],"sid":"b345e77e-ccaf-4d14-add2-a1f1c2556113","time":78233.984375}
		//parse of signals
		//delete the first `{"fac":[' chars
		string chans = data.Substring(8);
		//getting index of ]
		int indexofList = chans.IndexOf ("]");
		//cutting the string to only have the fac data
		chans = chans.Substring (0, indexofList);
		//splitting by comas
		string[] words = chans.Split(',');
		//converting to integer
		frownValue = float.Parse(words[2]);
		yield return 0;
	}

	public IEnumerator Shoot3(){
		//shootright
		GameObject shotClone = Instantiate (shotRight, parent);
		Vector3 pos = new Vector3();
		//pos = this.gameObject.transform.position;
		pos.Set (this.gameObject.transform.position.x, (this.gameObject.transform.position.y + 0.5f), this.gameObject.transform.position.z);
		shotClone.transform.position = pos;
		shotClone.SetActive (true);
		this.GetComponent<AudioSource> ().Play ();

		//shoorleft
		GameObject shotClone1 = Instantiate (shotLeft, parent);
		Vector3 pos1 = new Vector3();
		//pos = this.gameObject.transform.position;
		pos1.Set (this.gameObject.transform.position.x, (this.gameObject.transform.position.y + 0.5f), this.gameObject.transform.position.z);
		shotClone1.transform.position = pos1;
		shotClone1.SetActive (true);
		this.GetComponent<AudioSource> ().Play ();
		yield return 0;
	}

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
				Debug.Log ("Received answer for creating  new subscription to accelerometer/facial streams...");
			} else if (e.Data.IndexOf ("\"jsonrpc\":\"2.0\"") == -1 && e.Data.IndexOf ("\"mot\"") != -1) { //mot information data
				motEventClass motResult = JsonUtility.FromJson<motEventClass> (e.Data);
				UnityMainThreadDispatcher.Instance ().Enqueue (GetTime ());
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
			} else if (e.Data.IndexOf ("\"jsonrpc\":\"2.0\"") == -1 && e.Data.IndexOf ("\"fac\"") != -1) { //fac information data
				//test if frown
				if (e.Data.IndexOf ("\"frown\"") != -1) {
					UnityMainThreadDispatcher.Instance ().Enqueue (GetFrownValue (e.Data));
					if (frownValue > 0.5f && Time.time > nextFire) {
						nextFire = Time.time + fireRate;
						UnityMainThreadDispatcher.Instance ().Enqueue (Shoot3 ());
					}
				}
			} else if (e.Data.IndexOf ("\"jsonrpc\":\"2.0\"") == -1 && e.Data.IndexOf ("\"met\"") != -1) {
				bcolor.r = originalRed;
				bcolor.g = originalGreen;
				metEventClass metResult = JsonUtility.FromJson<metEventClass> (e.Data);
				if ((metResult.met [1] == 0.0f && metResult.met [2] == 0.0f) || (Mathf.Abs (metResult.met [1] - metResult.met [2]) <= 0.05)) {
					//Debug.Log ("You're neutral:" + metResult.met [1] + ", " + metResult.met [2]);
				} else if (metResult.met [1] > (metResult.met [2] * 0.9)) { //if stress is bigger than relax
					bcolor.r = bcolor.r + metResult.met [1] * 10;
				} else {
					bcolor.g = bcolor.g + metResult.met [2] * 10;
				}

				UnityMainThreadDispatcher.Instance ().Enqueue (changeBackground ());
			}
		}
	}

	void Start(){
		bcolor = background.GetComponent<MeshRenderer> ().material.color;
		originalRed = bcolor.r;
		originalGreen = bcolor.g;

		brenderer = background.GetComponent<MeshRenderer> ();
		globalscripts = GameObject.Find ("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		firstX = globalscripts.firstX;
		firstY = globalscripts.firstY;
		firstZ = globalscripts.firstZ;
		controller = GameObject.Find ("Canvas").GetComponent<GameController>();
		rigidBody = GetComponent<Rigidbody2D> ();
		InvokeRepeating("Shoot", 1.0f, 0.8f);

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
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"mot\", \"fac\", \"met\"]},\"id\": 2}");
	}

	public void UnsubscribeMot(){
		ws.Send ("{\"jsonrpc\": \"2.0\",\"method\": \"unsubscribe\",\"params\": {" +
			"\"_auth\": \""+globalscripts.authToken+"\", \"streams\": [\"mot\", \"fac\", \"met\"]},\"id\": 4}");
		ws.Close ();
	}

	public void closeWebsocketAndSubscriptions(){
		UnsubscribeMot ();
	}

	void FixedUpdate(){
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
			//code to move ship with EEG headset
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
}

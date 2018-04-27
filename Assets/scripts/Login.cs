using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using WebSocketSharp;
using System.Threading;

public class Login : MonoBehaviour {
	Text id;
	Text pwd;
	WebSocket websocket;
	ClassesJSON globalscripts;

	void Start(){
		globalscripts = GameObject.Find("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
	}

	public void SelectPT()
	{
		/*//how to get data from input
		id = GameObject.Find("Canvas/Login/EmotivID/Text").GetComponent<Text>();
		pwd = GameObject.Find("Canvas/Login/Password/Text").GetComponent<Text>();
		//Debug.Log(id.text);
		//Debug.Log(pwd.text); */

		string URL = "wss://emotivcortex.com:54321";

		var ws = new WebSocket (URL);
		ws.OnMessage += (sender, e) => {
			var generalResult= JsonUtility.FromJson<AuthResultClass>(e.Data);
			globalscripts.authToken = generalResult.result._auth;
			ws.Close ();

		};
		ws.OnOpen += (sender, e) => {
			Debug.Log 	("connected to socket");
		};
		ws.OnError += (sender, e) => {
			Debug.Log("Error on the socket:"+ e.Message);
		};
		ws.Connect ();

		string s = "{\"jsonrpc\": \"2.0\",\"method\": \"authorize\",\"params\": {}, \"id\": 1}";

		ws.Send (s);

		globalscripts.language = "pt";
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);

	} 

	public void SelectEN()
	{
		/*//how to get data from input
		id = GameObject.Find("Canvas/Login/EmotivID/Text").GetComponent<Text>();
		pwd = GameObject.Find("Canvas/Login/Password/Text").GetComponent<Text>();
		//Debug.Log(id.text);
		//Debug.Log(pwd.text);*/

		string URL = "wss://emotivcortex.com:54321";

		var ws = new WebSocket (URL);
		ws.OnMessage += (sender, e) => {
			var generalResult= JsonUtility.FromJson<AuthResultClass>(e.Data);
			globalscripts.authToken = generalResult.result._auth;
			ws.Close ();
		};
		ws.OnOpen += (sender, e) => {
			Debug.Log 	("connected to socket");
		};
		ws.OnError += (sender, e) => {
			Debug.Log("Error on the socket:"+ e.Message);
		};
		ws.Connect ();

		string s = "{\"jsonrpc\": \"2.0\",\"method\": \"authorize\",\"params\": {}, \"id\": 1}";

		ws.Send (s);

		globalscripts.language = "en";

		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);

	} 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ClassesJSON: MonoBehaviour{
	public HeadsetClass currentHeadset;
	public string authToken;
	public SessionClass session;
	public string headsetModel;
	public string language;
	public float firstX;
	public float firstY;
	public float firstZ;
	public bool firstUpdate;
	public static ClassesJSON globalscripts;

	public void updateFirstShipPosition(float x, float y, float z){
		firstX = x;
		firstY = y;
		firstZ = z;
		firstUpdate = true;
	}

	public void selectHeadset (HeadsetClass hs){
		currentHeadset = hs;
	}

	void Awake(){
		/*if (globalscripts == null) {
			DontDestroyOnLoad(this.gameObject);
			globalscripts = this;
		} else {
			DestroyObject(gameObject);
		}*/
	}

	public void NextPage()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

	public void QuitApp()
	{
		Application.Quit ();
	}

	public void SelectDevice()
	{
		SceneManager.LoadScene ("2-Select");
	}
}

//Classes referent WEBSOCKET RESPONSES --------------------------

[Serializable]
public class ResultClass{
	public int id;
	public string jsonrpc;
	public List<string> result;
}

[Serializable]
public class AuthResultClass{
	public int id;
	public string jsonrpc;
	public AuthClass result;
}

[Serializable]
public class HeadsetResultClass{
	public int id;
	public string jsonrpc;
	public List<HeadsetClass> result;
}

[Serializable]
public class NewSessionResultClass{
	public int id;
	public string jsonrpc;
	public SessionClass result;
}

//public class 
//General classes -------------------------

[Serializable]
public class LoginClass{
	public string username;
	public string password;
	public string client_id;
	public string client_secret;
}

[Serializable]
public class SettingsClass{
	int eegRate;
	int eegRes;
	int memsRate;
	int memsRes;
	string mode;
}

[Serializable]
public class HeadsetClass{
	public string connectedBy;
	public string dongle;
	public string firmware;
	public string id;
	public string label;
	public ArrayList sensors;
	public SettingsClass settings;
	public string status;
}


[Serializable]
public class SessionClass{
	public string appId;
	public HeadsetClass headset;
	public string id;
	public string license;
	public List<string> logs;
	public List<MarkerClass> markers;
	public string owner;
	public string profile;
	public string project;
	public bool recording;
	public string started;
	public string status;
	public string stopped;
	public List<string> streams;
	public string subject;
	public List<string> tags;
	public string title;
}

[Serializable] 
public class MarkerClass{
	public int code;
	public List<string> enums;
	public List<EventClass> events;
	public string label;
	public string port;
}

[Serializable]
public class DevClass{
	public int battery;
	public int signal;
	public List<float> channels;
}

[Serializable]
public class EventClass{
	public Array mot;
	public Array eeg;
	public ArrayList com;
	public ArrayList fac;
	public Array met;
	public DevClass dev;
	public Array pow;
	public List<string> sys;
	public string sid;
	public float time;
}

[Serializable]
public class AuthClass{
	public string _auth;
}

// STREAM EVENT classes (referent to the different streams of data)

[Serializable]
public class devEventClass{
	public List<int> dev;
	public string sid;
	public float time;
}

public class motEventClass{
	public List<float> mot;
	public string sid;
	public float time;
}

public class metEventClass{
	public List<float> met;
	public string sid;
	public float time;
}
// STREAM ACTUAL CLASSES ------------------------------------------

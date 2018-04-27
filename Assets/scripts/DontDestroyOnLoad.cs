using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {
	void Awake(){
		GameObject global = GameObject.Find ("UnityMainThreadDispatcher(Clone)");
		DontDestroyOnLoad(global);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour {

	public string targetScene;

	public void GoToScene(){
		SceneManager.LoadScene (this.targetScene);
	}

	void Start(){
		this.gameObject.GetComponent<Button> ().onClick.AddListener(GoToScene);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
	public GameObject timer;
	public GameObject player;
	public GameObject time;
	public GameObject canvas;

	// Use this for initialization
	void Start () {
		StartTimer ();
	}
	
	public void StartTimer(){
		Debug.Log ("started timer");
		StartCoroutine(waiter());
	}

	IEnumerator waiter()
	{
		timer.GetComponent<TextMeshProUGUI>().text = "3";
		yield return new WaitForSeconds (1f);
		timer.GetComponent<TextMeshProUGUI>().text = "2";
		yield return new WaitForSeconds (1f);
		timer.GetComponent<TextMeshProUGUI>().text = "1";
		yield return new WaitForSeconds (1f);
		timer.GetComponent<TextMeshProUGUI>().text = "Go!";
		yield return new WaitForSeconds (0.5f);
		ActivateGameControllerScript ();
		ActivatePlayerScript ();
		ActivateTimeScript ();
		timer.SetActive (false);
	}

	public void ActivateTimeScript(){
		time.GetComponent<TimeScript> ().enabled = true;
	}
	public void ActivatePlayerScript(){
		player.GetComponent<Player> ().enabled = true;
	}
	public void ActivateGameControllerScript(){
		canvas.GetComponent<GameController> ().enabled = true;
	}
}

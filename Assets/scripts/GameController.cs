using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour {
	public GameObject meteor1;
	public GameObject meteor2;
	public bool gameOver;
	public Vector3 spawnValues;
	public Transform parent;
	public GameObject endScreen;
	public GameObject timeObject;
	public GameObject changeDeviceBtn;
	public GameObject pauseGameBtn;
	public GameObject pauseScreen;
	public bool gamePaused;
	public GameObject player;
	public GameObject goverText;
	ClassesJSON globalscripts;
	public int level;
	float meteorSpeed;
	public GameObject leveltext;
	float invokeMeteor1;
	float invokeMeteor2;
	// Use this for initialization
	void Start () {
		globalscripts = GameObject.Find("GlobalScripts(Clone)").GetComponent<ClassesJSON>();
		invokeMeteor1 = 1.1f;
		invokeMeteor2 = 1.3f;
		InvokeRepeating("SpawnMeteor1", 1.0f, 1.05f);
		InvokeRepeating("SpawnMeteor2", 1.0f, 1.15f);
		gamePaused = false;
		level = 1;
		meteorSpeed = -15;
	}
	IEnumerator waiter()
	{
		while (leveltext.GetComponent<TextMeshProUGUI> ().color.a  < 1) {
			leveltext.GetComponent<FadeInAndOut> ().FadeIn ();
			//Wait for 4 seconds
			yield return new WaitForSeconds (0.05f);
		}

		while (leveltext.GetComponent<TextMeshProUGUI> ().color.a > 0) {
			leveltext.GetComponent<FadeInAndOut> ().FadeOut ();
			//Wait for 4 seconds
			yield return new WaitForSeconds (0.05f);
		}
	}
	public void changeLevel(int lvl){
		this.level = lvl;
		meteorSpeed = meteorSpeed * 1.1f;
		leveltext.GetComponent<TextMeshProUGUI> ().text = "Level " + this.level;
		StartCoroutine(waiter());
		if (invokeMeteor1 > 0.3 && invokeMeteor2 > 0.3) {
			CancelInvoke ();
			invokeMeteor1 = invokeMeteor1 - 0.1f;
			invokeMeteor2 = invokeMeteor2 - 0.1f;
			InvokeRepeating ("SpawnMeteor1", 0.0f, invokeMeteor1);
			InvokeRepeating ("SpawnMeteor2", 0.0f, invokeMeteor2);
		}
	}

	public void PauseGame(){
		if (gamePaused == false) {
			Time.timeScale = 0;
			gamePaused = true;
			if (globalscripts.language == "pt") {
				pauseGameBtn.GetComponentInChildren<TextMeshProUGUI> ().text = "Jogar";
			} else {
				pauseGameBtn.GetComponentInChildren<TextMeshProUGUI> ().text = "Play";
			}
			pauseScreen.SetActive (true);
		} else {
			Time.timeScale = 1;
			gamePaused = false;
			if (globalscripts.language == "pt") {
				pauseGameBtn.GetComponentInChildren<TextMeshProUGUI> ().text = "Pausar";
			} else {
				pauseGameBtn.GetComponentInChildren<TextMeshProUGUI> ().text = "Pause";
			}
			pauseScreen.SetActive (false);
		}

	}

	public void GameOver(){
		CancelInvoke();
		endScreen.SetActive (true);
		TimeScript time = timeObject.GetComponent<TimeScript> ();
		time.stopTime = true;
		timeObject.SetActive (false);
		changeDeviceBtn.SetActive (false);
		pauseGameBtn.SetActive (false);
		if (globalscripts.language == "pt") {
			goverText.GetComponent<TextMeshProUGUI> ().text += "\n Você sobreviveu:\n"+time.time+" segundos!";
		} else {
			goverText.GetComponent<TextMeshProUGUI> ().text += "\n You survived:\n"+time.time+" seconds!";
		}
		player.GetComponent<Player> ().UnsubscribeMot ();
	}
		
	// Update is called once per frame
	public void SpawnMeteor1() {
			Vector3 spawnPosition = new Vector3 (Random.Range (-(spawnValues.x), spawnValues.x), spawnValues.y, spawnValues.z);
			Quaternion spawnRotation = meteor1.transform.rotation;
			meteor1.transform.position = spawnPosition;
			GameObject clone = Instantiate (meteor1, spawnPosition, spawnRotation, parent);
			Vector2 movement = new Vector2 (0, 1);
			clone.GetComponent<Rigidbody2D>().AddForce (movement * meteorSpeed);
	}

	public void SpawnMeteor2() {
		Vector3 spawnPosition = new Vector3 (Random.Range (-(spawnValues.x), spawnValues.x), spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = meteor2.transform.rotation;
		meteor2.transform.position = spawnPosition;
		GameObject clone = Instantiate (meteor2, spawnPosition, spawnRotation, parent);
		Vector2 movement = new Vector2 (0, 1);
		clone.GetComponent<Rigidbody2D>().AddForce (movement * meteorSpeed);
	}
		
}

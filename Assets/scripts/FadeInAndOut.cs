using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeInAndOut : MonoBehaviour {
	[SerializeField] private float fadePerSecond = 2.5f;
	public bool turnedOff;

	public void FadeOut(){
		var material = GetComponent<TextMeshProUGUI>();
		var color = material.color;
		material.color = new Color(color.r, color.g, color.b, color.a - (fadePerSecond * Time.deltaTime));
	}
	
	public void FadeIn(){
		var material = GetComponent<TextMeshProUGUI>();
		var color = material.color;
		material.color = new Color(color.r, color.g, color.b, color.a + (fadePerSecond * Time.deltaTime));
	}
}

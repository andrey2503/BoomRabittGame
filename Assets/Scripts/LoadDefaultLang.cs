using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDefaultLang : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameControl.instance.LoadLocalizedText("localizedText_es.json");
	}
}

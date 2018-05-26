using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLang : MonoBehaviour {

	// Use this for initialization
	public void changeLang(string fileName) {
		GameControl.instance.LoadLocalizedText(fileName);
		LocalizedText.instance.updateLang();
	}
}

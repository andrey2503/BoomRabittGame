using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSonido : MonoBehaviour {

	public static ControlSonido instance;
	public GameObject musica_ambiente;
	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
			iniciarMusicaAmbiente ();
		} else {
			Destroy (this.gameObject);
		}
	}// fin de start
	
	// Update is called once per frame
	void Update () {
		
	}

	public void pararMusicaAmbiente(){
		musica_ambiente.GetComponent<AudioSource> ().Stop ();
	}

	public void iniciarMusicaAmbiente(){
		musica_ambiente.GetComponent<AudioSource> ().Play();
	}
}

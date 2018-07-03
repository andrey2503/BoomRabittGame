using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSonido : MonoBehaviour {

	public static ControlSonido instance;
	public GameObject musica_ambiente;
	bool musica_sonando=false;
	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
			if (!musica_sonando) {
				iniciarMusicaAmbiente ();
				musica_sonando = true;
			}

		} else {
			Destroy (this.gameObject);
		}
	}// fin de start
	
	// Update is called once per frame
	void Update () {
		
	}

	public void sonido(){
		if (!musica_sonando) {
			iniciarMusicaAmbiente ();
			musica_sonando = true;
		} else {
			pararMusicaAmbiente ();
			musica_sonando = false;
		}// fin del else
	}// fin de sonido

	public void pararMusicaAmbiente(){
		musica_ambiente.GetComponent<AudioSource> ().Stop ();
	}

	public void iniciarMusicaAmbiente(){
		musica_ambiente.GetComponent<AudioSource> ().Play();
	}
}

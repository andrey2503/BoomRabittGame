using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_sonidos : MonoBehaviour {

		
	public static Player_sonidos instance;
	public GameObject sonido_correr;
	public GameObject sonido_saltar;
	public bool corriendo=false;
		private void Start()
		{
			if(instance==null){
				instance = this;
			}else{
				Destroy (this.gameObject);
			}
		}

	public void iniciar_correr(){
		if(!corriendo){
			corriendo = true;
			sonido_correr.GetComponent<AudioSource> ().Play ();
		}

	}// fin de correr
	public void parar_correr(){
		sonido_correr.GetComponent<AudioSource> ().Stop ();
		corriendo = false;
	}// fin de correr

	public void iniciar_salto(){
		sonido_saltar.GetComponent<AudioSource> ().Play ();
		sonido_correr.GetComponent<AudioSource> ().Stop ();
		corriendo = false;
	}// fin de correr
	public void parar_salto(){
		sonido_saltar.GetComponent<AudioSource> ().Stop ();
	}// fin de correr

}

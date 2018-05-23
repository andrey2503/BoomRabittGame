using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_meta : MonoBehaviour {
	public int escena=0;
	public static Control_meta instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}

	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			Debug.Log ("Jugador llego a la meta");
			//Player_Inputs.instance.activo = false;
			//Player_engine.instance.resetPostPlayer ();
			CambioEscena.instance.setScene(escena);
		}
	}// OnTriggerEnter
	

}

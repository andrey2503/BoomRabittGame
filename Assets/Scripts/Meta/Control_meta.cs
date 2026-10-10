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

	private bool metaAlcanzada = false;

	void OnTriggerEnter(Collider meta){
		if (metaAlcanzada) {
			return;   // la meta solo se cruza una vez por nivel
		}
		if(meta.gameObject.tag=="Player"){
			metaAlcanzada = true;
			Debug.Log ("Jugador llego a la meta");
			ControlMenus.instance.activarMenuGano ();
		}
	}// OnTriggerEnter
	

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_zanahorias : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			Debug.Log ("Punto");
			//Player_Inputs.instance.activo = false;
			Destroy(this.gameObject);
			Puntos_control.instance.sumarPunto ();
		}
	}// OnTriggerEnter
}

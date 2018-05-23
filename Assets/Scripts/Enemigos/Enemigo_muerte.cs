using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_muerte : MonoBehaviour {

	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			Player_life.instance.disminuirVida ();
			Destroy (this.gameObject);
		}
	}// OnTriggerEnter
}

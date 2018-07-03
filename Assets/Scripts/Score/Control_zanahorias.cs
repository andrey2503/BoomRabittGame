using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_zanahorias : MonoBehaviour {
	// Use this for initialization
	GameObject objeto;
	void Start () {
		
	}
	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			Debug.Log ("Punto");
			//Player_Inputs.instance.activo = false;
			this.gameObject.GetComponent<AudioSource>().Play();
			Puntos_control.instance.sumarPunto ();
			StartCoroutine (destruirObjeto());
		}
	}// OnTriggerEnter
	IEnumerator destruirObjeto(){
		yield return new WaitForSeconds (0.1f);
			Destroy(this.gameObject);
	}
}

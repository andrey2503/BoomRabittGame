using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuevoPersonaje : MonoBehaviour {

	public GameObject personaje;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(0,10,0),ForceMode.Impulse);
		}
	}

}

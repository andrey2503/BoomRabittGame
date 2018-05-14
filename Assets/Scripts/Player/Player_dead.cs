﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_dead : MonoBehaviour {
	public static Player_dead instance;
	// Use this for initialization
	public GameObject muerte_precipicio;
	void Start () {
		instance = this;
	}

	void Update () {
		muerte_precipicio.GetComponent<Transform> ().position
		= new Vector3(Player_engine.instance.getPosX ()+12,muerte_precipicio.transform.position.y,muerte_precipicio.transform.position.z);
	}// fin de update

	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			Debug.Log ("Jugador ha muerto");
			Player_engine.instance.resetPostPlayer ();
		}
	}// OnTriggerEnter
}

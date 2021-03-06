﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_controller : MonoBehaviour {
	public static Camara_controller instance;
	// Use this for initialization
	public int posy=4;
	public float velocidad=1.0f;
	public float velocidadZoom=1.0f;
	public GameObject Main_Camera;
	public float originaly,originalz;
	void Start () {
		instance = this;
		originaly = Main_Camera.GetComponent<Transform> ().position.y;
		originalz = Main_Camera.transform.position.z;
	}// fin del start


	
	// Update is called once per frame
	void Update () {
		if (Player_engine.instance.getPosY () > 8) {
			Vector3 destino = new Vector3 (Player_engine.instance.getPosX (), Player_engine.instance.getPosY ()+4, -18f);
			Main_Camera.GetComponent<Transform> ().position=Vector3.Lerp (Main_Camera.GetComponent<Transform> ().position,destino,Time.deltaTime*velocidad);
		} else {
			Vector3 destino = new Vector3 (Player_engine.instance.getPosX (),originaly ,originalz);
			Main_Camera.GetComponent<Transform> ().position=Vector3.Lerp (Main_Camera.GetComponent<Transform> ().position,destino,Time.deltaTime*velocidad);
		}
	}// fin de update
}

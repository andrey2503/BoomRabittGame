using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_controller : MonoBehaviour {
	public static Camara_controller instance;
	// Use this for initialization
	public int posy=4;
	public float velocidad=1.0f;
	public GameObject Main_Camera;
	void Start () {
		instance = this;
	}// fin del start


	
	// Update is called once per frame
	void Update () {
		Vector3 destino = new Vector3 (Player_engine.instance.getPosX (), Main_Camera.GetComponent<Transform> ().position.y, Main_Camera.transform.position.z);
		//Main_Camera.GetComponent<Transform> ().position
		//= new Vector3(Player_engine.instance.getPosX (),Main_Camera.GetComponent<Transform>().position.y,Main_Camera.transform.position.z);
		Main_Camera.GetComponent<Transform> ().position=Vector3.Lerp (Main_Camera.GetComponent<Transform> ().position,destino,Time.deltaTime*velocidad);
	}// fin de update
}

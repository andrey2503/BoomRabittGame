using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_engine : MonoBehaviour {
	public static Player_engine instance;
	public GameObject personaje;
	public float salto=1;
	public float mover=1f;
	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoverDerecha(){
		personaje.transform.position = new Vector3 (personaje.transform.position.x+mover,personaje.transform.position.y,personaje.transform.position.z);
	}//fin de mover derecha

	public void MoverIzquierda(){
		personaje.transform.position = new Vector3 (personaje.transform.position.x-mover,personaje.transform.position.y,personaje.transform.position.z);
	}//fin de mover izquierda

	public void Salto(){
		Debug.Log ("salto");
		//personaje.GetComponent<Rigidbody> ().AddForce (personaje.transform.up*salto);
		personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(0,100*salto,0));
	}//fin de salto

	public float getPosX(){
		return personaje.transform.position.x;
	}
}

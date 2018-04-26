using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPersonaje : MonoBehaviour {
	public bool activo = true;
	public static ControlPersonaje instance;
	public GameObject personaje;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(activo){
			if(Input.GetAxis("Horizontal") != 0) {
				float valor=Input.GetAxis("Horizontal");
				personaje.transform.position = new Vector3 (personaje.transform.position.x+valor,personaje.transform.position.y,personaje.transform.position.z);
			}

			if(Input.GetAxis("Vertical") != 0) {
//				ControlPersonaje.instance.MoveVertical(Input.GetAxis("Vertical"));
			}
		}//if activo
	}// fin de update
}

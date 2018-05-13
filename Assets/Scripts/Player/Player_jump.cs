using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_jump : MonoBehaviour {

	public static Player_jump instance;
	// Use this for initialization
	public float distancia_deteccion=1.0f;
	void Start () {
		instance = this;
	}
	void Update(){
		//Debug.DrawLine(transform.position,transform.TransformDirection(Vector3.down*distancia_deteccion),Color.red);
		//Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) *distancia_deteccion, Color.yellow);
		Debug.DrawRay(this.transform.position, new Vector3(0,-1,0) *distancia_deteccion, Color.blue);
		//Debug.Log (hit.distance);
	}
	public bool verificar_suerlo(){
		int layerMask = 1 << 8;

		//RaycastHit hit;
		// Does the ray intersect any objects excluding the player layer
		//Debug.DrawLine(transform.position,new Vector3(0,-1,0) );
		Debug.Log(Physics.Raycast(this.transform.position, new Vector3(0,-1,0) *distancia_deteccion, layerMask));
		if ( Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), distancia_deteccion,  layerMask))
		{
			//Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
			Debug.Log("Did Hit"+transform.position);
			return true;
		}
		else
		{
			//Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
			Debug.Log("Did not Hit");
			return false;
		}



	}// fin de verificar suelo
}

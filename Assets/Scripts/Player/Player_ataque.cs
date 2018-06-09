using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ataque : MonoBehaviour {
	public float distancia_deteccion=1.5f;
	public static Player_ataque instance;
	// Use this for initialization
	void Start () {
		if(Player_ataque.instance == null){
			Player_ataque.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}


	public bool ataque(int direccion){
		int layerMask = 1 << 9;
		//Debug.Log(Physics.Raycast(this.transform.position, new Vector3(0,-1,0) *distancia_deteccion, layerMask));
		Debug.DrawRay(this.transform.position, new Vector3(direccion,0,0) *distancia_deteccion, Color.blue);

		if ( Physics.Raycast(transform.position, new Vector3(direccion,0,0) *distancia_deteccion, distancia_deteccion,  layerMask))
		{
			return true;
		}
		else
		{
			return false;
		}



	}// fin de verificar suelo
}

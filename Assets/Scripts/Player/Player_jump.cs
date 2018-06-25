using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_jump : MonoBehaviour {

	public static Player_jump instance;
	// Use this for initialization
	public float distancia_deteccion=0.55f;
	void Awake(){
		if(Player_jump.instance == null){
			Player_jump.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake
	void Start () {
	}
	void Update(){
		Debug.DrawRay(this.transform.position, new Vector3(0,-1,0) *distancia_deteccion, Color.blue);
		if (!verificar_suerlo ()) {
			Player_engine.instance.personajeEnSalto ();	
		} else {
			Player_engine.instance.personajeEnSuelo ();
		}
	}
	public bool verificar_suerlo(){
		int layerMask = 1 << 8;
		//Debug.Log(Physics.Raycast(this.transform.position, new Vector3(0,-1,0) *distancia_deteccion, layerMask));
		if ( Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), distancia_deteccion,  layerMask))
		{
			return true;
		}
		else
		{
			return false;
		}



	}// fin de verificar suelo
}

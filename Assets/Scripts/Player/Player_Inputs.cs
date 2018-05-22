using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inputs : MonoBehaviour {
	public bool activo = true;
	public static Player_Inputs instance;

	// Use this for initialization

	void Awake(){
		if(Player_Inputs.instance == null){
			Player_Inputs.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}// fin de update

	void FixedUpdate(){
		if(activo){
			if(Input.GetAxis("Horizontal") != 0) {
				float mover=Input.GetAxis("Horizontal");
				if (mover > 0) {
					Player_engine.instance.MoverDerecha ();
				} else {
					Player_engine.instance.MoverIzquierda ();
				}// fin de if izquierda Derecha

			}
			if (Input.GetKeyDown ("space")) {
				//Player_engine.instance.Salto ();
				if(Player_jump.instance.verificar_suerlo ()){
					Player_engine.instance.Salto ();
				}
			}// fin de input space
			//if(Input.GetAxis("Vertical") != 0) {
			//				ControlPersonaje.instance.MoveVertical(Input.GetAxis("Vertical"));
			//}if vertical
		}//if activo
	}//fin de 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inputs : MonoBehaviour {
	public bool activo = true;
	public static Player_Inputs instance;
	public int llamarCorrutina=1;
	private int direccion=0;
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
			if (Input.GetAxis ("Horizontal") != 0) {
				
				float mover = Input.GetAxis ("Horizontal");
				if (mover > 0) {
					direccion = 1;
					Player_engine.instance.MoverDerecha ();
					llamarCorrutinaMovimiento ();
					llamarCorrutina++;
					//ControlFondo.instace.moverEscenarioDerecha ();
				} else if (mover < 0) {
					//limite del mundo
					if(Player_engine.instance.getPosX()>0){
						direccion = -1;
					Player_engine.instance.MoverIzquierda ();
					llamarCorrutinaMovimiento ();
					llamarCorrutina++;
					}
					//ControlFondo.instace.moverEscenarioIzquierda ();
				}// fin de if izquierda Derecha
			} else {
				llamarCorrutina=1;
				Player_engine.instance.personajeDetenido ();
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				//Player_engine.instance.Salto ();
				if (Player_jump.instance.verificar_suerlo ()) {
					Player_engine.instance.Salto ();
				} else {
					Player_engine.instance.activarDobleSalto ();
				}
			}// fin de input space
			if(Input.GetKeyDown("space")){
				Debug.Log( Player_ataque.instance.ataque (direccion));
			}
		}//if activo
		else{
			
		}
	}//fin de fixexUpdate

	public void llamarCorrutinaMovimiento(){
		if(llamarCorrutina==1){
			Player_engine.instance.moviendose ();
		}
			
	}// fin de llamarCorrutinaMovimiento
	public void HabilitarInputs(){
		StartCoroutine (habilitarInputs());
	}
	private IEnumerator habilitarInputs(){
		yield return new WaitForSeconds (0.5f);
		activo = true;
	}// fin de subirVelocidadMovimeinto

	public void inhabilitarInputs(){
		activo = false;
	}
}

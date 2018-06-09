using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_engine : MonoBehaviour {
	public static Player_engine instance;
	public GameObject personaje;
	public float salto=1;
	public float mover=0.50f;
	public bool subirMovimiento = true;
	public bool saltando = false;

	public float jumpDuration = 0.3f;
	public float jumpDistance = 0.2f;

	public int pesoCaida = -1;
	public float velocidadDeMovimiento = 5f;
	// Use this for initialization
	void Awake(){
		if(Player_engine.instance == null){
			Player_engine.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	public float getVelocityPostY (){
		return  personaje.GetComponent<Rigidbody> ().velocity.y;
	}

	public void MoverDerecha(){
		//personaje.transform.position = new Vector3 (personaje.transform.position.x+mover,personaje.transform.position.y,personaje.transform.position.z);
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(velocidadDeMovimiento,getVelocityPostY(), 0f );
	}//fin de mover derecha

	public void MoverIzquierda(){
		//personaje.transform.position = new Vector3 (personaje.transform.position.x-mover,personaje.transform.position.y,personaje.transform.position.z);
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(-velocidadDeMovimiento, getVelocityPostY(), 0f );
	}//fin de mover izquierda

	public void Salto(){
		Vector3 saltoArriba = (transform.up) * jumpDistance;
		Debug.Log (saltoArriba);
		StartCoroutine (nuevoSalto(saltoArriba));
	}//fin de salto

	public Vector3 getPosition(){
		return transform.position;
	}

	private IEnumerator nuevoSalto(Vector3 direccion){
		bool jumping = false;
		jumping = true;
		float time = 0;
		float jumpProgress = 0;
		int contador = 0;
		while(jumping){
			jumpProgress = time / jumpDuration;
			if (jumpProgress > 1) {
				jumping = false;
				jumpProgress = 1;
				Debug.Log ("Add force hacia abajo");
				//personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(0,pesoCaida,0), ForceMode.Impulse);
				personaje.GetComponent<Rigidbody> ().velocity = Vector3.down * pesoCaida;
			} else {
				
			}
			//transform.Translate(Vector3.up*jumpDistance);
			personaje.GetComponent<Rigidbody> ().velocity = Vector3.up * jumpDistance;
			//personaje.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpDistance);
			contador++;
			time += Time.deltaTime;
			yield return null;
		}// fin del while
	}// fin de nuevoSalto

	public float getPosX(){
		return personaje.transform.position.x;
	}// fin de getPosx()
	public float getPosY(){
		return personaje.transform.position.y;
	}
	public float getPosZ(){
		return personaje.transform.position.z;
	}

	public void resetPostPlayer(){
		personaje.transform.position = new Vector3 (0,2,0);
	}
	public void personajeDetenido(){
		this.mover = 0.07f;
		subirMovimiento = false;
		StopCoroutine (SubirVelocidadMovimiento());
	}// fin de personajeDetenido

	public void moviendose(){
			StartCoroutine(SubirVelocidadMovimiento());
	}//
	private IEnumerator SubirVelocidadMovimiento(){
			yield return new WaitForSeconds (0.1f);
			actualizarVelocidad ();

	}// fin de subirVelocidadMovimeinto

	public void personajeEnSalto(){
		subirMovimiento = false;
	}// fin de personajeEnSalto

	public void personajeEnSuelo(){
		subirMovimiento = true;
	}// fin de personajeEnSuelo

	public void actualizarVelocidad(){
		
		if (this.mover < 0.10f) {
			if(subirMovimiento){
			this.mover += 0.02f;
			}
			moviendose ();
			//Debug.Log ("Subiendo velocidad de movimineto");
		} else {
			StopCoroutine (SubirVelocidadMovimiento());
		}
	}// fin de actualizar velocidad


	public void choqueEnemigo(int direccion){
		if (direccion == 1) {
			personaje.GetComponent<Rigidbody> ().AddForce (transform.right * 9, ForceMode.Impulse);
		} else {
			personaje.GetComponent<Rigidbody> ().AddForce (transform.right * -9, ForceMode.Impulse);
		}
		Player_Inputs.instance.HabilitarInputs ();
		}
}

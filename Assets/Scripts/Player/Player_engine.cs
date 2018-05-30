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

	public void MoverDerecha(){
		personaje.transform.position = new Vector3 (personaje.transform.position.x+mover,personaje.transform.position.y,personaje.transform.position.z);
	}//fin de mover derecha

	public void MoverIzquierda(){
		personaje.transform.position = new Vector3 (personaje.transform.position.x-mover,personaje.transform.position.y,personaje.transform.position.z);
	}//fin de mover izquierda

	public void Salto(){
		Debug.Log ("salto");
		personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(0,100*salto,0));
	}//fin de salto

	public float getPosX(){
		return personaje.transform.position.x;
	}// fin de getPosx()
	public float getPosY(){
		return personaje.transform.position.y;
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
		//iniciar corru
		//if(subirMovimiento){
			StartCoroutine(SubirVelocidadMovimiento());
		//}
	}//
	private IEnumerator SubirVelocidadMovimiento(){
			Debug.Log ("corriendo corrutina en valor "+this.mover);
			yield return new WaitForSeconds (0.1f);
			actualizarVelocidad ();

	}// fin de subirVelocidadMovimeinto

	public void personajeEnSalto(){
		Debug.Log ("Detener subirmovimiento por personaje en salto" + this.mover);
		subirMovimiento = false;
	}// fin de personajeEnSalto

	public void personajeEnSuelo(){
		subirMovimiento = true;
	}// fin de personajeEnSuelo

	public void actualizarVelocidad(){
		
		if (this.mover < 0.25f) {
			if(subirMovimiento){
			this.mover += 0.02f;
			}
			moviendose ();
			Debug.Log ("Subiendo velocidad de movimineto");
		} else {
			StopCoroutine (SubirVelocidadMovimiento());
		}
	}// fin de actualizar velocidad

	public void inputMoviendoceDerechaIzquierda(){
		//this.inputMoviendoce = true;
	}
}

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

	public float jumpDuration = 0.5f;
	public float jumpDistance = 3;
	private bool jumping = false;
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
	public float gravity = 10.0f;
	public void Salto(){
		Vector3 saltoArriba = (transform.up) * jumpDistance;
		Debug.Log (saltoArriba);
		StartCoroutine (nuevoSalto(saltoArriba));
	}//fin de salto

	private IEnumerator nuevoSalto(Vector3 direccion){

		jumping = true;
		Vector3 startPoint = transform.position;
		Vector3 targetPoint = startPoint + direccion;
		Debug.Log (startPoint + direccion);
		Debug.Log (direccion);
		Debug.Log (startPoint);
		Debug.Log (targetPoint);
		float time = 0;
		float jumpProgress = 0;
		int contador = 0;
		while(jumping){
			jumpProgress = time / jumpDuration;
			if (jumpProgress > 1)
			{
				jumping = false;
				jumpProgress = 1;
			}
			//Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, jumpProgress);
			transform.Translate(Vector3.up*jumpDistance);
			Debug.Log (contador+"->"+targetPoint);
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
		//iniciar corru
		//if(subirMovimiento){
			StartCoroutine(SubirVelocidadMovimiento());
		//}
	}//
	private IEnumerator SubirVelocidadMovimiento(){
			//Debug.Log ("corriendo corrutina en valor "+this.mover);
			yield return new WaitForSeconds (0.1f);
			actualizarVelocidad ();

	}// fin de subirVelocidadMovimeinto

	public void personajeEnSalto(){
		//Debug.Log ("Detener subirmovimiento por personaje en salto" + this.mover);
		subirMovimiento = false;
	}// fin de personajeEnSalto

	public void personajeEnSuelo(){
		subirMovimiento = true;
	}// fin de personajeEnSuelo

	public void actualizarVelocidad(){
		
		if (this.mover < 0.08f) {
			if(subirMovimiento){
			this.mover += 0.02f;
			}
			moviendose ();
			//Debug.Log ("Subiendo velocidad de movimineto");
		} else {
			StopCoroutine (SubirVelocidadMovimiento());
		}
	}// fin de actualizar velocidad

	public void inputMoviendoceDerechaIzquierda(){
		//this.inputMoviendoce = true;
	}
}

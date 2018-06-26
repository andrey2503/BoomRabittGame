using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_engine : MonoBehaviour {
	public static Player_engine instance;
	public GameObject personaje;
	public float salto=1;
	public float mover=0.50f; 
	public bool subirMovimiento = true;
	public float velocidadDeMovimiento = 5f;
	public int dobleSalto=1;
    public Animator anim;
    public GameObject animatedChar;


	//estados de animacion
	bool personajeMoviendoce=false;
	public bool personajeSaltando = false;


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
	
	public float getVelocityPostY (){
		return  personaje.GetComponent<Rigidbody> ().velocity.y;
	}

	public void MoverDerecha(){
		//personaje.transform.position = new Vector3 (personaje.transform.position.x+mover,personaje.transform.position.y,personaje.transform.position.z);
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(mover,getVelocityPostY(), 0f );
        animatedChar.transform.localScale = new Vector3(1,1,1);
		/*if (personajeSaltando) {
			anim.SetInteger("estado",4);
		} else {
			anim.SetInteger("estado",1);
		}
		*/
		personajeMoviendoce = true;

    }//fin de mover derecha

	public void MoverIzquierda(){
		//personaje.transform.position = new Vector3 (personaje.transform.position.x-mover,personaje.transform.position.y,personaje.transform.position.z);
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(-mover, getVelocityPostY(), 0f );
       // anim.SetInteger("estado", 1);

        animatedChar.transform.localScale = new Vector3(1, 1, -1);
		/*if (personajeSaltando) {
			anim.SetInteger("estado",4);
		} else {
			anim.SetInteger("estado",1);
		}
		*/
		personajeMoviendoce = true;
    }//fin de mover izquierda

	public void Salto(){
		anim.SetInteger("estado",2);
		StartCoroutine (tiempoDobleSalto());
		nuevoSalto ();
	}//fin de salto
	public float tiempoActivarDobleSalto=0.5f;
	IEnumerator tiempoDobleSalto(){
		yield return new WaitForSeconds (tiempoActivarDobleSalto);
		dobleSalto = 0;

	}

	public Vector3 getPosition(){
		return transform.position;
	}
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
		this.mover = 2f;
		subirMovimiento = false;
		StopCoroutine (SubirVelocidadMovimiento());
		personajeMoviendoce = false;
		//if (saltando) {
		//	anim.SetInteger ("estado", 0);
		//} 
    }// fin de personajeDetenido

	public void moviendose(){
			StartCoroutine(SubirVelocidadMovimiento());
	}//
	private IEnumerator SubirVelocidadMovimiento(){
			yield return new WaitForSeconds (0f);
			actualizarVelocidad ();

	}// fin de subirVelocidadMovimeinto

	public void personajeEnSalto(){
		subirMovimiento = false;
	}// fin de personajeEnSalto

	public void personajeEnSuelo(){
		subirMovimiento = true;
		dobleSalto = 1;

		if (!personajeMoviendoce && !personajeSaltando) {
			anim.SetInteger ("estado", 0);
		} 
		if(personajeMoviendoce && !personajeSaltando) {
			anim.SetInteger ("estado", 1);
		} 
		if(personajeSaltando){
			anim.SetInteger("estado",2);
		}
		if(personajeMoviendoce && personajeSaltando){
			anim.SetInteger("estado",2);
		}
		Debug.Log ("cambiar animacion porque el personaje toco suelo");
	}// fin de personajeEnSuelo

	public void actualizarVelocidad(){
		
		if (this.mover < velocidadDeMovimiento) {
			if(subirMovimiento){
			this.mover += 1f;
			}
			moviendose ();
		} else {
			StopCoroutine (SubirVelocidadMovimiento());
		}
	}// fin de actualizar velocidad

	public float fuerzaRebote=20f;
	public void choqueEnemigo(int direccion){
		if (direccion == 1) {
			personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(fuerzaRebote,0,0),ForceMode.Impulse);
		} else {
			personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(-fuerzaRebote,0,0),ForceMode.Impulse);
		}
		Player_Inputs.instance.HabilitarInputs ();
		}// fin de choque enemigo

	public float fuerzaDetenerSubido=-5f;
	public float velocidadDetenerSubida=0.5f;
	public IEnumerator fuerzaDetenida(){
		yield return new WaitForSeconds (velocidadDetenerSubida);
		personaje.GetComponent<Rigidbody> ().AddForce (new Vector2(0,fuerzaDetenerSubido), ForceMode.Impulse);
		StartCoroutine (fuerzaCaida());
	}// fin de fuerzaDetenida

	public float velidadCaida=0.5f;
	public float fuerzaCaidas = -5;
	public IEnumerator fuerzaCaida(){
		yield return new WaitForSeconds (velidadCaida);
		personaje.GetComponent<Rigidbody> ().AddForce (new Vector2(0,fuerzaCaidas), ForceMode.Impulse);
		personajeSaltando = false;
	}

	public float nuevosalto=10f;
	private void nuevoSalto(){
		personajeSaltando = true;
		//anim.SetInteger("estado",2);
		personaje.GetComponent<Rigidbody>().AddForce(new Vector2(0,nuevosalto),ForceMode.Impulse);
		StartCoroutine (fuerzaDetenida());
	}// fin de nuevoSalto

	public float fuerzaDobleSalto=2f;
	public void activarDobleSalto(){
		if(dobleSalto==0){
			dobleSalto++;
			personaje.GetComponent<Rigidbody>().AddForce(new Vector2(0,fuerzaDobleSalto),ForceMode.Impulse);
			StartCoroutine (fuerzaDetenida());
		}
	}

	public void activarAtaque(){
		anim.SetInteger("estado",6);
	}//

}// fin de la clase

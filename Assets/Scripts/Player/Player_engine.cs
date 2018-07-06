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
	public bool personajeSaltando2	= false;
	bool personajeAtacando= false;


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
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(mover,getVelocityPostY(), 0f );
        animatedChar.transform.localScale = new Vector3(1,1,1);
		personajeMoviendoce = true;
		if (!personajeSaltando2) {
			Player_sonidos.instance.iniciar_correr ();
		} else {
			Player_sonidos.instance.parar_correr ();
		}
    }//fin de mover derecha

	public void MoverIzquierda(){
		personaje.GetComponent<Rigidbody> ().velocity = new Vector3(-mover, getVelocityPostY(), 0f );
        animatedChar.transform.localScale = new Vector3(1, 1, -1);
		personajeMoviendoce = true;

		if (!personajeSaltando2) {
			Player_sonidos.instance.iniciar_correr ();
		} else {
			Player_sonidos.instance.parar_correr ();
		}
    }//fin de mover izquierda

	public void Salto(){
		anim.SetInteger("estado",2);
		StartCoroutine (tiempoDobleSalto());
		nuevoSalto ();
		Player_sonidos.instance.iniciar_salto ();
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
		Player_sonidos.instance.parar_correr ();
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
		//personajeSaltando = true;
		personajeSaltando2 = false;
		if (!personajeMoviendoce && !personajeSaltando && !personajeAtacando) {
			anim.SetInteger ("estado", 0);
		} 
		if(personajeMoviendoce && !personajeSaltando && !personajeAtacando) {
			anim.SetInteger ("estado", 1);
		} 
		if(personajeSaltando){
			anim.SetInteger("estado",2);
		}
		if(personajeMoviendoce && personajeSaltando && !personajeAtacando){
			anim.SetInteger("estado",2);
		}
		if(personajeAtacando){
			anim.SetInteger("estado",6);
			StartCoroutine (finalizarEstadoAtaque (1f));
		}
		if(personajeAtacando && personajeMoviendoce){
			anim.SetInteger("estado",6);
			StartCoroutine (finalizarEstadoAtaque (3f));
		}
	}// fin de personajeEnSuelo

	IEnumerator finalizarEstadoAtaque(float tiempo){
		yield return new WaitForSeconds (tiempo);
		personajeAtacando = false;
	}

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
		personajeSaltando = false;
		//personajeSaltando2= false;
	}// fin de fuerzaDetenida

	public float velidadCaida=0.5f;
	public float fuerzaCaidas = -5;
	public IEnumerator fuerzaCaida(){
		yield return new WaitForSeconds (velidadCaida);
		personaje.GetComponent<Rigidbody> ().AddForce (new Vector2(0,fuerzaCaidas), ForceMode.Impulse);
		personajeSaltando = false;
		//personajeSaltando2 = false;
	}

	public float nuevosalto=10f;
	private void nuevoSalto(){
		personajeSaltando = true;
		personajeSaltando2 = true;
		Player_sonidos.instance.iniciar_salto();
		personaje.GetComponent<Rigidbody>().AddForce(new Vector2(0,nuevosalto),ForceMode.Impulse);
		StartCoroutine (fuerzaDetenida());
	}// fin de nuevoSalto

	public float fuerzaDobleSalto=2f;
	public void activarDobleSalto(){
		if(dobleSalto==0){
			//personajeSaltando = true;
			//personajeSaltando2 = true;
			dobleSalto++;
			Player_sonidos.instance.iniciar_salto();
			personaje.GetComponent<Rigidbody>().AddForce(new Vector2(0,fuerzaDobleSalto),ForceMode.Impulse);
			StartCoroutine (fuerzaDetenida());
		}
	}

	public void activarAtaque(){
		personajeAtacando = true;

	}//

	public bool estadoAtaque(){
		return personajeAtacando;
	}// fin de estado Ataque

}// fin de la clase

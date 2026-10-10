using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_comportamiento : MonoBehaviour {
	bool atacando=false;
	int direccion=2;
	int direccion2=1;
	int velocidadMovimiento=1;
	//bool detenerEnemigo=false;
	// Use this for initialization

	public Animator anim;

	[Header ("Muerte")]
	public float duracionMuerte = 0.45f;
	public float alturaRebote = 0.6f;
	public float giroMuerte = 720f;
	private bool muerto = false;

	void Start () {

	}

	// Update is called once per frame
	void Update () {

			// sin esto la patrulla seguiria corriendo durante la muerte y pisaria
			// la posicion y la escala que anima la corrutina
			if (muerto) {
				return;
			}

				if (direccion == 1) {
					EnemigoPatrullando (1);
			anim.SetInteger ("estado_maquina",0);
				}
				if (direccion == 2) {
					EnemigoPatrullando (2);
			anim.SetInteger ("estado_maquina",0);
				}
				if (!atacando) {
					velocidadMovimiento=1;
				}
				else {
					atacandoPersonje ();
			anim.SetInteger ("estado_maquina",1);
				}
	}// fin del update

	public void atacandoPersonje (){
		//Debug.Log ("el personje esta cerca,atacar");
		velocidadMovimiento = 2;
	}// fin de atacando personaje

	public void muerteEnemigo(int direccionActual,int ladopersonaje){
		if (ladopersonaje == direccionActual) {
			direccion = ladopersonaje;
			atacando = true;
			atacandoPersonje ();
		} else {
			atacando = false;
		}
	}

	public void buscandoJugador(){
		float distancia;
		Vector3 transformpersonaje = new Vector3(Player_engine.instance.getPosX(),Player_engine.instance.getPosY(),Player_engine.instance.getPosZ());
		distancia = Vector3.Distance (transformpersonaje,transform.position);
		if (distancia < 3) {
			Vector3 v3 = transformpersonaje - transform.position;
			if (v3.x > 0.1) {
				//Debug.Log ("personaje esta a la derecha y la direccion es "+ direccion);
				muerteEnemigo (direccion,1);

				//velocidadMovimiento = 3;
			} else {
				//Debug.Log ("personaje esta a la izquierda y la direccion es "+ direccion);
				muerteEnemigo (direccion,2);
				//velocidadMovimiento = 3;
			}

		} else {
			atacando = false;
			//detenerEnemigo = false;
		}
	}// fin de buscandoJugador

	public void EnemigoPatrullando(int direccion){
		buscandoJugador ();
		if (direccion == 1) {
			transform.position = new Vector3(transform.position.x+(velocidadMovimiento*Time.deltaTime),transform.position.y,transform.position.z );
			transform.localScale = new Vector3 (50,50,-50);
			direccion2 = 2;
		} else {
			transform.position = new Vector3(transform.position.x-(velocidadMovimiento*Time.deltaTime),transform.position.y,transform.position.z );
			transform.localScale = new Vector3 (50,50,50);
			direccion2 = 1;
		}
	}// fin de enemigo buscando
		
	void OnCollisionEnter(Collision meta){
		if (muerto) {
			return;
		}
		if(meta.gameObject.tag=="Player"){
			if (atacando) {
				Player_Inputs.instance.activo = false;
				Player_life.instance.disminuirVida ();
				Player_engine.instance.choqueEnemigo (direccion);
			} else {
				if (Player_engine.instance.estadoAtaque ()) {
					matarEnemigo ();
				} else {
					Player_Inputs.instance.activo = false;
					Player_life.instance.disminuirVida ();
					Player_engine.instance.choqueEnemigo (direccion2);
				}
			}
		}
		if(meta.gameObject.tag=="suelo"){
			//detenerEnemigo = true;
			if (direccion == 1) {
				direccion = 2;
				direccion2 = 1;
			} else {
				direccion = 1;
				direccion2 = 2;
			}
		}
	}// OnTriggerEnter

	// Antes aqui solo se desactivaba el BoxCollider y el Destroy estaba comentado:
	// el Rigidbody seguia con gravedad y sin collider, asi que el enemigo se hundia
	// atravesando el suelo mientras la patrulla lo seguia moviendo de lado.
	public void matarEnemigo(){
		if (muerto) {
			return;
		}
		muerto = true;

		// que deje de colisionar y de caer
		foreach (Collider collider in GetComponentsInChildren<Collider> ()) {
			collider.enabled = false;
		}
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
		}
		// congelamos la pose para que la animacion no pelee con la corrutina
		if (anim != null) {
			anim.enabled = false;
		}
		AudioSource sonido = GetComponent<AudioSource> ();
		if (sonido != null) {
			sonido.Play ();
		}
		StartCoroutine (animacionMuerte ());
	}// fin de matarEnemigo

	// Rebote corto hacia arriba, giro y encogimiento hasta desaparecer.
	private IEnumerator animacionMuerte(){
		Vector3 escalaInicial = transform.localScale;
		Vector3 posicionInicial = transform.position;
		float tiempo = 0f;
		while (tiempo < duracionMuerte) {
			tiempo += Time.deltaTime;
			float k = Mathf.Clamp01 (tiempo / duracionMuerte);
			transform.position = posicionInicial + new Vector3 (0f, Mathf.Sin (k * Mathf.PI) * alturaRebote, 0f);
			// k*k mantiene el tamano un instante y luego lo colapsa rapido
			transform.localScale = escalaInicial * (1f - k * k);
			transform.Rotate (0f, giroMuerte * Time.deltaTime, 0f, Space.Self);
			yield return null;
		}
		Destroy (gameObject);
	}// fin de animacionMuerte

	public int getDireccion(){
		return direccion;
	}//S


}

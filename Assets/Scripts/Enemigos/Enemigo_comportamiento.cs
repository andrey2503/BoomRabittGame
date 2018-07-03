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
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

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

	public void muerteEnemigo(int direccion,int ladopersonaje){
		if (ladopersonaje == direccion) {
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
		if(meta.gameObject.tag=="Player"){
			if (atacando) {
				Player_Inputs.instance.activo = false;
				Player_life.instance.disminuirVida ();
				Player_engine.instance.choqueEnemigo (direccion);
			} else {
				if (Player_engine.instance.estadoAtaque ()) {
					this.gameObject.GetComponent<BoxCollider> ().enabled = false;
					//this.gameObject.GetComponent<Rigidbody> (). = false;
					//Destroy (this.gameObject);
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

	public int getDireccion(){
		return direccion;
	}//S


}

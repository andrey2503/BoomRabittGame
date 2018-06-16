﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_comportamiento : MonoBehaviour {
	bool atacando=false;
	int direccion=2;
	int velocidadMovimiento=1;
	//bool detenerEnemigo=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

				if (direccion == 1) {
					EnemigoPatrullando (1);
				}
				if (direccion == 2) {
					EnemigoPatrullando (2);
				}
				if (!atacando) {
					velocidadMovimiento=1;
				}
				else {
					atacandoPersonje ();
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
		} else {
			transform.position = new Vector3(transform.position.x-(velocidadMovimiento*Time.deltaTime),transform.position.y,transform.position.z );
		}
	}// fin de enemigo buscando
		
	void OnCollisionEnter(Collision meta){
		if(meta.gameObject.tag=="Player"){
			if (atacando) {
				Player_Inputs.instance.activo = false;
				Player_life.instance.disminuirVida ();
				Player_engine.instance.choqueEnemigo (direccion);
			} else {
				Destroy (this.gameObject);
			}
		}
		if(meta.gameObject.tag=="suelo"){
			//detenerEnemigo = true;
			if (direccion == 1) {
				direccion = 2;
			} else {
				direccion = 1;
			}
		}
	}// OnTriggerEnter

	public int getDireccion(){
		return direccion;
	}//S


}
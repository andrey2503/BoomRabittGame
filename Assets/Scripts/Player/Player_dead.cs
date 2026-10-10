using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_dead : MonoBehaviour {
	public static Player_dead instance;
	// Use this for initialization
	public GameObject muerte_precipicio;
	void Awake(){
		if(Player_dead.instance == null){
			Player_dead.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake
	void Start () {
	}

	void Update () {
		muerte_precipicio.GetComponent<Transform> ().position
		= new Vector3(Player_engine.instance.getPosX ()+12,muerte_precipicio.transform.position.y,muerte_precipicio.transform.position.z);
	}// fin de update

	// evita que el trigger dispare dos veces mientras el jugador reaparece
	public float tiempoEntreCaidas = 1f;
	private float tiempoUltimaCaida = -999f;

	void OnTriggerEnter(Collider meta){
		if(meta.gameObject.tag=="Player"){
			if (Time.time - tiempoUltimaCaida < tiempoEntreCaidas) {
				return;
			}
			tiempoUltimaCaida = Time.time;
			// Antes esto mandaba al inicio del nivel y sacaba el Game Over de una.
			// Ahora cuesta vida y reaparece en el ultimo suelo pisado; solo es
			// Game Over si la vida llega a cero.
			Player_life.instance.caidaAlVacio ();
		}
	}// OnTriggerEnter
}

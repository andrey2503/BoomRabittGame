using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puntos_control : MonoBehaviour {

	public static Puntos_control instance;
	public int puntos=0;
	// Use this for initialization
	public Text texto_puntos;
	void Start () {
	}

	void Awake(){
		if(Puntos_control.instance == null){
			Puntos_control.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake

	public void sumarPunto(){
		puntos++;
		texto_puntos.text = "Puntos: "+puntos;
		Debug.Log (puntos);
	}

	public int getPuntos(){
		return puntos;
	}
}

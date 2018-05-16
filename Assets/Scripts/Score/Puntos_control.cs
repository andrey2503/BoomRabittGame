using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puntos_control : MonoBehaviour {

	public static Puntos_control instance;
	public int puntos=0;
	// Use this for initialization
	void Start () {
		instance = this; 
	}

	public void sumarPunto(){
		puntos++;
		Debug.Log (puntos);
	}
}

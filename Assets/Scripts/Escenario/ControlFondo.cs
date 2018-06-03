using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFondo : MonoBehaviour {
	public static ControlFondo instace;
	public float velocidad=0.00f;
	// Use this for initialization
	void Start () {
		if (instace == null) {
			instace = this;
		} else {
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (Time.time*velocidad,0);
	}

	public void moverEscenarioDerecha(){
		velocidad = 0.01f;
	}

	public void moverEscenarioIzquierda(){
		velocidad = -0.01f;
	}
}

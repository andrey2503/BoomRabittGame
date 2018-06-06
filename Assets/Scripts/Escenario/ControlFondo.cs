using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFondo : MonoBehaviour {
	public static ControlFondo instace;
	public float velocidad1=0.00f;
	public float velocidad2=0.00f;
	public float velocidad3=0.00f;
	public GameObject []fondos;
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
		fondos[0].GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (Time.time*velocidad1,0);
		fondos[1].GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (Time.time*velocidad2,0);
		fondos[2].GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (Time.time*velocidad3,0);

	}

	public void moverEscenarioDerecha(){
		velocidad1= 0.01f;
	}

	public void moverEscenarioIzquierda(){
		velocidad2 = -0.01f;
	}
}

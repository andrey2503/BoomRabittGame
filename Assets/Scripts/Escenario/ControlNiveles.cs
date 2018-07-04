using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlNiveles : MonoBehaviour {
	public GameObject menuPrincipal;
	public GameObject menuNiveles;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void niveles(){
		menuPrincipal.SetActive (false);
		menuNiveles.SetActive (true);
	}

	public void regresarMenu(){
		menuPrincipal.SetActive (true);
		menuNiveles.SetActive (false);
	}

	public void cargarNivel (int nivel){
		CambioEscena.instance.setScene (nivel);
	}//
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour {
	public static CambioEscena instance;
	public int indiceEscena =  0 ;
	// public bool timer = false;
	public bool timer = true;
	public float tiempoEspera = 3f;

	void Awake(){
		//if(CambioEscena.instance==null){
			CambioEscena.instance = this;
		//}else{
		//	Destroy (this.gameObject);
		//}
	}
	// Use this for initialization

	// Update is called once per frame
	void Update () {
		if(timer){
			StartCoroutine (SceneSwitcher ());
		}
	}

	public void CambiarEscenaManual(){
		SceneManager.LoadScene (indiceEscena);
	}

	public void PlayAgain(int repetirescena){
		SceneManager.LoadScene (repetirescena);
	}

	public void BackToMenu(int escenaMenu){
			Debug.Log ("Cambio de scene menu"+indiceEscena);
		SceneManager.LoadScene (escenaMenu);
	}

	public void menuPrincipal(){
		SceneManager.LoadScene (2);
	}// fin de menuPrincipal

	private IEnumerator SceneSwitcher(){
		yield return new WaitForSeconds (tiempoEspera);
		Debug.Log ("Cambio de scene"+indiceEscena);
		SceneManager.LoadScene (indiceEscena);
	}

	public void setScene(int escena){
		this.indiceEscena = escena;
		CambiarEscenaManual ();
	}

	public void SalirJuego(){
		Application.Quit();
	}//salir del juego
}

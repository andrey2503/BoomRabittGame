using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour {

	public int indiceEscena =  0 ;
	// public bool timer = false;
	public bool timer = true;	
	public float tiempoEspera = 3f;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(timer){
			StartCoroutine (SceneSwitcher ());
		}
	}

	public void CambiarEscenaManual(){
		SceneManager.LoadScene (indiceEscena);
	}

	private IEnumerator SceneSwitcher(){
		yield return new WaitForSeconds (tiempoEspera);
		Debug.Log ("Cambio de scene");
		SceneManager.LoadScene (indiceEscena);
	}
}

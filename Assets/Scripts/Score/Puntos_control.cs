using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puntos_control : MonoBehaviour {

	public static Puntos_control instance;
	public int puntos=0;
	// Use this for initialization
	public Text texto_puntos;
	public Color colorDestacado = new Color(1f, 0.85f, 0.2f);
	public float duracionPunch = 0.25f;
	public float escalaPunch = 1.4f;
	private Color colorOriginal;
	private Coroutine punchRoutine;
	// El aspecto del contador lo arma HUDProfesional (ya corrio para cuando
	// llega este Start), asi que aqui tomamos el color definitivo como base
	// del destello y dejamos el marcador inicializado.
	void Start () {
		colorOriginal = texto_puntos.color;
		texto_puntos.text = puntos.ToString ();
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
		// el icono de zanahoria del HUD ya da el contexto: basta el numero
		texto_puntos.text = puntos.ToString ();
		if (punchRoutine != null) {
			StopCoroutine (punchRoutine);
		}
		punchRoutine = StartCoroutine (PunchPuntosRoutine ());
		Debug.Log (puntos);
	}

	private IEnumerator PunchPuntosRoutine(){
		Transform t = texto_puntos.transform;
		t.localScale = Vector3.one;
		texto_puntos.color = colorDestacado;
		float tiempo = 0f;
		float mitad = duracionPunch * 0.5f;
		while (tiempo < mitad) {
			tiempo += Time.deltaTime;
			float k = Mathf.Clamp01 (tiempo / mitad);
			t.localScale = Vector3.Lerp (Vector3.one, Vector3.one * escalaPunch, k);
			yield return null;
		}
		tiempo = 0f;
		while (tiempo < mitad) {
			tiempo += Time.deltaTime;
			float k = Mathf.Clamp01 (tiempo / mitad);
			t.localScale = Vector3.Lerp (Vector3.one * escalaPunch, Vector3.one, k);
			texto_puntos.color = Color.Lerp (colorDestacado, colorOriginal, k);
			yield return null;
		}
		t.localScale = Vector3.one;
		texto_puntos.color = colorOriginal;
		punchRoutine = null;
	}// fin de PunchPuntosRoutine

	public int getPuntos(){
		return puntos;
	}
}

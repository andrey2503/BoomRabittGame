using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_life : MonoBehaviour {
	public static Player_life instance;
	public int vida = 100;
	public Slider slideVida;
	// lo asigna HUDProfesional al construir el HUD (es el Image "Fill" del slider)
	public Image fillVida;
	public Color colorVidaAlta = new Color(0.25f, 0.85f, 0.35f);
	public Color colorVidaMedia = new Color(0.95f, 0.8f, 0.15f);
	public Color colorVidaBaja = new Color(0.9f, 0.2f, 0.2f);
	public float duracionAnimacionVida = 0.35f;
	public float umbralCritico = 0.25f;
	private Coroutine animacionVidaRoutine;
	private Coroutine pulsoCriticoRoutine;
	private bool enEstadoCritico = false;

	void Awake(){
		if(Player_life.instance == null){
			Player_life.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else
		Debug.Log("Inicio de sistema");

	}// fin del Awake

	// El aspecto de la barra (sprites, posicion, quitar el handle) lo arma
	// HUDProfesional; aqui solo queda la logica de vida y su animacion.

	public void disminuirVida(){
		this.vida=this.vida-25;
		animarSlideVida ();
		if(this.vida<=0){
			ControlMenus.instance.activarMenuRestart ();
		}
	}// fin de dismiuir Vida

	private void animarSlideVida(){
		if (animacionVidaRoutine != null) {
			StopCoroutine (animacionVidaRoutine);
		}
		animacionVidaRoutine = StartCoroutine (AnimarSlideVidaRoutine ());
	}// fin de animarSlideVida

	private IEnumerator AnimarSlideVidaRoutine(){
		float valorInicial = slideVida.value;
		float valorFinal = Mathf.Clamp (this.vida, 0, slideVida.maxValue);
		float tiempo = 0f;
		while (tiempo < duracionAnimacionVida) {
			tiempo += Time.deltaTime;
			float t = Mathf.Clamp01 (tiempo / duracionAnimacionVida);
			slideVida.value = Mathf.Lerp (valorInicial, valorFinal, t);
			actualizarColorVida ();
			yield return null;
		}
		slideVida.value = valorFinal;
		actualizarColorVida ();
		animacionVidaRoutine = null;
		actualizarPulsoCritico ();
	}// fin de AnimarSlideVidaRoutine

	private void actualizarPulsoCritico(){
		bool critico = (slideVida.value / slideVida.maxValue) <= umbralCritico && slideVida.value > 0f;
		if (critico == enEstadoCritico) {
			return;
		}
		enEstadoCritico = critico;
		if (pulsoCriticoRoutine != null) {
			StopCoroutine (pulsoCriticoRoutine);
			pulsoCriticoRoutine = null;
		}
		if (critico) {
			pulsoCriticoRoutine = StartCoroutine (PulsoCriticoRoutine ());
		} else {
			pulsoCritico = 1f;
			actualizarColorVida ();
		}
	}// fin de actualizarPulsoCritico

	// Parpadeo de alerta por brillo en vez de por escala: la barra vive dentro del
	// panel del HUD, asi que escalarla correria el resto de los elementos.
	private float pulsoCritico = 1f;
	private IEnumerator PulsoCriticoRoutine(){
		while (enEstadoCritico) {
			float t = (Mathf.Sin (Time.time * 7f) + 1f) * 0.5f;
			pulsoCritico = Mathf.Lerp (1f, 1.75f, t);
			actualizarColorVida ();
			yield return null;
		}
		pulsoCritico = 1f;
		actualizarColorVida ();
	}// fin de PulsoCriticoRoutine

	private void actualizarColorVida(){
		if (fillVida == null) {
			return;
		}
		float porcentaje = slideVida.value / slideVida.maxValue;
		Color color;
		if (porcentaje > 0.5f) {
			color = Color.Lerp (colorVidaMedia, colorVidaAlta, (porcentaje - 0.5f) / 0.5f);
		} else {
			color = Color.Lerp (colorVidaBaja, colorVidaMedia, porcentaje / 0.5f);
		}
		color = new Color (color.r * pulsoCritico, color.g * pulsoCritico, color.b * pulsoCritico, 1f);
		fillVida.color = color;
	}// fin de actualizarColorVida

	public void muertePrecipicio(){
		
	}// fin de muerte precipio








}

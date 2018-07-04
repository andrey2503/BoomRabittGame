using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlMenus : MonoBehaviour {

	public static ControlMenus instance;
	public GameObject menuGameOver;
	public GameObject menuPausa;
	public GameObject menuGano;

	public Text texto_puntos_menu_perdida;
	public Text texto_puntos_menu_ganado;

	public int nivelRestart = 0;
	public int nivelSiguiente = 0;

	// Use this for initialization
	void Start () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this.gameObject);
		}
	}

	public void activarMenuRestart(){
		texto_puntos_menu_perdida.text = Puntos_control.instance.getPuntos ()+"";
		menuGameOver.SetActive (true);
		Player_Inputs.instance.inhabilitarInputs ();
	}

	public void iniciarSiguienteNivel (){
		//yield return new WaitForSeconds (3f);
		CambioEscena.instance.setScene (nivelSiguiente);
	}

	public void activarMenuPausa(){
		if (menuPausa.activeSelf) {
			menuPausa.SetActive (false);
			Player_Inputs.instance.HabilitarInputs ();
		} else {
			menuPausa.SetActive (true);
			Player_Inputs.instance.inhabilitarInputs ();
		}
	}//

	public void reiniciarNivel(){
		CambioEscena.instance.setScene (nivelRestart);
	}//

	public void activarMenuGano(){
		texto_puntos_menu_ganado.text = Puntos_control.instance.getPuntos ()+"";
		menuGano.SetActive (true);
		//StartCoroutine (iniciarSiguienteNivel());
	}//

	public void menuprincipal(){
		CambioEscena.instance.menuPrincipal();
	}//



}

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

	// En Awake y no en Start: otros sistemas (Player_dead, el armado del menu de
	// victoria) consultan ControlMenus.instance antes de que corran los Start.
	void Awake () {
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
		if (menuGano.activeSelf) {
			return;
		}
		texto_puntos_menu_ganado.text = Puntos_control.instance.getPuntos ()+"";
		menuGano.SetActive (true);
		// Faltaba: sin esto el jugador seguia controlando al conejo detras de la
		// pantalla de victoria, y hasta podia caerse y sacar el Game Over encima.
		Player_Inputs.instance.inhabilitarInputs ();
	}//

	public void menuprincipal(){
		CambioEscena.instance.menuPrincipal();
	}//



}

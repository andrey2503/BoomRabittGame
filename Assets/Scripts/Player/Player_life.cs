using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_life : MonoBehaviour {
	public static Player_life instance;
	public int vida = 100;
	public Slider slideVida;
	void Awake(){
		if(Player_life.instance == null){
			Player_life.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake

	public void disminuirVida(){
		this.vida=this.vida-25;
		slideVida.value = this.vida;
		Debug.Log (this.vida);
	}// fin de dismiuir Vida

	public void muertePrecipicio(){
		
	}// fin de muerte precipio








}

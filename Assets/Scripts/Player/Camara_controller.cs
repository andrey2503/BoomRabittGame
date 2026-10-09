using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara_controller : MonoBehaviour {
	public static Camara_controller instance;
	// Use this for initialization
	public int posy=4;
	public float velocidad=1.0f;
	public float velocidadZoom=1.0f;
	public GameObject Main_Camera;
	public float originaly,originalz;
	// altura a la que empieza y termina la transicion hacia el modo "alto" (evita el salto brusco de destino al cruzar un unico umbral)
	public float alturaInicioTransicion = 6f;
	public float alturaFinTransicion = 10f;
	void Start () {
		instance = this;
		originaly = Main_Camera.GetComponent<Transform> ().position.y;
		originalz = Main_Camera.transform.position.z;
	}// fin del start



	// Update is called once per frame
	void Update () {
		Transform camTransform = Main_Camera.GetComponent<Transform> ();
		Vector3 posActual = camTransform.position;
		float posY = Player_engine.instance.getPosY ();
		float t = Mathf.Clamp01 (Mathf.InverseLerp (alturaInicioTransicion, alturaFinTransicion, posY));
		float targetZ = Mathf.Lerp (originalz, -18f, t);
		float targetY = Mathf.Lerp (originaly, posY+4, t);

		float nuevaX = Mathf.Lerp (posActual.x, Player_engine.instance.getPosX (), Time.deltaTime*velocidad);
		float nuevaY = Mathf.Lerp (posActual.y, targetY, Time.deltaTime*velocidad);
		float nuevaZ = Mathf.Lerp (posActual.z, targetZ, Time.deltaTime*velocidadZoom);

		camTransform.position = new Vector3 (nuevaX, nuevaY, nuevaZ);
	}// fin de update
}

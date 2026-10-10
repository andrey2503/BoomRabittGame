using UnityEngine;

// Seguimiento de camara para plataformas 2.5D.
//
// Hay DOS componentes de estos por escena, uno por camara del stack:
//   - "Charac Camera": la camara raiz, que es la que sigue al jugador.
//   - "Main Camera (1)": dibuja el fondo y es HIJA de la anterior, por lo que ya
//     hereda su posicion y rotacion. Esa solo aplica el desfase de parallax.
public class Camara_controller : MonoBehaviour {

	public static Camara_controller instance;
	public GameObject Main_Camera;

	[Header ("Suavizado (segundos aprox. en alcanzar al jugador)")]
	public float suavizadoX = 0.16f;
	public float suavizadoY = 0.30f;

	[Header ("Adelanto en la direccion de avance")]
	public float adelanto = 2.2f;              // cuanto se adelanta la camara al correr
	public float suavizadoAdelanto = 0.45f;
	public float velocidadMinimaAdelanto = 0.6f;

	[Header ("Margen vertical antes de seguir al jugador en el aire")]
	public float margenSuperior = 4.5f;
	public float margenInferior = 3.5f;

	// 1 = sigue al jugador 1:1. Valores menores hacen que esta camara se mueva
	// mas lento que el jugador, que es como se consigue el efecto de parallax
	// en la camara de fondo.
	[Header ("Parallax")]
	[Range (0f, 1f)] public float factorParallax = 1f;

	// se conservan porque las escenas ya los traen serializados
	public float originaly, originalz;

	private Transform camara;
	private float offsetX, offsetY;
	private float alturaBase;
	private float velX, velY;
	private float adelantoActual, velAdelanto;

	// La camara de fondo del stack es HIJA de la camara de juego (mismo transform
	// local cero), de modo que ya hereda su posicion y rotacion. Para esa no hay
	// que calcular un seguimiento propio: basta restarle parte del avance del
	// padre para lograr el parallax. Si le escribieramos la posicion de mundo,
	// el resultado dependeria de cual de los dos LateUpdate corre primero.
	private bool esCamaraHija;
	private Vector3 localInicial;
	private Vector3 posicionInicialPadre;

	void Awake(){
		instance = this;
	}

	void Start(){
		if (Main_Camera == null || Player_engine.instance == null) {
			enabled = false;
			return;
		}
		camara = Main_Camera.transform;
		originaly = camara.position.y;
		originalz = camara.position.z;

		esCamaraHija = camara.parent != null;
		if (esCamaraHija) {
			localInicial = camara.localPosition;
			posicionInicialPadre = camara.parent.position;
			return;
		}

		// Guardamos el encuadre con el que la camara fue colocada en la escena y
		// lo tratamos como offset, para no alterar el encuadre original.
		offsetX = camara.position.x - Player_engine.instance.getPosX () * factorParallax;
		alturaBase = Player_engine.instance.getPosY ();
		offsetY = originaly - alturaBase * factorParallax;
	}

	// LateUpdate y no Update: el jugador es un Rigidbody con interpolacion, y su
	// transform final se escribe despues de los Update. Leerlo en Update dejaba a
	// la camara un paso atras cada frame, que es lo que producia el tembleque.
	void LateUpdate(){
		if (camara == null || Player_engine.instance == null) {
			return;
		}
		if (esCamaraHija) {
			aplicarParallaxLocal ();
			return;
		}
		float jugadorX = Player_engine.instance.getPosX ();
		float jugadorY = Player_engine.instance.getPosY ();

		actualizarAdelanto ();
		actualizarAlturaBase (jugadorY);

		float objetivoX = (jugadorX + adelantoActual) * factorParallax + offsetX;
		float objetivoY = alturaBase * factorParallax + offsetY;

		// SmoothDamp es independiente de los FPS y no sobrepasa el objetivo, a
		// diferencia del Lerp(a, b, deltaTime * k) que se usaba antes.
		Vector3 posicion = camara.position;
		posicion.x = Mathf.SmoothDamp (posicion.x, objetivoX, ref velX, suavizadoX);
		posicion.y = Mathf.SmoothDamp (posicion.y, objetivoY, ref velY, suavizadoY);
		posicion.z = originalz;
		camara.position = posicion;
	}

	// La camara hija ya viaja pegada a su padre. Para el parallax le restamos la
	// fraccion del recorrido que NO debe acompañar: con factorParallax 0.4 se queda
	// con el 40% del avance. Se calcula contra la posicion inicial (no de forma
	// incremental), asi no acumula error aunque el padre se mueva en otro orden.
	private void aplicarParallaxLocal(){
		if (Mathf.Approximately (factorParallax, 1f)) {
			return;
		}
		Vector3 avance = camara.parent.position - posicionInicialPadre;
		Vector3 retraso = new Vector3 (avance.x, avance.y, 0f) * (1f - factorParallax);
		camara.localPosition = localInicial - retraso;
	}// fin de aplicarParallaxLocal

	// La camara se adelanta hacia donde corre el jugador, para que se vea mas
	// escenario por delante que por detras.
	private void actualizarAdelanto(){
		float velocidadJugador = Player_engine.instance.getVelocityPostX ();
		float objetivo = 0f;
		if (Mathf.Abs (velocidadJugador) > velocidadMinimaAdelanto) {
			objetivo = Mathf.Sign (velocidadJugador) * adelanto;
		}
		adelantoActual = Mathf.SmoothDamp (adelantoActual, objetivo, ref velAdelanto, suavizadoAdelanto);
	}

	// La referencia vertical solo se actualiza al pisar suelo: asi la camara no
	// sube y baja en cada salto, pero si acompaña cuando el jugador aterriza en
	// una plataforma a otra altura. Si se aleja demasiado (caida a un precipicio
	// o salto muy alto), lo sigue igual para no perderlo de vista.
	private void actualizarAlturaBase(float jugadorY){
		if (Player_engine.instance.EstaEnSuelo ()) {
			alturaBase = jugadorY;
			return;
		}
		float desvio = jugadorY - alturaBase;
		if (desvio > margenSuperior) {
			alturaBase = jugadorY - margenSuperior;
		} else if (desvio < -margenInferior) {
			alturaBase = jugadorY + margenInferior;
		}
	}
}

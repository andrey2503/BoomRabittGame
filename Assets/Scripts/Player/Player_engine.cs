using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_engine : MonoBehaviour {
	public static Player_engine instance;
	public GameObject personaje;
	public float salto=1;
	public float mover=0.50f; 
	public bool subirMovimiento = true;
	public float velocidadDeMovimiento = 5f;
	public int dobleSalto=1;
    public Animator anim;
    public GameObject animatedChar;


	//estados de animacion
	bool personajeMoviendoce=false;
	public bool personajeSaltando = false;
	public bool personajeSaltando2	= false;
	bool personajeAtacando= false;


	// Use this for initialization
	void Awake(){
		if(Player_engine.instance == null){
			Player_engine.instance = this;
		}else{
			Destroy (this.gameObject);
		}// else

	}// fin del Awake
	void Start () {
	}
	
	public float getVelocityPostY (){
		return  personaje.GetComponent<Rigidbody> ().linearVelocity.y;
	}

	public float getVelocityPostX (){
		if (personaje == null) {
			return 0f;
		}
		Rigidbody rb = personaje.GetComponent<Rigidbody> ();
		return rb == null ? 0f : rb.linearVelocity.x;
	}

	// estado de suelo autoritativo (incluye el bloqueo posterior al despegue).
	// Lo usa la camara para no rebotar en cada salto.
	public bool EstaEnSuelo (){
		return enSuelo;
	}

	public void MoverDerecha(){
		personaje.GetComponent<Rigidbody> ().linearVelocity = new Vector3(mover,getVelocityPostY(), 0f );
        animatedChar.transform.localScale = new Vector3(1,1,1);
		personajeMoviendoce = true;
		if (!personajeSaltando2) {
			Player_sonidos.instance.iniciar_correr ();
		} else {
			Player_sonidos.instance.parar_correr ();
		}
    }//fin de mover derecha

	public void MoverIzquierda(){
		personaje.GetComponent<Rigidbody> ().linearVelocity = new Vector3(-mover, getVelocityPostY(), 0f );
        animatedChar.transform.localScale = new Vector3(1, 1, -1);
		personajeMoviendoce = true;

		if (!personajeSaltando2) {
			Player_sonidos.instance.iniciar_correr ();
		} else {
			Player_sonidos.instance.parar_correr ();
		}
    }//fin de mover izquierda

	// Entradas de salto. Player_Inputs solo "pide" el salto; quien decide si sale
	// salto normal o doble salto es FixedUpdate, que es el unico que conoce el
	// estado real de suelo. Antes esa decision se tomaba en Update con un raycast
	// que todavia veia el suelo en el frame del despegue.
	public void SolicitarSalto(){
		tiempoInputSalto = Time.time;
		botonSaltoMantenido = true;
	}// fin de SolicitarSalto

	public void SoltarBotonSalto(){
		// al soltar, la subida pasa a pesar mas (ver aplicarGravedad) -> altura variable
		botonSaltoMantenido = false;
	}// fin de SoltarBotonSalto

	// se mantienen por compatibilidad con escenas/scripts que ya las llamaban
	public void Salto(){
		SolicitarSalto ();
	}//fin de salto

	public void activarDobleSalto(){
		SolicitarSalto ();
	}// fin de activarDobleSalto

	public Vector3 getPosition(){
		return transform.position;
	}
	public float getPosX(){
		return personaje.transform.position.x;
	}// fin de getPosx()
	public float getPosY(){
		return personaje.transform.position.y;
	}
	public float getPosZ(){
		return personaje.transform.position.z;
	}

	public void resetPostPlayer(){
		colocarJugadorEn (new Vector3 (0, 2, 0));
		puntoSeguro = new Vector3 (0, 2, 0);
	}

	// --- checkpoints blandos -------------------------------------------
	// En vez de obligar a colocar checkpoints a mano por el nivel, se recuerda
	// el ultimo sitio firme donde el jugador estuvo de pie. Al caer a un
	// precipicio reaparece ahi en lugar de volver al inicio del nivel.
	[Header ("Reaparicion")]
	public float intervaloPuntoSeguro = 0.3f;
	public float alturaReaparicion = 0.6f;
	private Vector3 puntoSeguro;
	private bool hayPuntoSeguro = false;
	private float tiempoUltimoPuntoSeguro = -999f;

	private void registrarPuntoSeguro(){
		if (Time.time - tiempoUltimoPuntoSeguro < intervaloPuntoSeguro) {
			return;
		}
		tiempoUltimoPuntoSeguro = Time.time;
		puntoSeguro = personaje.transform.position;
		hayPuntoSeguro = true;
	}// fin de registrarPuntoSeguro

	public void reaparecerEnPuntoSeguro(){
		Vector3 destino = hayPuntoSeguro ? puntoSeguro + Vector3.up * alturaReaparicion : new Vector3 (0, 2, 0);
		colocarJugadorEn (destino);
	}// fin de reaparecerEnPuntoSeguro

	private void colocarJugadorEn(Vector3 destino){
		personaje.transform.position = destino;
		// sin esto el jugador reaparecia arrastrando la velocidad de la caida anterior
		Rigidbody rb = personaje.GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
		saltoEnCurso = false;
		dobleSaltoDisponible = true;
		botonSaltoMantenido = false;
		tiempoInputSalto = -999f;
		tiempoUltimoSalto = -999f;
	}// fin de colocarJugadorEn
	public void personajeDetenido(){
		this.mover = 2f;
		subirMovimiento = false;
		StopCoroutine (SubirVelocidadMovimiento());
		personajeMoviendoce = false;
		Player_sonidos.instance.parar_correr ();
    }// fin de personajeDetenido

	public void moviendose(){
			StartCoroutine(SubirVelocidadMovimiento());
	}//
	private IEnumerator SubirVelocidadMovimiento(){
			yield return new WaitForSeconds (0f);
			actualizarVelocidad ();
	}// fin de subirVelocidadMovimeinto

	public void personajeEnSalto(){
		subirMovimiento = false;
	}// fin de personajeEnSalto

	public void personajeEnSuelo(){
		subirMovimiento = true;
		// Justo despues de despegar el raycast todavia ve el suelo durante un par de
		// frames. Si no lo ignoramos, se cancelaria el salto recien iniciado (ese era
		// el motivo de que la animacion de salto no llegara a verse).
		if (BloqueoPostSalto) {
			return;
		}
		dobleSalto = 1;
		personajeSaltando = false;
		personajeSaltando2 = false;
		if (!personajeMoviendoce && !personajeAtacando) {
			anim.SetInteger ("estado", 0);
		}
		if (personajeMoviendoce && !personajeAtacando) {
			anim.SetInteger ("estado", 1);
		}
		// mientras personajeAtacando sea true no se pisa el estado 6: lo controla activarAtaque()
	}// fin de personajeEnSuelo

	IEnumerator finalizarEstadoAtaque(float tiempo){
		yield return new WaitForSeconds (tiempo);
		personajeAtacando = false;
	}

	public void actualizarVelocidad(){
		
		if (this.mover < velocidadDeMovimiento) {
			if(subirMovimiento){
			this.mover += 1f;
			}
			moviendose ();
		} else {
			StopCoroutine (SubirVelocidadMovimiento());
		}
	}// fin de actualizar velocidad

	public float fuerzaRebote=20f;
	public void choqueEnemigo(int direccion){
		if (direccion == 1) {
			personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(fuerzaRebote,0,0),ForceMode.Impulse);
		} else {
			personaje.GetComponent<Rigidbody> ().AddForce (new Vector3(-fuerzaRebote,0,0),ForceMode.Impulse);
		}
		Player_Inputs.instance.HabilitarInputs ();
		}// fin de choque enemigo

	// ---------------------------------------------------------------------
	// SALTO estilo Crash Bandicoot
	//
	// El salto se define por ALTURA (metros) y TIEMPO DE SUBIDA (segundos) en vez
	// de por un impulso suelto: asi el arco es exacto y predecible. De ahi se deducen
	// la velocidad inicial y la gravedad necesaria.
	//
	// Los valores por defecto reproducen el alcance del salto original del juego
	// (~3.6 m de alto, ~1.2 s en el aire) para no romper los saltos entre plataformas
	// ya construidas, pero sin el "tiron" que generaban los impulsos con temporizador.
	// ---------------------------------------------------------------------
	[Header ("Salto (estilo Crash Bandicoot)")]
	public float alturaSalto = 3.6f;              // altura maxima del salto completo, en metros
	public float tiempoSubida = 0.62f;            // segundos hasta el punto mas alto
	public float alturaDobleSalto = 2.2f;         // altura extra del doble salto
	public float multiplicadorCaidaRapida = 1.1f; // la bajada pesa un poco mas que la subida
	public float multiplicadorSaltoCorto = 2f;    // al soltar el boton temprano, corta la subida
	public float escalaGravedadApex = 0.7f;       // flote caracteristico en el punto mas alto
	public float umbralApex = 2.5f;               // |velocidad Y| por debajo de la cual estamos en el apex
	public float velocidadMaximaCaida = 30f;      // velocidad terminal, evita atravesar colliders
	public float tiempoCoyote = 0.12f;            // margen para saltar despues de salir de la plataforma
	public float tiempoBufferSalto = 0.14f;       // margen para registrar el salto antes de aterrizar
	public float bloqueoSueloTrasSalto = 0.12f;   // ignora el raycast de suelo justo tras despegar
	public float frenadoEnSuelo = 14f;            // frenado al soltar las flechas (antes patinaba sin fin)

	private bool enSuelo = false;
	private bool saltoEnCurso = false;
	private bool dobleSaltoDisponible = true;
	private bool botonSaltoMantenido = false;
	private float tiempoInputSalto = -999f;
	private float tiempoUltimoSuelo = -999f;
	private float tiempoUltimoSalto = -999f;

	// gravedad que hace falta para que el salto alcance alturaSalto en tiempoSubida
	private float GravedadSubida {
		get { return (2f * alturaSalto) / (tiempoSubida * tiempoSubida); }
	}
	private float VelocidadSalto {
		get { return (2f * alturaSalto) / tiempoSubida; }
	}
	private float VelocidadDobleSalto {
		get { return Mathf.Sqrt (2f * GravedadSubida * alturaDobleSalto); }
	}
	// tras despegar el raycast todavia ve el suelo un par de frames: lo ignoramos
	private bool BloqueoPostSalto {
		get { return Time.time - tiempoUltimoSalto < bloqueoSueloTrasSalto; }
	}

	void FixedUpdate(){
		if (personaje == null) {
			return;
		}
		Rigidbody rb = personaje.GetComponent<Rigidbody> ();
		if (rb == null) {
			return;
		}

		bool sueloReal = Player_jump.instance != null && Player_jump.instance.verificar_suerlo ();
		enSuelo = sueloReal && !BloqueoPostSalto;

		if (enSuelo) {
			tiempoUltimoSuelo = Time.time;
			saltoEnCurso = false;
			dobleSaltoDisponible = true;
			registrarPuntoSeguro ();
		}

		procesarSaltoPendiente (rb);
		aplicarGravedad (rb);
		aplicarFrenadoEnSuelo (rb);

		if (!enSuelo && !personajeAtacando && anim != null) {
			anim.SetInteger ("estado", 2);
		}
	}// fin de FixedUpdate

	private void procesarSaltoPendiente(Rigidbody rb){
		if (Time.time - tiempoInputSalto > tiempoBufferSalto) {
			return;
		}
		// salto normal: en suelo, o dentro del margen de coyote tras salir de la plataforma
		bool puedeSaltoNormal = enSuelo || (!saltoEnCurso && Time.time - tiempoUltimoSuelo <= tiempoCoyote);
		if (puedeSaltoNormal) {
			ejecutarSalto (rb, VelocidadSalto);
			dobleSaltoDisponible = true;
		} else if (dobleSaltoDisponible) {
			ejecutarSalto (rb, VelocidadDobleSalto);
			dobleSaltoDisponible = false;
		}
	}// fin de procesarSaltoPendiente

	private void ejecutarSalto(Rigidbody rb, float velocidad){
		rb.linearVelocity = new Vector3 (rb.linearVelocity.x, velocidad, 0f);
		tiempoUltimoSalto = Time.time;
		tiempoInputSalto = -999f;   // consumimos el buffer
		saltoEnCurso = true;
		enSuelo = false;
		personajeSaltando = true;
		personajeSaltando2 = true;
		if (anim != null) {
			anim.SetInteger ("estado", 2);
		}
		if (Player_sonidos.instance != null) {
			Player_sonidos.instance.iniciar_salto ();
		}
	}// fin de ejecutarSalto

	private void aplicarGravedad(Rigidbody rb){
		float gravedadBase = Mathf.Abs (Physics.gravity.y);
		if (gravedadBase <= 0.001f) {
			return;
		}
		float gravedad = GravedadSubida;
		float velY = rb.linearVelocity.y;
		if (!enSuelo) {
			if (Mathf.Abs (velY) < umbralApex) {
				gravedad *= escalaGravedadApex;          // flote en el apex
			} else if (velY < 0f) {
				gravedad *= multiplicadorCaidaRapida;    // caida con peso
			} else if (!botonSaltoMantenido) {
				gravedad *= multiplicadorSaltoCorto;     // salto corto: se solto el boton durante la subida
			}
		}
		// el Rigidbody ya aplica la gravedad del proyecto: solo sumamos la diferencia
		float extra = gravedad - gravedadBase;
		if (Mathf.Abs (extra) > 0.001f) {
			rb.AddForce (Vector3.down * extra, ForceMode.Acceleration);
		}
		if (rb.linearVelocity.y < -velocidadMaximaCaida) {
			rb.linearVelocity = new Vector3 (rb.linearVelocity.x, -velocidadMaximaCaida, rb.linearVelocity.z);
		}
	}// fin de aplicarGravedad

	private void aplicarFrenadoEnSuelo(Rigidbody rb){
		if (!enSuelo || personajeMoviendoce) {
			return;
		}
		float velX = Mathf.MoveTowards (rb.linearVelocity.x, 0f, frenadoEnSuelo * Time.fixedDeltaTime);
		rb.linearVelocity = new Vector3 (velX, rb.linearVelocity.y, 0f);
	}// fin de aplicarFrenadoEnSuelo

	// duracion real de la animacion "patada": clip de 2.05s reproducido a velocidad 1.9x en el Animator
	public float duracionAtaque = 1.08f;
	public void activarAtaque(){
		if (personajeAtacando) {
			return;
		}
		personajeAtacando = true;
		anim.SetInteger ("estado", 6);
		StartCoroutine (finalizarEstadoAtaque (duracionAtaque));
	}//

	public bool estadoAtaque(){
		return personajeAtacando;
	}// fin de estado Ataque

}// fin de la clase

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Rediseña el HUD de juego (puntos, vida y boton de pausa) en tiempo de ejecucion.
//
// Por que en codigo y no tocando las escenas: el HUD esta construido a mano dentro
// de cada escena (no hay prefab), asi que replicarlo a mano en los 5 niveles seria
// fragil y facil de desincronizar. Aqui se reconstruye igual en todas.
//
// No hace falta agregar nada al Inspector: se engancha solo despues de cargar cada
// escena, antes de los Start(), usando las referencias que ya existen
// (Puntos_control.texto_puntos, Player_life.slideVida) y localizando el boton de
// pausa por el metodo al que llama su onClick.
//
// Todos los sprites e iconos se dibujan por codigo: el proyecto no tiene carpeta
// Resources, asi que no se puede cargar un .png por ruta en runtime.
public static class HUDProfesional {

	// --- paleta ---------------------------------------------------------
	private static readonly Color ColorPanel      = new Color (0.07f, 0.09f, 0.14f, 0.62f);
	private static readonly Color ColorBordePanel = new Color (1f, 1f, 1f, 0.16f);
	private static readonly Color ColorTexto      = Color.white;
	private static readonly Color ColorZanahoria  = new Color (1f, 0.52f, 0.13f);
	private static readonly Color ColorHoja       = new Color (0.35f, 0.74f, 0.28f);
	private static readonly Color ColorCorazon    = new Color (0.93f, 0.27f, 0.33f);
	private static readonly Color ColorFondoBarra = new Color (0.04f, 0.05f, 0.08f, 0.85f);

	// --- medidas del cluster superior izquierdo -------------------------
	private const float MargenPantalla = 16f;
	private const float AnchoPanel     = 236f;
	private const float AltoPanel      = 96f;

	private static Sprite spritePanel;
	private static Sprite spriteCirculo;
	private static Sprite spriteZanahoria;
	private static Sprite spriteCorazon;

	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Arrancar(){
		Aplicar ();
		SceneManager.sceneLoaded += (escena, modo) => Aplicar ();
	}

	private static void Aplicar(){
		// se ejecuta despues de los Awake (los singletons ya existen) y antes de los
		// Start, para que Puntos_control tome el color nuevo como color base del punch
		if (Puntos_control.instance != null && Puntos_control.instance.texto_puntos != null) {
			ConstruirClusterEstado ();
		}
		EstilizarBotonPausa ();
	}

	// ------------------------------------------------------------------
	// Cluster superior izquierdo: panel unico con puntos arriba y vida abajo.
	// Agrupar ambos en un panel oscuro translucido resuelve el problema de
	// legibilidad sobre el cielo claro y da un lenguaje visual comun.
	// ------------------------------------------------------------------
	private static void ConstruirClusterEstado(){
		Text textoPuntos = Puntos_control.instance.texto_puntos;
		Canvas lienzo = textoPuntos.GetComponentInParent<Canvas> ();
		if (lienzo == null) {
			return;
		}
		if (lienzo.transform.Find ("HUD_Estado") != null) {
			return; // ya construido en esta escena
		}

		GameObject panel = NuevoGrafico ("HUD_Estado", lienzo.transform, out Image fondoPanel);
		fondoPanel.sprite = PanelRedondeado ();
		fondoPanel.type = Image.Type.Sliced;
		fondoPanel.color = ColorPanel;
		fondoPanel.raycastTarget = false;
		RectTransform rtPanel = panel.GetComponent<RectTransform> ();
		rtPanel.anchorMin = new Vector2 (0f, 1f);
		rtPanel.anchorMax = new Vector2 (0f, 1f);
		rtPanel.pivot = new Vector2 (0f, 1f);
		rtPanel.anchoredPosition = new Vector2 (MargenPantalla, -MargenPantalla);
		rtPanel.sizeDelta = new Vector2 (AnchoPanel, AltoPanel);
		rtPanel.localScale = Vector3.one;
		// primero en la jerarquia = se dibuja debajo del resto: asi los menus de
		// pausa y game over quedan por encima del HUD y no se ve el panel flotando
		panel.transform.SetAsFirstSibling ();
		Outline bordePanel = panel.AddComponent<Outline> ();
		bordePanel.effectColor = ColorBordePanel;
		bordePanel.effectDistance = new Vector2 (1.5f, -1.5f);

		// --- fila 1: zanahoria + puntaje --------------------------------
		GameObject icono = NuevoGrafico ("Icono_Zanahoria", panel.transform, out Image imgZanahoria);
		imgZanahoria.sprite = IconoZanahoria ();
		imgZanahoria.preserveAspect = true;
		imgZanahoria.raycastTarget = false;
		Anclar (icono.GetComponent<RectTransform> (), 14f, 12f, 36f, 36f);

		textoPuntos.transform.SetParent (panel.transform, false);
		textoPuntos.alignment = TextAnchor.MiddleLeft;
		textoPuntos.fontStyle = FontStyle.Bold;
		textoPuntos.fontSize = 30;
		textoPuntos.color = ColorTexto;
		textoPuntos.raycastTarget = false;
		// sin esto el texto se recorta al crecer el numero
		textoPuntos.horizontalOverflow = HorizontalWrapMode.Overflow;
		textoPuntos.verticalOverflow = VerticalWrapMode.Overflow;
		// la escala venia en 0.21 desde la escena; dejarla en 1 evita el salto de
		// tamano que producia la animacion de punch al sumar el primer punto
		Anclar (textoPuntos.rectTransform, 58f, 12f, 160f, 36f);
		SombraLegible (textoPuntos.gameObject);

		// --- fila 2: corazon + barra de vida ----------------------------
		GameObject corazon = NuevoGrafico ("Icono_Vida", panel.transform, out Image imgCorazon);
		imgCorazon.sprite = IconoCorazon ();
		imgCorazon.preserveAspect = true;
		imgCorazon.raycastTarget = false;
		Anclar (corazon.GetComponent<RectTransform> (), 15f, 58f, 26f, 26f);

		if (Player_life.instance != null && Player_life.instance.slideVida != null) {
			EstilizarBarraVida (Player_life.instance.slideVida, panel.transform);
		}
	}

	// Convierte el Slider por defecto (que se lee como barra de volumen) en una
	// barra de vida: sin el "handle" arrastrable, con fondo oscuro y relleno plano.
	private static void EstilizarBarraVida(Slider barra, Transform padre){
		barra.transform.SetParent (padre, false);
		barra.interactable = false;
		barra.transition = Selectable.Transition.None;
		Anclar (barra.GetComponent<RectTransform> (), 50f, 60f, 168f, 20f);

		// el circulo arrastrable es lo que hacia que pareciera un control de volumen
		Transform zonaHandle = barra.transform.Find ("Handle Slide Area");
		if (zonaHandle != null) {
			zonaHandle.gameObject.SetActive (false);
		}
		barra.handleRect = null;

		Transform fondo = barra.transform.Find ("Background");
		if (fondo != null) {
			Image imgFondo = fondo.GetComponent<Image> ();
			if (imgFondo != null) {
				imgFondo.sprite = PanelRedondeado ();
				imgFondo.type = Image.Type.Sliced;
				imgFondo.color = ColorFondoBarra;
			}
			EstirarADueno (fondo as RectTransform);
		}

		Transform areaRelleno = barra.transform.Find ("Fill Area");
		if (areaRelleno != null) {
			RectTransform rtArea = areaRelleno as RectTransform;
			EstirarADueno (rtArea);
			// un margen pequeno deja ver el fondo como borde de la barra
			rtArea.offsetMin = new Vector2 (3f, 3f);
			rtArea.offsetMax = new Vector2 (-3f, -3f);

			Transform relleno = areaRelleno.Find ("Fill");
			if (relleno != null) {
				Image imgRelleno = relleno.GetComponent<Image> ();
				if (imgRelleno != null) {
					imgRelleno.sprite = PanelRedondeado ();
					imgRelleno.type = Image.Type.Sliced;
					// Player_life usa esta referencia para el degradado verde->rojo
					Player_life.instance.fillVida = imgRelleno;
				}
			}
		}
	}

	// ------------------------------------------------------------------
	// Boton de pausa: se le pone un disco oscuro detras para que contraste
	// contra el cielo. No se le cambia el sprite, asi conserva su icono "II".
	// ------------------------------------------------------------------
	private static void EstilizarBotonPausa(){
		Button botonPausa = LocalizarBotonPausa ();
		if (botonPausa == null) {
			return;
		}
		Transform padre = botonPausa.transform.parent;
		if (padre == null || padre.Find ("HUD_FondoPausa") != null) {
			return;
		}
		RectTransform rtBoton = botonPausa.GetComponent<RectTransform> ();

		GameObject disco = NuevoGrafico ("HUD_FondoPausa", padre, out Image imgDisco);
		imgDisco.sprite = Circulo ();
		imgDisco.color = ColorPanel;
		imgDisco.raycastTarget = false;
		RectTransform rtDisco = disco.GetComponent<RectTransform> ();
		rtDisco.anchorMin = rtBoton.anchorMin;
		rtDisco.anchorMax = rtBoton.anchorMax;
		rtDisco.pivot = rtBoton.pivot;
		rtDisco.anchoredPosition = rtBoton.anchoredPosition;
		rtDisco.sizeDelta = rtBoton.sizeDelta + new Vector2 (12f, 12f);
		rtDisco.localScale = rtBoton.localScale;
		// un indice menor lo deja por detras del boton
		disco.transform.SetSiblingIndex (rtBoton.GetSiblingIndex ());

		Outline bordeDisco = disco.AddComponent<Outline> ();
		bordeDisco.effectColor = ColorBordePanel;
		bordeDisco.effectDistance = new Vector2 (1.5f, -1.5f);
	}

	// Lo identificamos por el metodo al que llama su onClick: es mucho mas fiable
	// que buscarlo por nombre de GameObject, que cambia entre escenas.
	private static Button LocalizarBotonPausa(){
		Button[] botones = Object.FindObjectsByType<Button> (FindObjectsInactive.Include, FindObjectsSortMode.None);
		foreach (Button boton in botones) {
			for (int i = 0; i < boton.onClick.GetPersistentEventCount (); i++) {
				if (boton.onClick.GetPersistentMethodName (i) == "activarMenuPausa") {
					return boton;
				}
			}
		}
		return null;
	}

	// ------------------------------------------------------------------
	// utilidades
	// ------------------------------------------------------------------
	private static GameObject NuevoGrafico(string nombre, Transform padre, out Image imagen){
		GameObject go = new GameObject (nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		go.transform.SetParent (padre, false);
		imagen = go.GetComponent<Image> ();
		return go;
	}

	// ancla al borde superior izquierdo del padre; "y" se pasa en positivo hacia abajo
	private static void Anclar(RectTransform rt, float x, float y, float ancho, float alto){
		rt.anchorMin = new Vector2 (0f, 1f);
		rt.anchorMax = new Vector2 (0f, 1f);
		rt.pivot = new Vector2 (0f, 1f);
		rt.anchoredPosition = new Vector2 (x, -y);
		rt.sizeDelta = new Vector2 (ancho, alto);
		rt.localScale = Vector3.one;
		rt.localRotation = Quaternion.identity;
	}

	private static void EstirarADueno(RectTransform rt){
		if (rt == null) {
			return;
		}
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}

	private static void SombraLegible(GameObject objetivo){
		if (objetivo.GetComponent<Shadow> () != null) {
			return;
		}
		Shadow sombra = objetivo.AddComponent<Shadow> ();
		sombra.effectColor = new Color (0f, 0f, 0f, 0.65f);
		sombra.effectDistance = new Vector2 (1.5f, -1.5f);
	}

	// ------------------------------------------------------------------
	// sprites generados por codigo (el proyecto no tiene carpeta Resources)
	// ------------------------------------------------------------------
	private static Sprite PanelRedondeado(){
		if (spritePanel != null) {
			return spritePanel;
		}
		const int tam = 64;
		const int radio = 16;
		Texture2D tex = NuevaTextura (tam);
		for (int y = 0; y < tam; y++) {
			for (int x = 0; x < tam; x++) {
				tex.SetPixel (x, y, new Color (1f, 1f, 1f, AlfaEsquinaRedondeada (x, y, tam, radio)));
			}
		}
		tex.Apply ();
		// el borde de 'radio' activa el 9-slice: el panel escala sin deformar esquinas
		spritePanel = Sprite.Create (tex, new Rect (0f, 0f, tam, tam), new Vector2 (0.5f, 0.5f),
			100f, 0, SpriteMeshType.FullRect, new Vector4 (radio, radio, radio, radio));
		return spritePanel;
	}

	private static Sprite Circulo(){
		if (spriteCirculo != null) {
			return spriteCirculo;
		}
		const int tam = 96;
		Texture2D tex = NuevaTextura (tam);
		float centro = (tam - 1) * 0.5f;
		float radio = centro - 1f;
		for (int y = 0; y < tam; y++) {
			for (int x = 0; x < tam; x++) {
				float d = Mathf.Sqrt ((x - centro) * (x - centro) + (y - centro) * (y - centro));
				tex.SetPixel (x, y, new Color (1f, 1f, 1f, Mathf.Clamp01 (radio - d)));
			}
		}
		tex.Apply ();
		spriteCirculo = Sprite.Create (tex, new Rect (0f, 0f, tam, tam), new Vector2 (0.5f, 0.5f));
		return spriteCirculo;
	}

	// zanahoria: cuerpo triangular hacia abajo + tres hojas arriba
	private static Sprite IconoZanahoria(){
		if (spriteZanahoria != null) {
			return spriteZanahoria;
		}
		const int tam = 64;
		Texture2D tex = NuevaTextura (tam);
		for (int y = 0; y < tam; y++) {
			for (int x = 0; x < tam; x++) {
				Color c = new Color (0f, 0f, 0f, 0f);
				if (y >= 4 && y <= 42) {
					float t = (y - 4f) / 38f;
					float mitad = Mathf.Lerp (1.5f, 12f, t);
					if (Mathf.Abs (x - 32f) <= mitad) {
						c = ColorZanahoria;
					}
				} else if (y > 42 && y <= 58) {
					// tres hojas: central recta y dos inclinadas
					float h = (y - 42f) / 16f;
					if (Mathf.Abs (x - 32f) <= Mathf.Lerp (4f, 1.5f, h)) {
						c = ColorHoja;
					} else if (Mathf.Abs (x - (32f - Mathf.Lerp (2f, 11f, h))) <= Mathf.Lerp (3.5f, 1.2f, h)) {
						c = ColorHoja;
					} else if (Mathf.Abs (x - (32f + Mathf.Lerp (2f, 11f, h))) <= Mathf.Lerp (3.5f, 1.2f, h)) {
						c = ColorHoja;
					}
				}
				tex.SetPixel (x, y, c);
			}
		}
		tex.Apply ();
		spriteZanahoria = Sprite.Create (tex, new Rect (0f, 0f, tam, tam), new Vector2 (0.5f, 0.5f));
		return spriteZanahoria;
	}

	// corazon: union de dos circulos arriba y un triangulo apuntando abajo
	private static Sprite IconoCorazon(){
		if (spriteCorazon != null) {
			return spriteCorazon;
		}
		const int tam = 64;
		Texture2D tex = NuevaTextura (tam);
		Vector2 izq = new Vector2 (22f, 40f);
		Vector2 der = new Vector2 (42f, 40f);
		const float radio = 12f;
		for (int y = 0; y < tam; y++) {
			for (int x = 0; x < tam; x++) {
				bool dentro = Vector2.Distance (new Vector2 (x, y), izq) <= radio
					|| Vector2.Distance (new Vector2 (x, y), der) <= radio;
				if (!dentro && y <= 40f && y >= 6f) {
					// triangulo: se angosta hacia la punta inferior
					float t = (40f - y) / 34f;
					if (Mathf.Abs (x - 32f) <= Mathf.Lerp (22f, 0f, t)) {
						dentro = true;
					}
				}
				tex.SetPixel (x, y, dentro ? ColorCorazon : new Color (0f, 0f, 0f, 0f));
			}
		}
		tex.Apply ();
		spriteCorazon = Sprite.Create (tex, new Rect (0f, 0f, tam, tam), new Vector2 (0.5f, 0.5f));
		return spriteCorazon;
	}

	private static Texture2D NuevaTextura(int tam){
		Texture2D tex = new Texture2D (tam, tam, TextureFormat.RGBA32, false);
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Bilinear;
		tex.hideFlags = HideFlags.HideAndDontSave;
		return tex;
	}

	// alfa con antialias: 1 dentro, degradado de 1px en el borde de la esquina
	private static float AlfaEsquinaRedondeada(int x, int y, int tam, int radio){
		float cx = x < radio ? radio : (x > tam - 1 - radio ? tam - 1 - radio : x);
		float cy = y < radio ? radio : (y > tam - 1 - radio ? tam - 1 - radio : y);
		float d = Mathf.Sqrt ((x - cx) * (x - cx) + (y - cy) * (y - cy));
		return Mathf.Clamp01 (radio - d);
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Da presentacion a la pantalla de victoria, que hoy aparece de golpe sobre el
// juego en marcha: sin atenuar el fondo, sin animacion y con el boton
// "Siguiente nivel" como texto suelto, que no se lee como boton.
//
// Se engancha solo tras cargar la escena, igual que HUDProfesional: no hay que
// tocar nada en el Inspector.
public static class MenuGanarMejorado {

	private static readonly Color ColorScrim = new Color (0f, 0f, 0f, 0.55f);
	private static readonly Color ColorBoton = new Color (0.16f, 0.62f, 0.26f, 1f);
	private const string NombreScrim = "HUD_FondoVictoria";

	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Arrancar(){
		Aplicar ();
		SceneManager.sceneLoaded += (escena, modo) => Aplicar ();
	}

	private static void Aplicar(){
		if (ControlMenus.instance == null || ControlMenus.instance.menuGano == null) {
			return;
		}
		GameObject panel = ControlMenus.instance.menuGano;
		if (panel.GetComponent<AnimacionMenuGanar> () != null) {
			return;   // ya preparado en esta escena
		}

		GameObject scrim = CrearScrim (panel);
		EstilizarBotones (panel);

		AnimacionMenuGanar animacion = panel.AddComponent<AnimacionMenuGanar> ();
		animacion.scrim = scrim;
		animacion.textoPuntos = ControlMenus.instance.texto_puntos_menu_ganado;
	}

	// Capa oscura a pantalla completa POR DETRAS del panel. Tiene que ser hermano
	// anterior y no hijo: en UGUI el grafico del padre se dibuja antes que el de
	// sus hijos, asi que un hijo quedaria encima del panel en vez de detras.
	private static GameObject CrearScrim(GameObject panel){
		Transform padre = panel.transform.parent;
		if (padre == null) {
			return null;
		}
		Transform existente = padre.Find (NombreScrim);
		if (existente != null) {
			return existente.gameObject;
		}

		GameObject scrim = new GameObject (NombreScrim, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
		scrim.transform.SetParent (padre, false);
		Image img = scrim.GetComponent<Image> ();
		img.color = ColorScrim;
		img.raycastTarget = true;   // bloquea clics al escenario de atras

		RectTransform rt = scrim.GetComponent<RectTransform> ();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		rt.localScale = Vector3.one;

		scrim.transform.SetSiblingIndex (panel.transform.GetSiblingIndex ());
		scrim.SetActive (false);
		return scrim;
	}

	// El boton "Siguiente nivel" era texto plano sin fondo: no comunica que se
	// puede pulsar. Le ponemos cuerpo y la misma reaccion que el resto de botones.
	private static void EstilizarBotones(GameObject panel){
		foreach (Button boton in panel.GetComponentsInChildren<Button> (true)) {
			Image fondo = boton.GetComponent<Image> ();
			if (fondo == null) {
				fondo = boton.gameObject.AddComponent<Image> ();
			}
			// solo le damos cuerpo si no tenia uno visible
			if (fondo.sprite == null || fondo.color.a < 0.1f) {
				fondo.sprite = HUDProfesional.PanelRedondeado ();
				fondo.type = Image.Type.Sliced;
				fondo.color = ColorBoton;
			}
			boton.targetGraphic = fondo;

			foreach (Text texto in boton.GetComponentsInChildren<Text> (true)) {
				texto.fontStyle = FontStyle.Bold;
				texto.color = Color.white;
			}
			if (boton.GetComponent<UIBotonPunch> () == null) {
				boton.gameObject.AddComponent<UIBotonPunch> ();
			}
		}
	}
}

// Anima la aparicion del panel y el conteo de puntos.
// Usa tiempo sin escalar para que siga funcionando aunque el juego se pause.
public class AnimacionMenuGanar : MonoBehaviour {

	public GameObject scrim;
	public Text textoPuntos;

	public float duracionPop = 0.45f;
	public float duracionConteo = 0.9f;

	private RectTransform rt;

	void OnEnable(){
		rt = GetComponent<RectTransform> ();
		if (scrim != null) {
			scrim.SetActive (true);
		}
		OcultarHudDeJuego (true);
		StartCoroutine (Animar ());
	}

	void OnDisable(){
		if (scrim != null) {
			scrim.SetActive (false);
		}
		OcultarHudDeJuego (false);
	}

	private IEnumerator Animar(){
		int objetivo = Puntos_control.instance != null ? Puntos_control.instance.getPuntos () : 0;
		if (textoPuntos != null) {
			textoPuntos.text = "0";
		}

		// entrada con rebote
		float t = 0f;
		while (t < duracionPop) {
			t += Time.unscaledDeltaTime;
			rt.localScale = Vector3.one * SalidaConRebote (Mathf.Clamp01 (t / duracionPop));
			yield return null;
		}
		rt.localScale = Vector3.one;

		// los puntos suben hasta el total en vez de aparecer de golpe
		if (textoPuntos == null || objetivo <= 0) {
			yield break;
		}
		t = 0f;
		while (t < duracionConteo) {
			t += Time.unscaledDeltaTime;
			int valor = Mathf.RoundToInt (Mathf.Lerp (0f, objetivo, Mathf.Clamp01 (t / duracionConteo)));
			textoPuntos.text = valor.ToString ();
			yield return null;
		}
		textoPuntos.text = objetivo.ToString ();
	}

	// El HUD de juego compite con la pantalla de victoria; lo apartamos mientras tanto.
	// Se cachea la referencia la primera vez: una vez oculto, buscarlo de nuevo con
	// GetComponentInParent fallaria (ignora objetos inactivos) y no podriamos restaurarlo.
	private GameObject hudEstado;
	private bool hudBuscado;

	private void OcultarHudDeJuego(bool ocultar){
		if (!hudBuscado) {
			hudBuscado = true;
			foreach (Canvas lienzo in Object.FindObjectsByType<Canvas> (FindObjectsInactive.Include, FindObjectsSortMode.None)) {
				Transform encontrado = lienzo.transform.Find ("HUD_Estado");
				if (encontrado != null) {
					hudEstado = encontrado.gameObject;
					break;
				}
			}
		}
		if (hudEstado != null) {
			hudEstado.SetActive (!ocultar);
		}
	}

	// ease-out-back: sobrepasa un poco el tamano final y vuelve, da sensacion de premio
	private static float SalidaConRebote(float x){
		const float c1 = 1.70158f;
		const float c3 = c1 + 1f;
		float p = x - 1f;
		return 1f + c3 * p * p * p + c1 * p * p;
	}
}

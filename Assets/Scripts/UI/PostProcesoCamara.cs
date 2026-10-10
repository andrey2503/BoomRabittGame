using UnityEngine;
using UnityEngine.SceneManagement;

// Aplica el color grading y la vineta sobre la imagen final.
//
// Se engancha solo a la camara de mayor 'depth' del stack (la que dibuja al final),
// de modo que procesa la composicion completa: fondo + nivel + personaje. No hay
// que anadir nada en el Inspector ni en las escenas.
//
// Si el shader no carga o la tarjeta no lo soporta, la imagen pasa sin tocar: nunca
// deja la pantalla en magenta.
[DisallowMultipleComponent]
public class PostProcesoCamara : MonoBehaviour {

	private const string RutaShader = "Shaders/ColorGrade";

	[Range (0.5f, 1.5f)] public float brillo = 1f;
	[Range (0.5f, 2f)]   public float contraste = 1.06f;
	[Range (0f, 2f)]     public float saturacion = 1.12f;
	[Range (0f, 1f)]     public float vineta = 0.22f;
	[Range (0.1f, 1f)]   public float vinetaSuavidad = 0.65f;

	private Material material;
	private bool shaderNoDisponible;

	[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Arrancar(){
		Aplicar ();
		SceneManager.sceneLoaded += (escena, modo) => Aplicar ();
	}

	// lo colocamos en la camara que renderiza de ultima: con el stack de dos camaras
	// esa recibe ya compuesto lo que dibujo la de fondo
	private static void Aplicar(){
		Camera[] camaras = Object.FindObjectsByType<Camera> (FindObjectsInactive.Exclude, FindObjectsSortMode.None);
		Camera destino = null;
		foreach (Camera camara in camaras) {
			if (destino == null || camara.depth > destino.depth) {
				destino = camara;
			}
		}
		if (destino == null) {
			return;
		}

		// Al tener un image effect, Unity renderiza la camara destino a una
		// RenderTexture temporal. Como esa camara usa ClearFlags.Nothing para
		// componerse sobre la de fondo, hay que forzar tambien a las anteriores a
		// dibujar en RenderTexture; si no, la RT no contendria el fondo y se perderia.
		foreach (Camera camara in camaras) {
			if (camara != destino) {
				camara.forceIntoRenderTexture = true;
			}
		}

		if (destino.GetComponent<PostProcesoCamara> () == null) {
			destino.gameObject.AddComponent<PostProcesoCamara> ();
		}
	}

	void OnRenderImage(RenderTexture origen, RenderTexture destino){
		if (!PrepararMaterial ()) {
			Graphics.Blit (origen, destino);   // pasa sin efecto, nunca en magenta
			return;
		}
		material.SetFloat ("_Brillo", brillo);
		material.SetFloat ("_Contraste", contraste);
		material.SetFloat ("_Saturacion", saturacion);
		material.SetFloat ("_Vineta", vineta);
		material.SetFloat ("_VinetaSuavidad", vinetaSuavidad);
		Graphics.Blit (origen, destino, material);
	}

	private bool PrepararMaterial(){
		if (shaderNoDisponible) {
			return false;
		}
		if (material != null) {
			return true;
		}
		Shader shader = Resources.Load<Shader> (RutaShader);
		if (shader == null || !shader.isSupported) {
			shaderNoDisponible = true;
			Debug.LogWarning ("PostProcesoCamara: no se pudo cargar '" + RutaShader
				+ "'. La imagen se mostrara sin color grading.");
			return false;
		}
		material = new Material (shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		return true;
	}

	void OnDestroy(){
		if (material != null) {
			DestroyImmediate (material);
		}
	}
}

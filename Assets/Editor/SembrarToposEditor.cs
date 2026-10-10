using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Herramienta para colocar enemigos (topos) sobre las plataformas.
//
// El nivel 1 no tiene ni un enemigo, y los demas tienen entre 1 y 4. Colocarlos a
// mano uno por uno es tedioso porque hay que calcular la altura de cada plataforma;
// esto lo hace solo a partir de los bounds reales del objeto.
//
// Convenios copiados de los topos que ya existen en los niveles 3, 4 y 5:
//   - cuelgan de un contenedor llamado "ControlEneigos" (el nombre va con la errata
//     original para no romper la consistencia con las escenas que ya lo traen)
//   - rotacion Euler (0, -90, 0), si no el modelo mira hacia la camara
//   - z = 0, que es el plano por el que camina el jugador
//
// Menu: Tools > BoomRabitt > Enemigos
public static class SembrarToposEditor {

	private const string RutaPrefabTopo = "Assets/prefabs/topo_enemigo.prefab";
	private const string NombreContenedor = "ControlEneigos";
	private const string TagSuelo = "suelo";

	private static readonly Vector3 RotacionTopo = new Vector3 (0f, -90f, 0f);
	private const float PlanoZ = 0f;
	private const float AlturaExtra = 0.02f;   // despega un pelo de la superficie

	// filtros del modo automatico
	private const float AnchoMinimoPlataforma = 3f;    // no sembrar en plataformas diminutas
	private const float SeparacionMinima = 6f;         // distancia minima entre topos

	// ------------------------------------------------------------------
	[MenuItem ("Tools/BoomRabitt/Enemigos/Sembrar topo en plataformas seleccionadas")]
	public static void SembrarEnSeleccion(){
		GameObject prefab = CargarPrefab ();
		if (prefab == null) {
			return;
		}
		GameObject[] seleccion = Selection.gameObjects;
		if (seleccion == null || seleccion.Length == 0) {
			Debug.LogWarning ("Selecciona primero una o varias plataformas en la jerarquia.");
			return;
		}

		Transform contenedor = ObtenerContenedor ();
		int puestos = 0;
		foreach (GameObject plataforma in seleccion) {
			if (ColocarSobre (plataforma, prefab, contenedor)) {
				puestos++;
			}
		}
		Finalizar (puestos);
	}

	[MenuItem ("Tools/BoomRabitt/Enemigos/Sembrar automaticamente en plataformas grandes")]
	public static void SembrarAutomatico(){
		GameObject prefab = CargarPrefab ();
		if (prefab == null) {
			return;
		}

		// candidatas: todo lo que tenga el tag "suelo" y sea lo bastante ancho
		List<GameObject> candidatas = new List<GameObject> ();
		foreach (GameObject go in Object.FindObjectsByType<GameObject> (FindObjectsInactive.Exclude, FindObjectsSortMode.None)) {
			if (!go.CompareTag (TagSuelo)) {
				continue;
			}
			Bounds b;
			if (!TryBounds (go, out b)) {
				continue;
			}
			if (b.size.x >= AnchoMinimoPlataforma) {
				candidatas.Add (go);
			}
		}

		if (candidatas.Count == 0) {
			Debug.LogWarning ("No se encontraron plataformas con tag '" + TagSuelo + "' lo bastante anchas.");
			return;
		}

		// de izquierda a derecha, respetando una separacion minima para no amontonarlos
		candidatas.Sort ((a, b) => a.transform.position.x.CompareTo (b.transform.position.x));
		Transform contenedor = ObtenerContenedor ();
		int puestos = 0;
		float ultimaX = float.NegativeInfinity;
		foreach (GameObject plataforma in candidatas) {
			if (plataforma.transform.position.x - ultimaX < SeparacionMinima) {
				continue;
			}
			if (ColocarSobre (plataforma, prefab, contenedor)) {
				ultimaX = plataforma.transform.position.x;
				puestos++;
			}
		}
		Finalizar (puestos);
	}

	[MenuItem ("Tools/BoomRabitt/Enemigos/Quitar todos los topos de la escena")]
	public static void QuitarTodos(){
		int borrados = 0;
		foreach (Enemigo_comportamiento enemigo in Object.FindObjectsByType<Enemigo_comportamiento> (FindObjectsInactive.Include, FindObjectsSortMode.None)) {
			GameObject raiz = PrefabUtility.GetOutermostPrefabInstanceRoot (enemigo.gameObject);
			Undo.DestroyObjectImmediate (raiz != null ? raiz : enemigo.gameObject);
			borrados++;
		}
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Debug.Log ("Topos eliminados: " + borrados);
	}

	// ------------------------------------------------------------------
	private static bool ColocarSobre(GameObject plataforma, GameObject prefab, Transform contenedor){
		Bounds b;
		if (!TryBounds (plataforma, out b)) {
			Debug.LogWarning ("'" + plataforma.name + "' no tiene Renderer ni Collider; no se puede calcular su superficie.");
			return false;
		}

		GameObject topo = (GameObject)PrefabUtility.InstantiatePrefab (prefab, contenedor);
		Undo.RegisterCreatedObjectUndo (topo, "Sembrar topo");
		// sobre la cara superior real del objeto, no sobre su pivote
		topo.transform.position = new Vector3 (b.center.x, b.max.y + AlturaExtra, PlanoZ);
		topo.transform.rotation = Quaternion.Euler (RotacionTopo);
		topo.name = "topo_enemigo";
		return true;
	}

	// bounds en mundo: primero los Renderer (incluye hijos), si no hay, el Collider
	private static bool TryBounds(GameObject go, out Bounds bounds){
		bounds = new Bounds ();
		bool hay = false;
		foreach (Renderer r in go.GetComponentsInChildren<Renderer> ()) {
			if (!hay) {
				bounds = r.bounds;
				hay = true;
			} else {
				bounds.Encapsulate (r.bounds);
			}
		}
		if (hay) {
			return true;
		}
		Collider col = go.GetComponentInChildren<Collider> ();
		if (col != null) {
			bounds = col.bounds;
			return true;
		}
		return false;
	}

	private static Transform ObtenerContenedor(){
		GameObject contenedor = GameObject.Find (NombreContenedor);
		if (contenedor == null) {
			contenedor = new GameObject (NombreContenedor);
			Undo.RegisterCreatedObjectUndo (contenedor, "Crear contenedor de enemigos");
		}
		return contenedor.transform;
	}

	private static GameObject CargarPrefab(){
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> (RutaPrefabTopo);
		if (prefab == null) {
			Debug.LogError ("No se encontro el prefab del topo en " + RutaPrefabTopo);
		}
		return prefab;
	}

	private static void Finalizar(int puestos){
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Debug.Log ("Topos colocados: " + puestos + ". Revisa que patrullen bien y guarda con Ctrl+S.");
	}
}

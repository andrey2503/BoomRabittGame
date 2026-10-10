using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Repara las 15 zanahorias del nivel 2.
//
// Que paso: apuntaban al prefab con guid b3ea5b7765152e64b94da4329d96c6d3, que fue
// borrado del proyecto (no hay ningun .meta con ese guid). Unity ya no puede
// reconstruirlas y las dejo como GameObjects vacios llamados "Missing Prefab",
// con solo un Transform: sin malla, sin collider y sin Control_zanahorias.
//
// Por que las posiciones van escritas aqui y no se leen de la escena: al convertirlas
// en placeholders, Unity les puso m_LocalPosition (0,0,0) a las 15. La posicion
// original solo sobrevive dentro de los bloques m_Modification del .unity, de donde
// se extrajeron estas coordenadas. Si se copiara la posicion del objeto vivo,
// quedarian las 15 amontonadas en el mismo punto.
//
// Menu: Tools > BoomRabitt > Zanahorias
public static class RepararZanahoriasEditor {

	private const string RutaPrefabBueno = "Assets/prefabs/zanahoria_prefab.prefab";
	private const string NombrePadre = "ControlZanahorias";
	private const string NombrePlaceholder = "Missing Prefab";

	// Posiciones LOCALES respecto a 'ControlZanahorias', leidas del archivo de escena.
	private static readonly Vector3[] Posiciones = new Vector3[] {
		new Vector3 (-4.7700005f,  0.1f,        0f),
		new Vector3 (-4.3900003f,  0.0999999f,  0f),
		new Vector3 (-4.05f,       0.0999999f,  0f),
		new Vector3 ( 0.4099989f,  1.1199999f,  0f),
		new Vector3 ( 0.789999f,   1.1199999f,  0f),
		new Vector3 ( 1.13f,       1.12f,       0f),
		new Vector3 ( 2.6600018f,  3.2800002f,  0f),
		new Vector3 ( 3.04f,       3.28f,       0f),
		new Vector3 ( 3.380001f,   3.2800002f,  0f),
		new Vector3 ( 5.1100006f,  1.25f,       0f),
		new Vector3 ( 5.49f,       1.25f,       0f),
		new Vector3 ( 5.83f,       1.25f,       0f),
		new Vector3 (10.62f,       0.08f,       0f),
		new Vector3 (10.999998f,   0.0799999f,  0f),
		new Vector3 (11.339998f,   0.0799999f,  0f),
	};

	[MenuItem ("Tools/BoomRabitt/Zanahorias/Reparar las 15 del nivel 2")]
	public static void Reparar(){
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> (RutaPrefabBueno);
		if (prefab == null) {
			Debug.LogError ("No se encontro el prefab bueno en " + RutaPrefabBueno);
			return;
		}
		GameObject padre = GameObject.Find (NombrePadre);
		if (padre == null) {
			Debug.LogError ("No se encontro '" + NombrePadre + "'. Abre la escena escenario_nivel_2.");
			return;
		}

		int borrados = BorrarPlaceholders ();

		for (int i = 0; i < Posiciones.Length; i++) {
			GameObject zanahoria = (GameObject)PrefabUtility.InstantiatePrefab (prefab, padre.transform);
			Undo.RegisterCreatedObjectUndo (zanahoria, "Reparar zanahorias");
			zanahoria.name = "zanahoria (" + (i + 1) + ")";
			zanahoria.transform.localPosition = Posiciones[i];
			zanahoria.transform.localRotation = Quaternion.identity;
			// la escala se deja la del prefab, para que queden igual que en los otros niveles
		}

		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Debug.Log ("Zanahorias reparadas: " + Posiciones.Length + " creadas, "
			+ borrados + " placeholders rotos eliminados. GUARDA LA ESCENA con Ctrl+S "
			+ "para que el error desaparezca de la consola.");
	}

	[MenuItem ("Tools/BoomRabitt/Zanahorias/Solo borrar placeholders rotos")]
	public static void SoloBorrar(){
		int borrados = BorrarPlaceholders ();
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Debug.Log ("Placeholders rotos eliminados: " + borrados + ". Guarda con Ctrl+S.");
	}

	// Los busca por dos vias porque Unity, segun la version, a veces deja el objeto
	// marcado como instancia con prefab faltante y a veces solo lo renombra.
	private static int BorrarPlaceholders(){
		HashSet<GameObject> aBorrar = new HashSet<GameObject> ();
		foreach (GameObject go in Object.FindObjectsByType<GameObject> (FindObjectsInactive.Include, FindObjectsSortMode.None)) {
			bool roto = PrefabUtility.IsPrefabAssetMissing (go) || go.name.StartsWith (NombrePlaceholder);
			if (!roto) {
				continue;
			}
			GameObject raiz = PrefabUtility.GetOutermostPrefabInstanceRoot (go);
			aBorrar.Add (raiz != null ? raiz : go);
		}
		foreach (GameObject go in aBorrar) {
			Undo.DestroyObjectImmediate (go);
		}
		return aBorrar.Count;
	}
}

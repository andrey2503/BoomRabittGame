using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Mantiene una iluminacion consistente entre todos los niveles.
//
// IMPORTANTE: esta herramienta NO toca RenderSettings.ambientMode a proposito.
// Al asignarlo, Unity re-serializa el bloque de iluminacion y puede reescribir
// m_AmbientSkyColor con su valor por defecto (gris oscuro). Eso fue lo que dejo
// el nivel 2 con el gradiente invertido (cielo oscuro arriba, suelo claro abajo),
// que es como se ilumina un objeto "desde abajo" y lo hace ver plano y sucio.
// El modo gradiente ya esta guardado en las escenas (m_AmbientMode: 1).
//
// Menu: Tools > BoomRabitt > ...
public static class MejorarVisualesEditor {

	// Gradiente ambiental: CLARO arriba -> OSCURO abajo. Ese orden es el que da
	// volumen a los objetos; invertirlo es lo que los aplana.
	private static readonly Color ColorCielo   = new Color (0.52f, 0.62f, 0.75f);
	private static readonly Color ColorEcuador = new Color (0.44f, 0.44f, 0.44f);
	private static readonly Color ColorSuelo   = new Color (0.30f, 0.25f, 0.21f);

	private const float IntensidadLuz = 1.2f;
	private const float FuerzaSombra = 0.85f;
	private const float BiasSombra = 0.04f;
	private const float NormalBiasSombra = 0.25f;

	private static readonly string[] EscenasDeNivel = new string[] {
		"Assets/escenas/escenario_nivel_1.unity",
		"Assets/escenas/escenario_nivel_2.unity",
		"Assets/escenas/escenario_nivel_3.unity",
		"Assets/escenas/escenario_nivel_4.unity",
		"Assets/escenas/escenario_nivel_5.unity",
	};

	[MenuItem("Tools/BoomRabitt/Igualar Iluminacion de TODOS los Niveles")]
	public static void IgualarIluminacionTodosLosNiveles(){
		string escenaOriginal = EditorSceneManager.GetActiveScene ().path;
		if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
			return;
		}
		foreach (string ruta in EscenasDeNivel) {
			EditorSceneManager.OpenScene (ruta, OpenSceneMode.Single);
			IgualarIluminacion ();
			EditorSceneManager.SaveOpenScenes ();
		}
		if (!string.IsNullOrEmpty (escenaOriginal)) {
			EditorSceneManager.OpenScene (escenaOriginal, OpenSceneMode.Single);
		}
		Debug.Log ("Iluminacion igualada en los " + EscenasDeNivel.Length + " niveles.");
	}

	[MenuItem("Tools/BoomRabitt/Igualar Iluminacion del Nivel Actual")]
	public static void IgualarIluminacion(){
		RenderSettings.ambientSkyColor = ColorCielo;
		RenderSettings.ambientEquatorColor = ColorEcuador;
		RenderSettings.ambientGroundColor = ColorSuelo;
		RenderSettings.ambientIntensity = 1f;

		// niebla suave: separa el fondo del primer plano y da profundidad
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.ExponentialSquared;
		RenderSettings.fogColor = new Color (0.73f, 0.86f, 0.95f);
		RenderSettings.fogDensity = 0.006f;

		int luces = 0;
		foreach (Light luz in Object.FindObjectsByType<Light> (FindObjectsSortMode.None)) {
			if (luz.type != LightType.Directional) {
				continue;
			}
			luz.intensity = IntensidadLuz;
			luz.shadows = LightShadows.Soft;
			luz.shadowStrength = FuerzaSombra;
			luz.shadowBias = BiasSombra;
			luz.shadowNormalBias = NormalBiasSombra;
			EditorUtility.SetDirty (luz);
			luces++;
		}

		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		Debug.Log ("Iluminacion igualada en '" + EditorSceneManager.GetActiveScene ().name
			+ "': gradiente ambiental claro->oscuro, niebla suave y " + luces + " luz(ces) direccional(es).");
	}
}

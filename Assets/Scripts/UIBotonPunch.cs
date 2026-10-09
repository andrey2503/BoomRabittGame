using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

// Agregar este componente a un boton (de nivel, menu, etc.) para darle vida visual:
// crece un poco al pasar el mouse/tocar y rebota al hacer click.
public class UIBotonPunch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	public float escalaHover = 1.1f;
	public float escalaClick = 0.9f;
	public float velocidadTransicion = 10f;

	private Vector3 escalaBase;
	private Vector3 escalaObjetivo;
	private Coroutine clickRoutine;

	void Awake(){
		escalaBase = transform.localScale;
		escalaObjetivo = escalaBase;
	}

	void Update(){
		transform.localScale = Vector3.Lerp (transform.localScale, escalaObjetivo, Time.deltaTime * velocidadTransicion);
	}

	public void OnPointerEnter (PointerEventData eventData){
		escalaObjetivo = escalaBase * escalaHover;
	}

	public void OnPointerExit (PointerEventData eventData){
		escalaObjetivo = escalaBase;
	}

	public void OnPointerClick (PointerEventData eventData){
		if (clickRoutine != null) {
			StopCoroutine (clickRoutine);
		}
		clickRoutine = StartCoroutine (RebotarClick ());
	}

	private IEnumerator RebotarClick(){
		transform.localScale = escalaBase * escalaClick;
		yield return new WaitForSeconds (0.08f);
		escalaObjetivo = escalaBase * escalaHover;
		clickRoutine = null;
	}
}

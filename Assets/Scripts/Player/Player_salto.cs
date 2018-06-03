using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_salto : MonoBehaviour {

		#region Set in editor;

		public float jumpDuration = 0.5f;
		public float jumpDistance = 3;
		
		
		#endregion Set in editor;

		private bool jumping = false;
		private float jumpStartVelocityY;

		private void Start()
		{
			// For a given distance and jump duration
			// there is only one possible movement curve.
			// We are executing Y axis movement separately,
			// so we need to know a starting velocity.
			jumpStartVelocityY = -jumpDuration * Physics.gravity.y/1;
		}

		private void Update()
		{

			

			if (jumping)
			{
				return;
			}
		/*
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				// Warning: this will actually move jumpDistance forward
				// and jumpDistance to the side.
				// If you want to move jumpDistance diagonally, use:
				Vector3 forwardAndLeft = (transform.up - transform.right).normalized * jumpDistance;
				//Vector3 forwardAndLeft = (transform.forward - transform.right) * jumpDistance;
				StartCoroutine(Jump(forwardAndLeft));
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				Vector3 forwardAndRight = (transform.up + transform.right).normalized * jumpDistance;
				StartCoroutine(Jump(forwardAndRight));
			}
		*/
		else if(Input.GetKeyDown(KeyCode.UpArrow)){
			Vector3 saltoArriba = (transform.up) * jumpDistance;
			Debug.Log (saltoArriba);
			StartCoroutine (nuevoSalto(saltoArriba));
		}// fin del if
		}

	private IEnumerator nuevoSalto(Vector3 direccion){

		jumping = true;
		Vector3 startPoint = transform.position;
		Vector3 targetPoint = startPoint + direccion;
		Debug.Log (startPoint + direccion);
		Debug.Log (direccion);
		Debug.Log (startPoint);
		Debug.Log (targetPoint);
		float time = 0;
		float jumpProgress = 0;
		int contador = 0;
		while(jumping){
			jumpProgress = time / jumpDuration;
			if (jumpProgress > 1)
			{
				jumping = false;
				jumpProgress = 1;
			}
			//Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, jumpProgress);
			transform.Translate(Vector3.up*jumpDistance);
			Debug.Log (contador+"->"+targetPoint);
			contador++;
			time += Time.deltaTime;
			yield return null;
		}// fin del while
	}// fin de nuevoSalto

		private IEnumerator Jump(Vector3 direction)
		{
			Debug.Log (direction);
			jumping = true;
			Vector3 startPoint = transform.position;
			Vector3 targetPoint = startPoint + direction;
			Debug.Log (targetPoint);

			float time = 0;
			float jumpProgress = 0;
			float velocityY = jumpStartVelocityY;
			float height = startPoint.y;
		int contador = 0;

			while (jumping)
			{
				jumpProgress = time / jumpDuration;
			//Debug.Log (jumpProgress);

			if (jumpProgress > 1)
				{
				jumping = false;
				jumpProgress = 1;
				}

				Vector3 currentPos = Vector3.Lerp(startPoint, targetPoint, jumpProgress);
				Debug.Log (contador+"/"+currentPos);
				currentPos.y = height;
				transform.position = currentPos;
				contador++;
				//Wait until next frame.
				//yield return null;
				
				height += velocityY * Time.deltaTime;
				velocityY += Time.deltaTime * Physics.gravity.y;
				time += Time.deltaTime;
				
			yield return null;
				//Debug.Log ("tiem"+time);
			}

			transform.position = targetPoint;
			yield break;
		}
}

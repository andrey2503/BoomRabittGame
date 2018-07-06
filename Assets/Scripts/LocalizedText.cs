using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour {

    public string key;
    public static LocalizedText instance;
    // Use this for initialization


    void Start () 
    {
         if (instance == null) {
            instance = this;
        } 
		//else if (instance != this)
        //{
			//Debug.Log ("destruir " +gameObject.name);
          //  Destroy (gameObject);
        //}

        //Text text = GetComponent<Text> ();
        //Debug.Log(GameControl.instance.GetIsReady ());
        //text.text = GameControl.instance.GetLocalizedValue (key);
        //Debug.Log(GameControl.instance.GetLocalizedValue (key));
    }

    public void updateLang(){
        Text text = GetComponent<Text> ();
        Debug.Log(GameControl.instance.GetIsReady ());
        text.text = GameControl.instance.GetLocalizedValue (key);
        Debug.Log(GameControl.instance.GetLocalizedValue (key));
    }// end to updateLang


}
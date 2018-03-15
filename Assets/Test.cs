using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(1f);
	    Debug.Log(LocalizationManager.GetLocalizedValue("hi"));
	    Debug.Log(LocalizationManager.GetLocalizedValue("bye"));
	    yield return new WaitForSeconds(1f);
        LocalizationManager.LoadLocalizedText("Eng");
	    Debug.Log(LocalizationManager.GetLocalizedValue("hi"));
	    Debug.Log(LocalizationManager.GetLocalizedValue("bye"));
    }
}

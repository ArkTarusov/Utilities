using System.Collections;
using System.Collections.Generic;
using AiryCat.UtilitiesForUnity.Localization;
using AiryCat.UtilitiesForUnity.Utility;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		yield return new WaitForSeconds(1f);
	    Debug.Log("hi".Localized());
	    Debug.Log("bye".Localized());
	    yield return new WaitForSeconds(1f);
        LocalizationManager.LoadLocalizedText("Eng");
	    Debug.Log("hi".Localized());
	    Debug.Log("bye".Localized());
	}
}

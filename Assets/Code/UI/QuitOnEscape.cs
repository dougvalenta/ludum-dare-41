using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnEscape : MonoBehaviour {

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
	}

}

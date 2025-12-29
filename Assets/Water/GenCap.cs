using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenCap : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	public string pathName ="D:/1.png";
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			UnityEngine.ScreenCapture.CaptureScreenshot(pathName);
		}
	}
}

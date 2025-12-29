using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDepth : MonoBehaviour {
	public void Awake()
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}
}

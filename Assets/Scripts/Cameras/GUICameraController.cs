using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICameraController : MonoBehaviour {

    [SerializeField]
    Camera guiCamera;

    private void Awake()
    {
        guiCamera.orthographicSize = ( float )Screen.height / 2.0f;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

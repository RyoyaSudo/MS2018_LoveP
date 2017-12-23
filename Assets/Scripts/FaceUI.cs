using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUI : MonoBehaviour {

    [SerializeField] Sprite colorSp;
    [SerializeField] Sprite shadeSp;

    public bool IsColor { get{ return isColor; } set{ SetFaceUI(( value )); } }
    bool isColor;

    SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();   
    }

    // Use this for initialization
    void Start () {
        IsColor = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void SetFaceUI(bool flags)
    {
        isColor = flags;

        Sprite setSp;

        if (flags) setSp = colorSp;
        else setSp = shadeSp;

        renderer.sprite = setSp;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LpManager : MonoBehaviour {

    //Lpのロゴ
    [SerializeField]
    public TweenAnimation lpLogoObj;

    //ハートのひとまとまり
    [SerializeField]
    private TweenAnimation lpHearts;

    //LPとハートのひとまとまりs
    [SerializeField]
    private TweenAnimation lpScores;

    //[SerializeField]
    //private TweenAnimation lpHeart1_Obj;

    //[SerializeField]
    //private TweenAnimation lpHeart2_Obj;

    [SerializeField]
    private ScoreCtrl scoreObj;

    private bool lpFlag;
	// Use this for initialization
	void Start () {
        lpFlag = false;
        lpScores.Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (scoreObj.scoreFlag == true )
        {
            lpLogoObj.Play();
            lpHearts.Play();
            //lpFlag = true;
        }
        //else {
        //    lpFlag = false;
        //}
	}
}

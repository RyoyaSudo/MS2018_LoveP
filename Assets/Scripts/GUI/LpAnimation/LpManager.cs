using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LpManager : MonoBehaviour
{
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

    private bool lpAnimFlag;
    private bool heartsAnimFlag;

    // Use this for initialization
    void Start()
    {
        lpAnimFlag = lpLogoObj.GetComponent<LpAnimation_LpLogo>().lpLogoFlag;
        heartsAnimFlag = lpHearts.GetComponent<LpAnimation_Hearts>().heartsFlag;
        lpScores.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (scoreObj.scoreFlag == true 
            && lpLogoObj.GetComponent<LpAnimation_LpLogo>().lpLogoFlag == false
            && lpHearts.GetComponent<LpAnimation_Hearts>().heartsFlag == false)
        {
            lpLogoObj.Play();
            lpHearts.Play();
        }
    }
}


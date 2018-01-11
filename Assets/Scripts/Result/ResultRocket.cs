using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultRocket : MonoBehaviour {

    public bool blastFlag;
    public float moveSpeed;
    public GameObject transition;
    public Result resultObj;

    //エフェクト関係
    private EffectController effect;
    private ParticleSystem changeEffectObj;

    private void Awake()
    {
        blastFlag = false;
    }

    // Use this for initialization
    void Start () {
        //エフェクト関係
        effect = GameObject.Find("EffectManager").GetComponent<EffectController>();
        //ChangeEffectCreate();
        //changeEffectObj.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if( blastFlag )
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
            if( transform.position.y > 1000.0f)
            {
                transition.GetComponent<Transition>().StartTransition( null );
                resultObj.SetStateType( Result.State.STATE_SCORE );
                this.gameObject.SetActive(false);
            }
        }
	}

    public void RocketBlast()
    {
        if( resultObj.IsBlast == false )
        {
            return;
        }

        if( !blastFlag )
        {
            resultObj.portObj.SerialSendMessage();
        }

        blastFlag = true;
        
        //changeEffectObj.transform.localScale *= 100;
        //changeEffectObj.Play();
    }

    /// <summary>
    /// 変身エフェクト生成
    /// </summary>
    public void ChangeEffectCreate()
    {
        //生成
        changeEffectObj = effect.EffectCreate(EffectController.Effects.ROCKET_WAIT_EFFECT, gameObject.transform);

        //再生OFF
        var emissione = changeEffectObj.emission;
        emissione.enabled = true;

        //位置設定
        Vector3 pos;
        pos = changeEffectObj.transform.localPosition;
        pos.y = 0.0f;
        pos.z = -1.0f;
        changeEffectObj.transform.localPosition = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{

    //使用するエフェクトを追加
    public enum Effects
    {
        TEST_EFFECT0 = 0,
        TEST_EFFECT1 = 1,
        TEST_EFFECT2 = 2 ,
        CHARGE_EFFECT = 3 ,
        CHARGE_MAX_EFFECT = 4,
        SCORE_UP_EFFECT = 5
    }

    //エフェクト用の配列
    [SerializeField]
    private ParticleSystem[] effectArray;

    // エフェクトの生成
    public ParticleSystem EffectCreate(Effects type, Transform transform)
    {
        int num = (int)type; //引数で渡された番号をキャストして渡す
        ParticleSystem obj;
        obj = Instantiate(effectArray[num], transform.position, transform.rotation); //effect生成
        obj.transform.parent = transform; //生成時のエンティティと親子する
        return obj;
    }
}
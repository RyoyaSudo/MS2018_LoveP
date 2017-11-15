using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour {

    //アニメーション
    Animator anim;


    //状態
    public enum STATETYPE
    {
        CREATE,    // 生成
        READY,     // 待機
        EVADE,     // 回避
        RIDE,      // 乗車
        GETOFF,    // 下車
        TRANSPORT, // 運搬
        RELEASE,   // 解散
        DESTROY     // 消去
    };

    //宣言
    public STATETYPE stateType;     // 自身の状態管理要変数

    /// <summary>
    /// 待機時間用カウンタ。
    /// 10/24現在、この時間を元にオブジェクト消去判定を行うこともある。
    /// </summary>
    float destroyTimeCounter;
    public float destroyTime;

    // 初期化
    void Start ()
    {
        //状態を「生成」に
        stateType = STATETYPE.CREATE;

        anim = GetComponent<Animator>();

    }

    // 更新
    void Update ()
    {

        if (Input.GetKeyDown("1"))
        {
            //状態
            switch (stateType)
            {
                //待機
                case STATETYPE.READY:
                    stateType = STATETYPE.RIDE;
                    anim.SetBool("Ride" , true);
                    break;

                //回避
                case STATETYPE.EVADE:
                    break;

                //乗車
                case STATETYPE.RIDE:
                    stateType = STATETYPE.TRANSPORT;
                    anim.SetBool("Transport", true);
                    break;

                //下車
                case STATETYPE.GETOFF:
                    stateType = STATETYPE.RELEASE;
                    anim.SetBool("Release", true);
                    break;

                //運搬
                case STATETYPE.TRANSPORT:
                    stateType = STATETYPE.GETOFF;
                    anim.SetBool("Getoff", true);
                    break;

                //解散
                case STATETYPE.RELEASE:
                    break;

                //消去
                case STATETYPE.DESTROY:
                    break;
            }
        }











        //状態
        switch (stateType)
        {
            //生成
            case STATETYPE.CREATE:
                //状態を「待機」に
                stateType = STATETYPE.READY;
                break;

            //待機
            case STATETYPE.READY:

                break;

            //回避
            case STATETYPE.EVADE:
                break;

            //乗車
            case STATETYPE.RIDE:

                break;

            //下車
            case STATETYPE.GETOFF:
                break;

            //運搬
            case STATETYPE.TRANSPORT:
                break;

            //解散
            case STATETYPE.RELEASE:
                break;

            //消去
            case STATETYPE.DESTROY:
                Destroy(this.gameObject);
                break;
        }

    }
}

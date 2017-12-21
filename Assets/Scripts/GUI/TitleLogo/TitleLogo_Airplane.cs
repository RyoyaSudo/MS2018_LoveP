using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルロゴ_飛行機演出用スクリプト
/// </summary>
public class TitleLogo_Airplane : TweenAnimation
{
    [SerializeField] float moveTime;
    [SerializeField] float rotateTime;

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        MoveFront,
        RotateToBackIn,
        RotateToBackOut,
        MoveBack,
        RotateToFrontIn,
        RotateToFrontOut
    }

    // iTween用のハッシュテーブル各種
    Hashtable moveFrontHash;
    Hashtable scaleBackHash;
    Hashtable rotateBackInHash;
    Hashtable rotateBackOutHash;
    Hashtable moveBackHash;
    Hashtable scaleFrontHash;
    Hashtable rotateFrontInHash;
    Hashtable rotateFrontOutHash;

    // 初期値の保持
    Transform origin;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        moveFrontHash = new Hashtable()
        {
            { "x" , -480.0f } ,
            { "y" , -120.0f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToBackIn },
        };

        scaleBackHash = new Hashtable()
        {
            { "x" , -1.0f },
            { "time" , rotateTime * 2},
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },

        };


        rotateBackInHash = new Hashtable()
        {
            { "z" ,1.5f },
            { "x" , -60.0f},
            { "y ", -20.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeOutCubic},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToBackOut },
        };

        rotateBackOutHash = new Hashtable()
        {
            { "z" , 2.5f },
            { "x" , 60.0f},
            { "y ", 20.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeInCubic},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveBack },
        };

        moveBackHash = new Hashtable()
        {
            { "x" , 480.0f } ,
            { "y" , 120.0f },
            { "time", moveTime },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToFrontIn },
        };

        scaleFrontHash = new Hashtable()
        {
            { "x" , -1.0f },
            { "time" , rotateTime * 2 },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },

        };

        rotateFrontInHash = new Hashtable()
        {
            { "z" , -3.0f },
            { "x" , 60.0f},
            { "y ", 20.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeOutCubic },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToFrontOut },
        };

        rotateFrontOutHash = new Hashtable()
        {
            { "z" , -1.0f },
            { "x" , -60.0f},
            { "y ", -20.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeInCubic },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveFront },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState( State.MoveFront );
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    public override void Pause()
    {
        SetState( State.Pause );
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public override void Stop()
    {
        SetState( State.Stop );
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected override void Init()
    {
        transform.position = origin.position;
        transform.rotation = origin.rotation;
        transform.localScale = origin.localScale;
    }

    /// <summary>
    /// 状態設定処理
    /// </summary>
    /// <param name="param">設定したい状態</param>
    void SetState( State param )
    {
        switch (param)
        {
            case State.Stop:
                iTween.Stop(gameObject);
                break;

            case State.Pause:
                iTween.Pause(gameObject);
                break;

            case State.MoveFront:
                iTween.MoveBy(gameObject, moveFrontHash);
                break;

            case State.RotateToBackIn:
                iTween.ScaleBy(gameObject, scaleBackHash);
                iTween.MoveBy(gameObject, rotateBackInHash);
                break;

            case State.RotateToBackOut:
                iTween.MoveBy(gameObject, rotateBackOutHash);
                break;

            case State.MoveBack:
                iTween.MoveBy(gameObject, moveBackHash);
                break;

            case State.RotateToFrontIn:
                iTween.ScaleBy(gameObject, scaleFrontHash);
                iTween.MoveBy(gameObject, rotateFrontInHash);
                break;

            case State.RotateToFrontOut:
                iTween.MoveBy(gameObject, rotateFrontOutHash);
                break;
        }
    }
}

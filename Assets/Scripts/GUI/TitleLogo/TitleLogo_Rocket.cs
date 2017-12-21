using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogo_Rocket : TweenAnimation
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

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        moveFrontHash = new Hashtable()
        {
            { "x" , -400.0f } ,
            { "y" , -70.0f },
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
            { "z" , 2.0f },
            { "x" , -90.0f},
            { "y ", -30.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeOutCubic},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToBackOut },
        };

        rotateBackOutHash = new Hashtable()
        {
            { "z" , 2.0f },
            { "x" , 90.0f},
            { "y ", 30.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeInCubic},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveBack },
        };

        moveBackHash = new Hashtable()
        {
            { "x" , 400.0f } ,
            { "y" , 70.0f },
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
            { "z" , -2.0f },
            { "x" , 90.0f},
            { "y ", 30.0f },
            { "time" , rotateTime },
            { "easetype" , iTween.EaseType.easeOutCubic },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToFrontOut },
        };

        rotateFrontOutHash = new Hashtable()
        {
            { "z" , -2.0f },
            { "x" , -90.0f},
            { "y ", -30.0f },
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
        SetState(State.MoveBack);
    }

    /// <summary>
    /// 一時停止処理
    /// </summary>
    public override void Pause()
    {
        SetState(State.Pause);
    }

    /// <summary>
    /// 停止処理
    /// </summary>
    public override void Stop()
    {
        SetState(State.Stop);
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
    void SetState(State param)
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
                Debug.Log(param);
                iTween.MoveBy(gameObject, moveFrontHash);
                break;

            case State.RotateToBackIn:
                Debug.Log(param);
                iTween.ScaleBy(gameObject, scaleBackHash);
                iTween.MoveBy(gameObject, rotateBackInHash);
                break;
            case State.RotateToBackOut:
                Debug.Log(param);
                iTween.MoveBy(gameObject, rotateBackOutHash);
                break;

            case State.MoveBack:
                Debug.Log(param);
                iTween.MoveBy(gameObject, moveBackHash);
                break;

            case State.RotateToFrontIn:
                Debug.Log(param);
                iTween.ScaleBy(gameObject, scaleFrontHash);
                iTween.MoveBy(gameObject, rotateFrontInHash);
                break;
            case State.RotateToFrontOut:
                Debug.Log(param);
                iTween.MoveBy(gameObject, rotateFrontOutHash);
                break;
        }
    }
}

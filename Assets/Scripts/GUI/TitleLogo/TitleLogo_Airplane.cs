﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルロゴ_飛行機演出用スクリプト
/// </summary>
public class TitleLogo_Airplane : TweenAnimation
{
    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        MoveFront,
        RotateToBack,
        MoveBack,
        RotateToFront,
    }

    // iTween用のハッシュテーブル各種
    Hashtable moveFrontHash;
    Hashtable scaleBackHash;
    Hashtable rotateBackHash;
    Hashtable moveBackHash;
    Hashtable scaleFrontHash;
    Hashtable rotateFrontHash;

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
            { "x" , -640.0f } ,
            { "y" , -360.0f },
            { "time", 8.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToBack },
        };

        scaleBackHash = new Hashtable()
        {
            { "x" , -1.0f },
            { "time" , 1.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveBack },
        };

        rotateBackHash = new Hashtable()
        {
            { "z" , 4.0f },
            { "time" , 1.0f },
            { "easetype" , iTween.EaseType.linear },
        };

        moveBackHash = new Hashtable()
        {
            { "x" , 640.0f } ,
            { "y" , 360.0f },
            { "time", 8.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateToFront },
        };

        scaleFrontHash = new Hashtable()
        {
            { "x" , -1.0f },
            { "time" , 1.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveFront },
        };

        rotateFrontHash = new Hashtable()
        {
            { "z" , -4.0f },
            { "time" , 1.0f },
            { "easetype" , iTween.EaseType.linear },
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
        switch( param )
        {
            case State.Stop:
                iTween.Stop( gameObject );
                break;

            case State.Pause:
                iTween.Pause( gameObject );
                break;

            case State.MoveFront:
                iTween.MoveBy( gameObject , moveFrontHash );
                break;

            case State.RotateToBack:
                iTween.ScaleBy( gameObject , scaleBackHash );
                iTween.MoveBy( gameObject , rotateBackHash );
                break;

            case State.MoveBack:
                iTween.MoveBy( gameObject , moveBackHash );
                break;

            case State.RotateToFront:
                iTween.ScaleBy( gameObject , scaleFrontHash );
                iTween.MoveBy( gameObject , rotateFrontHash );
                break;
        }
    }

    /// <summary>
    /// 地球表面移動
    /// </summary>
    void MoveFront()
    {
        iTween.MoveTo( gameObject , moveFrontHash );
    }

    /// <summary>
    /// 裏面に向けてスケーリングしつつZ軸移動
    /// </summary>
    void RotateToBack()
    {
        iTween.ScaleBy( gameObject , scaleBackHash );
    }
}

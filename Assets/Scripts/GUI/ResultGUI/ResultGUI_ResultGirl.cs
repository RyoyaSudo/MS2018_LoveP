﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultGUI_ResultGirl : TweenAnimation
{

    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        RotateRight,
        RotateLeft,
    }

    // iTween用のハッシュテーブル各種
    Hashtable RotateRightHash;
    Hashtable RotateLeftHash;

    // 初期値の保持
    Transform origin;

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        RotateRightHash = new Hashtable()
        {
            //{ "x" , -480.0f } ,
            { "z" , -20.1f },
            { "time", 1.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateLeft },
        };

        RotateLeftHash = new Hashtable()
        {
            //{ "x" , -480.0f } ,
            { "z" , 20.0f },
            { "time", 1.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none },
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.RotateRight },
        };
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(State.RotateLeft);
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

            case State.RotateRight:
                //iTween.MoveBy(gameObject, moveDownHash);
                iTween.RotateTo(gameObject, RotateRightHash);
                break;
            case State.RotateLeft:
                iTween.RotateTo(gameObject, RotateLeftHash);
                break;
        }
    }
}
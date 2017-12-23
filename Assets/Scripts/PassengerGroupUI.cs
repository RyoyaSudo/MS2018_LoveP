using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerGroupUI : TweenAnimation
{
    [SerializeField] float moveTime;


    // 状態変数の列挙値
    enum State
    {
        Stop,
        Pause,
        Start,
        MoveUp,
        MoveDown
    }

    // iTween用のハッシュテーブル各種
    Hashtable startHash;
    Hashtable moveUpHash;
    Hashtable moveDownHash;
    Hashtable scaleUpHash;
    Hashtable scaleDownHash;

    // 初期値の保持
    Transform origin;

    Camera mainCamera;

    private void Awake()
    {
        origin = transform;

        // 各ハッシュの初期化
        startHash = new Hashtable()
        {
            { "y" , 10.0f } ,
            { "time", 10.0f },
            { "easetype" , iTween.EaseType.linear },
            { "loopType" , iTween.LoopType.none},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.Start },
        };

        moveUpHash = new Hashtable()
        {
            { "y" , 2.0f } ,
            { "time", 2.0f },
            { "easetype" , iTween.EaseType.easeInOutQuad },
            { "loopType" , iTween.LoopType.none},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveDown },
        };

        moveDownHash = new Hashtable()
        {
            { "y" , -2.0f } ,
            { "time", 2.0f },
            { "easetype" , iTween.EaseType.easeInOutQuad },
            { "loopType" , iTween.LoopType.none},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveUp },
        };

        scaleUpHash = new Hashtable()
        {
            { "x" , 1.05f },
            { "y" , 1.05f },
            { "time", 2.0f },
            { "easetype" , iTween.EaseType.easeInOutQuad },
            { "loopType" , iTween.LoopType.none},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveDown },
        };

        scaleDownHash = new Hashtable()
        {
            { "x" , 0.95f },
            { "y" , 0.95f },
            { "time", 2.0f },
            { "easetype" , iTween.EaseType.easeInOutQuad },
            { "loopType" , iTween.LoopType.none},
            { "oncompletetarget" , gameObject },
            { "oncomplete" , "SetState" },
            { "oncompleteparams" , State.MoveUp },
        };

    }

    // 初期化
    void Start ()
    {
        Play();
	}
	
	//更新
	void Update ()
    {
        if( !mainCamera.enabled )
        {
            return;
        }

        Vector3 p = mainCamera.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }

    /// <summary>
    /// 再生処理
    /// </summary>
    public override void Play()
    {
        Init();
        SetState(State.MoveUp);
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

            case State.Start:
                iTween.RotateBy(gameObject, startHash);
                //iTween.PunchScale(gameObject, startHash);
                break;
            case State.MoveUp:
                iTween.MoveBy(gameObject, moveUpHash);
                iTween.ScaleTo(gameObject, scaleUpHash);
                break;
            case State.MoveDown:
                iTween.MoveBy(gameObject, moveDownHash);
                iTween.ScaleTo(gameObject, scaleDownHash);
                break;
        }
    }
}

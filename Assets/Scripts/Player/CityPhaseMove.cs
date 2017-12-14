using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 街フェイズ時のプレイヤーの移動制御用クラス
/// </summary>
public class CityPhaseMove : MonoBehaviour {

    #region 変数宣言

    /// <summary>
    /// プレイヤーオブジェクト。
    /// Player.csで管理しているものを処理する際に利用。
    /// </summary>
    Player playerObj;
    [SerializeField] string playerObjPath;

    /// <summary>
    /// ゲームシーン管理オブジェクト。
    /// 状態を確認し、それに応じた処理をするために利用。
    /// </summary>
    Game gameObj;
    [SerializeField] string gameObjPath;

    /// <summary>
    /// 移動処理有効化フラグ。Player.csで制御してもらう。
    /// </summary>
    public bool IsEnable { get { return isEnable; } set { SetEnable( value ); } }
    private bool isEnable;

    /// <summary>
    /// 旋回力
    /// </summary>
    public float turnPower;
    public float turnPowerPush;

    /// <summary>
    /// 現在のY軸の旋回力。
    /// </summary>
    float moveRadY;
    // HACK: 2017/11/22 moveRadYは現在、Y軸の角度そのものの値になってるため、後に回転に慣性を付ける際に変更する。

    /// <summary>
    /// 移動量(スカラー)
    /// </summary>
    public float Velocity { get; private set; }

    /// <summary>
    /// 速度限界値
    /// </summary>
    [SerializeField] float velocityMax;

    /// <summary>
    /// 移動量ベクトル
    /// </summary>
    public Vector3 VelocityVec { get; private set; }

    /// <summary>
    /// 前回フレームの移動量ベクトル
    /// </summary>
    Vector3 velocityVecOld;

    /// <summary>
    /// 初速
    /// </summary>
    [SerializeField] float initialVelocity;

    /// <summary>
    /// 加速度
    /// </summary>
    [SerializeField] float acceleration;

    /// <summary>
    /// 停車までの予定時間(単位:秒)
    /// </summary>
    [SerializeField] float stoppingTime;

    /// <summary>
    /// 停車にかける力の倍率
    /// </summary>
    [SerializeField] float stoppingRate;

    /// <summary>
    /// 停車時のデッドゾーンの値。
    /// この値以下になった場合、完全停車 = 移動量を0 にする
    /// </summary>
    [SerializeField] float stoppingDeadZone;

    /// <summary>
    /// 停車力。初期化時に算出する。
    /// </summary>
    float stoppingPower;

    /// <summary>
    /// プッシュ動作時のチャージ量の現在値
    /// </summary>
    float pushCharge;

    /// <summary>
    /// チャージ限界量
    /// </summary>
    [SerializeField] private float chargeMax;

    /// <summary>
    /// チャージマックス状態の判定フラグ
    /// </summary>
    private bool isChargeMax;

    /// <summary>
    /// チャージブースト継続時間
    /// </summary>
    [SerializeField] float boostDuration;

    /// <summary>
    /// チャージブースト時間計測変数
    /// </summary>
    float boostTimer;

    /// <summary>
    /// チャージブースト時の速度倍率
    /// </summary>
    [SerializeField] float boostVelocityRate;

    /// <summary>
    /// 通常時の速度倍率
    /// </summary>
    [SerializeField] float defaultVelocityRate;

    /// <summary>
    /// 現在の速度倍率
    /// </summary>
    float velocityRate;

    /// <summary>
    /// 重力量。Playerは個別に設定する。
    /// </summary>
    [SerializeField] Vector3 gravity;

    /// <summary>
    /// 現在の重力量の累計
    /// </summary>
    Vector3 curGravity;

    /// <summary>
    /// 前回フレーム時の位置。重力計算などに参照する。
    /// </summary>
    Vector3 oldPos;

    /// <summary>
    /// プレイヤー移動に用いるUnityコンポーネント
    /// </summary>
    CharacterController controller;

    /// <summary>
    /// CharacterControllerコンポーネントのHierarchy上のパス
    /// </summary>
    public string controllerPath;

    /// <summary>
    /// 独自デバイス入力処理オブジェクト
    /// </summary>
    private LoveP_Input inputObj;
    [SerializeField] string inputObjPath;

    #endregion

    /// <summary>
    /// 生成時処理
    /// </summary>
    private void Awake()
    {
        // 初期化系
        Velocity = 0.0f;
        pushCharge = 0.0f;
        isChargeMax = false;
        velocityRate = defaultVelocityRate;
        boostTimer = 0.0f;
        isEnable = false;
        VelocityVec = Vector3.zero;
        velocityVecOld = Vector3.zero;
        inputObj = null;
        gameObj = null;

        // 算出系
        stoppingPower = stoppingTime == 0.0f ? 1.0f : ( 1.0f / stoppingTime ) * stoppingRate;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start () {
        // 初期化系
        oldPos = transform.position;

        // シーン内から必要なオブジェクトを取得
        playerObj  = GameObject.Find( playerObjPath ).GetComponent<Player>();
        inputObj   = GameObject.Find( inputObjPath ).GetComponent<LoveP_Input>();
        gameObj    = GameObject.Find( gameObjPath ).GetComponent<Game>();
        controller = GameObject.Find( controllerPath ).GetComponent<CharacterController>();

        // HACK: 初期化時に移動しておく
        //       移動しないと開始時に埋まった状態になってしまうため、少し移動してCharcterControllerで所定の高さに移動してもらう
        controller.Move( Vector3.forward );
        controller.Move( Vector3.zero );
        controller.enabled = isEnable;
        //controller.detectCollisions = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update () {
        if( IsEnable )
        {
            CityMoveCharcterController();
        }
        else
        {
            if( gameObj.PhaseParam == Game.Phase.GAME_PAHSE_CITY )
            {
                // HACK: 移動無効化時に行うべきこと
                //       CharcterControllerを利用している場合、移動ベクトルを0にしなくてはならないらしい(検証不十分)
                //       それに伴い、重力値が反映されているか怪しいため、あとでキチンと調査すること。
                controller.Move( Vector3.zero );
            }
        }
    }

    /// <summary>
    /// CharcterControllerを用いたプレイヤーの移動処理。
    /// </summary>
    public void CityMoveCharcterController()
    {
        float moveV = inputObj.GetAxis( "Vertical" );
        float moveH = inputObj.GetAxis( "Horizontal" );
        bool isPush = Input.GetKey( KeyCode.Space ) || inputObj.GetButton( "Fire1" );

        //プッシュ時と通常時で旋回力を分ける
        if( isPush )
        {
            moveH *= turnPowerPush;
        }
        else
        {
            moveH *= turnPower;
        }

        // TODO: バイク旋回
        //       Z軸回転させるため
        //float radZ = -moveH * 45.0f;
        float radZ = 0.0f;

        // 旋回処理
        if( Mathf.Abs( moveH ) > 0.2f )
        {
            moveRadY += moveH * 180.0f * Time.deltaTime;

            transform.rotation = Quaternion.Euler( transform.rotation.x , moveRadY , transform.rotation.z + radZ );
        }

        // HACK: 地上の速度演算
        if( !isPush )
        {
            Velocity = Mathf.Max( Velocity , initialVelocity );
            Velocity += acceleration * Time.deltaTime;
        }

        // プッシュ動作
        if( isPush )
        {
            Velocity += ( ( 0.0f - Velocity ) * stoppingPower * Time.deltaTime );

            // デッドゾーン確認
            if( Velocity < stoppingDeadZone )
            {
                Velocity = 0.0f;
                playerObj.IsStopped = true;
            }

            //チャージエフェクト再生
            if( pushCharge == 0 )
            {
                var emission = playerObj.ChargeEffectObj.emission;
                emission.enabled = true;
            }

            //チャージがマックスになったら
            // HACK: チャージマックスの判定式
            //       もっときれいに、わかりやすく、エレガントに修正
            if( pushCharge >= chargeMax )
            {
                //チャージマックスエフェクト再生
                if( !isChargeMax )
                {
                    //チャージエフェクト停止
                    var emission = playerObj.ChargeEffectObj.emission;
                    emission.enabled = false;

                    //チャージマックスエフェクト再生
                    emission = playerObj.ChargeMaxEffectObj.emission;
                    emission.enabled = true;

                    isChargeMax = true;
                }
            }

            pushCharge += Time.deltaTime;
        }

        // 速度倍率の変更
        if( boostTimer > 0.0f )
        {
            velocityRate = boostVelocityRate;
            boostTimer -= Time.deltaTime;
        }
        else
        {
            velocityRate = defaultVelocityRate;
        }

        // プッシュ解放した後のダッシュ
        if( !isPush )
        {
            if( isChargeMax )
            {
                //チャージマックスエフェクト停止
                var emission = playerObj.ChargeMaxEffectObj.emission;
                emission.enabled = false;
                isChargeMax = false;
            }
            else
            {
                //チャージエフェクト停止
                var emissione = playerObj.ChargeEffectObj.emission;
                emissione.enabled = false;

            }

            // ブースト時間を与える
            if( pushCharge >= chargeMax )
            {
                boostTimer = boostDuration;
            }

            pushCharge = 0;
            playerObj.IsStopped = false;
        }

        // 今回の速度加算
        Velocity = Mathf.Min( Velocity , velocityMax ) * velocityRate;
        VelocityVec = Velocity * transform.forward * Time.deltaTime;

        // TODO: 画面外に落ちたときの処理
        //       仮で追加
        if( transform.position.y < -50.0f )
        {
            Vector3 newPos = transform.position;
            newPos.y = 30.0f;
            transform.position = newPos;
        }

        // TODO: 重力計算
        //       Unity内蔵のものだと重力のかかりが弱いので、自前で計算する。
        curGravity += ( gravity * Time.deltaTime );

        controller.Move( curGravity );
        //rb.AddForce( curGravity , ForceMode.Acceleration );

        if( transform.position.y == oldPos.y )
        {
            curGravity = Vector3.zero;
            //Debug.Log( "Gravity Reset" );
        }

        // TODO: ジャンプ処理
        //       デバッグ時に活用できそうなので実装
        if( Input.GetKey( KeyCode.J ) )
        {
            curGravity = Vector3.zero;

            Vector3 jumpForce = -gravity * 2.0f;
            controller.Move( jumpForce * Time.deltaTime );
            //rb.AddForce( jumpForce , ForceMode.Acceleration );
        }

        // 移動量の反映
        controller.Move( VelocityVec );

        // 過去位置を保存しておく
        oldPos = transform.position;
    }

    /// <summary>
    /// 有効化フラグ設定時の諸処理
    /// </summary>
    /// <param name="flags"></param>
    private void SetEnable( bool flags )
    {
        isEnable = flags;

        if( controller != null ) controller.enabled = flags;
    }

    /// <summary>
    /// OnGUI処理
    /// 主にデバッグ情報を出す
    /// </summary>
    private void OnGUI()
    {
        if( Game.IsOnGUIEnable )
        {
            GUIStyleState styleState;
            styleState = new GUIStyleState();
            styleState.textColor = Color.white;

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 48;
            guiStyle.normal = styleState;

            string str = "";
            //str = "速度ベクトル:" + VelocityVec + "\n速度量:" + VelocityVec.magnitude + "\nフレーム間速度:" + Velocity;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}

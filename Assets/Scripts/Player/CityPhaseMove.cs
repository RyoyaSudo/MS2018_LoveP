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
    /// 移動量ベクトル
    /// </summary>
    private Vector3 velocityVec;

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
    public string controllerPath;

    /// <summary>
    /// 独自デバイス入力処理オブジェクト
    /// </summary>
    private LoveP_Input inputObj;
    [SerializeField] string inputObjPath;

    /// <summary>
    /// 当たり判定
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// 当たり判定実体用ゲームオブジェクト
    /// </summary>
    [SerializeField] GameObject modelObj;

    /// <summary>
    /// ブースト動作を行ったか
    /// </summary>
    public bool IsBoost { get; private set; }
    public bool IsBoostOld { get; private set; }
    private bool isBoostTrigger;
    private bool isBoostRelese;

    /// <summary>
    /// ブースト開始時の初速度倍率
    /// </summary>
    [ SerializeField] float boostImpactRate;

    /// <summary>
    /// ブースト起動中の加速度
    /// </summary>
    [SerializeField] float boostAccelerationRate;

    /// <summary>
    /// 重力速度ベクトル
    /// </summary>
    Vector3 gravityVec;

    /// <summary>
    /// 速度限界値
    /// </summary>
    [SerializeField] float velocityMax;
    [SerializeField] float velocityBoostMax;

    public float VelocityMax { get { return velocityMax; } }

    /// <summary>
    /// 地面判断の際にレイキャストする対象のレイヤー群
    /// </summary>
    [SerializeField] string[] groundCheckRaycastLayerName;

    /// <summary>
    /// 地上の摩擦抵抗値。
    /// </summary>
    [SerializeField] [Range( 0f , 1f )] float groundFriction;

    /// <summary>
    /// 速度ベクトルの補正用補間値
    /// </summary>
    [SerializeField] [Range( 0f , 1f )] float defaultHandlingInterpolate;
    [SerializeField] [Range( 0f , 1f )] float boostHandlingInterpolate;

    /// <summary>
    /// ブレーキしきい値
    /// </summary>
    [SerializeField] [Range( -1.5f , 1.5f )] float brakeBiasMin;
    [SerializeField] [Range( -1.5f , 1.5f )] float brakeBiasMax;

    [SerializeField] bool isDebugBrake;

    /// <summary>
    /// 地上接地判定
    /// </summary>
    public bool IsGround { get; set; }
    bool isGroundOld;

    /// <summary>
    /// 開始位置
    /// </summary>
    Transform spawnPoint;
    [SerializeField] string spawnPointPath;

    float originPosY;

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
        velocityVec = Vector3.zero;
        velocityVecOld = Vector3.zero;
        inputObj = null;
        gameObj = null;
        IsBoost = false;
        IsBoostOld = false;
        isBoostTrigger = false;
        isBoostRelese = false;
        gravityVec = Vector3.zero;
        IsGround = false;
        isGroundOld = false;

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
        spawnPoint = GameObject.Find( spawnPointPath ).GetComponent<Transform>();

        rb = gameObject.GetComponent<Rigidbody>();

        // 初期位置設定
        Vector3 eular = spawnPoint.eulerAngles;

        moveRadY = eular.y;

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // HACK: 初期化時に移動しておく
        //       移動しないと開始時に埋まった状態になってしまうため、少し移動してCharcterControllerで所定の高さに移動してもらう
        controller.Move( Vector3.forward );
        controller.Move( Vector3.zero );
        controller.enabled = isEnable;
        controller.detectCollisions = false;
        //controller.enabled = false;

        originPosY = transform.position.y;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update () {
        if( IsEnable )
        {
            //CityMoveCharcterController();

            CityMoveCharcterControllerNew();
        }
        else
        {
            if( gameObj.PhaseParam == Game.Phase.GAME_PAHSE_CITY )
            {
                // HACK: 移動無効化時に行うべきこと
                //       CharcterControllerを利用している場合、移動ベクトルを0にしなくてはならないらしい(検証不十分)
                //       それに伴い、重力値が反映されているか怪しいため、あとでキチンと調査すること。
                if( controller.enabled )
                {
                    controller.Move( Vector3.zero );
                }
            }
        }

        rb.WakeUp();
    }

    /// <summary>
    /// CharcterControllerを用いたプレイヤーの移動処理。
    /// </summary>
    public void CityMoveCharcterControllerNew()
    {
        // 入力情報取得
        float moveV  = inputObj.GetAxis( "Vertical" );
        float moveH  = inputObj.GetAxis( "Horizontal" );

        IsBoost = Input.GetKey( KeyCode.Space ) || inputObj.GetButton( "Fire1" );
        isBoostTrigger = IsBoost & !IsBoostOld;
        isBoostRelese  = !IsBoost & IsBoostOld;

        float brakeRate = moveV + inputObj.Device_V_Default;
        bool isBrake = ( brakeRate > brakeBiasMin ) && ( brakeRate < brakeBiasMax ) ? true : false;

        // 旋回処理
        moveRadY += moveH * 180.0f * Time.deltaTime;
        transform.rotation = Quaternion.Euler( transform.rotation.x , moveRadY , transform.rotation.z );

        // 地面との当たり判定
        //int layerMask = LayerMask.GetMask( groundCheckRaycastLayerName );
        //RaycastHit hitInfo;
        //Vector3 upV = Vector3.up;
        //
        //if( Physics.Raycast( transform.position + ( transform.up * 0.1f ) , -transform.up , out hitInfo , 0.5f , layerMask ) )
        //{
        //    // 接した場合
        //    upV = hitInfo.normal;
        //    isGround = true;
        //}

        // HACK: 姿勢修正
        //       こだわる場合は実装
        //Quaternion targetQuatanion = Quaternion.FromToRotation( Vector3.up , upV );
        //float qT = Time.deltaTime * 10.0f;
        //
        //transform.rotation = Quaternion.Slerp( targetQuatanion , transform.rotation , qT );
        //Vector3 afterForwardV = Vector3.ProjectOnPlane( transform.forward , upV );
        //Quaternion targetQuatanion = Quaternion.LookRotation( afterForwardV , upV );
        //float qT = Time.deltaTime * 10.0f;

        // 加速度演算
        Vector3 accV = transform.rotation * Vector3.forward;
        accV *= acceleration;

        // 速度演算
        velocityVec += accV * Time.deltaTime;

        // 初速演算
        if( isBrake == false )
        {
            float velocityForce = velocityVec.magnitude;
            Vector3 velocityDir = velocityVec.normalized;

            velocityVec = Mathf.Max( velocityForce , initialVelocity ) * velocityDir;
        }

        // ブレーキ処理
        if( isBrake )
        {
            velocityVec *= isDebugBrake ? 0.0f : groundFriction;

            if( velocityVec.magnitude < stoppingDeadZone )
            {
                velocityVec = Vector3.zero;
                playerObj.IsStopped = true;
            }
        }
        else
        {
            playerObj.IsStopped = false;
        }

        // ハンドリングに合わせて速度ベクトルを補正
        Vector3 targetVel = transform.rotation * Vector3.forward;
        float t = IsBoost ? boostHandlingInterpolate : defaultHandlingInterpolate;

        velocityVec = Vector3.Lerp( velocityVec.normalized , targetVel , t ) * velocityVec.magnitude;

        // ブーストON・OFF時の移動ベクトル変換
        if( isBoostTrigger || isBoostRelese )
        {
            float force = velocityVec.magnitude;
            velocityVec = transform.rotation * Vector3.forward;
            velocityVec *= force;
        }

        // ブースト処理
        if( isBoostTrigger )
        {
            velocityVec *= boostImpactRate;
        }

        if( IsBoost )
        {
            velocityVec = velocityVec * ( 1.0f + ( boostAccelerationRate * Time.deltaTime ) );
        }

        // 重力処理
        gravityVec += gravity * Time.deltaTime;

        if( IsGround || isGroundOld || true )
        {
            gravityVec = Vector3.zero;
        }

        // TODO: ジャンプ処理
        //       デバッグ時に活用できそうなので実装
        if( Input.GetKey( KeyCode.J ) )
        {
            gravityVec = gravity * -2.0f;
        }

        // 速度制限
        {
            float velocityForce = velocityVec.magnitude;
            float velocityLimit = IsBoost ? velocityBoostMax : velocityMax;
            Vector3 velocityDir = velocityVec.normalized;

            velocityVec = Mathf.Min( velocityForce , velocityLimit ) * velocityDir;
        }

        // 移動量の反映
        Vector3 curMoveV = ( velocityVec + gravityVec ) * Time.deltaTime;

        controller.Move( curMoveV );
        //rb.AddForce( ( velocityVec + gravityVec ) , ForceMode.Acceleration );
        rb.velocity = ( velocityVec + gravityVec );
        //rb.AddForce( ( accV + gravityVec ) , ForceMode.Acceleration );
        
        // 過去情報を保存しておく
        oldPos = transform.position;
        IsBoostOld = IsBoost;

        Velocity = velocityVec.magnitude;

        Vector3 defaultPos = transform.position;
        defaultPos.y = originPosY;

        transform.position = defaultPos;
    }

    /// <summary>
    /// CharcterControllerを用いたプレイヤーの移動処理。
    /// </summary>
    //　HACK: 街フェイズ移動プログラム(旧式)
    //        この関数は旧式のため使用しない。
    //        一応残しておく程度
    //public void CityMoveCharcterController()
    //{
    //    // 入力情報取得
    //    float moveV = inputObj.GetAxis( "Vertical" );
    //    float moveH = inputObj.GetAxis( "Horizontal" );
    //    bool IsBoost = Input.GetKey( KeyCode.Space ) || inputObj.GetButton( "Fire1" );
    //
    //    //プッシュ時と通常時で旋回力を分ける
    //    if( IsBoost ) moveH *= turnPowerPush;
    //    else         moveH *= turnPower;
    //
    //    // TODO: バイク旋回
    //    //       Z軸回転させるため
    //    //float radZ = -moveH * 45.0f;
    //    float radZ = 0.0f;
    //
    //    // 旋回処理
    //    if( Mathf.Abs( moveH ) > 0.2f )
    //    {
    //        moveRadY += moveH * 180.0f * Time.deltaTime;
    //
    //        transform.rotation = Quaternion.Euler( transform.rotation.x , moveRadY , transform.rotation.z + radZ );
    //    }
    //
    //    // HACK: 地上の速度演算
    //    if( !IsBoost )
    //    {
    //        // 加速度付与
    //        Vector3 accV = Vector3.forward * acceleration;
    //        accV = transform.rotation * accV;
    //
    //        Velocity = Mathf.Max( Velocity , initialVelocity );
    //        Velocity += acceleration * Time.deltaTime;
    //
    //    }
    //
    //    // プッシュ動作
    //    if( IsBoost )
    //    {
    //        Velocity += ( ( 0.0f - Velocity ) * stoppingPower * Time.deltaTime );
    //
    //        // デッドゾーン確認
    //        if( Velocity < stoppingDeadZone )
    //        {
    //            Velocity = 0.0f;
    //            playerObj.IsStopped = true;
    //        }
    //
    //        //チャージエフェクト再生
    //        if( pushCharge == 0 )
    //        {
    //            var emission = playerObj.ChargeEffectObj.emission;
    //            emission.enabled = true;
    //        }
    //
    //        //チャージがマックスになったら
    //        // HACK: チャージマックスの判定式
    //        //       もっときれいに、わかりやすく、エレガントに修正
    //        if( pushCharge >= chargeMax )
    //        {
    //            //チャージマックスエフェクト再生
    //            if( !isChargeMax )
    //            {
    //                //チャージエフェクト停止
    //                var emission = playerObj.ChargeEffectObj.emission;
    //                emission.enabled = false;
    //
    //                //チャージマックスエフェクト再生
    //                emission = playerObj.ChargeMaxEffectObj.emission;
    //                emission.enabled = true;
    //
    //                isChargeMax = true;
    //            }
    //        }
    //
    //        pushCharge += Time.deltaTime;
    //    }
    //
    //    // 速度倍率の変更
    //    if( boostTimer > 0.0f )
    //    {
    //        velocityRate = boostVelocityRate;
    //        boostTimer -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        velocityRate = defaultVelocityRate;
    //    }
    //
    //    // プッシュ解放した後のダッシュ
    //    if( !IsBoost )
    //    {
    //        if( isChargeMax )
    //        {
    //            //チャージマックスエフェクト停止
    //            var emission = playerObj.ChargeMaxEffectObj.emission;
    //            emission.enabled = false;
    //            isChargeMax = false;
    //        }
    //        else
    //        {
    //            //チャージエフェクト停止
    //            var emissione = playerObj.ChargeEffectObj.emission;
    //            emissione.enabled = false;
    //
    //        }
    //
    //        // ブースト時間を与える
    //        if( pushCharge >= chargeMax )
    //        {
    //            boostTimer = boostDuration;
    //        }
    //
    //        pushCharge = 0;
    //        playerObj.IsStopped = false;
    //    }
    //
    //    // 今回の速度加算
    //    Velocity = Mathf.Min( Velocity , velocityMax ) * velocityRate;
    //    velocityVec = Velocity * transform.forward * Time.deltaTime;
    //
    //    // TODO: 画面外に落ちたときの処理
    //    //       仮で追加
    //    if( transform.position.y < -50.0f )
    //    {
    //        Vector3 newPos = transform.position;
    //        newPos.y = 30.0f;
    //        transform.position = newPos;
    //    }
    //
    //    // TODO: 重力計算
    //    //       Unity内蔵のものだと重力のかかりが弱いので、自前で計算する。
    //    curGravity += ( gravity * Time.deltaTime );
    //
    //    controller.Move( curGravity );
    //
    //    if( transform.position.y == oldPos.y )
    //    {
    //        curGravity = Vector3.zero;
    //        //Debug.Log( "Gravity Reset" );
    //    }
    //
    //    // TODO: ジャンプ処理
    //    //       デバッグ時に活用できそうなので実装
    //    if( Input.GetKey( KeyCode.J ) )
    //    {
    //        curGravity = Vector3.zero;
    //
    //        Vector3 jumpForce = -gravity * 2.0f;
    //        controller.Move( jumpForce * Time.deltaTime );
    //    }
    //
    //    // 移動量の反映
    //    controller.Move( velocityVec );
    //
    //    // 過去位置を保存しておく
    //    oldPos = transform.position;
    //}

    /// <summary>
    /// 更新処理(物理系)
    /// </summary>
    private void FixedUpdate()
    {
        // PhysXによる当たり判定を行った後の位置を制御点のtransformに反映
        //Vector3 v = Vector3.Lerp( transform.position , modelObj.transform.position , 0.75f );
        //v.y = modelObj.transform.localPosition.y;
        //transform.position = v;

        //Vector3 v = transform.position;
        //v.y = modelObj.transform.position.y;

        //transform.position = v;

        isGroundOld = IsGround;
        IsGround = false;

        //rb.velocity = ( velocityVec + gravityVec );

        //// ハンドリングに合わせて速度ベクトルを補正
        //Vector3 targetVel = transform.rotation * Vector3.forward;
        //float t = IsBoost ? boostHandlingInterpolate : defaultHandlingInterpolate;
        //
        //rb.velocity = Vector3.Lerp( rb.velocity.normalized , targetVel , t ) * rb.velocity.magnitude;
        //
        //// ブーストON・OFF時の移動ベクトル変換
        //if( isBoostTrigger || isBoostRelese )
        //{
        //    float force = rb.velocity.magnitude;
        //    rb.velocity = transform.rotation * Vector3.forward;
        //    rb.velocity *= force;
        //}
        //
        //// 速度制限
        //{
        //    float velocityForce = rb.velocity.magnitude;
        //    float velocityLimit = IsBoost ? velocityBoostMax : velocityMax;
        //    Vector3 velocityDir = rb.velocity.normalized;
        //
        //    rb.velocity = Mathf.Min( velocityForce , velocityLimit ) * velocityDir;
        //}
    }

    /// <summary>
    /// 有効化フラグ設定時の諸処理
    /// </summary>
    /// <param name="flags"></param>
    private void SetEnable( bool flags )
    {
        isEnable = flags;

        //if( controller != null ) controller.enabled = flags;
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
            str = "速度ベクトル:" + rb.velocity + "\n重力量:" + gravityVec + "\n速度量:" + velocityVec;

            GUI.Label( new Rect( 0 , 200 , 800 , 600 ) , str , guiStyle );
        }
    }
}

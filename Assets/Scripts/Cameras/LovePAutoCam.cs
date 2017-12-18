using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityEngine.PostProcessing;

public class LovePAutoCam : AutoCam
{
    /// <summary>
    /// カメラ本体
    /// </summary>
    [SerializeField] Camera cameraObj;

    /// <summary>
    /// 街移動時のオブジェクト
    /// </summary>
    private CityPhaseMove playerCityMoveObj;
    [SerializeField] string playerCityMoveObjPath;

    /// <summary>
    /// ポストプロセス関連
    /// </summary>
    PostProcessingBehaviour postProcessing;
    PostProcessingProfile originProfile;

    /// <summary>
    /// 画角関連
    /// </summary>
    [SerializeField] [Range( 1 , 179 )] float fovBoost;
    private float fovOrigin;

    /// <summary>
    /// モーションブラー関連
    /// </summary>
    float blurBlendingOrigin;
    [SerializeField] [Range( 0f , 1f )] float blurBlendingBoost;

    /// <summary>
    /// 生成時処理
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        fovOrigin = cameraObj.fieldOfView;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    protected override void Start()
    {
        base.Start();

        if( !Application.isPlaying )
        {
            return;
        }

        playerCityMoveObj = GameObject.Find( playerCityMoveObjPath ).GetComponent<CityPhaseMove>();
        postProcessing = cameraObj.GetComponent<PostProcessingBehaviour>();
        originProfile = postProcessing.profile;
        blurBlendingOrigin = postProcessing.profile.motionBlur.settings.frameBlending;
    }

    /// <summary>
    /// 破棄時処理
    /// </summary>
    private void OnDestroy()
    {
        if( !Application.isPlaying )
        {
            return;
        }

        // 変更したプロファイル値を元に戻す
        MotionBlurModel.Settings motionBlurSettings = postProcessing.profile.motionBlur.settings;
        motionBlurSettings.frameBlending = blurBlendingOrigin;

        postProcessing.profile.motionBlur.settings = motionBlurSettings;
        postProcessing.profile.motionBlur.enabled = true;
    }

    /// <summary>
    /// ターゲットに追従するカメラ
    /// </summary>
    /// <param name="deltaTime"></param>
    protected override void FollowTarget( float deltaTime )
    {
        base.FollowTarget( deltaTime );

        if( !Application.isPlaying )
        {
            return;
        }

        bool isBoost = playerCityMoveObj.IsBoost;

        // 画角の処理
        float targetFov = isBoost ? fovBoost : fovOrigin;

        cameraObj.fieldOfView = Mathf.Lerp( cameraObj.fieldOfView , targetFov , 2.0f * Time.deltaTime );

        // モーションブラーの処理
        MotionBlurModel.Settings motionBlurSettings = postProcessing.profile.motionBlur.settings;
        float targetBlending = isBoost ? blurBlendingBoost : blurBlendingOrigin;

        motionBlurSettings.frameBlending = Mathf.Lerp( motionBlurSettings.frameBlending , targetBlending , 2.0f * Time.deltaTime );

        postProcessing.profile.motionBlur.settings = motionBlurSettings;

        postProcessing.profile.motionBlur.enabled = isBoost;
    }
}

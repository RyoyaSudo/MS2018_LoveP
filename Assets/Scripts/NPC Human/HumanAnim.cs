using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 人オブジェクトのアニメーション処理
/// </summary>
public class HumanAnim : MonoBehaviour {

    //アニメーター
    Animator anim;

    //初期化
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // 更新
    void Update()
    {
    }

    /// <summary>
    ///  乗車するときのアニメーションをON
    /// </summary>
    public void RideAnimON()
    {
        anim.SetBool( "Ride" , true );
    }

    /// <summary>
    ///  乗車するときのアニメーションをON
    /// </summary>
    public void RideAnimOFF()
    {
        anim.SetBool( "Ride" , false );
    }

    /// <summary>
    ///  乗車ジャンプするときのアニメーションをON
    /// </summary>
    public void RideJumpAnimOn()
    {
        anim.SetBool( "RideJump" , true );
    }

    /// <summary>
    ///  運搬のときのアニメーションをON
    /// </summary>
    public void TransportAnimON()
    {
        anim.SetBool( "Transport" , true );
    }

    /// <summary>
    ///  下車するときのアニメーションをON
    /// </summary>
    public void GetoffAnimON()
    {
        anim.SetBool( "Getoff" , true );
    }

    /// <summary>
    ///  下車するときの走りアニメーションをON
    /// </summary>
    public void GetoffRunAnimON()
    {
        anim.SetBool("GetOffRun", true);
    }

    /// <summary>
    ///  下車するときの待ちアニメーションをON
    /// </summary>
    public void GetoffWaitAnimON()
    {
        anim.SetBool("GetOffWait", true);
    }

    /// <summary>
    ///  フレンズポーズ0
    /// </summary>
    public void FriendsPose0ON()
    {
        anim.SetBool("Pose0", true);
    }

    /// <summary>
    ///  フレンズポーズ1
    /// </summary>
    public void FriendsPose1ON()
    {
        anim.SetBool("Pose1", true);
    }

    /// <summary>
    ///  フレンズポーズ2
    /// </summary>
    public void FriendsPose2ON()
    {
        anim.SetBool("Pose2", true);
    }

    /// <summary>
    ///  フレンズポーズ3
    /// </summary>
    public void FriendsPose3ON()
    {
        anim.SetBool("Pose3", true);
    }

    /// <summary>
    ///  フレンズポーズ4
    /// </summary>
    public void FriendsPose4ON()
    {
        anim.SetBool("Pose4", true);
    }

    /// <summary>
    ///  解散するときのアニメーションをON
    /// </summary>
    public void ReleaseAnimON()
    {
        anim.SetBool( "Release" , true );
    }

    /// <summary>
    ///  回避するときのアニメーションをON
    /// </summary>
    public void EvadeAnimON()
    {
        anim.SetTrigger( "Evade" );
    }
}

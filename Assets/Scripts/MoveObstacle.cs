using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour {

    private Rigidbody obstacleRb;

    // 加える力
    [SerializeField] float addPower; 

    // Velocity（向き）の力
    [SerializeField] float impactRate;

    /// <summary>
    /// 吹き飛ばす時に有効となる値。
    /// この値以上でないと吹き飛ばないように設定する。
    /// </summary>
    [SerializeField] PlayerVehicle.Type enableMoveType;

    private Vector3 velocity;

    private Vector3 pos;
	// Use this for initialization
	void Start () {
        obstacleRb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>().position;
        velocity = obstacleRb.velocity;
        obstacleRb.constraints = RigidbodyConstraints.FreezeAll;
        obstacleRb.WakeUp();
        if(GameObject.FindWithTag("Parcel") )
        {
            //ダンボール箱だけポジションX,Zだけフリーズさせる
            obstacleRb.constraints = RigidbodyConstraints.None;
            obstacleRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
    }
	
    void OnCollisionEnter( Collision col )
    {
        // HACK: 障害物吹き飛ばし処理に関して
        //       プレイヤーが当たった時のみ処理する
        if( col.transform.root.gameObject.name != "Player" )
        {
            return;
        }

        // 吹き飛ばせるか判定
        PlayerVehicle.Type currentType = col.transform.root.gameObject.GetComponent<PlayerVehicle>().VehicleType;

        if( currentType >= enableMoveType )
        {
            // 吹き飛ばす処理
            obstacleRb.constraints = RigidbodyConstraints.None;

            Player playerObj = col.transform.root.gameObject.GetComponent<Player>();
            float playerVelocity = playerObj.Velocity;
            Vector3 playerVelocityVec = playerObj.VelocityVec;

            //velocity.y += velocity.y * addPower;
            velocity = playerVelocityVec.normalized * playerVelocity * addPower;
            //pos = new Vector3(pos.x + addPower, pos.y + addPower, pos.z + addPower);
            obstacleRb.AddForce( transform.forward * addPower , ForceMode.Impulse );
            obstacleRb.AddForce( velocity * impactRate , ForceMode.Impulse );
            playerObj.PlaySoundEffect(SoundController.SoundsSeType.BUMP_MIDDLE);
        }
        else
        {
            Player playerObj = col.transform.root.gameObject.GetComponent<Player>();
            playerObj.PlaySoundEffect(SoundController.SoundsSeType.BUMP_SMALL);
        }
    }
}

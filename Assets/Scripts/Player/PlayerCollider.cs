using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] Player playerObj;

    /// <summary>
    /// 当たり判定後に行う処理
    /// </summary>
    private void OnTriggerStay( Collider other )
    {
        switch( other.gameObject.tag )
        {
            // 乗車エリアに関する処理
            case "RideArea":
                Human obj = other.transform.parent.GetComponent<Human>();

                playerObj.PassengerRide( obj );
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            case "Obstacle":
                {
                    //if(collision.gameObject.GetComponent<MoveObstacle>().)
                    //playerObj.GetComponent<Player>().PlaySoundEffect(SoundController.Sounds.BUMP_MIDDLE);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] Player playerObj;
    [SerializeField] CityPhaseMove moveControllerObj;

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

            case "CityPlane":
                moveControllerObj.IsGround = true;
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        switch( other.gameObject.tag )
        {
            case "MobPointAdder":
                MobPointAdder adder = other.gameObject.GetComponent<MobPointAdder>();

                if( adder != null )
                {
                    adder.State = MobPointAdder.StateType.Add;
                    adder.IsPlayerStay = true;
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit( Collider other )
    {
        switch( other.gameObject.tag )
        {
            case "MobPointAdder":
                MobPointAdder adder = other.gameObject.GetComponent<MobPointAdder>();

                if( adder != null )
                { 
                    adder.IsPlayerStay = false;
                }
                break;

            default:
                break;
        }
    }
}

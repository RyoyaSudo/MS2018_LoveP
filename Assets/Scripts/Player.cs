using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed;

    Rigidbody rb;
    float moveRadY;

    float pushPower;
    float pushAddValue;
    float pushForceFriction;

    int rideCount; //現在乗車人数
    int rideGroupNum; //グループ乗車人数
    private GameObject[] passengerObj;

    public enum State
    {
        PLAYER_STATE_STOP = 0,
        PLAYER_STATE_MOVE,
        PLAYER_STATE_TAKE,
        PLAYER_STATE_TAKE_READY
    }
    State state;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        pushPower = 0.0f;
        pushAddValue = 0.10f;
        pushForceFriction = 0.05f;
        rideCount = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        CityMove();
    }

    private void OnTriggerStay(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "RideArea"://乗車エリアなら
                {
                    if( rb.velocity.magnitude < 1.0f)//ほぼ停止してるなら
                    {
                        if (other.transform.parent.gameObject.GetComponent<Human>().stateType != Human.STATETYPE.READY) break;
                        state = State.PLAYER_STATE_TAKE_READY;

                        //乗客を子にする
                        other.transform.parent.gameObject.transform.position = transform.position;
                        other.transform.parent.transform.parent = transform;
                        other.transform.parent.gameObject.GetComponent<Human>().SetStateType(Human.STATETYPE.TRANSPORT);
                        Debug.Log("Ride");

                        if( rideCount == 0 )//最初の乗客なら
                        {
                            switch(other.transform.parent.gameObject.GetComponent<Human>().groupType)
                            {
                                case Human.GROUPTYPE.PEAR:
                                    {
                                        rideGroupNum = 2;
                                        Debug.Log("PEAR");
                                        break;
                                    }
                                case Human.GROUPTYPE.SMAlLL:
                                    {
                                        rideGroupNum = 3;
                                        Debug.Log("SMALL");
                                        break;
                                    }
                                case Human.GROUPTYPE.BIG:
                                    {
                                        rideGroupNum = 5;
                                        Debug.Log("BIG");
                                        break;
                                    }
                            }
                            passengerObj = new GameObject[rideGroupNum];
                        }
                        passengerObj[rideCount] = other.transform.parent.gameObject;
                        rideCount++;

                        //最後の人なら降ろす
                        if( rideCount >= rideGroupNum)
                        {
                            for( int i = 0; i < rideCount; i++ )
                            {
                                passengerObj[i].transform.parent = null;
                                passengerObj[i].GetComponent<Human>().stateType = Human.STATETYPE.GETOFF;
                            }
                            rideCount = 0;
                        }
                    }
                    
                    break;
                }
        }
    }

    void CityMove()
    {
        float moveV = Input.GetAxis("Vertical");
        float moveH = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(moveH, 0.0f, moveV);

        //Debug.Log( "Horizontal:" + moveH );

        if (Mathf.Abs(moveH) > 0.2f)
        {
            moveRadY += moveH * 180.0f * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, moveRadY, 0);
        }

        Vector3 force = transform.forward * speed;

        //rb.AddForce(force);

        // プッシュ動作
        if (Input.GetKey(KeyCode.Space))
        {
            force = new Vector3(0.0f, 0.0f, 0.0f);
            rb.velocity *= 0.975f;//減速
            //速度が一定以下なら停止する
            if (rb.velocity.magnitude < 1.0f)
            {
                rb.velocity *= 0.0f;
                state = State.PLAYER_STATE_STOP;
            }
        }

        // プッシュ解放した後のダッシュ
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            //force *= ( 30.0f * rb.mass );
            rb.AddForce(force * 4.0f, ForceMode.VelocityChange);
        }

        // 今回の速度加算
        rb.AddForce(force, ForceMode.Acceleration);


        ////停止処理
        //if( rb.velocity.magnitude < 1.0f)
        //{
        //    rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        //    if ( state == State.PLAYER_STATE_MOVE )
        //    {
        //        state = State.PLAYER_STATE_STOP;
        //    }
        //}
        //else
        //{
        //    state = State.PLAYER_STATE_MOVE;
        //}
    }
}



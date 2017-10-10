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

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        pushPower = 0.0f;
        pushAddValue = 0.10f;
        pushForceFriction = 0.05f;
    }
	
	// Update is called once per frame
	void Update () {
        float moveV = Input.GetAxis( "Vertical" );
        float moveH = Input.GetAxis( "Horizontal" );

        Vector3 direction = new Vector3( moveH , 0.0f , moveV );

        //Debug.Log( "Horizontal:" + moveH );

        if( Mathf.Abs( moveH ) > 0.2f )
        {
            moveRadY += moveH * 180.0f * Time.deltaTime;
            transform.rotation = Quaternion.Euler( 0 , moveRadY , 0 );
        }

        Vector3 force = transform.forward * speed;

        rb.AddForce( force );

        // プッシュ動作
        if( Input.GetKey( KeyCode.Space ) )
        {
            force = new Vector3( 0.0f , 0.0f , 0.0f );
        }

        // プッシュ解放した後のダッシュ
        if( Input.GetKeyUp( KeyCode.Space ) )
        {
            rb.velocity = new Vector3( 0.0f , 0.0f , 0.0f );
            //force *= ( 30.0f * rb.mass );
            rb.AddForce( force * 4.0f , ForceMode.VelocityChange );
        }

        // 今回の速度加算
        rb.AddForce( force , ForceMode.Acceleration );
    }
}

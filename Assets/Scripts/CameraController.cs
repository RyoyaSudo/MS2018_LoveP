using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;
    Vector3 offset;
    float radius;

    // Use this for initialization
    void Start()
    {
        Vector3 offsetV = transform.position - player.transform.position;
        radius = offsetV.magnitude;
        offset = new Vector3( 0.0f , 5.0f , -3.0f );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = player.transform.rotation;

        Vector3 s = new Vector3( 1.0f , 1.0f , 1.0f );
        Vector3 p = new Vector3( 0.0f , 0.0f , 0.0f );

        Matrix4x4 mtxR = Matrix4x4.TRS( p , transform.rotation , s );

        Vector3 dirStd = new Vector3( 0.0f , 0.5f , -10.0f );
        Vector3 trsV = dirStd.normalized * radius + offset;

        transform.position = mtxR.MultiplyVector( trsV ) + player.transform.position;
    }
}

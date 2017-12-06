using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : MonoBehaviour {

    private Rigidbody obstacleRb;

    [SerializeField]
    private float addPower; //加える力

    [SerializeField]
    private float impactRate;　//Velocity（向き）の力

    private Vector3 velocity;

    private Vector3 pos;
	// Use this for initialization
	void Start () {
        obstacleRb = GetComponent<Rigidbody>();
        pos = GetComponent<Transform>().position;
        velocity = obstacleRb.velocity;
	}
	
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Player")
        {
            Player playerObj = col.gameObject.GetComponent<Player>();
            float playerVelocity = playerObj.Velocity;
            Vector3 playerVelocityVec = playerObj.VelocityVec;

            //velocity.y += velocity.y * addPower;
            velocity = playerVelocityVec.normalized * playerVelocity * addPower;
            //pos = new Vector3(pos.x + addPower, pos.y + addPower, pos.z + addPower);
            obstacleRb.AddForce(transform.forward * addPower, ForceMode.Impulse);
            obstacleRb.AddForce(velocity * impactRate, ForceMode.Impulse);
        }
    }
}

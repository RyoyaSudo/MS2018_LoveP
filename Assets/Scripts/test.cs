using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    //アニメーター
    Animator anim;

    //スピード
    public float walkSpeed;
    public float runSpeed;


    //初期化
    void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	// 更新
	void Update ()
    {
        Vector3 position = this.transform.position;
        Quaternion rotation = this.transform.rotation;

        //移動
        //float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speed;

        if (z == 0)
        {
            anim.SetBool("Moving", false);
        }
        else
        {
            anim.SetBool("Moving", true);
        }

        if (z > 0)
        {
            anim.SetBool("FrontWalk", true);
        }
        else
        {
            anim.SetBool("FrontWalk", false);
        }

        if (z < 0)
        {
            anim.SetBool("BackWalk", true);
   
        }
        else
        {
            anim.SetBool("BackWalk", false);
        }

        //if (x > 0)
        //{
        //    rotation.y += 0.03f;
        //    if (rotation.y >= 170.0f)
        //    {
        //        rotation.y = -170.0f;
        //    }
        //}
        //else if (x < 0)
        //{
        //    rotation.y -= 0.01f;
        //    if (rotation.y <= -170)
        //    {
        //        rotation.y = 170;
        //    }

        //}

        if ( Input.GetKey("d"))
        {
            transform.Rotate(new Vector3(0, 1, 0) );
        }
        else if ( Input.GetKey("a"))
        {
            transform.Rotate(new Vector3(0, -1, 0));
        }


        //ダッシュ
        if (Input.GetKey("space") && z > 0 )
        {
            anim.SetBool("Running", true);
            speed = runSpeed;
        }
        else
        {
            anim.SetBool("Running", false);
            speed = walkSpeed;
        }

        //this.transform.rotation = rotation;

        position += transform.forward * z * speed;
        this.transform.position = position;

        //勝利ポーズ
        if ( Input.GetKey("q"))
        {
            anim.SetTrigger("Win");
        }
	}
}

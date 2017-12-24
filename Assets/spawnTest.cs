using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTest : MonoBehaviour {

    [SerializeField] SpawnPoint p;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown( KeyCode.N ) )
        {
            Human h = p.PassengerSpawn( 0 , PassengerController.GROUPTYPE.Lovers , SpawnPoint.PASSENGER_ORDER.DEFOULT );
            h.Role = Human.RoleType.Mob;

            //p.MobSpawn();
        }
	}
}

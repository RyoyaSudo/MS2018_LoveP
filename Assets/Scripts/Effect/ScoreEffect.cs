using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEffect : MonoBehaviour {

    public enum StateType
    {
        None = 0,
        Wait,
        Chase,
        Destroy,
    }

    public StateType State { get { return state; } set { SetState( value ); } }
    StateType state;

    float stateTimer;

    ParticleSystem particle;
    public Transform Target { get; set; }

    public float waitTime;
    [SerializeField] float chaseDuration;
    float chaseAddValuePerFlame;
    float chaseRate;

    Vector3 chaseStPos;

    public int AddScore { get; set; }

    ScoreCtrl scoreObj;
    [SerializeField] string scorePath;

    private void Awake()
    {
        stateTimer = 0.0f;
        Target = null;
        chaseRate = 0.0f;
        chaseAddValuePerFlame = 1.0f / chaseDuration;

        chaseStPos = Vector3.zero;

        particle = GetComponent<ParticleSystem>();

        AddScore = 0;
        scoreObj = GameObject.Find( scorePath ).GetComponent<ScoreCtrl>();
    }

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update ()
    {
        switch( state )
        {
            case StateType.None:
                break;

            case StateType.Wait:
                stateTimer -= Time.deltaTime;

                if( stateTimer < 0.0f )
                {
                    State = StateType.Chase;
                }
                break;

            case StateType.Chase:
                chaseRate += chaseAddValuePerFlame * Time.deltaTime;
                chaseRate = Mathf.Min( chaseRate , 1.0f );

                Vector3 curV = Vector3.Slerp( chaseStPos , Target.position , chaseRate );
                transform.position = curV;

                if( chaseRate == 1.0f )
                {
                    State = StateType.Destroy;
                }
                break;

            default:
                break;
        }
    }

    // 状態設定
    void SetState( StateType type )
    {
        state = type;

        switch( type )
        {
            case StateType.None:
                break;

            case StateType.Wait:
                particle.Play();
                stateTimer = waitTime;
                break;

            case StateType.Chase:
                chaseStPos = transform.position;
                break;

            case StateType.Destroy:
                scoreObj.AddScoreValue( AddScore );
                Destroy( gameObject );
                break;

            default:
                break;
        }
    }

}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //画面遷移を可能にする

public class Transition : MonoBehaviour
{
    [SerializeField]
    private Material _transitionIn;

    [SerializeField]
    private Material _transitionOut;

    [SerializeField]
    private UnityEvent OnTransition;
    [SerializeField]
    private UnityEvent OnComplete;

    private string transitionScene;
    private bool transitionFlag;//トランジション中か否かのフラグ

    public enum State
    {
        STATE_NONE = 0,
        STATE_FADE_IN,
        STATE_FADE_OUT,

    }

    public State state;

    private void Awake()
    {
        transitionFlag = false;
        state = State.STATE_NONE;
    }

    void Start()
    {
        StartCoroutine(StartTransition());
    }

    void Update()
    {
    }

    IEnumerator BeginTransition()
    {
        yield return Animate(_transitionIn, 1);
        if (OnTransition != null) { OnTransition.Invoke(); }
        yield return new WaitForEndOfFrame();
        if( transitionScene != null)
        {
           yield return SceneManager.LoadSceneAsync(transitionScene);
        }
        state = State.STATE_FADE_OUT;

        yield return Animate(_transitionOut, 1);
        if (OnComplete != null) { OnComplete.Invoke(); }
    }

    IEnumerator StartTransition()
    {
        yield return Animate(_transitionOut, 1);
        if (OnComplete != null) { OnComplete.Invoke(); }
    }

    /// <summary>
    /// time秒かけてトランジションを行う
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Animate(Material material, float time)
    {
        GetComponent<SpriteRenderer>().material = material;
        float current = 0;
        while (current < time)
        {
            material.SetFloat("_Alpha", current / time);
            yield return new WaitForEndOfFrame();
            current += Time.deltaTime;
        }
        material.SetFloat("_Alpha", 1);
    }

    public void StartTransition( string sceneName )
    {
        if ( transitionFlag == true ) return;
        transitionFlag = true;
        state = State.STATE_FADE_IN;
        transitionScene = sceneName;
        StartCoroutine(BeginTransition());
    }
}
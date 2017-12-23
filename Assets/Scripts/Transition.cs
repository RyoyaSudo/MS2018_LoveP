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

    private void Awake()
    {
        transitionFlag = false;
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
        transitionScene = sceneName;
        StartCoroutine(BeginTransition());
    }
}
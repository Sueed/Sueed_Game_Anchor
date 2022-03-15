using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGmaeObserver
{
    public SceneFader fadePrefabs;

    public SceneFader fadeDead;

    bool fadeFinished;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start() 
    {
        fadeFinished = true;
        GameManager.Instance.AddObserver(this);
    }

    // IEnumerator Trasition(string scene)
    // {
    //     yield return SceneManager.LoadSceneAsync(scene);
    //     yield break;
    // }

    public void TransitionToFirst(string scene)
    {
        StartCoroutine(LoadLevel(scene));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(fadePrefabs);
        if(scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return StartCoroutine(fade.FadeIn(2f));
        }
        
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(fadeDead);
        yield return StartCoroutine(fade.FadeOut(3f));
        yield return SceneManager.LoadSceneAsync("SampleScene");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    public void EndGame()
    {
        StartCoroutine(LoadMain());
    }

    public void EndNotify()
    {
        if(fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}

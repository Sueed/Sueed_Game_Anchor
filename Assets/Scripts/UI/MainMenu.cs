using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button StartGameBtn;
    Button QuitGameBtn;

    PlayableDirector Director;

    void Awake()
    {
        StartGameBtn = transform.GetChild(1).GetComponent<Button>();
        QuitGameBtn = transform.GetChild(2).GetComponent<Button>();

        StartGameBtn.onClick.AddListener(PlayTimeLine);
        QuitGameBtn.onClick.AddListener(QuitGame);

        Director = FindObjectOfType<PlayableDirector>();
        Director.stopped += StartGame;
    }

    void PlayTimeLine()
    {
        Director.Play();
    }

    void StartGame(PlayableDirector obj)
    {
        SceneController.Instance.TransitionToFirst("CharacterController");
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

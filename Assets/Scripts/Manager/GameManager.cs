using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public CharacterStatus playStatus;

    // public CharacterStatus bossStatus;

    public float delaytime;

    List<IEndGmaeObserver> endGmaeObservers = new List<IEndGmaeObserver>();
    
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStatus player)
    {
        playStatus = player;
    }

    // public void RigisterBoss(CharacterStatus boss)
    // {
    //     bossStatus = boss;
    // }

    // private void Update() 
    // {
    //     if(playStatus.currentHealth == 0)
    //         NotifyObserver();
    // }

    public void AddObserver(IEndGmaeObserver observer)
    {
        endGmaeObservers.Add(observer);
    }

    public void RemoveObserver(IEndGmaeObserver observer)
    {
        endGmaeObservers.Remove(observer);
    }

    public void NotifyObserver(bool isBoss)
    {
        if(!isBoss)
        {
            foreach(var observer in endGmaeObservers)
            {
                observer.EndNotify();
            }
        }else
        {
            if(delaytime <= 0)
            {
                foreach(var observer in endGmaeObservers)
                {
                    observer.EndNotify();
                }
            }else
            {
                delaytime -= Time.deltaTime;
            }
        }
        
    }
}

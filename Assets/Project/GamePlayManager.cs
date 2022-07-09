using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    public enum StateGame
    {
        Prepare = 0,
        Countdown = 1,
        InGame = 2,
        EndGame = 3
    }
    public StateGame state = StateGame.Prepare;
    public int prepareTime = 0;
    public int gameTime = 10;

    public float time;

    private static GamePlayManager instance;

    void MakeInstance()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else instance = this;
    }

    public static GamePlayManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        MakeInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = StateGame.Countdown;
            time = Time.time;
        }

        if (Time.time - time >= prepareTime && state == StateGame.Countdown)
        {
            time = Time.time;
            state = StateGame.InGame;
            Debug.Log("Start");
        }

        if (Time.time - time >= gameTime && state == StateGame.InGame)
        {
            Debug.Log("End");
            state = StateGame.EndGame;
        }
    }
}

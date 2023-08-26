using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GameController : Singleton<GameController>
{
    public GameState state = GameState.IDLE;
    public GameState State { get => state; private set => state = value; }
    [SerializeField]
    private Player player;

    [SerializeField]
    private Shadow shadow;

    [SerializeField] GameObject tracker;

    private float runTime;
    private float timeScale;
    [SerializeField]
    private int pageNumber;
    [SerializeField]
    private int totalPage;
    private void Start()
    {
        ChangeState(GameState.STARTING);
    }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onTotalPageLoaded", SaveTotalPage);
        EventManager<int>.Instance.StartListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StartListening("onStateChanged", ChangeState);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
        EventManager<bool>.Instance.StartListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StartListening("pause", Pause);
    }
    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onTotalPageLoaded", SaveTotalPage);
        EventManager<int>.Instance.StopListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StopListening("onStateChanged", ChangeState);
        EventManager<bool>.Instance.StopListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StopListening("pause", Pause);
        EventManager<GameState>.Instance.StopListening("onPlayerDead", ChangeState);


    }

    private void Update()
    {
        switch(State)
        {
            case GameState.IDLE:
                break;
            case GameState.STARTING:
                
                timeScale = 1;
                pageNumber = 0;
                EventManager<bool>.Instance.TriggerEvent("onGameStartingState",true);
                break;
            case GameState.PAUSING:
                Time.timeScale = 0;
                break;
            case GameState.PLAYING:
                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    timeScale = 2;
                    Debug.Log("FASTER");
                }
                runTime += Time.unscaledDeltaTime;
                EventManager<float>.Instance.TriggerEvent("onTimer", runTime);
                Time.timeScale = timeScale;
                break;
            case GameState.END_LEVEL:
                break;
            case GameState.LOSING:
                EventManager<int>.Instance.TriggerEvent("SaveTotalPage", totalPage + pageNumber);
                EventManager<bool>.Instance.TriggerEvent("onGameOver", true);
                Time.timeScale = 0;
                break;
        }
    }


    private void ChangeState(GameState _state)
    {
        State = _state;
    }


    private void SetGameScene(bool isGameSceneStarted)
    {
        Player _player = Instantiate(player, new Vector2(0, 30), Quaternion.identity).GetComponent<Player>();
        Shadow _shadow = Instantiate(shadow, new Vector2(-40, 13), Quaternion.identity).GetComponent<Shadow>();
        shadow.Setup(player);
        Instantiate(tracker, new Vector3(0, 0, 0), Quaternion.identity);
        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
        ChangeState(GameState.PLAYING);
    }


    void Pause (bool isPausing)
    {

        if (isPausing)
        {
            ChangeState(GameState.PAUSING);
        }
        else
        {
            ChangeState(GameState.PLAYING);
        }
    }

    private void AddPage(int number)
    {
        pageNumber += number;
        EventManager<int>.Instance.TriggerEvent("SavePage", pageNumber);//DATA_MANAGER
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);//UI
    }

    private void SaveTotalPage(int _totalPage)
    {
        totalPage = _totalPage;
    }

}
public enum GameState
{
    IDLE,
    STARTING,
    PAUSING,
    PLAYING,
    END_LEVEL,
    LOSING,
}
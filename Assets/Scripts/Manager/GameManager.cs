using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBase<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        Application.runInBackground = true;
        //float targetAspectRatio = 9f / 16f;
        //int screenHeight = Mathf.Clamp(Screen.height, 0, 1920);
        //Screen.SetResolution((int)(screenHeight * targetAspectRatio), screenHeight, false);
        Application.targetFrameRate = 60;
    }
    public enum GameStage
    {
        Playing, StopPlay, MainMenu, BeginPlay
    }
    public GameStage gameStage;
    // Start is called before the first frame update
    void Start()
    {
        gameStage = GameStage.MainMenu;
    }
    private void OnApplicationQuit()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public GameStage GetGameStage()
    {
        return gameStage;
    }
    public void SetGameStage(GameStage newGameStage)
    {
        gameStage = newGameStage;
    }
    public void HandleDataFormServer(string data)
    {
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}

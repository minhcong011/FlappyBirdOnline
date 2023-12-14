using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderBoardManager : SingletonBase<LeaderBoardManager>
{
    [SerializeField] private GameObject leaderBoard;
    private List<PlayerData> listPlayerData = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        ManageScorePanel();
    }
    public void RegisterScoreData(PlayerData data)
    {
        listPlayerData.Add(data);
    }
    public void RemoveScoreData(PlayerData data)
    {
        listPlayerData.Remove(data);
    }
    private void ManageScorePanel()
    {
        SortListDataByScore();
        UIManager.Instance.UpdateScoreBar(listPlayerData);
    }
    private void SortListDataByScore()
    {
        var sortedPlayers = listPlayerData.OrderByDescending(player => player.highScore).ToList();
        listPlayerData = sortedPlayers;
    }
}

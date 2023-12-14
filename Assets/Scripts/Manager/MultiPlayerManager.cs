using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MultiPlayerManager : SingletonBase<MultiPlayerManager>
{
    [SerializeField] private GameObject otherPlayerPref;
    [SerializeField] public List<GameObject> listOtherPlayer = new();
    [SerializeField] public List<PlayerData> listOtherPlayerData = new();
    private bool inProcessHandlerData;
    protected override void Awake()
    {
        base.Awake();
        ServerManager.Instance.OnReceivedData += HandleDataFormServer;
    }

    public void HandleDataFormServer(string messageType, string message)
    {
        if (messageType == "No Other Player In Room")
        {
            return;
        }
        if (messageType == "PlayerLeave")
        {
            DeletePlayer(message);
        }
        if(messageType == "PlayerData")
        {
            HandlePlayerData(message);
        }

    }
    private void HandlePlayerData(string data)
    {
        if (inProcessHandlerData) return;
        inProcessHandlerData = true;

        StartCoroutine(HandlePlayerDataCoroutine(data));

        IEnumerator HandlePlayerDataCoroutine(string data) {
            Debug.Log(data);
            string[] playersData = data.Split(";");
            try
            {
                foreach (string playerData in playersData)
                {
                    string[] dataArr = playerData.Split("|");
                    if (dataArr.Length > 1)
                    {
                        string id = dataArr[0];
                        string name = dataArr[1];
                        int.TryParse(dataArr[2], out int score);
                        float.TryParse(dataArr[3], out float posX);
                        float.TryParse(dataArr[4], out float posY);
                        float.TryParse(dataArr[5], out float rotationZ);
                        UpdatePlayerData(name ,id, score, posX, posY, rotationZ);
                    }
                }
            }
            catch
            {
                inProcessHandlerData = false;
            }
            finally
            {
                inProcessHandlerData = false;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    private void Update()
    {
    }
    private PlayerData CheckDataIsNotAvailable(string id)
    {
        if (listOtherPlayerData.Count == 0) return null;
        return listOtherPlayerData.Find(data => data.id == id);
    }
    private void DeletePlayer(string id)
    {
        PlayerData playerToRemove = listOtherPlayerData.Find(data => data.id == id.Split('\n')[0]);

        if (playerToRemove != null)
        {
            playerToRemove.isDelete = true;
            listOtherPlayerData.Remove(playerToRemove);
            LeaderBoardManager.Instance.RemoveScoreData(playerToRemove);
        }
    }
    private void CreateNewOtherPlayer(string id, string name)
    {
        PlayerData newData = new(id, name);
        GameObject newOtherPlayer = Instantiate(otherPlayerPref);
        newOtherPlayer.GetComponent<OtherPlayerController>().SetData(newData, newData.name);
        listOtherPlayer.Add(newOtherPlayer);
        listOtherPlayerData.Add(newData);
    }
    private void UpdatePlayerData(string name,string id, int score, float posX, float posY, float rotationZ)
    {
        PlayerData dataToUpdate = CheckDataIsNotAvailable(id);
        if (dataToUpdate == null)
        {
            CreateNewOtherPlayer(id, name);
        }
        else
        {
            dataToUpdate.x = posX;
            dataToUpdate.y = posY;
            dataToUpdate.rotationZ = rotationZ;
            dataToUpdate.highScore = score;
        }
    }
    public void CleanupOtherPlayer()
    {
        foreach(PlayerData playerData in listOtherPlayerData)
        {
            playerData.isDelete = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    [Header("Panel")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gamePlayUI;
    [SerializeField] private GameObject lostGamePanel;

    [Header("other")]
    [SerializeField] private GameObject playerName;
    [SerializeField] private Vector3 offSet;

    [SerializeField] private GameObject uiHolder;

    [Header("LeaderBoard")]
    [SerializeField] private GameObject[] scoreBar;
    [SerializeField] private TextMeshProUGUI localPlayerScore;
    [SerializeField] private TextMeshProUGUI serverID;
    [SerializeField] private TextMeshProUGUI amountPlayerInRoom;
    [SerializeField] private TMP_InputField roomIdInput;
    [SerializeField] private TMP_InputField nameInput;

    [SerializeField] private GameObject transformTextEf;
    [SerializeField] private GameObject popup;
    [SerializeField] private TextMeshProUGUI speedBuftStage;
    // Start is called before the first frame update
    void Start()
    {
        ServerManager.Instance.OnReceivedData += HandleDataFormServer;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetGameStage() == GameManager.GameStage.MainMenu)
        {
            mainMenuUI.SetActive(true);
            gamePlayUI.SetActive(false);
        }
        else
        {
            SetAmountPlayerInRoomText(MultiPlayerManager.Instance.listOtherPlayerData.Count + 1);
            SetRoomIDText(ServerManager.Instance.GetCurrentRoomID());
            gamePlayUI.SetActive(true);
            mainMenuUI.SetActive(false);
        }
    }
    public void CreatePlayerName(GameObject player, PlayerData nameData)
    {
        GameObject newName = Instantiate(playerName);
        newName.transform.SetParent(uiHolder.transform, false);
        StartCoroutine(UpdatePlayerName(player, newName, nameData));
    }
    public void UpdateScoreBar(List<PlayerData> listPlayerData)
    {
        for(int i = 0; i < scoreBar.Length; i++)
        {
            if(i < listPlayerData.Count)
            {
                scoreBar[i].SetActive(true);
                TextMeshProUGUI playerName = scoreBar[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI score = scoreBar[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                playerName.text = listPlayerData[i].name;
                score.text = listPlayerData[i].highScore.ToString();
            }
            else
            {
                scoreBar[i].SetActive(false);
            }
        }
    }
    IEnumerator UpdatePlayerName(GameObject player, GameObject nameText, PlayerData nameData)
    {
        while(nameText != null)
        {
            if (player == null)
            {
                Destroy(nameText);
                break;
            }
            nameText.GetComponent<TextMeshProUGUI>().text = nameData.name;
            nameText.transform.position = Camera.main.WorldToScreenPoint(player.transform.position + offSet);
            yield return new WaitForFixedUpdate();
        }
    }
    public void HandleDataFormServer(string dataType, string dataValue)
    {
        switch (dataType)
        {
            case "RoomIsFull": CreatePopup("ROOM IS FULL"); break;
        }
    }
    void SetAmountPlayerInRoomText(int amount)
    {
        amountPlayerInRoom.text = $"online: {amount}";
    }
    public void UpdateLocalPlayerScoreText(int score)
    {
        localPlayerScore.text = $"Your Score: {score}";
    }
    public void SetRoomID(string roomID)
    {
        serverID.text = $"Server #{roomID}";
    }
    public void HandleRoomIdInput()
    {
        if (!int.TryParse(roomIdInput.text, out _))
        {
            roomIdInput.text = "Random";
            ServerManager.Instance.SetRequestRoomId("Random");
            return;
        }
        ServerManager.Instance.SetRequestRoomId(roomIdInput.text);
    }
    public void HandleNameInput()
    {
        PlayerController.instance.data.name = nameInput.text;
    }
    public void ActiveGamePlayUI(string roomID)
    {
        mainMenuUI.SetActive(false);
        gamePlayUI.SetActive(true);
    }
    public void SetRoomIDText(string roomID) 
    {
        //string roomIDInt = "";
        //try
        //{
        //    roomIDInt = Regex.Match(roomID, @"([a-zA-Z]+)(\d+)").Groups[2].Value;
        //}
        //catch
        //{

        //}
        serverID.text = $"Server #{roomID}";
    }
    public void ActiveMainMenuUI()
    {
        mainMenuUI.SetActive(true);
        gamePlayUI.SetActive(false);
    }
    public void ShowLostGamePanel(int score)
    {
        lostGamePanel.SetActive(true);
        TextMeshProUGUI scoreText = lostGamePanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        scoreText.text = $"{score}";
    }
    public void CreateTranformTextEf()
    {
        Instantiate(transformTextEf).transform.SetParent(uiHolder.transform, false);
    }
    public void SetSpeedBuftText(int value)
    {
        speedBuftStage.text = $"BIRD SPEED: {value+1}";
    }
    public void CreatePopup(string content)
    {
        GameObject newPopup = Instantiate(popup);
        newPopup.transform.SetParent(uiHolder.transform, false);
        TextMeshProUGUI contentText = newPopup.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        contentText.text = content;
    }
}

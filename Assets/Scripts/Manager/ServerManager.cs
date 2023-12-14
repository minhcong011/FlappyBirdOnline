using System;
using UnityEngine;
using System.Text;
using HybridWebSocket;

public class ServerManager : SingletonBase<ServerManager>
{
    [SerializeField] private string websocketAddr;
    [SerializeField] private string password;

    public Action<string, string> OnReceivedData;   
    public Action<string> OnReceivedPlayerData;   

    private string currentRoomID;
    private string requestRoomID = "random";

    private WebSocket webSocket;

    private bool isCanSendData = true;
    void Start()
    {
        ConnectToWebSocket();    
        OnReceivedData += HandlerData;
    }
    void Update()
    {
        Debug.Log(isCanSendData);
        if (Input.GetKeyDown(KeyCode.H))
        {
            string message = "Hello from Unity!";
            SendData("helloFromUnity");
            Debug.Log("Sent message: " + message);
        }
    }
    private void ConnectToWebSocket()
    {
        webSocket = WebSocketFactory.CreateInstance($"{websocketAddr}");

        webSocket.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + webSocket.GetState().ToString());
        };

        webSocket.OnMessage += (byte[] msg) =>
        {
            string data = Encoding.UTF8.GetString(msg).Split("\n")[0];
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                isCanSendData = true;
                OnReceivedData?.Invoke(GetDataType(data), GetDataValue(data));
            });
            if(GetDataType(data) == "PlayerData")
            {
                OnReceivedPlayerData?.Invoke(GetDataValue(data));
            }
            //Debug.Log("WS received message: " + data);
        };

        webSocket.OnError += (string errMsg) =>
        {
            Debug.Log("WS error: " + errMsg);
        };

        webSocket.OnClose += (WebSocketCloseCode code) =>
        {
            if(GameManager.Instance.GetGameStage() != GameManager.GameStage.MainMenu) SendData("OutRoom");
            Debug.Log("WS closed with code: " + code.ToString());
        };
        webSocket.Connect();
    }
    public void SendData(string data)
    {
        if (!isCanSendData) return;
        webSocket.Send(Encoding.UTF8.GetBytes(data));
        isCanSendData = false;
    }
    public void SendPlayerDataToServer(string name, int highScore, float posX, float posY, float rotaionZ)
    {
        string data = $"PlayerData/{name}|{highScore}|{posX}|{posY}|{rotaionZ}";
        SendData(data);
    }
    public void WebSocketMessage(string message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            string cleanMessage = message.Split("\n")[0];
            OnReceivedData?.Invoke(GetDataType(cleanMessage), GetDataValue(cleanMessage));
        });
    }
    private void HandlerData(string dataType, string data)
    {
        if (dataType == "JoinRoomCompleted")
        {
            currentRoomID = data;
            GameManager.Instance.SetGameStage(GameManager.GameStage.BeginPlay);
        }
    }
    public void JoinRoom()
    {
        RequestRoomToJoin();
    }
    public void OutRoom()
    {
        SendData("OutRoom");
        MultiPlayerManager.Instance.CleanupOtherPlayer();
    }
    public void RequestRoomToJoin()
    {
        if (requestRoomID == "random") SendData("RequestRoom/JoinRoomAvailable");
        else SendData($"RequestRoom/{requestRoomID}");
    }
    public void SetRequestRoomId(string roomID)
    {
        requestRoomID = roomID;
    }
    private string GetDataType(string data)
    {
        return data.Split("/")[0];
    }
    private string GetDataValue(string data)
    {
        try
        {
            return data.Split("/")[1];
        }
        catch
        {
            return null;
        }
    }
    public string GetCurrentRoomID()
    {
        return currentRoomID;
    }
    private void OnApplicationQuit()
    {
        webSocket.Close();
    }
}

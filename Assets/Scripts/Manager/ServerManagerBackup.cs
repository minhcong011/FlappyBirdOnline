using System;
using UnityEngine;
using WebSocketSharp;

public class ServerManagerBackup : SingletonBase<ServerManagerBackup>
{
    [SerializeField] private string websocketAddr;

    public Action<string, string> OnReceivedData;

    private string currentRoomID;
    private string requestRoomID = "random";

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    private WebSocket webSocket;
#endif
    void Start()
    {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ConnectToWebSocket();
#endif
        Application.ExternalEval(@" 
            var unityInstance = unityInstance || {};
            var socket = new WebSocket('ws://localhost:8080/ws');

            socket.onopen = function(event) {
                console.log('WebSocket connected');
            };

            socket.onmessage = function(event) {
                console.log('Received message: ' + event.data);
            };

            socket.onclose = function(event) {
                console.log('WebSocket closed');
            };

            function sendMessage(message) {
                socket.send(message);
            }
            function SendMessageToUnity(functionName, message) {
                unityInstance.SendMessage('Server', functionName, message);
            }
        ");
        OnReceivedData += HandlerData;
        SendData("hello from redis");
    }
    void Update()
    {
        //webSocket.Send("helloFormUnity");
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    string message = "Hello from Unity!";
        //    webSocket.Send(message);
        //    Debug.Log("Sent message: " + message);
        //}
    }
    private void ConnectToWebSocket()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //webSocket = new WebSocket("ws://" + websocketAddr + "/ws");
        webSocket = new WebSocket("ws://localhost:8080/ws");
        webSocket.OnMessage += GetDataFromWebSocket;
        webSocket.OnOpen += (sender, e) =>
        {
            Debug.LogError("complete connection ");
        };
        webSocket.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket error: " + e.Message);
        };
        webSocket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket closed");
            // Xử lý sự kiện đóng kết nối ở đây
        };
        webSocket.Connect();
#endif
    }
    public void SendData(string data)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        webSocket.Send(data);
#else
        // Chuyển đổi dữ liệu sang định dạng JSONS
        Application.ExternalCall("sendMessage", data);
#endif
    }
    public void SendPlayerDataToServer(string name, int highScore, float posX, float posY, float rotaionZ)
    {
        string data = $"PlayerData/{name}|{highScore}|{posX}|{posY}|{rotaionZ}";
        SendData(data);
    }
    private void GetDataFromWebSocket(object sender, MessageEventArgs e)
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        string data = e.Data.Split("\n")[0];
        Debug.Log("Received message: " + data);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            OnReceivedData?.Invoke(GetDataType(data), GetDataValue(data));
        });

#endif
    }
    public void WebSocketMessage(string message)
    {
        Debug.Log(message);
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
        }
    }
    public void JoinRoom()
    {
        RequestRoomToJoin();
        GameManager.Instance.SetGameStage(GameManager.GameStage.BeginPlay);
    }
    public void OutRoom()
    {
        SendData("OutRoom");
    }
    public void RequestRoomToJoin()
    {
        if (requestRoomID == "random") SendData("RequestRoom/JoinRoomAvailable/");
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
}

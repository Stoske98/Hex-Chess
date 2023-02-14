using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
public class Player : MonoBehaviour
{
    #region Player Singleton
    private static Player _instance;

    public static Player Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
    #endregion
    public string FakeDeviceID;
    [HideInInspector]public string DeviceID;
    public bool IsTest = true;

    public Data.Player data;

    private void Start()
    {
        if (IsTest)
            DeviceID = FakeDeviceID;
        else
            DeviceID = SystemInfo.deviceUniqueIdentifier;

        Receiver.Instance.SubscibeMainMenu();
        ConnectedToServer();
    }

    private void DisconnectedFromServer()
    {
        Debug.Log("Disconnected from server.");
        RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
    }

    public void ConnectedToServer()
    {
        RealtimeNetworking.OnConnectingToServerResult += ConnectResult;
        RealtimeNetworking.Connect();
    }
    private void ConnectResult(bool successful)
    {
        if (successful)
        {
            Debug.Log("Connected to server successfully.");
            RealtimeNetworking.OnDisconnectedFromServer += DisconnectedFromServer;

        }
        else
        {
            Debug.Log("Failed to connect the server.");
        }
        RealtimeNetworking.OnConnectingToServerResult -= ConnectResult;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Transports.UNET;
using System;
using Random = UnityEngine.Random;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using MLAPI.NetworkedVar.Collections;

public class MatchManager : MonoBehaviour
{



    public List<Color> AllPlayerColors;

    public NetworkedList<Color> PlayerColors = new NetworkedList<Color>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.ServerOnly, ReadPermission = NetworkedVarPermission.Everyone });




    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Subscribe to network manager events for connection and disconnection
        NetworkingManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkingManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkingManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // if the network manager is valid
        if (NetworkingManager.Singleton == null)
            return;

        // Unsubscribe to network manager events
        NetworkingManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkingManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkingManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    //--------------------------------------------------------------------------------------
    // HandleClientConnected: Event when a client is connected to a server
    //--------------------------------------------------------------------------------------
    private void HandleClientConnected(ulong ulClientID)
    {
        // if player is a standard client disable panel
        if (ulClientID == NetworkingManager.Singleton.LocalClientId)
        {
            //SetPlayerColor();
        }
    }

    //--------------------------------------------------------------------------------------
    // HandleClientDisconnected: Event when a client is disconnected from a server
    //--------------------------------------------------------------------------------------
    private void HandleClientDisconnected(ulong ulClientID)
    {
        // if player is a standard client enable panel
        if (ulClientID == NetworkingManager.Singleton.LocalClientId)
        {

        }
    }

    //--------------------------------------------------------------------------------------
    // HandleServerStarted: Event triggered when the server is started
    //--------------------------------------------------------------------------------------
    private void HandleServerStarted()
    {
        // if player is a host client 
        if (NetworkingManager.Singleton.IsHost)
        {
            //SetAvailableColors();
            //SetPlayerColor();
        }
    }






    [ServerRPC]
    public void SetAvailableColors()
    {
        for (int i = 0; i < AllPlayerColors.Count; i++)
        {
            PlayerColors.Add(AllPlayerColors[i]);
        }
    }

    [ServerRPC]
    public Color GetRandomAvailableColor()
    {
        return PlayerColors[Random.Range(0, PlayerColors.Count)];
    }

    [ServerRPC]// Maybe add a bool to this function so can add colors back when a player leaves
    public void UpdateAvailableColors(Color cColor)
    {
        for (int i = 0; i < PlayerColors.Count; i++)
        {
            if (PlayerColors[i] == cColor)
            {
                PlayerColors.RemoveAt(i);
            }
        }
    }

    public void SetPlayerColor()
    {
        ulong localclientID = NetworkingManager.Singleton.LocalClientId;

        if (!NetworkingManager.Singleton.ConnectedClients.TryGetValue(localclientID, out NetworkedClient networkClient))
            return;

        if (!networkClient.PlayerObject.TryGetComponent<Player>(out Player player))
            return;

        Color randcolor = GetRandomAvailableColor();
        UpdateAvailableColors(randcolor);
        player.SetBodyColorRPC(randcolor);
    }
}

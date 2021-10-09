//--------------------------------------------------------------------------------------
// Purpose: A script for handling various match/game management.
//
// Description: The main purpose of this script will be to run the scene/network functions 
// and variables needed to get a gamemode functioning.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
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

//--------------------------------------------------------------------------------------
// MatchManager object. Inheriting from NetworkedBehaviour.
//--------------------------------------------------------------------------------------
public class MatchManager : NetworkedBehaviour
{
    // PLAYER CONNECTION SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Player Connection Settings:")]

    // a public list of colors representing all colors available.
    [LabelOverride("Player Colors")] [Tooltip("A list of all colors the players can spawn as when the server starts.")]
    public List<Color> m_lcAllPlayerColors;
    //--------------------------------------------------------------------------------------

    // PRIVATE NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // private network variable for a list of colors, used for setting colors for the different player clients
    private NetworkedList<Color> mn_lcAvailablePlayerColors = new NetworkedList<Color>(new NetworkedVarSettings { WritePermission = NetworkedVarPermission.Everyone, ReadPermission = NetworkedVarPermission.Everyone });
    //--------------------------------------------------------------------------------------

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

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
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
            // Set the colour of each player picked randomly
            SetPlayerColor();
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
            // Initialize network list of colors based on the public list
            SetAvailableColors();

            // Set the colour of the host player randomly
            HandleClientConnected(NetworkingManager.Singleton.LocalClientId);
        }
    }

    // WORK IN PROGRESS, SEEMS TO BE A BUG WITH SERVERRPC AND THESE ALL NEED TO BE
    // RUN ON THE SERVER WITH THE NETWORK VARIABLE BEING PROTECTED TO SERVER ONLY TOO.

    //-------------// WIP //-------------//
    private void SetAvailableColors()
    {
        for (int i = 0; i < m_lcAllPlayerColors.Count; i++)
        {
            mn_lcAvailablePlayerColors.Add(m_lcAllPlayerColors[i]);
        }
    }

    private Color GetRandomAvailableColor()
    {
        return mn_lcAvailablePlayerColors[Random.Range(0, mn_lcAvailablePlayerColors.Count)];
    }

    private void UpdateAvailableColors(Color cColor)
    {
        for (int i = 0; i < mn_lcAvailablePlayerColors.Count; i++)
        {
            if (mn_lcAvailablePlayerColors[i] == cColor)
                mn_lcAvailablePlayerColors.RemoveAt(i);
        }
    }

    private void RefreshColors(Color cColor)
    {
        mn_lcAvailablePlayerColors.Remove(cColor);

        for (int i = 0; i < m_lcAllPlayerColors.Count; i++)
        {
            mn_lcAvailablePlayerColors.Add(m_lcAllPlayerColors[i]);
        }
    }

    private void SetPlayerColor()
    {
        ulong ulLocalClientID = NetworkingManager.Singleton.LocalClientId;

        if (!NetworkingManager.Singleton.ConnectedClients.TryGetValue(ulLocalClientID, out NetworkedClient networkClient))
            return;

        if (!networkClient.PlayerObject.TryGetComponent<Player>(out Player oPlayer))
            return;

        Color cRandomColor = Color.white;

        if (mn_lcAvailablePlayerColors.Count > 1)
        {
            cRandomColor = GetRandomAvailableColor();
            UpdateAvailableColors(cRandomColor);
        }

        else if (mn_lcAvailablePlayerColors.Count == 1)
        {
            cRandomColor = mn_lcAvailablePlayerColors[0];
            RefreshColors(cRandomColor);
        }

        oPlayer.SetBodyColor(cRandomColor);
    }
    //-------------// WIP //-------------//
}

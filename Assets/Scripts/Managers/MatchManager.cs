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
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

//--------------------------------------------------------------------------------------
// MatchManager object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class MatchManager : NetworkBehaviour
{
    // PLAYER CONNECTION SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Player Connection Settings:")]

    // a public list of colors representing all colors available.
    [LabelOverride("Player Colors")] [Tooltip("A list of all colors the players can spawn as when the server starts.")]
    public List<Color> m_acAllPlayerColors;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PRIVATE NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // private network variable for a list of colors, used for setting colors for the different player clients
    private NetworkList<Color> mn_acAvailablePlayerColors = new NetworkList<Color>(new NetworkVariableSettings { SendTickrate = 0 });
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Initialization
    //--------------------------------------------------------------------------------------
    private void Start()
    {
        // Subscribe to network manager events for connection and disconnection
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // if the network manager is valid
        if (NetworkManager.Singleton == null)
            return;

        // Unsubscribe to network manager events
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
    }

    //--------------------------------------------------------------------------------------
    // HandleClientConnected: Event when a client is connected to a server
    //
    // Params:
    //      ulClientID: ulong value for the connected client
    //--------------------------------------------------------------------------------------
    private void HandleClientConnected(ulong ulClientID)
    {
        // if player is a standard client disable panel
        if (ulClientID == NetworkManager.Singleton.LocalClientId)
        {
            // Set the colour of each player picked randomly
            AssignPlayerColorServerRpc(ulClientID);
        }
    }

    //--------------------------------------------------------------------------------------
    // HandleClientDisconnected: Event when a client is disconnected from a server
    //
    // Params:
    //      ulClientID: ulong value for the disconnecting client
    //--------------------------------------------------------------------------------------
    private void HandleClientDisconnected(ulong ulClientID)
    {
        // if player is a standard client enable panel
        if (ulClientID == NetworkManager.Singleton.LocalClientId)
        {
        }
    }

    //--------------------------------------------------------------------------------------
    // HandleServerStarted: Event triggered when the server is started
    //--------------------------------------------------------------------------------------
    private void HandleServerStarted()
    {
        // if player is a host client 
        if (NetworkManager.Singleton.IsHost)
        {
            // Initialize network list of colors based on the public list
            InitializeAvailablePlayerColorsServerRpc();

            // Set the colour of the host player randomly
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
        }
    }

    //--------------------------------------------------------------------------------------
    // InitializeAvailablePlayerColorsServerRpc: Server function for initializing the
    // mn_acAvailablePlayerColors variable on ServerStart.
    //--------------------------------------------------------------------------------------
    [ServerRpc]
    private void InitializeAvailablePlayerColorsServerRpc()
    {
        // loop through all colors and initalize the available colors array.
        for (int i = 0; i < m_acAllPlayerColors.Count; i++)
        {
            mn_acAvailablePlayerColors.Add(m_acAllPlayerColors[i]);
        }
    }

    //--------------------------------------------------------------------------------------
    // AssignPlayerColorServerRpc: A server function for assigning a random color to a
    // newly connected player/client.
    //
    // Params:
    //      ulClientID: ulong value for client requesting player color change.
    //--------------------------------------------------------------------------------------
    [ServerRpc (RequireOwnership = false)]
    private void AssignPlayerColorServerRpc(ulong ulClientID)
    {
        // Get the clients network object, return if not valid.
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(ulClientID, out NetworkClient ncClient))
            return;

        // Get the clients player object, return if not valid
        if (!ncClient.PlayerObject.TryGetComponent<Player>(out Player oPlayer))
            return;

        // new color variable for assigning random color
        Color cRandomColor = Color.white;

        // if mn_acAvailablePlayerColors count is at least 2
        if (mn_acAvailablePlayerColors.Count > 1)
        {
            // Get a random color and assign to color variable
            cRandomColor = mn_acAvailablePlayerColors[Random.Range(0, mn_acAvailablePlayerColors.Count)];

            // loop through mn_acAvailablePlayerColors array
            for (int i = 0; i < mn_acAvailablePlayerColors.Count; i++)
            {
                // if the random color is in the array remove it
                if (mn_acAvailablePlayerColors[i] == cRandomColor)
                    mn_acAvailablePlayerColors.RemoveAt(i);
            }
        }

        // if only one value is left in mn_acAvailablePlayerColors
        else if (mn_acAvailablePlayerColors.Count == 1)
        {
            // Assign last color in array to the color variable
            cRandomColor = mn_acAvailablePlayerColors[0];

            // Remove the final value from the array, making it empty
            mn_acAvailablePlayerColors.Remove(cRandomColor);

            // Loop through all colors and re-initialize mn_acAvailablePlayerColors array
            for (int i = 0; i < m_acAllPlayerColors.Count; i++)
            {
                mn_acAvailablePlayerColors.Add(m_acAllPlayerColors[i]);
            }
        }

        // else if the mn_acAvailablePlayerColors count is 0
        else
        {
            // Print error message, change color of player to black to make it clear there is a problem
            Debug.Log("Error: mn_acAvailablePlayerColors is Empty?");
            cRandomColor = Color.black;
        }

        // Send color to player object
        oPlayer.SetBodyColorServerRpc(cRandomColor);
    }
}

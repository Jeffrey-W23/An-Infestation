//--------------------------------------------------------------------------------------
// Purpose: Give function to Pause Menu buttons.
//
// Description: Main script for pause menu user interface functions, used for network
// connection settings, returning to menu, resuming, etc.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine.UI;
using System.Text;

//--------------------------------------------------------------------------------------
// PauseMenu object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class PauseMenu : NetworkedBehaviour
{
    // Menu Settings //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Menu Settings:")]

    // public gameobject for pause menus main panel
    [LabelOverride("Menu Panel")] [Tooltip("The main panel used for the pause menu.")]
    public GameObject m_gMenuPanel;

    // public inputfield for ip address connect address
    [LabelOverride("IP Address Text Field")] [Tooltip("The inputfield object used for entering connect address on the pause menu")]
    public InputField m_ifIPAddress;

    // public inputfield for password setting/entering for servers.
    [LabelOverride("Password Text Field")] [Tooltip("The inputfield object used for setting/entering the password for servers")]
    public InputField m_ifPassword;
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
    public void Update()
    {
        // if escape key is pressed enable the menu
        if (Input.GetKey(KeyCode.Escape))
            m_gMenuPanel.SetActive(true);
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
            m_gMenuPanel.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // HandleClientDisconnected: Event when a client is disconnected from a server
    //--------------------------------------------------------------------------------------
    private void HandleClientDisconnected(ulong ulClientID)
    {
        // if player is a standard client enable panel
        if (ulClientID == NetworkingManager.Singleton.LocalClientId)
            m_gMenuPanel.SetActive(true);
    }

    //--------------------------------------------------------------------------------------
    // HandleServerStarted: Event triggered when the server is started
    //--------------------------------------------------------------------------------------
    private void HandleServerStarted()
    {
        // if player is a host client 
        if (NetworkingManager.Singleton.IsHost)
        {
            // Run handle client connected event
            HandleClientConnected(NetworkingManager.Singleton.LocalClientId);
        }
    }

    //--------------------------------------------------------------------------------------
    // ApprovalCheck: Event running approval check for server connection
    //
    // Param:
    //      baConnectionData: An Array of bytes representing the connection data for server setup
    //      ulClientID: A ulong for Client ID
    //      Callback: A NetworkingManager.ConnectionApprovedDelegate for setting callback delegate.
    //--------------------------------------------------------------------------------------
    private void ApprovalCheck(byte[] baConnectionData, ulong ulClientID, NetworkingManager.ConnectionApprovedDelegate Callback)
    {
        // New string for internal server password
        string strPassword = Encoding.ASCII.GetString(baConnectionData);

        // Check if the password matches
        bool bApproveConnection = strPassword == m_ifPassword.text;

        // new variables for spawn position of players
        Vector2 v2SpawnPos = Vector2.zero;
        Quaternion qSpawnRot = Quaternion.identity;

        // Switch through each connected client (Other than host)
        switch (NetworkingManager.Singleton.ConnectedClients.Count)
        {
            // Set spawn pos of next connected player
            case 1:
                v2SpawnPos = new Vector2(0f, 0f);
                qSpawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;

            // Set spawn pos of next connected player
            case 2:
                v2SpawnPos = new Vector2(0f, 5f);
                qSpawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
        }

        // Run approval callback
        Callback(true, null, bApproveConnection, v2SpawnPos, qSpawnRot);
    }

    //--------------------------------------------------------------------------------------
    // Host: Trigger function for Host button on pause menu
    //--------------------------------------------------------------------------------------
    public void HostButton()
    {
        // subscribe network manager connection approve event
        NetworkingManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        // Get Networking Mananger singleton and start hosting server / set spawn pos of host player
        NetworkingManager.Singleton.StartHost(new Vector2(-5f, 0f), Quaternion.Euler(0f, 0f, 0f));
    }

    //--------------------------------------------------------------------------------------
    // Join: Trigger function for Join button on pause menu
    //--------------------------------------------------------------------------------------
    public void JoinButton()
    {
        // Check to see if password entered is correct
        NetworkingManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(m_ifPassword.text);

        // if no ip is entered use default local address for networking manager connect address
        // otherwise use IP address from the Input field variable.
        if (m_ifIPAddress.text.Length <= 0)
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
        else
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = m_ifIPAddress.text;

        // Get networking manager singleton and connect to client
        NetworkingManager.Singleton.StartClient();
    }

    //--------------------------------------------------------------------------------------
    // LeaveButton: Trigger function for Leave button on pause menu
    //--------------------------------------------------------------------------------------
    public void LeaveButton()
    {
        // If the client leaving is the host
        if (NetworkingManager.Singleton.IsHost)
        {
            // Stop hosting session and unsubscribe from approval check event
            NetworkingManager.Singleton.StopHost();
            NetworkingManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }

        // else if the player is just a standard client disconnect them
        else if (NetworkingManager.Singleton.IsClient)
            NetworkingManager.Singleton.StopClient();

        // Turn off menu panel
        m_gMenuPanel.SetActive(true);
    }
}
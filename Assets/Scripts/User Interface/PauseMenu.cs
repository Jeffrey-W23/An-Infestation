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
using UnityEngine.SceneManagement;

//--------------------------------------------------------------------------------------
// PauseMenu object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class PauseMenu : NetworkBehaviour
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
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    public void Update()
    {
        // Check to make sure there is no inventory container open
        if (!InventoryManager.m_oInstance.IsInventoryOpen())
        {
            // if escape key is pressed enable the menu
            if (Input.GetKeyDown(KeyCode.Escape) && !m_gMenuPanel.activeSelf)
            {
                // Set cursor back to default
                CustomCursor.m_oInstance.SetDefaultCursor();

                // enable the menu
                m_gMenuPanel.SetActive(true);
            }

            else if (Input.GetKeyDown(KeyCode.Escape) && m_gMenuPanel.activeSelf)
            {
                // Set cursor back to previous
                CustomCursor.m_oInstance.SetPreviousCursor();

                // disable the menu
                m_gMenuPanel.SetActive(false);
            }
        }
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
            m_gMenuPanel.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // HandleClientDisconnected: Event when a client is disconnected from a server
    //
    // Params:
    //      ulClientID: ulong value for the disconnecting client
    //--------------------------------------------------------------------------------------
    private void HandleClientDisconnected(ulong ulClientID)
    {
        // if player is a standard client
        if (ulClientID == NetworkManager.Singleton.LocalClientId)
        {
            // Set cursor back to default
            CustomCursor.m_oInstance.SetDefaultCursor();

            //  enable pause panel
            m_gMenuPanel.SetActive(true);
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
            // Run handle client connected event
            HandleClientConnected(NetworkManager.Singleton.LocalClientId);
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
    private void ApprovalCheck(byte[] baConnectionData, ulong ulClientID, NetworkManager.ConnectionApprovedDelegate Callback)
    {
        // New string for internal server password
        string strPassword = Encoding.ASCII.GetString(baConnectionData);

        // Check if the password matches
        bool bApproveConnection = strPassword == m_ifPassword.text;

        ////////////////// TEMP: MOVE TO MATCH MANAGER LATER AND SPAWN RANDOMLY //////////////////

        // new variables for spawn position of players
        Vector2 v2SpawnPos = Vector2.zero;
        Quaternion qSpawnRot = Quaternion.identity;

        // Switch through each connected client (Other than host)
        switch (NetworkManager.Singleton.ConnectedClients.Count)
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

        ////////////////// TEMP: MOVE TO MATCH MANAGER LATER AND SPAWN RANDOMLY //////////////////

        // Run approval callback
        Callback(true, null, bApproveConnection, v2SpawnPos, qSpawnRot);
    }

    //--------------------------------------------------------------------------------------
    // Host: Trigger function for Host button on pause menu
    //--------------------------------------------------------------------------------------
    public void HostButton()
    {
        // subscribe network manager connection approve event
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        // Get Networking Mananger singleton and start hosting server / set spawn pos of host player
        NetworkManager.Singleton.StartHost(new Vector2(-5f, 0f), Quaternion.Euler(0f, 0f, 0f));
    }

    //--------------------------------------------------------------------------------------
    // Join: Trigger function for Join button on pause menu
    //--------------------------------------------------------------------------------------
    public void JoinButton()
    {
        // Check to see if password entered is correct
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(m_ifPassword.text);

        // if no ip is entered use default local address for networking manager connect address
        // otherwise use IP address from the Input field variable.
        if (m_ifIPAddress.text.Length <= 0)
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = "127.0.0.1";
        else
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = m_ifIPAddress.text;

        // Get networking manager singleton and connect to client
        NetworkManager.Singleton.StartClient();
    }

    //--------------------------------------------------------------------------------------
    // LeaveButton: Trigger function for Leave button on pause menu
    //--------------------------------------------------------------------------------------
    public void LeaveButton()
    {
        // If the client leaving is the host
        if (NetworkManager.Singleton.IsHost)
        {
            // Stop hosting session and unsubscribe from approval check event
            NetworkManager.Singleton.StopHost();
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;

            // Reset the scene ready for reconnection
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // else if the player is just a standard client
        else if (NetworkManager.Singleton.IsClient)
        {
            // disconnect said client
            NetworkManager.Singleton.StopClient();

            // Reset the scene ready for reconnection
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Turn off menu panel
        m_gMenuPanel.SetActive(true);

        // Set cursor back to default
        CustomCursor.m_oInstance.SetPreviousCursor();
    }
}
//--------------------------------------------------------------------------------------
// Purpose: Give function to Pause Menu buttons.
//
// Description: Main script for pause menu user interface functions, used for network
// settings, returning to menu, resuming, etc.
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

//--------------------------------------------------------------------------------------
// PauseMenu object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class PauseMenu : MonoBehaviour
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
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // Host: Trigger function for Host button on pause menu
    //--------------------------------------------------------------------------------------
    public void HostButton()
    {
        // Get Networking Mananger singleton and start hosting server
        NetworkingManager.Singleton.StartHost();

        // Turn off menu panel
        m_gMenuPanel.SetActive(false);
    }

    //--------------------------------------------------------------------------------------
    // Join: Trigger function for Join button on pause menu
    //--------------------------------------------------------------------------------------
    public void JoinButton()
    {
        // if no ip is entered use default local address for networking manager connect address
        // otherwise use IP address from the Input field variable.
        if (m_ifIPAddress.text.Length <= 0)
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
        else
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = m_ifIPAddress.text;

        // Get networking manager singleton and connect to client
        NetworkingManager.Singleton.StartClient();

        // Turn off menu panel
        m_gMenuPanel.SetActive(false);
    }
}
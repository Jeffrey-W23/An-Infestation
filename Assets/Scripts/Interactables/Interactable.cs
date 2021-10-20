//--------------------------------------------------------------------------------------
// Purpose: The base script for all interactable objects.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Interactable object. Inheriting from NetworkBehaviour.
//--------------------------------------------------------------------------------------
public class Interactable : NetworkBehaviour
{
    // INTERACTABLE SETTINGS //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Interactable Settings:")]

    // public bool for if the interactble is single use or not.
    [LabelOverride("Single Use?")] [Tooltip("Is this interactable object for single use or can it be interacted with again later?")]
    public bool m_bSingleUse;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------
    
    // VISUAL INDICATOR //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Visual Indicator:")]

    // public text mesh pro for the visual indicator of the interactable
    [LabelOverride("Text Mesh")] [Tooltip("The TextMesh pro prefab to display when interaction is possible.")]
    public TextMeshPro m_tmpBtnVisual;

    // public vector3 for the offest of the visual indicators position.
    [LabelOverride("Position Offset")] [Tooltip("A Vector3 for setting an offset for the position of the Visual Indicator.")]
    public Vector3 m_v3BtnVisualPosOffset = new Vector3(0.0f,0.0f,0.0f);

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // AUDIO //
    //--------------------------------------------------------------------------------------
    // Title for this section of public values.
    [Header("Audio:")]

    // public bool to give the option for audio or not.
    [LabelOverride("Has Interact Audio?")] [Tooltip("Is there interaction audio for this interactable object?")]
    public bool m_bInteractAudio = false;

    // public audioclip for interaction audio.
    [LabelOverride("Interact Audio")] [Tooltip("The audio clip for what will play when the interactable is interacted with.")]
    public AudioClip m_acInteractAudio;

    // Leave a space in the inspector.
    [Space]
    //--------------------------------------------------------------------------------------

    // PROTECTED VALUES //
    //--------------------------------------------------------------------------------------
    // protected hashset of collider2Ds used for the potential objects up for interaction
    protected HashSet<Collider2D> m_acPotentialInteracts = new HashSet<Collider2D>();

    // protected audio source
    protected AudioSource m_asAudioSource;

    // protected bool for if the object has been interacted with or not.
    protected bool m_bInteracted;

    // protected bool for if an object is interactable
    protected bool m_bInteractable = false;

    // protected bool for if an interact has been used when the interactable is set to single use
    protected bool m_bInteractUsed = false;

    // protected bool for holding interaction from happening
    protected bool m_bHoldInteraction = false;
    //--------------------------------------------------------------------------------------

    // PROTECTED NETWORKED VARS //
    //--------------------------------------------------------------------------------------
    // protected network variable bool used for checking if an interactable has been collected
    protected NetworkVariableBool m_nbInteractableCollected = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone }, false);
    //--------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Set the interacted bool to false for starting.
        m_bInteracted = false;

        // Instantiate gameobject for visual indicator and set up
        m_tmpBtnVisual = Instantiate(m_tmpBtnVisual, transform);
        m_tmpBtnVisual.transform.localPosition = new Vector3(m_v3BtnVisualPosOffset.x, m_v3BtnVisualPosOffset.y, m_v3BtnVisualPosOffset.z);
        m_tmpBtnVisual.transform.SetParent(transform, false);
        m_tmpBtnVisual.gameObject.SetActive(false);

        //if there is an audio clip on the object.
        // get the audiosource component of the interactable object
        if (m_bInteractAudio)
            m_asAudioSource = GetComponent<AudioSource>();
    }

    //--------------------------------------------------------------------------------------
    // FixedUpdate: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    protected void FixedUpdate()
    {
        // Loop through each collider in the potential interacts hashset
        foreach (Collider2D c in m_acPotentialInteracts)
        {
            // Prepare for a potential interaction
            InitiatePotentialInteraction(c.gameObject.GetComponent<Player>());
        }
    }

    //--------------------------------------------------------------------------------------
    // OnTriggerEnter2D: Function is called when the Collider cObject enters the trigger.
    //
    // Param:
    //      cObject: The other Collider invloved in the collision.
    //--------------------------------------------------------------------------------------
    protected void OnTriggerEnter2D(Collider2D cObject)
    {
        // if the collider is a player, add it to the list of potential interactions
        if (cObject.tag == "Player")
            m_acPotentialInteracts.Add(cObject);
    }

    //--------------------------------------------------------------------------------------
    // OnTriggerExit: OnTriggerExit is called when the Collider cObject exits the trigger.
    //
    // Param:
    //      cObject: The other Collider invloved in the collision.
    //--------------------------------------------------------------------------------------
    protected void OnTriggerExit2D(Collider2D cObject)
    {
        // if the collider is a player
        if (cObject.tag == "Player")
        {
            // Remove collider from list of potential interactions
            m_acPotentialInteracts.Remove(cObject);

            // check if interaction has been used and then cancel the potential interaction
            if (!m_bInteractUsed)
                CancelPotentialInteraction(cObject.gameObject.GetComponent<Player>());
        }
    }

    //--------------------------------------------------------------------------------------
    // OnEnable: Function that will call when this gameObject is enabled.
    //--------------------------------------------------------------------------------------
    protected void OnEnable()
    {
        // subscribe to value change event for collected bool
        m_nbInteractableCollected.OnValueChanged += OnInteractableCollectedChange;
    }

    //--------------------------------------------------------------------------------------
    // OnDestroy: Function that will call on this gameObjects destruction.
    //--------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        // Unsubscribe from collected on change event
        m_nbInteractableCollected.OnValueChanged -= OnInteractableCollectedChange;
    }

    //--------------------------------------------------------------------------------------
    // OnInteractableCollectedChange: Event function for on Interactable Collected bool change.
    //
    // Params:
    //      bOldState: The previous bool state before the change event triggered.
    //      bNewState: The new bool state that triggered the event change.
    //--------------------------------------------------------------------------------------
    protected void OnInteractableCollectedChange(bool bOldState, bool bNewState)
    {
        // Finalize the interaction
        if (bNewState)
            FinalizeCollectableInteractionClientRpc();
    }

    //--------------------------------------------------------------------------------------
    // InitiatePotentialInteraction: A function for preparing the interactable object for 
    // potential interaction.
    //
    // Params:
    //      oPlayer: Player gameobject interacting with the interactable object
    //--------------------------------------------------------------------------------------
    protected void InitiatePotentialInteraction(Player oPlayer)
    {
        // If the player attempting interaction is the local player
        if (oPlayer.IsLocalPlayer)
        {
            // if collides is player and not interacted or interactable
            if (!m_bInteracted && !m_bInteractable && !m_bInteractUsed && !m_bHoldInteraction)
            {
                // Display debug message showing interaction.
                if (oPlayer.m_bDebugMode)
                    Debug.Log(oPlayer + ": Subscribed for Interaction");

                // Subscribe the function InteractedWith with the InteractionEvent delegate event
                oPlayer.InteractionCallback += InitiateInteraction;

                // activate gameobject for visual indicator
                m_tmpBtnVisual.gameObject.SetActive(true);

                // set the object as interactable
                m_bInteractable = true;
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // CancelPotentialInteraction: A function for Unsubscribing the interaction, canceling 
    // interaction set up and reseting object ready for new interaction.
    //
    // Params:
    //      oPlayer: Player gameobject interacting with the interactable object
    //--------------------------------------------------------------------------------------
    protected void CancelPotentialInteraction(Player oPlayer)
    {
        // If the player attempting interaction is the local player
        if (oPlayer.IsLocalPlayer)
        {
            // Display debug message showing interaction.
            if (oPlayer.m_bDebugMode)
                Debug.Log("Exited Interactable Trigger");

            // if callback is not null
            if (oPlayer.InteractionCallback != null)
            {
                // Display debug message showing interaction.
                if (oPlayer.m_bDebugMode)
                    Debug.Log("Unsubscribed for Interaction");

                // Unsubscribe the function InteractedWith with the InteractionEvent delegate event
                oPlayer.InteractionCallback -= InitiateInteraction;

                // deactivate gameobject for visual indicator
                m_tmpBtnVisual.gameObject.SetActive(false);

                // not interactable anymore
                m_bInteractable = false;
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // InitiateInteraction: Virtual function for what Interactable objects do once they 
    // have been interacted with.
    //
    // Params:
    //      oPlayer: Player gameobject interacting with the interactable object
    //--------------------------------------------------------------------------------------
    protected virtual void InitiateInteraction(Player oPlayer)
    {
        // Display debug message showing interaction.
        if (oPlayer.m_bDebugMode)
            Debug.Log("Interaction Triggered");

        // if the interactable is not single use.
        if (!m_bSingleUse)
        {
            // the object has been interacted with.
            m_bInteracted = true;

            // Play Interaction Audio.
            if (m_bInteractAudio)
                m_asAudioSource.PlayOneShot(m_acInteractAudio);

            // Exit interaction and make object interactable again
            CancelPotentialInteraction(oPlayer);
        }

        // if the interactable is single use.
        else if (m_bSingleUse)
        {
            // the object has been interacted with.
            m_bInteracted = true;

            // set interact to used 
            m_bInteractUsed = true;

            // Make sure that the function is being unsubscribed from the delegate.
            oPlayer.InteractionCallback -= InitiateInteraction;

            // deactivate and destory gameobject for visual indicator
            Destroy(m_tmpBtnVisual);

            // if interaction audio is being used.
            if (m_bInteractAudio)
            {
                // Stop any previously playing audio
                if (m_asAudioSource.isPlaying)
                    m_asAudioSource.Stop();

                // Play Interaction Audio.
                m_asAudioSource.PlayOneShot(m_acInteractAudio);
            }
        }
    }

    //--------------------------------------------------------------------------------------
    // FinalizeCollectableInteractionClientRpc: Client function for finalizing interaction 
    // if the interactable is a collectable item.
    //--------------------------------------------------------------------------------------
    [ClientRpc]
    protected void FinalizeCollectableInteractionClientRpc()
    {
        // Unsubscribe from collected on change event
        m_nbInteractableCollected.OnValueChanged -= OnInteractableCollectedChange;

        // Destory the gameobject for the interactable
        Destroy(gameObject);
    }
}
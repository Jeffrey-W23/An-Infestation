//--------------------------------------------------------------------------------------
// Purpose: The base script for all interactable objects.
//
// Author: Thomas Wiltshire
//--------------------------------------------------------------------------------------

// using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------------------------------------------
// Interactable object. Inheriting from MonoBehaviour.
//--------------------------------------------------------------------------------------
public class Interactable : MonoBehaviour
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

    // public text mesh for the visual indicator of the interactable
    [LabelOverride("Text Mesh")] [Tooltip("The TextMesh prefab to display when interaction is possible.")]
    public TextMesh m_tmBtnVisual;

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
    // protected player script for getting the player objects attached script.
    protected Player m_oPlayerObject;

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
    
    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    protected void Awake()
    {
        // Set the player script object to the player script.
        m_oPlayerObject = GameObject.Find("Player").GetComponent<Player>();

        // Set the interacted bool to false for starting.
        m_bInteracted = false;

        // Instantiate gameobject for visual indicator and set active to false.
        m_tmBtnVisual = Instantiate(m_tmBtnVisual, transform);
        m_tmBtnVisual.gameObject.SetActive(false);
        
        //if there is an audio clip on the object.
        if (m_bInteractAudio)
        {
            // get the audiosource component of the interactable object
            m_asAudioSource = GetComponent<AudioSource>();
        }
    }

    //--------------------------------------------------------------------------------------
    // OnTriggerStay2D: OnTriggerStay2D is called when the Collider cObject enters the trigger
    // and contiunes to trigger it.
    //
    // Param:
    //      cObject: The other Collider invloved in the collision.
    //--------------------------------------------------------------------------------------
    private void OnTriggerStay2D(Collider2D cObject)
    {
        // if collides is player and not interacted or interactable
        if (cObject.tag == "Player" && !m_bInteracted && !m_bInteractable && !m_bInteractUsed && !m_bHoldInteraction)
        {
            // Display debug message showing interaction.
            if (m_oPlayerObject.m_bDebugMode)
                Debug.Log("Subscribed for Interaction");

            // Subscribe the function InteractedWith with the InteractionEvent delegate event
            m_oPlayerObject.InteractionCallback += InteractedWith;

            // activate gameobject for visual indicator
            m_tmBtnVisual.gameObject.SetActive(true);

            // set the object as interactable
            m_bInteractable = true;
        }
    }

    //--------------------------------------------------------------------------------------
    // OnTriggerExit: OnTriggerExit is called when the Collider cObject exits the trigger.
    //
    // Param:
    //      cObject: The other Collider invloved in the collision.
    //--------------------------------------------------------------------------------------
    private void OnTriggerExit2D(Collider2D cObject)
    {
        // if collide is player
        if (cObject.tag == "Player" && !m_bInteractUsed)
        {
            // Display debug message showing interaction.
            if (m_oPlayerObject.m_bDebugMode)
                Debug.Log("Exited Interactable Trigger");

            // call exit interaction function.
            ExitInteract();
        }
    }
    
    //--------------------------------------------------------------------------------------
    // ExitInteract: Unsubscribes the interaction, resets object for new interaction.
    //--------------------------------------------------------------------------------------
    protected void ExitInteract()
    {
        // if callback is not null
        if (m_oPlayerObject.InteractionCallback != null)
        {
            // Display debug message showing interaction.
            if (m_oPlayerObject.m_bDebugMode)
                Debug.Log("Unsubscribed for Interaction");

            // Unsubscribe the function InteractedWith with the InteractionEvent delegate event
            m_oPlayerObject.InteractionCallback -= InteractedWith;

            // deactivate gameobject for visual indicator
            m_tmBtnVisual.gameObject.SetActive(false);

            // not interactable anymore
            m_bInteractable = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // InteractedWith: Virtual function for what Interactable objects do once they have 
    // been interacted with.
    //--------------------------------------------------------------------------------------
    protected virtual void InteractedWith()
    {
        // Display debug message showing interaction.
        if (m_oPlayerObject.m_bDebugMode)
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
            ExitInteract();
        }

        // if the interactable is single use.
        else if (m_bSingleUse)
        {
            // the object has been interacted with.
            m_bInteracted = true;

            // set interact to used 
            m_bInteractUsed = true;

            // Make sure that the function is being unsubscribed from the delegate.
            m_oPlayerObject.InteractionCallback -= InteractedWith;

            // deactivate and destory gameobject for visual indicator
            Destroy(m_tmBtnVisual);

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
}
//--------------------------------------------------------------------------------------
// Purpose: Custom Attribute for overriding labels.
//--------------------------------------------------------------------------------------

// Using, etc
using UnityEngine;

// Check if using the editor.
#if UNITY_EDITOR
using UnityEditor;
#endif

//--------------------------------------------------------------------------------------
// LabelOverrideAttribute object. Inheriting from PropertyAttribute. Used for overriding
// the public inspector labels for the varaibles.
//--------------------------------------------------------------------------------------
public class LabelOverrideAttribute : PropertyAttribute
{
    // public string for label to replace.
    public string m_strLabel;

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    public LabelOverrideAttribute(string strLabel)
    {
        // set label.
        this.m_strLabel = strLabel;
    }

    // Check if using the editor.
#if UNITY_EDITOR

    //--------------------------------------------------------------------------------------
    // ThisPropertyDrawer object. Inheriting from PropertyDrawer. For replacing the 
    // actual text.
    //--------------------------------------------------------------------------------------
    [CustomPropertyDrawer(typeof(LabelOverrideAttribute))]
    public class ThisPropertyDrawer : PropertyDrawer
    {
        //--------------------------------------------------------------------------------------
        // OnGUI: OnGUI is called for rendering and handling GUI events.
        // 
        // Param:
        //      rPosition: The position of the gui event.
        //      spProperty: The property of the event.
        //      guiLabel: The label of the gui event.
        //--------------------------------------------------------------------------------------
        public override void OnGUI(Rect rPosition, SerializedProperty spProperty, GUIContent guiLabel)
        {
            // Set the local label to the inspector value label.
            var propertyAttribute = this.attribute as LabelOverrideAttribute;
            guiLabel.text = propertyAttribute.m_strLabel;
            EditorGUI.PropertyField(rPosition, spProperty, guiLabel);
        }
    }
#endif
}
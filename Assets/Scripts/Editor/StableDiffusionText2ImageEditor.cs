using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector to add a models list as a drop-down selection UI 
/// and add a "Generate" button for the StableDiffusionImage.
/// </summary>
[CustomEditor(typeof(text2img))]
public class StableDiffusionText2ImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        text2img myComponent = (text2img)target;
        
        // Draw the drop-down list for the Samplers list
        myComponent.selectedSampler = EditorGUILayout.Popup("Sampler", myComponent.selectedSampler, myComponent.samplersList);

        // Draw the drop-down list for the Models list
        myComponent.selectedModel = EditorGUILayout.Popup("Model", myComponent.selectedModel, myComponent.modelsList);

        // Apply the changes to the serialized object
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate"))
            myComponent.Generate();
    }
}

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(UIControl))]
public class UIControlCustomEditor : ButtonEditor
{
	public override void OnInspectorGUI()
	{
		serializedObject.UpdateIfRequiredOrScript();	
		
		var targetButton = (UIControl)target;
		var control = serializedObject.FindProperty("Control");
		EditorGUILayout.PropertyField(control, new GUIContent("Control"));
		serializedObject.ApplyModifiedProperties();
		// Show default inspector property editor
		base.OnInspectorGUI();
	}
}
using UnityEditor;
using UnityEngine;
using ARVisionFrameWork.Core;

namespace ARVisionFrameWork.Core
{
    [CustomEditor(typeof(ARVisionPortalManager))]
    public class ARVisionPortalManagerCustomEditor : Editor
    {
        public SerializedProperty _hasEntered;
        public SerializedProperty _wasInFront;
        public SerializedProperty _inOtherWorld;
        public SerializedProperty _isColliding;
        public SerializedProperty _shaderStatus;       
        private SerializedProperty _materials;
        private SerializedProperty _device;

        public SerializedProperty _portalEnter;

        private void OnEnable()
        {
            _hasEntered = serializedObject.FindProperty("hasEntered");
            _wasInFront = serializedObject.FindProperty("wasInFront");
            _inOtherWorld = serializedObject.FindProperty("inOtherWorld");
            _isColliding = serializedObject.FindProperty("isColliding");

            _shaderStatus = serializedObject.FindProperty("shaderStatus");          
            _materials = serializedObject.FindProperty("materials");
            _device = serializedObject.FindProperty("device");

            _portalEnter = serializedObject.FindProperty("portalEnter");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_hasEntered);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_wasInFront);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_inOtherWorld);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_isColliding);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_shaderStatus);
            EditorGUILayout.Space();            
            EditorGUILayout.PropertyField(_materials);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_device);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_portalEnter);
            EditorGUILayout.Space();

            ARVisionPortalManager mScript = (ARVisionPortalManager)target;
            if (GUILayout.Button("Fill Material List"))
            {
                mScript.FillPortalMaterialList();
            }

            serializedObject.ApplyModifiedProperties(); //Apply values from editor to scriptable object asset.
        }


    }
}



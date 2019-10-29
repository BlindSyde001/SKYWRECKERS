﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class SetupProjectSettings : EditorWindow
    {
        [MenuItem("Tools/Third Person System/Setup Project Settings")]
        public static void ShowWindow()
        {
            GetWindow<SetupProjectSettings>(true, "Setup Project Settings");
        }

        private void OnGUI()
        {
            GUIStyle button = new GUIStyle(EditorStyles.miniButton);
            button.fontSize = 12;
            button.stretchWidth = false;
            button.fixedWidth = 175;
            button.fixedHeight = 35;

            GUIStyle label = new GUIStyle(EditorStyles.wordWrappedLabel);
            label.fontStyle = FontStyle.Bold;
            label.fontSize = 12;
            label.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();

            GUILayout.Label("Here you can set Input Manager and Layer Collisions. It will override your current settings.", label);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Set Input", button))
            {
                SetupInputManager();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Set Layers Collision", button))
            {
                UpdateLayers();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void UpdateLayers()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layersProperty = tagManager.FindProperty("layers");

            AddNewLayer(layersProperty, 13, "Ragdoll");
            AddNewLayer(layersProperty, 14, "Ground");
            AddNewLayer(layersProperty, 15, "Character");
            AddNewLayer(layersProperty, 16, "StepUp");
            AddNewLayer(layersProperty, 17, "LowerClimb");
            AddNewLayer(layersProperty, 18, "Climb");
            AddNewLayer(layersProperty, 19, "DroppableLedge");
            AddNewLayer(layersProperty, 20, "Ladder");
            AddNewLayer(layersProperty, 21, "Vault");
            AddNewLayer(layersProperty, 22, "Cover");
            AddNewLayer(layersProperty, 23, "Item");
            AddNewLayer(layersProperty, 24, "Enemy");

            tagManager.ApplyModifiedProperties();

            Physics.IgnoreLayerCollision(13, 15, true);
        }

        private static void AddNewLayer(SerializedProperty property, int index, string name)
        {
            var propertyElement = property.GetArrayElementAtIndex(index);
            propertyElement.stringValue = name;
        }

        public static void SetupInputManager()
        {
            // Add mouse definitions
            AddAxis(new InputAxis() { name = "Walk", positiveButton = "left shift"});
            AddAxis(new InputAxis() { name = "Roll", positiveButton = "x"});
            AddAxis(new InputAxis() { name = "Crouch", positiveButton = "c"});
            AddAxis(new InputAxis() { name = "Jump", positiveButton = "space"});
            AddAxis(new InputAxis() { name = "Zoom", positiveButton = "mouse 1"});
            AddAxis(new InputAxis() { name = "Drop", positiveButton = "x"});
            AddAxis(new InputAxis() { name = "Toggle", positiveButton = "t"});
            AddAxis(new InputAxis() { name = "RightWeapon", positiveButton = "6"});
            AddAxis(new InputAxis() { name = "LeftWeapon", positiveButton = "4"});
            AddAxis(new InputAxis() { name = "Fire", positiveButton = "mouse 0"});
            AddAxis(new InputAxis() { name = "Reload", positiveButton = "r"});
            AddAxis(new InputAxis() { name = "Interact", positiveButton = "e"});
            
        }


        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        private static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }

    }
}
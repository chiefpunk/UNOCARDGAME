//#define UAS
//#define CHUPA
#define SMA

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SellReadMe))]
public class SellReadMeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Edit Game Settings (Admob, In-app Purchase..)", EditorStyles.boldLabel);

        if (GUILayout.Button("Edit Game Settings", GUILayout.MinHeight(40)))
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Uno/MyCombo/GameMaster.prefab");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("2. Game Documentation", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Full Documentation", GUILayout.MinHeight(40)))
        {
            Application.OpenURL("https://drive.google.com/open?id=1VthNqPx-0zhm6-00_qI4ePEAmqpQAPckHAmtklacacg");
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Build For iOS Guide", GUILayout.MinHeight(40)))
        {
            Application.OpenURL("https://drive.google.com/open?id=1rkgXuyFlJ2BhyNZkcn5ASuHunNExDwW5ypmFdXcd0uA");
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("3. My Other Great Source Codes", EditorStyles.boldLabel);
        if (GUILayout.Button("Pixel Art - Color by Number", GUILayout.MinHeight(30)))
        {
#if UAS
            Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/pixel-art-color-by-number-117587");
#elif CHUPA
            Application.OpenURL("https://www.chupamobile.com/unity-arcade/pixel-art-color-by-number-top-free-game-20127");
#elif SMA
            Application.OpenURL("https://www.sellmyapp.com/downloads/pixel-art-color-by-number-top-free-game/");
#endif
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("1Line: One-stroke Puzzle", GUILayout.MinHeight(30)))
        {
#if UAS
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/1line-one-stroke-puzzle-118439");
#elif CHUPA
            Application.OpenURL("https://www.chupamobile.com/unity-family/1line-one-stroke-puzzle-20370");
#elif SMA
            Application.OpenURL("https://www.sellmyapp.com/downloads/1line-one-stroke-puzzle/");
#endif
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Sudoku Pro", GUILayout.MinHeight(30)))
        {
#if UAS
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/sudoku-pro-118434");
#elif CHUPA
            Application.OpenURL("https://www.chupamobile.com/unity-arcade/sudoku-pro-20433");
#elif SMA
            Application.OpenURL("https://www.sellmyapp.com/downloads/sudoku-pro/");
#endif
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Knife Hit (Ketchapp)", GUILayout.MinHeight(30)))
        {
#if UAS
            Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/knife-hit-115843");
#elif CHUPA
            Application.OpenURL("https://www.chupamobile.com/unity-arcade/knife-hit-unity-clone-20090");
#elif SMA
            Application.OpenURL("https://www.sellmyapp.com/downloads/knife-hit-unity-clone/");
#endif
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("4. Contact Us For Support", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Email: ", "moana.gamestudio@gmail.com");
    }
}
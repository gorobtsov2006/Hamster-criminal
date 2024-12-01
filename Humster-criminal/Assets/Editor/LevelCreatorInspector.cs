using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(LevelCreator))]
[ExecuteInEditMode]
public class LevelCreatorInspector : Editor
{
    Dictionary<ElementTypes, Texture> textureHolder = new Dictionary<ElementTypes, Texture>();
    private void OnEnable()
    {
        textureHolder.Add(ElementTypes.Empty, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Wall/empty.png"));
        textureHolder.Add(ElementTypes.Wall, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Wall/wall.png"));
        textureHolder.Add(ElementTypes.Hamster, (Texture)EditorGUIUtility.Load("Assets/Sprites/Characters/Hamster/Hamster_right.png"));
        textureHolder.Add(ElementTypes.Box, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Objects/box.png"));
        textureHolder.Add(ElementTypes.Goal, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Objects/goal.png"));
        textureHolder.Add(ElementTypes.IsWord, (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/isWord.png"));
        textureHolder.Add(ElementTypes.HamsterWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/hamsterWord.png"));
        textureHolder.Add(ElementTypes.WallWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/wallWord.png"));
        textureHolder.Add(ElementTypes.GoalWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/goalWord.png"));
        textureHolder.Add(ElementTypes.BoxWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/boxWord.png"));
        textureHolder.Add(ElementTypes.YouWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/youWord.png"));
        textureHolder.Add(ElementTypes.PushWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/pushWord.png"));
        textureHolder.Add(ElementTypes.WinWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/winWord.png"));
        textureHolder.Add(ElementTypes.StopWord, (Texture)EditorGUIUtility.Load("Assets/Sprites/Tiles/Words/stopWord.png"));
    }
    ElementTypes currentSelected = ElementTypes.Empty;
    public override void OnInspectorGUI()
    {
        // emptyTexture = (Texture)EditorGUIUtility.Load("Assets/EditorDefaultResources/empty.png");

        base.OnInspectorGUI();
        GUILayout.Label("Current Selected : " + currentSelected.ToString());

        LevelCreator levelCreator = (LevelCreator)target;
        int rows = (int)Mathf.Sqrt(levelCreator.level.Count);
        //int currentI = levelCreator.level.Count-1;
        GUILayout.BeginVertical();
        for (int r = rows - 1; r >= 0; r--)
        {

            GUILayout.BeginHorizontal();
            for (int c = 0; c < rows; c++)
            {
                if (GUILayout.Button(textureHolder[levelCreator.level[c + ((rows) * r)]], GUILayout.Width(50), GUILayout.Height(50)))
                {
                    levelCreator.level[c + ((rows) * r)] = currentSelected;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        int count = 0;
        foreach (KeyValuePair<ElementTypes, Texture> e in textureHolder)
        {
            count++;
            if (GUILayout.Button(e.Value, GUILayout.Width(50), GUILayout.Height(50)))
            {
                currentSelected = e.Key;
            }
            if (count % 4 == 0)
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }

        GUILayout.EndVertical();
    }
}

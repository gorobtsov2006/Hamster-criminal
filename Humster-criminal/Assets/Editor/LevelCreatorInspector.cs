using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
        LevelCreator levelCreator = (LevelCreator)target;

        // Отображение настроек сетки
        GUILayout.Label("Grid Settings");
        levelCreator.gridWidth = EditorGUILayout.IntField("Width", levelCreator.gridWidth);
        levelCreator.gridHeight = EditorGUILayout.IntField("Height", levelCreator.gridHeight);

        if (GUILayout.Button("Generate Grid"))
        {
            levelCreator.GenerateLevel(); // Генерация новой сетки
            EditorUtility.SetDirty(levelCreator); // Сохранение изменений
        }

        GUILayout.Space(10);

        GUILayout.Label("Current Selected: " + currentSelected.ToString());

        // Отображение сетки
        int rows = levelCreator.gridHeight;
        int cols = levelCreator.gridWidth;

        GUILayout.BeginVertical();
        for (int r = rows - 1; r >= 0; r--)
        {
            GUILayout.BeginHorizontal();
            for (int c = 0; c < cols; c++)
            {
                int index = c + (cols * r); // Рассчитываем индекс элемента в списке
                if (index >= 0 && index < levelCreator.level.Count) // Проверка, что индекс не выходит за пределы
                {
                    if (GUILayout.Button(textureHolder[levelCreator.level[index]], GUILayout.Width(50), GUILayout.Height(50)))
                    {
                        levelCreator.level[index] = currentSelected;
                        EditorUtility.SetDirty(levelCreator); // Помечаем объект как изменённый
                    }
                }
                else
                {
                    GUILayout.Button("?", GUILayout.Width(50), GUILayout.Height(50)); // Пустая кнопка для некорректных индексов
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);

        // Отображение выбора элементов
        GUILayout.Label("Select Element:");
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
        GUILayout.EndHorizontal();
    }
}

using UnityEditor;
using UnityEngine;

public class DistributeObjectsTool : EditorWindow
{
    [MenuItem("Tools/Distribute Selected Objects on Z")]
    private static void DistributeOnZ()
    {
        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        Undo.RecordObjects(Selection.transforms, "Distribute on Z");

        // Сортируем объекты по имени (можно изменить на сортировку по X или Y)
        var sortedObjects = Selection.gameObjects;
        System.Array.Sort(sortedObjects, (a, b) => a.name.CompareTo(b.name));

        // Размещаем с шагом 1f по Z
        for (int i = 0; i < sortedObjects.Length; i++)
        {
            Vector3 pos = sortedObjects[i].transform.position;
            pos.z = i * 1f; // Каждый следующий объект +1 по Z
            sortedObjects[i].transform.position = pos;
        }

        Debug.Log($"Distributed {sortedObjects.Length} objects on Z-axis.");
    }
}
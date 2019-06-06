using UnityEngine;
using UnityEditor;

public class MoanaWindowEditor
{
    [MenuItem("Moana Games/Clear all playerprefs")]
    static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
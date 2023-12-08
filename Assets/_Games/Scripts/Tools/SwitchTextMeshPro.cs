#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(SwitchTextMeshPro))]
public class SwitchTextMeshProEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Switch"))
        {
            SwitchTextMeshPro x = target as SwitchTextMeshPro;
            x.Switch();
        }
    }
}
public class SwitchTextMeshPro : MonoBehaviour
{
    public Font font;
    public void Switch()
    {
        TextMeshProUGUI[] textMeshPros = FindObjectsOfType<TextMeshProUGUI>(true);
        for (int i = 0; i < textMeshPros.Length; i++)
        {
            GameObject gx = textMeshPros[i].gameObject;
            string text = textMeshPros[i].text;
            DestroyImmediate(textMeshPros[i]);
            var tx = gx.AddComponent<Text>();
            if (font)
            {
                tx.font = font;
            }
            tx.text = text;
            EditorUtility.SetDirty(gx);
        }
    }
}
#endif
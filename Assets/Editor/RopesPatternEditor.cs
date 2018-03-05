using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RopesPattern)), CanEditMultipleObjects]
public class RopesPatternEditor : Editor {

    RopesPattern targetScript;
    PatternElement e;
    Rope r;
    Collectible c;

    Vector2 previewPos = new Vector2(0, -50);

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneGUI;
        targetScript = (RopesPattern)target;
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneGUI;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Test Random Generation"))
        {
            targetScript.GeneratePattern();
            SceneView.RepaintAll();
        }
    }

    void SceneGUI(SceneView sceneView)
    {
        if (!targetScript)
            return;

        Color tmp = Handles.color;

        if (targetScript.Elements != null)
        {
            for (int i = 0; i < targetScript.Elements.Length; i++)
            {
                e = targetScript.Elements[i];

                Handles.color = Color.white;
                Handles.Label(e.Pos + previewPos, i.ToString());

                if(!e.Element && !e.IsCollectible && e.RopeLength < 3)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(e.Pos + previewPos + Vector2.one, e.Pos + previewPos - Vector2.one);
                    Handles.DrawLine(e.Pos + previewPos + new Vector2(-1, 1), e.Pos + previewPos + new Vector2(1, -1));
                    continue;
                }

                if(!e.Element)
                {
                    if (e.IsCollectible)
                        DrawCollectible(e.Pos + previewPos);
                    else
                        DrawRope(e.Pos + previewPos, e.RopeLength);

                    continue;
                }

                r = e.Element.GetComponent<Rope>();
                c = e.Element.GetComponent<Collectible>();

                if (r)
                    DrawRope(e.Pos + previewPos, r.RopeLength);

                if (c)
                    DrawCollectible(e.Pos + previewPos);

                if (!c && !r)
                {
                    Handles.color = Color.white;
                    Handles.DrawWireCube(e.Pos + previewPos, Vector3.one);
                }
            }
        }

        Handles.color = tmp;
        Handles.DrawWireCube(previewPos, new Vector3(targetScript.PatternExtents.x * 2, targetScript.PatternExtents.y * 2, 1));
    }

    void DrawRope(Vector3 pos, float length)
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(pos, Vector3.forward, 1);
        Handles.DrawLine(pos, pos - Vector3.up * length);
    }

    void DrawCollectible(Vector3 pos)
    {
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(pos, Vector3.forward, 1);
    }
}

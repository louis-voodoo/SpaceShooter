using UnityEngine;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using System.Linq;

public class Executor : EditorWindow
{
    private GameObject gameObject;
    private Vector2 scrollPosition;
    private List<bool> foldouts;
    private string status;
    private bool showPrivates;
    private bool showInherited;
    private bool lockSelection;
    private bool showArguments;
    private Regex humanizePattern;
    private GUIStyle borderStyle;
    private Dictionary<string, object[]> parameters = new Dictionary<string, object[]>();

    [MenuItem("Voodoo/Tools/Executor")]
    public static void OpenExecutor()
    {
        EditorWindow.GetWindow<Executor>().Show();
    }

    protected void OnEnable()
    {
        titleContent = new GUIContent("Executor");

        scrollPosition = Vector2.zero;
        foldouts = new List<bool>();
        status = "";
        showPrivates = false;
        showInherited = false;
        lockSelection = false;
        humanizePattern = new Regex(@"(?<=[^A-Z])([A-Z])", System.Text.RegularExpressions.RegexOptions.None);

        Texture2D black = new Texture2D(1, 1);
        black.SetPixels(0, 0, 1, 1, new Color[] { new Color(0, 0, 0, 1) });
        black.Apply();
        borderStyle = new GUIStyle();
        borderStyle.normal.background = black;
        borderStyle.hover.background = black;
        borderStyle.active.background = black;
        borderStyle.focused.background = black;
    }

    protected void OnDestroy()
    {
        UnityEngine.Object.DestroyImmediate(borderStyle.normal.background);
    }

    protected void OnSelectionChange()
    {
        parameters = new Dictionary<string, object[]>();
        Repaint();
    }

    protected void OnGUI()
    {
        if (!lockSelection || gameObject == null)
            gameObject = Selection.activeGameObject;

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (gameObject == null)
            EditorGUILayout.Space();
        else
            EditorGUILayout.LabelField(gameObject.name);
        showArguments = GUILayout.Toggle(showArguments, new GUIContent("A", "Show Arguments"), EditorStyles.toolbarButton, GUILayout.Width(24));
        showPrivates = GUILayout.Toggle(showPrivates, new GUIContent("P", "Show Private"), EditorStyles.toolbarButton, GUILayout.Width(24));
        showInherited = GUILayout.Toggle(showInherited, new GUIContent("I", "Show Inherited"), EditorStyles.toolbarButton, GUILayout.Width(24));
        lockSelection = GUILayout.Toggle(lockSelection, new GUIContent("L", "Lock Selection"), EditorStyles.toolbarButton, GUILayout.Width(24));
        GUILayout.EndHorizontal();

        if (gameObject == null)
            return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        Component[] components = gameObject.GetComponents<Component>();
        while (foldouts.Count < components.Length)
            foldouts.Add(false);
        for (int i = 0; i < components.Length; ++i)
        {
            Component component = components[i];
            // If the game object contains broken components (missing mono script instance)
            // the list of components will actually contain null components...
            if (component == null)
                continue;
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (showPrivates)
                bindingFlags |= BindingFlags.NonPublic;
            if (!showInherited)
                bindingFlags |= BindingFlags.DeclaredOnly;
            MethodInfo[] methods = component.GetType().GetMethods(bindingFlags);
            if (methods.Length > 0)
            {
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], Humanize(component.GetType().Name));
                if (foldouts[i])
                {
                    int index = 0;
                    for (int j = 0; j < methods.Length; ++j)
                    {
                        MethodInfo method = methods[j];
                        if (!method.IsSpecialName && !method.ContainsGenericParameters)
                        {
                            int rowCount = 1;
                            if(showArguments)
                                rowCount += (method.GetParameters().Count(m=>m.ParameterType == typeof(int) || m.ParameterType == typeof(float) || m.ParameterType == typeof(string) || m.ParameterType == typeof(bool) || m.ParameterType == typeof(Enum)));
                            Rect lastRect = GUILayoutUtility.GetLastRect();
                            if (index % 2 == 1)
                            {
                                DrawRect(new Rect(0, lastRect.y + lastRect.height + 2, this.position.width, 20f * rowCount));
                            }

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(20);
                            EditorGUILayout.LabelField(Humanize(method.Name));

                            if (!parameters.ContainsKey(method.Name))
                            {
                                parameters[method.Name] = new object[method.GetParameters().Length];
                            }
                            else if (parameters[method.Name].Length != method.GetParameters().Length)
                            {
                                parameters[method.Name] = new object[method.GetParameters().Length];
                            }

                            if (GUILayout.Button("\u25B6", GUILayout.Width(20f)))
                            {
                                object returnValue = method.Invoke(component, parameters[method.Name]);
                                if (method.ReturnType == typeof(void))
                                    status = "";
                                else
                                    status = Humanize(method.Name) + " : " + returnValue;
                            }
                            EditorGUILayout.EndHorizontal();

                            if (showArguments)
                            {
                                // Parameters
                                for (int p = 0; p < method.GetParameters().Length; p++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space();
                                    ParameterInfo paramInfo = method.GetParameters()[p];
                                    Type paramType = paramInfo.ParameterType;
                                    if (paramType == typeof(int))
                                    {
                                        if (parameters[method.Name][p] == null || parameters[method.Name][p].GetType() != typeof(int))
                                            parameters[method.Name][p] = 0;
                                        parameters[method.Name][p] = EditorGUILayout.IntField(paramInfo.Name, (int)parameters[method.Name][p]);
                                        rowCount++;
                                    }
                                    else if (paramType == typeof(float))
                                    {
                                        if (parameters[method.Name][p] == null || parameters[method.Name][p].GetType() != typeof(float))
                                            parameters[method.Name][p] = 0f;
                                        parameters[method.Name][p] = EditorGUILayout.FloatField(paramInfo.Name, (float)parameters[method.Name][p]);
                                        rowCount++;
                                    }
                                    else if (paramType == typeof(bool))
                                    {
                                        if (parameters[method.Name][p] == null || parameters[method.Name][p].GetType() != typeof(bool))
                                            parameters[method.Name][p] = false;
                                        parameters[method.Name][p] = EditorGUILayout.Toggle(paramInfo.Name, (bool)parameters[method.Name][p]);
                                        rowCount++;
                                    }
                                    else if (paramType == typeof(string))
                                    {
                                        if (parameters[method.Name][p] == null || parameters[method.Name][p].GetType() != typeof(string))
                                            parameters[method.Name][p] = "";
                                        parameters[method.Name][p] = EditorGUILayout.TextField(paramInfo.Name, (string)parameters[method.Name][p]);
                                        rowCount++;
                                    } else if (paramType.BaseType == typeof(Enum)) {
                                        if (parameters[method.Name][p] == null || parameters[method.Name][p].GetType().BaseType != typeof (Enum))
                                            parameters[method.Name][p] = Enum.GetValues(paramType).GetValue(0);
                                        parameters[method.Name][p] = EditorGUILayout.EnumPopup((Enum)parameters[method.Name][p]);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }

                            }

                            index++;

                        }
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.Box(GUIContent.none, borderStyle, GUILayout.Height(1));

        if (!string.IsNullOrEmpty(status))
            EditorGUILayout.SelectableLabel("Return value of " + status, GUILayout.Height(20));
    }

    protected void DrawRect(Rect a_rect)
    {
        EditorGUI.DrawRect(a_rect, EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f): Color.grey);
    }

    string Humanize(string name)
    {
        return humanizePattern.Replace(name, EvaluateHumanizeMatch);
    }

    private static string EvaluateHumanizeMatch(Match match)
    {
        return " " + match.Groups[1];
    }
}

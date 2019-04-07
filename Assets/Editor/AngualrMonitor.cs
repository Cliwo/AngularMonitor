using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AngualrMonitor : EditorWindow
{
    public const int MinWidth = 480;
    public const int MinHeight = 600;
    string[] Menus = { "Angular Velocity", "Angular Accelerate", "Euler Angles" };
    [SerializeField] GameObject selectedGameObject = null;
    Material[] lineMats;
    Material lineMat;
    string prefix;
    int selectedTab;
    int startedFrame = -1;

    GraphView graphView;

    private GameObject Head;

    [MenuItem("Window/Angualr Monitor")]
    private static void ShowWindow() {
        var window = GetWindow<AngualrMonitor>();
        window.titleContent = new GUIContent("Angualr Monitor");
        window.minSize = new Vector2(MinWidth, MinHeight);
        window.Show();
    }
    private void OnEnable() {
        var data = EditorPrefs.GetString("Angular Monitor", JsonUtility.ToJson(this, true));
        JsonUtility.FromJsonOverwrite(data, this);
    }

    private void OnDisable() {
        var data = JsonUtility.ToJson(this, true);
        EditorPrefs.SetString("Angular Monitor", data);
    }

    private void Awake()
    {
        lineMats = new Material[3];
        lineMats[0] = new Material(Shader.Find("Unlit/Color"));
        lineMats[0].color = Color.red;
        lineMats[1] = new Material(Shader.Find("Unlit/Color"));
        lineMats[1].color = Color.green;
        lineMats[2] = new Material(Shader.Find("Unlit/Color"));
        lineMats[2].color = Color.blue;
        
        lineMat = new Material(Shader.Find("Unlit/Color"));
        graphView = new GraphView();
        graphView.Init(MinWidth, MinHeight);
    }
    private void Update()
    {
        Repaint();
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Run"))
            {
                if (startedFrame == -1)
                    startedFrame = Time.frameCount;
                Time.timeScale = 1.0f;
            }
            if (GUILayout.Button("Pause"))
            {
                Time.timeScale = 0.0f;
            }
            if (GUILayout.Button("Stop"))
            {
                startedFrame = Time.frameCount;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            selectedGameObject = EditorGUILayout.ObjectField("Target Object", selectedGameObject, typeof(GameObject), true, null) as GameObject;
            prefix = EditorGUILayout.TextField("Object Prefix : ", "Prefix");
        }
        GUILayout.EndHorizontal();

        selectedTab = GUILayout.Toolbar(selectedTab, Menus);
        switch (selectedTab)
        {
            case 0:
                break;

            case 1:
                break;

            default:

                break;
        }

        if(selectedGameObject == null)
            return;

        FindJoints(selectedGameObject);
        string[] jointName = {"Head", 
        //"Spine", "RightShoulder", "RightElbow", "RightWrist", 
        //"LeftShoulder", "LeftElbow", "LeftWrist", "RightHips", "RightKnee", "RightAnkle", "LeftHips", "LeftKnee", "LeftAnkle"
        };
        int itemCount = 4;
        Rect rectLast = GUILayoutUtility.GetLastRect();
        float RestOfRegion = position.height - rectLast.y - rectLast.height;
        
        GUILayout.BeginVertical();
        for(int i = 0 ; i<itemCount; i++)
        {
            GUILayout.BeginHorizontal();
            for(int j = 0 ; j < itemCount; j++)
            {
                GUILayout.BeginVertical();
                Rect rect = GUILayoutUtility.GetRect(10,position.width / itemCount,10 , RestOfRegion / itemCount); 
                GUI.Box(rect, "");
                if(i*itemCount + j < jointName.Length)
                {
                    GraphView(jointName[i*itemCount + j], rect, lineMats, Head.transform.rotation.eulerAngles);
                }    
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            // Rect rect = GUILayoutUtility.GetRect(10, position.width, 10, RestOfRegion / itemCount); 
            // GraphView(jointName[i], rect, lineMats);
        }
        GUILayout.EndVertical();
    }

    private void FindJoints(GameObject root)
    {
        Head = root.transform.Find("Head").gameObject;
    }
    private void GraphView(string label, Rect rect, Material[] linMats, Vector3 val)
    {
        // GUILayout.BeginVertical();
        GUILayout.Label(label);
        for(int i= 0 ; i < 3; i++)
        {
            //(position.height- rectLast.y - rectLast.height)/3
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();

                GL.Clear(true, false, Color.black);
                lineMats[i].SetPass(0);

                GL.Begin(GL.LINES);
                GL.Vertex( new Vector3(0, rect.height/6 * (2 * i + 1), 0)); GL.Vertex(new Vector3(rect.width, rect.height/6 * (2 * i + 1), 0));
                EulerAngle(new Rect(0, rect.height/3 * i, rect.width, rect.height/3), 1.0f, val[i]);
                // if(i%2 == 0)
                //     Sin(new Rect(0, rect.height/3 * i, rect.width, rect.height/3), 1.0f);
                // else
                //     Cos(new Rect(0, rect.height/3 * i, rect.width, rect.height/3), 1.0f);
            
                GL.End();

                GL.PopMatrix();
                GUI.EndClip();
            }
        }
        // GUILayout.EndVertical();
    }
    private void EulerAngle(Rect rect, float fillRatio, float EulerAngle)
    {
        EulerAngle = ClampAngle(EulerAngle);
        float currentFrame = (float)EditorApplication.timeSinceStartup * 100; 
        float currentPosition = rect.width * fillRatio;
        int windowSize = 300;
        List<Vector3> positions = new List<Vector3>();
        for(float i = currentFrame ; i >= currentFrame-windowSize; i--)
        {
            float xPos = ( (i - (currentFrame-windowSize)) / (windowSize)) * currentPosition;
            float yPos = rect.height/2 - EulerAngle / 180.0f * rect.height/2;
            positions.Add(new Vector3(rect.x + xPos, rect.y + yPos,0));
        }

        foreach(var p in positions)
        {
            GL.Vertex(p);
        }
    }

    private float ClampAngle(float angle)
    {
        return Mathf.Clamp(angle, -180.0f, 180.0f); //이거 말고 다른걸로 바꿔야함
    }

    private void Sin(Rect rect, float fillRatio)
    {
        float currentFrame = (float)EditorApplication.timeSinceStartup * 100; 
        float currentPosition = rect.width * fillRatio;
        int windowSize = 300;
        List<Vector3> positions = new List<Vector3>();
        for(float i = currentFrame ; i >= currentFrame-windowSize; i--)
        {
            float xPos = ( (i - (currentFrame-windowSize)) / (windowSize)) * currentPosition;
            float yPos = rect.height/2 - Mathf.Sin(i * Mathf.PI / 180.0f) * rect.height/2;
            positions.Add(new Vector3(rect.x + xPos, rect.y + yPos,0));
        }

        foreach(var p in positions)
        {
            GL.Vertex(p);
        }
    }
    private void Cos(Rect rect, float fillRatio)
    {
        float currentFrame = (float)EditorApplication.timeSinceStartup * 100; 
        float currentPosition = rect.width * fillRatio;
        int windowSize = 300;
        List<Vector3> positions = new List<Vector3>();
        for(float i = currentFrame ; i >= currentFrame-windowSize; i--)
        {
            float xPos = ( (i - (currentFrame-windowSize)) / (windowSize)) * currentPosition;
            float yPos = rect.height/2 - Mathf.Cos(i * Mathf.PI / 180.0f) * rect.height/2;
            positions.Add(new Vector3(rect.x + xPos, rect.y + yPos,0));
        }

        foreach(var p in positions)
        {
            GL.Vertex(p);
        }
    }
    // private void Sin(float maxWidth, float maxHeight, float fillRatio)
    // {
    //     float currentFrame = (float)EditorApplication.timeSinceStartup * 100; 
    //     float currentPosition = maxWidth * fillRatio;
    //     int windowSize = 300;
    //     List<Vector3> positions = new List<Vector3>();
    //     for(float i = currentFrame ; i >= currentFrame-windowSize; i--)
    //     {
    //         float xPos = ( (i - (currentFrame-windowSize)) / (windowSize)) * currentPosition;
    //         float yPos = maxHeight/2 - Mathf.Sin(i * Mathf.PI / 180.0f) * maxHeight/2;
    //         positions.Add(new Vector3(xPos, yPos,0));
    //     }

    //     foreach(var p in positions)
    //     {
    //         GL.Vertex(p);
    //     }
    // }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GraphView
{
    public Rect rect 
    {
        get { return rect; } 
        set
        {
            rect = value;
            // this.position = rect;
        }
    }
    Material lineMat;
    int MinWidth; 
    int MinHeight;
    AxisView x;
    AxisView y;
    AxisView z;

    public GraphView() { lineMat = new Material(Shader.Find("Unlit/Color")); }
    public void Init(int minWidth, int minHeight)
    {
        MinWidth = minWidth;
        MinHeight = minHeight;
        // x = new AxisView(rect.height / 3, rect.width / 3);
        // y = new AxisView(rect.height / 3, rect.width / 3);
        // z = new AxisView(rect.height / 3, rect.width / 3);
    }

    public void OnGUI()
    {
        // Rect rect1 = GUILayoutUtility.GetRect(MinWidth/2, this.position.width/2, MinHeight/2, this.position.height/2); 
        Rect rect1 = GUILayoutUtility.GetRect(MinWidth/2, rect.width/2, MinHeight/2, rect.height/2); 
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect1);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            lineMat.SetPass(0);
            
            GL.Begin(GL.LINES);
            GL.Color(Color.cyan);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(rect1.width, rect1.height, 0);
            GL.End();

            GL.PopMatrix(); //Push 다음에 꼭 필요하다.
            GUI.EndClip();
        }

        // Rect rect2 = GUILayoutUtility.GetRect(MinWidth/2, this.position.width/2, MinHeight/2, this.position.height/2); 
        Rect rect2 = GUILayoutUtility.GetRect(MinWidth/2, rect.width/2, MinHeight/2, rect.height/2); 
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect2);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            lineMat.SetPass(0);
            
            GL.Begin(GL.LINES);
            GL.Color(Color.cyan);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 100, 0);
            GL.End();

            GL.PopMatrix(); 
            GUI.EndClip();
        }
    }


    private class AxisView
    {
        public int frameWindowSize = 180; //180 개의 frame을 본다
        public List<Vector2> points; //소트 상태가 유지되어야한다.

        public float height { get; private set; }
        public float width { get; private set; }
        public AxisView(float height, float width)
        {
            points = new List<Vector2>();
            this.height = height;
            this.width = width;
        }

        public void OnGUI()
        {
            
            points.RemoveAt(0);
            Vector2 currentPoint = Vector2.zero; //현재 Angular정보를 구한다
            var x = (frameWindowSize - currentPoint.x) / frameWindowSize * width;
            var y = (((currentPoint.y) / 180.0f) + 1.0f) / 2.0f * height;
            points.Add(new Vector2(x, y));

            for (int i = 0; i < points.Count - 1; i++)
            {
                
                DrawLine(points[i], points[i + 1]);
            }
        }

        private void DrawLine(Vector2 a, Vector2 b)
        {

        }
    }
}

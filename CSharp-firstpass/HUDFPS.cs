using System;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
    private float accum;
    public float accumTime;
    public bool allowDrag = true;
    private Color color = Color.white;
    private int frames;
    public float frequency = 0.5f;
    public int nbDecimal = 1;
    public static string sFPS = string.Empty;
    public Rect startRect = new Rect(10f, 10f, 75f, 50f);
    private GUIStyle style;
    public bool updateColor = true;

    private void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(0f, 0f, this.startRect.width, this.startRect.height), sFPS + " FPS", this.style))
        {
        }
        if (this.allowDrag)
        {
            GUI.DragWindow(new Rect(0f, 0f, (float) Screen.width, (float) Screen.height));
        }
    }

    private void OnGUI()
    {
        if (this.style == null)
        {
            this.style = new GUIStyle(GUI.skin.label);
            this.style.normal.textColor = Color.white;
            this.style.alignment = TextAnchor.MiddleCenter;
        }
        this.startRect.x = 0f;
        this.startRect.y = Screen.height - this.startRect.height;
        GUI.color = !this.updateColor ? Color.white : this.color;
        this.startRect = GUI.Window(0, this.startRect, new GUI.WindowFunction(this.DoMyWindow), string.Empty);
    }

    private void Update()
    {
        this.accumTime += Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        this.frames++;
        if (this.accumTime >= this.frequency)
        {
            float num = this.accum / ((float) this.frames);
            sFPS = num.ToString("f" + Mathf.Clamp(this.nbDecimal, 0, 10));
            this.color = (num < 20f) ? ((num <= 10f) ? Color.yellow : Color.red) : Color.green;
            this.accumTime = 0f;
            this.accum = 0f;
            this.frames = 0;
        }
    }
}


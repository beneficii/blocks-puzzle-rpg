using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-30)]
public class ScreenLogger : MonoBehaviour
{

    private string log = "";
    private const int MAXCHARS = 10000;
    private Queue myLogQueue = new Queue();
    GUIStyle myStyle;
    void Start()
    {
        //Debug.Log("Screen logger started");
        myStyle = new GUIStyle
        {
            fontSize = 20
        };
        myStyle.normal.textColor = Color.black;
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("\n [" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue("\n" + stackTrace);
    }

    void Update()
    {
        while (myLogQueue.Count > 0)
            log = myLogQueue.Dequeue() + log;
        if (log.Length > MAXCHARS)
            log = log.Substring(0, MAXCHARS);
    }

    void OnGUI()
    {
        GUILayout.Label(log, myStyle);
    }
}
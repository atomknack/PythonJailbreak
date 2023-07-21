using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class LogToUI : MonoBehaviour
{
    // Adjust via the Inspector
    [SerializeField]
    private int maxLines = 8;
    private Queue<string> queue = new Queue<string>();

    [SerializeField]
    private string textElementName;
    private Label _textElement;

    private StringWriter consoleOut2;

    public void UpdateText(string newText)
    {
            _textElement.text = newText;
    }

    public void OnAfterPythonScriptRun()
    {
        UpdateText(consoleOut2.ToString());
        consoleOut2.GetStringBuilder().Clear();
    }

    void OnEnable()
    {
        VisualElement _rootUI = GetComponent<UIDocument>().rootVisualElement;
        _textElement = _rootUI.Q<Label>(textElementName);
        Application.logMessageReceivedThreaded += HandleLog;

        //consoleReader = new StreamReader(System.Console.OpenStandardOutput());

        consoleOut2 = new StringWriter();
        System.Console.SetOut(consoleOut2);

        //consoleOut3 = System.Console.Out;
        UKnack.PythonRunner.PythonRun.AfterPythonScriptRun += OnAfterPythonScriptRun;
    }
    void Start()
    {
        UKnack.PythonRunner.PythonRun.Stdout = consoleOut2;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
        UKnack.PythonRunner.PythonRun.AfterPythonScriptRun -= OnAfterPythonScriptRun;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        //consoleReader.

        //consoleReader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
       // builder.AppendLine(consoleReader.ReadToEnd());


        builder.AppendLine(consoleOut2.ToString());

        // builder.AppendLine(consoleOut3.ToString());

        foreach (string st in queue.Reverse())
        {
            builder.Append(st).Append("\n");
        }

        UpdateText( builder.ToString());
    }
}

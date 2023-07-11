//using UnityEditor.Scripting.Python;
//using RunPython;
using UnityEngine;
using UnityEngine.UIElements;

public class PythonNoJailRunner : MonoBehaviour
{
    public string ButtonName;
    private Button runScriptButton;
    public string TextSourceName;
    private TextField pythonScriptSource;
    private void OnEnable()
    {
        VisualElement _rootUI = GetComponent<UIDocument>().rootVisualElement;
        runScriptButton = _rootUI.Q<Button>(ButtonName);
        runScriptButton.RegisterCallback<ClickEvent>(ev => RunScript());
        pythonScriptSource = _rootUI.Q<TextField>(TextSourceName);

    }

    private void RunScript()
    {
        //PythonRunner.RunString(pythonScriptSource.text);
        //PySideExample.Run(pythonScriptSource.text);
        PythonRunner.PythonRun.RunScript(pythonScriptSource.text);
    }
}

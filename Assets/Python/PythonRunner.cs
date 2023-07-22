using Python.Runtime;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace UKnack.PythonNet
{
    public partial class PythonRunner : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _afterInit;
        public static PythonRunner Instance => 
            _instance;
        public CancellationToken CurrentScriptRunCancelationToken =>
            _secondThreadRunner._currentScriptRunCancelationTokenSource.Token;

        public void RunScriptWithScopeInSecondThread(string pythonScript) =>
            _secondThreadRunner.Run( ()=> RunScriptWithScope(pythonScript) );

        private readonly string PythonPath = Application.dataPath + "/StreamingAssets/python-3.11.4-embed-amd64/python311.dll";
        private static PythonRunner _instance;
        private SecondThreadRunner _secondThreadRunner;
        private System.IntPtr _threadState;


        private void OnEnable()
        {
            Debug.Log($"{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            if (_instance != null) 
                throw new System.Exception($"there is already another RythonRunner existing: {_instance.name} of {_instance.gameObject.name}");

            Runtime.PythonDLL = PythonPath;
            PythonEngine.Initialize();// mode: ShutdownMode.Reload);
            _threadState = PythonEngine.BeginAllowThreads();
            _secondThreadRunner = new SecondThreadRunner();

            _instance = this;
            _afterInit?.Invoke();
        }


        private void OnDisable()
        {
            if (_instance == null)
                throw new System.ArgumentNullException("Cannot properly disable script because singleton already null");
            if (_instance != this)
                throw new System.Exception($"Cannot properly stop script, because there is another singleton: {_instance.name} of {_instance.gameObject.name}");

            PythonEngine.EndAllowThreads(_threadState);
            PythonEngine.Shutdown();// ShutdownMode.Reload);

            _instance = null;
        }


        public static void RunScriptWithScope(string pythonScript, System.Action onSuccess = null)
        {
            using (Py.GIL())
            {
                try
                {
                    using (var scope = Py.CreateScope())
                    {
                        //                        scope.Exec($@"import os
                        //print(os.listdir())
                        //print(""example"")");
                        var scriptCompiled = PythonEngine.Compile(pythonScript);
                        scope.Execute(scriptCompiled);
                    }

                    onSuccess?.Invoke();
                }
                catch (System.Exception e)
                {
                    print(e);
                    print(e.StackTrace);
                }
            }
        }
    }
}

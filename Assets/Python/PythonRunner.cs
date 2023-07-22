using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UKnack.PythonNet
{
    public partial class PythonRunner : MonoBehaviour
    {
        public static PythonRunner Instance => 
            _instance;
        public CancellationToken CurrentScriptRunCancelationToken =>
            _secondThreadRunner._currentScriptRunCancelationTokenSource.Token;

        public void RunScriptWithScopeInSecondThread(string pythonScript) =>
            _secondThreadRunner.Run( ()=> RunScriptWithScope(pythonScript) );

        private readonly string PythonPath = Application.dataPath + "/StreamingAssets/python-3.11.4-embed-amd64/python311.dll";
        private static PythonRunner _instance;
        private SecondThreadRunner _secondThreadRunner;
        private IntPtr _threadState;


        private void OnEnable()
        {
            if (_instance != null) 
                throw new Exception($"there is already another RythonRunner existing: {_instance.name} of {_instance.gameObject.name}");

            Runtime.PythonDLL = PythonPath;
            PythonEngine.Initialize();// mode: ShutdownMode.Reload);
            _threadState = PythonEngine.BeginAllowThreads();
            _secondThreadRunner = new SecondThreadRunner();

            _instance = this;
        }


        private void OnDisable()
        {
            if (_instance == null)
                throw new ArgumentNullException("Cannot properly disable script because singleton already null");
            if (_instance != this)
                throw new Exception($"Cannot properly stop script, because there is another singleton: {_instance.name} of {_instance.gameObject.name}");

            PythonEngine.EndAllowThreads(_threadState);
            PythonEngine.Shutdown();// ShutdownMode.Reload);

            _instance = null;
        }


        public static void RunScriptWithScope(string pythonScript, Action onSuccess = null)
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
                catch (Exception e)
                {
                    print(e);
                    print(e.StackTrace);
                }
            }
        }
    }
}

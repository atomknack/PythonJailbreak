using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TreeEditor;
using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;
using UnityEngine.Events;

namespace UKnack.PythonNet
{
    [Obsolete("not tested")]
    public class CubeCommands : MonoBehaviour
    {
        [SerializeField]
        [ValidReference]
        private SOPublisher<Vector3> _move;

        private static CubeCommands _singleton = null;

        private CommandsForPython _cube = new();
        public class CommandsForPython
        {
            public static void move(float x, float y, float z) =>
                move(new Vector3(x, y, z));
            public static void move(Vector3 amount)
            {
                //////
                int sleep = 100;
                Debug.Log($"{nameof(CommandsForPython)}move - python thread sleep {sleep}");
                System.Threading.Thread.Sleep(sleep);
                //////

                SecondThreadCommandBridgeToMainThread.Singleton.RunInFixedUpdateAndReturnAfterCompletion
                    (()=> { 
                        _singleton._move.Publish(amount);
                    }, PythonRunner.Instance.CurrentScriptRunCancelationToken);
            }
        }

        private void Awake()
        {
            if (_move == null)
                throw new System.ArgumentNullException(nameof(_move));
            if (_singleton != null)
                throw new InvalidOperationException("Threre should be only one running");
            _singleton = this;
        }


        private PythonRunner _localReference;
        private void OnEnable()
        {
            _localReference = PythonRunner.Instance;
            ValidatePythonRunner();
            PythonRunner.Instance.NewScopeTorRunScript += AddToScope;
        }

        private void OnDisable()
        {
            ValidatePythonRunner();
            PythonRunner.Instance.NewScopeTorRunScript -= AddToScope;
        }

        private void ValidatePythonRunner()
        {
            if (_localReference == null)
                throw new ArgumentNullException(nameof(_localReference));
        }

        private void AddToScope(PyModule scope)
        {
            PyObject cube = _cube.ToPython();
            scope.Set("cube", cube);
        }

        /*
        public static void ApplyCommands()
        {
            using (Py.GIL())
            {
                // alternative way to do https://pythonnet.github.io/pythonnet/dotnet.html
                // PyObject cube = UKnack.PythonNet.CubeCommands.CommandsForPython.ToPython();


                PythonEngine.Exec(@"
import sys
import UKnack.PythonNet
cube = UKnack.PythonNet.CubeCommands.CommandsForPython
                    ");
            }

        }
        */
    }
}

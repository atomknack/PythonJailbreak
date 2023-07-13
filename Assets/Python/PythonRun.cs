using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Python.Runtime;
using System;
using System.IO;

namespace PythonRunner
{


    public class PythonRun : MonoBehaviour
    {
        public static event Action AfterPythonScriptRun;
        private static StringWriter s_stdout;
        public static StringWriter Stdout
        {
            set
            {
                s_stdout = value;
                Writer.RedirectStdOut();
            }
        }

        public readonly static Writer writer = new Writer();
        public class Writer
        {
            public static void write(string s)
            {
                s_stdout.Write(s);
            }
            public static void RedirectStdOut()
            {
                using (Py.GIL())
                {
                    PythonEngine.Exec(@"
import sys
import PythonRunner
sys.stdout = PythonRunner.PythonRun.writer
                    ");
                }

            }
        }

        public static void RunScript(string script)
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
                        var scriptCompiled = PythonEngine.Compile(script);
                        scope.Execute(scriptCompiled);
                    }

                    AfterPythonScriptRun?.Invoke();
                }
                catch (Exception e)
                {
                    print(e);
                    print(e.StackTrace);
                }

            }
        }

        void OnEnable()
        {
            Runtime.PythonDLL = Application.dataPath + "/StreamingAssets/python-3.11.4-embed-amd64/python311.dll";
            PythonEngine.Initialize();// mode: ShutdownMode.Reload);


        }

        public void OnApplicationQuit()
        {
            if (PythonEngine.IsInitialized)
            {
                Debug.Log("ending python");
                PythonEngine.Shutdown();// ShutdownMode.Reload);
            }
        }
    }

}
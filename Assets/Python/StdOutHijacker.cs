using Python.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace UKnack.PythonRunner
{
    [Obsolete("not tested")]
    public class StdOutHijacker : MonoBehaviour
    {
        public delegate void SomethingWriten(string message);
        public static event SomethingWriten StaticEvent;


        public readonly static Writer writer = new Writer(); //Used only for calling from python

        [SerializeField]
        private UnityEvent<string> _onWrite;

        private static StdOutHijacker _singleton = null;
        public class Writer
        {
            /// <summary>
            /// should be called only from python code
            /// </summary>
            /// <param name="s">string message that was written to stdout and is hijacked</param>
            public static void write(string s)
            {
                SecondThreadCommandBridgeToMainThread.Singleton.RunInFixedUpdateAndReturnAfterCompletion
                    (()=> { 
                        _singleton._onWrite.Invoke(s); 
                        StaticEvent?.Invoke(s);
                    });
            }
        }

        private void Awake()
        {
            if (_singleton != null)
                throw new InvalidOperationException("Threre should be only one running");
            RedirectStdOutHijack();
            _singleton = this;
        }


        public static void RedirectStdOutHijack()
        {
            using (Py.GIL())
            {
                PythonEngine.Exec(@"
import sys
import PythonRunner
sys.stdout = UKnack.PythonRunner.StdOutHijacker.writer
                    ");
            }

        }
    }
}
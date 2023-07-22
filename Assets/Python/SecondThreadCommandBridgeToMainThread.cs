using Python.Runtime;
using System;
using System.Threading;
using UnityEngine;

namespace UKnack.PythonNet
{

    // for use with PythonNet 
    // https://github.com/pythonnet/pythonnet/wiki
    // https://github.com/pythonnet/pythonnet/wiki/Threading
    [Obsolete("Not tested, not tested at all")]
    public class SecondThreadCommandBridgeToMainThread : MonoBehaviour
    {
        private static SecondThreadCommandBridgeToMainThread _singleton = null;
        public static SecondThreadCommandBridgeToMainThread Singleton => _singleton;

        private int _mainThreadID;

        private int _threadSleepTimeForHaveCommand = 0;
        public int ThreadSleepTimeForHaveCommand
        {
            get => _threadSleepTimeForHaveCommand;
            set
            {
                if (value < 0)
                    throw new System.ArgumentException("value should be 0 or bigger");
                _threadSleepTimeForHaveCommand = value;
            }
        }

        private bool _runningCommandForMainThread = false;
        private System.Action _command = null;

        private void Awake()
        {
            _mainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        private void OnEnable()
        {
            if (_singleton != null)
                throw new InvalidOperationException("Threre should be only one running");
            _singleton = this;
        }

        private void OnDisable()
        {
            _singleton = null;
            RunCommandIfAny();
        }

        //CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        //CancellationToken token = cancelTokenSource.Token;
        //to cancel cancelTokenSource.Cancel();
        //don't forget to cancelTokenSource.Dispose(); after done with token
        public void RunInFixedUpdateAndReturnAfterCompletion(System.Action newCommand, CancellationToken token)
        {

            if (newCommand == null)
                throw new System.ArgumentNullException("newCommand action should not be null");

            if (_mainThreadID == System.Threading.Thread.CurrentThread.ManagedThreadId)
                throw new System.ArgumentException($"{nameof(RunInFixedUpdateAndReturnAfterCompletion)} should not be called from Main(Unity) thread");

            if (_runningCommandForMainThread)
                throw new System.Exception("While waiting for command, no other commands allowed");

            if (_command != null)
                throw new System.Exception("Somehow there is already another non null command action, while trying to run newCommand");

            //var state = PythonEngine.BeginAllowThreads();

            System.Threading.Thread.MemoryBarrier();
            _command = newCommand;
            System.Threading.Thread.MemoryBarrier();
            _runningCommandForMainThread = true;
            System.Threading.Thread.MemoryBarrier();

            CheckCommandCycle(token);

            //PythonEngine.EndAllowThreads(state);


            void CheckCommandCycle(CancellationToken token)
            {
                while (_runningCommandForMainThread) // a, cyclic check
                {
                    if (token.IsCancellationRequested)
                        return;
                    if (_singleton == null)
                    {
                        System.Threading.Thread.MemoryBarrier();
                        if (_runningCommandForMainThread) //b, double check, because threads
                            throw new System.Exception("Cannot run command because there is no runner singleton");
                    }
                    if (Thread.Yield() == false)
                        Thread.Sleep(ThreadSleepTimeForHaveCommand);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_runningCommandForMainThread == false) //1, pleriminary check for speed
                return;

            RunCommandIfAny();
        }

        private void RunCommandIfAny()
        {
            if (_runningCommandForMainThread == false) //check for speed in case of call without preliminary check
                return;

            System.Threading.Thread.MemoryBarrier();

            if (_runningCommandForMainThread == false) //double check, because threads
                return;

            _command();
            _command = null;

            System.Threading.Thread.MemoryBarrier();

            _runningCommandForMainThread = false;

            System.Threading.Thread.MemoryBarrier();
        }

    }
}

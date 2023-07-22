using System;
using System.Threading;
using System.Threading.Tasks;

namespace UKnack.PythonNet
{
    public partial class PythonRunner
    {
        private class SecondThreadRunner
        {
            private Task _currentTask = null;
            internal CancellationTokenSource _currentScriptRunCancelationTokenSource = null;

            public void Run(Action action)
            {
                if (_currentTask != null)
                {
                    System.Threading.Thread.MemoryBarrier();
                    if (_currentTask != null)
                        throw new InvalidOperationException("there is already script running in second thread");
                }
                CleanupCancelationToken();

                _currentScriptRunCancelationTokenSource = new CancellationTokenSource();

                System.Threading.Thread.MemoryBarrier();

                _currentTask = Task.Run(() => {

                    action();
                    CleanupCancelationToken();
                    CleanupCurrentTask();
                });

            }

            private void CleanupCurrentTask()
            {
                if (_currentTask != null)
                {
                    _currentScriptRunCancelationTokenSource?.Cancel();
                }
                System.Threading.Thread.MemoryBarrier();
                _currentTask = null;
            }

            private void CleanupCancelationToken()
            {
                if (_currentScriptRunCancelationTokenSource == null)
                    return;
                _currentScriptRunCancelationTokenSource.Dispose();
                _currentScriptRunCancelationTokenSource = null;
            }
        }
    }
}

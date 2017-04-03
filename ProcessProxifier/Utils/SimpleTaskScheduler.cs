using System;
using System.Threading;
using ThreadTimer = System.Threading.Timer;

namespace ProcessProxifier.Utils
{
    public class SimpleTaskScheduler
    {
        private ThreadTimer _threadTimer; //keep it alive

        public Action DoWork { set; get; }

        public void Start(long startAfter = 1 * 60 * 1000, long interval = 15 * 1000)
        {
            _threadTimer = new ThreadTimer(doWork, null, Timeout.Infinite, 1000);
            _threadTimer.Change(startAfter, interval);
        }

        public void Stop()
        {
            if (_threadTimer != null)
                _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void doWork(object state)
        {
            if (DoWork != null)
                DoWork();
        }
    }
}
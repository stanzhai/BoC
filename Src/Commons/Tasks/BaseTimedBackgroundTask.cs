using System.Threading;
using BoC.EventAggregator;
using BoC.Helpers;
using Timer=System.Timers.Timer;

namespace BoC.Tasks
{
    public abstract class BaseTimedBackgroundTask : BaseBackgroundTask
    {
        protected BaseTimedBackgroundTask(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
            timer.Interval = 5000;
            timer.AutoReset = false;
        }

        private volatile bool stopping = false;
        public bool IsStopping
        {
            get { return stopping; }
            protected set
            {
                stopping = value;
            }
        }

        public double Interval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }

        public bool AutoReset
        {
            get
            {
                return timer.AutoReset;
            }
            set
            {
                timer.AutoReset = value;
            }
        }

        private System.Timers.Timer timer = new Timer();

        /// <summary>
        /// This method is started in a background thread
        /// </summary>
        public abstract void DoWork();

        protected override void OnStart()
        {
            //allows initialization stuff
        }

        protected override void OnStop()
        {
            //allows cleanup stuff
        }

        override public void Start()
        {
            OnStart();
            timer.Elapsed += (sender, args) =>
                                 {
                                     try
                                     {
                                         DoWork();
                                     }
                                     finally 
                                     {
                                         if (!AutoReset)
                                            timer.Start();
                                     }
                                 };
            timer.Start();

            IsRunning = true;
        }

        override public void Stop()
        {
            IsStopping = true;
            timer.Stop();
            OnStop();
            IsRunning = false;
        }

    }
}
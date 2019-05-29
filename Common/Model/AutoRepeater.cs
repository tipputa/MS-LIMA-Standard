using System;

namespace Metabolomics.Core
{
    public class AutoRepeater
    {
        public AutoRepeater() { }

        public AutoRepeater(int exportIntervalMillisecond)
        {
            this.ExportIntervalMillisecond = exportIntervalMillisecond;
            ExportTimer = new System.Timers.Timer(ExportIntervalMillisecond);
        }

        public int ExportIntervalMillisecond { get; set; } = 10000;
        public System.Timers.Timer ExportTimer { get; set; }

        public event EventHandler<EventArgs> OnTimeEventHandler;

        public void Start()
        {
            ExportTimer.Elapsed += OnTimedEvent;
            ExportTimer.Enabled = true;
        }

        public void Stop()
        {
            ExportTimer.Enabled = false;
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            OnTimeEventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}

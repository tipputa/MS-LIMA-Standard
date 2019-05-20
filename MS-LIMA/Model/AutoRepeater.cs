using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metabolomics.MsLima
{
    public class AutoRepeater
    {
        public AutoRepeater() { }

        public AutoRepeater(int exportIntervalMillisecond)
        {
            this.ExportIntervalMillisecond = exportIntervalMillisecond;
        }

        public int ExportIntervalMillisecond { get; set; } = 60000;
        public System.Timers.Timer ExportTimer { get; set; }

        public event EventHandler<EventArgs> OnTimeEventHandler;

        private void Start()
        {
            ExportTimer = new System.Timers.Timer(ExportIntervalMillisecond);
            ExportTimer.Elapsed += OnTimedEvent;
            ExportTimer.Enabled = true;
        }

        private void Stop()
        {
            ExportTimer.Enabled = false;
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            OnTimeEventHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}

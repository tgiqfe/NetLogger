using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetLogger.Worker
{
    internal class BackgroundWorker
    {
        public bool Running = false;
        public int Interval = 5000;

        public List<IBackgroundRepeat> RepeatTargets = new List<IBackgroundRepeat>();

        public BackgroundWorker()
        {
            this.RepeatTargets = new();

            this.Running = true;
            Run().ConfigureAwait(false);
        }

        public async Task Run()
        {
            while (this.Running)
            {
                await Task.Delay(this.Interval);
                Console.WriteLine("aaaa");

                foreach (var repeatTarget in this.RepeatTargets)
                {
                    await repeatTarget.Work();
                }
            }
        }
    }
}

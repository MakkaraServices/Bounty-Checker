using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyChecker.Reporting
{
    public class ProgressReporter<T> : IProgress<T>
    {

        public event EventHandler<T> ProgressChanged;

        public ProgressReporter()
        {
        }

        public void Report(T value)
        {
            ProgressChanged(this, value);
        }


    }
}

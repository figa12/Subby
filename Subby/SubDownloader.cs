using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subby
{
    public abstract class SubDownloader
    {
        public string Filename;

        public SubDownloader(string filename)
        {
            this.Filename = filename;
        }

        public abstract bool Run();
    }
}

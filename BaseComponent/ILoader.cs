using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseComponent
{
    public interface ILoader
    {
        (int taskCount, int progress) ProgressInfo { get; set; }
    }
}

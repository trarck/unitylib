using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YH.Timeline
{
    public  interface IClip
    {
        double start { get; set; }
        double end { get; set; }
        double duration { get; set; }
    }
}

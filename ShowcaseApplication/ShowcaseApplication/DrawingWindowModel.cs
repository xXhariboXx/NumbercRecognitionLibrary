using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace WKiRO.MainApplication
{
    public class DrawingWindowModel
    {
        public DrawingWindowModel()
        {
            Strokes = new StrokeCollection();
        }
        public bool SondeApplicationActive { get; internal set; }
        public bool DistributionSequenceActive { get; internal set; }
        public bool BarrFeaturesActive { get; internal set; }
        public StrokeCollection Strokes { get; internal set; }
    }
}

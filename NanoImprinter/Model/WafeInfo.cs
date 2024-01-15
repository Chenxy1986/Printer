using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WestLakeShape.Common;

namespace NanoImprinter.Model
{
    public class WafeInfo
    {
        public Point2D Center { get; set; }

        public Point2D Radius { get; set; }

        public PrintMask PrintMask { get; set; }

    }


    public class PrintMask
    {
        public int RowIndex { get; set; }

        public int ColIndex { get; set; }

        public int CurrentIndex { get; set; }

        public int Count { get; set; }

        public double XOffset { get; set; }

        public double YOffset { get; set; }
    }


    public struct MaskInfo
    {
        public double Length { get; private set; }

        public double Width { get; private set; }
        public MaskInfo(double length, double width)
        {
            Length = length;
            Width = width;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Build
{
    class Frame
    {
        public int SourceFrameNum { get; set; }
        public int Duration { get; set; }
        public int BuildImageIdx { get; set; }
        public float PivotX { get; set; }
        public float PivotY { get; set; }
        public float PivotWidth { get; set; }
        public float PivotHeight { get; set; }
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public int Time { get; set; }

        public Frame(int sourceFrameNum, int duration, int buildImageIdx, float pivotX, float pivotY, float pivotWidth, float pivotHeight, float x1, float y1, float x2, float y2, int time)
        {
            SourceFrameNum = sourceFrameNum;
            Duration = duration;
            BuildImageIdx = buildImageIdx;
            PivotX = pivotX;
            PivotY = pivotY;
            PivotWidth = pivotWidth;
            PivotHeight = pivotHeight;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Time = time;
        }

    }
}

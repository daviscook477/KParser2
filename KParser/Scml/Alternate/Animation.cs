using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Scml.Alternate
{
    /* this class is in-memory representation of the scml file
     * designed to be loaded, modified, and then written 
     * in order to faciliate programmatically modifying scml */
    class Animation
    {
        /* the name of the animation */
        public string Name { get; set; }

        /* interval between frames in ms - only integer amounts */
        public int Step { get; set; }

        /* the length of the animation in milliseconds - integer */
        public int Length { get; set; }

        /* 2d array of the frames, 1st axis is the ids of the timelines for all
         * of the sprites and bones in the animation, 2nd axis is the timesteps */
        public List<List<Frame>> FrameArray { get; set; }

        public Animation(string name, int step, int length, List<List<Frame>> frameArray)
        {
            Name = name;
            Step = step;
            Length = length;
            FrameArray = frameArray;
        }
    }
}

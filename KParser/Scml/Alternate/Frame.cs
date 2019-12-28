using System;
using System.Collections.Generic;
using System.Text;

namespace KParser.Scml.Alternate
{
    class Frame
    {   
        /* parent id is the id of the parent of this object 
         * all ids are either zero or a positive integer so -1
         * represents an object without a parent */
        public int ParentId { get; set; }

        /* just z-index no need to change - just note that
         * z-index only changes on the frame that it is changed
         * when Spriter interpolates so when interpolating in-between
         * frames the z-index of the prior frame should be used instead
         * 
         * also z-index is ignored when Type is ChildType.Bone but I
         * didn't think of a more elegant way of making z-index optional*/
        public int ZIndex { get; set; }

        /* the folder and file are ignored when Type is ChildType.Bone
         * same as above with z-index */
        public int Folder { get; set; }
        public int File { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Angle { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        private bool populated = false;

        public Frame(int parentId, int zIndex)
        {
            ParentId = parentId;
            ZIndex = zIndex;
        }

        public bool IsPopulated()
        {
            return populated;
        }

        public void Populate(int folder, int file, float x, float y, float angle, float scaleX, float scaleY)
        {
            Folder = folder;
            File = file;
            X = x;
            Y = y;
            Angle = angle;
            ScaleX = scaleX;
            ScaleY = scaleY;
            populated = true;
        }
    }
}

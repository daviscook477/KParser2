using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Scml.Conversion
{
    class ScmlToAlternateConverter
    {
        public File ScmlFile { get; internal set; }

        private bool converted = false;
        private Alternate.File alternateFile = null;

        public ScmlToAlternateConverter(File scmlFile)
        {
            ScmlFile = scmlFile;
        }

        public Alternate.File GetAlternateFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return alternateFile;
        }

        private void ConvertFile()
        {
            XmlElement spriterData = (XmlElement) ScmlFile.Scml.GetElementsByTagName("spriter_data")[0];
            XmlElement entity = GetFirstChildByName(spriterData, "entity");
            string name = entity.GetAttribute("name");
            List<Alternate.Animation> animations = new List<Alternate.Animation>();
            foreach (XmlNode node in entity.ChildNodes)
            {
                if (node is XmlElement && node.Name.Equals("animation"))
                {
                    animations.Add(ParseAnimation((XmlElement)node));
                }
            }
            alternateFile = new Alternate.File(name, animations);
        }

        private Alternate.Animation ParseAnimation(XmlElement animation)
        {
            string name = animation.GetAttribute("name");
            int length = int.Parse(animation.GetAttribute("length"));
            int interval = int.Parse(animation.GetAttribute("interval"));
            Alternate.SpriteSymbolIdProvider idProvider = new Alternate.SpriteSymbolIdProvider(ScmlFile, name);

            /* rename the ids to match timelines in "id" and "parent" */
            IdRename(animation, idProvider);

            /* build the frame array - description is in Alternate.Animation */
            List<List<Alternate.Frame>> frameArray = new List<List<Alternate.Frame>>();
            /* count of frames to know how large the frame array should be */
            int numberOfFrames = length / interval;
            /* for each sprite and bone id a list of frames is created */
            for (int i = 0; i < idProvider.Size(); i++)
            {
                List<Alternate.Frame> frames = new List<Alternate.Frame>();
                for (int j = 0; j < numberOfFrames; j++)
                {
                    /* start out with a completely empty frame array so it can later be populated
                     * with the existing data and then finally with the interpolated data */
                    frames.Add(null);
                }
                frameArray.Add(frames);
            }

            /* read all the data from mainline 
             * oddly it is not needed to read which timeline key frame is associated
             * with each mainline key frame since the timline key frames contain timing
             * information which can be used to exactly place them in the array since
             * we force snapping to intervals for ONI animations */
            XmlElement mainline = GetFirstChildByName(animation, "mainline");
            foreach (XmlNode keyNode in mainline.ChildNodes)
            {
                if (keyNode is XmlElement && keyNode.Name.Equals("key"))
                {
                    int time = 0;
                    XmlElement keyElement = (XmlElement)keyNode;
                    if (keyElement.HasAttribute("time"))
                    {
                        time = int.Parse(keyElement.GetAttribute("time"));
                    }
                    /* scale the time by the interval between frames to figure out which
                     * frame index in the array this goes to */
                    int frameIndex = time / interval;
                    foreach (XmlNode refNode in keyNode.ChildNodes)
                    {
                        if (refNode is XmlElement &&
                            (refNode.Name.Equals("object_ref") || refNode.Name.Equals("bone_ref")))
                        {
                            XmlElement refElement = (XmlElement)refNode;
                            /* the call to IdRename ensures that the ids on each ref now match the actual index in the array */
                            int id = int.Parse(refElement.GetAttribute("id"));
                            int zIndex = 0;
                            if (refElement.HasAttribute("z_index")) /* will always be true for sprites never true for bones */
                            {
                                zIndex = int.Parse(refElement.GetAttribute("z_index"));
                            }
                            int parent = -1;
                            if (refElement.HasAttribute("parent")) /* optional for both sprites and bones */
                            {
                                parent = int.Parse(refElement.GetAttribute("parent"));
                            }
                            /* set this frame to contain the data stored in the mainline */
                            frameArray[id][frameIndex] = new Alternate.Frame(zIndex, parent);
                        }
                    }
                }
            }

            /* read all the data from each timeline and use it to further populate the data of each frame */
            foreach (XmlNode timelineNode in animation.ChildNodes)
            {
                if (timelineNode is XmlElement && timelineNode.Name.Equals("timeline"))
                {
                    XmlElement timelineElement = (XmlElement)timelineNode;
                    int timeline = int.Parse(timelineElement.GetAttribute("id"));
                    int timelineIndex = idProvider.GetId(timeline);
                    float x = 0;
                    float y = 0;
                    float angle = 0;
                    float scaleX = 1.0f;
                    float scaleY = 1.0f;
                    foreach (XmlNode keyNode in timelineNode.ChildNodes)
                    {
                        if (keyNode is XmlElement && keyNode.Name.Equals("key"))
                        {
                            XmlElement keyElement = (XmlElement)keyNode;
                            int time = 0;
                            if (keyElement.HasAttribute("time"))
                            {
                                time = int.Parse(keyElement.GetAttribute("time"));
                            }
                            int frameIndex = time / interval;
                            XmlElement child = GetFirstChildByName(keyElement, "object");
                            if (child == null)
                            {
                                child = GetFirstChildByName(keyElement, "bone");
                            }
                            if (child == null)
                            {
                                throw new ArgumentException("Found timeline key without child object or bone");
                            }
                            int folder = -1;
                            if (child.HasAttribute("folder"))
                            {
                                folder = int.Parse(child.GetAttribute("folder"));
                            }
                            int file = -1;
                            if (child.HasAttribute("file"))
                            {
                                file = int.Parse(child.GetAttribute("file"));
                            }
                            if (child.HasAttribute("x"))
                            {
                                x = float.Parse(child.GetAttribute("x"));
                            }
                            if (child.HasAttribute("y"))
                            {
                                y = float.Parse(child.GetAttribute("y"));
                            }
                            if (child.HasAttribute("angle"))
                            {
                                angle = float.Parse(child.GetAttribute("angle"));
                            }
                            if (child.HasAttribute("scale_x"))
                            {
                                scaleX = float.Parse(child.GetAttribute("scale_x"));
                            }
                            if (child.HasAttribute("scale_y"))
                            {
                                scaleY = float.Parse(child.GetAttribute("scale_y"));
                            }
                            frameArray[timelineIndex][frameIndex].Populate(folder, file, x, y, angle, scaleX, scaleY);
                        }
                    }
                }
            }

            /* determine which frames need to be interpolated by checking which frames are key frames in the mainline */
            List<bool> keyFrames = new List<bool>();
            for (int i = 0; i < numberOfFrames; i++)
            {
                keyFrames.Add(false);
            }
            foreach (XmlNode keyNode in mainline.ChildNodes)
            {
                if (keyNode is XmlElement && keyNode.Name.Equals("key"))
                {
                    int time = 0;
                    XmlElement keyElement = (XmlElement)keyNode;
                    if (keyElement.HasAttribute("time"))
                    {
                        time = int.Parse(keyElement.GetAttribute("time"));
                    }
                    /* scale the time by the interval between frames to figure out which
                     * frame index in the array this goes to */
                    int frameIndex = time / interval;
                    /* now we know this particular time step is a key frame in the mainline */
                    keyFrames[frameIndex] = true;
                }
            }

            /* create an additional array that indicates presence of each timeline on a per-frame basis */
            List<List<bool>> presenceArray = new List<List<bool>>();
            for (int i = 0; i < idProvider.Size(); i++)
            {
                List<bool> presences = new List<bool>();
                for (int j = 0; j < numberOfFrames; j++)
                {
                    /* start out with a completely empty presence array so it can later be populated
                     * with the existing data and then finally with the interpolated data */
                    presences.Add(false);
                }
                presenceArray.Add(presences);
            }
            for (int i = 0; i < idProvider.Size(); i++)
            {
                bool currentPresence = false;
                for (int j = 0; j < numberOfFrames; j++)
                {
                    /* if this frame is a key frame then update the current presence based on if there
                     * is a frame populated at this location */
                    if (keyFrames[j])
                    {
                        currentPresence = (frameArray[i][j] != null);
                    }
                    presenceArray[i][j] = currentPresence;
                }
                for (int j = 0; j < numberOfFrames; j++)
                {
                    /* if this frame is a key frame then update the current presence based on if there
                     * is a frame populated at this location */
                    if (keyFrames[j])
                    {
                        currentPresence = (frameArray[i][j] != null);
                    }
                    presenceArray[i][j] = currentPresence;
                }
                /* executing the loop twice is the most straightforward way to ensure that a keyframe at the end
                 * of the timeline wraps around to the front of the timeline
                 * this does mess with animations that aren't looped that don't have keyframes at time = 0 but that just doesn't make
                 * much sense (who wouldn't keyframe at time = 0 for a non-looping animation!)
                 * so I'll just document that and ignore that problem for now */
            }

            /* for every frame with presence in the array set to true that still has a null frame
             * interpolate the missing frame */
            for (int i = 0; i < idProvider.Size(); i++)
            {
                Alternate.Frame beforeFrame = null;
                Alternate.Frame afterFrame = null;
                int beforeFrameIndex = -1;
                int afterFrameIndex = -1;
                for (int j = 0; j < numberOfFrames; j++)
                {
                    /* skip this frame if it isn't supposed to be present */
                    if (!presenceArray[i][j])
                    {
                        continue;
                    }
                    /* if this frame exists and is populated then it will be used
                     * as the before frame */
                    if (frameArray[i][j] != null && frameArray[i][j].IsPopulated())
                    {
                        beforeFrame = frameArray[i][j];
                        beforeFrameIndex = j;
                        /* probe forward to find the after array when a before array is found
                         * will use a endless loop because eventually at least we know we will
                         * terminate when it hits the exact same before array */
                        int jPrime = j + 1;
                        while (true)
                        {
                            if (jPrime >= numberOfFrames)
                            {
                                jPrime = 0;
                            }
                            if (frameArray[i][jPrime] != null && frameArray[i][jPrime].IsPopulated())
                            {
                                afterFrame = frameArray[i][jPrime];
                                afterFrameIndex = jPrime;
                                break;
                            }
                            jPrime++;
                        }
                    }
                    else if (beforeFrame != null && afterFrame != null)
                    {
                        float x = LinearInterpolate(beforeFrame.X, afterFrame.X, beforeFrameIndex,
                            afterFrameIndex + ((afterFrameIndex < beforeFrameIndex) ? numberOfFrames : 0), j);
                        float y = LinearInterpolate(beforeFrame.Y, afterFrame.Y, beforeFrameIndex,
                            afterFrameIndex + ((afterFrameIndex < beforeFrameIndex) ? numberOfFrames : 0), j);
                        float angle = LinearInterpolateAngle(beforeFrame.Angle, afterFrame.Angle, beforeFrameIndex,
                            afterFrameIndex + ((afterFrameIndex < beforeFrameIndex) ? numberOfFrames : 0), j);
                        float xScale = LinearInterpolate(beforeFrame.ScaleX, afterFrame.ScaleX, beforeFrameIndex,
                            afterFrameIndex + ((afterFrameIndex < beforeFrameIndex) ? numberOfFrames : 0), j);
                        float yScale = LinearInterpolate(beforeFrame.ScaleY, afterFrame.ScaleY, beforeFrameIndex,
                            afterFrameIndex + ((afterFrameIndex < beforeFrameIndex) ? numberOfFrames : 0), j);
                        frameArray[i][j] = new Alternate.Frame(beforeFrame.ParentId, beforeFrame.ZIndex);
                        frameArray[i][j].Populate(beforeFrame.Folder, beforeFrame.File, x, y, angle, xScale, yScale);
                    }
                }
            }

            return new Alternate.Animation(name, interval, length, frameArray);
        }

        private float LinearInterpolate(float x0, float x1, float t0, float t1, float t)
        {
            if (t0 == t1)
            {
                return x0;
            }
            float a = (x0 - x1) / (t0 - t1);
            float b = x0 - a * t0;
            return a * t + b;
        }

        /* requires that both input angles x0 x1 are within 0 to 2pi and returns an interpolated
         * angle also between 0 and 2pi
         * interpolates the shortest route rather than explicitly clockwise or counterclockwise */
        private float LinearInterpolateAngle(float x0, float x1, float t0, float t1, float t)
        {
            if (t0 == t1)
            {
                return x0;
            }
            /* see https://stackoverflow.com/questions/2708476/rotation-interpolation for math
             * explanation of why this works */
            float delta = Math.Abs(x1 - x0);
            if (delta > Math.PI)
            {
                if (x1 > x0)
                {
                    x0 += (float) (2 * Math.PI);
                }
                else
                {
                    x1 += (float) (2 * Math.PI);
                }
            }

            float x = LinearInterpolate(x0, x1, t0, t1, t);
            if (x >= 2 * Math.PI)
            {
                x -= (float)(2 * Math.PI);
            }
            return x;
        }

        /* makes it such that each sprite and bones id in the scml matches those given by the id provider
         * while also ensuring that the parent ids are also kept accurate
         * this allows for consistent ids that are 0-indexed and without any missing integers */
            private void IdRename(XmlElement animation, Alternate.SpriteSymbolIdProvider idProvider)
        {
            /* when renaming parent, the id always refers to the id for the bone refs */
            XmlElement mainline = GetFirstChildByName(animation, "mainline");
            foreach (XmlNode keyNode in mainline.ChildNodes)
            {
                if (keyNode is XmlElement && keyNode.Name.Equals("key"))
                {
                    Dictionary<int, int> symbolIdToTimelineMap = new Dictionary<int, int>();
                    foreach (XmlNode refNode in keyNode.ChildNodes)
                    {
                        if (refNode is XmlElement && refNode.Name.Equals("bone_ref"))
                        {
                            int id = int.Parse(((XmlElement)refNode).GetAttribute("id"));
                            int timeline = int.Parse(((XmlElement)refNode).GetAttribute("timeline"));
                            symbolIdToTimelineMap.Add(id, timeline);
                        }
                    }
                    foreach (XmlNode refNode in keyNode.ChildNodes)
                    {
                        if (refNode is XmlElement)
                        {
                            XmlElement refElement = (XmlElement)refNode;
                            int timeline = int.Parse(refElement.GetAttribute("timeline"));
                            refElement.SetAttribute("id", idProvider.GetId(timeline).ToString());
                            if (refElement.HasAttribute("parent"))
                            {
                                int parent = int.Parse(refElement.GetAttribute("parent"));
                                refElement.SetAttribute("parent", idProvider.GetId(symbolIdToTimelineMap[parent]).ToString());
                            }
                        }
                    }
                }
            }
        }

        private XmlElement GetFirstChildByName(XmlElement parent, string tagName)
        {
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement && node.Name.Equals(tagName))
                {
                    return (XmlElement)node;
                }
            }
            return null;
        }
    }
}

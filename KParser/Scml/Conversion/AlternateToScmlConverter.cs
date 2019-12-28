using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KParser.Scml.Conversion
{
    class AlternateToScmlConverter
    {
        public File ScmlFile { get; internal set; }
        public Alternate.File AlternateFile { get; internal set; }

        private bool converted = false;
        private File convertedScmlFile = null;

        public AlternateToScmlConverter(File scmlFile, Alternate.File alternateFile)
        {
            ScmlFile = scmlFile;
            AlternateFile = alternateFile;
        }

        public File GetConvertedScmlFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return convertedScmlFile;
        }

        private void ConvertFile()
        {
            convertedScmlFile = new File((XmlDocument)ScmlFile.Scml.Clone());
            XmlElement spriterData = (XmlElement)convertedScmlFile.Scml.GetElementsByTagName("spriter_data")[0];
            XmlElement entity = GetFirstChildByName(spriterData, "entity");
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in entity.ChildNodes)
            {
                nodes.Add(node);
            }
            foreach (XmlNode node in nodes)
            {
                if (node is XmlElement && node.Name.Equals("animation"))
                {
                    entity.RemoveChild(node);
                }
            }
            for (int i = 0; i < AlternateFile.Animations.Count; i++)
            {
                entity.AppendChild(MakeAnimationNode(i, AlternateFile.Animations[i]));
            }
        }

        private XmlElement MakeAnimationNode(int id, Alternate.Animation animation)
        {
            XmlDocument scml = convertedScmlFile.Scml;
            XmlElement animationElement = scml.CreateElement("animation");
            animationElement.SetAttribute("id", id.ToString());
            animationElement.SetAttribute("name", animation.Name);
            animationElement.SetAttribute("length", animation.Length.ToString());
            animationElement.SetAttribute("interval", animation.Step.ToString());
            XmlElement mainlineElement = scml.CreateElement("mainline");
            for (int j = 0; j < animation.FrameArray[0].Count; j++)
            {
                XmlElement keyElement = scml.CreateElement("key");
                keyElement.SetAttribute("id", j.ToString());
                keyElement.SetAttribute("time", (j * animation.Step).ToString());
                for (int i = 0; i < animation.FrameArray.Count; i++)
                {
                    if (animation.FrameArray[i][j] != null)
                    {
                        Alternate.Frame frame = animation.FrameArray[i][j];
                        KParser.Conversion.ChildType type = animation.IdProvider.GetType(i);
                        if (type == KParser.Conversion.ChildType.Bone)
                        {
                            XmlElement boneElement = scml.CreateElement("bone_ref");
                            boneElement.SetAttribute("id", i.ToString());
                            boneElement.SetAttribute("timeline", i.ToString());
                            boneElement.SetAttribute("key", j.ToString());
                            if (frame.ParentId != -1)
                            {
                                boneElement.SetAttribute("parent", frame.ParentId.ToString());
                            }
                            keyElement.AppendChild(boneElement);
                        }
                        else if (type == KParser.Conversion.ChildType.Sprite)
                        {
                            XmlElement objectElement = scml.CreateElement("object_ref");
                            objectElement.SetAttribute("id", i.ToString());
                            objectElement.SetAttribute("timeline", i.ToString());
                            objectElement.SetAttribute("key", j.ToString());
                            if (frame.ParentId != -1)
                            {
                                objectElement.SetAttribute("parent", frame.ParentId.ToString());
                            }
                            objectElement.SetAttribute("z_index", frame.ZIndex.ToString());
                            keyElement.AppendChild(objectElement);
                        }
                    }
                }
                mainlineElement.AppendChild(keyElement);
            }
            animationElement.AppendChild(mainlineElement);
            for (int i = 0; i < animation.FrameArray.Count; i++)
            {
                XmlElement timelineElement = scml.CreateElement("timeline");
                timelineElement.SetAttribute("id", i.ToString());
                timelineElement.SetAttribute("name", animation.IdProvider.GetName(i));
                for (int j = 0; j < animation.FrameArray[0].Count; j++)
                {
                    if (animation.FrameArray[i][j] != null)
                    {
                        XmlElement keyElement = scml.CreateElement("key");
                        keyElement.SetAttribute("id", j.ToString());
                        keyElement.SetAttribute("time", (j * animation.Step).ToString());
                        Alternate.Frame frame = animation.FrameArray[i][j];
                        KParser.Conversion.ChildType type = animation.IdProvider.GetType(i);
                        if (type == KParser.Conversion.ChildType.Bone)
                        {
                            XmlElement boneElement = scml.CreateElement("bone");
                            boneElement.SetAttribute("x", frame.X.ToString());
                            boneElement.SetAttribute("y", frame.Y.ToString());
                            boneElement.SetAttribute("angle", frame.Angle.ToString());
                            boneElement.SetAttribute("scale_x", frame.ScaleX.ToString());
                            boneElement.SetAttribute("scale_y", frame.ScaleY.ToString());
                            keyElement.AppendChild(boneElement);
                        }
                        else if (type == KParser.Conversion.ChildType.Sprite)
                        {
                            XmlElement objectElement = scml.CreateElement("object");
                            objectElement.SetAttribute("folder", frame.Folder.ToString());
                            objectElement.SetAttribute("file", frame.File.ToString());
                            objectElement.SetAttribute("x", frame.X.ToString());
                            objectElement.SetAttribute("y", frame.Y.ToString());
                            objectElement.SetAttribute("angle", frame.Angle.ToString());
                            objectElement.SetAttribute("scale_x", frame.ScaleX.ToString());
                            objectElement.SetAttribute("scale_y", frame.ScaleY.ToString());
                            keyElement.AppendChild(objectElement);
                        }
                        timelineElement.AppendChild(keyElement);
                    }
                }
                animationElement.AppendChild(timelineElement);
            }
            return animationElement;
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

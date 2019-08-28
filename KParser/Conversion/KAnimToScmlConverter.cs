﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace KParser.Conversion
{
    class KAnimToScmlConverter
    {
        public const float MsPerS = 1000.0f;

        public Atlas.File AtlasFile { get; internal set; }
        public Build.File BuildFile { get; internal set; }
        public Animation.File AnimationFile { get; internal set; }

        private bool converted = false;
        private Textures.File texturesFile = null;
        private Scml.File scmlFile = null;

        public KAnimToScmlConverter(Atlas.File atlasFile, Build.File buildFile, Animation.File animationFile)
        {
            AtlasFile = atlasFile;
            BuildFile = buildFile;
            AnimationFile = animationFile;
        }

        public Textures.File GetTexturesFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return texturesFile;
        }

        public Scml.File GetScmlFile()
        {
            if (!converted)
            {
                ConvertFile();
                converted = true;
            }
            return scmlFile;
        }

        private void ConvertFile()
        {
            AtlasToTexturesConverter atlasToTexturesConverter = new AtlasToTexturesConverter(AtlasFile, BuildFile);
            texturesFile = atlasToTexturesConverter.GetTexturesFile();

            XmlDocument scml = new XmlDocument();
            XmlElement root = MakeRootNode(scml);
            XmlDeclaration xmlDeclaration = scml.CreateXmlDeclaration("1.0", "UTF-8", null);
            scml.AppendChild(root);
            scml.InsertBefore(xmlDeclaration, root);
            scmlFile = new Scml.File(scml);
        }

        private XmlElement MakeRootNode(XmlDocument scml)
        {
            XmlElement root = scml.CreateElement(string.Empty, "spriter_data", string.Empty);
            root.SetAttribute("scml_version", "1.0");
            root.SetAttribute("generator", "BrashMonkey Spriter");
            root.SetAttribute("generator_version", "r11");
            FileIdProvider fileIdProvider = new FileIdProvider(BuildFile);
            root.AppendChild(MakeFolderNode(scml, fileIdProvider));
            root.AppendChild(MakeEntityNode(scml, fileIdProvider));
            return root;
        }

        private XmlElement MakeFolderNode(XmlDocument scml, FileIdProvider fileIdProvider)
        {
            XmlElement folder = scml.CreateElement(string.Empty, "folder", string.Empty);
            folder.SetAttribute("id", "0");
            HashSet<string>  names = new HashSet<string>();
            foreach (Build.Symbol symbol in BuildFile.Build.SymbolsList)
            {
                foreach (Build.Frame frame in symbol.FramesList)
                {
                    string name = BuildFile.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
                    if (names.Contains(name))
                    {
                        continue;
                    }
                    names.Add(name);
                    folder.AppendChild(MakeFileNode(scml, symbol, frame, BuildFile, name, fileIdProvider));
                }
            }
            return folder;
        }

        private XmlElement MakeFileNode(XmlDocument scml, Build.Symbol symbol, Build.Frame frame, Build.File BuildFile, string name, FileIdProvider fileIdProvider)
        {
            int imageWidth = AtlasFile.Atlas.Width;
            int imageHeight = AtlasFile.Atlas.Height;
            float x = frame.PivotX - frame.PivotWidth / 2f;
            float y = frame.PivotY - frame.PivotHeight / 2f;
            float pivotX = 0 - x / frame.PivotWidth;
            float pivotY = 1 + y / frame.PivotHeight;
            int width = (int)((frame.X2 - frame.X1) * imageWidth);
            int height = (int)((frame.Y2 - frame.Y1) * imageHeight);

            XmlElement file = scml.CreateElement(string.Empty, "file", string.Empty);
            file.SetAttribute("id", fileIdProvider.NameToFileId[name].ToString());
            file.SetAttribute("name", name);
            file.SetAttribute("width", width.ToString());
            file.SetAttribute("height", height.ToString());
            file.SetAttribute("pivot_x", pivotX.ToString());
            file.SetAttribute("pivot_y", pivotY.ToString());
            return file;
        }

        private XmlElement MakeEntityNode(XmlDocument scml, FileIdProvider fileIdProvider)
        {
            XmlElement entity = scml.CreateElement(string.Empty, "entity", string.Empty);
            entity.SetAttribute("id", "0");
            entity.SetAttribute("name", BuildFile.Build.Name);
            int animationId = 0;
            HashSet<string> boneNames = new HashSet<string>();
            foreach (Animation.Bank bank in AnimationFile.Animation.BanksList)
            {
                entity.AppendChild(MakeAnimationNode(scml, bank, animationId++, fileIdProvider, boneNames));
            }
            foreach(string boneName in boneNames)
            {
                entity.AppendChild(MakeObjectInfoNode(scml, boneName));
            }
            return entity;
        }

        private XmlElement MakeAnimationNode(XmlDocument scml, Animation.Bank bank, int animationId, FileIdProvider fileIdProvider, HashSet<string> boneNames)
        {
            XmlElement animation = scml.CreateElement(string.Empty, "animation", string.Empty);
            animation.SetAttribute("id", animationId.ToString());
            animation.SetAttribute("name", bank.Name);
            int rate = (int)(MsPerS / bank.Rate);
            animation.SetAttribute("length", (rate * bank.Frames).ToString());
            animation.SetAttribute("interval", rate.ToString());
            animation.AppendChild(MakeMainlineNode(scml, bank, boneNames));
            List<XmlElement> timelines = MakeTimelineNodes(scml, bank, fileIdProvider);
            foreach (XmlElement timeline in timelines)
            {
                animation.AppendChild(timeline);
            }
            return animation;
        }

        private XmlElement MakeMainlineNode(XmlDocument scml, Animation.Bank bank, HashSet<string> boneNames)
        {
            XmlElement mainline = scml.CreateElement(string.Empty, "mainline", string.Empty);
            int rate = (int)(MsPerS / bank.Rate);
            int frameId = 0;
            SpriteIdProvider spriteIdProvider = new SpriteIdProvider(bank, AnimationFile.HashToName);
            SymbolIdProvider symbolIdProvider = new SymbolIdProvider(bank, spriteIdProvider.IdMap.Count, AnimationFile.HashToName);
            foreach (string name in symbolIdProvider.IdMap.Keys)
            {
                boneNames.Add(name);
            }

            foreach (Animation.Frame frame in bank.FramesList)
            {
                mainline.AppendChild(MakeMainlineKeyNode(scml, frame, frameId++, rate, spriteIdProvider, symbolIdProvider));
            }
            return mainline;
        }

        private XmlElement MakeMainlineKeyNode(XmlDocument scml, Animation.Frame frame, int frameId, int rate, SpriteIdProvider spriteIdProvider, SymbolIdProvider symbolIdProvider)
        {
            XmlElement key = scml.CreateElement(string.Empty, "key", string.Empty);
            key.SetAttribute("id", frameId.ToString());
            key.SetAttribute("time", (frameId * rate).ToString());
            int elementId = 0;
            foreach(Animation.Element element in frame.ElementsList)
            {
                key.AppendChild(MakeBoneRefNode(scml, frame, element, frameId, symbolIdProvider));
                key.AppendChild(MakeObjectRefNode(scml, frame, element, frameId, elementId++, spriteIdProvider, symbolIdProvider));
            }
            return key;
        }

        private XmlElement MakeBoneRefNode(XmlDocument scml, Animation.Frame frame, Animation.Element element, int frameId, SymbolIdProvider symbolIdProvider)
        {
            XmlElement boneRef = scml.CreateElement(string.Empty, "bone_ref", string.Empty);
            int id = symbolIdProvider.GetId(frame, element);
            boneRef.SetAttribute("id", id.ToString());
            boneRef.SetAttribute("timeline", id.ToString());
            boneRef.SetAttribute("key", frameId.ToString());
            return boneRef;
        }

        private XmlElement MakeObjectRefNode(XmlDocument scml, Animation.Frame frame, Animation.Element element, int frameId, int elementId, SpriteIdProvider spriteIdProvider, SymbolIdProvider symbolIdProvider)
        {
            XmlElement objectRef = scml.CreateElement(string.Empty, "object_ref", string.Empty);
            string id = spriteIdProvider.GetId(frame, element).ToString();
            objectRef.SetAttribute("id", id);
            objectRef.SetAttribute("parent", symbolIdProvider.GetId(frame, element).ToString());
            objectRef.SetAttribute("timeline", id);
            objectRef.SetAttribute("key", frameId.ToString());
            objectRef.SetAttribute("z_index", (frame.Elements - elementId).ToString());
            return objectRef;
        }

        private List<XmlElement> MakeTimelineNodes(XmlDocument scml, Animation.Bank bank, FileIdProvider fileIdProvider)
        {
            int rate = (int)(MsPerS / bank.Rate);
            SpriteIdProvider spriteIdProvider = new SpriteIdProvider(bank, AnimationFile.HashToName);
            Dictionary<int, XmlElement> idToTimeline = new Dictionary<int, XmlElement>();
            foreach (string name in spriteIdProvider.IdMap.Keys)
            {
                XmlElement timeline = scml.CreateElement(string.Empty, "timeline", string.Empty);
                int id = spriteIdProvider.IdMap[name];
                timeline.SetAttribute("id", id.ToString());
                timeline.SetAttribute("name", name);
                idToTimeline.Add(id, timeline);
            }

            SymbolIdProvider symbolIdProvider = new SymbolIdProvider(bank, spriteIdProvider.IdMap.Count, AnimationFile.HashToName);
            foreach (string name in symbolIdProvider.IdMap.Keys)
            {
                XmlElement timeline = scml.CreateElement(string.Empty, "timeline", string.Empty);
                int id = symbolIdProvider.IdMap[name];
                timeline.SetAttribute("id", id.ToString());
                timeline.SetAttribute("name", name);
                timeline.SetAttribute("object_type", "bone");
                idToTimeline.Add(id, timeline);
            }

            int frameId = 0;
            foreach (Animation.Frame frame in bank.FramesList)
            {
                foreach (Animation.Element element in frame.ElementsList)
                {
                    idToTimeline[spriteIdProvider.GetId(frame, element)].AppendChild(MakeTimelineKeyNode(scml, element, frameId, rate, fileIdProvider, ChildType.Sprite));
                    idToTimeline[symbolIdProvider.GetId(frame, element)].AppendChild(MakeTimelineKeyNode(scml, element, frameId, rate, fileIdProvider, ChildType.Bone));
                }
                frameId++;
            }

            List<XmlElement> timelines = new List<XmlElement>();
            foreach(XmlElement timeline in idToTimeline.Values)
            {
                timelines.Add(timeline);
            }
            return timelines;
        }

        private XmlElement MakeTimelineKeyNode(XmlDocument scml, Animation.Element element, int frameId, int rate, FileIdProvider fileIdProvider, ChildType childType)
        {
            XmlElement key = scml.CreateElement(string.Empty, "key", string.Empty);
            key.SetAttribute("id", frameId.ToString());
            key.SetAttribute("time", (frameId * rate).ToString());
            switch (childType)
            {
                case ChildType.Sprite:
                    key.AppendChild(MakeObjectNode(scml, element, fileIdProvider));
                    break;
                case ChildType.Bone:
                    key.AppendChild(MakeBoneNode(scml, element));
                    break;
            }
            
            return key;
        }

        private XmlElement MakeObjectNode(XmlDocument scml, Animation.Element element, FileIdProvider fileIdProvider)
        {
            XmlElement obj = scml.CreateElement(string.Empty, "object", string.Empty);
            obj.SetAttribute("folder", "0");
            obj.SetAttribute("file", GetThisOrPrecedingFile(element, fileIdProvider));
            return obj;
        }

        private XmlElement MakeBoneNode(XmlDocument scml, Animation.Element element)
        {
            XmlElement obj = scml.CreateElement(string.Empty, "bone", string.Empty);
            obj.SetAttribute("x", (element.M5 * 0.5f).ToString());
            obj.SetAttribute("y", (element.M6 * -0.5f).ToString());
            float scaleX = (float)Math.Sqrt(element.M1 * element.M1 + element.M2 * element.M2);
            float scaleY = (float)Math.Sqrt(element.M3 * element.M3 + element.M4 * element.M4);
            float det = element.M1 * element.M4 - element.M3 * element.M2;
            if (det < 0)
            {
                scaleY = -scaleY;
            }
            float sinApprox = 0.5f * (element.M3 / scaleY - element.M2 / scaleX);
            float cosApprox = 0.5f * (element.M1 / scaleX + element.M4 / scaleY);
            float angle = (float)Math.Atan2(sinApprox, cosApprox);
            if (angle < 0)
            {
                angle += (float) (2 * Math.PI);
            }
            angle *= (float) (180.0f / Math.PI);
            obj.SetAttribute("angle", angle.ToString());
            obj.SetAttribute("scale_x", scaleX.ToString());
            obj.SetAttribute("scale_y", scaleY.ToString());
            return obj;
        }

        private XmlElement MakeObjectInfoNode(XmlDocument scml, string name)
        {
            XmlElement objectInfo = scml.CreateElement(string.Empty, "obj_info", string.Empty);
            objectInfo.SetAttribute("name", name);
            objectInfo.SetAttribute("type", "bone");
            objectInfo.SetAttribute("w", "50");
            objectInfo.SetAttribute("h", "10");
            return objectInfo;
        }

        private string GetThisOrPrecedingFile(Animation.Element element, FileIdProvider fileIdProvider)
        {
            int index = element.Index;
            while (index >= 0)
            {
                string name = $"{AnimationFile.HashToName[element.Image]}_{index}";
                try
                {
                    return fileIdProvider.NameToFileId[name].ToString();
                }
                catch
                {
                    index--;
                }
            }
            // If a file doesn't exist for the sprite then assume that this is intentional and return no corresponding file
            // Note that this is done because there are some sprites defined in Klei's animation files that do not have
            // any corresponding actual texture/file to go with.
            return "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KParser.Animation;
using KParser.File;
using Frame = KParser.Build.Frame;

namespace KParser.Conversion
{
    internal enum ChildType
    {
        Sprite,
        Bone
    }

    internal class KAnimToScmlConverter
    {
        public const float MsPerS = 1000.0f;
        public AnimFile AnimationFile;

        public AtlasFile AtlasFile;
        public BuildFile BuildFile;
        public string OutDir;
        public string OutPath;
        private bool scmlConverted;
        private ScmlFile scmlFile;

        private bool texturesConverted;
        private TextureFile texturesFile;

        public KAnimToScmlConverter(AtlasFile atlasFile, BuildFile buildFile, AnimFile animationFile, string outDir,
            string outFile)
        {
            AtlasFile = atlasFile;
            BuildFile = buildFile;
            AnimationFile = animationFile;
            OutDir = outDir;
            OutPath = Path.Join(outDir, outFile);
        }

        public TextureFile GetTexturesFile()
        {
            if (texturesConverted) return texturesFile;

            var atlasToTexturesConverter = new AtlasToTexturesConverter(AtlasFile, BuildFile, OutDir);
            texturesFile = atlasToTexturesConverter.GetTexturesFile();
            texturesConverted = true;
            return texturesFile;
        }

        public ScmlFile GetScmlFile()
        {
            if (scmlConverted) return scmlFile;

            var scmlData = new XmlDocument();
            var root = MakeRootNode(scmlData);
            var xmlDeclaration = scmlData.CreateXmlDeclaration("1.0", "UTF-8", null);
            scmlData.AppendChild(root);
            scmlData.InsertBefore(xmlDeclaration, root);
            scmlFile = new ScmlFile(OutPath, scmlData);
            scmlConverted = true;
            return scmlFile;
        }


        private XmlElement MakeRootNode(XmlDocument scml)
        {
            var root = scml.CreateElement(string.Empty, "spriter_data", string.Empty);
            root.SetAttribute("scml_version", "1.0");
            root.SetAttribute("generator", "BrashMonkey Spriter");
            root.SetAttribute("generator_version", "r11");
            var fileIdProvider = new FileIdProvider(BuildFile);
            root.AppendChild(MakeFolderNode(scml, fileIdProvider));
            root.AppendChild(MakeEntityNode(scml, fileIdProvider));
            return root;
        }

        private XmlElement MakeFolderNode(XmlDocument scml, FileIdProvider fileIdProvider)
        {
            var folder = scml.CreateElement(string.Empty, "folder", string.Empty);
            folder.SetAttribute("id", "0");
            var names = new HashSet<string>();
            foreach (var symbol in BuildFile.BuildData.Build.SymbolsList)
            foreach (var frame in symbol.FramesList)
            {
                var name = BuildFile.BuildData.HashToName[symbol.Hash] + '_' + frame.SourceFrameNum;
                if (names.Contains(name)) continue;
                names.Add(name);
                folder.AppendChild(MakeFileNode(scml, frame, name, fileIdProvider));
            }

            return folder;
        }

        private XmlElement MakeFileNode(XmlDocument scml, Frame frame, string name, FileIdProvider fileIdProvider)
        {
            var imageWidth = AtlasFile.Atlas.Width;
            var imageHeight = AtlasFile.Atlas.Height;
            var x = frame.PivotX - frame.PivotWidth / 2f;
            var y = frame.PivotY - frame.PivotHeight / 2f;
            var pivotX = 0 - x / frame.PivotWidth;
            var pivotY = 1 + y / frame.PivotHeight;
            var width = (int) ((frame.X2 - frame.X1) * imageWidth);
            var height = (int) ((frame.Y2 - frame.Y1) * imageHeight);

            var file = scml.CreateElement(string.Empty, "file", string.Empty);
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
            var entity = scml.CreateElement(string.Empty, "entity", string.Empty);
            entity.SetAttribute("id", "0");
            entity.SetAttribute("name", BuildFile.BuildData.Build.Name);
            var animationId = 0;
            var boneNames = new HashSet<string>();
            foreach (var bank in AnimationFile.AnimData.Animation.BanksList)
                entity.AppendChild(MakeAnimationNode(scml, bank, animationId++, fileIdProvider, boneNames));
            foreach (var boneName in boneNames) entity.AppendChild(MakeObjectInfoNode(scml, boneName));
            return entity;
        }

        private XmlElement MakeAnimationNode(XmlDocument scml, Bank bank, int animationId,
            FileIdProvider fileIdProvider, HashSet<string> boneNames)
        {
            var animation = scml.CreateElement(string.Empty, "animation", string.Empty);
            animation.SetAttribute("id", animationId.ToString());
            animation.SetAttribute("name", bank.Name);
            var rate = (int) (MsPerS / bank.Rate);
            animation.SetAttribute("length", (rate * bank.FrameCount).ToString());
            animation.SetAttribute("interval", rate.ToString());
            animation.AppendChild(MakeMainlineNode(scml, bank, boneNames));
            var timelines = MakeTimelineNodes(scml, bank, fileIdProvider);
            foreach (var timeline in timelines) animation.AppendChild(timeline);
            return animation;
        }

        private XmlElement MakeMainlineNode(XmlDocument scml, Bank bank, ISet<string> boneNames)
        {
            var mainline = scml.CreateElement(string.Empty, "mainline", string.Empty);
            var rate = (int) (MsPerS / bank.Rate);
            var frameId = 0;
            var spriteIdProvider = new SpriteIdProvider(bank, AnimationFile.AnimData.HashToName);
            var symbolIdProvider = new SymbolIdProvider(bank, AnimationFile.AnimData.HashToName);
            foreach (var name in symbolIdProvider.IdMap.Keys) boneNames.Add(name);

            foreach (var frame in bank.FramesList)
                mainline.AppendChild(MakeMainlineKeyNode(scml, frame, frameId++, rate, spriteIdProvider,
                    symbolIdProvider));
            return mainline;
        }

        private XmlElement MakeMainlineKeyNode(XmlDocument scml, Animation.Frame frame, int frameId, int rate,
            SpriteIdProvider spriteIdProvider, SymbolIdProvider symbolIdProvider)
        {
            var key = scml.CreateElement(string.Empty, "key", string.Empty);
            key.SetAttribute("id", frameId.ToString());
            key.SetAttribute("time", (frameId * rate).ToString());
            var elementId = 0;
            foreach (var element in frame.ElementsList)
            {
                key.AppendChild(MakeBoneRefNode(scml, frame, element, frameId, spriteIdProvider, symbolIdProvider));
                key.AppendChild(MakeObjectRefNode(scml, frame, element, frameId, elementId++, spriteIdProvider,
                    symbolIdProvider));
            }

            return key;
        }

        private XmlElement MakeBoneRefNode(XmlDocument scml, Animation.Frame frame, Element element, int frameId,
            SpriteIdProvider spriteIdProvider, SymbolIdProvider symbolIdProvider)
        {
            var boneRef = scml.CreateElement(string.Empty, "bone_ref", string.Empty);
            var id = symbolIdProvider.GetId(frame, element);
            boneRef.SetAttribute("id", id.ToString());
            boneRef.SetAttribute("timeline", (id + spriteIdProvider.IdMap.Count).ToString());
            boneRef.SetAttribute("key", frameId.ToString());
            return boneRef;
        }

        private XmlElement MakeObjectRefNode(XmlDocument scml, Animation.Frame frame, Element element, int frameId,
            int elementId, SpriteIdProvider spriteIdProvider, SymbolIdProvider symbolIdProvider)
        {
            var objectRef = scml.CreateElement(string.Empty, "object_ref", string.Empty);
            var id = spriteIdProvider.GetId(frame, element).ToString();
            objectRef.SetAttribute("id", id);
            objectRef.SetAttribute("parent", symbolIdProvider.GetId(frame, element).ToString());
            objectRef.SetAttribute("timeline", id);
            objectRef.SetAttribute("key", frameId.ToString());
            objectRef.SetAttribute("z_index", (frame.ElementCount - elementId).ToString());
            return objectRef;
        }

        private List<XmlElement> MakeTimelineNodes(XmlDocument scml, Bank bank, FileIdProvider fileIdProvider)
        {
            var rate = (int) (MsPerS / bank.Rate);
            var spriteIdProvider = new SpriteIdProvider(bank, AnimationFile.AnimData.HashToName);
            var idToTimeline = new Dictionary<int, XmlElement>();
            foreach (var name in spriteIdProvider.IdMap.Keys)
            {
                var timeline = scml.CreateElement(string.Empty, "timeline", string.Empty);
                var id = spriteIdProvider.IdMap[name];
                timeline.SetAttribute("id", id.ToString());
                timeline.SetAttribute("name", name);
                idToTimeline.Add(id, timeline);
            }

            var symbolIdProvider = new SymbolIdProvider(bank, AnimationFile.AnimData.HashToName);
            foreach (var name in symbolIdProvider.IdMap.Keys)
            {
                var timeline = scml.CreateElement(string.Empty, "timeline", string.Empty);
                var id = symbolIdProvider.IdMap[name];
                timeline.SetAttribute("id", (id + spriteIdProvider.IdMap.Count).ToString());
                timeline.SetAttribute("name", name);
                timeline.SetAttribute("object_type", "bone");
                idToTimeline.Add(id + spriteIdProvider.IdMap.Count, timeline);
            }

            var frameId = 0;
            foreach (var frame in bank.FramesList)
            {
                foreach (var element in frame.ElementsList)
                {
                    idToTimeline[spriteIdProvider.GetId(frame, element)].AppendChild(
                        MakeTimelineKeyNode(scml, element, frameId, rate, fileIdProvider, ChildType.Sprite));
                    idToTimeline[symbolIdProvider.GetId(frame, element) + spriteIdProvider.IdMap.Count]
                        .AppendChild(MakeTimelineKeyNode(scml, element, frameId, rate, fileIdProvider, ChildType.Bone));
                }

                frameId++;
            }

            var timelines = new List<XmlElement>();
            foreach (var timeline in idToTimeline.Values) timelines.Add(timeline);
            return timelines;
        }

        private XmlElement MakeTimelineKeyNode(XmlDocument scml, Element element, int frameId, int rate,
            FileIdProvider fileIdProvider, ChildType childType)
        {
            var key = scml.CreateElement(string.Empty, "key", string.Empty);
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

        private XmlElement MakeObjectNode(XmlDocument scml, Element element, FileIdProvider fileIdProvider)
        {
            var obj = scml.CreateElement(string.Empty, "object", string.Empty);
            obj.SetAttribute("folder", "0");
            obj.SetAttribute("file", GetThisOrPrecedingFile(element, fileIdProvider));
            return obj;
        }

        private XmlElement MakeBoneNode(XmlDocument scml, Element element)
        {
            var obj = scml.CreateElement(string.Empty, "bone", string.Empty);
            obj.SetAttribute("x", (element.M5 * 0.5f).ToString());
            obj.SetAttribute("y", (element.M6 * -0.5f).ToString());
            var scaleX = (float) Math.Sqrt(element.M1 * element.M1 + element.M2 * element.M2);
            var scaleY = (float) Math.Sqrt(element.M3 * element.M3 + element.M4 * element.M4);
            var det = element.M1 * element.M4 - element.M3 * element.M2;
            if (det < 0) scaleY = -scaleY;
            var sinApprox = 0.5f * (element.M3 / scaleY - element.M2 / scaleX);
            var cosApprox = 0.5f * (element.M1 / scaleX + element.M4 / scaleY);
            var angle = (float) Math.Atan2(sinApprox, cosApprox);
            if (angle < 0) angle += (float) (2 * Math.PI);
            angle *= (float) (180.0f / Math.PI);
            obj.SetAttribute("angle", angle.ToString());
            obj.SetAttribute("scale_x", scaleX.ToString());
            obj.SetAttribute("scale_y", scaleY.ToString());
            return obj;
        }

        private XmlElement MakeObjectInfoNode(XmlDocument scml, string name)
        {
            var objectInfo = scml.CreateElement(string.Empty, "obj_info", string.Empty);
            objectInfo.SetAttribute("name", name);
            objectInfo.SetAttribute("type", "bone");
            objectInfo.SetAttribute("w", "50");
            objectInfo.SetAttribute("h", "10");
            return objectInfo;
        }

        private string GetThisOrPrecedingFile(Element element, FileIdProvider fileIdProvider)
        {
            var index = element.Index;
            while (index >= 0)
            {
                var name = $"{AnimationFile.AnimData.HashToName[element.Image]}_{index}";
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
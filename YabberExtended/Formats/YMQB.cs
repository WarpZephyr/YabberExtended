using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YMQB
    {
        public static void Unpack(this MQB mqb, string filename, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\{Path.GetFileNameWithoutExtension(filename)}.mqb.xml", xws);
            xw.WriteStartElement("MQB");
            xw.WriteElementString("Name", mqb.Name);
            xw.WriteElementString("Version", mqb.Version.ToString());
            xw.WriteElementString("Filename", filename);
            xw.WriteElementString("Framerate", mqb.Framerate.ToString());
            xw.WriteElementString("BigEndian", mqb.BigEndian.ToString());
            DcxHelper.WriteCompressionInfo(xw, mqb.Compression, "Compression");
            xw.WriteElementString("ResourceDirectory", mqb.ResourceDirectory);
            xw.WriteStartElement("Resources");
            foreach (var resource in mqb.Resources)
                UnpackResource(xw, resource);
            xw.WriteEndElement();
            xw.WriteStartElement("Cuts");
            foreach (var cut in mqb.Cuts)
                UnpackCut(xw, cut);
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        #region Unpack Helpers

        public static void UnpackResource(XmlWriter xw, MQB.Resource resource)
        {
            xw.WriteStartElement($"Resource");
            xw.WriteElementString("Name", $"{resource.Name}");
            xw.WriteElementString("Path", $"{resource.Path}");
            xw.WriteElementString("ParentIndex", $"{resource.ParentIndex}");
            xw.WriteElementString("Unk48", $"{resource.Unk48}");
            xw.WriteStartElement("Resource_Parameter");
            foreach (var parameter in resource.Parameters)
                UnpackParameter(xw, parameter);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackParameter(XmlWriter xw, MQB.Parameter parameter)
        {
            xw.WriteStartElement($"Parameter");
            xw.WriteElementString("Name", $"{parameter.Name}");
            xw.WriteElementString("Type", $"{parameter.Type}");

            switch (parameter.Type)
            {
                case MQB.Parameter.DataType.Vector:
                    switch (parameter.MemberCount)
                    {
                        case 2:
                            xw.WriteElementString("Value", ((Vector2)parameter.Value).Vector2ToString());
                            break;
                        case 3:
                            xw.WriteElementString("Value", ((Vector3)parameter.Value).Vector3ToString());
                            break;
                        case 4:
                            xw.WriteElementString("Value", ((Vector4)parameter.Value).Vector4ToString());
                            break;
                        default:
                            throw new NotImplementedException($"{nameof(MQB.Parameter.MemberCount)} {parameter.MemberCount} not implemented for: {nameof(MQB.Parameter.DataType.Vector)}");
                    }
                    break;
                case MQB.Parameter.DataType.Custom:
                    xw.WriteElementString("Value", ((byte[])parameter.Value).ToHexString());
                    break;
                default:
                    xw.WriteElementString("Value", $"{parameter.Value}");
                    break;
            }

            xw.WriteElementString("MemberCount", $"{parameter.MemberCount}");
            xw.WriteStartElement("Sequences");
            foreach (var sequence in parameter.Sequences)
                UnpackSequence(xw, sequence);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackSequence(XmlWriter xw, MQB.Parameter.Sequence sequence)
        {
            xw.WriteStartElement($"Sequence");
            xw.WriteElementString("ValueIndex", $"{sequence.ValueIndex}");
            xw.WriteElementString("ValueType", $"{sequence.ValueType}");
            xw.WriteElementString("PointType", $"{sequence.PointType}");
            xw.WriteStartElement("Points");
            foreach (var point in sequence.Points)
                UnpackPoint(xw, point);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackPoint(XmlWriter xw, MQB.Parameter.Sequence.Point point)
        {
            xw.WriteStartElement($"Point");
            xw.WriteElementString("Value", $"{point.Value}");
            xw.WriteElementString("Unk08", $"{point.Unk08}");
            xw.WriteElementString("Unk10", $"{point.Unk10}");
            xw.WriteElementString("Unk14", $"{point.Unk14}");
            xw.WriteEndElement();
        }

        public static void UnpackCut(XmlWriter xw, MQB.Cut cut)
        {
            xw.WriteStartElement($"Cut");
            xw.WriteElementString("Name", $"{cut.Name}");
            xw.WriteElementString("Duration", $"{cut.Duration}");
            xw.WriteElementString("Unk44", $"{cut.Unk44}");
            xw.WriteStartElement($"Timelines");
            foreach (var timeline in cut.Timelines)
                UnpackTimeline(xw, timeline);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackTimeline(XmlWriter xw, MQB.Timeline timeline)
        {
            xw.WriteStartElement($"Timeline");
            xw.WriteElementString("Unk10", $"{timeline.Unk10}");
            xw.WriteStartElement($"Events");
            foreach (var @event in timeline.Events)
                UnpackEvent(xw, @event);
            xw.WriteEndElement();
            xw.WriteStartElement($"Timeline_Parameter");
            foreach (var parameter in timeline.Parameters)
                UnpackParameter(xw, parameter);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackEvent(XmlWriter xw, MQB.Event @event)
        {
            xw.WriteStartElement($"Event");
            xw.WriteElementString("ID", $"{@event.ID}");
            xw.WriteElementString("Duration", $"{@event.Duration}");
            xw.WriteElementString("ResourceIndex", $"{@event.ResourceIndex}");
            xw.WriteElementString("StartFrame", $"{@event.StartFrame}");
            xw.WriteElementString("Unk08", $"{@event.Unk08}");
            xw.WriteElementString("Unk14", $"{@event.Unk14}");
            xw.WriteElementString("Unk18", $"{@event.Unk18}");
            xw.WriteElementString("Unk1C", $"{@event.Unk1C}");
            xw.WriteElementString("Unk20", $"{@event.Unk20}");
            xw.WriteElementString("Unk28", $"{@event.Unk28}");
            xw.WriteStartElement($"Transforms");
            foreach (var transform in @event.Transforms)
                UnpackTransform(xw, transform);
            xw.WriteEndElement();
            xw.WriteStartElement($"Event_Parameter");
            foreach (var parameter in @event.Parameters)
                UnpackParameter(xw, parameter);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void UnpackTransform(XmlWriter xw, MQB.Transform transform)
        {
            xw.WriteStartElement($"Transform");
            xw.WriteElementString("Frame", $"{transform.Frame}");
            xw.WriteElementString("Translation", transform.Translation.Vector3ToString());
            xw.WriteElementString("Rotation", transform.Rotation.Vector3ToString());
            xw.WriteElementString("Scale", transform.Scale.Vector3ToString());
            xw.WriteElementString("Unk10", transform.Unk10.Vector3ToString());
            xw.WriteElementString("Unk1C", transform.Unk1C.Vector3ToString());
            xw.WriteElementString("Unk34", transform.Unk34.Vector3ToString());
            xw.WriteElementString("Unk40", transform.Unk40.Vector3ToString());
            xw.WriteElementString("Unk58", transform.Unk58.Vector3ToString());
            xw.WriteElementString("Unk64", transform.Unk64.Vector3ToString());
            xw.WriteEndElement();
        }

        #endregion

        public static void Repack(string sourceFile)
        {
            var mqb = new MQB();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);

            string name = xml.SelectSingleNode("MQB/Name").InnerText;
            var version = FriendlyParseEnum<MQB.MQBVersion>(nameof(MQB), nameof(MQB.Version), xml.SelectSingleNode("MQB/Version").InnerText);
            float framerate = FriendlyParseFloat32(nameof(MQB), nameof(MQB.Framerate), xml.SelectSingleNode("MQB/Framerate").InnerText);
            bool bigendian = FriendlyParseBool(nameof(MQB), nameof(MQB.BigEndian), xml.SelectSingleNode("MQB/BigEndian").InnerText);

            string resDir = xml.SelectSingleNode("MQB/ResourceDirectory").InnerText;
            List<MQB.Resource> resources = new List<MQB.Resource>();
            List<MQB.Cut> cuts = new List<MQB.Cut>();

            var resourcesNode = xml.SelectSingleNode("MQB/Resources");
            foreach (XmlNode resNode in resourcesNode.SelectNodes("Resource"))
                resources.Add(RepackResource(resNode));

            var cutsNode = xml.SelectSingleNode("MQB/Cuts");
            foreach (XmlNode cutNode in cutsNode.SelectNodes("Cut"))
                cuts.Add(RepackCut(cutNode));

            mqb.Name = name;
            mqb.Version = version;
            mqb.Framerate = framerate;
            mqb.BigEndian = bigendian;
            mqb.Compression = DcxHelper.ReadCompressionInfo(xml.SelectSingleNode("MQB"), "Compression");
            mqb.ResourceDirectory = resDir;
            mqb.Resources = resources;
            mqb.Cuts = cuts;

            string outPath = sourceFile.Replace(".mqb.xml", ".mqb");
            YabberUtil.BackupFile(outPath);
            mqb.Write(outPath);
        }

        #region Repack Helpers

        public static MQB.Resource RepackResource(XmlNode resNode)
        {
            MQB.Resource resource = new MQB.Resource();

            string name = resNode.SelectSingleNode("Name").InnerText;
            string path = resNode.SelectSingleNode("Path").InnerText;
            int parentIndex = FriendlyParseInt32(nameof(MQB.Resource), nameof(MQB.Resource.ParentIndex), resNode.SelectSingleNode("ParentIndex").InnerText);
            int unk48 = FriendlyParseInt32(nameof(MQB.Resource), nameof(MQB.Resource.Unk48), resNode.SelectSingleNode("Unk48").InnerText);
            List<MQB.Parameter> parameter = new List<MQB.Parameter>();

            var resParamNode = resNode.SelectSingleNode("Resource_Parameter");
            foreach (XmlNode paramNode in resParamNode.SelectNodes("Parameter"))
                parameter.Add(RepackParameter(paramNode));

            resource.Name = name;
            resource.Path = path;
            resource.ParentIndex = parentIndex;
            resource.Unk48 = unk48;
            resource.Parameters = parameter;
            return resource;
        }

        public static MQB.Parameter RepackParameter(XmlNode parameterNode)
        {
            MQB.Parameter parameter = new MQB.Parameter();

            string name = parameterNode.SelectSingleNode("Name").InnerText;
            var type = FriendlyParseEnum<MQB.Parameter.DataType>(nameof(MQB.Parameter), nameof(MQB.Parameter.Type), parameterNode.SelectSingleNode("Type").InnerText);

            int memberCount = FriendlyParseInt32(nameof(MQB.Parameter), nameof(MQB.Parameter.MemberCount), parameterNode.SelectSingleNode("MemberCount").InnerText);
            object value = ConvertValueToDataType(parameterNode.SelectSingleNode("Value").InnerText, type, memberCount);
            List<MQB.Parameter.Sequence> sequences = new List<MQB.Parameter.Sequence>();

            var seqsNode = parameterNode.SelectSingleNode("Sequences");
            foreach (XmlNode seqNode in seqsNode.SelectNodes("Sequence"))
                sequences.Add(RepackSequence(seqNode));

            parameter.Name = name;
            parameter.Type = type;
            parameter.Value = value;
            parameter.MemberCount = memberCount;
            parameter.Sequences = sequences;
            return parameter;
        }

        public static MQB.Parameter.Sequence RepackSequence(XmlNode seqNode)
        {
            MQB.Parameter.Sequence sequence = new MQB.Parameter.Sequence();

            int valueIndex = FriendlyParseInt32(nameof(MQB.Parameter.Sequence), nameof(MQB.Parameter.Sequence.ValueIndex), seqNode.SelectSingleNode("ValueIndex").InnerText);
            var type = FriendlyParseEnum<MQB.Parameter.DataType>(nameof(MQB.Parameter.Sequence), nameof(MQB.Parameter.Sequence.ValueType), seqNode.SelectSingleNode("ValueType").InnerText);
            int pointType = FriendlyParseInt32(nameof(MQB.Parameter.Sequence), nameof(MQB.Parameter.Sequence.PointType), seqNode.SelectSingleNode("PointType").InnerText);
            List<MQB.Parameter.Sequence.Point> points = new List<MQB.Parameter.Sequence.Point>();

            var pointsNode = seqNode.SelectSingleNode("Points");
            foreach (XmlNode pointNode in pointsNode.SelectNodes("Point"))
                points.Add(RepackPoint(pointNode, type));

            sequence.ValueIndex = valueIndex;
            sequence.ValueType = type;
            sequence.PointType = pointType;
            sequence.Points = points;
            return sequence;
        }

        public static MQB.Parameter.Sequence.Point RepackPoint(XmlNode pointNode, MQB.Parameter.DataType type)
        {
            MQB.Parameter.Sequence.Point point = new MQB.Parameter.Sequence.Point();

            string valueStr = pointNode.SelectSingleNode("Value").InnerText;
            object value;

            switch (type)
            {
                case MQB.Parameter.DataType.Byte: value = FriendlyParseByte(nameof(MQB.Parameter.Sequence.Point), nameof(MQB.Parameter.Sequence.Point.Value), valueStr); break;
                case MQB.Parameter.DataType.Float: value = FriendlyParseFloat32(nameof(MQB.Parameter.Sequence.Point), nameof(MQB.Parameter.Sequence.Point.Value), valueStr); break;
                default: throw new NotSupportedException($"Unsupported sequence point value type: {type}");
            }

            int unk08 = FriendlyParseInt32(nameof(MQB.Parameter.Sequence.Point), nameof(MQB.Parameter.Sequence.Point.Unk08), pointNode.SelectSingleNode("Unk08").InnerText);
            float unk10 = FriendlyParseFloat32(nameof(MQB.Parameter.Sequence.Point), nameof(MQB.Parameter.Sequence.Point.Unk10), pointNode.SelectSingleNode("Unk10").InnerText);
            float unk14 = FriendlyParseFloat32(nameof(MQB.Parameter.Sequence.Point), nameof(MQB.Parameter.Sequence.Point.Unk14), pointNode.SelectSingleNode("Unk14").InnerText);

            point.Value = value;
            point.Unk08 = unk08;
            point.Unk10 = unk10;
            point.Unk14 = unk14;
            return point;
        }

        public static MQB.Cut RepackCut(XmlNode cutNode)
        {
            MQB.Cut cut = new MQB.Cut();

            string name = cutNode.SelectSingleNode("Name").InnerText;
            int duration = FriendlyParseInt32("Cut", nameof(cut.Duration), cutNode.SelectSingleNode("Duration").InnerText);
            int unk44 = FriendlyParseInt32("Cut", nameof(cut.Unk44), cutNode.SelectSingleNode("Unk44").InnerText);
            List<MQB.Timeline> timelines = new List<MQB.Timeline>();

            var timelinesNode = cutNode.SelectSingleNode("Timelines");
            foreach (XmlNode timelineNode in timelinesNode.SelectNodes("Timeline"))
                timelines.Add(RepackTimeline(timelineNode));

            cut.Name = name;
            cut.Duration = duration;
            cut.Unk44 = unk44;
            cut.Timelines = timelines;
            return cut;
        }

        public static MQB.Timeline RepackTimeline(XmlNode timelineNode)
        {
            MQB.Timeline timeline = new MQB.Timeline();

            int unk10 = FriendlyParseInt32(nameof(MQB.Timeline), nameof(MQB.Timeline.Unk10), timelineNode.SelectSingleNode("Unk10").InnerText);
            List<MQB.Event> events = new List<MQB.Event>();
            List<MQB.Parameter> parameters = new List<MQB.Parameter>();

            var eventsNode = timelineNode.SelectSingleNode("Events");
            foreach (XmlNode eventNode in eventsNode.SelectNodes("Event"))
                events.Add(RepackEvent(eventNode));

            var timelineParamNode = timelineNode.SelectSingleNode("Timeline_Parameter");
            foreach (XmlNode paramNode in timelineParamNode.SelectNodes("Parameter"))
                parameters.Add(RepackParameter(paramNode));

            timeline.Unk10 = unk10;
            timeline.Events = events;
            timeline.Parameters = parameters;
            return timeline;
        }

        public static MQB.Event RepackEvent(XmlNode eventNode)
        {
            MQB.Event mqbEvent = new MQB.Event();

            int id = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.ID), eventNode.SelectSingleNode("ID").InnerText);
            int duration = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Duration), eventNode.SelectSingleNode("Duration").InnerText);
            int resIndex = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.ResourceIndex), eventNode.SelectSingleNode("ResourceIndex").InnerText);
            int startFrame = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.StartFrame), eventNode.SelectSingleNode("StartFrame").InnerText);
            int unk08 = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk08), eventNode.SelectSingleNode("Unk08").InnerText);
            int unk14 = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk14), eventNode.SelectSingleNode("Unk14").InnerText);
            int unk18 = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk18), eventNode.SelectSingleNode("Unk18").InnerText);
            int unk1C = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk1C), eventNode.SelectSingleNode("Unk1C").InnerText);
            int unk20 = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk20), eventNode.SelectSingleNode("Unk20").InnerText);
            int unk28 = FriendlyParseInt32(nameof(MQB.Event), nameof(MQB.Event.Unk28), eventNode.SelectSingleNode("Unk28").InnerText);
            List<MQB.Transform> transforms = new List<MQB.Transform>();
            List<MQB.Parameter> parameters = new List<MQB.Parameter>();

            var transformsNode = eventNode.SelectSingleNode("Transforms");
            foreach (XmlNode transformNode in transformsNode.SelectNodes("Transform"))
                transforms.Add(RepackTransform(transformNode));

            var eventParamNodes = eventNode.SelectSingleNode("Event_Parameter");
            foreach (XmlNode paramNode in eventParamNodes.SelectNodes("Parameter"))
                parameters.Add(RepackParameter(paramNode));

            mqbEvent.ID = id;
            mqbEvent.Duration = duration;
            mqbEvent.ResourceIndex = resIndex;
            mqbEvent.StartFrame = startFrame;
            mqbEvent.Unk08 = unk08;
            mqbEvent.Unk14 = unk14;
            mqbEvent.Unk18 = unk18;
            mqbEvent.Unk1C = unk1C;
            mqbEvent.Unk20 = unk20;
            mqbEvent.Unk28 = unk28;
            mqbEvent.Transforms = transforms;
            mqbEvent.Parameters = parameters;
            return mqbEvent;
        }

        public static MQB.Transform RepackTransform(XmlNode transNode)
        {
            MQB.Transform transform = new MQB.Transform();

            float frame = FriendlyParseFloat32(nameof(MQB.Transform), nameof(MQB.Transform.Frame), transNode.SelectSingleNode("Frame").InnerText);
            Vector3 translation = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Translation), transNode.SelectSingleNode("Translation").InnerText);
            Vector3 rotation = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Rotation), transNode.SelectSingleNode("Rotation").InnerText);
            Vector3 scale = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Scale), transNode.SelectSingleNode("Scale").InnerText);
            Vector3 unk10 = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk10), transNode.SelectSingleNode("Unk10").InnerText);
            Vector3 unk1C = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk1C), transNode.SelectSingleNode("Unk1C").InnerText);
            Vector3 unk34 = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk34), transNode.SelectSingleNode("Unk34").InnerText);
            Vector3 unk40 = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk40), transNode.SelectSingleNode("Unk40").InnerText);
            Vector3 unk58 = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk58), transNode.SelectSingleNode("Unk58").InnerText);
            Vector3 unk64 = FriendlyParseVector3(nameof(MQB.Transform), nameof(MQB.Transform.Unk64), transNode.SelectSingleNode("Unk64").InnerText);

            transform.Frame = frame;
            transform.Translation = translation;
            transform.Rotation = rotation;
            transform.Scale = scale;
            transform.Unk10 = unk10;
            transform.Unk1C = unk1C;
            transform.Unk34 = unk34;
            transform.Unk40 = unk40;
            transform.Unk58 = unk58;
            transform.Unk64 = unk64;
            return transform;
        }

        public static string Vector2ToString(this Vector2 vector)
        {
            return $"X:{vector.X} Y:{vector.Y}";
        }

        public static string Vector3ToString(this Vector3 vector)
        {
            return $"X:{vector.X} Y:{vector.Y} Z:{vector.Z}";
        }

        public static string Vector4ToString(this Vector4 vector)
        {
            return $"X:{vector.X} Y:{vector.Y} Z:{vector.Z} W:{vector.W}";
        }

        public static Vector2 ToVector2(this string str)
        {
            int xStartIndex = str.IndexOf("X:") + 2;
            int yStartIndex = str.IndexOf("Y:") + 2;

            string xStr = str.Substring(xStartIndex, yStartIndex - xStartIndex - 3);
            string yStr = str.Substring(yStartIndex, str.Length - yStartIndex);

            float x = float.Parse(xStr);
            float y = float.Parse(yStr);

            return new Vector2(x, y);
        }

        public static Vector3 ToVector3(this string str)
        {
            int xStartIndex = str.IndexOf("X:") + 2;
            int yStartIndex = str.IndexOf("Y:") + 2;
            int zStartIndex = str.IndexOf("Z:") + 2;

            string xStr = str.Substring(xStartIndex, yStartIndex - xStartIndex - 3);
            string yStr = str.Substring(yStartIndex, zStartIndex - yStartIndex - 3);
            string zStr = str.Substring(zStartIndex, str.Length - zStartIndex);

            float x = float.Parse(xStr);
            float y = float.Parse(yStr);
            float z = float.Parse(zStr);

            return new Vector3(x, y, z);
        }

        public static Vector4 ToVector4(this string str)
        {
            int xStartIndex = str.IndexOf("X:") + 2;
            int yStartIndex = str.IndexOf("Y:") + 2;
            int zStartIndex = str.IndexOf("Z:") + 2;
            int wStartIndex = str.IndexOf("W:") + 2;

            string xStr = str.Substring(xStartIndex, yStartIndex - xStartIndex - 3);
            string yStr = str.Substring(yStartIndex, zStartIndex - yStartIndex - 3);
            string zStr = str.Substring(zStartIndex, wStartIndex - zStartIndex - 3);
            string wStr = str.Substring(wStartIndex, str.Length - wStartIndex);

            float x = float.Parse(xStr);
            float y = float.Parse(yStr);
            float z = float.Parse(zStr);
            float w = float.Parse(wStr);

            return new Vector4(x, y, z, w);
        }

        public static object ConvertValueToDataType(string str, MQB.Parameter.DataType type, int memberCount)
        {
            try
            {
                switch (type)
                {
                    case MQB.Parameter.DataType.Bool: return Convert.ToBoolean(str);
                    case MQB.Parameter.DataType.SByte: return Convert.ToSByte(str);
                    case MQB.Parameter.DataType.Byte: return Convert.ToByte(str);
                    case MQB.Parameter.DataType.Short: return Convert.ToInt16(str);
                    case MQB.Parameter.DataType.Int: return Convert.ToInt32(str);
                    case MQB.Parameter.DataType.UInt: return Convert.ToUInt32(str);
                    case MQB.Parameter.DataType.Float: return Convert.ToSingle(str);
                    case MQB.Parameter.DataType.String: return str;
                    case MQB.Parameter.DataType.Custom: return str.FriendlyHexToByteArray();
                    case MQB.Parameter.DataType.Color: return ConvertValueToColor(str);
                    case MQB.Parameter.DataType.IntColor: return ConvertValueToColor(str);
                    case MQB.Parameter.DataType.Vector:
                        switch (memberCount)
                        {
                            case 2: return str.ToVector2();
                            case 3: return str.ToVector3();
                            case 4: return str.ToVector4();
                            default: throw new NotSupportedException($"{nameof(MQB.Parameter.MemberCount)} {memberCount} not supported for: {nameof(MQB.Parameter.DataType.Vector)}");
                        }
                    default: throw new NotImplementedException($"Unimplemented parameter type: {type}");
                }
            }
            catch
            {
                throw new FriendlyException($"The value \"{str}\" could not be converted to the type {type} during parameter repacking.");
            }
        }

        public static object ConvertValueToColor(string value)
        {
            string GetComponent(string component)
            {
                int componentIndex = value.IndexOf(component);
                if (componentIndex < 0)
                {
                    return string.Empty;
                }

                int startIndex = componentIndex + component.Length;
                if (startIndex >= value.Length)
                {
                    return string.Empty;
                }

                int endIndex = value.IndexOf(',', startIndex);
                if (endIndex < 0)
                {
                    endIndex = value.IndexOf(']', startIndex);
                }

                if (endIndex < 0)
                {
                    return string.Empty;
                }
                else
                {
                    int length = endIndex - startIndex;
                    return value.Substring(startIndex, length);
                }
            }

            static string CleanComponent(string componentValue)
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in componentValue)
                {
                    if (char.IsDigit(c))
                    {
                        sb.Append(c);
                    }
                }

                return sb.ToString();
            }

            static int ParseComponent(string componentValue, int defaultValue)
            {
                if (int.TryParse(componentValue, out int result))
                {
                    return result;
                }

                return defaultValue;
            }

            string alphaComponent = GetComponent("A=");
            string redComponent = GetComponent("R=");
            string greenComponent = GetComponent("G=");
            string blueComponent = GetComponent("B=");
            alphaComponent = CleanComponent(alphaComponent);
            redComponent = CleanComponent(redComponent);
            greenComponent = CleanComponent(greenComponent);
            blueComponent = CleanComponent(blueComponent);
            int alpha = ParseComponent(alphaComponent, 255);
            int red = ParseComponent(redComponent, 0);
            int green = ParseComponent(greenComponent, 0);
            int blue = ParseComponent(blueComponent, 0);
            alpha = Math.Clamp(alpha, 0, 255);
            red = Math.Clamp(red, 0, 255);
            green = Math.Clamp(green, 0, 255);
            blue = Math.Clamp(blue, 0, 255);

            return Color.FromArgb(alpha, red, green, blue);
        }

        public static byte FriendlyParseByte(string parentName, string valueName, string value)
        {
            try
            {
                return byte.Parse(value);
            }
            catch (FormatException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} is a number, but its value \"{value}\" could not be read as a number.");
            }
            catch (OverflowException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} with value \"{value}\" caused an overflow error, it may be too large or too small.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        public static bool FriendlyParseBool(string parentName, string valueName, string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch (FormatException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} is a True or False, but its value \"{value}\" could not be read as a True or False.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        public static int FriendlyParseInt32(string parentName, string valueName, string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (FormatException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} is a number, but its value \"{value}\" could not be read as a number.");
            }
            catch (OverflowException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} with value \"{value}\" caused an overflow error, it may be too large or too small.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        public static float FriendlyParseFloat32(string parentName, string valueName, string value)
        {
            try
            {
                return float.Parse(value);
            }
            catch (FormatException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} is a number, but its value \"{value}\" could not be read as a number.");
            }
            catch (OverflowException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} with value \"{value}\" caused an overflow error, it may be too large or too small.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        public static Vector3 FriendlyParseVector3(string parentName, string valueName, string value)
        {
            try
            {
                return value.ToVector3();
            }
            catch (FormatException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} is a vector, but its value \"{value}\" could not be parsed as one. Example how format should be: 1 1 1");
            }
            catch (OverflowException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} with value \"{value}\" caused an overflow error, it may be too large or too small.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        public static TEnum FriendlyParseEnum<TEnum>(string parentName, string valueName, string value) where TEnum : Enum
        {
            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), value);
            }
            catch (OverflowException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} with value \"{value}\" caused an overflow error, it may be too large or too small.");
            }
            catch (ArgumentNullException)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had a null value, make sure it exists.");
            }
            catch (Exception ex)
            {
                throw new FriendlyException($"In a {parentName} {valueName} had an unknown error occur; Exception message: {ex.Message}");
            }
        }

        #endregion

        #region Hex Helpers

        public static byte[] FriendlyHexToByteArray(this string hex)
        {
            if (!IsValidHexString(hex))
            {
                throw new FriendlyException("A hex string in a Parameter's value could not be parsed as hex. Valid hex characters are: 0-9 A-F a-f");
            }

            if (hex.Length == 0)
            {
                hex = "00000000";
                Console.WriteLine("Warning: Hex string was empty, adding 00000000...");
            }

            if (hex.Length % 2 != 0)
            {
                hex += "0";
                Console.WriteLine("Warning: Hex string was not divisible by 2, adding 0...");
            }

            if (hex.Length / 2 % 4 != 0)
            {
                while (hex.Length / 2 % 4 != 0)
                {
                    hex += "00";
                }
                Console.WriteLine("Warning: Hex string was not divisible by 4 for Custom type of Parameter, added 00 until it was.");
            }

            try
            {
                return hex.HexToByteArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An issue occurred in parsing a hex string into a byte array.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        public static bool IsValidHexString(IEnumerable<char> hexString)
        {
            return hexString.Select((char currentCharacter) =>
                (currentCharacter >= '0' && currentCharacter <= '9')
             || (currentCharacter >= 'a' && currentCharacter <= 'f')
             || (currentCharacter >= 'A' && currentCharacter <= 'F')
             ).All((bool isHexCharacter) => isHexCharacter);
        }

        public static string ToHexString(this byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString().ToUpper();
        }

        public static byte[] HexToByteArray(this string hex)
        {
            int charLength = hex.Length;
            byte[] bytes = new byte[charLength / 2];
            for (int i = 0; i < charLength; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        #endregion
    }
}

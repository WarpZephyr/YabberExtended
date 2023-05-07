using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SoulsFormats;

namespace Yabber
{
    static class YDebSub
    {
        public static void Unpack(this DebriefingSubtitle sub, string sourceName, string sourceDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(sourceDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.NewLineHandling = NewLineHandling.None;
            var xw = XmlWriter.Create($"{sourceDir}\\{sourceName}.debsub.xml", xws);
            xw.WriteStartElement("DebriefingSubtitle");
            xw.WriteElementString("SubtitleCount", $"{sub.Subtitles.Count}");
            xw.WriteElementString("VideoName", $"{sub.VideoName}");
            xw.WriteElementString("VideoWidth", $"{sub.Width}");
            xw.WriteElementString("VideoHeight", $"{sub.Height}");
            xw.WriteElementString("Unk0C", $"{sub.Unk0C}");
            xw.WriteElementString("Unk14", $"{sub.Unk14}");
            xw.WriteElementString("EventID", $"{sub.EventID}");
            xw.WriteStartElement("Subtitles");
            for (int i = 0; i < sub.Subtitles.Count; i++)
            {
                var subtitle = sub.Subtitles[i];
                xw.WriteStartElement("Subtitle");
                xw.WriteElementString("FrameDelay", $"{subtitle.FrameDelay}");
                xw.WriteElementString("FrameTime", $"{subtitle.FrameTime}");
                xw.WriteElementString("Text", $"{subtitle.Text}");
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceFile)
        {
            DebriefingSubtitle sub = new DebriefingSubtitle();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);
            sub.VideoName = xml.SelectSingleNode("DebriefingSubtitle/VideoName").InnerText;
            sub.Width = short.Parse(xml.SelectSingleNode("DebriefingSubtitle/VideoWidth").InnerText);
            sub.Height = short.Parse(xml.SelectSingleNode("DebriefingSubtitle/VideoHeight").InnerText);
            sub.Unk0C = uint.Parse(xml.SelectSingleNode("DebriefingSubtitle/Unk0C").InnerText);
            sub.Unk14 = ushort.Parse(xml.SelectSingleNode("DebriefingSubtitle/Unk14").InnerText);
            sub.EventID = ushort.Parse(xml.SelectSingleNode("DebriefingSubtitle/EventID").InnerText);
            sub.Subtitles = new List<DebriefingSubtitle.Subtitle>();

            foreach (XmlNode textNode in xml.SelectNodes("DebriefingSubtitle/Subtitles/Subtitle"))
            {
                short frameDelay = short.Parse(textNode.ChildNodes[0].InnerText);
                short frameTime = short.Parse(textNode.ChildNodes[1].InnerText);
                string text = textNode.ChildNodes[2].InnerText;
                sub.Subtitles.Add(new DebriefingSubtitle.Subtitle(frameDelay, frameTime, text));
            }

            string outPath = sourceFile.Replace(".bin.debsub.xml", ".bin");
            YBUtil.Backup(outPath);
            sub.Write(outPath);
        }
    }
}

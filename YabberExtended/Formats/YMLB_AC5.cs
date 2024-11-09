using SoulsFormats;
using System;
using System.Xml;
using YabberExtended.Extensions.Xml;

namespace YabberExtended
{
    static class YMLB_AC5
    {
        public static void Unpack(this MLB_AC5 mlb, string sourceFile)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;

            XmlWriter xw = XmlWriter.Create($"{sourceFile}.ac5.xml", xws);
            xw.WriteStartElement("mlb_ac5");
            xw.WriteElementString("type", mlb.Type.ToString());
            xw.WriteStartElement("resources");
            for (int i = 0; i < mlb.Resources.Count; i++)
            {
                var resource = mlb.Resources[i];
                UnpackResource(xw, mlb, resource, i);
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceFile)
        {
            MLB_AC5 mlb = new MLB_AC5();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);

            mlb.Type = xml.ReadEnum<MLB_AC5.ResourceType>("mlb_ac5/type");

            XmlNodeList? resourceNodes = xml.SelectNodes("mlb_ac5/resources/resource");
            if (resourceNodes != null)
            {
                foreach (XmlNode resourceNode in resourceNodes)
                {
                    MLB_AC5.Model resource;
                    if (mlb.Type == MLB_AC5.ResourceType.Model)
                    {
                        resource = new MLB_AC5.Model();
                        resource.Path = resourceNode.ReadString("path");
                        resource.RelativePath = resourceNode.ReadString("relativePath");
                        RepackModel(resource, resourceNode);
                    }
                    else
                    {
                        throw new NotSupportedException($"{nameof(MLB_AC5.ResourceType)} {mlb.Type} is not supported or is invalid.");
                    }

                    mlb.Resources.Add(resource);
                }
            }

            string outPath = sourceFile.Replace(".mlb.ac5.xml", ".mlb");
            YabberUtil.BackupFile(outPath);
            mlb.Write(outPath);
        }

        #region Unpack

        #region Resource

        private static void UnpackResource(XmlWriter xw, MLB_AC5 mlb, MLB_AC5.Model resource, int index)
        {
            xw.WriteStartElement("resource");
            xw.WriteElementString("path", resource.Path);
            xw.WriteElementString("relativePath", resource.RelativePath);
            if (mlb.Type == MLB_AC5.ResourceType.Model)
            {
                UnpackModel(xw, resource);
            }
            else
            {
                throw new NotSupportedException($"{nameof(MLB_AC5.ResourceType)} {mlb.Type} is not supported or is invalid.");
            }
            xw.WriteEndElement();
        }

        #endregion

        #region Model

        private static void UnpackModel(XmlWriter xw, MLB_AC5.Model model)
        {
            if (model.Materials.Count > 0)
            {
                xw.WriteStartElement("materials");
                foreach (var material in model.Materials)
                {
                    UnpackMaterial(xw, material);
                }
                xw.WriteEndElement();
            }

            if (model.Bones.Count > 0)
            {
                xw.WriteStartElement("bones");
                for (int i = 0; i < model.Bones.Count; i++)
                {
                    var bone = model.Bones[i];
                    UnpackModelBone(xw, bone);
                }
                xw.WriteEndElement();
            }

            if (model.HasLodConfig)
            {
                xw.WriteStartElement("lodConfig");
                xw.WriteElementString("distance", model.LodConfig.Distance.ToString());
                xw.WriteEndElement();
            }

            if (model.HasBreakConfig)
            {
                xw.WriteStartElement("breakConfig");
                xw.WriteElementString("unk00", model.BreakConfig.Unk00.ToString());
                xw.WriteElementString("unk04", model.BreakConfig.Unk04.ToString());
                xw.WriteEndElement();
            }

            if (model.HasShadowConfig)
            {
                xw.WriteStartElement("shadowConfig");
                xw.WriteElementString("unk00", model.ShadowConfig.Unk00.ToString());
                xw.WriteEndElement();
            }

            if (model.HasCollisionConfig)
            {
                xw.WriteStartElement("collisionConfig");
                xw.WriteElementString("unk00", model.CollisionConfig.Unk00.ToString());
                xw.WriteEndElement();
            }
        }

        private static void UnpackMaterial(XmlWriter xw, MLB_AC5.Model.Material material)
        {
            xw.WriteStartElement("material");
            xw.WriteElementString("name", material.Name);
            if (material.HasConfig1)
            {
                xw.WriteStartElement("config1");
                xw.WriteElementString("unk00", material.Config1.Unk00.ToString());
                xw.WriteElementString("unk04", material.Config1.Unk04.ToString());
                xw.WriteElementString("unk08", material.Config1.Unk08.ToString());
                xw.WriteElementString("unk0C", material.Config1.Unk0C.ToString());
                xw.WriteElementString("unk10", material.Config1.Unk10.ToString());
                xw.WriteElementString("unk14", material.Config1.Unk14.ToString());
                xw.WriteElementString("unk18", material.Config1.Unk18.ToString());
                xw.WriteElementString("unk1C", material.Config1.Unk1C.ToString());
                xw.WriteElementString("unk20", material.Config1.Unk20.ToString());
                xw.WriteElementString("unk24", material.Config1.Unk24.ToString());
                xw.WriteElementString("unk28", material.Config1.Unk28.ToString());
                xw.WriteElementString("unk2C", material.Config1.Unk2C.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void UnpackModelBone(XmlWriter xw, MLB_AC5.Model.Bone bone)
        {
            xw.WriteStartElement("bone");
            xw.WriteElementString("name", bone.Name);
            if (bone.HasBreakConfig)
            {
                xw.WriteStartElement("breakConfig");
                xw.WriteElementString("unk00", bone.BreakConfig.Unk00.ToString());
                xw.WriteElementString("unk04", bone.BreakConfig.Unk04.ToString());
                xw.WriteElementString("unk08", bone.BreakConfig.Unk08.ToString());
                xw.WriteEndElement();
            }

            if (bone.HasCollisionConfig)
            {
                xw.WriteStartElement("collisionConfig");
                xw.WriteElementString("unk00", bone.CollisionConfig.Unk00.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        #endregion

        #endregion

        #region Repack

        #region Model

        private static void RepackModel(MLB_AC5.Model resource, XmlNode node)
        {
            XmlNodeList? materialNodes = node.SelectNodes("materials/material");
            if (materialNodes != null)
            {
                foreach (XmlNode materialNode in materialNodes)
                {
                    var material = new MLB_AC5.Model.Material();
                    RepackMaterial(material, materialNode);
                    resource.Materials.Add(material);
                }
            }

            XmlNodeList? boneNodes = node.SelectNodes("bones/bone");
            if (boneNodes != null)
            {
                foreach (XmlNode boneNode in boneNodes)
                {
                    var bone = new MLB_AC5.Model.Bone();
                    RepackModelBone(bone, boneNode);
                    resource.Bones.Add(bone);
                }
            }

            XmlNode? lodNode = node.SelectSingleNode("lodConfig");
            if (lodNode != null)
            {
                resource.HasLodConfig = true;
                resource.LodConfig.Distance = lodNode.ReadSingleOrDefault("distance");
            }

            XmlNode? breakNode = node.SelectSingleNode("breakConfig");
            if (breakNode != null)
            {
                resource.HasBreakConfig = true;
                resource.BreakConfig.Unk00 = breakNode.ReadInt32OrDefault("unk00");
                resource.BreakConfig.Unk04 = breakNode.ReadInt32OrDefault("unk04");
            }

            XmlNode? shadowNode = node.SelectSingleNode("shadowConfig");
            if (shadowNode != null)
            {
                resource.HasShadowConfig = true;
                resource.ShadowConfig.Unk00 = shadowNode.ReadSingleOrDefault("unk00");
            }

            XmlNode? collisionNode = node.SelectSingleNode("collisionConfig");
            if (collisionNode != null)
            {
                resource.HasCollisionConfig = true;
                resource.CollisionConfig.Unk00 = collisionNode.ReadInt16OrDefault("unk00");
            }
        }

        private static void RepackMaterial(MLB_AC5.Model.Material material, XmlNode node)
        {
            material.Name = node.ReadString("name");
            material.Config1 = new MLB_AC5.Model.Material.MaterialConfig1();
            XmlNode? nodeConfig1 = node.SelectSingleNode("config1");
            if (nodeConfig1 != null)
            {
                material.HasConfig1 = true;
                material.Config1.Unk00 = nodeConfig1.ReadSingleOrDefault("unk00");
                material.Config1.Unk04 = nodeConfig1.ReadSingleOrDefault("unk04");
                material.Config1.Unk08 = nodeConfig1.ReadSingleOrDefault("unk08");
                material.Config1.Unk0C = nodeConfig1.ReadSingleOrDefault("unk0C");
                material.Config1.Unk10 = nodeConfig1.ReadSingleOrDefault("unk10");
                material.Config1.Unk14 = nodeConfig1.ReadSingleOrDefault("unk14");
                material.Config1.Unk18 = nodeConfig1.ReadSingleOrDefault("unk18");
                material.Config1.Unk1C = nodeConfig1.ReadSingleOrDefault("unk1C");
                material.Config1.Unk20 = nodeConfig1.ReadSingleOrDefault("unk20");
                material.Config1.Unk24 = nodeConfig1.ReadSingleOrDefault("unk24");
                material.Config1.Unk28 = nodeConfig1.ReadSingleOrDefault("unk28");
                material.Config1.Unk2C = nodeConfig1.ReadSingleOrDefault("unk2C");
            }
            else
            {
                material.HasConfig1 = false;
            }
        }

        private static void RepackModelBone(MLB_AC5.Model.Bone bone, XmlNode node)
        {
            bone.Name = node.ReadString("name");
            XmlNode? breakNode = node.SelectSingleNode("breakConfig");
            if (breakNode != null)
            {
                bone.HasBreakConfig = true;
                bone.BreakConfig.Unk00 = breakNode.ReadInt32OrDefault("unk00");
                bone.BreakConfig.Unk04 = breakNode.ReadInt32OrDefault("unk04");
                bone.BreakConfig.Unk08 = breakNode.ReadByteOrDefault("unk08");
            }
            else
            {
                bone.HasBreakConfig = false;
            }

            XmlNode? collisionNode = node.SelectSingleNode("collisionConfig");
            if (collisionNode != null)
            {
                bone.HasCollisionConfig = true;
                bone.CollisionConfig.Unk00 = collisionNode.ReadInt16OrDefault("unk00");
            }
            else
            {
                bone.HasCollisionConfig = false;
            }
        }

        #endregion

        #endregion
    }
}

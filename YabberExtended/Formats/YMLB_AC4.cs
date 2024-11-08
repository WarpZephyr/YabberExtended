using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using YabberExtended.Extensions.Xml;

namespace YabberExtended
{
    static class YMLB_AC4
    {
        public static void Unpack(this MLB_AC4 mlb, string sourceFile)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;

            XmlWriter xw = XmlWriter.Create($"{sourceFile}.ac4.xml", xws);
            xw.WriteStartElement("mlb_ac4");
            xw.WriteElementString("type", mlb.Type.ToString());
            xw.WriteElementString("isAnimation", mlb.IsAnimation.ToString());
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
            MLB_AC4 mlb = new MLB_AC4();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);

            mlb.Type = xml.ReadEnum<MLB_AC4.ResourceType>("mlb_ac4/type");
            mlb.IsAnimation = xml.ReadBoolean("mlb_ac4/isAnimation");

            XmlNodeList? resourceNodes = xml.SelectNodes("mlb_ac4/resources/resource");
            if (resourceNodes != null)
            {
                foreach (XmlNode resourceNode in resourceNodes)
                {
                    MLB_AC4.IResource resource;
                    bool isDummy = resourceNode.ReadBooleanOrDefault("isDummy", false);
                    if (isDummy)
                    {
                        resource = new MLB_AC4.Dummy();
                    }
                    else if (mlb.Type == MLB_AC4.ResourceType.Model && !mlb.IsAnimation)
                    {
                        resource = new MLB_AC4.Model();
                        resource.Path = resourceNode.ReadString("path");
                        resource.RelativePath = resourceNode.ReadString("relativePath");
                        RepackModel(mlb, (MLB_AC4.Model)resource, resourceNode);
                    }
                    else if (mlb.Type == MLB_AC4.ResourceType.Model && mlb.IsAnimation)
                    {
                        resource = new MLB_AC4.Animation();
                        resource.Path = resourceNode.ReadString("path");
                        resource.RelativePath = resourceNode.ReadString("relativePath");
                        RepackAnimation(mlb, (MLB_AC4.Animation)resource, resourceNode);
                    }
                    else if (mlb.Type == MLB_AC4.ResourceType.Texture)
                    {
                        resource = new MLB_AC4.Texture();
                        resource.Path = resourceNode.ReadString("path");
                        resource.RelativePath = resourceNode.ReadString("relativePath");
                        RepackTexture(mlb, (MLB_AC4.Texture)resource, resourceNode);
                    }
                    else
                    {
                        throw new FriendlyException($"Could not determine resource type to pack into MLB.");
                    }

                    mlb.Resources.Add(resource);
                }
            }

            string outPath = sourceFile.Replace(".mlb.ac4.xml", ".mlb");
            YabberUtil.BackupFile(outPath);
            mlb.Write(outPath);
        }

        #region Unpack

        #region Resource

        private static void UnpackResource(XmlWriter xw, MLB_AC4 mlb, MLB_AC4.IResource resource, int index)
        {
            xw.WriteStartElement("resource");
            if (resource is MLB_AC4.Dummy)
            {
                xw.WriteElementString("isDummy", true.ToString());
                xw.WriteEndElement();
                return;
            }

            xw.WriteElementString("path", resource.Path);
            xw.WriteElementString("relativePath", resource.RelativePath);
            if (mlb.Type == MLB_AC4.ResourceType.Model)
            {
                if (!mlb.IsAnimation)
                {
                    if (resource is MLB_AC4.Model model)
                    {
                        UnpackModel(xw, model);
                    }
                    else
                    {
                        throw new NotSupportedException($"Specified {nameof(mlb.IsAnimation)} {mlb.IsAnimation} on {nameof(MLB_AC4.ResourceType)} {mlb.Type} but {nameof(mlb.Resources)}[{index}] was not a {nameof(MLB_AC4.Model)}.");
                    }
                }
                else
                {
                    if (resource is MLB_AC4.Animation animation)
                    {
                        UnpackAnimation(xw, animation);
                    }
                    else
                    {
                        throw new NotSupportedException($"Specified {nameof(mlb.IsAnimation)} {mlb.IsAnimation} on {nameof(MLB_AC4.ResourceType)} {mlb.Type} but {nameof(mlb.Resources)}[{index}] was not a {nameof(MLB_AC4.Animation)}.");
                    }
                }
            }
            else if (mlb.Type == MLB_AC4.ResourceType.Texture)
            {
                if (resource is MLB_AC4.Texture texture)
                {
                    UnpackTexture(xw, texture);
                }
                else
                {
                    throw new NotSupportedException($"Specified {nameof(MLB_AC4.ResourceType)} {mlb.Type} but {nameof(mlb.Resources)}[{index}] was not a {nameof(MLB_AC4.Texture)}.");
                }
            }
            xw.WriteEndElement();
        }

        #endregion

        #region Model

        private static void UnpackModel(XmlWriter xw, MLB_AC4.Model model)
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
                    UnpackModelBone(xw, bone, model.OrderIndices[i]);
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
                UnpackModelBreakConfig(xw, model.BreakConfig);
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

        private static void UnpackMaterial(XmlWriter xw, MLB_AC4.Model.Material material)
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
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void UnpackModelBone(XmlWriter xw, MLB_AC4.Model.Bone bone, int orderIndex)
        {
            xw.WriteStartElement("bone");
            xw.WriteElementString("order", orderIndex.ToString());
            if (bone.IsDummy)
            {
                xw.WriteElementString("isDummy", true.ToString());
                xw.WriteEndElement();
                return;
            }

            xw.WriteElementString("name", bone.Name);
            if (bone.HasConfig1)
            {
                xw.WriteStartElement("config1");
                xw.WriteElementString("unk00", bone.Config1.Unk00.ToString());
                xw.WriteEndElement();
            }

            if (bone.HasBreakConfig)
            {
                UnpackBoneBreakConfig(xw, bone.BreakConfig);
            }

            if (bone.HasCollisionConfig)
            {
                xw.WriteStartElement("collisionConfig");
                xw.WriteElementString("unk00", bone.CollisionConfig.Unk00.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void UnpackBoneBreakConfig(XmlWriter xw, MLB_AC4.Model.Bone.BoneBreakConfig config)
        {
            xw.WriteStartElement("breakConfig");
            xw.WriteElementString("unk00", config.Unk00.ToString());
            xw.WriteElementString("unk01", config.Unk01.ToString());
            xw.WriteElementString("unk02", config.Unk02.ToString());
            if (config.HasConfig1)
            {
                xw.WriteStartElement("config1");
                xw.WriteElementString("unk00", config.Config1.Unk00.ToString());
                xw.WriteElementString("unk02", config.Config1.Unk02.ToString());
                xw.WriteElementString("unk04", config.Config1.Unk04.ToString());
                xw.WriteEndElement();
            }

            if (config.HasSoundEvent)
            {
                xw.WriteStartElement("soundEventConfig");
                if (config.SoundEvent.HasSoundEventName)
                {
                    xw.WriteElementString("soundEventName", config.SoundEvent.SoundEventName);
                }
                xw.WriteEndElement();
            }

            if (config.HasConfig3)
            {
                xw.WriteStartElement("config3");
                xw.WriteElementString("unk00", config.Config3.Unk00.ToString());
                xw.WriteElementString("unk04", config.Config3.Unk04.ToString());
                xw.WriteElementString("unk08", config.Config3.Unk08.ToString());
                xw.WriteElementString("unk0C", config.Config3.Unk0C.ToString());
                xw.WriteElementString("unk10", config.Config3.Unk10.ToString());
                xw.WriteElementString("unk14", config.Config3.Unk14.ToString());
                xw.WriteElementString("unk18", config.Config3.Unk18.ToString());
                xw.WriteElementString("unk19", config.Config3.Unk19.ToString());
                xw.WriteElementString("unk1A", config.Config3.Unk1A.ToString());
                xw.WriteElementString("unk1C", config.Config3.Unk1C.ToString());
                xw.WriteEndElement();
            }

            if (config.HasConfig4)
            {
                xw.WriteStartElement("config4");
                xw.WriteElementString("unk00", config.Config4.Unk00.ToString());
                xw.WriteElementString("unk04", config.Config4.Unk04.ToString());
                xw.WriteElementString("unk08", config.Config4.Unk08.ToString());
                xw.WriteElementString("unk0C", config.Config4.Unk0C.ToString());
                xw.WriteElementString("unk10", config.Config4.Unk10.ToString());
                xw.WriteEndElement();
            }

            if (config.HasConfig5)
            {
                xw.WriteStartElement("config5");
                xw.WriteElementString("unk00", config.Config5.Unk00.ToString());
                xw.WriteElementString("unk00", config.Config5.Unk02.ToString());
                xw.WriteElementString("unk00", config.Config5.Unk04.ToString());
                xw.WriteEndElement();
            }

            if (config.HasConfig6)
            {
                xw.WriteStartElement("config6");
                xw.WriteElementString("unk00", config.Config6.Unk00.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        private static void UnpackModelBreakConfig(XmlWriter xw, MLB_AC4.Model.ModelBreakConfig config)
        {
            xw.WriteStartElement("breakConfig");
            xw.WriteElementString("unk00", config.Unk00.ToString());
            xw.WriteElementString("unk01", config.Unk01.ToString());
            xw.WriteElementString("unk02", config.Unk02.ToString());
            if (config.HasConfig1)
            {
                xw.WriteStartElement("config1");
                xw.WriteElementString("unk00", config.Config1.Unk00.ToString());
                xw.WriteElementString("unk02", config.Config1.Unk02.ToString());
                xw.WriteElementString("unk04", config.Config1.Unk04.ToString());
                xw.WriteEndElement();
            }

            if (config.HasSoundEvent)
            {
                xw.WriteStartElement("soundEventConfig");
                if (config.SoundEvent.HasSoundEventName)
                {
                    xw.WriteElementString("soundEventName", config.SoundEvent.SoundEventName);
                }
                xw.WriteEndElement();
            }

            if (config.HasConfig3)
            {
                xw.WriteStartElement("config3");
                xw.WriteElementString("unk00", config.Config3.Unk00.ToString());
                xw.WriteElementString("unk04", config.Config3.Unk04.ToString());
                xw.WriteElementString("unk08", config.Config3.Unk08.ToString());
                xw.WriteElementString("unk0C", config.Config3.Unk0C.ToString());
                xw.WriteElementString("unk10", config.Config3.Unk10.ToString());
                xw.WriteElementString("unk14", config.Config3.Unk14.ToString());
                xw.WriteElementString("unk18", config.Config3.Unk18.ToString());
                xw.WriteElementString("unk19", config.Config3.Unk19.ToString());
                xw.WriteElementString("unk1A", config.Config3.Unk1A.ToString());
                xw.WriteElementString("unk1C", config.Config3.Unk1C.ToString());
                xw.WriteEndElement();
            }

            if (config.HasConfig4)
            {
                xw.WriteStartElement("config4");
                xw.WriteElementString("unk00", config.Config4.Unk00.ToString());
                xw.WriteElementString("unk04", config.Config4.Unk04.ToString());
                xw.WriteElementString("unk08", config.Config4.Unk08.ToString());
                xw.WriteElementString("unk0C", config.Config4.Unk0C.ToString());
                xw.WriteElementString("unk10", config.Config4.Unk10.ToString());
                xw.WriteEndElement();
            }

            if (config.HasConfig6)
            {
                xw.WriteStartElement("config6");
                xw.WriteElementString("unk00", config.Config6.Unk00.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
        }

        #endregion

        #region Animation

        private static void UnpackAnimation(XmlWriter xw, MLB_AC4.Animation animation)
        {
            if (animation.Bones.Count > 0)
            {
                xw.WriteStartElement("bones");
                for (int i = 0; i < animation.Bones.Count; i++)
                {
                    var bone = animation.Bones[i];
                    UnpackAnimationBone(xw, bone, animation.OrderIndices[i]);
                }
                xw.WriteEndElement();
            }

            xw.WriteStartElement("config1");
            xw.WriteEndElement();
        }

        private static void UnpackAnimationBone(XmlWriter xw, MLB_AC4.Animation.Bone bone, int orderIndex)
        {
            xw.WriteStartElement("bone");
            xw.WriteElementString("order", orderIndex.ToString());
            xw.WriteElementString("name", bone.Name);
            xw.WriteStartElement("config1");
            xw.WriteElementString("unk00", bone.Config1.Unk00.ToString());
            xw.WriteElementString("unk04", bone.Config1.Unk04.ToString());
            xw.WriteElementString("unk08", bone.Config1.Unk08.ToString());
            xw.WriteElementString("unk0C", bone.Config1.Unk0C.ToString());
            xw.WriteElementString("unk10", bone.Config1.Unk10.ToString());
            xw.WriteElementString("unk14", bone.Config1.Unk14.ToString());
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        #endregion

        #region Texture

        private static void UnpackTexture(XmlWriter xw, MLB_AC4.Texture texture)
        {
            xw.WriteElementString("fileArg", texture.FileArgument);
            xw.WriteElementString("typeArg", texture.TypeArgument);
            xw.WriteElementString("numMipmapsArg", texture.NumMipmapsArgument);
        }

        #endregion

        #endregion

        #region Repack

        #region Model

        private static void RepackModel(MLB_AC4 mlb, MLB_AC4.Model resource, XmlNode node)
        {
            XmlNodeList? materialNodes = node.SelectNodes("materials/material");
            if (materialNodes != null)
            {
                foreach (XmlNode materialNode in materialNodes)
                {
                    var material = new MLB_AC4.Model.Material();
                    RepackMaterial(material, materialNode);
                    resource.Materials.Add(material);
                }
            }

            XmlNodeList? boneNodes = node.SelectNodes("bones/bone");
            if (boneNodes != null)
            {
                foreach (XmlNode boneNode in boneNodes)
                {
                    var bone = new MLB_AC4.Model.Bone();
                    int orderIndex = RepackModelBone(bone, boneNode);
                    resource.Bones.Add(bone);
                    resource.OrderIndices.Add(orderIndex);
                }
            }

            if (resource.OrderIndices.Count > 0)
            {
                // Ensure order indices are usable
                var orderCheck = new HashSet<int>();
                bool allDifferent = resource.OrderIndices.All(orderCheck.Add);
                if (!allDifferent)
                {
                    throw new FriendlyException($"{nameof(resource.OrderIndices)} is invalid, make sure order indexes are unique.");
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
                RepackModelBreakConfig(resource.BreakConfig, breakNode);
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

        private static void RepackMaterial(MLB_AC4.Model.Material material, XmlNode node)
        {
            material.Name = node.ReadString("name");
            material.Config1 = new MLB_AC4.Model.Material.MaterialConfig1();
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
            }
            else
            {
                material.HasConfig1 = false;
            }
        }

        private static int RepackModelBone(MLB_AC4.Model.Bone bone, XmlNode node)
        {
            int orderIndex = node.ReadInt32("order");
            bool isDummy = node.ReadBooleanOrDefault("isDummy", false);
            if (isDummy)
            {
                bone.IsDummy = true;
                return orderIndex;
            }

            bone.Name = node.ReadString("name");
            XmlNode? nodeConfig1 = node.SelectSingleNode("config1");
            if (nodeConfig1 != null)
            {
                bone.Config1.Unk00 = nodeConfig1.ReadByteOrDefault("unk00");
                bone.HasConfig1 = true;
            }
            else
            {
                bone.HasConfig1 = false;
            }

            XmlNode? breakConfigNode = node.SelectSingleNode("breakConfig");
            if (breakConfigNode != null)
            {
                bone.HasBreakConfig = true;
                RepackBoneBreakConfig(bone.BreakConfig, breakConfigNode);
            }
            else
            {
                bone.HasBreakConfig = false;
            }

            XmlNode? collisionConfigNode = node.SelectSingleNode("collisionConfig");
            if (collisionConfigNode != null)
            {
                bone.HasCollisionConfig = true;
                bone.CollisionConfig.Unk00 = collisionConfigNode.ReadInt16OrDefault("unk00");
            }
            else
            {
                bone.HasCollisionConfig = false;
            }

            return orderIndex;
        }

        private static void RepackBoneBreakConfig(MLB_AC4.Model.Bone.BoneBreakConfig config, XmlNode node)
        {
            config.Unk00 = node.ReadByteOrDefault("unk00");
            config.Unk01 = node.ReadByteOrDefault("unk01");
            config.Unk02 = node.ReadByteOrDefault("unk02");

            XmlNode? nodeConfig1 = node.SelectSingleNode("config1");
            XmlNode? soundEventConfigNode = node.SelectSingleNode("soundEventConfig");
            XmlNode? nodeConfig3 = node.SelectSingleNode("config3");
            XmlNode? nodeConfig4 = node.SelectSingleNode("config4");
            XmlNode? nodeConfig5 = node.SelectSingleNode("config5");
            XmlNode? nodeConfig6 = node.SelectSingleNode("config6");
            if (nodeConfig1 != null)
            {
                config.HasConfig1 = true;
                config.Config1.Unk00 = nodeConfig1.ReadInt16OrDefault("unk00");
                config.Config1.Unk02 = nodeConfig1.ReadInt16OrDefault("unk02");
                config.Config1.Unk04 = nodeConfig1.ReadInt16OrDefault("unk04");
            }

            if (soundEventConfigNode != null)
            {
                config.HasSoundEvent = true;
                string? soundEventName = soundEventConfigNode.ReadStringIfExists("soundEventName");
                if (soundEventName != null)
                {
                    config.SoundEvent.HasSoundEventName = true;
                    config.SoundEvent.SoundEventName = soundEventName;
                }
                else
                {
                    config.SoundEvent.HasSoundEventName = false;
                }
            }

            if (nodeConfig3 != null)
            {
                config.HasConfig3 = true;
                config.Config3.Unk00 = nodeConfig3.ReadSingleOrDefault("unk00");
                config.Config3.Unk04 = nodeConfig3.ReadSingleOrDefault("unk04");
                config.Config3.Unk08 = nodeConfig3.ReadSingleOrDefault("unk08");
                config.Config3.Unk0C = nodeConfig3.ReadSingleOrDefault("unk0C");
                config.Config3.Unk10 = nodeConfig3.ReadSingleOrDefault("unk10");
                config.Config3.Unk14 = nodeConfig3.ReadSingleOrDefault("unk14");
                config.Config3.Unk18 = nodeConfig3.ReadByteOrDefault("unk18");
                config.Config3.Unk19 = nodeConfig3.ReadByteOrDefault("unk19");
                config.Config3.Unk1A = nodeConfig3.ReadInt16OrDefault("unk1A");
                config.Config3.Unk1C = nodeConfig3.ReadInt16OrDefault("unk1C");
            }

            if (nodeConfig4 != null)
            {
                config.HasConfig4 = true;
                config.Config4.Unk00 = nodeConfig4.ReadSingleOrDefault("unk00");
                config.Config4.Unk04 = nodeConfig4.ReadSingleOrDefault("unk04");
                config.Config4.Unk08 = nodeConfig4.ReadSingleOrDefault("unk08");
                config.Config4.Unk0C = nodeConfig4.ReadSingleOrDefault("unk0C");
                config.Config4.Unk10 = nodeConfig4.ReadByteOrDefault("unk10");
            }

            if (nodeConfig5 != null)
            {
                config.HasConfig5 = true;
                config.Config5.Unk00 = nodeConfig5.ReadInt16OrDefault("unk00");
                config.Config5.Unk02 = nodeConfig5.ReadInt16OrDefault("unk02");
                config.Config5.Unk04 = nodeConfig5.ReadInt16OrDefault("unk04");
            }

            if (nodeConfig6 != null)
            {
                config.HasConfig6 = true;
                config.Config6.Unk00 = nodeConfig6.ReadInt32OrDefault("unk00");
            }
        }

        private static void RepackModelBreakConfig(MLB_AC4.Model.ModelBreakConfig config, XmlNode node)
        {
            config.Unk00 = node.ReadByteOrDefault("unk00");
            config.Unk01 = node.ReadByteOrDefault("unk01");
            config.Unk02 = node.ReadByteOrDefault("unk02");

            XmlNode? nodeConfig1 = node.SelectSingleNode("config1");
            XmlNode? soundEventConfigNode = node.SelectSingleNode("soundEventConfig");
            XmlNode? nodeConfig3 = node.SelectSingleNode("config3");
            XmlNode? nodeConfig4 = node.SelectSingleNode("config4");
            XmlNode? nodeConfig6 = node.SelectSingleNode("config6");
            if (nodeConfig1 != null)
            {
                config.HasConfig1 = true;
                config.Config1.Unk00 = nodeConfig1.ReadInt16OrDefault("unk00");
                config.Config1.Unk02 = nodeConfig1.ReadInt16OrDefault("unk02");
                config.Config1.Unk04 = nodeConfig1.ReadInt16OrDefault("unk04");
            }

            if (soundEventConfigNode != null)
            {
                config.HasSoundEvent = true;
                string? soundEventName = soundEventConfigNode.ReadStringIfExists("soundEventName");
                if (soundEventName != null)
                {
                    config.SoundEvent.HasSoundEventName = true;
                    config.SoundEvent.SoundEventName = soundEventName;
                }
                else
                {
                    config.SoundEvent.HasSoundEventName = false;
                }
            }

            if (nodeConfig3 != null)
            {
                config.HasConfig3 = true;
                config.Config3.Unk00 = nodeConfig3.ReadSingleOrDefault("unk00");
                config.Config3.Unk04 = nodeConfig3.ReadSingleOrDefault("unk04");
                config.Config3.Unk08 = nodeConfig3.ReadSingleOrDefault("unk08");
                config.Config3.Unk0C = nodeConfig3.ReadSingleOrDefault("unk0C");
                config.Config3.Unk10 = nodeConfig3.ReadSingleOrDefault("unk10");
                config.Config3.Unk14 = nodeConfig3.ReadSingleOrDefault("unk14");
                config.Config3.Unk18 = nodeConfig3.ReadByteOrDefault("unk18");
                config.Config3.Unk19 = nodeConfig3.ReadByteOrDefault("unk19");
                config.Config3.Unk1A = nodeConfig3.ReadInt16OrDefault("unk1A");
                config.Config3.Unk1C = nodeConfig3.ReadInt16OrDefault("unk1C");
            }

            if (nodeConfig4 != null)
            {
                config.HasConfig4 = true;
                config.Config4.Unk00 = nodeConfig4.ReadSingleOrDefault("unk00");
                config.Config4.Unk04 = nodeConfig4.ReadSingleOrDefault("unk04");
                config.Config4.Unk08 = nodeConfig4.ReadSingleOrDefault("unk08");
                config.Config4.Unk0C = nodeConfig4.ReadSingleOrDefault("unk0C");
                config.Config4.Unk10 = nodeConfig4.ReadByteOrDefault("unk10");
            }

            if (nodeConfig6 != null)
            {
                config.HasConfig6 = true;
                config.Config6.Unk00 = nodeConfig6.ReadInt32OrDefault("unk00");
            }
        }

        #endregion

        #region Animation

        private static void RepackAnimation(MLB_AC4 mlb, MLB_AC4.Animation resource, XmlNode node)
        {
            XmlNodeList? boneNodes = node.SelectNodes("bones/bone");
            if (boneNodes != null)
            {
                foreach (XmlNode boneNode in boneNodes)
                {
                    var bone = new MLB_AC4.Animation.Bone();
                    int orderIndex = RepackAnimationBone(bone, boneNode);
                    resource.Bones.Add(bone);
                    resource.OrderIndices.Add(orderIndex);
                }
            }

            if (resource.OrderIndices.Count > 0)
            {
                // Ensure order indices are usable
                var orderCheck = new HashSet<int>();
                bool allDifferent = resource.OrderIndices.All(orderCheck.Add);
                if (!allDifferent)
                {
                    throw new FriendlyException($"{nameof(resource.OrderIndices)} is invalid, make sure order indexes are unique.");
                }
            }
        }

        private static int RepackAnimationBone(MLB_AC4.Animation.Bone bone, XmlNode node)
        {
            int orderIndex = node.ReadInt32("order");
            bone.Name = node.ReadString("name");
            bone.Config1.Unk00 = node.ReadSingleOrDefault("config1/unk00");
            bone.Config1.Unk04 = node.ReadSingleOrDefault("config1/unk04");
            bone.Config1.Unk08 = node.ReadSingleOrDefault("config1/unk08");
            bone.Config1.Unk0C = node.ReadSingleOrDefault("config1/unk0C");
            bone.Config1.Unk10 = node.ReadSingleOrDefault("config1/unk10");
            bone.Config1.Unk14 = node.ReadSingleOrDefault("config1/unk14");
            return orderIndex;
        }

        #endregion

        #region Texture

        private static void RepackTexture(MLB_AC4 mlb, MLB_AC4.Texture resource, XmlNode node)
        {
            resource.FileArgument = node.ReadStringOrDefault("fileArg", "-file");
            resource.TypeArgument = node.ReadStringOrDefault("typeArg", "[32bit]");
            resource.NumMipmapsArgument = node.ReadStringOrDefault("numMipmapsArg", "-nmips 0");
        }

        #endregion

        #endregion
    }
}

using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using YabberExtended.Extensions.Value;
using YabberExtended.Extensions.Xml;
using YabberExtended.Helpers;
using YabberExtended.Parse;

namespace YabberExtended
{
    static class YAcParts4
    {
        public static void Unpack(this AcParts4 acparts, string sourceName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            string[] names =
            [
                "head.txt",
                "core.txt",
                "arms.txt",
                "legs.txt",
                "fcs.txt",
                "generator.txt",
                "booster.txt",
                "backbooster.txt",
                "sidebooster.txt",
                "overedbooster.txt",
                "arm unit.txt",
                "back unit.txt",
                "shoulder.txt",
                "subheadtop.txt",
                "subheadside.txt",
                "subcoreupper.txt",
                "subcorelower.txt",
                "subarms.txt",
                "sublegsback.txt",
                "sublegsupper.txt",
                "sublegsmiddle.txt",
                "sublegslower.txt"
            ];

            bool[] results = new bool[names.Length];

            int nameIndex = 0;
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.Heads, acparts.Version, UnpackHead);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.Cores, acparts.Version, UnpackCore);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.Arms, acparts.Version, UnpackArm);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.Legs, acparts.Version, UnpackLeg);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.FCSs, acparts.Version, UnpackFCS);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.Generators, acparts.Version, UnpackGenerator);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.MainBoosters, acparts.Version, UnpackMainBooster);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.BackBoosters, acparts.Version, UnpackBackBooster);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.SideBoosters, acparts.Version, UnpackSideBooster);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.OveredBoosters, acparts.Version, UnpackOveredBooster);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.ArmUnits, acparts.Version, UnpackArmUnit);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.BackUnits, acparts.Version, UnpackBackUnit);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.ShoulderUnits, acparts.Version, UnpackShoulderUnit);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.HeadTopStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.HeadSideStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.CoreUpperSideStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.CoreLowerSideStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.ArmStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegBackStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegUpperStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegMiddleStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegLowerStabilizers, acparts.Version, UnpackStabilizer);

            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-acparts4.xml"), xws);
            xw.WriteStartElement("acparts4");
            xw.WriteElementString("acparts_name", sourceName);
            xw.WriteElementString("acparts_version", acparts.Version.ToString());
            xw.WriteStartElement("partlists");
            for (int i = 0; i < names.Length; i++)
            {
                if (results[i])
                {
                    xw.WriteStartElement("partlist");
                    xw.WriteElementString("category", ((AcParts4.PartComponent.PartCategory)i).ToString());
                    xw.WriteElementString("filename", names[i]);
                    xw.WriteEndElement();
                }
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            AcParts4 acparts = new AcParts4();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-acparts4.xml"));
            acparts.Version = xml.ReadEnumOrDefault("acparts4/acparts_version", AcParts4.AcParts4Version.AC4);
            string outPath = Path.Combine(targetDir, xml.ReadString("acparts4/acparts_name"));

            XmlNodeList? partlistNodes = xml.SelectNodes("acparts4/partlists/partlist");
            if (partlistNodes != null)
            {
                foreach (XmlNode partlistNode in partlistNodes)
                {
                    string category = partlistNode.ReadString("category");
                    string partsFileName = partlistNode.ReadString("filename");
                    string partsFilePath = Path.Combine(sourceDir, partsFileName);
                    switch (category)
                    {
                        case "Head":
                            RepackParts(partsFilePath, acparts.Heads, acparts.Version, RepackHead);
                            break;
                        case "Core":
                            RepackParts(partsFilePath, acparts.Cores, acparts.Version, RepackCore);
                            break;
                        case "Arms":
                            RepackParts(partsFilePath, acparts.Arms, acparts.Version, RepackArm);
                            break;
                        case "Legs":
                            RepackParts(partsFilePath, acparts.Legs, acparts.Version, RepackLeg);
                            break;
                        case "FCS":
                            RepackParts(partsFilePath, acparts.FCSs, acparts.Version, RepackFCS);
                            break;
                        case "Generator":
                            RepackParts(partsFilePath, acparts.Generators, acparts.Version, RepackGenerator);
                            break;
                        case "MainBooster":
                            RepackParts(partsFilePath, acparts.MainBoosters, acparts.Version, RepackMainBooster);
                            break;
                        case "BackBooster":
                            RepackParts(partsFilePath, acparts.BackBoosters, acparts.Version, RepackBackBooster);
                            break;
                        case "SideBooster":
                            RepackParts(partsFilePath, acparts.SideBoosters, acparts.Version, RepackSideBooster);
                            break;
                        case "OveredBooster":
                            RepackParts(partsFilePath, acparts.OveredBoosters, acparts.Version, RepackOveredBooster);
                            break;
                        case "ArmUnit":
                            RepackParts(partsFilePath, acparts.ArmUnits, acparts.Version, RepackArmUnit);
                            break;
                        case "BackUnit":
                            RepackParts(partsFilePath, acparts.BackUnits, acparts.Version, RepackBackUnit);
                            break;
                        case "ShoulderUnit":
                            RepackParts(partsFilePath, acparts.ShoulderUnits, acparts.Version, RepackShoulderUnit);
                            break;
                        case "HeadTopStabilizer":
                            RepackParts(partsFilePath, acparts.HeadTopStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "HeadSideStabilizer":
                            RepackParts(partsFilePath, acparts.HeadSideStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "CoreUpperSideStabilizer":
                            RepackParts(partsFilePath, acparts.CoreUpperSideStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "CoreLowerSideStabilizer":
                            RepackParts(partsFilePath, acparts.CoreLowerSideStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "ArmStabilizer":
                            RepackParts(partsFilePath, acparts.ArmStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "LegBackStabilizer":
                            RepackParts(partsFilePath, acparts.LegBackStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "LegUpperStabilizer":
                            RepackParts(partsFilePath, acparts.LegUpperStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "LegMiddleStabilizer":
                            RepackParts(partsFilePath, acparts.LegMiddleStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        case "LegLowerStabilizer":
                            RepackParts(partsFilePath, acparts.LegLowerStabilizers, acparts.Version, RepackStabilizer);
                            break;
                        default:
                            throw new FriendlyException($"Unknown part category: {category}");
                    }
                }
            }

            YabberUtil.BackupFile(outPath);
            acparts.Write(outPath);
        }

        private static bool UnpackParts<TPart>(string targetDir, string fileName, IList<TPart> parts, AcParts4.AcParts4Version version, Action<StreamWriter, TPart, AcParts4.AcParts4Version> unpackPart)
        {
            if (parts.Count > 0)
            {
                using var fs = File.OpenWrite(Path.Combine(targetDir, fileName));
                using var sw = new StreamWriter(fs, EncodingHelper.ShiftJIS);
                for (int i = 0; i < parts.Count; i++)
                {
                    var part = parts[i];
                    sw.WriteLine($"[{i}]");
                    unpackPart(sw, part, version);
                }
                sw.WriteLine(); // I don't know why either

                return true;
            }

            return false;
        }

        private static void RepackParts<TPart>(string filePath, IList<TPart> parts, AcParts4.AcParts4Version version, Action<ValueDictionary, TPart, AcParts4.AcParts4Version> repackPart) where TPart : class, new()
        {
            var sr = new StreamReader(filePath, EncodingHelper.ShiftJIS);

            TPart? part = null;
            FieldParser fp = new FieldParser("=");
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                int length = line.Length;
                if (length > 1 && line[0] == '[' && line[length - 1] == ']')
                {
                    if (part != null)
                    {
                        repackPart(fp.FieldDictionary, part, version);
                        parts.Add(part);
                        fp.FieldDictionary = new ValueDictionary();
                    }

                    part = new TPart();
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    fp.ParseField(line);
                }
            }

            if (part != null)
            {
                repackPart(fp.FieldDictionary, part, version);
                parts.Add(part);
            }
        }

        #region Unpack

        #region Component

        private static void UnpackPartComponent(StreamWriter sw, AcParts4.PartComponent component, AcParts4.AcParts4Version version)
        {
            sw.WriteLine($"ID = {component.PartID}");
            sw.WriteLine($"ModelID = {(component.PartID == component.ModelID ? string.Empty : component.ModelID.ToString())}"); // Simulate found file behavior where an empty string means part and model ID are the same.
            sw.WriteLine($"Price = {component.Price}");
            sw.WriteLine($"Weight = {component.Weight}");
            sw.WriteLine($"ConstantEPWaste = {component.ENCost}");
            sw.WriteLine($"InitStatus = {component.InitStatus}");
            sw.WriteLine($"CapID = {component.CapID}");
            sw.WriteLine($"PartsName = {component.Name}");
            sw.WriteLine($"MakerName = {component.MakerName}");
            sw.WriteLine($"SubCategory = {component.SubCategory}");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"SubCategoryID = {component.SubCategoryID}");
                sw.WriteLine($"SetID = {component.SetID}");
            }
            sw.WriteLine($"Explain = {component.Explain.Replace("\r\n", "\\r\\n").Replace("\n", "\\n")}");
        }

        private static void UnpackWeaponComponent(StreamWriter sw, AcParts4.WeaponComponent component)
        {
            sw.WriteLine($"prog = {component.WeaponFiringMode}");
            sw.WriteLine($"lock_enable = {(component.CanLockOn ? "ON" : "OFF")}");
            sw.WriteLine($"missile_locktime = {component.MissileLockTime}");
            sw.WriteLine($"WeaponUnk03 = {(component.Unk03 ? 1 : 0)}");
            sw.WriteLine($"EffectiveRange = {component.FiringRange}");
            sw.WriteLine($"WeightBalance = {component.MeleeAbility}");
            sw.WriteLine($"bulletID = {component.BulletID}");
            sw.WriteLine($"sfxID = {(component.SFXID == 1 ? string.Empty : component.SFXID.ToString())}"); // Simulate found file behavior where an empty string means sfxID is 1.
            sw.WriteLine($"hitEffectID = {(component.HitEffectID == 0 ? string.Empty : component.HitEffectID.ToString())}"); // Simulate found file behavior where an empty string means hitEffectID is 0.
            sw.WriteLine($"initspeed = {component.BallisticsVelocity}");
            sw.WriteLine($"ep_drain = {component.ENCost}");
            sw.WriteLine($"multi_proc = {(component.MultiProc ? 1 : 0)}");
            sw.WriteLine($"once_cnt = {component.ProjectileCount}");
            sw.WriteLine($"auto_cnt = {component.ContinuousFireCount}");
            sw.WriteLine($"WeaponUnk1F = {component.Unk1F}");
            sw.WriteLine($"WeaponUnk20 = {component.Unk20}");
            sw.WriteLine($"auto_interval = {component.AutoInterval}");
            sw.WriteLine($"reload_time = {component.FireRate}");
            sw.WriteLine($"recoil = {component.Recoil}");
            sw.WriteLine($"cost = {component.CostPerRound}");
            sw.WriteLine($"ShootPrecision = {component.ShotPrecision}");
            sw.WriteLine($"mag_cartridge_num = {component.NumberofMagazines}");
            sw.WriteLine($"mag_bullet_cnt = {component.MagazineCapacity}");
            sw.WriteLine($"mag_reload_time = {component.MagazineReloadTime}");
            sw.WriteLine($"WeaponType = {component.Weapon_Type}");
            sw.WriteLine($"koji_DrainPerSec = {component.ChargeTime}");
            sw.WriteLine($"koji_RefillFrame = {component.KPChargeCost}");
            sw.WriteLine($"koji_MaxDamageRate = {component.KojimaMaxDamageRate}");
            sw.WriteLine($"BladeReload = {component.AttackLatency}");
            sw.WriteLine($"WeaponUnk3E = {component.Unk3E}");
            sw.WriteLine($"Damage_Type = {component.Damage_Type}");
            sw.WriteLine($"Damage_Pierce = {(component.DamagePierce ? 1 : 0)}");
            sw.WriteLine($"WeaponUnk41 = {(component.Unk41 ? 1 : 0)}");
            sw.WriteLine($"Damage_Radial = {(component.DamageRadial ? 1 : 0)}");
            sw.WriteLine($"Damage_Power = {component.AttackPower}");
            sw.WriteLine($"Damage_Recoil = {component.ImpactForce}");
            sw.WriteLine($"Damage_Diffsibility = {component.PAAttentuation}");
            sw.WriteLine($"Damage_Penetrability = {component.PAPenetration}");
        }

        private static void UnpackDefenseComponent(StreamWriter sw, AcParts4.DefenseComponent component)
        {
            sw.WriteLine($"ShellDef = {component.BallisticDefense}");
            sw.WriteLine($"EnergyDef = {component.EnergyDefense}");
        }

        private static void UnpackPAComponent(StreamWriter sw, AcParts4.PAComponent component)
        {
            sw.WriteLine($"WaveStabilizability = {component.PARectification}");
            sw.WriteLine($"AntiDiffuse = {component.PADurability}");
        }

        private static void UnpackFrameComponent(StreamWriter sw, AcParts4.FrameComponent component)
        {
            sw.WriteLine($"TuneMax_WaveStabilizability = {component.TuneMaxRectification}");
            sw.WriteLine($"TuneEff_WaveStabilizability = {component.TuneEfficiencyRectification}");
            sw.WriteLine($"AP = {component.AP}");
            sw.WriteLine($"FrameUnk06 = {component.Unk06}");
            sw.WriteLine($"Cp = {component.DragCoefficient}");
            sw.WriteLine($"WeightBalanceFront = {component.WeightBalanceFront}");
            sw.WriteLine($"WeightBalanceBack = {component.WeightBalanceBack}");
            sw.WriteLine($"WeightBalanceRight = {component.WeightBalanceRight}");
            sw.WriteLine($"WeightBalanceLeft = {component.WeightBalanceLeft}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnpackBoosterComponent(StreamWriter sw, AcParts4.BoosterComponent component, string boostType)
            => UnpackBoosterComponent(sw, component, boostType, boostType, boostType);

        private static void UnpackBoosterComponent(StreamWriter sw, AcParts4.BoosterComponent component, string thrustName, string boostDurationName, string enCostName)
        {
            sw.WriteLine($"{thrustName}Power = {component.Thrust}");
            sw.WriteLine($"TuneMax_{thrustName}Power = {component.TuneMaxThrust}");
            sw.WriteLine($"TuneEff_{thrustName}Power = {component.TuneEfficiencyThrust}");
            sw.WriteLine($"{boostDurationName}AccelTime = {component.QuickBoostDuration}");
            sw.WriteLine($"{enCostName}EPWaste = {component.ThrustENCost}");
        }

        private static void UnpackRadarComponent(StreamWriter sw, AcParts4.RadarComponent component)
        {
            sw.WriteLine($"RadarRange = {component.RadarRange}");
            sw.WriteLine($"AntiECM = {component.ECMResistance}");
            sw.WriteLine($"TuneMax_AntiECM = {component.TuneMaxECMResistance}");
            sw.WriteLine($"TuneEff_AntiECM = {component.TuneEfficiencyECMResistance}");
            sw.WriteLine($"ScanInterval = {component.RadarRefreshRate}");
            sw.WriteLine($"TuneMax_ScanInterval = {component.TuneMaxRadarRefreshRate}");
            sw.WriteLine($"TuneEff_ScanInterval = {component.TuneEfficiencyRadarRefreshRate}");
        }

        private static void UnpackWeaponBoosterComponent(StreamWriter sw, AcParts4.WeaponBoosterComponent component)
        {
            sw.WriteLine($"HorizontalPower = {component.HorizontalThrust}");
            sw.WriteLine($"VerticalPower = {component.VerticalThrust}");
            sw.WriteLine($"QBPower = {component.QuickBoost}");
            sw.WriteLine($"WeaponBoosterUnk0C = {component.Unk0CThrust}");
            sw.WriteLine($"HorizontalEPWaste = {component.HorizontalENCost}");
            sw.WriteLine($"VerticalEPWaste = {component.VerticalENCost}");
            sw.WriteLine($"QBEPWaste = {component.QuickBoostENCost}");
            sw.WriteLine($"WeaponBoosterUnk1C = {component.Unk0CENCost}");
        }

        private static void UnpackStabilizerComponent(StreamWriter sw, AcParts4.StabilizerComponent component)
        {
            sw.WriteLine($"WeightRate = {component.ControlCalibration}");
        }

        #endregion

        #region Part

        private static void UnpackHead(StreamWriter sw, AcParts4.Head part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackDefenseComponent(sw, part.DefenseComponent);
            UnpackPAComponent(sw, part.PAComponent);
            UnpackFrameComponent(sw, part.FrameComponent);
            sw.WriteLine($"Balance = {part.Stability}");
            sw.WriteLine($"Sfx_Monoeye = {part.SFXMonoeye}");
            sw.WriteLine($"TuneMax_Balance = {part.TuneMaxStability}");
            sw.WriteLine($"TuneEff_Balance = {part.TuneEfficiencyStability}");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"CameraFunctionality = {part.CameraFunctionality}");
                sw.WriteLine($"SystemRecovery = {part.SystemRecovery}");
                sw.WriteLine($"HeadUnk28 = {part.Unk28}");
                sw.WriteLine($"HeadUnk2A = {part.Unk2A}");
            }

            sw.WriteLine($"StbzrTopX = {part.StabilizerTopX}");
            sw.WriteLine($"StbzrTopY = {part.StabilizerTopY}");
            sw.WriteLine($"StbzrSideX = {part.StabilizerSideX}");
            sw.WriteLine($"StbzrSideY = {part.StabilizerSideY}");
        }

        private static void UnpackCore(StreamWriter sw, AcParts4.Core part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackDefenseComponent(sw, part.DefenseComponent);
            UnpackPAComponent(sw, part.PAComponent);
            UnpackFrameComponent(sw, part.FrameComponent);
            if (version == AcParts4.AcParts4Version.AC4)
            {
                sw.WriteLine($"hunger unit = {part.HungerUnit}");
                sw.WriteLine($"CoreUnk20 = {part.Unk20}");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"CoreUnk1C = {part.Unk1C}");
                sw.WriteLine($"TuneMax_CoreUnk1C = {part.TuneMaxUnk1C}");
                sw.WriteLine($"TuneEff_CoreUnk1C = {part.TuneEfficiencyUnk1C}");
                sw.WriteLine($"Balance = {part.Stability}");
                sw.WriteLine($"TuneMax_Balance = {part.TuneMaxStability}");
                sw.WriteLine($"TuneEff_Balance = {part.TuneEfficiencyStability}");
            }

            sw.WriteLine($"StbzrUpX = {part.StabilizerUpX}");
            sw.WriteLine($"StbzrUpY = {part.StabilizerUpY}");
            sw.WriteLine($"StbzrLowX = {part.StabilizerLowX}");
            sw.WriteLine($"StbzrLowY = {part.StabilizerLowY}");
        }

        private static void UnpackArm(StreamWriter sw, AcParts4.Arm part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackDefenseComponent(sw, part.DefenseComponent);
            UnpackPAComponent(sw, part.PAComponent);
            UnpackFrameComponent(sw, part.FrameComponent);
            sw.WriteLine($"Type = {(part.IsWeaponArm ? "Weapon" : string.Empty)}");
            sw.WriteLine($"ArmUnk1D = {part.Unk1D}");
            sw.WriteLine($"RecoilCtrl = {part.FiringStability}");
            sw.WriteLine($"EnergySupply = {part.EnergyWeaponSkill}");
            sw.WriteLine($"Manipulation = {part.WeaponManeuverability}");
            sw.WriteLine($"Aiming = {part.AimPrecision}");
            sw.WriteLine($"ArmUnk26 = {part.Unk26}");
            sw.WriteLine($"ArmUnk27 = {part.Unk27}");
            sw.WriteLine($"ArmUnk28 = {part.Unk28}");
            sw.WriteLine($"ArmUnk2A = {part.Unk2A}");
            sw.WriteLine($"TuneMax_RecoilCtrl = {part.TuneMaxFiringStability}");
            sw.WriteLine($"TuneEff_RecoilCtrl = {part.TuneEfficiencyFiringStability}");
            sw.WriteLine($"TuneMax_EnergySupply = {part.TuneMaxEnergyWeaponSkill}");
            sw.WriteLine($"TuneEff_EnergySupply = {part.TuneEfficiencyEnergyWeaponSkill}");
            sw.WriteLine($"TuneMax_Manipulation = {part.TuneMaxWeaponManeuverability}");
            sw.WriteLine($"TuneEff_Manipulation = {part.TuneEfficiencyWeaponManeuverability}");
            sw.WriteLine($"TuneMax_Aiming = {part.TuneMaxAimPrecision}");
            sw.WriteLine($"TuneEff_Aiming = {part.TuneEfficiencyAimPrecision}");
            sw.WriteLine($"DispType = {(AcParts4.DispType)part.DisplayType}");
            sw.WriteLine($"ArmUnk3D = {part.Unk3D}");
            sw.WriteLine($"ArmUnk3E = {part.Unk3E}");
            sw.WriteLine($"ArmUnk3F = {part.Unk3F}");
            sw.WriteLine($"AimType = {part.AimType}");
            UnpackWeaponComponent(sw, part.WeaponComponent);
            sw.WriteLine($"StbzrX = {part.StabilizerX}");
            sw.WriteLine($"StbzrY = {part.StabilizerY}");
        }

        private static void UnpackLeg(StreamWriter sw, AcParts4.Leg part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackDefenseComponent(sw, part.DefenseComponent);
            UnpackPAComponent(sw, part.PAComponent);
            UnpackFrameComponent(sw, part.FrameComponent);
            string type = part.Type switch
            {
                AcParts4.Leg.LegType.Bipedal => "2Legs",
                AcParts4.Leg.LegType.ReverseJoint => "RevLegs",
                AcParts4.Leg.LegType.Quad => "4Legs",
                AcParts4.Leg.LegType.Tank => "Tank",
                _ => throw new FriendlyException($"Unknown Leg Type: {part.Type}"),
            };

            sw.WriteLine($"Type = {type}");
            sw.WriteLine($"Motion = {part.Motion}");
            sw.WriteLine($"LegUnk1E = {part.Unk1E}");
            sw.WriteLine($"MaxPayload = {part.MaxLoad}");
            sw.WriteLine($"AllowedPayload = {part.Load}");
            sw.WriteLine($"TuneMax_AllowedPayload = {part.TuneMaxLoad}");
            sw.WriteLine($"TuneEff_AllowedPayload = {part.TuneEfficiencyLoad}");
            sw.WriteLine($"CannonAng = {part.BackUnitAngle}");
            sw.WriteLine($"MovePerformance = {part.MovementAbility}");
            sw.WriteLine($"TurnPerformance = {part.TurningAbility}");
            sw.WriteLine($"BrakePerformance = {part.BrakingAbility}");
            sw.WriteLine($"JumpPerformance = {part.JumpingAbility}");
            sw.WriteLine($"LandStability = {part.LandingStability}");
            sw.WriteLine($"HitStability = {part.HitStability}");
            sw.WriteLine($"ShootStability = {part.ShootStability}");
            sw.WriteLine($"TuneMax_MovePerformance = {part.TuneMaxMovementAbility}");
            sw.WriteLine($"TuneMax_TurnPerformance = {part.TuneMaxTurningAbility}");
            sw.WriteLine($"TuneMax_BrakePerformance = {part.TuneMaxBrakingAbility}");
            sw.WriteLine($"TuneMax_JumpPerformance = {part.TuneMaxJumpingAbility}");
            sw.WriteLine($"TuneMax_LandStability = {part.TuneMaxLandingStability}");
            sw.WriteLine($"TuneMax_HitStability = {part.TuneMaxHitStability}");
            sw.WriteLine($"TuneMax_ShootStability = {part.TuneMaxShootStability}");
            sw.WriteLine($"TuneEff_MovePerformance = {part.TuneEfficiencyMovementAbility}");
            sw.WriteLine($"TuneEff_TurnPerformance = {part.TuneEfficiencyTurningAbility}");
            sw.WriteLine($"TuneEff_BrakePerformance = {part.TuneEfficiencyBrakingAbility}");
            sw.WriteLine($"TuneEff_JumpPerformance = {part.TuneEfficiencyJumpingAbility}");
            sw.WriteLine($"TuneEff_LandStability = {part.TuneEfficiencyLandingStability}");
            sw.WriteLine($"TuneEff_HitStability = {part.TuneEfficiencyHitStability}");
            sw.WriteLine($"TuneEff_ShootStability = {part.TuneEfficiencyShootStability}");
            sw.WriteLine($"LegUnk70 = {part.Unk70}");
            UnpackBoosterComponent(sw, part.HorizontalBoost, "Horizontal");
            UnpackBoosterComponent(sw, part.VerticalBoost, "Vertical");
            UnpackBoosterComponent(sw, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"LegUnkA4 = {part.UnkA4}");
                sw.WriteLine($"LegUnkA5 = {part.UnkA5}");
                sw.WriteLine($"LegUnkA6 = {part.UnkA6}");
            }

            sw.WriteLine($"StbzrBackX = {part.StabilizerBackX}");
            sw.WriteLine($"StbzrBackY = {part.StabilizerBackY}");
            sw.WriteLine($"StbzrUpX = {part.StabilizerUpX}");
            sw.WriteLine($"StbzrUpY = {part.StabilizerUpY}");
            sw.WriteLine($"StbzrMidX = {part.StabilizerMidX}");
            sw.WriteLine($"StbzrMidY = {part.StabilizerMidY}");
            sw.WriteLine($"StbzrLowX = {part.StabilizerLowX}");
            sw.WriteLine($"StbzrLowY = {part.StabilizerLowY}");
            sw.WriteLine($"StbzrUpRX = {part.StabilizerUpRightX}");
            sw.WriteLine($"StbzrUpRY = {part.StabilizerUpRightY}");
            sw.WriteLine($"StbzrMidRX = {part.StabilizerMidRightX}");
            sw.WriteLine($"StbzrMidRY = {part.StabilizerMidRightY}");
            sw.WriteLine($"StbzrLowRX = {part.StabilizerLowRightX}");
            sw.WriteLine($"StbzrLowRY = {part.StabilizerLowRightY}");
        }

        private static void UnpackFCS(StreamWriter sw, AcParts4.FCS part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            sw.WriteLine($"DeflectType = {part.Deflect}");
            sw.WriteLine($"lockmax = {part.LockTargetMax}");
            sw.WriteLine($"BladeLockRange = {part.BladeLockDistance}");
            sw.WriteLine($"Paraeffic = {part.ParallelProcessing}");
            sw.WriteLine($"Visibility = {part.Visibility}");
            sw.WriteLine($"LockRange = {part.LockDistance}, {part.LockBoxHeight}, {part.LockBoxWidth}, {part.UnkLockRange4}");
            sw.WriteLine($"second_locktime = {part.SecondLockTime}");
            sw.WriteLine($"missile_lockspd = {part.MissileLockSpeed}");
            sw.WriteLine($"multilock = {(part.MultiLock ? 1 : 0)}");
            sw.WriteLine($"FCSUnk15 = {part.Unk15}");
            sw.WriteLine($"FCSUnk16 = {part.Unk16}");
            sw.WriteLine($"ZoomRange = {part.ZoomRange}");
            UnpackRadarComponent(sw, part.RadarComponent);
            sw.WriteLine($"TuneMax_second_locktime = {part.TuneMaxSecondLockTime}");
            sw.WriteLine($"TuneEff_second_locktime = {part.TuneEfficiencySecondLockTime}");
            sw.WriteLine($"TuneMax_missile_lockspd = {part.TuneMaxMissileLockSpeed}");
            sw.WriteLine($"TuneEff_missile_lockspd = {part.TuneEfficiencyMissileLockSpeed}");
        }

        private static void UnpackGenerator(StreamWriter sw, AcParts4.Generator part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            sw.WriteLine($"Capacity = {part.EnergyCapacity}");
            sw.WriteLine($"TuneMax_Capacity = {part.TuneMaxEnergyCapacity}");
            sw.WriteLine($"TuneEff_Capacity = {part.TuneEfficiencyEnergyCapacity}");
            sw.WriteLine($"GeneratorUnk0A = {part.Unk0A}");
            sw.WriteLine($"AllowedEPWaste = {part.EnergyOutputSoftLimit}");
            sw.WriteLine($"ConstantEPSupply = {part.EnergyOutput}");
            sw.WriteLine($"TuneMax_ConstantEPSupply = {part.TuneMaxEnergyOutput}");
            sw.WriteLine($"TuneEff_ConstantEPSupply = {part.TuneEfficiencyEnergyOutput}");
            sw.WriteLine($"PARecover = {part.KPOutput}");
            sw.WriteLine($"TuneMax_PARecover = {part.TuneMaxKPOutput}");
            sw.WriteLine($"TuneEff_PARecover = {part.TuneEfficiencyKPOutput}");
            sw.WriteLine($"ActiveSE = {part.ActiveSE}");
            sw.WriteLine($"GeneratorUnk22 = {part.Unk22}");
        }

        private static void UnpackMainBooster(StreamWriter sw, AcParts4.MainBooster part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackBoosterComponent(sw, part.HorizontalBoost, "Horizontal");
            UnpackBoosterComponent(sw, part.VerticalBoost, "Vertical");
            UnpackBoosterComponent(sw, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"QBReloadTime = {part.QuickReloadTime}");
                sw.WriteLine($"MainBoosterUnk31 = {part.Unk31}");
                sw.WriteLine($"MainBoosterUnk32 = {part.Unk32}");
            }
        }

        private static void UnpackBackBooster(StreamWriter sw, AcParts4.BackBooster part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackBoosterComponent(sw, part.HorizontalBoost, "Horizontal");
            UnpackBoosterComponent(sw, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"QBReloadTime = {part.QuickReloadTime}");
                sw.WriteLine($"BackBoosterUnk31 = {part.Unk31}");
                sw.WriteLine($"BackBoosterUnk32 = {part.Unk32}");
            }
        }

        private static void UnpackSideBooster(StreamWriter sw, AcParts4.SideBooster part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackBoosterComponent(sw, part.HorizontalBoost, "Horizontal");
            UnpackBoosterComponent(sw, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"QBReloadTime = {part.QuickReloadTime}");
                sw.WriteLine($"SideBoosterUnk31 = {part.Unk31}");
                sw.WriteLine($"SideBoosterUnk32 = {part.Unk32}");
            }
        }

        private static void UnpackOveredBooster(StreamWriter sw, AcParts4.OveredBooster part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackBoosterComponent(sw, part.HorizontalBoost, "Fly", "Fly", "FlyAccel");
            sw.WriteLine($"PAWaste = {part.OveredBoostKPCost}");
            sw.WriteLine($"OveredBoosterUnk12 = {part.Unk12}");
            sw.WriteLine($"PrepareEPWaste = {part.PrepareENCost}");
            sw.WriteLine($"PrepareKPWaste = {part.PrepareKPCost}");
            sw.WriteLine($"FirstPower = {part.OBActivationThrust}");
            sw.WriteLine($"FirstAccelEPWaste = {part.OBActivationENCost}");
            sw.WriteLine($"FirstAccelKPWaste = {part.OBActivationKPCost}");
            sw.WriteLine($"FirstAccelTime = {part.OBActivationLimit}");
            if (version == AcParts4.AcParts4Version.AC4)
            {
                sw.WriteLine($"Sfx_ObCharge = {part.SFXOverboostCharge}");
                sw.WriteLine($"Sfx_ObLaunch = {part.SFXOverboostLaunch}");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"OveredBoosterUnk2C = {part.Unk2C}");
                sw.WriteLine($"AssaultArmorAttack = {part.AssaultArmorAttackPower}");
                sw.WriteLine($"AssaultArmorRange = {part.AssaultArmorRange}");
                sw.WriteLine($"OveredBoosterUnk34 = {part.Unk34}");
                sw.WriteLine($"OveredBoosterUnk38 = {part.Unk38}");
                sw.WriteLine($"OveredBoosterUnk3C = {part.Unk3C}");
            }
        }

        private static void UnpackArmUnit(StreamWriter sw, AcParts4.ArmUnit part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackWeaponComponent(sw, part.WeaponComponent);
            sw.WriteLine($"Hanger = {part.HangerRequirement}");
            sw.WriteLine($"DispType = {(AcParts4.DispType)part.DisplayType}");
            sw.WriteLine($"ArmUnitUnk56 = {part.Unk56}");
        }

        private static void UnpackBackUnit(StreamWriter sw, AcParts4.BackUnit part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackWeaponComponent(sw, part.WeaponComponent);
            sw.WriteLine($"BackUnitUnk54 = {part.Unk54}");
            sw.WriteLine($"BackUnitUnk55 = {part.Unk55}");
            sw.WriteLine($"BackUnitUnk56 = {part.Unk56}");
            sw.WriteLine($"BackUnitUnk58 = {part.Unk58}");
            UnpackRadarComponent(sw, part.RadarComponent);
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                UnpackWeaponBoosterComponent(sw, part.WeaponBoosterComponent);
                sw.WriteLine($"AssaultCannonAttack = {part.AssaultCannonAttackPower}");
                sw.WriteLine($"BackUnitUnk8A = {part.Unk8A}");
                sw.WriteLine($"AssaultCannonImpact = {part.AssaultCannonImpact}");
                sw.WriteLine($"AssaultCannonDiffsibility = {part.AssaultCannonAttentuation}");
                sw.WriteLine($"AssaultCannonPenetrability = {part.AssaultCannonPenetration}");
                sw.WriteLine($"BackUnitUnk92 = {part.Unk92}");
            }

            sw.WriteLine($"Type = {part.Type}");
            sw.WriteLine($"DispType = {(AcParts4.DispType)part.DisplayType}");
            if (version == AcParts4.AcParts4Version.AC4)
            {
                sw.WriteLine($"BackUnitUnk6A = {part.Unk6A}");
                sw.WriteLine($"BackUnitUnk6B = {part.Unk6B}");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"TakesBothSlots = {(part.TakesBothSlots ? 1 : 0)}");
                sw.WriteLine($"BackUnitUnk97 = {part.Unk97}");
            }

            UnpackPAComponent(sw, part.PAComponent);
        }

        private static void UnpackShoulderUnit(StreamWriter sw, AcParts4.ShoulderUnit part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            sw.WriteLine($"Type = {part.Type}");
            sw.WriteLine($"bWeaponExist = {(part.IsWeapon ? "ON" : "OFF")}");
            sw.WriteLine($"DispType = {(AcParts4.DispType)part.DisplayType}");
            sw.WriteLine($"ShoulderUnitUnk03 = {part.Unk03}");
            UnpackPAComponent(sw, part.PAComponent);
            sw.WriteLine($"DeviceName = {part.DeviceName}");
            sw.WriteLine($"UseCount = {part.UseCount}");
            sw.WriteLine($"EffectiveFrame = {part.EffectDuration}");
            sw.WriteLine($"ReloadFrame = {part.ReloadFrame}");
            sw.WriteLine($"ShoulderUnitUnk1E = {part.Unk1E}");
            if (version == AcParts4.AcParts4Version.AC4)
            {
                sw.WriteLine($"EffectParam_0 = {part.EffectParam_0}");
                sw.WriteLine($"EffectParam_1 = {part.EffectParam_1}");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"AssaultArmorAttack = {part.AAAttackPower}");
                sw.WriteLine($"AssaultArmorRangeBoost = {part.AARangeBoost}");
            }

            UnpackWeaponComponent(sw, part.WeaponComponent);

            if (version == AcParts4.AcParts4Version.ACFA)
            {
                UnpackWeaponBoosterComponent(sw, part.WeaponBoosterComponent);
            }
        }

        private static void UnpackStabilizer(StreamWriter sw, AcParts4.IStabilizer part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            UnpackStabilizerComponent(sw, part.StabilizerComponent);
        }

        #endregion

        #endregion

        #region Repack

        #region Component

        private static void RepackPartComponent(ValueDictionary fs, AcParts4.PartComponent component, AcParts4.AcParts4Version version)
        {
            component.PartID = fs.ReadUInt16("ID");
            component.ModelID = fs.ReadUInt16OrDefault("ModelID", component.PartID);
            component.Price = fs.ReadInt32OrDefault("Price", 0);
            component.Weight = fs.ReadUInt16("Weight");
            component.ENCost = fs.ReadUInt16("ConstantEPWaste");
            component.InitStatus = fs.ReadByteOrDefault("InitStatus", 0);
            component.CapID = fs.ReadUInt16OrDefault("CapID", 0);
            component.Name = fs.ReadStringOrDefault("PartsName", string.Empty);
            component.MakerName = fs.ReadStringOrDefault("MakerName", string.Empty);
            component.SubCategory = fs.ReadStringOrDefault("SubCategory", string.Empty);
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                component.SubCategoryID = fs.ReadUInt16OrDefault("SubCategoryID", 0);
                component.SetID = fs.ReadUInt16OrDefault("SetID", 0);
            }
            component.Explain = fs.ReadStringOrDefault("Explain", string.Empty).Replace("<BR>", "\r\n");
        }

        private static void RepackWeaponComponent(ValueDictionary fs, AcParts4.WeaponComponent component, bool isWeapon = true)
        {
            component.WeaponFiringMode = fs.ReadEnumOrDefault<AcParts4.WeaponComponent.FiringMode>("prog", 0);
            component.CanLockOn = fs.ReadStringOrDefault("lock_enable", string.Empty) == "ON";
            component.MissileLockTime = fs.ReadByteOrDefault("missile_locktime", 0);
            component.Unk03 = fs.ReadByteOrDefault("WeaponUnk03", 0) == 1;
            component.FiringRange = fs.ReadUInt16OrDefault("EffectiveRange", 0);
            component.MeleeAbility = fs.ReadUInt16OrDefault("WeightBalance", 0);
            component.BulletID = fs.ReadUInt32OrDefault("bulletID", 0);
            component.SFXID = fs.ReadUInt32OrDefault("sfxID", (uint)(isWeapon ? 1 : 0));
            component.HitEffectID = fs.ReadUInt32OrDefault("hitEffectID", 0);
            component.BallisticsVelocity = fs.ReadSingleOrDefault("initspeed", 0f);
            component.ENCost = fs.ReadSingleOrDefault("ep_drain", 0f);
            component.MultiProc = fs.ReadByteOrDefault("multi_proc", 0) == 1;
            component.ProjectileCount = fs.ReadByteOrDefault("once_cnt", 0);
            component.ContinuousFireCount = fs.ReadByteOrDefault("auto_cnt", 0);
            component.Unk1F = fs.ReadByteOrDefault("WeaponUnk1F", 0);
            component.Unk20 = fs.ReadUInt16OrDefault("WeaponUnk20", 0);
            component.AutoInterval = fs.ReadUInt16OrDefault("auto_interval", 0);
            component.FireRate = fs.ReadUInt16OrDefault("reload_time", 0);
            component.Recoil = fs.ReadUInt16OrDefault("recoil", 0);
            component.CostPerRound = fs.ReadUInt16OrDefault("cost", 0);
            component.ShotPrecision = fs.ReadUInt16OrDefault("ShootPrecision", 0);
            component.NumberofMagazines = fs.ReadUInt16OrDefault("mag_cartridge_num", 0);
            component.MagazineCapacity = fs.ReadUInt16OrDefault("mag_bullet_cnt", 0);
            component.MagazineReloadTime = fs.ReadUInt16OrDefault("mag_reload_time", 0);
            component.Weapon_Type = fs.ReadEnumOrDefault<AcParts4.WeaponComponent.WeaponType>("WeaponType", 0);
            component.ChargeTime = fs.ReadUInt16OrDefault("koji_DrainPerSec", 0);
            component.KPChargeCost = fs.ReadUInt16OrDefault("koji_RefillFrame", 0);
            component.KojimaMaxDamageRate = fs.ReadSingleOrDefault("koji_MaxDamageRate", 0f);
            component.AttackLatency = fs.ReadUInt16OrDefault("BladeReload", 0);
            component.Unk3E = fs.ReadUInt16OrDefault("WeaponUnk3E", 0);
            component.Damage_Type = fs.ReadEnumOrDefault<AcParts4.WeaponComponent.DamageType>("Damage_Type", 0);
            component.DamagePierce = fs.ReadByteOrDefault("Damage_Pierce", 0) == 1;
            component.Unk41 = fs.ReadByteOrDefault("WeaponUnk41", 0) == 1;
            component.DamageRadial = fs.ReadByteOrDefault("Damage_Radial", 0) == 1;
            component.AttackPower = fs.ReadSingleOrDefault("Damage_Power", 0f);
            component.ImpactForce = fs.ReadSingleOrDefault("Damage_Recoil", 0f);
            component.PAAttentuation = fs.ReadSingleOrDefault("Damage_Diffsibility", 0f);
            component.PAPenetration = fs.ReadSingleOrDefault("Damage_Penetrability", 0f);
        }

        private static void RepackDefenseComponent(ValueDictionary fs, AcParts4.DefenseComponent component)
        {
            component.BallisticDefense = fs.ReadUInt16("ShellDef");
            component.EnergyDefense = fs.ReadUInt16("EnergyDef");
        }

        private static void RepackPAComponent(ValueDictionary fs, AcParts4.PAComponent component)
        {
            component.PARectification = fs.ReadUInt16OrDefault("WaveStabilizability", 0);
            component.PADurability = fs.ReadUInt16OrDefault("AntiDiffuse", 0);
        }

        private static void RepackFrameComponent(ValueDictionary fs, AcParts4.FrameComponent component)
        {
            component.TuneMaxRectification = fs.ReadUInt16("TuneMax_WaveStabilizability");
            component.TuneEfficiencyRectification = fs.ReadUInt16("TuneEff_WaveStabilizability");
            component.AP = fs.ReadUInt16("AP");
            component.Unk06 = fs.ReadUInt16OrDefault("FrameUnk06", 0);
            component.DragCoefficient = fs.ReadSingle("Cp");
            component.WeightBalanceFront = fs.ReadUInt16("WeightBalanceFront");
            component.WeightBalanceBack = fs.ReadUInt16("WeightBalanceBack");
            component.WeightBalanceRight = fs.ReadUInt16("WeightBalanceRight");
            component.WeightBalanceLeft = fs.ReadUInt16("WeightBalanceLeft");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RepackBoosterComponent(ValueDictionary fs, AcParts4.BoosterComponent component, string boostType)
            => RepackBoosterComponent(fs, component, boostType, boostType, boostType);

        private static void RepackBoosterComponent(ValueDictionary fs, AcParts4.BoosterComponent component, string thrustName, string boostDurationName, string enCostName)
        {
            component.Thrust = fs.ReadUInt32OrDefault($"{thrustName}Power", 0);
            component.TuneMaxThrust = fs.ReadUInt32OrDefault($"TuneMax_{thrustName}Power", 0);
            component.TuneEfficiencyThrust = fs.ReadUInt16OrDefault($"TuneEff_{thrustName}Power", 0);
            component.QuickBoostDuration = fs.ReadUInt16OrDefault($"{boostDurationName}AccelTime", 0);
            component.ThrustENCost = fs.ReadUInt32OrDefault($"{enCostName}EPWaste", 0);
        }

        private static void RepackRadarComponent(ValueDictionary fs, AcParts4.RadarComponent component)
        {
            component.RadarRange = fs.ReadUInt16OrDefault("RadarRange", 0);
            component.ECMResistance = fs.ReadUInt16OrDefault("AntiECM", 0);
            component.TuneMaxECMResistance = fs.ReadUInt16OrDefault("TuneMax_AntiECM", 0);
            component.TuneEfficiencyECMResistance = fs.ReadUInt16OrDefault("TuneEff_AntiECM", 0);
            component.RadarRefreshRate = fs.ReadUInt16OrDefault("ScanInterval", 0);
            component.TuneMaxRadarRefreshRate = fs.ReadUInt16OrDefault("TuneMax_ScanInterval", 0);
            component.TuneEfficiencyRadarRefreshRate = fs.ReadUInt16OrDefault("TuneEff_ScanInterval", 0);
        }

        private static void RepackWeaponBoosterComponent(ValueDictionary fs, AcParts4.WeaponBoosterComponent component)
        {
            component.HorizontalThrust = fs.ReadUInt32OrDefault("HorizontalPower", 0);
            component.VerticalThrust = fs.ReadUInt32OrDefault("VerticalPower", component.HorizontalThrust);
            component.QuickBoost = fs.ReadUInt32OrDefault("QBPower", component.HorizontalThrust);
            component.Unk0CThrust = fs.ReadUInt32OrDefault("WeaponBoosterUnk0CPower", component.HorizontalThrust);
            component.HorizontalENCost = fs.ReadUInt32OrDefault("HorizontalEPWaste", 0);
            component.VerticalENCost = fs.ReadUInt32OrDefault("VerticalEPWaste", component.HorizontalENCost);
            component.QuickBoostENCost = fs.ReadUInt32OrDefault("QBEPWaste", component.HorizontalENCost);
            component.Unk0CENCost = fs.ReadUInt32OrDefault("WeaponBoosterUnk0CEPWaste", component.HorizontalENCost);
        }

        private static void RepackStabilizerComponent(ValueDictionary fs, AcParts4.StabilizerComponent component)
        {
            component.ControlCalibration = fs.ReadSingle("WeightRate");
        }

        #endregion

        #region Part

        private static void RepackHead(ValueDictionary fs, AcParts4.Head part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackDefenseComponent(fs, part.DefenseComponent);
            RepackPAComponent(fs, part.PAComponent);
            RepackFrameComponent(fs, part.FrameComponent);
            part.Stability = fs.ReadUInt16("Balance");
            part.SFXMonoeye = fs.ReadUInt16("Sfx_Monoeye");
            part.TuneMaxStability = fs.ReadUInt16("TuneMax_Balance");
            part.TuneEfficiencyStability = fs.ReadUInt16("TuneEff_Balance");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.CameraFunctionality = fs.ReadUInt16OrDefault("CameraFunctionality", 500);
                part.SystemRecovery = fs.ReadUInt16OrDefault("SystemRecovery", 500);
                part.Unk28 = fs.ReadUInt16OrDefault("HeadUnk28", 0);
                part.Unk2A = fs.ReadUInt16OrDefault("HeadUnk2A", 0);
            }
            part.StabilizerTopX = fs.ReadInt16("StbzrTopX");
            part.StabilizerTopY = fs.ReadInt16("StbzrTopY");
            part.StabilizerSideX = fs.ReadInt16("StbzrSideX");
            part.StabilizerSideY = fs.ReadInt16("StbzrSideY");
        }

        private static void RepackCore(ValueDictionary fs, AcParts4.Core part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackDefenseComponent(fs, part.DefenseComponent);
            RepackPAComponent(fs, part.PAComponent);
            RepackFrameComponent(fs, part.FrameComponent);
            if (version == AcParts4.AcParts4Version.AC4)
            {
                part.HungerUnit = fs.ReadUInt32("hunger unit");
                part.Unk20 = fs.ReadUInt32OrDefault("CoreUnk20", 0);
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.Unk1C = fs.ReadUInt16OrDefault("CoreUnk1C", 0);
                part.TuneMaxUnk1C = fs.ReadUInt16OrDefault("TuneMax_CoreUnk1C", part.Unk1C);
                part.TuneEfficiencyUnk1C = fs.ReadUInt16OrDefault("TuneEff_CoreUnk1C", 1);
                part.Stability = fs.ReadUInt16OrDefault("Balance", 500);
                part.TuneMaxStability = fs.ReadUInt16OrDefault("TuneMax_Balance", part.Stability);
                part.TuneEfficiencyStability = fs.ReadUInt16OrDefault("TuneEff_Balance", 1);
            }

            part.StabilizerUpX = fs.ReadInt16("StbzrUpX");
            part.StabilizerUpY = fs.ReadInt16("StbzrUpY");
            part.StabilizerLowX = fs.ReadInt16("StbzrLowX");
            part.StabilizerLowY = fs.ReadInt16("StbzrLowY");
        }

        private static void RepackArm(ValueDictionary fs, AcParts4.Arm part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackDefenseComponent(fs, part.DefenseComponent);
            RepackPAComponent(fs, part.PAComponent);
            RepackFrameComponent(fs, part.FrameComponent);
            part.IsWeaponArm = fs.ReadString("Type") == "Weapon";
            part.Unk1D = fs.ReadByteOrDefault("ArmUnk1D", 0);
            part.FiringStability = fs.ReadUInt16("RecoilCtrl");
            part.EnergyWeaponSkill = fs.ReadUInt16("EnergySupply");
            part.WeaponManeuverability = fs.ReadUInt16("Manipulation");
            part.AimPrecision = fs.ReadUInt16("Aiming");
            part.Unk26 = fs.ReadByteOrDefault("ArmUnk26", (byte)(version == AcParts4.AcParts4Version.AC4 ? 0 : 75));
            part.Unk27 = fs.ReadByteOrDefault("ArmUnk27", (byte)(version == AcParts4.AcParts4Version.AC4 ? 0 : 70));
            part.Unk28 = fs.ReadUInt16OrDefault("ArmUnk28", 0);
            part.Unk2A = fs.ReadUInt16OrDefault("ArmUnk2A", 0);
            part.TuneMaxFiringStability = fs.ReadUInt16("TuneMax_RecoilCtrl");
            part.TuneEfficiencyFiringStability = fs.ReadUInt16("TuneEff_RecoilCtrl");
            part.TuneMaxEnergyWeaponSkill = fs.ReadUInt16("TuneMax_EnergySupply");
            part.TuneEfficiencyEnergyWeaponSkill = fs.ReadUInt16("TuneEff_EnergySupply");
            part.TuneMaxWeaponManeuverability = fs.ReadUInt16("TuneMax_Manipulation");
            part.TuneEfficiencyWeaponManeuverability = fs.ReadUInt16("TuneEff_Manipulation");
            part.TuneMaxAimPrecision = fs.ReadUInt16("TuneMax_Aiming");
            part.TuneEfficiencyAimPrecision = fs.ReadUInt16("TuneEff_Aiming");
            part.DisplayType = (byte)fs.ReadEnumOrDefault<AcParts4.DispType>("DispType", 0);
            part.Unk3D = fs.ReadByteOrDefault("ArmUnk3D", 0);
            part.Unk3E = fs.ReadByteOrDefault("ArmUnk3E", 0);
            part.Unk3F = fs.ReadByteOrDefault("ArmUnk3F", 0);
            part.AimType = fs.ReadString("AimType");
            RepackWeaponComponent(fs, part.WeaponComponent, part.IsWeaponArm);
            part.StabilizerX = fs.ReadInt16("StbzrX");
            part.StabilizerY = fs.ReadInt16("StbzrY");
        }

        private static void RepackLeg(ValueDictionary fs, AcParts4.Leg part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackDefenseComponent(fs, part.DefenseComponent);
            RepackPAComponent(fs, part.PAComponent);
            RepackFrameComponent(fs, part.FrameComponent);
            string type = fs.ReadString("Type");
            part.Type = type switch
            {
                "2Legs" => AcParts4.Leg.LegType.Bipedal,
                "RevLegs" => AcParts4.Leg.LegType.ReverseJoint,
                "4Legs" => AcParts4.Leg.LegType.Quad,
                "Tank" => AcParts4.Leg.LegType.Tank,
                _ => throw new FriendlyException($"Unknown Leg Type: {type}"),
            };

            part.Motion = fs.ReadByte("Motion");
            part.Unk1E = fs.ReadUInt16OrDefault("LegUnk1E", 0);
            part.MaxLoad = fs.ReadUInt16("MaxPayload");
            part.Load = fs.ReadUInt16("AllowedPayload");
            part.TuneMaxLoad = fs.ReadUInt16("TuneMax_AllowedPayload");
            part.TuneEfficiencyLoad = fs.ReadUInt16("TuneEff_AllowedPayload");
            part.BackUnitAngle = fs.ReadInt32("CannonAng");
            part.MovementAbility = fs.ReadInt32("MovePerformance");
            part.TurningAbility = fs.ReadInt32("TurnPerformance");
            part.BrakingAbility = fs.ReadInt32("BrakePerformance");
            part.JumpingAbility = fs.ReadInt32("JumpPerformance");
            part.LandingStability = fs.ReadInt32("LandStability");
            part.HitStability = fs.ReadInt32("HitStability");
            part.ShootStability = fs.ReadInt32("ShootStability");
            part.TuneMaxMovementAbility = fs.ReadInt32("TuneMax_MovePerformance");
            part.TuneMaxTurningAbility = fs.ReadInt32("TuneMax_TurnPerformance");
            part.TuneMaxBrakingAbility = fs.ReadInt32("TuneMax_BrakePerformance");
            part.TuneMaxJumpingAbility = fs.ReadInt32("TuneMax_JumpPerformance");
            part.TuneMaxLandingStability = fs.ReadInt32("TuneMax_LandStability");
            part.TuneMaxHitStability = fs.ReadInt32("TuneMax_HitStability");
            part.TuneMaxShootStability = fs.ReadInt32("TuneMax_ShootStability");
            part.TuneEfficiencyMovementAbility = fs.ReadUInt16("TuneEff_MovePerformance");
            part.TuneEfficiencyTurningAbility = fs.ReadUInt16("TuneEff_TurnPerformance");
            part.TuneEfficiencyBrakingAbility = fs.ReadUInt16("TuneEff_BrakePerformance");
            part.TuneEfficiencyJumpingAbility = fs.ReadUInt16("TuneEff_JumpPerformance");
            part.TuneEfficiencyLandingStability = fs.ReadUInt16("TuneEff_LandStability");
            part.TuneEfficiencyHitStability = fs.ReadUInt16("TuneEff_HitStability");
            part.TuneEfficiencyShootStability = fs.ReadUInt16("TuneEff_ShootStability");
            part.Unk70 = fs.ReadUInt16OrDefault("LegUnk70", 0);
            RepackBoosterComponent(fs, part.HorizontalBoost, "Horizontal");
            RepackBoosterComponent(fs, part.VerticalBoost, "Vertical");
            RepackBoosterComponent(fs, part.QuickBoost, "QB");

            if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.UnkA4 = fs.ReadByteOrDefault("LegUnkA4", (byte)(part.Type == AcParts4.Leg.LegType.Tank ? 40 : 0));
                part.UnkA5 = fs.ReadByteOrDefault("LegUnkA5", 0);
                part.UnkA6 = fs.ReadInt16OrDefault("LegUnkA6", 0);
            }

            part.StabilizerBackX = fs.ReadInt16OrDefault("StbzrBackX", 0);
            part.StabilizerBackY = fs.ReadInt16OrDefault("StbzrBackY", 0);
            part.StabilizerUpX = fs.ReadInt16OrDefault("StbzrUpX", 0);
            part.StabilizerUpY = fs.ReadInt16OrDefault("StbzrUpY", 0);
            part.StabilizerMidX = fs.ReadInt16OrDefault("StbzrMidX", 0);
            part.StabilizerMidY = fs.ReadInt16OrDefault("StbzrMidY", 0);
            part.StabilizerLowX = fs.ReadInt16OrDefault("StbzrLowX", 0);
            part.StabilizerLowY = fs.ReadInt16OrDefault("StbzrLowY", 0);
            part.StabilizerUpRightX = fs.ReadInt16OrDefault("StbzrUpRX", 0);
            part.StabilizerUpRightY = fs.ReadInt16OrDefault("StbzrUpRY", 0);
            part.StabilizerMidRightX = fs.ReadInt16OrDefault("StbzrMidRX", 0);
            part.StabilizerMidRightY = fs.ReadInt16OrDefault("StbzrMidRY", 0);
            part.StabilizerLowRightX = fs.ReadInt16OrDefault("StbzrLowRX", 0);
            part.StabilizerLowRightY = fs.ReadInt16OrDefault("StbzrLowRY", 0);
        }

        private static void RepackFCS(ValueDictionary fs, AcParts4.FCS part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            part.Deflect = fs.ReadEnum<AcParts4.FCS.DeflectType>("DeflectType");
            part.LockTargetMax = fs.ReadByte("lockmax");
            part.BladeLockDistance = fs.ReadUInt16("BladeLockRange");
            part.ParallelProcessing = fs.ReadUInt16("Paraeffic");
            part.Visibility = fs.ReadUInt16("Visibility");

            string lockRange = fs.ReadString("LockRange");
            string[] lockRangeValues = lockRange.Split(',', StringSplitOptions.TrimEntries);
            switch (lockRangeValues.Length)
            {
                default:
                case 4:
                    part.UnkLockRange4 = lockRangeValues[3].ToUInt16("UnkLockRange4");
                    goto case 3;
                case 3:
                    part.LockBoxWidth = lockRangeValues[2].ToUInt16("LockBoxWidth");
                    goto case 2;
                case 2:
                    part.LockBoxHeight = lockRangeValues[1].ToUInt16("LockBoxHeight");
                    goto case 1;
                case 1:
                    part.LockDistance = lockRangeValues[0].ToUInt16("LockDistance");
                    break;
                case 0:
                    part.LockDistance = default;
                    part.LockBoxHeight = 16;
                    part.LockBoxWidth = 30;
                    part.UnkLockRange4 = 30;
                    break;
            }

            part.SecondLockTime = fs.ReadUInt16("second_locktime");
            part.MissileLockSpeed = fs.ReadUInt16("missile_lockspd");
            part.MultiLock = fs.ReadByte("multilock") == 1;
            part.Unk15 = fs.ReadByteOrDefault("FCSUnk15", 0);
            part.Unk16 = fs.ReadUInt16OrDefault("FCSUnk16", 0);
            part.ZoomRange = fs.ReadUInt16("ZoomRange");
            RepackRadarComponent(fs, part.RadarComponent);
            part.TuneMaxSecondLockTime = fs.ReadUInt16("TuneMax_second_locktime");
            part.TuneEfficiencySecondLockTime = fs.ReadUInt16("TuneEff_second_locktime");
            part.TuneMaxMissileLockSpeed = fs.ReadUInt16("TuneMax_missile_lockspd");
            part.TuneEfficiencyMissileLockSpeed = fs.ReadUInt16("TuneEff_missile_lockspd");
        }

        private static void RepackGenerator(ValueDictionary fs, AcParts4.Generator part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            part.EnergyCapacity = fs.ReadInt32("Capacity");
            part.TuneMaxEnergyCapacity = fs.ReadInt32("TuneMax_Capacity");
            part.TuneEfficiencyEnergyCapacity = fs.ReadUInt16("TuneEff_Capacity");
            part.Unk0A = fs.ReadUInt16OrDefault("GeneratorUnk0A", 0);
            part.EnergyOutputSoftLimit = fs.ReadInt32("AllowedEPWaste");
            part.EnergyOutput = fs.ReadInt32("ConstantEPSupply");
            part.TuneMaxEnergyOutput = fs.ReadInt32("TuneMax_ConstantEPSupply");
            part.TuneEfficiencyEnergyOutput = fs.ReadUInt16("TuneEff_ConstantEPSupply");
            part.KPOutput = fs.ReadUInt16("PARecover");
            part.TuneMaxKPOutput = fs.ReadUInt16("TuneMax_PARecover");
            part.TuneEfficiencyKPOutput = fs.ReadUInt16("TuneEff_PARecover");
            part.ActiveSE = fs.ReadUInt16("ActiveSE");
            part.Unk22 = fs.ReadUInt16OrDefault("GeneratorUnk22", 0);
        }

        private static void RepackMainBooster(ValueDictionary fs, AcParts4.MainBooster part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackBoosterComponent(fs, part.HorizontalBoost, "Horizontal");
            RepackBoosterComponent(fs, part.VerticalBoost, "Vertical");
            RepackBoosterComponent(fs, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.QuickReloadTime = fs.ReadByteOrDefault("QBReloadTime", 6);
                part.Unk31 = fs.ReadByteOrDefault("MainBoosterUnk31", 0);
                part.Unk32 = fs.ReadUInt16OrDefault("MainBoosterUnk32", 0);
            }
        }

        private static void RepackBackBooster(ValueDictionary fs, AcParts4.BackBooster part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackBoosterComponent(fs, part.HorizontalBoost, "Horizontal");
            RepackBoosterComponent(fs, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.QuickReloadTime = fs.ReadByteOrDefault("QBReloadTime", 6);
                part.Unk31 = fs.ReadByteOrDefault("BackBoosterUnk31", 0);
                part.Unk32 = fs.ReadUInt16OrDefault("BackBoosterUnk32", 0);
            }
        }

        private static void RepackSideBooster(ValueDictionary fs, AcParts4.SideBooster part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackBoosterComponent(fs, part.HorizontalBoost, "Horizontal");
            RepackBoosterComponent(fs, part.QuickBoost, "QB");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.QuickReloadTime = fs.ReadByteOrDefault("QBReloadTime", 6);
                part.Unk31 = fs.ReadByteOrDefault("SideBoosterUnk31", 0);
                part.Unk32 = fs.ReadUInt16OrDefault("SideBoosterUnk32", 0);
            }
        }

        private static void RepackOveredBooster(ValueDictionary fs, AcParts4.OveredBooster part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackBoosterComponent(fs, part.HorizontalBoost, "Fly", "Fly", "FlyAccel");
            part.OveredBoostKPCost = fs.ReadUInt16("PAWaste");
            part.Unk12 = fs.ReadUInt16OrDefault("OveredBoosterUnk12", 0);
            part.PrepareENCost = fs.ReadUInt32("PrepareEPWaste");
            part.PrepareKPCost = fs.ReadUInt32("PrepareKPWaste");
            part.OBActivationThrust = fs.ReadUInt32("FirstPower");
            part.OBActivationENCost = fs.ReadUInt32("FirstAccelEPWaste");
            part.OBActivationKPCost = fs.ReadUInt32("FirstAccelKPWaste");
            part.OBActivationLimit = fs.ReadUInt32("FirstAccelTime");
            if (version == AcParts4.AcParts4Version.AC4)
            {
                part.SFXOverboostCharge = fs.ReadUInt16("Sfx_ObCharge");
                part.SFXOverboostLaunch = fs.ReadUInt16("Sfx_ObLaunch");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.Unk2C = fs.ReadUInt32OrDefault("OveredBoosterUnk2C", 0);
                part.AssaultArmorAttackPower = fs.ReadUInt16OrDefault("AssaultArmorAttack", 0);
                part.AssaultArmorRange = fs.ReadUInt16OrDefault("AssaultArmorRange", 0);
                part.Unk34 = fs.ReadUInt32OrDefault("OveredBoosterUnk34", 0);
                part.Unk38 = fs.ReadUInt32OrDefault("OveredBoosterUnk38", 0);
                part.Unk3C = fs.ReadUInt32OrDefault("OveredBoosterUnk3C", 0);
            }
        }

        private static void RepackArmUnit(ValueDictionary fs, AcParts4.ArmUnit part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackWeaponComponent(fs, part.WeaponComponent);
            part.HangerRequirement = fs.ReadEnum<AcParts4.ArmUnit.HangerType>("Hanger");
            part.DisplayType = (byte)fs.ReadEnumOrDefault<AcParts4.DispType>("DispType", 0);
            part.Unk56 = fs.ReadUInt16OrDefault("ArmUnitUnk56", 0);
        }

        private static void RepackBackUnit(ValueDictionary fs, AcParts4.BackUnit part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackWeaponComponent(fs, part.WeaponComponent);
            part.Unk54 = fs.ReadByteOrDefault("BackUnitUnk54", 1);
            part.Unk55 = fs.ReadByteOrDefault("BackUnitUnk55", 0);
            part.Unk56 = fs.ReadUInt16OrDefault("BackUnitUnk56", 0);
            part.Unk58 = fs.ReadUInt16OrDefault("BackUnitUnk58", 0);
            RepackRadarComponent(fs, part.RadarComponent);
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                RepackWeaponBoosterComponent(fs, part.WeaponBoosterComponent);
                part.AssaultCannonAttackPower = fs.ReadUInt16OrDefault("AssaultCannonAttack", 0);
                part.Unk8A = fs.ReadUInt16OrDefault("BackUnitUnk8A", 0);
                part.AssaultCannonImpact = fs.ReadUInt16OrDefault("AssaultCannonImpact", 0);
                part.AssaultCannonAttentuation = fs.ReadUInt16OrDefault("AssaultCannonDiffsibility", 0);
                part.AssaultCannonPenetration = fs.ReadUInt16OrDefault("AssaultCannonPenetrability", 0);
                part.Unk92 = fs.ReadUInt16OrDefault("BackUnitUnk92", 0);
            }

            part.Type = fs.ReadEnum<AcParts4.BackUnit.BackUnitType>("Type");
            part.DisplayType = (byte)fs.ReadEnumOrDefault<AcParts4.DispType>("DispType", 0);
            if (version == AcParts4.AcParts4Version.AC4)
            {
                part.Unk6A = fs.ReadByteOrDefault("BackUnitUnk6A", 0);
                part.Unk6B = fs.ReadByteOrDefault("BackUnitUnk6B", 0);
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.TakesBothSlots = fs.ReadByteOrDefault("TakesBothSlots", 0) == 1;
                part.Unk97 = fs.ReadByteOrDefault("BackUnitUnk97", 0);
            }

            RepackPAComponent(fs, part.PAComponent);
        }

        private static void RepackShoulderUnit(ValueDictionary fs, AcParts4.ShoulderUnit part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            part.Type = fs.ReadEnum<AcParts4.ShoulderUnit.ShoulderType>("Type");
            part.IsWeapon = fs.ReadString("bWeaponExist") == "ON";
            part.DisplayType = (byte)fs.ReadEnumOrDefault<AcParts4.DispType>("DispType", 0);
            part.Unk03 = fs.ReadByteOrDefault("ShoulderUnitUnk03", 0);
            RepackPAComponent(fs, part.PAComponent);
            part.DeviceName = fs.ReadString("DeviceName");
            part.UseCount = fs.ReadUInt16OrDefault("UseCount", 0);
            part.EffectDuration = fs.ReadUInt16OrDefault("EffectiveFrame", 0);
            part.ReloadFrame = fs.ReadUInt16OrDefault("ReloadFrame", 0);
            part.Unk1E = fs.ReadUInt16OrDefault("ShoulderUnitUnk1E", 0);
            if (version == AcParts4.AcParts4Version.AC4)
            {
                part.EffectParam_0 = fs.ReadSingleOrDefault("EffectParam_0", 0f);
                part.EffectParam_1 = fs.ReadSingleOrDefault("EffectParam_1", 0f);
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                part.AAAttackPower = fs.ReadSingleOrDefault("AssaultArmorAttack", 0f);
                part.AARangeBoost = fs.ReadSingleOrDefault("AssaultArmorRangeBoost", 0f);
            }

            RepackWeaponComponent(fs, part.WeaponComponent, part.IsWeapon);

            if (version == AcParts4.AcParts4Version.ACFA)
            {
                RepackWeaponBoosterComponent(fs, part.WeaponBoosterComponent);
            }
        }

        private static void RepackStabilizer(ValueDictionary fs, AcParts4.IStabilizer part, AcParts4.AcParts4Version version)
        {
            RepackPartComponent(fs, part.PartComponent, version);
            RepackStabilizerComponent(fs, part.StabilizerComponent);
        }

        #endregion

        #endregion
    }
    }

using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace YabberExtended
{
    static class YAcParts4
    {
        private static readonly Encoding ShiftJIS = Encoding.GetEncoding("shift-jis");

        public static void Unpack(this AcParts4 acparts, string sourceName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            Encoding shiftJIS = Encoding.GetEncoding("shift-jis");
            string[] names = new string[]
            {
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
            };

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
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegBackStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegUpperStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex++], acparts.LegMiddleStabilizers, acparts.Version, UnpackStabilizer);
            results[nameIndex] = UnpackParts(targetDir, names[nameIndex], acparts.LegLowerStabilizers, acparts.Version, UnpackStabilizer);

            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-acparts4.xml"), xws);
            xw.WriteStartElement("acparts4");
            xw.WriteElementString("acparts_name", sourceName);
            xw.WriteElementString("acparts_version", acparts.Version.ToString());
            xw.WriteElementString("divide_stabilizers", true.ToString());
            xw.WriteStartElement("partlists");
            for (int i = 0; i < names.Length; i++)
            {
                if (results[i])
                {
                    xw.WriteElementString("partlist", names[i]);
                }
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        private static bool UnpackParts<TPart>(string targetDir, string fileName, IList<TPart> parts, AcParts4.AcParts4Version version, Action<StreamWriter, TPart, AcParts4.AcParts4Version> unpackPart)
        {
            if (parts.Count > 0)
            {
                using (var fs = File.OpenWrite(Path.Combine(targetDir, fileName)))
                using (var sw = new StreamWriter(fs, ShiftJIS))
                {
                    for (int i = 0; i < parts.Count; i++)
                    {
                        var part = parts[i];
                        sw.WriteLine($"[{i}]");
                        unpackPart(sw, part, version);
                    }
                }

                return true;
            }

            return false;
        }

        #region Component

        private static void UnpackPartComponent(StreamWriter sw, AcParts4.PartComponent component, AcParts4.AcParts4Version version)
        {
            sw.WriteLine($"ID = {component.PartID}");
            sw.WriteLine($"ModelID = {(component.PartID == component.ModelID ? string.Empty : component.ModelID.ToString())}"); // Simulate found file behavior where an empty string means part and model ID are the same.
            sw.WriteLine($"Price = {component.Price}");
            sw.WriteLine($"Weight = {component.Weight}");
            sw.WriteLine($"ConstantEPWaste = {component.ENCost}");
            sw.WriteLine($"InitStatus =  = {component.InitStatus}");
            sw.WriteLine($"CapID = {component.CapID}");
            sw.WriteLine($"PartsName = {component.Name}");
            sw.WriteLine($"MakerName = {component.MakerName}");
            sw.WriteLine($"SubCategory = {component.SubCategory}");
            if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"SubCategoryID = {component.SubCategoryID}");
                sw.WriteLine($"SetID = {component.SetID}");
            }
            sw.WriteLine($"Explain = {component.Explain.Replace("\r\n", "<BR>").Replace("\n", "<BR>")}");
        }

        private static void UnpackWeaponComponent(StreamWriter sw, AcParts4.WeaponComponent component)
        {
            sw.WriteLine($"prog = {component.WeaponFiringMode}");
            sw.WriteLine($"lock_enable = {(component.CanLockOn ? "ON" : "OFF")}");
            sw.WriteLine($"missile_locktime = {component.MissileLockTime}");
            sw.WriteLine($"WeaponUnk03 = {component.Unk03}");
            sw.WriteLine($"EffectiveRange = {component.FiringRange}");
            sw.WriteLine($"WeightBalance = {component.MeleeAbility}");
            sw.WriteLine($"bulletID = {component.BulletID}");
            sw.WriteLine($"sfxID = {(component.SFXID == 1 ? string.Empty : component.SFXID.ToString())}"); // Simulate found file behavior where an empty string means sfxID is 1.
            sw.WriteLine($"hitEffectID = {(component.HitEffectID == 0 ? string.Empty : component.HitEffectID.ToString())}"); // Simulate found file behavior where an empty string means hitEffectID is 0.
            sw.WriteLine($"initspeed = {component.BallisticsVelocity}");
            sw.WriteLine($"ep_drain = {component.ENCost}");
            sw.WriteLine($"multi_proc = {component.MultiProc}");
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
            sw.WriteLine($"Damage_Pierce = {component.DamagePierce}");
            sw.WriteLine($"WeaponUnk41 = {component.Unk41}");
            sw.WriteLine($"Damage_Radial = {component.DamageRadial}");
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
            sw.WriteLine($"WeaponBoosterUnk0C = {component.Unk0C}");
            sw.WriteLine($"HorizontalEPWaste = {component.HorizontalENCost}");
            sw.WriteLine($"VerticalEPWaste = {component.VerticalENCost}");
            sw.WriteLine($"QBEPWaste = {component.QuickBoostENCost}");
            sw.WriteLine($"WeaponBoosterUnk1C = {component.Unk1C}");
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
            sw.WriteLine($"TuneEff_Manipulation = {part.TuneMaxWeaponManeuverability}");
            sw.WriteLine($"TuneEff_Manipulation = {part.TuneEfficiencyWeaponManeuverability}");
            sw.WriteLine($"TuneEff_Aiming = {part.TuneMaxAimPrecision}");
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
            string type;
            switch (part.Type)
            {
                case AcParts4.Leg.LegType.Bipedal:
                    type = "2Legs";
                    break;
                case AcParts4.Leg.LegType.ReverseJoint:
                    type = "RevLegs";
                    break;
                case AcParts4.Leg.LegType.Quad:
                    type = "4Legs";
                    break;
                case AcParts4.Leg.LegType.Tank:
                    type = "Tank";
                    break;
                default:
                    type = part.Type.ToString();
                    break;
            }

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

            sw.WriteLine($"Type = {part.BackUnitType}");
            sw.WriteLine($"DispType = {(AcParts4.DispType)part.DisplayType}");
            if (version == AcParts4.AcParts4Version.AC4)
            {
                sw.WriteLine($"BackUnitUnk6A = {part.Unk6A}");
                sw.WriteLine($"BackUnitUnk6B = {part.Unk6B}");
            }
            else if (version == AcParts4.AcParts4Version.ACFA)
            {
                sw.WriteLine($"{part.TakesBothSlots}");
                sw.WriteLine($"BackUnitUnk97 = {part.Unk97}");
            }

            UnpackPAComponent(sw, part.PAComponent);
        }

        private static void UnpackShoulderUnit(StreamWriter sw, AcParts4.ShoulderUnit part, AcParts4.AcParts4Version version)
        {
            UnpackPartComponent(sw, part.PartComponent, version);
            sw.WriteLine($"Type = {part.ShoulderUnitType}");
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
    }
}

using SoulsFormats;
using System;
using System.IO;

namespace Yabber
{
    static class YAcParts4
    {
        public static void Unpack(this AcParts4 ap, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            string headDir = $"{targetDir}\\Heads";
            Directory.CreateDirectory(headDir);
            for (int i = 0; i < ap.Heads.Count; i++)
            {

            }

            for (int i = 0; i < ap.Cores.Count; i++)
            {

            }

            for (int i = 0; i < ap.Arms.Count; i++)
            {

            }

            for (int i = 0; i < ap.Legs.Count; i++)
            {

            }

            for (int i = 0; i < ap.FCSs.Count; i++)
            {

            }

            for (int i = 0; i < ap.Generators.Count; i++)
            {

            }

            for (int i = 0; i < ap.MainBoosters.Count; i++)
            {

            }

            for (int i = 0; i < ap.BackBoosters.Count; i++)
            {

            }

            for (int i = 0; i < ap.SideBoosters.Count; i++)
            {

            }

            for (int i = 0; i < ap.OveredBoosters.Count; i++)
            {

            }

            for (int i = 0; i < ap.ArmUnits.Count; i++)
            {

            }

            for (int i = 0; i < ap.BackUnits.Count; i++)
            {

            }

            for (int i = 0; i < ap.ShoulderUnits.Count; i++)
            {

            }

            for (int i = 0; i < ap.HeadTopStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.HeadSideStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.CoreUpperSideStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.CoreLowerSideStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.ArmStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.LegBackStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.LegUpperStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.LegMiddleStabilizers.Count; i++)
            {

            }

            for (int i = 0; i < ap.LegLowerStabilizers.Count; i++)
            {

            }
        }
    }
}

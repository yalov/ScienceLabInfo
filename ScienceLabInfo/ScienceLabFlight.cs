using System;

namespace ScienceLabInfo
{
    public class ModuleScienceInfo : PartModule
    {
        [KSPField(guiActive = true, guiName = "#SLI_ScientistsRateModificator")]
        string ScientistsModificator_str;

        [KSPField(guiActive = false, guiName = "#SLI_LabRateModificator", advancedTweakable = true)]
        string LabModificator_str;

        [KSPField(guiActive = true, guiName = "#SLI_DatatoScienceConversion", advancedTweakable = true)]
        string DataScienceConversion_str;

        ModuleScienceConverter msc;

        public void Start()
        {
            msc = part.Modules.GetModule<ModuleScienceConverter>();

            DataScienceConversion_str = "1:" + msc.scienceMultiplier;

            double labModificator = msc.dataProcessingMultiplier / 0.5 * Math.Pow(10, 7 - msc.researchTime);

            if (labModificator != 1.0)
            {
                Fields["LabModificator_str"].guiActive = true;
                LabModificator_str = String.Format("×{0:F2}", labModificator);
            }
        }

        public void Update()
        {
            int Stars = 0;
            int ScientistCount = 0;
            foreach (ProtoCrewMember crewmember in part.protoModuleCrew)
            {
                if (crewmember.experienceTrait.Config.Name == "Scientist")
                {
                    Stars += crewmember.experienceLevel;
                    ScientistCount++;
                }
            }

            ScientistsModificator_str = String.Format("×{0:F2}", ScientistCount + Stars * msc.scientistBonus);
        }
    }
}
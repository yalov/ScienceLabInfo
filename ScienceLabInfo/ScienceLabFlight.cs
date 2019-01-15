using System;
using System.Collections.Generic;

namespace ScienceLabInfo
{
    public class ModuleScienceInfo : PartModule
    {
        [KSPField(guiActive = false, guiName = "#SLI_LabRateModifier", advancedTweakable = true)]
        string LabModifier_str;

        [KSPField(guiActive = false, guiName = "#SLI_ScientistsRateModifier")]
        string ScientistsModifier_str;

        [KSPField(guiActive = false, guiName = "#SLI_DatatoScienceConversion", advancedTweakable = true)]
        string DataScienceConversion_str;

        ModuleScienceConverter ModuleSC;

        public void Start()
        {
            List<ModuleScienceConverter> listMSC = part.Modules.GetModules<ModuleScienceConverter>();

            if (listMSC.Count != 1)
                return;

            ModuleSC = listMSC[0];

            Fields["ScientistsModifier_str"].guiActive = true;
            Fields["DataScienceConversion_str"].guiActive = true; 
            DataScienceConversion_str = "1:" + ModuleSC.scienceMultiplier;

            double labModifier = ModuleSC.dataProcessingMultiplier / 0.5 * Math.Pow(10, 7 - ModuleSC.researchTime);

            if (labModifier != 1.0)
            {
                Fields["LabModifier_str"].guiActive = true;
                LabModifier_str = String.Format("×{0:F2}", labModifier);
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

            ScientistsModifier_str = String.Format("×{0:F2}", ScientistCount + Stars * ModuleSC.scientistBonus);
        }
    }
}
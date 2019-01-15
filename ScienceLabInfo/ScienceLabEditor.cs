using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.Localization;

namespace ScienceLabInfo
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class ScienceLabEditor : MonoBehaviour
    {
        private void Start()
        {
            List<AvailablePart> parts = PartLoader.LoadedPartsList.Where
                    (p => p.partPrefab.Modules.GetModules<ModuleScienceConverter>().Any() &&
                        p.partPrefab.Modules.GetModules<ModuleScienceLab>().Any()).ToList();

            foreach (AvailablePart part in parts)
            {
                List<ModuleScienceConverter> modulesSC = part.partPrefab.Modules.GetModules<ModuleScienceConverter>();
                List<ModuleScienceLab> modulesSL = part.partPrefab.Modules.GetModules<ModuleScienceLab>();

                if (modulesSC.Count != 1) continue;
                if (modulesSL.Count != 1) continue;

                ModuleScienceConverter moduleSC = modulesSC[0];
                ModuleScienceLab moduleSL = modulesSL[0];

                moduleSL.Fields.ToString();

                List<AvailablePart.ModuleInfo> modinfos = part.moduleInfos;

                foreach (AvailablePart.ModuleInfo modinfo in modinfos)
                {
                    if (modinfo.moduleName == moduleSL.GUIName)
                    {

                        double lab_modifier = moduleSC.dataProcessingMultiplier / 0.5 * Math.Pow(10, 7 - moduleSC.researchTime);

                        modinfo.info = 
                              Localizer.Format("#SLI_ScientistsRequired", moduleSL.crewsRequired)
                            + Localizer.Format("#SLI_PowerUsage", moduleSC.powerRequirement)
                            + Localizer.Format("#SLI_DataStorage", moduleSL.dataStorage)
                            + Localizer.Format("#SLI_ScienceStorage", moduleSC.scienceCap)
                            + Localizer.Format("#SLI_DatatoScience", moduleSC.scienceMultiplier)

                            + Localizer.Format("#SLI_ResearchSpeed")
                            + " " + Localizer.Format("#SLI_TheLabModifier", lab_modifier.ToString("#0.0#"))
                            + " " + Localizer.Format("#SLI_ScientistLevelBonus", (moduleSC.scientistBonus).ToString("+#0%;-#0%"))

                            + Localizer.Format("#SLI_DataBonuses")
                            + " " + Localizer.Format("#SLI_SurfaceBonus", moduleSL.SurfaceBonus.ToString("+#0%;-#0%"))
                            + " " + Localizer.Format("#SLI_ContextBonus", moduleSL.ContextBonus.ToString("+#0%;-#0%"))
                            + " " + Localizer.Format("#SLI_HomeworldBonus", (moduleSL.homeworldMultiplier - 1).ToString("+#0%;-#0%"));
                        
                    }
                }
            }
        }
    }
}

using KSP.Localization;
using ScienceLabInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static ScienceLabInfo.Logging;

namespace ScienceLabInfo
{
    public class ModuleScienceInfo : PartModule
    {
        [KSPField(guiActive = true, guiName = "#autoLOC_6001440",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string LabStatus_str;  // Operational | Not Enough Crew (2/3) 

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001435",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string Research_str;   // Inactive | Researching | No Power 

        // editor Research: inactive, operational
        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#SLI_PAW_DatatoScience",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string DataToScience_str;

        [KSPField(guiActive = true, guiName = "#autoLOC_6001433",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string Data_str;

        [KSPField(guiActive = true, guiName = "#autoLOC_6001432",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string Science_str;


        [KSPField(guiActive = false, guiActiveEditor = true, guiName = "#SLI_PAW_ScientistsRequired",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string ScientistsRequired_str;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#SLI_PAW_LabRateModifier",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string LabModifier_str;

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#SLI_PAW_ScientistsRateModifier",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string ScientistsModifier_str;

        [KSPField(guiActive = true, guiName = "#SLI_PAW_FinalRate",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string Rate_str;


        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "#SLI_PAW_PowerUsage",
            groupName = "ScienceLabGroup", groupDisplayName = "#ScienceLabGroup_Name", groupStartCollapsed = false)]
        string PowerConsumption_str;



        ModuleScienceConverter ModuleSC;
        ModuleScienceLab ModuleSL;


        public void OnDisable()
        {
            GameEvents.onEditorScreenChange.Remove(OnEditorScreenChange);
            GameEvents.onVesselCrewWasModified.Remove(OnVesselCrewWasModified);
            GameEvents.onKerbalLevelUp.Remove(OnKerbalLevelUp);
        }

        private void OnKerbalLevelUp(ProtoCrewMember member)
        {
            UpdateScientistFieldsInFlight();
        }

        private void OnVesselCrewWasModified(Vessel data)
        {
            UpdateScientistFieldsInFlight();

            // TODO: Check dead, or tourist
        }

        public void OnEditorScreenChange(EditorScreen screen)
        {
            UpdateScientistFieldsInEditor();
        }


        public void Start()
        {
            List<ModuleScienceConverter> listMSC = part.Modules.GetModules<ModuleScienceConverter>();
            List<ModuleScienceLab> listMSL = part.Modules.GetModules<ModuleScienceLab>();

            if (listMSC.Count != 1) return;
            if (listMSL.Count != 1) return;

            ModuleSC = listMSC[0];
            ModuleSL = listMSL[0];

            // --- Hide Stock Fields
            ModuleSC.Fields["sciString"].guiActive = false;
            ModuleSC.Fields["datString"].guiActive = false;
            ModuleSC.Fields["rateString"].guiActive = false;
            ModuleSC.Fields["status"].guiActive = false;
            ModuleSC.Fields["status"].guiActiveEditor = false;
            ModuleSL.Fields["statusText"].guiActive = false;

            DataToScience_str = "1:" + ModuleSC.scienceMultiplier;

            PowerConsumption_str = ModuleSC.powerRequirement + " " + Localizer.Format("#autoLOC_7001414"); // EC/s

            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                UpdateScientistFieldsInEditor();
            else if (HighLogic.LoadedScene == GameScenes.FLIGHT)
                UpdateScientistFieldsInFlight();

            double labModifier = ModuleSC.dataProcessingMultiplier / 0.5 * Math.Pow(10, 7 - ModuleSC.researchTime);
            LabModifier_str = String.Format("×{0:F2}", labModifier);

            if (labModifier == 1.0)
            {
                Fields["LabModifier_str"].guiActive = false;
                Fields["LabModifier_str"].guiActiveEditor = false;
            }


            GameEvents.onEditorScreenChange.Add(OnEditorScreenChange);
            GameEvents.onVesselCrewWasModified.Add(OnVesselCrewWasModified);
        }


        /// <returns> Count of crewed scientists</returns>
        void UpdateScientistFieldsInEditor()
        {
            if (ShipConstruction.ShipManifest != null)
            {
                // an unique ID, which differs one PART from another in the Editor, called craftID
                // Length of members equals to amount of seats, and some ProtoCrewMember could be null 
                var crewmembers = ShipConstruction.ShipManifest.GetPartCrewManifest(this.part.craftID)?.GetPartCrew().Where(n => n != null);
                UpdateScientistFields(crewmembers);
            }
        }
        

        /// <returns> Count of crewed scientists</returns>
        void UpdateScientistFieldsInFlight()
        {
            UpdateScientistFields(this.part.protoModuleCrew);
        }

        
        /// <returns> Count of crewed scientists</returns>
        void UpdateScientistFields(IEnumerable crewmembers)
        {
            int Stars = 0;
            int ScientistCount = 0;

            if (crewmembers != null)
            {
                foreach (ProtoCrewMember crewmember in crewmembers)
                {
                    if (crewmember == null) continue;

                    if (crewmember.experienceTrait.Config.Name == "Scientist")
                    {
                        Stars += crewmember.experienceLevel;
                        ScientistCount++;
                    }
                }
            }

            ScientistsModifier_str = String.Format("×{0:F2}", ScientistCount + Stars * ModuleSC.scientistBonus);
            ScientistsRequired_str = ScientistCount + "/" + ModuleSL.crewsRequired.ToString("F0") + 
                (ScientistCount < Math.Round(ModuleSL.crewsRequired) ? " (" + Localizer.Format("#autoLOC_217388"/*Not Enough Crew*/)+")" : "" );
        }


        public void LateUpdate()
        {
            if (ModuleSC)
            {
                Research_str = ModuleSC.status;

                if (HighLogic.LoadedScene == GameScenes.FLIGHT && ModuleSL)
                {
                    LabStatus_str = ModuleSL.statusText;

                    Data_str = ModuleSC.datString;
                    Science_str = ModuleSC.sciString;

                    Rate_str = ModuleSC.rateString;

                    if (ModuleSC.IsActivated)
                        PowerConsumption_str = ModuleSC.powerRequirement + " " + Localizer.Format("#autoLOC_7001414"); // EC/s
                    else
                        PowerConsumption_str = Localizer.Format("#autoLOC_257023"); // Inactive
                }
                
            }
        }
    }
}

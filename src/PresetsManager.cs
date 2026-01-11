/**
 *  This class manages the UI and controls presets, which can be customized per aircraft model.
 *
 *  Copyright (C) 2022 by Mar'yan Rachynskyy
 *  rmaryan@gmail.com
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using MSFSConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace MSFS2020_Ardunio_Cockpit
{
    [Flags]
    internal enum FS_VERSIONS
    {
        None = 0,
        FS2020 = 1,
        FS2024 = 2
    };

    internal enum SIMVAR_TYPE
    {
        TYPE_STRING,  // the value is shown as string
        TYPE_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading spaces
        TYPE_P0_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading zeroes
        TYPE_BOOLEAN  // a special processing is performed if value is 1.0 or 0.0
    };

    internal class ScreenFieldItem
    {
        public string visibilityCondition = ""; // if empty - the field is always visible
        [JsonIgnore]
        public Boolean visible = true;          // all fields are visible by default

        public string text = "";

        public string x = "0"; // field coordinates on the screen XXX
        public string y = "0"; // field coordinates on the screen YYY
        /*
         *  CCCC - a hexadecimal representation of the 16-bit color(without the leading 0x)
         *         Examples:
         *          BLACK   0000
         *          BLUE    001F
         *          RED F800
         *          GREEN   07E0
         *          CYAN    07FF
         *          MAGENTA F81F
         *          YELLOW  FFE0
         *          WHITE   FFFF
         *          GREY    D6BA
         */
        public string color = "D6BA";
        public string fontSize = "2";   // F - the relative font size from 1 to 4

        [JsonIgnore]
        public string screenItemDefinition = "";

        public ushort textWidth = 0;    // max width of the field
        public string simVariable = ""; // If empty - the item is a constant text, which is never changed
        public string simEvent = "";    // Some variables in MSFS are randomly unwritable. For such cases an event name should be specified.
                                        // I.e. "VOR1 SET" - value is sent to sim using "VOR1 SET" event
        [JsonIgnore]
        public int simEventID = -1;     // the ID which was used to register the sim event
        public string unitOfMeasure = ""; // foot, meter per second squared, etc
        public SIMVAR_TYPE simvarType = SIMVAR_TYPE.TYPE_STRING;  // simple values are just shown on the screen, booleans have two text/color variants
                                                                  // 0 - TYPE_STRING - the value is shown as string
                                                                  // 1 - TYPE_NUMBER - the value is rounded as defined by decimalPoints, padded with leading spaces
                                                                  // 2 - TYPE_P0_NUMBER - the value is rounded as defined by decimalPoints, padded with leading zeroes
                                                                  // 3 - TYPE_BOOLEAN - a special processing is performed if value is 1.0 or 0.0

        public int decimalPlaces = 0;   // the number of decimals after the point
        public string altText = "";     // for boolean - text to show if value=false
        public string altColor = "";    // for boolean - color to use if value=false
        [JsonIgnore]
        public bool lastBooleanValue = true; // the last boolean value assumed for the field. Used only for TYPE_BOOLEAN.
        public string knobSpec = "";    // If set - associates the item with the dashboard encoder.
                                        // Format: NmmmmmmMMMMMMC
                                        // N - knob ID ('0' - '3'),
                                        // mmmmmm - minimum value (can have a leading minus sign)
                                        // MMMMMM - maximum value (can have a leading minus sign)
                                        // C - if 'Y' change the value in circle (when the knob rolls bellow the minumum, the value changes to max and vise versa).
                                        // Example: 1000000000359Y
        public string knobStep = ""; // Knob step specification. If it is parsable to integer (i.e. "0100") - this hard-coded value is taken.
                                     // Otherwise - we'll try to get the increment from the SimVar with such name (i.e. "XMLVAR_AUTOPILOT_ALTITUDE_INCREMENT")

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            // after loading the item - build its definition string
            // Format: xxxyyyCCCCFWWP
            screenItemDefinition = x.PadLeft(3, '0') +
                y.PadLeft(3, '0') +
                color +
                fontSize;
            if (textWidth == 0)
            {
                screenItemDefinition += text.Length.ToString("D2");
            }
            else
            {
                screenItemDefinition += textWidth.ToString("D2");
            }
            if (simvarType == SIMVAR_TYPE.TYPE_P0_NUMBER)
            {
                screenItemDefinition += "0";
            }
            else
            {
                screenItemDefinition += " ";
            }
        }
    }

    internal class SwitchDefItem
    {
        public string switchLabel = ""; // the label to be shown on the app screen
        public string simEventOn = "";  // event to trigger when turned on
        [JsonIgnore]
        public int simEventOnID = -1;
        public uint simEventOnValue = 0;
        public string simEventOff = ""; // event to trigger when turned off
        [JsonIgnore]
        public int simEventOffID = -1;
        public uint simEventOffValue = 0;
    }

    internal class VisibilityCondition
    {
        // This class is used to store the visibility conditions and their references
        public string visibilityVar = "";   // SimVar or WASM variable name (i.e. "FLAPS HANDLE INDEX" or "(L:A32NX_FLAPS_HANDLE_INDEX,enum)")
        public string visibilityVarUnit = ""; // The unit of measure for the visibility variable. Taken into account only for regular SimConnect variables.
        public string visibilityValue = ""; // The value expected to make the screen item visible (i.e. "1")
        public double visibilityDValue = Double.MinValue; // The double representation of the visibilityValue. Double.MinValue if conversion is impossible.
        public int screenItemID = -1;       // The ID of the screen item which visibility should be toggled

        public VisibilityCondition(string vVar, string vUnit, string vVal, int sID)
        {
            visibilityVar = vVar;
            visibilityVarUnit = vUnit;
            visibilityValue = vVal;
            screenItemID = sID;

            if (!double.TryParse(visibilityValue, NumberStyles.Float, null, out visibilityDValue))
            {
                visibilityDValue = Double.MinValue;
            }

        }
    }

    internal class CockpitPreset
    {
        public const uint SWITCHES_COUNT = 20; // overall switches count on the dashboard

        public string presetName = "";   // the preset name to be shown on the screen


        /*
         *  Preset applicability definitions.
         *  As it is impossible to precisely identify the aircraft model in MSFS using SimVars only, a more flexible approach is used.
         *  First, the Flight Simulator version is checked (fsVersion).
         *  Then- ATC_MODEL and ATC_TYPE sim variables are used to identify the aircraft. We search for the specified keywords in these variables as substrings.
         */
        [JsonConverter(typeof(StringEnumConverter))]
        public FS_VERSIONS fsVersion = FS_VERSIONS.None; // Flight Simulator version for which the preset is intended (can be more than one, i.e. "fsVersion":"FS2020, FS2024")
        public List<string> AtcModelKeywords = new List<string>(); // keywords to search in the ATC_MODEL_VAR to identify the aircraft for which this preset is intended
                                               // multiple keywords can be separated by comma. If at least one keyword matches - the preset is accepted.
        public List<string> AtcTypeKeywords = new List<string>();  // keywords to search in the ATC_TYPE_VAR to identify the aircraft for which this preset is intended

        public string bgColor = "";      // dashboard background color        
        public List<ScreenFieldItem> screenFieldItems = new List<ScreenFieldItem>();
        public SwitchDefItem[] switchDefItems;

        public List<VisibilityCondition> visibilityConditions = new List<VisibilityCondition>();

        public CockpitPreset()
        {
            switchDefItems = new SwitchDefItem[SWITCHES_COUNT];
            for (int i = 0; i < CockpitPreset.SWITCHES_COUNT; i++)
            {
                switchDefItems[i] = new SwitchDefItem();
            }
        }
        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            // parse the visibility condition statements
            for (int i = 0; i < screenFieldItems.Count; i++)
            {
                if (screenFieldItems[i].visibilityCondition != "")
                {
                    string[] statements = screenFieldItems[i].visibilityCondition.Split('=');
                    if (statements.Length == 2) {
                        string varName = statements[0];
                        string varUnit = "";
                        if (statements[0][0]!='(')
                        {
                            // for regular variable - check if we have a type definition
                            string[] varDef = statements[0].Split(',');
                            if (varDef.Length == 2)
                            {
                                varName = varDef[0];
                                varUnit = varDef[1];
                            }
                        }
                        visibilityConditions.Add(new VisibilityCondition(varName, varUnit, statements[1], i));
                        screenFieldItems[i].visible = false;
                    }
                }
            }
        }
    }

    internal class PresetsManager
    {
        private readonly string[] initialPresetSwitchLabels = { "SW 0/1","SW 2/3","SW 4/5","SW 6/7","SW 8/9","SW 10/11",
            "ENC 1 / SW 12","ENC 2 / SW 13","ENC 3 / SW 14","ENC 4 / SW 15","SW 16","SW 17","SW 18","SW 19"
        };
        private readonly  string[] presetSwitchLabels = new string[14];

        public List<CockpitPreset> presets = new List<CockpitPreset>();
        public int pID = -1; // current preset ID
        public PresetsManager()
        {
            // load all available presets
            string[] files = Directory.GetFiles("presets", "*.json");
            foreach (string file in files)
            {
                string presetString = File.ReadAllText(file);
                presets.Add(JsonConvert.DeserializeObject<CockpitPreset>(presetString));
            }
        }

        public string GetPresetName()
        {
            return (pID > -1) ? presets[pID].presetName : "";
        }

        /// <summary>
        /// Returns a string with the preset backgrouind color definition ready to go for the Aruino
        /// </summary>
        /// <returns>A string representation of the preset BG color in the format "cccc" </returns>
        public string GetBGColor()
        {
            return (pID > -1) ? presets[pID].bgColor : "";
        }

        /// <summary>
        /// Returns the number of the preset items in the current preset
        /// </summary>
        /// <returns>items count</returns>
        public int GetScreenFieldItemsCount()
        {
            return (pID > -1) ? presets[pID].screenFieldItems.Count : 0;
        }

        public ScreenFieldItem GetScreenFieldItem(int itemID)
        {
            return (pID > -1) ? presets[pID].screenFieldItems[itemID] : null;
        }

        public List<VisibilityCondition> GetVisibilityConditions()
        {
            return (pID > -1) ? presets[pID].visibilityConditions : null;
        }

        /**
         * Returns 1 if the item should be visible, 0 - if invisible.
         */
        public static bool EvaluateVisibilityFlag(string visibilityValue, VisibilityCondition vc)
        {
            // if the values are convertable to double - a double comparison is performed
            // otherwise - a String comparison is done
            if (vc.visibilityDValue > Double.MinValue)
            {
                if (double.TryParse(visibilityValue, NumberStyles.Float, null, out double visibilityDValue))
                {
                    return (visibilityDValue == vc.visibilityDValue);
                }
            }
            return visibilityValue.Equals(vc.visibilityValue);
        }

        public int GetItemIDForSimVar(string simVar)
        {
            if (pID > -1)
            {
                for (int i = 0; i < presets[pID].screenFieldItems.Count; i++)
                {
                    if (presets[pID].screenFieldItems[i].simVariable.Equals(simVar))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public string[] GetPresetSwitchLabels()
        {
            if (pID > -1)
            {
                return presetSwitchLabels;
            }
            else
            {
                return initialPresetSwitchLabels;
            }
        }

        public string GetSwitchEventON(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOn : "";
        }
        public string GetSwitchEventOFF(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOff : "";
        }
        public int GetSwitchEventONID(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOnID : -1;
        }
        public int GetSwitchEventOFFID(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOffID : -1;
        }
        public void SetSwitchEventONID(int sID, int eventID)
        {
            if (pID > -1)
            {
                presets[pID].switchDefItems[sID].simEventOnID = eventID;
            }
        }
        public void SetSwitchEventOFFID(int sID, int eventID)
        {
            if (pID > -1)
            {
                presets[pID].switchDefItems[sID].simEventOffID = eventID;
            }
        }
        public uint GetSwitchEventONValue(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOnValue : 0;
        }
        public uint GetSwitchEventOFFValue(int sID)
        {
            return (pID > -1) ? presets[pID].switchDefItems[sID].simEventOffValue : 0;
        }

        public void DeactivatePreset()
        {
            pID = -1;
        }

        /*
         * Returns:
         *  <0 if preset not found
         *  0 if Default preset is being applied
         *  >0 if precise aircraft preset is available
         */
        public int EmergePresetForAircraft(string[] ac_model_vars, MSFS_VERSION msfsVersion, MainWindow mainWindow_ref)
        {
            // find the matching presets
            pID = -1;
            bool defaultPreset = false;
            var possibleMatches = new List<int>();
            int defaultIndex = -1;

            for (int i = 0; i < presets.Count; i++)
            {
                var preset = presets[i];

                // Step 1: check Flight Simulator version
                if (msfsVersion == MSFS_VERSION.MSFS_2020)
                {
                    if ((preset.fsVersion & FS_VERSIONS.FS2020) == 0)
                    {
                        // preset is not intended for MSFS 2020
                        continue;
                    }
                }
                else if (msfsVersion == MSFS_VERSION.MSFS_2024)
                {
                    if ((preset.fsVersion & FS_VERSIONS.FS2024) == 0)
                    {
                        // preset is not intended for MSFS 2024
                        continue;
                    }
                }

                // discover default preset index by presetName == "Default"
                if (preset.presetName.Equals("Default", StringComparison.OrdinalIgnoreCase))
                {
                    defaultIndex = i;
                }

                // Step 2: check AC MODEL and TYPE keywords
                bool modelKeywordMatched = false;
                if (preset.AtcModelKeywords != null && preset.AtcModelKeywords.Count > 0)
                {
                    foreach (var kw in preset.AtcModelKeywords)
                    {
                        if (!string.IsNullOrEmpty(kw) && ac_model_vars[1].IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            possibleMatches.Add(i);
                            modelKeywordMatched = true;
                            break;
                        }
                    }
                }
                if (!modelKeywordMatched && (preset.AtcTypeKeywords != null) && (preset.AtcTypeKeywords.Count > 0))
                {
                    foreach (var kw in preset.AtcTypeKeywords)
                    {
                        if (!string.IsNullOrEmpty(kw) && ac_model_vars[2].IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            possibleMatches.Add(i);
                            break;
                        }
                    }
                }
            }

            if (possibleMatches.Count == 0)
            {
                if (defaultIndex != -1)
                {
                    pID = defaultIndex;
                    defaultPreset = true;
                }
                else
                {
                    // preset not found
                    return -1;
                }
            }
            else if (possibleMatches.Count == 1)
            {
                pID = possibleMatches[0];
            }
            else
            {
                // multiple exact matches found - always prompt user to pick one
                var names = new List<string>();
                foreach (var idx in possibleMatches) names.Add(presets[idx].presetName);

                DialogResult dlgResult = DialogResult.Cancel;
                int dlgSelectedIndex = -1;

                // Ensure the dialog is created and shown on the UI thread of the owner window.
                // mainWindow_ref is a Form passed by the caller and provides the UI thread context.
                mainWindow_ref.Invoke(new MethodInvoker(delegate
                {
                    using (var dlg = new SelectPresetDialog(names))
                    {
                        dlgResult = dlg.ShowModalOver(mainWindow_ref);
                        dlgSelectedIndex = dlg.SelectedPresetIndex;
                    }
                }));

                if (dlgResult == DialogResult.OK && dlgSelectedIndex >= 0)
                {
                    pID = possibleMatches[dlgSelectedIndex];
                    defaultPreset = false;
                }
                else
                {
                    // user cancelled selection
                    pID = -1;
                }
            }

            if (pID == -1)
            {
                return -1;
            }
            else
            {
                // wipe the event ID's
                foreach (ScreenFieldItem sfi in presets[pID].screenFieldItems)
                {
                    sfi.simEventID = -1;
                }
                foreach (SwitchDefItem sdi in presets[pID].switchDefItems)
                {
                    sdi.simEventOnID = -1;
                    sdi.simEventOffID = -1;
                }

                // generate the main window form labels
                // 3-positions switches
                for (int i = 0; i < 6; i++)
                {
                    presetSwitchLabels[i] = presets[pID].switchDefItems[i * 2].switchLabel + '\n' +
                        presets[pID].switchDefItems[i * 2 + 1].switchLabel;
                }

                // pushbuttons
                for (int i = 6; i < 14; i++)
                {
                    presetSwitchLabels[i] = presets[pID].switchDefItems[i + 6].switchLabel;
                }

                return defaultPreset ? 0 : 1;
            }
        }
    }
}

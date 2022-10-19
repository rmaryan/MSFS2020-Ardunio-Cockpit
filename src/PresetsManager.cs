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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace MSFS2020_Ardunio_Cockpit
{
    internal enum SIMVAR_TYPE
    {
        TYPE_STRING,  // the value is shown as string
        TYPE_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading spaces
        TYPE_P0_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading zeroes
        TYPE_BOOLEAN  // a special processing is performed if value is 1.0 or 0.0
    };

    internal class ScreenFieldItem
    {
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
        public string simEventOn = "";  // event to trigger when turned on
        [JsonIgnore]
        public int simEventOnID = -1;
        public uint simEventOnValue = 0;
        public string simEventOff = ""; // event to trigger when turned off
        [JsonIgnore]
        public int simEventOffID = -1;
        public uint simEventOffValue = 0;
    }

    internal class CockpitPreset
    {
        public const uint SWITCHES_COUNT = 20; // overall switches count on the dashboard

        public string presetName = "";   // the preset name to be shown on the screen
        public string aircraftName = ""; // the aircraft name as specified in the ATC_MODEL_VAR
        public string bgColor = "";      // dashboard background color
        public List<ScreenFieldItem> screenFieldItems = new List<ScreenFieldItem>();
        public SwitchDefItem[] switchDefItems;

        public CockpitPreset()
        {
            switchDefItems = new SwitchDefItem[SWITCHES_COUNT];
            for (int i = 0; i < CockpitPreset.SWITCHES_COUNT; i++)
            {
                switchDefItems[i] = new SwitchDefItem();
            }
        }
    }

    internal class PresetsManager
    {
        private string[] initialPresetSwitchLabels = { "SW 0/1","SW 2/3","SW 4/5","SW 6/7","SW 8/9","SW 10/11",
            "ENC 1 / SW 12","ENC 2 / SW 13","ENC 3 / SW 14","ENC 4 / SW 15","SW 16","SW 17","SW 18","SW 19"
        };
        private string[] presetSwitchLabels = new string[14];
        private readonly Dictionary<string, string> SIMVAR_TO_TITLE_MAP = new Dictionary<string, string>();

        private string _niceTitle(string key)
        {
            if (!SIMVAR_TO_TITLE_MAP.TryGetValue(key, out string result))
            {
                result = key;
            }
            return result;
        }

        public List<CockpitPreset> presets = new List<CockpitPreset>();
        public int pID = -1; // current preset ID
        public PresetsManager()
        {
            // initialize the mapping between the Sim Var names and human-readable titles
            string fileName = "nicetitles.json";
            string jsonString = File.ReadAllText(fileName);
            SIMVAR_TO_TITLE_MAP = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

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

        public void EmergePresetForAircraft(string atc_model)
        {
            // find the approriate preset
            pID = -1;
            for (int i = 0; i < presets.Count; i++)
            {
                if (presets[i].aircraftName.Equals(atc_model))
                {
                    // full match, stop the search
                    pID = i;
                    break;
                }
                if (presets[i].aircraftName.Equals("Default"))
                {
                    // default preset - use it but keep searching
                    pID = i;
                }
            }
            if (pID == -1)
            {
                // preset not found
                return;
            }

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
                presetSwitchLabels[i] = _niceTitle(presets[pID].switchDefItems[i * 2].simEventOn) + '\n' +
                    _niceTitle(presets[pID].switchDefItems[i * 2].simEventOff) + '\n' +
                    _niceTitle(presets[pID].switchDefItems[i * 2 + 1].simEventOff) + '\n' +
                    _niceTitle(presets[pID].switchDefItems[i * 2 + 1].simEventOn);
            }

            // pushbuttons
            for (int i = 6; i < 14; i++)
            {
                presetSwitchLabels[i] = _niceTitle(presets[pID].switchDefItems[i + 6].simEventOn) + '\n' +
                    _niceTitle(presets[pID].switchDefItems[i + 6].simEventOff);
            }

            // knobs
            for (int i = 0; i < presets[pID].screenFieldItems.Count; i++)
            {
                if (!presets[pID].screenFieldItems[i].knobSpec.Equals(""))
                {
                    int knobID = presets[pID].screenFieldItems[i].knobSpec[0] - '0';
                    presetSwitchLabels[knobID + 6] = _niceTitle(presets[pID].screenFieldItems[i].simVariable) + "\n\n" +
                        presetSwitchLabels[knobID + 6];
                }
            }
        }
    }
}

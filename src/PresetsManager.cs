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

using System.Collections.Generic;

namespace MSFS2020_Ardunio_Cockpit
{
    internal enum SIMVAR_TYPE
    {
        TYPE_STRING,  // the value is shown as string
        TYPE_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading spaces
        TYPE_P0_NUMBER,  // the value is rounded as defined by decimalPoints, padded with leading zeroes
        TYPE_BOOLEAN  // a special processing is performed if value is 1.0 or 0.0
    };

    internal class PresetItem
    {
        public string text;

        /**
         * The screen definition is a string which defines how the item is going to be shown on the screen
         * Format: xxxyyyCCCCFWW 
         *         xxx, yyyy  - field coordinates on the screen
         *         CCCC       - a hexadecimal representation of the 16-bit color (without the leading 0x)
         *                       Examples:
         *                        BLACK   0000
         *                        BLUE    001F
         *                        RED     F800
         *                        GREEN   07E0
         *                        CYAN    07FF
         *                        MAGENTA F81F
         *                        YELLOW  FFE0
         *                        WHITE   FFFF
         *                        GREY    D6BA
         *         F          - the relative font size from 1 to 4
         *         WW         - the max number of characters in the field
         */
        public string screenItemDefinition = "";
        public ushort textWidth = 0;      // max width of the field
        public string simVariable; // the variable at the sim side to bind to
        public string simEvent;    // the sim event name to use to send the item data to sim
        public int    simEventID;     // the ID which was used to register the sim event
        public string unitOfMeasure; // foot, meter per second squared, etc
        public SIMVAR_TYPE simvarType;  // simple values are just shown on the screen, booleans have two text/color variants
        public int decimalPlaces;   // the number of decimals after the point
        public string altText;       // for boolean - text to show if value=false
        public string altColor;      // for boolean - color to use if value=false
        public string knobSpec;      // If set - associates the item with the dashboard encoder.
                                     // Format: NmmmmmmMMMMMMC
                                     // N - knob ID ('0' - '3'),
                                     // mmmmmm - minimum value (can have a leading minus sign)
                                     // MMMMMM - maximum value (can have a leading minus sign)
                                     // C - if 'Y' change the value in circle (when the knob rolls bellow the minumum, the value changes to max and vise versa).
                                     // Example: 1000000000359Y
        public string knobStep;      // Knob step specification. If it is parsable to integer (i.e. "0100") - this hard-coded value is taken.
                                     // Otherwise - we'll try to get the increment from the SimVar with such name (i.e. "XMLVAR_AUTOPILOT_ALTITUDE_INCREMENT")

        public PresetItem(string _text,
            ushort _x,
            ushort _y,
            ushort _fontSize,
            ushort _textWidth = 0, // if zero - the _text length will be used
            string _color = "FFFF",
            string _sim_variable = "", // If empty - the item is a constant text, which is never changed
                                       // Some variables are randomly unwritable. For such cases an event name should be specified after the '!'
                                       // I.e. "NAV OBS:1!VOR1 SET" - value is taken from the "NAV OBS:1" variable and sent to sim using "VOR1 SET" event.
            string _unitOfMeasure = "", // if empty - a plain text will be assumed
            SIMVAR_TYPE _simvarType = SIMVAR_TYPE.TYPE_STRING,
            int _decimalPlaces = 0,
            string _altText = "",
            string _altColor = "",
            string _knobSpec = "",
            string _knobStep = "0001"
            )
        {
            text = _text;

            char padCharacter = ' ';
            if (_simvarType == SIMVAR_TYPE.TYPE_P0_NUMBER)
            {
                padCharacter = '0';
            }
            else if (_simvarType == SIMVAR_TYPE.TYPE_BOOLEAN)
            {
                padCharacter = '*';
            }

            screenItemDefinition =
                _x.ToString("D3") +
                _y.ToString("D3") +
                _color +
                _fontSize +
                ((_textWidth == 0) ? (ushort)_text.Length : _textWidth).ToString("D2") +
                padCharacter;
            textWidth = _textWidth;
            string[] simVarEvent = _sim_variable.Split('!');
            simVariable = simVarEvent[0];
            if(simVarEvent.Length>1)
            {
                simEvent = simVarEvent[1];
            } else
            {
                simEvent = "";
            }
            simEventID = -1;
            unitOfMeasure = _unitOfMeasure;
            simvarType = _simvarType;
            decimalPlaces = _decimalPlaces;
            altText = _altText;
            altColor = _altColor;
            knobSpec = _knobSpec;
            knobStep = _knobStep;
        }
    }

    internal class CockpitPreset
    {
        public string bgColor; // dashboard background color
        public List<PresetItem> presetItems = new List<PresetItem>();
    }

    internal class PresetsManager
    {
        private CockpitPreset preset;
        public PresetsManager()
        {

        }

        /// <summary>
        /// Returns a string with the preset backgrouind color definition ready to go for the Aruino
        /// </summary>
        /// <returns>A string representation of the preset BG color in the format "cccc" </returns>
        public string getBGColor()
        {
            return preset.bgColor;
        }

        /// <summary>
        /// Returns the number of the preset items in the current preset
        /// </summary>
        /// <returns>items count</returns>
        public int GetPresetItemsCount()
        {
            return (preset == null) ? 0 : preset.presetItems.Count;
        }

        public PresetItem GetPresetItem(int itemID)
        {
            return preset.presetItems[itemID];
        }

        public int GetItemIDForSimVar(string simVar)
        {
            for (int i = 0; i < preset.presetItems.Count; i++)
            {
                if (preset.presetItems[i].simVariable.Equals(simVar))
                {
                    return i;
                }
            }
            return -1;
        }

        public void BuildPresetForAircraft(string atc_model)
        {
            preset = new CockpitPreset();
            // HACK: just a hard-coded preset as for now
            if (atc_model == "TT:ATCCOM.AC_MODEL_A20N.0.text")
            {
                preset.bgColor = "0000";
                preset.presetItems.Add(new PresetItem(
                      "SPD", 10, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "220", 20, 30, 4, 3, "FFE0",
                      "!!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 110, 42, 4, 1, "FFE0",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "HDG", 190, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "011", 200, 30, 4, 3, "FFE0",
                      "!!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 285, 42, 4, 0, "FFE0",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "ALT", 10, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "22000", 20, 120, 4, 5, "FFE0",
                      "!!!"
                    ));
                //XMLVAR_AUTOPILOT_ALTITUDE_INCREMENT - returns 100 or 1000
                preset.presetItems.Add(new PresetItem(
                      "\x4", 160, 132, 4, 0, "FFE0",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "V/S", 190, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "-----", 200, 120, 4, 5, "FFE0",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "AP", 10, 185, 2, 0, "D6BA",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "A/THR", 60, 185, 2, 0, "07E0",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "APPR", 150, 185, 2, 0, "D6BA",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "LDG GEAR", 10, 220, 2, 0, "D6BA",
                      "!!!"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "AUTO BRK: MAX", 160, 220, 2, 0, "07E0",
                      "!!!"
                    ));
            }
            else
                if (atc_model == "Seneca V")
            {
                preset.bgColor = "0000";
                preset.presetItems.Add(new PresetItem(
                      "HDG", 10, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "220", 20, 30, 4, 3, "FFE0",
                      "AUTOPILOT HEADING LOCK DIR",
                      "degrees",
                      SIMVAR_TYPE.TYPE_P0_NUMBER,
                      0,
                      "",
                      "",
                      "0000000000359Y" // knob specification
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 110, 42, 4, 1, "FFE0",
                      "AUTOPILOT HEADING LOCK",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "\x4",
                      "0000"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "CRS", 10, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "120", 20, 120, 4, 3, "FFE0",
                      "NAV OBS:1!VOR1_SET",
                      "degrees",
                      SIMVAR_TYPE.TYPE_P0_NUMBER,
                      0,
                      "",
                      "",
                      "1000000000359Y" // knob specification
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 110, 132, 4, 0, "FFE0",
                      "AUTOPILOT NAV1 LOCK",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "\x4",
                      "0000"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "GPS ", 20, 160, 2, 4, "07E0",
                      "GPS DRIVES NAV1",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "VLOC",
                      "07E0"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "ALT", 160, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "01100", 170, 30, 4, 5, "FFE0",
                      "INDICATED ALTITUDE",
                      "feet",
                      SIMVAR_TYPE.TYPE_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 310, 42, 4, 0, "FFE0",
                      "AUTOPILOT ALTITUDE LOCK",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "\x4",
                      "0000"
                    ));

                preset.presetItems.Add(new PresetItem(
                      "V/S", 160, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "     ", 170, 120, 4, 5, "FFE0",
                      "AUTOPILOT VERTICAL HOLD VAR",
                      "feet/minute",
                      SIMVAR_TYPE.TYPE_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "\x4", 310, 132, 4, 0, "FFE0",
                      "AUTOPILOT VERTICAL HOLD",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "\x4",
                      "0000"
                    ));


                preset.presetItems.Add(new PresetItem(
                      "AP", 10, 220, 2, 0, "07E0",
                      "AUTOPILOT MASTER",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "AP",
                      "D6BA"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "Flaps:", 60, 220, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "0", 130, 220, 2, 1, "FFE0",
                      "FLAPS HANDLE INDEX",
                      "number",
                      SIMVAR_TYPE.TYPE_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "GEAR: DOWN", 170, 220, 2, 0, "FFE0",
                      "GEAR POSITION",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "GEAR: UP  ",
                      "FFE0"
                    ));
            }
            else
            {
                // simple default layout
                preset.bgColor = "0000";


                preset.presetItems.Add(new PresetItem(
                      "SPD", 10, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "220", 20, 30, 4, 3, "FFE0",
                      "AIRSPEED INDICATED",
                      "knots",
                      SIMVAR_TYPE.TYPE_P0_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "HDG", 190, 10, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "011", 200, 30, 4, 3, "FFE0",
                      "HEADING INDICATOR",
                      "degrees",
                      SIMVAR_TYPE.TYPE_P0_NUMBER
                      ));
                preset.presetItems.Add(new PresetItem(
                      "ALT", 10, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "22000", 20, 120, 4, 5, "FFE0",
                      "INDICATED ALTITUDE",
                      "feet",
                      SIMVAR_TYPE.TYPE_P0_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "V/S", 190, 100, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "-----", 200, 120, 4, 5, "FFE0",
                      "VERTICAL SPEED",
                      "feet/minute",
                      SIMVAR_TYPE.TYPE_P0_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "AP", 10, 220, 2, 0, "07E0",
                      "AUTOPILOT MASTER",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "AP",
                      "D6BA"
                    ));
                preset.presetItems.Add(new PresetItem(
                      "Flaps:", 60, 220, 2, 0, "FFE0"));
                preset.presetItems.Add(new PresetItem(
                      "0", 130, 220, 2, 1, "FFE0",
                      "FLAPS HANDLE INDEX",
                      "number",
                      SIMVAR_TYPE.TYPE_NUMBER
                    ));
                preset.presetItems.Add(new PresetItem(
                      "GEAR: DOWN", 165, 220, 2, 0, "FFE0",
                      "GEAR POSITION",
                      "Bool",
                      SIMVAR_TYPE.TYPE_BOOLEAN,
                      0,
                      "GEAR: UP  ",
                      "FFE0"
                    ));
            }
        }
    }
}
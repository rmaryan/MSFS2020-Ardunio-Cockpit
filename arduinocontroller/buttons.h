/**
    Arduino MEGA firmware for the cockpit hardware.

    Copyright (C) 2022 by Mar'yan Rachynskyy
    rmaryan@gmail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
   Connected pins layout and bits to switches mapping.
   The actual connections are different to what is shown on the schematics, to keep them on the side of practicality.
   The only requirement is to have the first encoder pin suitable for interruptions binding.
   Hardware SPI pins can't be changed.
*/

// The switch states are loaded to 3 8-bit registers.
// Here we define maping of bit# -> switch #
const int8_t BIT_TO_SWITCH[] = {
  -1, // not used
  -1,
  -1,
  -1,
  20, // bit 4
  16,
  19,
  15,
  5, // bit 8
  6,
  12,
  9,
  18, // bit 12
  17,
  14,
  13,
  1, // bit 16
  2,
  8,
  7,
  10, // bit 20
  3,
  4,
  11
};

#define ENC1_PIN1 19
#define ENC1_PIN2 40
#define ENC2_PIN1 18
#define ENC2_PIN2 42
#define ENC3_PIN1 20
#define ENC3_PIN2 46
#define ENC4_PIN1 21
#define ENC4_PIN2 44

// my encoders change the value by 2 per one click
#define CHANGE_PER_CLICK 2

#include <SPI.h>
#define ENCODER_OPTIMIZE_INTERRUPTS
#include <Encoder.h>

// encoders
Encoder knobs[4] = {
  Encoder (ENC1_PIN1, ENC1_PIN2),
  Encoder (ENC2_PIN1, ENC2_PIN2),
  Encoder (ENC3_PIN1, ENC3_PIN2),
  Encoder (ENC4_PIN1, ENC4_PIN2)
};

struct KnobState {
  long knobValue = 0; // this is the actual in-sim variable value
  long lastKnobPosition = -99; // this is the actual last known knob "hardware" position
  int8_t fieldID = -1;
  long min, max;
  uint16_t step;
  bool cycle;
} knobStates[4];

// switches
const byte REG_LATCH = 48;
byte switchBank[3];
byte oldSwitchBank[3] = {0, 0, 0}; // previous state
char switchesStatusString[22] = "S00000000000000000000"; // the switches state representation string, ready to be sent to the PC


String getKnobString(uint8_t id) {
  if (id < 4) {
    return String(knobStates[id].knobValue);
  } else {
    return "";
  }
}

// Returns true if the knob position was changed
bool refreshKnob(uint8_t knobID) {
  bool knobChanged = false;
  long newPosition = knobs[knobID].read() / CHANGE_PER_CLICK;
  if (newPosition != knobStates[knobID].lastKnobPosition) {
    knobChanged = true;

    // calculate the new value based on knob ticks
    long newValue = knobStates[knobID].knobValue + (newPosition - knobStates[knobID].lastKnobPosition) * knobStates[knobID].step;

    if (knobStates[knobID].cycle) {
      // cycle the value
      if (newValue < knobStates[knobID].min) {
        newValue = knobStates[knobID].max + 1 + (newValue - knobStates[knobID].min);
      } else if (newValue > knobStates[knobID].max) {
        newValue = knobStates[knobID].min + (newValue - knobStates[knobID].max - 1);
      }
    } else {
      // make sure value is within the range
      if (newValue < knobStates[knobID].min) {
        newValue = knobStates[knobID].min;
      } else if (newValue > knobStates[knobID].max) {
        newValue = knobStates[knobID].max;
      }
    }
    knobStates[knobID].knobValue = newValue;
    knobStates[knobID].lastKnobPosition = newPosition;

    // show the new value on the screen
    if (knobStates[knobID].fieldID != -1) {
      FormatScreenField(knobStates[knobID].fieldID, String(newValue));

      // in regular mode - text changes should be drawn immediatelly
      if (!configurationMode) {
        DrawItem(knobStates[knobID].fieldID - 1);
      }
    }
  }
  return knobChanged;
}


// Returns true if the switch changes need to be sent back to the PC
bool readSwitchPositions() {
  digitalWrite (REG_LATCH, LOW);    // pulse the parallel load latch
  digitalWrite (REG_LATCH, HIGH);
  bool switchChanged = false;

  for (int n = 0; n < 3; n++) {
    switchBank[n] = SPI.transfer (0);

    if (switchBank[n] != oldSwitchBank[n]) {
      switchChanged = true;
      byte mask = 1;
      for (int i = 0; i < 8; i++) {
        if ((switchBank[n] & mask) != (oldSwitchBank[n] & mask)) {
          int8_t switchID = BIT_TO_SWITCH[n * 8 + i];
          if (switchID >= 0) {
            switchesStatusString[switchID] = (switchBank[n] & mask) ? '0' : '1';
          }
        }  // end of bit has changed
        mask <<= 1;
      }  // end of for each bit
    }
    oldSwitchBank[n] = switchBank[n];
  }

  return switchChanged;
}

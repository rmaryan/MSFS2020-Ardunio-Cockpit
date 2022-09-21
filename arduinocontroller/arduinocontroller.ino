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

bool connectionActive = false;

#include "display.h"
#include "buttons.h"
#include "messaging.h"

void setup() {
  Serial.begin(2400);

  tft.begin();
  tft.setRotation(3);

  initStartScreen();

  while (!Serial) {
    ; // wait for serial port to connect. Needed for Native USB only
  }


  // initialize the switches and knobs
  SPI.begin ();
  pinMode (REG_LATCH, OUTPUT);
  digitalWrite (REG_LATCH, HIGH);
}

void loop() {

  /*
     Scan the buttons and encoders
     if the connection with PC was already established
  */
  if (connectionActive) {
    // encoders
    for(uint8_t i = 0; i<4; i++) {
      if(refreshKnob(i)) {
        SendMsg("K" + String(i) + getKnobString(i));
      }
    }

    // switches
    if (readSwitchPositions()) {
      SendMsg(switchesStatusString);
    }
  }

  /*
     Process the messages
  */
  while (Serial.available() > 0)
  {
    int incomingByte = Serial.read();

    switch (incomingByte) {
      case -1:
        // byte not available - nothing to do
        break;
      case 10:
        // this is the message end character
        // add the terminating zero and send it for processing
        messageBuffer[bufferPos] = 0;
        processMessage(messageBuffer);
        bufferPos = 0;
      case 13:
        break;
      default:
        if (bufferPos < (sizeof(messageBuffer) - 1)) {
          messageBuffer[bufferPos++] = (char)incomingByte;
        }
    }
  }
}

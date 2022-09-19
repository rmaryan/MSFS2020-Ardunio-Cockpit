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
   Messaging definitions
*/
int bufferPos = 0;
char messageBuffer[256];

void SendMsg(String data)
{
  Serial.print(data); // send to PC
  Serial.print(char(10));
}

void processMessage(String rawMessage)
{
  uint8_t itemID = 0;
  uint8_t knobID = 0;
  String kValueString;
  long kValue;

  switch (rawMessage[0]) {
    case 'P':
      // Respond to ping
      SendMsg("MSFSCP0");
      connectionActive = true;
      break;
    case 'C':
      // Prepare for the configuration cycle
      if (screenItems != NULL) {
        delete[] screenItems;
        screenItems = NULL;
      }
      screenItemsCount = 0;

      if (rawMessage.length() != 3) {
        SendMsg("EConfiguration message length should be 3. Got: " + rawMessage);
        break;
      }
      screenItemsCount = rawMessage.substring(1, 3).toInt();
      if (screenItemsCount > 0) {
        screenItems = new ScreenItemDefinition[screenItemsCount];
      }
      configurationMode = true;
      break;
    case 'B':
      // Set background color
      if (rawMessage.length() != 5) {
        SendMsg("EBG color length is incorrect. Got: " + rawMessage);
        break;
      }
      bgColor = strtol(rawMessage.substring(1).c_str(), NULL, 16);

      break;
    case 'S':
      // Submit the configuration changes
      tft.fillScreen(bgColor);
      for (int i = 0; i < screenItemsCount; i++) {
        DrawItem(i);
      }
      configurationMode = false;
      break;
    case 'I':
      // Accept screen item
      if (rawMessage.length() != 17) {
        SendMsg("EItem definition length is incorrect. Got: " + rawMessage);
        break;
      }
      itemID = rawMessage.substring(1, 3).toInt();
      if ((itemID == 0) || (itemID > screenItemsCount)) {
        SendMsg("EIncorrect item ID: " + String(itemID) + " Got: " + rawMessage);
        break;
      }

      screenItems[itemID - 1].x = rawMessage.substring(3, 6).toInt();
      screenItems[itemID - 1].y = rawMessage.substring(6, 9).toInt();
      screenItems[itemID - 1].color = strtol(rawMessage.substring(9, 13).c_str(), NULL, 16);
      screenItems[itemID - 1].fontSize = rawMessage.substring(13, 14).toInt();
      screenItems[itemID - 1].textWidth = rawMessage.substring(14, 15).toInt();
      screenItems[itemID - 1].paddingChar = rawMessage.substring(15, 16).toInt();
      if (!configurationMode) {
        DrawItem(itemID - 1);
      }
      break;

    case 'T':
      // Change text
      if (rawMessage.length() < 3) {
        SendMsg("EText change message length should be >=3. Got: " + rawMessage);
        break;
      }
      itemID = rawMessage.substring(1, 3).toInt();
      if ((itemID == 0) || (itemID > screenItemsCount)) {
        SendMsg("EIncorrect text ID. Got: " + rawMessage);
        break;
      }

      FormatScreenField(itemID, rawMessage.substring(3, 3 + screenItems[itemID - 1].textWidth));

      // in regular mode - text changes should be drawn immediatelly
      if (!configurationMode) {
        DrawItem(itemID - 1);
      }
      break;

    case 'K':
      // KNFFmmmmmmMMMMMMSSSC
      if (rawMessage.length() != 20) {
        SendMsg("EKnob definition length is incorrect. Got: " + rawMessage);
        break;
      }

      knobID = rawMessage[1] - 48; // code of '0'
      if (knobID > 3) {
        SendMsg("EKnob ID is incorrect. Got: " + rawMessage);
        break;
      }

      knobStates[knobID].fieldID = rawMessage.substring(2, 4).toInt();
      if ((knobStates[knobID].fieldID == 0) || (knobStates[knobID].fieldID > screenItemsCount)) {
        // no valid mapping was provided, let's mark this knob as unlinked to the scren field
        knobStates[knobID].fieldID = -1;
      }
      knobStates[knobID].min = rawMessage.substring(4, 10).toInt();
      knobStates[knobID].max = rawMessage.substring(10, 16).toInt();
      knobStates[knobID].step = rawMessage.substring(16, 19).toInt();
      knobStates[knobID].cycle = (rawMessage[19] == 'Y');
      break;

    case 'D':
      // DNVVVVV
      if (rawMessage.length() != 7) {
        SendMsg("EKnob value length is incorrect. Got: " + rawMessage);
        break;
      }
      knobID = rawMessage[1] - 48;
      if (knobID > 3) {
        SendMsg("EKnob ID is incorrect. Got: " + rawMessage);
        break;
      }
      kValueString = rawMessage.substring(2);
      kValueString.trim();
      kValue = kValueString.toInt();
      if (knobStates[knobID].knobValue != kValue) {
        knobStates[knobID].knobValue = kValue;
        knobStates[knobID].lastKnobPosition = 0;
        knobs[knobID].write(0);
        if (knobStates[knobID].fieldID != -1) {
          strncpy(screenItems[knobStates[knobID].fieldID - 1].text, kValueString.c_str(), MAX_TEXT_SIZE - 1);
          // in regular mode - text changes should be drawn immediatelly
          if (!configurationMode) {
            DrawItem(knobStates[knobID].fieldID - 1);
          }
        }
      }
      break;

    case 'R':
      // reset to the initial state
      if (screenItems != NULL) {
        delete[] screenItems;
        screenItems = NULL;
      }
      screenItemsCount = 0;
      connectionActive = false;
      initStartScreen();
      break;

    default:
      // ignore unknown commands
      ;
  }
}

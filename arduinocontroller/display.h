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

#include <Adafruit_GFX.h>
#include <MCUFRIEND_kbv.h>

/*
   Screen definitions
*/
MCUFRIEND_kbv tft;

#define START_SCREEN_BG   0x0000
#define START_SCREEN_FG   0xFFE0

#define MAX_TEXT_SIZE     20

// the screen configuration array
struct ScreenItemDefinition {
  char text[MAX_TEXT_SIZE];
  uint16_t x, y, color, fontSize, textWidth;
  char paddingChar;
};

ScreenItemDefinition* screenItems = NULL;
uint8_t screenItemsCount = 0;
bool configurationMode = false;

uint16_t bgColor = 0x0000;

// Populates the screen item with properly formatted text (aligned and padded if needed)
// Field ID starts from 1
void FormatScreenField(uint8_t itemID, String srcString) {
  unsigned int len = srcString.length();
  // is padding needed?
  if (len < screenItems[itemID - 1].textWidth) {
    String buffer;
    char paddingChar = screenItems[itemID - 1].paddingChar;
    if(paddingChar != '0') {
      paddingChar = ' ';
    }
    
    buffer.reserve((screenItems[itemID - 1].textWidth - len));
    for (int i = 0; i < (screenItems[itemID - 1].textWidth - len); i++) {
      buffer = buffer + paddingChar;
    }

    if (screenItems[itemID - 1].paddingChar == ' ') {
      srcString = buffer + srcString;
    } else if (screenItems[itemID - 1].paddingChar == '0') {
      if(srcString[0] == '-') {
        srcString = '-' + buffer + srcString.substring(1);
      } else {
        srcString = buffer + srcString;
      }
    } else {
      // align left, add the missing spaces
      srcString.concat(buffer);
    }
  }

  strncpy(screenItems[itemID - 1].text, srcString.c_str(), MAX_TEXT_SIZE - 1);
}

void DrawItem(uint8_t i) {
  if (i < screenItemsCount) {
    if (screenItems[i].text[0] == 4) {
      // this is a special character for a nice circle
      tft.fillCircle(screenItems[i].x,
                     screenItems[i].y,
                     screenItems[i].fontSize * 2,
                     screenItems[i].color);

    }
    else {
      tft.setTextSize(screenItems[i].fontSize);
      tft.setCursor(screenItems[i].x, screenItems[i].y);
      tft.setTextColor(screenItems[i].color, bgColor);
      tft.print(screenItems[i].text);
    }
  }
}

void initStartScreen() {
  tft.fillScreen(START_SCREEN_BG);
  tft.setTextSize(3);
  tft.setCursor(25, 100);
  tft.setTextColor(START_SCREEN_FG);
  tft.print("Waiting for sim");
}

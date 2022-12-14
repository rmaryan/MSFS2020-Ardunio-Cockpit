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
  char text[MAX_TEXT_SIZE] = "";
  uint16_t x, y, color, fontSize, textWidth;
  char paddingChar;
};

ScreenItemDefinition* screenItems = NULL;
uint8_t screenItemsCount = 0;
bool configurationMode = false;

uint16_t bgColor = 0x0000;

// Sets the screen field text, cutting it if needed
void SetFieldText(uint8_t itemID, const char* srcString) {
  // if the source string is too long, the rightmost part will be trimmed
  strncpy(screenItems[itemID - 1].text, srcString, screenItems[itemID - 1].textWidth);
  screenItems[itemID - 1].text[screenItems[itemID - 1].textWidth] = 0;
}

// Populates the screen item with properly formatted text (aligned and padded if needed)
// Field ID starts from 1
void SetScreenDecimalField(uint8_t itemID, double value, uint8_t decimalPlaces) {
  char buffer[MAX_TEXT_SIZE + 1] = "";

  // snprintf does not work with floats???!!!

  dtostrf(value, screenItems[itemID - 1].textWidth, decimalPlaces, buffer);

  // for zero-padded fields need to play a bit
  if(screenItems[itemID - 1].paddingChar == '0') {
    uint8_t pos = 0;

    // move the leading minus to the first place if needed
    if(value < 0) {
      buffer[0] = '-';
      pos = 1;
    }

    for(uint8_t i = pos; i<screenItems[itemID - 1].textWidth; i++) {
      if((buffer[i] == ' ') || (buffer[i] == '-')) {
        buffer[i] = '0';
      } else {
        break;
      }
    }
  }
  SetFieldText(itemID, buffer);
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

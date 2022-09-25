  <img src="extra/MSFS2020-Ardunio-Cockpit.png"
       alt="Application Logo"
       width="256px"
       align="right"
  />
# Microsoft Flight Simulator Arduino-driven Cockpit

## Introduction

This project a yet another Arduino-driven cockpit that uses inexpensive hardware components and simple configurable software components.

The code is heavily based on the original SimConnect sample [SimvarWatcher](https://docs.flightsimulator.com/html/Samples_And_Tutorials/Samples/SimvarWatcher.htm).

## Windows Sim Connector

MSFS Arduino Cockpit Connector is a mediator between the cockpit Arduino controller and the MSFS simulator.

Arduino is connected through the native Serial over the USB interface.

MSFS connection is done through the SimConnect API.

Start the Cockpit Connector application. Once Arduino is connected to USB and the MSFS is started, click on the "Connect" box - the indicators to the right will show if the connection to the Arduino and MSFS were successful.

![Windows Sim Connector App](extra/pc_connector.png)

As long as "Connect" box is checked, the application will try reconnecting to the simulator and Arduino.

Once the connections are established, Sim Connector checks the aircraft model which is currently loaded in MSFS, and tries to find the most appropriate dashboard layout for it.

Dashboard layouts are currently hard-coded in the C# code. Later we plan to implement dynamic layouts loading from the plain text files.

*Currently only Seneca V preset is functioning.*

## Dashboard Layout

![Dashboard layout](extra/dashboard.png)

We use 6 three-position switches (SW1-SW12), 4 rotary encoders (ENC1 - ENC4) with built-in buttons (SW13-SW16), and 4 push buttons (SW17-SW20).

2.8 TFT screen on the right side is used to show the instrument indications.

## Arduino Firmware

### Serial Connection Protocol

The Windows Sim Connector keeps a constant data exchange with the Arduino cockpit controls.

First, it checks if the compatible Arduino with the proper firmware is connected.

Then - a dashboard layout and controls definitions are uploaded to Arduino, so it knows what to paint on the screen and how to react to the controls changes.

As the simulation goes on, the Connector keeps feeding the new indicator values and keeps receiving the data about the controls. If the user leaves the game (or puts it on pause) the Arduino cockpit also pauses.

In all messages, the first letter defines the message type. The ending character is 13 (CR). All messages are sent as plain text and are case-sensitive.


#### Messages from PC to Arduino
| Type | Format | Function |
| :--- | :---: | :--- |
| P | P | Ping. Properly configured Arduino will respond with "MSFSCPx" string where x is the protocol version. |
| C | Cxx | Configure. Arduino enters the configuration reading mode. Commands after this one should define the new screen layout and controls behavior. xx is the number of controls to be transferred. All controls above this number will be ignored. Example "С21". |
| S | S | Submit. The accumulated layout information will be applied. Please note - the complete screen refresh takes a lot of time. |
| B | Bcccc | Background color. cccc - color code string in hex. |
| I | INNxxxyyyCCCCFWWP | Definition of the screen item. <br/>NN - item ID, starts from 1<br/> xxx,yyy - coordinates; <br/> CCCC - color code string in hex <br/> F - font size (normally 1-4)<br/>WW - field width (characters). P - padding character (if ' ' or '0' the field is right-aligned and padded with the character specified. If other value - the field is left-aligned and not padded). <br/>Example: "I01010010F81F230" |
| T | TNNxxx... | Change field text. Can be sent outside of the configuration mode. <br/> NN - item ID, starts from 1<br/>xxx... Text to be placed in the field (all text till the command end is loaded). In configuration mode, text changes will be shown on the screen after the "S" message, together with all other layout changes. |
| K | KNFFmmmmmmMMMMMMCSSSS | Knob behavior definition. N - knob ID (0 - 3),  FF - associated screen item ID (starts from 1, the knob rotation will instantly update the associated field on the screen, if no field associated - put two spaces here '  '), mmmmmm - minimum value (can have a leading minus sign), MMMMMM - maximum value (can have a leading minus sign), C - if 'Y' change the value in circle (when the knob rolls bellow the minimum, the value changes to max and vise versa), SSSS - change step (integer). Example: K102000000000359Y0001  |
| D | DNVVVVV | Set the knob current value. N - knob ID (0 - 3), VVVVV - value (can have a leading minus sign. Please note - if some screen was associated with that knob - the value on the screen is also updated. No need to send a T-command. |
| R | R | Reset the screen. Arduino will go to the initial "Waiting..." state. |

#### Messages from Arduino to PC
| Type | Format | Function |
| :--- | :---: | :--- |
| M | MSFSCPx | A magic response to the ping message. x - is a protocol version. Currently 0. |
| E | Exxxx... | Error message. xxxx... - error text. |
| K | KNv...  | Knob position change notification. N - knob ID (0-3), v... - value, up to 5 symbols (can be negative). |
| S | SABCDEFGHIJKLMNOPQRST | Knob position change notification. A-T - switch position '1' - ON, other value - OFF. |

### Hardware

 * [Arduino Mega 2560](http://www.banggood.com/Mega2560-R3-ATmega2560-16AU-Control-Board-With-USB-Cable-For-Arduino-p-73020.html?p=M908156347868201609Y)
 * [2.8 TFT screen shield (320x240)](https://www.banggood.com/2_8-Inch-TFT-LCD-Shield-Touch-Display-Screen-Module-Geekcreit-for-Arduino-products-that-work-with-official-Arduino-boards-p-989697.html?p=M908156347868201609Y)
 * [Prototype PCB Boards](https://www.banggood.com/Geekcreit-40pcs-FR-4-2_54mm-Double-Side-Prototype-PCB-Printed-Circuit-Board-p-995732.html?p=M908156347868201609Y)
 * 6x three-position switches
 * 4x rotary encoders with a push button
 * 4x pushbuttons
 * 3x 74HC165 shift registers. We need them to avoid wasting too many Arduino pins for switches connections. Check for more details on [Gammon Forum](http://www.gammon.com.au/forum/?id=11979). 
 * 3x 0.1 uF condensers to stabilize power for the register chips
 * 28x 10-15 kOhm resistors to pull up the switches


 All the electronic elements are soldered to the prototype PCB boards. Arduino MEGA with the screen shield comes separately.
 
 The box for the dashboard was printed on 3D printer. See the design [here](/extra/box_design).

 ![Front Panel](extra/box_design/front_panel.png)
 ![Base Panel](extra/box_design/base_panel.png)

### Schematics

![Electric circuit schematics](extra/schematics/schematics.png )

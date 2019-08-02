# CO2Monitor
## Overview
CO2Monitor is a ASP.NET Core Application for IOT automatization. Smart device in network must return json with information about it state. Device actions (like turn on/off something) is called via rest-api. Devices make no differencies between HTTP methods and accept data only from url.


Device response example:
\{
"temperature" : "24"
\}

Calling device action example: http://192.168.xxx.xxx:xxx/turn/off

## WebClient 
WebClient is a dashboard with information about all devices in network and system log. Also you can see list rules, which binding events with action.

### Adding new device 
You should describe all device state fields and their types (float, string, time, enum). Additionally you can add some device extensions for device. Device extensions add events to devices.

### Timers 

Timer are virtual devices executed on server. They contains one event "Alarm". You can use them for daily actions (like turn off light on evening). 

### Rules

Rules are used for binding device event with device action. You can add some conditions for rule (like check is day off). 

## Slack integration
CO2Monitor support Slack integration via bot. Slack API requires public IP adress. And so, CO2Monitor uses Azure Cloud for communicating with Slack. 


#Unify
####Rhino to Unity exporter

![Screenshot](http://api.ning.com/files/SJDB94xeBdkpvzmAudPOWpVMfuH4LDwh6N0LN4D9mBc5hiw-LX2WQljKb6D1*PAz4-zUvXMVUR5XNpfniGYrD3onoNgR0vC1/UnifyLogo.png?crop=1%3A1&width=171)

This is work in progress. 

#Agenda:

The idea is to create a streamlined pipeline from Rhino to Unity. So far and what have been posted on this github page we have a Rhino exporter plug-in that writes the OBJ file to C:/Temp location along with a UnifySettings.txt file that contains meta-data information about the Rhino file that OBJ doesn't. For example the TXT file will have information about Lights, Cameras, Materials etc so that it can be used to re-create these assets in Unity without a need to manually place and update them. 

The Unity side has not yet, been posted as it is a severe WIP, but so far we were able to automate and wrap into Editor Scripts things like creating the Assets folder structure and importing the OBJ File. Next we also automated creation of prefabs for Light objects and override of Material Assets (not fully functioning, see Issues for more details). 

#Features:

1. Obj export of Rhino Geometry to custom location. 
2. HUD Display (includes map, time of day, etc)

License
============

Unify: A Rhino to Unity exporter (GPL) started by Leland Jobsen.

Copyright (c) 2016-Present, Leland Jobsen

Unify is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

Unify is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Unify; If not, see http://www.gnu.org/licenses/.

@license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+

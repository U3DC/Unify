#Unify
####Rhino to Unity exporter

![](https://github.com/ksobon/Unify/blob/master/_Icons/UnifyLogo-01.png?raw=true)

This is work in progress...still.

#Agenda:

The idea is to create a streamlined pipeline from Rhino to Unity. So far and what have been posted on this github page we have a Rhino exporter plug-in that writes the OBJ file along with a UnifySettings.txt file that contains meta-data information about the Rhino file. For example the TXT file will have information about Lights, Cameras, Materials etc so that it can be used to re-create these assets in Unity without a need to manually place and update them. 

The Unity side has not yet, been posted as it is an early WIP, but so far we were able to automate and wrap into Editor Scripts things like creating the Assets folder structure and importing the OBJ File. Next we also automated creation of prefabs for Light objects and override of Material Assets. 

There is also an effort being put into making the Unity experience better and geared towards Architects. We have put together a HUD display that can be activated using a "M" key while "in-game". Once that is activated you can choose to set a handful of things like turn on design options layers, change camera height or simply teleport yourself to a different location.

#Features:

1. Obj export of Rhino Geometry to Unity Projects. 
2. HUD Display that contains a few things:
  - Camera Controls (location, height, field of view, saturation, ambient occlusion, etc)
  - Design Options Controls (can turn on and off layers specified as design options on expot)
  - Date and Time picker (controls location of sun)
  - Screenshots (using "P" hotkey while "in game")

License
============

Unify: A Rhino to Unity exporter (GPL) started by Leland Jobsen.

Copyright (c) 2016-Present, Leland Jobsen, Konrad K Sobon

Unify is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

Unify is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Unify; If not, see http://www.gnu.org/licenses/.

@license GPL-3.0+ http://spdx.org/licenses/GPL-3.0+

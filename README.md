# Lumascope-600-Board-Initializer
This project demonstrates how to initialize the Lumascope 600-series board via a C# application and a Python script.

This project has three main components:

(1) LumaScopeInitialization.dll :  
This is a C# DLL.  It allows user code to initialize a 600-series Lumascope board from a Python/Matlab script or C# application.  
LumaScopeInitialization.dll is dependent on LumaUSB.dll (which is part of the Lumaview installation).

(2) LumaviewMicroscopeBoardInit.exe :
This is a C# EXE.  It demontrates how to interface to LumaScopeInitialization.dll.
The EXE has controls to light the LEDs on the board and can display video.

(3) main.py :
This is a Python script.  It runs in a console and is not a GUI application.  It shows how to interface to LumaScopeInitialization.dll.
The script allows the user to light the LEDs on the board but cannot display video.
The Python script has comments explaining important details.

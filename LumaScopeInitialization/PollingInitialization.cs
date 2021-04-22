//************************************************************************
// PROJECT PURPOSE:
// This file is part of a C# project, whose purpose is to initialize
// the Lumascope board, which is part of the Lumascope 600, 620, 720, etc.
// The original motivation was to provide a C# DLL to allow a Python  
// script to initialize the Lumascope board.  Ths project also has a
// demo C# EXE, which can initialize the Lumascope board.
//
// PROJECT AND RUNTIME DEPENDENCIES:
// This depends on 'LumaUSB.dll' to build, and 'Lumascope600.hex' to run.
// Those files are part of the Lumaview installation, which are available
// from https://etaluma.com/products/downloads/.  The files need to be
// together in the same folder in order to run.
//
// THIS FILE'S PURPOSE/DESCRIPTION:
// This code initializes (loads "Lumascope600.hex") the Lumascope board but the 
// client needs to  poll the status until the board is initialized.
// Call "connectToMicroscope()" to start initialization process and
// poll the function, "isBoardInitialized()", until it returns true. 
// This code is the interface to the client code, e.g., a C# EXE or a 
// Python script.
// This code is part of the DLL project.
//
// DATE ORIGINALLY CREATED/MODIFIED:
// Early 2021.
//
// AUTHOR:
// Jonathan Spurgin, jpspurgin@yahoo.com
//
//************************************************************************


using System.Diagnostics;
using LumaUSB_ns;
using libusbK;


namespace LumaScopeInitialization
{
    /// <summary>
    /// This class is an example of how to interface to the Etaluma "LumaUSB" 
    /// DLL for initialization without using the Windows messages for callbacks that
    /// inform the application when the USB cable is connected to the microscope.
    /// 
    /// NOTE:
    /// When the Lumascope is powered off, it loses the programming in the Cypress FX2 chip.
    /// The Cypress FX2 chip contains an embedded 8051 microscontroller, into which Lumaview loads
    /// code at startup.  The code is contained in "Lumascope600.hex".
    /// Before the Cypress FX2 8051 microcontroller has the code uploaded, the USB PID (Product ID)
    /// is 0x8613.  After the code in "Lumascope600.hex" is programmed into the 8051, the FX2
    /// gets reset and the PID changes to 0x1004.
    /// 
    /// In the other example in the SDK, "Read ISO", the application uses the "HotK" class to register Windows
    /// messages with the LibUsbK driver, which the LibUsbK driver sends to the application when the 
    /// USB is connected or disconnected.  In that case, as the 8051 microcontroller has just been 
    /// programmed, the LibUsbK driver sends a USB DISCONNECT message followed by a USB CONNECT message.
    /// 
    /// This demo application does not use the "HotK" class but polls the programming/connection state
    /// of the Lumascope.
    /// 
    /// </summary>
    /// 


#if true

    public class PollingInitialization
    {
        /// <summary>
        /// Initiates connection to a Lumascope board.
        /// Note that the board does not get initialized during this call,
        /// but just starts the initialization process.
        /// </summary>
        /// <returns>
        /// True if this function has kicked-off the initialization of the  Lumascope board.
        /// Also, the board must be uninitialized in order to return true.
        /// A board also must be connected in order to return true.
        /// </returns>

#if true

        static public bool initializeLumaUsbObject()
        {
            bool initializationStarted = false;

            VideoParameters videoParams = VideoParametersFactory.getVideoParameters();
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();


            if (true == searchForDevice(LumaUSB.PID_FX2_DEV, ref deviceInfo))
            {
                LumaUSB lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                lumaUsb.DeviceAdded(deviceInfo);
                initializationStarted = true;
            }
            else if (true == searchForDevice(LumaUSB.PID_LSCOPE, ref deviceInfo))
            {
                LumaUSB lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                //Globals.lumaUsb.DeviceAdded(deviceInfo); ***** Calling 'DeviceAdded() here causes exceptions when starting video streaming *****
                initializationStarted = true;
            }
            else
            {
                Trace.WriteLine("ERROR - Can't initialize LumaUSB object.");
            }

            return initializationStarted;
        }

#else

        // TODO : remove?
        static public bool initializeLumaUsbObject(out bool uninitializedBoardConnected, out bool initializedBoardConnected)
        {
            bool initializationStarted = false;
            uninitializedBoardConnected = false;
            initializedBoardConnected = false;

            VideoParameters videoParams = VideoParametersFactory.getVideoParameters();
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();


            if (true == searchForDevice(LumaUSB.PID_FX2_DEV, ref deviceInfo))
            {
                LumaUSB lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                lumaUsb.DeviceAdded(deviceInfo);
                initializationStarted = true;
                uninitializedBoardConnected = true;
            }
            else if (true == searchForDevice(LumaUSB.PID_LSCOPE, ref deviceInfo))
            {
                LumaUSB lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                //Globals.lumaUsb.DeviceAdded(deviceInfo); ***** Calling 'DeviceAdded() here causes exceptions when starting video streaming *****
                initializationStarted = true;
                initializedBoardConnected = true;
            }
            else
            {
                Trace.WriteLine("ERROR - Can't initialize LumaUSB object.");
            }

            return initializationStarted;
        }
#endif

        /// <summary>
        /// Call this function to see if the computer is connected to a Lumascope.
        /// The Lumascope can be programmed or unprogrammed.  The 'usbProductId' 
        /// parameter specifies if this function will search for a unprogrammed or
        /// a programmed Lumascope.
        /// </summary>
        /// <param name="usbProductId">
        /// This can be one of two values: LumaUSB.PID_FX2_DEV, LumaUSB.PID_LSCOPE,
        /// which specifies a programmed and an unprogrammed Lumascope, respectively.
        /// </param>
        /// <param name="outputDeviceInfo">This is a structs that gets filled with device info.</param> 
        /// <returns>True if this function found the device that the 'usbProductId' parameter specified.</returns>
        static private bool searchForDevice(int usbProductId, ref KLST_DEVINFO_HANDLE deviceInfo)
        {
            LstK deviceList = new LstK(KLST_FLAG.NONE);
            return deviceList.FindByVidPid(LumaUSB.VID_CYPRESS, usbProductId, out deviceInfo);
        }


        /// <summary>
        /// Tests if uninitialized Lumascope board is connected to the USB port.
        /// </summary>
        /// <returns>Returns true if an uninitialized board is connected.</returns>
        static public bool isFX2Connected()
        {
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();
            bool result = searchForDevice(LumaUSB.PID_FX2_DEV, ref deviceInfo);
            return result;
        }


        /// <summary>
        /// Tests if initialized (meaning "Lumascope600.hex" downloaded) Lumascope board is connected to the USB port.
        /// </summary>
        /// <returns>Returns true if an initialized board is connected.</returns>
        static public bool isLScopeConnected()
        {
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();
            bool result = searchForDevice(LumaUSB.PID_LSCOPE, ref deviceInfo);
            return result;
        }

    }


#else
    public class PollingInitialization
    {
        /// <summary>
        /// Initiates connection to a Lumascope board.
        /// Note that the board does not get initialized during this call,
        /// but just starts the initialization process.
        /// </summary>
        /// <returns>
        /// True if this function has kicked-off the initialization of the  Lumascope board.
        /// Also, the board must be uninitialized in order to return true.
        /// A board also must be connected in order to return true.
        /// </returns>
        static public bool initializeLumaUsbObject( out bool uninitializedBoardConnected, out bool initializedBoardConnected )
        {
            bool initializationStarted = false;
            uninitializedBoardConnected = false;
            initializedBoardConnected = false;

            VideoParameters videoParams = VideoParametersFactory.getVideoParameters();
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();


            if (true == searchForDevice(LumaUSB.PID_FX2_DEV, ref deviceInfo))
            {
                Globals.lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                Globals.lumaUsb.DeviceAdded(deviceInfo);
                initializationStarted = true;
                uninitializedBoardConnected = true;
            }
            else if(true == searchForDevice(LumaUSB.PID_LSCOPE, ref deviceInfo))
            {
                Globals.lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, videoParams);
                //Globals.lumaUsb.DeviceAdded(deviceInfo); ***** Calling 'DeviceAdded() here causes exceptions when starting video streaming *****
                initializationStarted = true;
                initializedBoardConnected = true;
            }
            else
            {
                Trace.WriteLine("ERROR - Can't initialize LumaUSB object.");
            }

            return initializationStarted;
        }


        /// <summary>
        /// Call this function to see if the computer is connected to a Lumascope.
        /// The Lumascope can be programmed or unprogrammed.  The 'usbProductId' 
        /// parameter specifies if this function will search for a unprogrammed or
        /// a programmed Lumascope.
        /// </summary>
        /// <param name="usbProductId">
        /// This can be one of two values: LumaUSB.PID_FX2_DEV, LumaUSB.PID_LSCOPE,
        /// which specifies a programmed and an unprogrammed Lumascope, respectively.
        /// </param>
        /// <param name="outputDeviceInfo">This is a structs that gets filled with device info.</param> 
        /// <returns>True if this function found the device that the 'usbProductId' parameter specified.</returns>
        static private bool searchForDevice(int usbProductId, ref KLST_DEVINFO_HANDLE deviceInfo)
        {
            LstK deviceList = new LstK(KLST_FLAG.NONE);
            return deviceList.FindByVidPid(LumaUSB.VID_CYPRESS, usbProductId, out deviceInfo);
        }


        /// <summary>
        /// Tests if uninitialized Lumascope board is connected to the USB port.
        /// </summary>
        /// <returns>Returns true if an uninitialized board is connected.</returns>
        static public bool isFX2Connected()
        {
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();
            bool result = searchForDevice(LumaUSB.PID_FX2_DEV, ref deviceInfo);
            return result;
        }


        /// <summary>
        /// Tests if initialized (meaning "Lumascope600.hex" downloaded) Lumascope board is connected to the USB port.
        /// </summary>
        /// <returns>Returns true if an initialized board is connected.</returns>
        static public bool isLScopeConnected()
        {
            KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE();
            bool result = searchForDevice(LumaUSB.PID_LSCOPE, ref deviceInfo);
            return result;
        }

    }
#endif

    }

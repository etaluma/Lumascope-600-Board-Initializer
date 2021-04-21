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
// This code initializes the Aptina video chip on the 
// Lumascope board with default parameters.
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


using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using LumaUSB_ns;


namespace LumaScopeInitialization
{
    /// <summary>
    /// This initializes the Aptina video chip on the 
    /// Lumascope board with default parameters.
    /// </summary>
    public class VideoInitialization
    {
        // This index maps to a specific pixel clock frequency.
        // Corresponds to about 23 MHz.
        const int DEFAULT_PIXEL_CLOCK_FREQUENCY_INDEX = 7; 

        // Default gain in percent.
        public const int DEFAULT_GAIN = 50;


        /// <summary>Initializes the image sensor by sending commands to the LumaScope.</summary>
        /// 
        public static bool initializeImageSensor(LumaUSB lumaUsbObj, out string errorMessage)
        {
            Debug.Assert(null != lumaUsbObj);

            VideoParameters videoParams = VideoParametersFactory.getVideoParameters();

            UInt16 u16RegisterID = LumaUSB.IMAGE_SENSOR_SHUTTER_WIDTH_LOWER;


            if (false == lumaUsbObj.ImageSensorRegisterWrite(u16RegisterID, (UInt16)videoParams.width))
            {
                errorMessage = "Failed to write to the image sensor 'Shutter Width Lower' register!";
                Trace.WriteLine(errorMessage);
                return false;
            }
            else
            {
                errorMessage = string.Format("Set image sensor 'Shutter Width Lower' (exposure) register to {0}", (UInt16)videoParams.width);
                Trace.WriteLine(errorMessage);
            }


            if (false == lumaUsbObj.SetWindowSize(videoParams.width, videoParams.height))
            {
                errorMessage = "Failed to write to the image sensor 'Shutter Width Lower' register!";
                Trace.WriteLine(errorMessage);
                return false;
            }
            else
            {
                errorMessage = string.Format("Set image sensor window size to {0} x {1}.", videoParams.width, videoParams.height);
                Trace.WriteLine(errorMessage);
            }


            // Set the pixel-clock frequency.
            lumaUsbObj.SetImageSensorPixelClockFrequency(DEFAULT_PIXEL_CLOCK_FREQUENCY_INDEX);
            Thread.Sleep(200);  // TODO: document why the delay.  From Aptina?

            // We need to reset the GPIF on the FX2 USB chip on the LumaScope after changing the pixel clock.
            lumaUsbObj.InitializeGPIF();

            // Set the gain of the image sensor.
            lumaUsbObj.ImageSensorRegisterWrite(LumaUSB.IMAGE_SENSOR_GLOBAL_GAIN, DEFAULT_GAIN);

            // Set the exposure of the image sensor
            // and set the default value to maximum.
            if (false == lumaUsbObj.ImageSensorRegisterWrite(LumaUSB.IMAGE_SENSOR_SHUTTER_WIDTH_LOWER, LumaUSB.MAX_IMAGE_SENSOR_EXPOSURE))
            {
                MessageBox.Show("Exposure failed to update.");
                errorMessage = "Failed to write to the image sensor 'Shutter Width Lower' register!";
                Trace.WriteLine(errorMessage);
                return false;
            }
            else
            {
                errorMessage = string.Format("Set image sensor 'Shutter Width Lower' (exposure) register to {0}", LumaUSB.MAX_IMAGE_SENSOR_EXPOSURE);
                Trace.WriteLine(errorMessage);
            }

            Thread.Sleep(200); // TODO: document why the delay.  From Aptina?

            return true;
        }

    }
}

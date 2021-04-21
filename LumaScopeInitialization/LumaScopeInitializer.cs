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
// This code launches the dialog box that initializes the Lumascope board.
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



using LumaScopeInitialization;
using System;

namespace Board_ns
{
    public class LumaScopeInitializer
    {

        /// <summary>
        /// Gives build information.
        /// </summary>
        /// <returns>Returns string with the build date and if DEBUG or RELEASE.</returns>
        static public string version()
        {
            DateTime buildDate = DateTime.Now;

#if DEBUG
            String buildType = "DEBUG";
#else
            String buildType = "RELEASE";
#endif

            return "Version: " + buildDate.ToString() + ", buildType: " + buildType;
        }


        /// <summary>
        /// Launches the initialization dialog box.
        /// </summary>
        /// <param name="autoClose">If true, specifies that this dialog closed when the board has completed initializing.</param>
        static public void launchInitDialog(bool autoClose = false)
        {
            InitializerForm usbMsgForm = new InitializerForm(autoClose);
            usbMsgForm.ShowDialog();
        }
    }
}

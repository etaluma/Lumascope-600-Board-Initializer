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
// This code is part of the EXE project.
//
// DATE ORIGINALLY CREATED/MODIFIED:
// Early 2021.
//
// AUTHOR:
// Jonathan Spurgin, jpspurgin@yahoo.com
//
//************************************************************************



using System;
using System.Windows.Forms;

namespace LumaviewMicroscopeBoardInit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

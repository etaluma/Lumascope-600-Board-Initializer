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
// This is the code for the a dialog box, that the user code launches in
// order to initialize the Lumascope board.  This code depends on 
// messages from the operating system to transition through the 
// initialization steps.  When the Lumascope board is completely
// initialized, the title bar (originally) will display:
// "LumaScope FX2 firmware version number: nn".
// This dialog has three numeric controls to allow the user
// to adjust the brightness of the LEDs attached to the board.
// This code is part of the DLL project.
//
// NOTE:
// This code was grabbed from the Lumaview project and modified.
//
// DATE ORIGINALLY CREATED/MODIFIED:
// Early 2021.
//
// AUTHOR:
// Jonathan Spurgin, jpspurgin@yahoo.com
//
//************************************************************************

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using LumaUSB_ns;
using libusbK;


namespace LumaScopeInitialization
{
    /// <summary>
    /// This is a popup dialog box that initializes the Etaluma Lumascope board.
    /// </summary>
    public partial class InitializerForm : Form
    {
        LumaUSB lumaUsb = null;

        // Declarations to detect if the USB is plugged-in or unplugged.
        private const int WM_USER = 0x400;
        private const int WM_USER_HOT_BASE = WM_USER;
        private const int WM_USER_HOT_REMOVAL = WM_USER_HOT_BASE;
        private const int WM_USER_HOT_ARRIVAL = WM_USER_HOT_BASE + 1;
        private HotK m_HotK;
        private KHOT_PARAMS m_HotParams;
        public VideoParameters m_videoParams;
        string strM600HexPathFile = "Lumascope600.hex";
        bool autoClose = false; // If true, specifies that this dialog closed when the board has completed initializing.

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="autoClose">If true, specifies that this dialog closed when the board has completed initializing.</param>
        public InitializerForm(bool autoClose = false)
        {
            InitializeComponent();
            this.autoClose = autoClose;
        }


        ///<summary>Handles the Form load event.</summary>
        ///<param name='sender'>Boilerplate.</param>
        ///<param name='e'>Boilerplate.</param>
        ///<returns>void.</returns>
        ///<remarks>At startup time this event provides a means by which to fetch user choices that were saved
        ///in persistent storage and to do other initialization as necessary.</remarks>
        private void UsbMsgForm_Load(object sender, EventArgs e)
        {
            //*****************************************************************
            // This block is registration so that we receive USB messages 
            // for plugging in and unplugging the USB connect to the LumaScope.
            //*****************************************************************
            {
                m_HotParams.PatternMatch.DeviceInterfaceGUID = "*";

                // Set the Hwnd handle.
                m_HotParams.UserHwnd = Handle;

                // Set the base user message.  This can be any value greater than or equal to 0x400
                m_HotParams.UserMessage = WM_USER_HOT_BASE;

                // After initializing, send plug events for devices that are currently connected.
                m_HotParams.Flags = KHOT_FLAG.PLUG_ALL_ON_INIT;

                // This will cause HotK to use PostMessage instead of SendMessage.
                m_HotParams.Flags |= KHOT_FLAG.POST_USER_MESSAGE;

                m_HotK = new HotK(ref m_HotParams);
            }


            // Give the user something active to see.
            timer1.Enabled = true;
            pollingProgressBar.Value = 0;

            TopMost = true; // This should force this dialog to the front as Brian said that it was buried when launched from Matlab.
        }


        /// <summary>
        /// The 'WndProc()' calls this when the user connects the LumaScope to the computer with the USB.
        /// This loads the HEX file if not yet loaded, which will cause the LumaScope's FX2 to do
        /// a USB reboot (and change the PID, the HEX file contains the new PID), 
        /// which will cause 'WndProc()' to call this function a second time, which launches 'LiveImageWindow'.
        /// </summary>
        /// <param name="deviceInfo">Information about the USB device.</param>
        private void OnHotPlugInvoked(KLST_DEVINFO_HANDLE deviceInfo)
        {
            if (true == GetDeviceDescriptionList(LumaUSB.PID_FX2_DEV))
            {
                Trace.WriteLine("The LumaScope does not have the HEX file loaded.");

                if (false == VerifyHexFilePath())
                {
                    return;
                }

                if (null == lumaUsb)
                {
                    lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, m_videoParams);
                }

                Trace.WriteLine("Loading the HEX file ...");
                statusLabel.Text = "Loading Lumascope ...";

                lumaUsb.HexPath = strM600HexPathFile;

                // This call loads the HEX file.
                lumaUsb.DeviceAdded(deviceInfo);
            }
            else if (true == GetDeviceDescriptionList(LumaUSB.PID_LSCOPE))
            {
                Trace.WriteLine("The LumaScope has the HEX file loaded.");

                statusLabel.Text = "The LumaScope has the HEX file loaded.";

                // Create the USB here in case that the LumaScope was 
                // initialized during a previous run of LumaView.
                if (null == lumaUsb)
                {
                    lumaUsb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, m_videoParams);
                }

                // This is the final initializatilon step; the board is initialized!!!
                // ------ Get the FX2 8051 firmware version ------ 
                Int16 firmwareVersionWord = 0;
                lumaUsb.GetFx2FirmwareVersion(out firmwareVersionWord);
                string firmwareVersion = string.Format("LumaScope FX2 firmware version number: {0}", firmwareVersionWord);
                Trace.WriteLine(firmwareVersion);
                statusLabel.Text = firmwareVersion;
                timer1.Enabled = false;
                pollingProgressBar.Value = pollingProgressBar.Maximum;

                if (this.autoClose)
                {
                    Close();
                }
            }
            else
            {
                // It appears that the LumaScope is not connected.
                Trace.WriteLine("The LumaScope is not connected!");
            }
        }


        /// <summary>
        /// This tests if a specified LibUsbK device is connected to the computer,
        /// where the product ID is the Cypress.
        /// </summary>
        /// <param name="pid">This is the 'product ID', which normally should be for the uninitialize or initialized LumaScope 600.</param>
        /// <returns>True is the specified device is connected to the computer.</returns>
        public bool GetDeviceDescriptionList(ushort pid)
        {
            bool foundLumaScope = false;
            KLST_DEVINFO_HANDLE deviceInfo;
            int deviceCount = 0;

            LstK deviceList = new LstK(KLST_FLAG.NONE);
            List<string> deviceDescriptions = new List<string>();

            if (deviceList.Count(ref deviceCount) && (deviceCount > 0))
            {

                // Look for this: VID_04B4&PID_8613.
                while (deviceList.MoveNext(out deviceInfo))
                {
                    deviceDescriptions.Add(deviceInfo.DeviceDesc);

                    string vidString = string.Format("{0:x4}", LumaUSB.VID_CYPRESS);
                    vidString = vidString.ToUpper();

                    string pidString = string.Format("{0:x4}", pid);
                    pidString = pidString.ToUpper();

                    string idString = deviceInfo.DeviceID.ToString();
                    idString = idString.ToUpper();

                    if ((true == idString.Contains(vidString)) && (true == idString.Contains(pidString)))
                    {
                        foundLumaScope = true;
                        Trace.WriteLine("Found LumaScope!!!");
                    }


                    Trace.WriteLine(deviceInfo.DeviceID.ToString());

                } // End while.
            } // End if.

            return foundLumaScope;
        }


        /// <summary>
        /// Tests if the HEX file exists.  This function sets the global variable, 'strM600HexPathFile', if not already set.
        /// </summary>
        /// <returns>
        /// True if the file exists and that the global variable, 'strM600HexPathFile', 
        /// contains the path of the HEX file.
        /// </returns>
        private bool VerifyHexFilePath()
        {
            return File.Exists(strM600HexPathFile);
        }


        /// <summary>
        /// The 'WndProc()' calls this when the user disconnects the LumaScope to the computer from the USB.
        /// </summary>
        void UsbDeviceRemoved()
        {
            // Here we need to explictly destroy the USB objects as if 
            // don't we otherwise could not communicate with the LumaScope 
            // after the HEX file is loaded with a PID of 0x1004. 
            // 
            // Before the HEX file is loaded into the LumaScope the PID
            // is 0x8613.  After the HEX file is loaded, the FX2 reboots 
            // and the PID becomes 0x1004.
            //
            // Before the HEX file is loaded we create a USB object with
            // a PID of 0x8613 -  this is the object we must destroy in
            // order to communicate with the LumaScope after the reboot.
            if (null != lumaUsb)
            {
                lumaUsb.Dispose();
                lumaUsb = null;
            }

            statusLabel.Text = "USB device unplugged or resetting";
        }



        ///<summary>Overrides the WndProc message pump interface.</summary>
        ///<param name='msg'>Window message being sent.</param>
        ///<returns>void.</returns>
        ///<remarks>This override provide a mechanism to see if the user may have
        ///double clicked an associated file type on a second copy of the application.</remarks>
        protected override void WndProc(ref Message msg)
        {
            // NOTE: These messages are only for the LibUsbK driver.
            // 'WM_USER_HOT_ARRIVAL' gets called if the LumaScope 
            // is connected when this application starts.

            if ((msg.Msg == WM_USER_HOT_REMOVAL) || (msg.Msg == WM_USER_HOT_ARRIVAL))
            {
                KHOT_HANDLE hotHandle = new KHOT_HANDLE(msg.WParam);
                KLST_DEVINFO_HANDLE deviceInfo = new KLST_DEVINFO_HANDLE(msg.LParam);

                if (msg.Msg == WM_USER_HOT_REMOVAL)
                {
                    Trace.WriteLine("WM_USER_HOT_REMOVAL");
                    statusLabel.Text = "WM_USER_HOT_REMOVAL";
                    UsbDeviceRemoved();
                }
                else
                {
                    Trace.WriteLine("WM_USER_HOT_ARRIVAL");
                    statusLabel.Text = "WM_USER_HOT_ARRIVAL";
                    OnHotPlugInvoked(deviceInfo);
                }

                return;
            }

            base.WndProc(ref msg);

        }


        /// <summary>
        /// Handler for the timer.  Updates progress bar.
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            pollingProgressBar.PerformStep();

            if (pollingProgressBar.Value == pollingProgressBar.Maximum)
            {
                pollingProgressBar.Value = 0;
            }
        }

    }
}


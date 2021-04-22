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
// After the board is initialized, this object can set the brightness
// levels of the LEDs.
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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Board_ns;
using LumaScopeInitialization;
using LumaUSB_ns;


namespace LumaviewMicroscopeBoardInit
{
    public partial class MainForm : Form
    {
        LumaUSB usb = null; // This is the reference to the main class in the 'LumaDLL' DLL.

        
        // ----- Parameters for displaying the images we receive from the LumaScope over USB -----
        VideoParameters videoParams = null; // This is part of the 'LumaUSB' DLL, we need video parameters.
        Bitmap bmap1, bmap2; // These are the bitmaps into which we copy the image data from the LumaScope.
        bool useBitmap1; // Flag to toggle between 'bmap1' and 'bmap2'.
        Rectangle rectBmap; // A Rectangle structure that specifies the portion of the Bitmap to lock.
        Point ptTopLeft; // Upper-left of 'rectBmap'.
        byte[] buffer;   // Caling the 'LumaUSB' DLL will fill this buffer with the image data from the LumaScope.

        // Flag to keep track of if we have initialized the video parameters, which we do only once unless the user unplugs the USB cable.
        bool videoParamsAlreadyInitialized = false;

        int pollingCount = 0; // This keeps track of the number of times we poll the state of the connection when attempting to connect.


        // These are the identifiers for specifying a specific LED 
        // in order to set the brightness of the given LED.
        // These are a parameter to be passed to this function:
        // LumaUSB.LedControllerWrite( XXX_LED_ID, brightness);,
        // where the 'brightness' range is 0 to 255.
        byte BF_LED_ID = (byte)'D';
        byte BLUE_LED_ID = (byte)'C';
        byte GREEN_LED_ID = (byte)'B';
        byte RED_LED_ID = (byte)'A';



        /// <summary>
        /// CTOR
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            videoParams = VideoParametersFactory.getVideoParameters();

            richTextBox.AppendText(
                "Purpose:\r\nThis application is to demonstrate how user code can interface to LumaUSB.dll and LumaScopeInitialization.dll in order to initialize and control a 600-series Lumascope." + 
                "\r\n\r\nInstructions:\r\n\r\n" + 
                "(1) Connect the microscope board to the computer with a USB cable.\r\n\r\n" + 
                "(2) Press either 'Initialize Lumascope Board(Dialog)' button or the 'Initialize Lumascope Board (Polling)' button. The application will indicate if/when the board is initialized.  \r\n\r\n" + 
                "(3) Turn on LEDs and press 'Start Video Streaming'.\r\n\r\n" +
                "(4) Press press 'Stop Video Streaming'\r\n\r\n" + 
                "Note: Terminating this application does not uninitilize the Lumascope board.  If you rerun the application you can directly set the LED and stream video."
                );
        }


        /// <summary>
        /// Called when the form is loaded.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (true == PollingInitialization.isLScopeConnected())
            {
#if false
                // TODO : remove?
                bool uninitializedBoardConnected;
                bool initializedBoardConnected;

                if (true == PollingInitialization.initializeLumaUsbObject(out uninitializedBoardConnected, out initializedBoardConnected))
                {
                    this.usb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, this.videoParams);

                    if (true == initializedBoardConnected)
                    {
                        setControlsToBoardInitializedState();
                    }
                }
                
#else
                if (true == PollingInitialization.initializeLumaUsbObject())
                {
                    this.usb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, this.videoParams);

                    if (true == PollingInitialization.isLScopeConnected())
                    {
                        setControlsToBoardInitializedState();
                    }
                }
            }
#endif
        }


        /// <summary>
        /// Sets the GUI control to enabled or disabled as appropriate.
        /// </summary>
        private void setControlsToBoardInitializedState()
        {
            ledGroupBox.Enabled = true;
            pollingProgressBar.Value = pollingProgressBar.Maximum;
            startStreamingButton.Enabled = true;
            pollingStatusLabel.Text = "Board Initialized";
            msgBasedInitButton.Enabled = false;
            autoCloseCheckBox.Enabled = false;
            pollingBasedInitButton.Enabled = false;
        }


        /// <summary>
        /// Called when the user clicks the button to initialize with Windows Msg-based dialog box.
        /// </summary>
        private void msgBasedInitButton_Click(object sender, EventArgs e)
        {
            LumaScopeInitializer.launchInitDialog(autoCloseCheckBox.Checked);

            if (true == PollingInitialization.isLScopeConnected())
            {
                // *** Note: instantiating a LumaUSB object here is critical, if we
                // *** reuse earlier instantiations, we'll get exceptions when streaming video.
                this.usb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, this.videoParams);
                setControlsToBoardInitializedState();
            }
            else
            {
                pollingStatusLabel.Text = "Failed to connect to Lumascope board.";
            }
        }


        /// <summary>
        /// This kicks off the initialization of the Lumascope board.
        /// We must continually poll for the status in order to determine
        /// when the board is initialized.
        /// This function determines if the Lumascope has already been programmed with 
        /// the "Lumascope600.hex" file or not and programs the Lumascope if not.
        /// This function starts a TIMER, which polls the status of the Lumascope.
        /// </summary>
        private void pollingBasedInitButton_Click(object sender, EventArgs e)
        {
            // Launch the TIMER for indicating status.

#if false
            // TODO: Remove?
            
            bool uninitializedBoardConnected;
            bool initializedBoardConnected;

            bool result = PollingInitialization.initializeLumaUsbObject(out uninitializedBoardConnected, out initializedBoardConnected);
#else
            bool result = PollingInitialization.initializeLumaUsbObject();
#endif
            if (true == result)
            {
                timer1.Enabled = true;
                pollingProgressBar.Value = 0;
                pollingBasedInitButton.Enabled = false;
                pollingCount = 0;
            }
            else
            {
                pollingStatusLabel.Text = "Not connected to an uninitialized board!";
            }
        }


        /// <summary>
        /// This is the WM_TIMER handler, where we check if the board is initialized.
        /// If the board is initialized, we end the periodic checking and enable/disable
        /// the GUI controls as appropriate.
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (true == PollingInitialization.isLScopeConnected())
            {
                this.usb = new LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, this.videoParams);

                pollingStatusLabel.Text = "The Lumascope is initialized.";
                timer1.Enabled = false;
                setControlsToBoardInitializedState();
            }
            else
            {
                pollingProgressBar.PerformStep();

                if (pollingProgressBar.Value == pollingProgressBar.Maximum)
                {
                    pollingProgressBar.Value = 0;
                }

                ++pollingCount;
                pollingStatusLabel.Text = "Initializing Lumascope (" + pollingCount + ") ...";
            }
        }


        ///<summary>
        /// This is invoked when the user clicks the numeric control
        /// to adjust the brightness level of the #1 LED.
        ///</summary>
        private void led1NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            byte brightness = (byte)led1NumericUpDown.Value;
            this.usb.LedControllerWrite(RED_LED_ID, brightness);
        }


        ///<summary>
        /// This is invoked when the user clicks the numeric control
        /// to adjust the brightness level of the #2 LED.
        ///</summary>
        private void led2NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            byte brightness = (byte)led2NumericUpDown.Value;
            this.usb.LedControllerWrite(GREEN_LED_ID, brightness);
        }


        ///<summary>
        /// This is invoked when the user clicks the numeric control
        /// to adjust the brightness level of the #3 LED.
        ///</summary>
        private void led3NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            byte brightness = (byte)led3NumericUpDown.Value;
            this.usb.LedControllerWrite(BLUE_LED_ID, brightness);
        }


        /// <summary>
        /// This is the handler for when the user presses the "Start Streaming" button.
        /// It starts the video streaming.
        /// </summary>
        private void startStreamingButton_Click(object sender, EventArgs e)
        {
            initializeLiveImageGuiObject();

            string errorMessage;
            VideoInitialization.initializeImageSensor(usb, out errorMessage);

            Debug.Assert(null != usb);

            Thread t = new Thread(() => usb.ISOStreamStart());
            usb.StartStreaming();


            t.IsBackground = true;
            t.Priority = ThreadPriority.Highest;
            t.Start();
            tmrUpdateImage.Enabled = true;

            startStreamingButton.Enabled = false;
            stopStreamingButton.Enabled = true;
        }


        /// <summary>Handler for when the user presses the button to stop video streaming.</summary>
        private void stopStreamingButton_Click(object sender, EventArgs e)
        {
            stopStreamRead();
        }


        /// <summary>Stop video streaming and including updating the display.</summary>
        private void stopStreamRead()
        {
            tmrUpdateImage.Enabled = false;
            usb.ISOStreamStop();
            usb.StopStreaming();
            startStreamingButton.Enabled = true;
            stopStreamingButton.Enabled = false;
        }


        /// <summary>
        /// Initialized the video parameters, which we need to do just once.
        /// </summary>
        private void initializeLiveImageGuiObject()
        {
            if (false == videoParamsAlreadyInitialized)
            {
                bmap1 = new Bitmap(videoParams.width, videoParams.height, PixelFormat.Format24bppRgb);
                bmap2 = new Bitmap(videoParams.width, videoParams.height, PixelFormat.Format24bppRgb);
                useBitmap1 = true;
                ptTopLeft = new Point();
                rectBmap = new Rectangle(ptTopLeft, bmap1.Size);

                videoParamsAlreadyInitialized = true;
            }
        }


        /// <summary>
        /// Polls the USB for image data from the LumaScope and displays the image if there data.
        /// </summary>
        private void tmrUpdateImage_Tick(object sender, EventArgs e)
        {
            if (usb.GetLatest24bppBuffer(out buffer))
            {
                // Copy data to picturebox. Use alternate bitmap objects to avoid writing to
                // bitmap currently in use by PictureBox (else would crash occasionally).
                if (useBitmap1)
                {
                    BitmapData bmData = bmap1.LockBits(rectBmap, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    //     .Copy( byte[] source, int startIndex, IntPtr destination, int length )
                    Marshal.Copy(buffer, 0, bmData.Scan0, buffer.Length);
                    bmap1.UnlockBits(bmData);
                    pbImage.Image = bmap1;
                    pbImage.Refresh();
                    useBitmap1 = false;
                }
                else
                {
                    BitmapData bmData = bmap2.LockBits(rectBmap, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    //     .Copy( byte[] source, int startIndex, IntPtr destination, int length )
                    Marshal.Copy(buffer, 0, bmData.Scan0, buffer.Length);
                    bmap2.UnlockBits(bmData);
                    pbImage.Image = bmap2;
                    pbImage.Refresh();
                    useBitmap1 = true;
                }
            }
        }

    }
}

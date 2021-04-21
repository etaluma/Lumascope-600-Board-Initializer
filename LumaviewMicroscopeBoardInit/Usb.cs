using System;
using System.Collections.Generic;
using System.Text;
using BitwiseSystems;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Globalization;  //NumberStyles
using System.IO;
using System.Drawing.Imaging;   //BitmapData


namespace LumaView600
    {
    ///////<summary></summary>
    ////public enum E_LED
    ////    {
    ////    /// <summary></summary>
    ////    NONE    = 0x00,
    ////    /// <summary></summary>
    ////    F1      = 0x01,
    ////    /// <summary></summary>
    ////    F2      = 0x02,
    ////    /// <summary></summary>
    ////    F3      = 0x04,
    ////    /// <summary></summary>
    ////    WHITE   = 0x08,

    ////    /// <summary></summary>
    ////    SNAP    = 0x10,
    ////    /// <summary></summary>
    ////    WAIT    = 0x20,
    ////    /// <summary>Lets the code know it is time to go to the top of the loop</summary>
    ////    LOOP = 0x40,
    ////    /// <summary></summary>
    ////    OFF     = 0x80,

    ////    /// <summary>Mask to see just the Led bits</summary>
    ////    LED_MASK = 0x0F,
    ////    /// <summary>Mask to see just the operation bits</summary>
    ////    OP_MASK = 0xF0,

    ////    /// <summary>Mask to turn off F1 in a bit array</summary>
    ////    F1_OFF = 0xFE,
    ////    /// <summary>Mask to turn off F2 in a bit array</summary>
    ////    F2_OFF = 0xFD,
    ////    /// <summary>Mask to turn off F3 in a bit array</summary>
    ////    F3_OFF = 0xFB,
    ////    /// <summary>Mask to turn off WHITE in a bit array</summary>
    ////    WHITE_OFF = 0xF7,
    ////    }

    class QUsbClass : QuickUsb
        {

        const int   COM8    = 0x13;
        const int   GAIN    = 0x00;
        const int   AEC     = 0x10;
        const int   AECH    = 0x16;

        /// <summary></summary>
        public const int OV9712 = 0x97;

        // indexes into the Led arrays
        public const Int32  F1_IX       = 0;
        public const Int32  F2_IX       = 1;
        public const Int32  F3_IX       = 2;
        public const Int32  WHITE_IX    = 3;

        ///<summary></summary>
        // Everything is driven off the sensorID value
        public Int32                m_s32SensorID           = OV9712; //= Properties.Settings.Default.s32SensorID;
        ///<summary></summary>
        E_SENSOR_COM                m_eSensorCom            = E_SENSOR_COM.I2C;
        E_CLOCK_SPEED               m_eClockSpeed           = E_CLOCK_SPEED.SPEED24MHz;

        Int32                       m_s32SensorAddr         = 96;  //kludge alert
        public Boolean              m_ynDeviceInited        = false;
        public Boolean              m_ynQUsbPluggedIn       = false;

        //You cannot read back what you have driven the port outputs to
        // so we will keep a shadow collection
        public Byte[]               m_au8PortOutShadow      = { 0, 0, 0, 0, 0 };
        String                      m_strVersions           = String.Empty;
        public Int32                m_s32ImageWidth         = 1280;
        public Int32                m_s32ImageHeight        = 800;
        Int32                       m_s32ImageDepth         = 8;
        public Int32                m_s32BmapPixelCount     = 0;

        // for the M500 if the white value is zero it is off, else on
        public Int32                m_s32ColorsToInclude    = 0;   // mask bits for R,G,B,Bright
        public String               m_strMasterValuesPreserved   = String.Empty;

        public String               m_strLedsActive          = String.Empty;
        
        public Boolean[]            m_aynMasterLedsActive   = { true, false, false, true };
        public Byte[]               m_au8MasterLedPercents  = { 0, 0, 0, 0, };
        public Boolean              m_ynMasterAutoGain      = false;
        public Byte                 m_u8MasterGainPercent   = 50;
        public Byte                 m_u8MasterExposurePercent  = 20;

        public Boolean[]            m_aynCaptureLedsActive   = { true, false, false, true };
        public Byte[]               m_au8CaptureLedPercents = { 0, 0, 0, 0, };
        public Byte[]               m_au8CaptureGains       = { 50, 50, 50, 50, };
        public Byte[]               m_au8CaptureExposures   = { 20, 20, 20, 20, };
        public Boolean[]            m_aynCaptureAutoGains   = { true, true, true, true };
        public Boolean              m_ynCaptureLedsOnOff    = false;

        // This next value indicates the first step in an auto capture sequence.
        // This essentially tell us to compare with the master values instead of the last values
        public Boolean              m_ynFirstAutoStep       = true;
        public Boolean              m_ynLastAutoGain        = false;
        public Byte                 m_u8LastGainPercent     = 0;
        public Byte                 m_u8LastExposurePercent = 0;
        public Byte                 m_u8LastLedPercent      = 0;



        ////[DllImport( "kernel32" )]
        ////public extern static int LoadLibrary( string strLibFileName );

        ////[DllImport( "kernel32" )]
        ////public extern static bool FreeLibrary( int hLibModule );


        ////public bool IsDllRegistered( )
        ////    {

        ////    int libId = LoadLibrary( "QuickUSB.dll" );

        ////    if( libId > 0 )
        ////        {
        ////        FreeLibrary( libId );
        ////        }
        ////    return ( libId > 0 );
        ////    }


        //----------------------- Q U s b C l a s s ------------------------
        //--------------------- C o n s t r u c t o r ----------------------
        public QUsbClass( )
            {
            UInt16                  u16Major;
            UInt16                  u16Minor;
            UInt16                  u16Build;
            String[]                astrQusbDevs;
            String                  strQusbDevce;
            Boolean                 ynOK;


            //ynOK = IsDllRegistered();


            ynOK = GetDllVersion( out u16Major, out u16Minor, out u16Build );
            if( ynOK == true )
                {
                m_strVersions = "DLL - " + u16Major + "." + u16Minor + "." + u16Build;
                }

            ynOK = GetDriverVersion( out u16Major, out u16Minor, out u16Build );
            if( ynOK == true )
                {
                m_strVersions += "  Driver - " + u16Major + "." + u16Minor + "." + u16Build;
                }
            else
                {
                m_strVersions += "  QUsb Driver not installed ";
                Program.Log( 6800, m_strVersions );
                return;
                }

            astrQusbDevs = FindModules();
            if( astrQusbDevs.Length == 0 )
                {
                Program.Log( 6810, "No QuickUSB Modules found" );
                // m_ynQUsbPluggedIn already set to false
                return;
                }

            strQusbDevce = astrQusbDevs[ 0 ];

            ynOK = Open( strQusbDevce );
            if( ynOK == false )
                {
                Program.Log( 4800, m_strVersions );
                Program.Log( 6810, "Unable to open Quick USB device " + strQusbDevce + " error " + LastError_str() );
                return;
                }

            ynOK = GetFirmwareVersion( out u16Major, out u16Minor, out u16Build );
            if( ynOK == true )
                {
                m_strVersions += "  Firmware - " + u16Major + "." + u16Minor + "." + u16Build;
                }
            Program.Log( 4810, m_strVersions );

            m_ynQUsbPluggedIn = true;
            Globals.ynDsetDirty = false;
            }


        //---------------- A u t o G a i n W r i t e _ y n -----------------
        //
        ///<summary>The AutoGainWrite_yn function.</summary>
        ///<param name='ynAuto'>Arg1Purpose.</param>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean AutoGainWrite_yn(
            Boolean                 ynAuto )
            {
            UInt16                  u16Value = 0;
            Boolean                 ynOK;

            m_ynLastAutoGain = ynAuto;

            if( ynAuto == true )
                {
                u16Value = 0x85;
                }
            //else - false gets no auto 0x00;
            ynOK = Globals.QUsb.CtrlRegisterWrite_yn( (ushort)COM8, u16Value );
            if( ynOK == true )
                {
                Program.Log( 2810, String.Format( "AutoGainWrite( {0} )", ynAuto.ToString() ) );
                return ynOK;
                }

            Program.Log( 4820, String.Format( "AutoGainWrite( {0} ) failed", ynAuto.ToString() ) );
            return ( ynOK );
            }
        

        //----------------------- G a i n W r i t e ------------------------
        //
        ///<summary>The GainWrite_yn function.</summary>
        ///<param name='s32Percent'>Arg1Purpose.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean GainWrite_yn(
            Int32                   s32Percent )
            {
            UInt16                  u16Value;
            Boolean                 ynOK;
            UInt16                  u16Adder;

            m_u8LastGainPercent = (Byte)s32Percent;

            // The gain value ranges from 0 - 79 which is translated to a step function
            u16Value = (UInt16)( .79f * s32Percent );

            if( s32Percent > 79 )
                {
                u16Adder = (UInt16)( ( s32Percent - 80 ) * 0.75f );   //20% yields 15, do not go over 255
                u16Value = (UInt16)(240 + u16Adder);
                }
            else if( s32Percent > 59 )
                {
                u16Adder = (UInt16)( ( s32Percent - 60 ) * 0.80f );   //20% yields 16
                u16Value = (UInt16)(112 + u16Adder);    // 127 -> 112  Lane Fix
                }
            else if( s32Percent > 39 )
                {
                u16Adder = (UInt16)( ( s32Percent - 40 ) * 0.80f );   //20% yields 16
                u16Value = (UInt16)(48 + u16Adder);
                }
            else
                {
                u16Value = (UInt16)( s32Percent * 0.80f );   // 0% to 40% gives 0 - 32 value
                }

            Program.Log( 2820, String.Format( "GainWrite({0}%) = {1}.(x{2})", s32Percent, u16Value, u16Value.ToString( "X2" ) ) );

            ynOK = CtrlRegisterWrite_yn( (ushort)0x00, u16Value );
            Thread.Sleep( 10 );
            return ( ynOK );
            }


        //--------------------- G a i n R e a d _ y n ----------------------
        //
        ///<summary>The GainRead_yn function.</summary>
        ///<param name='s32Value'>Arg1Purpose.</param>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean GainRead_yn(
            out Int32               s32Value )
            {

            UInt16                  u16Length = 1;
            UInt16                  u16SensorAddr;
            Byte[]                  au8Data;
            Boolean                 ynOK = false;

            s32Value = 0;       // in case of error return
            if( IsOpened == false )
                {
                //m_slblReady.Text = "No microscope device found";
                return false;
                }

            u16SensorAddr = (UInt16)m_s32SensorAddr;
            au8Data = new Byte[] { (Byte)0 };

            u16SensorAddr = (UInt16)( m_s32SensorAddr / 2 );  // /2
            Globals.mutxQUsb.WaitOne();
            ynOK = WriteI2C( u16SensorAddr, au8Data, u16Length );
            Globals.mutxQUsb.ReleaseMutex();
            if( ynOK == true )
                {
                Globals.mutxQUsb.WaitOne();
                ynOK = ReadI2C( u16SensorAddr, au8Data, ref u16Length );
                Globals.mutxQUsb.ReleaseMutex();
                if( ynOK == false )
                    {  // read failed
                    Program.Log( 4530, "ReadI2C( x" + u16SensorAddr.ToString( "X2" ) +
                                        ", buffer, 1) Failed - " + LastError_str() );
                    }
                }
            else  // write failed
                {
                Program.Log( 4540, "WriteI2C( x" + u16SensorAddr.ToString( "X2" ) +
                                        ", buffer, 1) Failed - " + LastError_str() );
                }

            if( ynOK == true )
                {
                s32Value = au8Data[ 0 ];
                Program.Log( 1260, "Gain Read success = x" + s32Value.ToString( "X" ) );
                }
            else
                {
                Program.Log( 4550, "Gain Read failed" );
                }

            return ( ynOK );
            }


        //---------------- E x p o s u r e W r i t e _ y n -----------------
        //
        ///<summary>The ExposureWrite_yn function.</summary>
        ///<param name='s32Percent'>Arg1Purpose.</param>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean ExposureWrite_yn(
            Int32                   s32Percent )
            {
            UInt16                  u16Exposure;
            UInt16                  u16Partial;
            Boolean                 ynOK;


            m_u8LastExposurePercent = (Byte)s32Percent;

            // covert 0 - 100 percent to 0 - 2047
            u16Exposure = (UInt16)( 655.35f * s32Percent );   // Lane mod, higher gain

            u16Partial = (UInt16)( u16Exposure & 0x00FF );
            ynOK = CtrlRegisterWrite_yn( (ushort)AEC, u16Partial );

            if( ynOK == true )
                {
                Thread.Sleep( 5 );
                u16Partial = (UInt16)( ( u16Exposure >> 8 ) & 0x00FF );
                ynOK = CtrlRegisterWrite_yn( (ushort)AECH, u16Partial );
                }

            if( ynOK == true )
                {
                Thread.Sleep( 5 );
                Program.Log( 2830, String.Format( "ExposureWrite( {0}%) = {1}.(x{2})", s32Percent, u16Exposure, u16Exposure.ToString( "X2" ) ) );
                return ynOK;
                }

            Program.Log( 4870, String.Format( "ExposureWrite() failed" ) );
            return ynOK;
            }


        //------------- E t a l u m a B o a r d I n i t _ y n --------------
        //
        ///<summary>The EtalumaBoardInit_yn function.</summary>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean EtalumaBoardInit_yn( )
            {

            Boolean                 ynOK;
            Byte[]                  au8Buff = { 0, 0, 0, 0 };
            UInt16                  u16Len = 1;
      //    UInt16                  u16Adr = 0;
            UInt16                  u16Value = 0;
            Byte                    u8Mask;
            String                  strPathFile;
            String                  strSelect;
            ListDset.tblDevicesRow[] arowsDevs;
            ListDset.tblDevicesRow  rowDev;

            if( IsOpened == false )
                {
                Program.Log( 4830, "No microscope device found" );
                return false;
                }

            // First we will fetch the device specific data
            strSelect = "s32SensorID=" + m_s32SensorID.ToString();
            arowsDevs = (ListDset.tblDevicesRow[])Globals.dsetList.tblDevices.Select( strSelect );
            if( arowsDevs.Length < 1 )
                {
                Program.Log( 4840, "Device DB select failed to return rows for - " + strSelect );
                return false;
                }

            rowDev = arowsDevs[ 0 ];
            m_s32SensorAddr = rowDev.s32SensorAddress;


            // the default for m_eSensorCom is already "I2C"
            if( rowDev.IstxtInterfaceNull() == false )
                {
                strSelect = rowDev.txtInterface.ToUpper();
                if( strSelect.IndexOf( "PORT" ) > -1 )  // is a "PORT" somewhere in the String
                    {
                    m_eSensorCom = E_SENSOR_COM.PORT_A;
                    }
                }

            // the default for m_eClockSpeed is already 24MHz
            if( rowDev.IstxtClockNull() == false )
                {
                strSelect = rowDev.txtClock;
                if( strSelect.IndexOf( "12" ) > -1 )  // is a "12" somewhere in the String
                    {
                    m_eClockSpeed = E_CLOCK_SPEED.SPEED12MHz;
                    }
                }

            m_s32ImageHeight = rowDev.s32Height;
            m_s32ImageWidth = rowDev.s32Width;
            m_s32ImageDepth = rowDev.s32Depth / 8;  // in bytes you know

            m_s32BmapPixelCount = m_s32ImageWidth * m_s32ImageHeight;

            ////switch( m_s32ImageDepth ) // Depth in bytes per pixel
            ////    {
            ////    case 1:
            ////        Globals.ePixmapFormat = PixelFormat.Format8bppIndexed;
            ////        break;
            ////    case 2:
            ////        Globals.ePixmapFormat = PixelFormat.Format16bppRgb555;
            ////        break;
            ////    case 3:
            ////        Globals.ePixmapFormat = PixelFormat.Format24bppRgb;
            ////        break;
            ////    case 4:
            ////        Globals.ePixmapFormat = PixelFormat.Format32bppArgb;
            ////        break;
            ////    }


            ynOK = ReadSetting( QuickUsb.Setting.CpuConfig, out u16Value );
            if( ynOK == false )
                {
                Program.Log( 4850, "Quick Usb read setting failed - " + LastError_str() );
                return false; // ynOK;
                }
            u16Value = (UInt16)( u16Value & ~0x18 );

            switch( m_eClockSpeed )
                {
                case E_CLOCK_SPEED.SPEED12MHz:
                    break;
                case E_CLOCK_SPEED.SPEED24MHz:
                    u16Value = (UInt16)( u16Value | 0x08 );
                    break;
                }
            ynOK = WriteSetting( QuickUsb.Setting.CpuConfig, u16Value );

            ynOK = WriteSetting( QuickUsb.Setting.WordWide, 0 );

            ynOK = SetTimeout( (uint)Properties.Settings.Default.s32QUsbTimeout );

            Program.Log( 4850, "Initializing FPGA" );

            // Program FPGA (Comment out if using adapter board)
            strPathFile = Globals.strAppPath + @"\ILAB_EVB.rbf";
            if( File.Exists( strPathFile ) == false )
                {
                Program.Log( 6830, "Could not open FPGA initializing file - " + strPathFile );
                return false;
                }

            ynOK = ConfigureFpga( strPathFile );
            if( ynOK == false )
                {
                Program.Log( 6840, "Failed to load FPGA." );
                return false;
                }
            Program.Log( 4860, "Loaded FPGA." );

            ////ynOK = FpgaUpload_yn( Globals.strAppPath + @"\ILAB_EVB.rbf", fpgaProgress );
            //ynOK = UploadFpga( strPathFile, fpgaProgress );
            //if( ynOK == false )
            //    {
            //    Program.Log( 6850, "Failed to load FPGAa.");
            //    return false;
            //    }

            //Hardware startup sequence v1.0 begin.
            // 1) clear the following port pins:
            //  D.0 (DOVDD)  Digital IO power, 3.3V
            //  D.1 (AVDD)   Analog power, 3.3V
            //  D.2 (DVDD)   Digital core power, 1.5V
            //  D.3 (\RESET) Active low hardware reset
            //  D.4 (TM)     Test Mode, makes a test pattern video output
            //  D.5 (PWDN)   Power Down
            u8Mask = 0x3F;    //0011 1111  1 = output
            if( WritePortDir( QuickUsb.Port.D, u8Mask ) == false )
                {  // we will instrument the first occurrence of each call type
                Program.Log( 6860, "WritePortDir failed." );
                return false;
                }
            Thread.Sleep( 10 );
            // 2) set D.5, set D.3       Power down on, reset off.
            // 3) Wait more than 0 ms.
            au8Buff[ 0 ] = 0x28;   // Set D.5, D.3

            if( WritePort( QuickUsb.Port.D, au8Buff, u16Len ) == false )
                {
                Program.Log( 6870, "WritePort failed." );
                return false;
                }
            Thread.Sleep( 10 );

            // 4) set D.0                Digital IO power on.
            // 5) Wait more than 0 ms.
            au8Buff[ 0 ] |= 0x01;  // Set D.0
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );


            // 6) Set D.1                Analog power on.
            au8Buff[ 0 ] |= 0x02;  //Set D.1
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );

            // 7) Write to register 0x63. Set bit 2.
            //  Disable internal 1.5V digital core power regulator.
            //  I don't know the default value of the other bits in this register.
            //  Try a read/modify/write sequence to avoid altering other bits.
            //  I'll ask Omnivision for the default value for the whole register.
            ynOK = CtrlRegisterRead_yn( 0x63, out u16Value );
            ////if( CtrlRegisterRead_yn( 0x63, out u16Value ) == false )
            ////    {
            ////    Program.Log( 6880, "ReadSetting failed" );
            ////    //return false;
            ////    }
            if( ynOK == true )  // We will only do the write if the read was a success
                {
                Thread.Sleep( 10 );
                u16Value |= 0x04;
                if( CtrlRegisterWrite_yn( 0x63, u16Value ) == false )
                    {
                    Program.Log( 6890, "WriteSetting failed" );
                    // return false;
                    }
                }
            Thread.Sleep( 10 );

            // 8) Set D.2                Digital core power on.
            au8Buff[ 0 ] |= 0x04;   // Set D.2
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );

            // 9) Clear D.5              Power down off.
            // 10) Wait more than 1ms.
            au8Buff[ 0 ] &= (Byte)0xDF;  // Clear D.5
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );

            // 11) Clear D.3.            Reset on.
            // 12) Wait more than 1ms.
            au8Buff[ 0 ] &= (Byte)0xF7;  // Clear D.3
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );

            // 13) Set D.3.              Reset off.
            // Hardware startup sequence end.
            au8Buff[ 0 ] |= 0x08;  // Set D.3  Reset Off
            ynOK = WritePort( QuickUsb.Port.D, au8Buff, u16Len );
            Thread.Sleep( 10 );

            au8Buff[ 0 ] = 0x28;   // Set D.5, D.3

            //ynOK = PortReadDir_yn( u16PortID, out u8Mask );

            // Almost all outputs
            u8Mask = 0x3F;    //0011 1111  1 = output
            ynOK = WritePortDir( QuickUsb.Port.D, u8Mask );
            Thread.Sleep( 10 );

            // If it is the new device which requires the start up sequence
            if( Properties.Settings.Default.ynQUsbRegistersInit == true )
                {
                SensorRegistersLoadFromDB_yn();
                }

            //Now we find out if it is a model 500 or 600
            u16Len = 1;


                   // Init the Led controls to off
                    // The port A direction bits need to be set to 0x03 -- 0x02 = bright, 0x01 exciter
                ynOK = WritePortDir( QuickUsb.Port.A, 0x03 );
                Thread.Sleep( 2 );

                m_au8PortOutShadow[ 0 ] = 0x03;  //turn both on
                au8Buff[ 0 ] = m_au8PortOutShadow[ 0 ];
                u16Len = 1;

                ynOK = WritePort( QuickUsb.Port.A, au8Buff, u16Len );
                Thread.Sleep( 2 );

            // Essentially the hardware initialization is complete
            // Now deal with the last operational values

            // Load Led an Gain/Exposure values from persistent memory
            CaptureValuesFromString( Properties.Settings.Default.strCaptureValues );

            // First lets load our master values
            // Master values get their name because these are the values that the user
            // sets during his operations and are essentially the start up values.
            // These are typically not the values used for individual leds in timed 
            // collection series.
            MasterValuesFromString( Properties.Settings.Default.strMasterValues );

            if( MasterValuesLoad_yn() == false )
                {
                return false;
                }

            return true;
            }


        public void ShutDown()
            {
            Properties.Settings.Default.strMasterValues = MasterValuesToString_str();
            Properties.Settings.Default.strCaptureValues = CaptureValuesToString_str();
            LedsOff();
            }

        //------------------------ L e d s I n i t -------------------------
        //
        ///<summary>The LedsInit function.</summary>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void LedsInit()
            {

            LedOnOffSet( E_LED.F1,    m_aynMasterLedsActive[ F1_IX ] );
            LedOnOffSet( E_LED.WHITE, m_aynMasterLedsActive[ WHITE_IX ] );
            }


        //------------------------- L e d s O f f --------------------------
        //
        ///<summary>The LedsOff function.</summary>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void LedsOff()
            {
            LedOnOffSet( E_LED.F1,    false );
            LedOnOffSet( E_LED.WHITE, false );
            }


        //------------ M a s t e r V a l u e s P e r s e r v e -------------
        //
        ///<summary>The MasterValuesPerserve function.</summary>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void MasterValuesPerserve()
            {

            m_strMasterValuesPreserved = MasterValuesToString_str();
            }


        //------------- M a s t e r V a l u e s R e s t o r e --------------
        //
        ///<summary>The MasterValuesRestore function.</summary>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void MasterValuesRestore()
            {

            MasterValuesFromString( m_strMasterValuesPreserved );
            MasterValuesLoad_yn();
            }

        //---------- M a s t e r V a l u e s F r o m S t r i n g -----------
        //
        ///<summary>The MasterValuesFromString function.</summary>
        ///<param name='strMasterValues'>Arg1Purpose.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        private void MasterValuesFromString(
            String                  strMasterValues )
            {
            String[]                astrValues;
            Byte                    u8Value;

            Program.Log( 2980, "M500 master values from string - " + strMasterValues );
            astrValues = strMasterValues.Split( ',' );

            // u8LedsActive[4], LedsPercent[4], GainsPercent[4], LedsOnOff, ExposurePercent[4], AutoGain[4]
            //  0, 1, 2, 3,       4, 5, 6, 7,    8, 9, 10, 11,       12,    13, 14, 15, 16,    17, 18, 19, 20

            m_aynMasterLedsActive[ F1_IX ] = ( astrValues[ 0 ] == "1" ? true : false );
            m_aynMasterLedsActive[ WHITE_IX ] = ( astrValues[ 3 ] == "1" ? true : false );

            if( byte.TryParse( astrValues[ 4 ], out u8Value ) == false )
                {
                u8Value = 0;
                }
            m_au8MasterLedPercents[ F1_IX ] = u8Value;

            //Now the Gain percentages
            if( byte.TryParse( astrValues[ 8 ], out u8Value ) == false )
                {
                u8Value = 0;
                }
            m_u8MasterGainPercent = u8Value;

            //ExposurePercent[4], 
            if( byte.TryParse( astrValues[ 13 ], out u8Value ) == false )
                {
                u8Value = 0;
                }
            m_u8MasterExposurePercent = u8Value;

            //AutoGain[4]
            m_ynMasterAutoGain = ( astrValues[ 17 ] == "1" ? true : false );
            }


        //-------- M a s t e r V a l u e s T o S t r i n g _ s t r ---------
        //
        ///<summary>The MasterValuesToString_str function.</summary>
        ///<returns>String.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public String MasterValuesToString_str()
            {
            String          strMasterValues = String.Empty;

            // u8LedsActive[4], LedsPercent[4], GainsPercent[4], LedsOnOff, ExposurePercent[4], AutoGain[4]
            //  0, 1, 2, 3,       4, 5, 6, 7,    8, 9, 10, 11,       12,    13, 14, 15, 16,    17, 18, 19, 20

            strMasterValues = m_aynMasterLedsActive[ 0 ] == true ? "1," : "0,";
            strMasterValues += "0,0,";
            strMasterValues += m_aynMasterLedsActive[ 3 ] == true ? "1," : "0,";

            strMasterValues += m_au8MasterLedPercents[ 0 ].ToString() + ",";
            strMasterValues += "0,0,0,";

            strMasterValues += m_u8MasterGainPercent.ToString() + ",";
            strMasterValues += "0,0,0,";

            strMasterValues += m_u8MasterExposurePercent.ToString() + ",";
            strMasterValues += "0,0,0,";

            strMasterValues += "0,";  // dummy for leds on/off

            strMasterValues += m_ynMasterAutoGain == true ? "1," : "0,";
            strMasterValues += "0,0,0,";

            Program.Log( 2985, "M500 master Values string - " + strMasterValues );
            return ( strMasterValues );
            }


        //------- C a p t u r e V a l u e s T o S t r i n g _ s t r --------
        //
        ///<summary>The CaptureValuesToString_str function.</summary>
        ///<returns>String.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public String CaptureValuesToString_str()
            {
            String                  strCaptureValues    = String.Empty;

            // u8LedsActive[4],LedsPercent[4],GainsPercent[4], LedsOnOff, ExposurePercent[4], AutoGain[4]
            //  0, 1, 2, 3,      4, 5, 6, 7,   8, 9, 10, 11,       12,    13, 14, 15, 16,    17, 18, 19, 20

            strCaptureValues += m_aynCaptureLedsActive[ F1_IX ] == true ? "1," : "0,";
            strCaptureValues += "0,0,";    // for F2,F3
            strCaptureValues += m_aynCaptureLedsActive[ WHITE_IX ] == true ? "1," : "0,";

            strCaptureValues += m_au8CaptureLedPercents[ F1_IX ].ToString() + ",";
            strCaptureValues += "0,0,0,";    // for F2,F3,White
            
            strCaptureValues += m_au8CaptureGains[ F1_IX ].ToString() + ",";
            strCaptureValues += "0,0,";    // for F2,F3
            strCaptureValues += m_au8CaptureGains[ WHITE_IX ].ToString() + ",";

            strCaptureValues += m_ynCaptureLedsOnOff == true ? "1," : "0,";

            strCaptureValues += m_au8CaptureExposures[ F1_IX ].ToString() + ",";
            strCaptureValues += "0,0,";    // for F2,F3
            strCaptureValues += m_au8CaptureExposures[ WHITE_IX ].ToString() + ",";

            strCaptureValues += m_aynCaptureAutoGains[ F1_IX ] == true ? "1," : "0,";
            strCaptureValues += "0,0,";    // for F2,F3
            strCaptureValues += m_aynCaptureAutoGains[ WHITE_IX ] == true ? "1," : "0,";

            Program.Log( 2990, "M500 capture Values string - " + strCaptureValues );
            return strCaptureValues;
            }


        //--------- C a p t u r e V a l u e s F r o m S t r i n g ----------
        //
        ///<summary>The CaptureValuesFromString function.</summary>
        ///<param name='strCaptureValues'>Arg1Purpose.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void CaptureValuesFromString(
            String                  strCaptureValues )
            {
            String[]                astrValues;
            Byte                    u8Value;
 
            Program.Log( 2995, "M500 capture values from string - " + strCaptureValues );
            astrValues = strCaptureValues.Split( ',' );

            // u8LedsActive[4],LedsPercent[4],GainsPercent[4], LedsOnOff, ExposurePercent[4], AutoGain[4]
            //  0, 1, 2, 3,      4, 5, 6, 7,   8, 9, 10, 11,       12,    13, 14, 15, 16,    17, 18, 19, 20

            m_aynCaptureLedsActive[ F1_IX ]    = ( astrValues[ 0 ] == "1" ? true : false);
            m_aynCaptureLedsActive[ WHITE_IX ] = ( astrValues[ 3 ] == "1" ? true : false );

            if( byte.TryParse( astrValues[ 4 ], out u8Value ) == false )
                {
                u8Value = 0;
                }
            m_au8CaptureLedPercents[ F1_IX  ] = u8Value;


            //Now the Gain percentages
            if( byte.TryParse( astrValues[ 8 ], out u8Value ) == false )
                   {
                   u8Value = 0;
                   }
            m_au8CaptureGains[ F1_IX ] = u8Value;
            if( byte.TryParse( astrValues[ 11 ], out u8Value ) == false )
               {
               u8Value = 0;
               }
            m_au8CaptureGains[ WHITE_IX ] = u8Value;

            //LedsOnOff, 
            m_ynCaptureLedsOnOff = ( astrValues[ 12 ] == "1" ? true : false );

            //ExposurePercent[4], 
            if( byte.TryParse( astrValues[ 13 ], out u8Value ) == false )
               {
               u8Value = 0;
               }
            m_au8CaptureExposures[ F1_IX ] = u8Value;

            if( byte.TryParse( astrValues[ 16 ], out u8Value ) == false )
               {
               u8Value = 0;
               }
            m_au8CaptureExposures[ WHITE_IX ] = u8Value;
            
            //AutoGain[4]
            m_aynCaptureAutoGains[ F1_IX ]    = ( astrValues[ 17 ] == "1" ? true : false);
            m_aynCaptureAutoGains[ WHITE_IX ] = ( astrValues[ 20 ] == "1" ? true : false );

            }



        //------------- M a s t e r V a l u e s L o a d _ y n --------------
        //
        ///<summary>The MasterValuesLoad_yn function.</summary>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean MasterValuesLoad_yn()
            {
            if( AutoGainWrite_yn( m_ynMasterAutoGain ) == false )
                {
                return false;
                }

            if( m_ynMasterAutoGain == false ) // false -- manual control
                {
                if( GainWrite_yn( m_u8MasterGainPercent ) == false )
                    {
                    return false;
                    }

                if( ExposureWrite_yn( m_u8MasterExposurePercent ) == false )
                    {
                    return false;
                    }
                }

            LedsInit();

            return true;
            }

        //----- C a p t u r e E x p o s u r e A n d G a i n W r i t e ------
        //
        ///<summary>The CaptureExposureAndGainWrite function.</summary>
        ///<param name='eLed'>Arg1Purpose.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void CaptureExposureAndGainWrite(
            E_LED                   eLed )
            {
            Int32                   s32LedIX = (Int32)F1_IX;  // the default value
            Boolean                 ynAutoGain;

            // m_ynFirstAutoStep, m_ynLastAutoGain, m_u8LastGainPercent, m_u8LastExposurePercent
            switch( eLed )
                {
                case E_LED.F2:      s32LedIX = (Int32)F2_IX;        break;
                case E_LED.F3:      s32LedIX = (Int32)F3_IX;        break;
                case E_LED.WHITE:   s32LedIX = (Int32)WHITE_IX;     break;
                    default:                                        break;
                }

            ynAutoGain = m_aynCaptureAutoGains[ s32LedIX ];
            if( ynAutoGain == true )
                {
                if( m_ynLastAutoGain == true )
                    {
                    return;  // There is nothing to do here, want auto was auto
                    }

                AutoGainWrite_yn( true );
                return;
                }


            if( ynAutoGain == false )
                {
                if( m_ynLastAutoGain == true )
                    {
                    AutoGainWrite_yn( false );
                    }

                // A transition from true to false requires a gain and exposure write
                GainWrite_yn( m_au8CaptureGains[ s32LedIX ] );
                ExposureWrite_yn( m_au8CaptureExposures[ s32LedIX ] );
                return;
                }

            // here ynAutoGain was false we only have to load values that are different
            if( m_u8LastGainPercent != m_au8CaptureGains[ s32LedIX ] )
                {
                GainWrite_yn( m_au8CaptureLedPercents[ s32LedIX ] );
                }

            if( m_u8LastExposurePercent != m_au8CaptureExposures[ s32LedIX ] )
                {
                ExposureWrite_yn( m_au8CaptureExposures[ s32LedIX ] );
                }
            }


        //------------ M 5 0 0 F l u o r e s c e n t W r i t e -------------
        //
        ///<summary>The M500FluorescentWrite function.</summary>
        ///<param name='u8Percent'>Arg1Purpose.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void M500FluorescentWrite(
            Byte                    u8Percent )
            {
            Byte[]                  au8Vals         = { 0, 0 };

            UInt16                  u16Len          = 1;
            Boolean                 ynOK;
            UInt16                  u16Adr          = 0x09;
            Single                  sfpPercent      = 2.548f;
            Byte                    u8Value;

            if( u8Percent == 0 )
                {
                m_au8PortOutShadow[ 0 ] = (Byte)( m_au8PortOutShadow[ 0 ] & 0xFE );  // Off the lowest bit off
                au8Vals[ 0 ] = m_au8PortOutShadow[ 0 ];
                ynOK = WritePort( Port.A, au8Vals, u16Len );
                Program.Log( 1810, "M500 F1 Port.A Off Write" );
                return;
                }

            // It is something other than zero
            if( ( m_au8PortOutShadow[ 0 ] & 0x01 ) == 0 )  // we need to turn on the led
                {
                m_au8PortOutShadow[ 0 ] |= 0x01;
                au8Vals[ 0 ] = m_au8PortOutShadow[ 0 ];

                ynOK = WritePort( Port.A, au8Vals, u16Len );
                Program.Log( 1820, "M500 F1 Port.A On Write" );
                Thread.Sleep( 2 );
                }

            u8Value = (Byte)( u8Percent * sfpPercent );    // covert to binary magnitude
            Program.Log( 1830, String.Format( "M500 F1 I2C Write( {0}% = {1}.(x{2}) )", u8Percent, u8Value, u8Value.ToString( "X2" ) ) );

            au8Vals[ 0 ] = (Byte)( ( u8Value >> 4 ) & 0x0F );
            au8Vals[ 1 ] = (Byte)( ( u8Value << 4 ) & 0xF0 );
            u16Len = 2;
            u16Adr = 0x09;
            ynOK = WriteI2C( u16Adr, au8Vals, u16Len );
            }



        //-------------- L e d A c t i v e S t r i n g S e t ---------------
        //
        ///<summary>The LedActiveStringSet function.</summary>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        private void LedActiveStringSet()
            {
            m_strLedsActive = String.Empty;

            if( m_aynMasterLedsActive[ QUsbClass.F1_IX ] == true )
                {
                m_strLedsActive += "F1";
                }
            if( m_aynMasterLedsActive[ QUsbClass.F2_IX ] == true )
                {
                m_strLedsActive += "F2";
                }
            if( m_aynMasterLedsActive [ QUsbClass.F3_IX ] == true )
                {
                m_strLedsActive += "F3";
                }
            if( m_aynMasterLedsActive[ QUsbClass.WHITE_IX ] == true )
                {
                m_strLedsActive += "W";
                }
            }

        //--------------------- L e d O n O f f S e t ----------------------
        //
        ///<summary>Handles the LedOnOffSet event.</summary>
        ///<param name='eLed'>Object sending the event.</param>
        ///<param name='ynOn'>Event arguments.</param>
        ///<returns>void.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public void LedOnOffSet(
            E_LED                   eLed,
            Boolean                 ynOn )
            {
            UInt16                  u16Len = 1;
            Boolean                 ynOK;
            Byte                    u8Percent = 0;

            if( eLed == E_LED.F1 )
                {
                if( ynOn == true )
                    {
                    u8Percent = m_au8MasterLedPercents[ F1_IX ];
                    m_aynMasterLedsActive[ F1_IX ] = true;
                    }
                else // it is off
                    {
                    m_aynMasterLedsActive[ F1_IX ] = false;
                    }

                M500FluorescentWrite( u8Percent );
                }

            else // that leave only if( eLed == E_LED.WHITE )
                {
                if( ynOn == false )
                    {
                    m_aynMasterLedsActive[ WHITE_IX ] = false;
                    m_au8PortOutShadow[ 0 ] = (Byte)( m_au8PortOutShadow[ 0 ] & 0xFD );
                    Program.Log( 1840,  "M500 Port.A Write White Led OFF" );
                    }
                else // turn it on
                    {
                    m_aynMasterLedsActive[ WHITE_IX ] = true;
                    m_au8PortOutShadow[ 0 ] = (Byte)( m_au8PortOutShadow[ 0 ] | 0x02 );
                    Program.Log( 1950, "M500 Port.A Write White Led ON" );
                    }

                ynOK = WritePort( Port.A, m_au8PortOutShadow, u16Len );
               }

           LedActiveStringSet();
            }

 
        //---- S e n s o r R e g i s t e r s L o a d F r o m D B _ y n -----
        //
        ///<summary>The SensorRegistersLoadFromDB_yn function.</summary>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        private Boolean SensorRegistersLoadFromDB_yn()
            {
            Int32                   s32IX = 0;
            String                  strMsg;
            String                  strSelect;
            String[]                astrReset;
            String                  strSort = "s32InitID";
            Byte[]                  au8Hexs = new Byte[] { 0, 0, 0 };
            Boolean                 ynOK;
            ListDset.tblRegistersRow rowReg;
            ListDset.tblRegistersRow[] arowsReg;
            ListDset.tblDevicesRow[] arowsDevs;
            ListDset.tblDevicesRow  rowDev;

            // First we will fetch the device specific data
            strSelect = "s32SensorID=" + m_s32SensorID.ToString();
            arowsDevs = (ListDset.tblDevicesRow[])Globals.dsetList.tblDevices.Select( strSelect );
            if( arowsDevs.Length < 1 )
                {
                Program.Log( 4580, "Usb tblDevice.select failed to return rows for - " + strSelect );
                return false;
                }

            rowDev = arowsDevs[ 0 ];
            // txtClock, txtDescription, txtInterface, s32Depth

            m_s32ImageWidth = rowDev.s32Width;
            m_s32ImageHeight = rowDev.s32Height;
            strMsg = "Width = " + rowDev.s32Width.ToString() + " x Height = " + rowDev.s32Height.ToString() + "  " +
                        rowDev.txtDescription;
            Program.Log( 4880, strMsg );

            strMsg = String.Empty;
            if( rowDev.IsNull( "txtInitSequence" ) == false )
                {
                strMsg = rowDev.txtInitSequence;
                Program.Log( 4890, "txtInitSequence in Device table is null" );
                //return false;
                }

            astrReset = strMsg.Split( ' ' );
            ynOK = true;
            for( s32IX = 0; s32IX < 2 && ynOK == true; s32IX++ )
                {
                ynOK = Byte.TryParse(
                            astrReset[ s32IX ],        // String to convert
                            NumberStyles.HexNumber,     // bitwise style
                            null,                       // FormatProvider
                            out au8Hexs[ s32IX ] );     // result
                }

            if( ynOK == true )
                {
                // Send the reset to the sensor
                CtrlRegisterWrite_yn( au8Hexs[ 0 ], au8Hexs[ 1 ] );
                strMsg = String.Format( "DeviceID(0x{0}) - Reseting control registers - [0x{1}] = 0x{2}",
                                        m_s32SensorID.ToString( "X2" ),
                                        au8Hexs[ 0 ].ToString( "X2" ),
                                        au8Hexs[ 1 ].ToString( "X2" ) );
                Program.Log( 4900, strMsg );
                }
            else
                {
                Program.Log(4910, "No device init sequence - defined" );
                }


            strSelect = "s32SensorID=" + m_s32SensorID.ToString() + " and s32InitID is not Null";

            // Now the register list data
            arowsReg = (ListDset.tblRegistersRow[])Globals.dsetList.tblRegisters.Select( strSelect, strSort );
            if( arowsReg.Length < 1 )
                {
                return false;
                }
            s32IX = 0;

            // we will iterate through the rows initing registers to the right value
            while( s32IX < arowsReg.Length )
                {
                rowReg = arowsReg[ s32IX++ ];
                // First we will check the last set value
                if( rowReg.IsNull( "s32Value" ) == true )
                    {
                    if( rowReg.IsNull( "s32Default" ) == true )
                        {
                        continue;
                        }
                    else // we have a default value that was never set
                        {
                        rowReg.BeginEdit();
                        rowReg.s32Value = rowReg.s32Default;
                        rowReg.EndEdit();

                        Globals.ynDsetDirty = true;
                        }
                    }
                if( CtrlRegisterWrite_yn( (UInt16)rowReg.s32Index, (UInt16)rowReg.s32Value ) == false )
                    {
                    strMsg = String.Format( "DeviceID(0x{0}) Control Register[0x{1}] = 0x{2}  {3} FAILED - {4}",
                       m_s32SensorID.ToString( "X2" ),
                       rowReg.s32Index.ToString( "X2" ),
                       rowReg.s32Value.ToString( "X2" ),
                       rowReg.txtLabel,
                       LastError_str() );
                    Program.Log( 680, strMsg );
                    return false;
                    }

                strMsg = String.Format( "DeviceID(0x{0}) Control Register[0x{1}] = 0x{2}  {3}",
                       m_s32SensorID.ToString( "X2" ),
                       rowReg.s32Index.ToString( "X2" ),
                       rowReg.s32Value.ToString( "X2" ),
                       rowReg.txtLabel );
                Program.Log( 4820, strMsg );
                }  // Register init loop

            m_ynDeviceInited = true;
            return true;
            }


        //------------------- L a s t E r r o r _ s t r --------------------
        //
        ///<summary>The LastError_str function.</summary>
        ///<returns>String.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public String LastError_str()
            {
            switch( LastError() )
                {
                case QuickUsb.Error.NoError:                return "No Error";
                case QuickUsb.Error.OutOfMemory:            return "Out Of Memory";
                case QuickUsb.Error.CannotOpenModule:       return "Cannot Open Module";
                case QuickUsb.Error.CannotFindDevice:       return "Cannot Find Device";
                case QuickUsb.Error.IoctlFailed:            return "Ioctl Failed";
                case QuickUsb.Error.InvalidParameter:       return "Invalid Parameter";
                case QuickUsb.Error.Timeout:                return "Timeout";
                case QuickUsb.Error.FunctionNotSupported:   return "Function Not Supported";
                case QuickUsb.Error.I2cBusError:            return "I2C Bus Error";
                case QuickUsb.Error.I2cNoAck:               return "I2C No Ack";
                case QuickUsb.Error.I2cSlaveWait:           return "I2C Slave Wait";
                case QuickUsb.Error.I2cTimeout:             return "I2C Timeout";
                case QuickUsb.Error.Aborted:                return "Aborted";
                case QuickUsb.Error.AlreadyCompleted:       return "Already Completed";
                case QuickUsb.Error.AlreadyOpened:          return "Already Opened";
                case QuickUsb.Error.CannotCloseModule:      return "Cannot Close Module";
                case QuickUsb.Error.CannotOpenFile:         return "Cannot Open File";
                case QuickUsb.Error.Deprecated:             return "Deprecated";
                case QuickUsb.Error.EpcsNotFound:           return "Epcs Not Found";
                case QuickUsb.Error.EpcsTooSmall:           return "Epcs Too Small";
                case QuickUsb.Error.VerifyFailed:           return "Verify Failed";
                case QuickUsb.Error.UnknownDriverType:      return "Unknown Driver Type";
                case QuickUsb.Error.FpgaInitFailed:         return "Fpga Init Failed";
                case QuickUsb.Error.PacketNotMultiple512:   return "Packet Not Multiple 512";
                case QuickUsb.Error.PacketNotMultiple64:    return "Packet Not Multiple 64";
                case QuickUsb.Error.UnknownSystemError:     return "Unknown System Error";
                case QuickUsb.Error.InvalidSerial:          return "Invalid Serial";
                case QuickUsb.Error.FirmwareError:          return "Firmware Error";
                case QuickUsb.Error.NotCompleted:           return "Not Completed";
                case QuickUsb.Error.FpgaConfigFailed:       return "Fpga Config Failed";
                case QuickUsb.Error.InvalidOperation:       return "Invalid Operation";
                case QuickUsb.Error.TooManyRequests:        return "Too Many Requests";
                case QuickUsb.Error.NotStreaming:           return "Not Streaming";
                default:                                    return "Unknown Error";
                }
            }


        //------------- C t r l R e g i s t e r R e a d _ y n --------------
        //
        ///<summary>The CtrlRegisterRead_yn function.</summary>
        ///<param name='u16DevAddr'>Object sending the event.</param>
        ///<param name='u16Value'>Event arguments.</param>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean CtrlRegisterRead_yn(
            UInt16                  u16DevAddr,
            out UInt16              u16Value )
            {

            UInt16                  u16Length           = 1;
            UInt16                  u16SensorAddr;
            Byte[]                  au8Data;
            Byte                    u8Value             = 0; // in case of error return
            Boolean                 ynOK                = false;

            u16Value = 0;
            if( IsOpened == false )
                {
                Program.Log( 6120, "CtrlRegisterRead_yn No microscope device found" );
                return false;
                }

            u16SensorAddr = (UInt16)m_s32SensorAddr;
            au8Data = new Byte[] { (Byte)u16DevAddr };
            switch( m_eSensorCom )
                {
                case E_SENSOR_COM.I2C:
                    u16SensorAddr = (UInt16)( u16SensorAddr / 2 );

                    Globals.mutxQUsb.WaitOne();
                    ynOK = WriteI2C( u16SensorAddr,
                                                     au8Data,
                                                     u16Length );
                    ////ynOK = CachedWriteI2C( u16SensorAddr,
                    ////                                 au8Data,
                    ////                                 u16Length );
                    if( ynOK == true )
                        {
                        ynOK = ReadI2C( u16SensorAddr,
                                                        au8Data,
                                                        ref u16Length );
                        Globals.mutxQUsb.ReleaseMutex();
                        if( ynOK == false )
                            {  // read failed
                            Program.Log( 4920, "ReadI2C( 0x" + u16DevAddr.ToString( "X2" ) +
                                                ", buffer, 1) Failed - " + LastError_str() );
                            }
                        }
                    else  // write failed
                        {
                        Globals.mutxQUsb.ReleaseMutex();
                        Program.Log( 6900, "WriteI2C( 0x" + u16DevAddr.ToString( "X2" ) +
                                                ", buffer, 1) Failed - " + LastError_str() );
                        }
                    break;

                case E_SENSOR_COM.PORT_A:
                    u8Value = (Byte)( u16DevAddr + 128 );
                    Globals.mutxQUsb.WaitOne();
                    ynOK = ReadSPI( u8Value,
                                                      au8Data,
                                                     ref u16Length );
                    Globals.mutxQUsb.ReleaseMutex();
                    if( ynOK == false )
                        {
                        Program.Log( 4930, "ReadSPI( " + u8Value.ToString( "X2" ) +
                                                ", buffer, 1 ) Failed - " + LastError_str() );
                        }

                    break;

                default:
                    break;
                }

            if( ynOK == true )
                {
                u16Value = au8Data[ 0 ];
                }

            return ( ynOK );
            }


        //------------ C t r l R e g i s t e r W r i t e _ y n -------------
        //
        ///<summary>The CtrlRegisterWrite_yn function.</summary>
        ///<param name='u16DevAddr'>Object sending the event.</param>
        ///<param name='u16Value'>Event arguments.</param>
        ///<returns>Boolean.</returns>
        ///<remarks>Narrative.</remarks>
        //------------------------------------------------------------------
        public Boolean CtrlRegisterWrite_yn(
            UInt16                  u16DevAddr,
            UInt16                  u16Value )
            {

            Byte[]                  au8Buffer;
            UInt16                  u16SensorAddr;
            Boolean                 ynOK = false;

            if( IsOpened == false )
                {
                // m_slblReady.Text = "No microscope device found";
                return false;
                }

            switch( m_eSensorCom )
                {
                case E_SENSOR_COM.I2C:
                    au8Buffer = new Byte[] { (Byte)u16DevAddr, (Byte)u16Value };
                    u16SensorAddr = (UInt16)( m_s32SensorAddr / 2 );
                    // x60(96.) becomes x30(48.)

                    Globals.mutxQUsb.WaitOne();
                    ynOK = WriteI2C( u16SensorAddr,
                                    au8Buffer,
                                    2 );
                    Globals.mutxQUsb.ReleaseMutex();
                    if( ynOK == true )
                        {
                        Program.Log( 1800, "I2CWrite_yn( ).., reg(x" + u16DevAddr.ToString( "X" ) +
                                                 ") = x" + u16Value.ToString( "X" ) );
                        }
                    else
                        {
                        Program.Log( 4940, "I2CWrite_yn( .., " + u16Value.ToString() +
                                  " ) failed - " + LastError_str() );
                        }

                    break;

                case E_SENSOR_COM.PORT_A:
                    au8Buffer = new Byte[] { (Byte)u16DevAddr };

                    Globals.mutxQUsb.WaitOne();
                    ynOK = WriteSPI( (Byte)( m_s32SensorAddr + 128 ),
                                                au8Buffer,
                                                1 );
                    Globals.mutxQUsb.ReleaseMutex();

                    if( ynOK == true )
                        {
                        Program.Log( 1810, "WriteSPI( ).., reg(x" + u16DevAddr.ToString( "X" ) +
                                                 ") = x" + u16Value.ToString( "X" ) );
                        }
                    else
                        {
                        Program.Log( 4950, "WriteSPI( .., " + u16Value.ToString() +
                                  " ) failed - " + LastError_str() );
                        }
                    break;

                default:
                    break;
                }

            return ( ynOK );
            }
        }
    }

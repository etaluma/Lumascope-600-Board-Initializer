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
// This code instantiates a 'VideoParameters' object with default parameters.
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


using LumaUSB_ns;


namespace LumaScopeInitialization
{
    /// <summary>
    /// This instantiates a 'VideoParameters' object with default parameters.
    /// </summary>
    public class VideoParametersFactory
    {
        // Default image dimensions, in pixels.
        // NOTE: LumaView 600 is contrained to a square image - 
        // this is because the lens is round and thus we'd keep 
        // the distortion the same around the edges of the image.
        // But you may decide to have a non-square image.
        // NOTE: Width and height both must be multiples of four!!!
        public const int DEFAULT_VIDEO_WIDTH = 1200;
        public const int DEFAULT_VIDEO_HEIGHT = 1200;


        public static VideoParameters getVideoParameters()
        {
            VideoParameters videoParams = new VideoParameters();

            // This is the byte sequence that is the identifier for the start of
            // a video frame from the LumaScope 600.  Having a delimiter allows
            // us to maintain synchronization with the data stream.
            videoParams.frameDelimiter = new byte[] { 0x01, 0xfe, 0x00, 0xff };
            videoParams.height = DEFAULT_VIDEO_HEIGHT;
            videoParams.width = DEFAULT_VIDEO_WIDTH;

            return videoParams;
        }
    }
    
}

# *****************************************************************************
# This is a Python (3.6) script as a proof-of-concept showing that
# it is possible to initialize (load the HEX file) the Lumascope board.
# This script show that there are two ways to initialize the board:
# (1) A pop up dialog box and
# (2) Polling.
# After the board is initialized, the user can enter values in the console to
# set the LEDs.  This script does not display video images but there is a
# menu option to initialize the Aptina video chip.
#
# In order to get this script to function, you will need the following
# files in the same directory as this script:
# * Lumascope600.hex
# * LumaScopeInitialization.dll
# * LumaUSB.dll
# *****************************************************************************


# This is the 'Common Language Runtime' library (https://pypi.org/project/pythonnet/)
# and NOT the 'Terminal string styling' library (https://pypi.org/project/clr/).
# There is a name conflict between the two, they are need "import clr".
# Additionally, Python 3.9 does not support 'Common Language Runtime' library when
# this code was written, versions earlier than 3.9.  This code was developed with Python 3.6.
import clr
# from datetime import time
import time

# Must match Lumaview's setting (image size, in pixels).
IMAGE_SIZE = 1200

clr.AddReference("LumaUSB")
clr.AddReference("LumaScopeInitialization")

from Board_ns import LumaScopeInitializer
from LumaUSB_ns import LumaUSB
from LumaScopeInitialization import PollingInitialization
from LumaScopeInitialization import VideoInitialization



# Press the green button in the gutter to run the script.
if __name__ == '__main__':

    print('Etaluma Python script for initializing Lumascope board on Windows.')

    # There are two way

    scopeConnected = PollingInitialization.isLScopeConnected()
    print("Is Lumascope connected = {}".format(scopeConnected))

    if not scopeConnected:

        uninitializedBoardConnected = PollingInitialization.isFX2Connected()
        print("Is FX2 connected = {}".format(uninitializedBoardConnected))

        if not uninitializedBoardConnected:
            print("No board is connected.  Plug in the board and then press <ENTER>:")
            key_val = input()

        print("Select method to initialize the board:")
        print("Select '1' and <ENTER> for Windows-message based, popup dialog :")
        print("Select '2' and <ENTER> for polling method :")
        key_val = input()

        if '1' == key_val:
            # Here is where use LumascopeInitialization.dll to launch a modal dialog box initialize the board.
            # This method intercepts Windows operating system messages to know when the board is plugged
            # into the computer and when the board has been initialized.
            # Wait for the "LumaScope FX2 firmware version number: nn" in the title of the dialog box.
            # before setting the LEDs with the controls in the dialog box.
            # Note: The dialog may not appear and be hidden behind the Python development environment.
            # This has reported to happen with Matlab too.
            lumaScopeInitializer = LumaScopeInitializer()
            version = lumaScopeInitializer.version()
            print(version)
            # Set to 'True' if you with the dialog box to automatically close when init. is done.
            dialogAutoCloseFlag = True
            value = lumaScopeInitializer.launchInitDialog(dialogAutoCloseFlag)
        elif '2' == key_val:
            # This method of initialization uses polling to know when the board is initialized.
            # It doesn't not rely on Windows messages or need a dialog box to pop up.
            PollingInitialization.initializeLumaUsbObject()
            i = 1
            while i < 20:
                # Looping twenty times should be more than enough time from the board to be initialized.
                print("Waiting for initialization ({0}) ...".format(i))
                time.sleep(1)
                i += 1
                if PollingInitialization.isLScopeConnected():
                    print("The board is initialized.")
                    break

    if not PollingInitialization.isLScopeConnected():
        print("Failed to initialize the board, exiting Python script.")
        quit()

    # Here is where we test access to LumaUSB.dll.  The LumaUSB class gives access to controlling the microscope.
    lumaUsb = LumaUSB(LumaUSB.VID_CYPRESS, LumaUSB.PID_LSCOPE, IMAGE_SIZE, IMAGE_SIZE)

    while True:
        print("Enter keystroke to select given LED: '1', '2' or '3' followed by <ENTER>:")
        print("Enter 'v' and <ENTER> to initialize video chip:")
        print("Enter 'q' and <ENTER> to quit:")
        key_val = input()

        if '1' == key_val:
            print("Enter level for LED #1 (0 to 255) followed by <ENTER>")
            level = int(input())
            val = 0x41  # = 'A'
            lumaUsb.LedControllerWrite(val, level)

        elif '2' == key_val:
            print("Enter level for LED #2 (0 to 255) followed by <ENTER>")
            level = int(input())
            val = 0x42  # = 'B'
            lumaUsb.LedControllerWrite(val, level)

        elif '3' == key_val:
            print("Enter level for LED #3 (0 to 255) followed by <ENTER>")
            level = int(input())
            val = 0x43  # = 'C'
            lumaUsb.LedControllerWrite(val, level)

        elif 'v' == key_val:
            errorMessage = "test"
            videoResult = VideoInitialization.initializeImageSensor(lumaUsb, errorMessage)
            print("Aptina chip initialization result = {0}.".format(videoResult))

        elif 'q' == key_val:
            print("Pressed quit!")
            break

    print("----- Finished -------")

# See PyCharm help at https://www.jetbrains.com/help/pycharm/

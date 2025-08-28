# ADEL Card Reader Manager

This application provides a Windows Forms interface for managing ADEL card readers (like the ADEL 8MF) for hotel key card systems. **Supports both traditional serial (RS232) and modern USB connections.**

## ADEL SDK Documentation Summary

Based on the provided documentation, the ADEL Lock System Interface Functions support various lock systems:

### Supported Lock Systems:
- **A90** - RF Card system (recommended for most applications, **USB compatible**)
- **A92** - RF Card system variant (**USB compatible**)
- **A30** - IC Card system
- **Lock9200T** - Temic card system
- **Lock3200K** - IC Card system
- And others (Lock5200, Lock6200, Lock7200, etc.)

### Key Features:
1. **Card Reading** - Read existing guest cards to extract room number, guest info, validity period
2. **Card Writing** - Create new guest cards with room access permissions
3. **Card Management** - Erase/logout cards, duplicate cards
4. **Multiple Card Types** - Support for RF cards, IC cards, magnetic cards, Temic cards
5. **USB Support** - Automatic detection of USB card readers as virtual COM ports

## Setup Requirements

### Hardware:
1. ADEL card reader device (e.g., ADEL 8MF)
2. **Connection options:**
   - **USB connection** (Recommended) - Modern USB card readers (creates virtual COM port)
   - **Serial port connection** (COM1, COM2, etc.) - Traditional RS232
3. Compatible cards for your lock system

### Software:
1. **MAINDLL.DLL** - This is the core SDK library that must be placed in the same directory as your executable
2. **USB Drivers** - For USB card readers (usually provided by ADEL or Windows automatically)
3. Windows operating system (32-bit or 64-bit)
4. .NET 9.0 or later

## Installation

1. **Copy MAINDLL.DLL** to your application directory:
   ```
   C:\YourApp\MAINDLL.DLL
   C:\YourApp\AdelCardReaderManager.exe
   ```

2. **Connect your ADEL card reader:**
   
   **For USB Card Readers (Recommended):**
   - Connect the USB cable to your computer
   - Windows should automatically install drivers
   - The device will appear as a virtual COM port (e.g., COM3, COM4, etc.)
   - Use Device Manager to check which COM port was assigned
   
   **For Serial (RS232) Card Readers:**
   - Connect to a physical COM port (COM1 or COM2)
   - May require a USB-to-Serial adapter for modern computers

3. **Install any required drivers** for your specific ADEL model

## USB Configuration Guide for ADEL Card Readers

### Step 1: Connect USB Card Reader
1. **Plug your ADEL card reader** into a USB port on your computer
2. **Wait for Windows** to attempt automatic driver installation
3. **Check the notification area** for driver installation messages
4. If automatic installation fails, continue to Step 2

### Step 2: Install USB Drivers (if needed)
1. **Check ADEL documentation** or CD for USB drivers
2. **Download latest drivers** from ADEL's website if available
3. **Install the drivers** following ADEL's instructions
4. **Restart your computer** after driver installation

### Step 3: Find the Virtual COM Port
1. **Open Device Manager:**
   - Right-click "This PC" ? "Manage" ? "Device Manager"
   - OR press `Win + X` and select "Device Manager"
2. **Expand "Ports (COM & LPT)"**
3. **Look for your ADEL device** - it may appear as:
   - "USB Serial Port (COM3)" or similar
   - "ADEL Card Reader" or similar
   - "Prolific USB-to-Serial" or similar
4. **Note the COM port number** (e.g., COM3, COM4, COM5, etc.)

### Step 4: Configure the Application
1. **Run the ADEL Card Reader Manager**
2. **Click "Refresh"** next to the COM Port dropdown
3. **Select the COM port** where your USB card reader is connected
4. **Choose the appropriate Lock System** (usually A90 or A92 for USB models)
5. **Click "Initialize"**
6. **Look for success message**: "? Card reader initialized successfully"

## How to Use

### 1. Initialize the Card Reader
- Select the correct **COM Port** (check Device Manager for USB readers)
- Select the correct **Lock System** (A90 is most common for RF cards)
- Click **Initialize**
- Look for "? Card reader initialized successfully" message

### 2. Read an Existing Card
- Insert a card into the reader
- Click **Read Card**
- The application will display:
  - Room number
  - Guest name and ID
  - Validity period (start/end dates)
  - Card status (normal, expired, etc.)
  - Card number

### 3. Create a New Guest Card
- Insert a blank card into the reader
- Fill in the form fields:
  - **Room**: Room number (e.g., "101")
  - **Guest**: Guest name
  - **ID**: Guest ID number
  - **Start**: Check-in date/time
  - **End**: Check-out date/time
- Click **Create Card**
- Remove the programmed card

### 4. Erase a Card
- Insert the card to be erased
- Click **Erase Card**
- The card will be deactivated

## USB Troubleshooting Guide

### Problem: "No COM ports detected"
**Solutions:**
1. **Check USB cable connection** - try unplugging and reconnecting
2. **Try a different USB port** - preferably USB 2.0 or USB 3.0
3. **Install USB drivers** from ADEL's website or CD
4. **Restart the application** after connecting the device
5. **Click "Refresh"** button next to COM port dropdown
6. **Check Device Manager** for unknown devices with warning icons

### Problem: "Device not recognized" in Windows
**Solutions:**
1. **Download latest drivers** from ADEL website
2. **Try a different USB cable** - cable may be damaged
3. **Check Device Manager** for devices with warning icons
4. **Run Windows Hardware Troubleshooter:**
   - Settings ? Update & Security ? Troubleshoot ? Hardware and Devices
5. **Update Windows** to get latest USB drivers

### Problem: "Failed to initialize card reader"
**Solutions:**
1. **Verify correct COM port** is selected (check Device Manager)
2. **Ensure USB drivers** are properly installed
3. **Close other applications** that might be using the COM port
4. **Run as Administrator** if permission issues occur
5. **Try different Lock System** settings (A90, A92, A30)
6. **Check MAINDLL.DLL** is in the application directory

### Problem: "Permission denied" or "Access denied"
**Solutions:**
1. **Run the application as Administrator**
2. **Close other applications** using COM ports (Arduino IDE, PuTTY, etc.)
3. **Check Windows permissions** for COM ports
4. **Disable antivirus temporarily** to test if it's blocking access

### Problem: USB card reader randomly disconnects
**Solutions:**
1. **Disable USB selective suspend:**
   - Control Panel ? Power Options ? Change plan settings ? Advanced
   - USB settings ? USB selective suspend ? Disabled
2. **Update USB drivers** through Device Manager
3. **Try a different USB port** - avoid USB hubs
4. **Use a powered USB hub** if necessary

## Understanding Time Format

The ADEL system uses a specific time format: `yyyymmddhhnnyyyy mmddhhnn`

Example: `200012311230200101011230` means:
- Start: December 31, 2000 at 12:30
- End: January 1, 2001 at 12:30

The application handles this conversion automatically.

## Understanding Public Channels

Public channels control which areas the card can access:
- **"00"** - Default authorization channel
- **"99"** - All public channels authorized
- **"010203"** - Specific channels 01, 02, and 03

## Error Codes Reference

- **0** - Success
- **1** - Read/write error or data error
- **2** - Card is damaged
- **3** - No card detected
- **4** - Serial port error (check USB connection)
- **5** - Card is replaced
- **6** - No new card
- **7** - New card detected
- **8** - Non-system card
- **9** - Not a guest card

## Advanced USB Configuration

### Finding USB Card Reader Information
1. **Open Command Prompt** as Administrator
2. **Run command:** `wmic path Win32_SerialPort get DeviceID,Description,Name`
3. **Look for ADEL-related entries** in the output
4. **Note the DeviceID** (e.g., COM3, COM4)

### Manual Driver Installation
1. **Download .inf driver file** from ADEL
2. **Right-click the .inf file** ? "Install"
3. **Follow installation prompts**
4. **Restart computer** when prompted

### USB Power Management
1. **Open Device Manager**
2. **Find your USB card reader** under "Ports (COM & LPT)"
3. **Right-click** ? Properties ? Power Management
4. **Uncheck "Allow computer to turn off this device"**
5. **Click OK** and test connection

## Notes

1. **USB vs Serial**: USB connection is recommended for modern computers as it's easier to set up and more reliable.

2. **Driver Compatibility**: Some ADEL USB card readers use generic USB-to-Serial chips (like Prolific or FTDI) which have standard Windows drivers.

3. **Multiple Readers**: You can connect multiple USB card readers - each will get its own COM port number.

4. **Card Compatibility**: Ensure you're using the correct card type for your ADEL system (RF, IC, magnetic, or Temic).

5. **Backup**: Always keep backup copies of important cards and consider implementing a master card system.
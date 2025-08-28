namespace AdelCardReaderManager;

public partial class Form1 : Form
{
    private AdelCardReader cardReader;

    public Form1()
    {
        InitializeComponent();
        cardReader = new AdelCardReader();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // Set default values
        dtpStartTime.Value = DateTime.Now;
        dtpEndTime.Value = DateTime.Now.AddDays(1);
        
        // Check if MAINDLL.DLL exists
        CheckDLLExists();
        
        // Populate COM ports (including USB virtual COM ports)
        PopulateComPorts();
        
        LogMessage("ADEL Card Reader Manager started");
        LogMessage("Please initialize the card reader first");
        LogMessage("Note: USB card readers appear as virtual COM ports (e.g., COM3, COM4, etc.)");
    }

    private void CheckDLLExists()
    {
        string dllPath = Path.Combine(Application.StartupPath, "MAINDLL.DLL");
        if (File.Exists(dllPath))
        {
            LogMessage("? MAINDLL.DLL found in application directory");
            
            // Try to get additional DLL information
            try
            {
                var fileInfo = new FileInfo(dllPath);
                LogMessage($"  File size: {fileInfo.Length:N0} bytes");
                LogMessage($"  Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
                
                // Check if we can load the DLL (basic test)
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                if (!string.IsNullOrEmpty(versionInfo.FileVersion))
                {
                    LogMessage($"  Version: {versionInfo.FileVersion}");
                }
                if (!string.IsNullOrEmpty(versionInfo.FileDescription))
                {
                    LogMessage($"  Description: {versionInfo.FileDescription}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"  ? Could not read DLL details: {ex.Message}");
            }
        }
        else
        {
            LogMessage("? WARNING: MAINDLL.DLL not found!");
            LogMessage("Please copy MAINDLL.DLL to the application directory:");
            LogMessage($"  ?? {Application.StartupPath}");
            LogMessage("");
            LogMessage("SDK Setup Instructions:");
            LogMessage("1. Create 'SDK' folder in your project directory");
            LogMessage("2. Copy MAINDLL.DLL from ADEL SDK to the SDK folder");
            LogMessage("3. Rebuild your project");
            LogMessage("4. The DLL will be automatically copied to output directory");
            LogMessage("");
            LogMessage("Alternative: Copy MAINDLL.DLL directly to:");
            LogMessage($"  ?? {Application.StartupPath}");
            LogMessage("");
            LogMessage("The SDK will not work without this file.");
            LogMessage("?? See SDK_SETUP.md for detailed instructions");
        }
    }

    private void PopulateComPorts()
    {
        try
        {
            // Clear existing items
            cmbComPort.Items.Clear();
            
            LogMessage("Detecting COM ports...");
            
            // Get all available COM ports (including USB virtual COM ports)
            string[] availablePorts = System.IO.Ports.SerialPort.GetPortNames();
            
            LogMessage($"System.IO.Ports.SerialPort.GetPortNames() returned {availablePorts.Length} ports");
            
            if (availablePorts.Length > 0)
            {
                Array.Sort(availablePorts); // Sort them numerically
                cmbComPort.Items.AddRange(availablePorts);
                cmbComPort.SelectedIndex = 0;
                LogMessage($"? Found {availablePorts.Length} COM port(s): {string.Join(", ", availablePorts)}");
                
                // Try to detect USB card readers
                // DetectUSBCardReaders(availablePorts); // Temporarily commented out
            }
            else
            {
                LogMessage("? No COM ports detected by .NET SerialPort.GetPortNames()");
                
                // Try alternative detection methods
                TryAlternativePortDetection();
                
                // Add default ports if none detected
                cmbComPort.Items.AddRange(new object[] { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8" });
                cmbComPort.SelectedIndex = 0;
                LogMessage("Using default COM port list (COM1-COM8)");
                LogMessage("If using USB card reader, please:");
                LogMessage("  1. Install the USB-to-Serial driver");
                LogMessage("  2. Check Device Manager for COM ports");
                LogMessage("  3. Try running as Administrator");
                LogMessage("  4. Check if card reader is properly connected");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Error detecting COM ports: {ex.Message}");
            LogMessage($"Exception type: {ex.GetType().Name}");
            
            // Try alternative detection if main method fails
            TryAlternativePortDetection();
            
            // Fallback to default ports
            if (cmbComPort.Items.Count == 0)
            {
                cmbComPort.Items.AddRange(new object[] { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8" });
                cmbComPort.SelectedIndex = 0;
                LogMessage("Using fallback COM port list");
            }
        }
    }

    private void DetectUSBCardReaders(string[] comPorts)
    {
        try
        {
            LogMessage("Analyzing COM ports for USB card readers...");
            
            // Try to detect which COM ports might be USB card readers
            // This is done by checking Windows Management Instrumentation (WMI)
            using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SerialPort"))
            {
                bool foundAnyUSB = false;
                foreach (System.Management.ManagementObject port in searcher.Get())
                {
                    string deviceId = port["DeviceID"]?.ToString() ?? "";
                    string description = port["Description"]?.ToString() ?? "";
                    string name = port["Name"]?.ToString() ?? "";
                    string pnpDeviceId = port["PNPDeviceID"]?.ToString() ?? "";
                    
                    // Check if this looks like a USB card reader
                    if (description.ToLower().Contains("usb") || 
                        description.ToLower().Contains("card") ||
                        description.ToLower().Contains("adel") ||
                        name.ToLower().Contains("usb") ||
                        pnpDeviceId.ToLower().Contains("usb"))
                    {
                        LogMessage($"  ?? {deviceId}: {description} (Possible USB card reader)");
                        if (!string.IsNullOrEmpty(pnpDeviceId))
                            LogMessage($"      PnP ID: {pnpDeviceId}");
                        foundAnyUSB = true;
                    }
                    else if (comPorts.Contains(deviceId))
                    {
                        LogMessage($"  ?? {deviceId}: {description}");
                    }
                }
                
                if (!foundAnyUSB && comPorts.Length > 0)
                {
                    LogMessage("  ? No obvious USB card readers detected, but COM ports are available");
                }
            }
        }
        catch (Exception ex)
        {
            // WMI query failed, not critical
            LogMessage($"? Could not query USB devices via WMI: {ex.Message}");
            LogMessage("  This might be due to insufficient permissions or WMI service issues");
        }
    }

    private void TryAlternativePortDetection()
    {
        try
        {
            LogMessage("Trying alternative COM port detection via Registry...");
            
            // Try to read COM ports from Windows Registry
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                if (key != null)
                {
                    var ports = new List<string>();
                    foreach (string valueName in key.GetValueNames())
                    {
                        string? portName = key.GetValue(valueName)?.ToString();
                        if (!string.IsNullOrEmpty(portName) && portName.StartsWith("COM"))
                        {
                            ports.Add(portName);
                        }
                    }
                    
                    if (ports.Count > 0)
                    {
                        ports.Sort();
                        LogMessage($"? Registry found {ports.Count} COM port(s): {string.Join(", ", ports)}");
                        
                        if (cmbComPort.Items.Count == 0)
                        {
                            cmbComPort.Items.AddRange(ports.ToArray());
                            cmbComPort.SelectedIndex = 0;
                        }
                        return;
                    }
                }
            }
            
            LogMessage("? No COM ports found in Registry");
        }
        catch (Exception ex)
        {
            LogMessage($"? Registry COM port detection failed: {ex.Message}");
        }
        
        // Try WMI-based detection as another alternative
        TryWMIPortDetection();
    }

    private void TryWMIPortDetection()
    {
        try
        {
            LogMessage("Trying WMI-based COM port detection...");
            
            using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SerialPort"))
            {
                var ports = new List<string>();
                foreach (System.Management.ManagementObject port in searcher.Get())
                {
                    string deviceId = port["DeviceID"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(deviceId) && deviceId.StartsWith("COM"))
                    {
                        ports.Add(deviceId);
                    }
                }
                
                if (ports.Count > 0)
                {
                    ports.Sort();
                    LogMessage($"? WMI found {ports.Count} COM port(s): {string.Join(", ", ports)}");
                    
                    if (cmbComPort.Items.Count == 0)
                    {
                        cmbComPort.Items.AddRange(ports.ToArray());
                        cmbComPort.SelectedIndex = 0;
                    }
                }
                else
                {
                    LogMessage("? No COM ports found via WMI");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? WMI COM port detection failed: {ex.Message}");
        }
    }

    private void btnInitialize_Click(object sender, EventArgs e)
    {
        try
        {
            // Extract COM port number from selection (e.g., "COM3" -> 3)
            string selectedPort = cmbComPort.SelectedItem?.ToString() ?? "COM1";
            int comPort = ExtractComPortNumber(selectedPort);
            int lockSystem = GetSelectedLockSystem();

            LogMessage($"Initializing card reader...");
            LogMessage($"COM Port: {selectedPort} (Port Number: {comPort})");
            LogMessage($"Lock System: {cmbLockSystem.SelectedItem}");

            // Try standalone initialization first (no database connection required)
            bool success = cardReader.InitializeStandalone(lockSystem, comPort, AdelCardReader.AUTO_ENCODER, 1);

            if (success)
            {
                LogMessage("? Card reader initialized successfully");
                lblStatus.Text = "Status: Connected";
                lblStatus.ForeColor = Color.Green;
                EnableCardOperations(true);
            }
            else
            {
                LogMessage("? Failed to initialize card reader");
                LogMessage("Please check:");
                LogMessage("- MAINDLL.DLL is in the application directory");
                LogMessage($"- Card reader is connected to {selectedPort}");
                LogMessage("- USB drivers are installed (for USB card readers)");
                LogMessage("- No other application is using the COM port");
                LogMessage("- Try different COM port if unsure");
                lblStatus.Text = "Status: Connection failed";
                lblStatus.ForeColor = Color.Red;
                EnableCardOperations(false);
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Error during initialization: {ex.Message}");
            lblStatus.Text = "Status: Error";
            lblStatus.ForeColor = Color.Red;
            EnableCardOperations(false);
        }
    }

    private int ExtractComPortNumber(string comPortName)
    {
        try
        {
            // Extract number from "COM3" -> 3
            string numberPart = comPortName.Replace("COM", "").Trim();
            return int.Parse(numberPart);
        }
        catch
        {
            return 1; // Default to COM1 if parsing fails
        }
    }

    private void btnReadCard_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("Reading card...");
            
            var cardInfo = cardReader.ReadCardInfo();
            
            if (cardInfo != null)
            {
                LogMessage("? Card read successfully:");
                LogMessage($"  Room: {cardInfo.Room}");
                LogMessage($"  Guest: {cardInfo.GuestName}");
                LogMessage($"  Guest ID: {cardInfo.GuestId}");
                LogMessage($"  Card Number: {cardInfo.CardNumber}");
                LogMessage($"  Valid From: {cardInfo.StartTime}");
                LogMessage($"  Valid Until: {cardInfo.EndTime}");
                LogMessage($"  Status: {GetCardStatusDescription(cardInfo.Status)}");
                LogMessage($"  Public Channels: {cardInfo.Gate}");
                
                if (!string.IsNullOrEmpty(cardInfo.Track1))
                    LogMessage($"  Track 1: {cardInfo.Track1}");
                if (!string.IsNullOrEmpty(cardInfo.Track2))
                    LogMessage($"  Track 2: {cardInfo.Track2}");

                // Update form fields with read data
                txtRoomNumber.Text = cardInfo.Room;
                txtGuestName.Text = cardInfo.GuestName;
                txtGuestId.Text = cardInfo.GuestId;
            }
            else
            {
                LogMessage("? No card detected or unable to read card");
                LogMessage("Please ensure a card is properly positioned in the reader");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Error reading card: {ex.Message}");
        }
    }

    private void btnCreateCard_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                LogMessage("? Please enter a room number");
                return;
            }

            string roomNumber = txtRoomNumber.Text.Trim();
            string guestName = txtGuestName.Text.Trim();
            string guestId = txtGuestId.Text.Trim();
            DateTime startTime = dtpStartTime.Value;
            DateTime endTime = dtpEndTime.Value;

            if (endTime <= startTime)
            {
                LogMessage("? End time must be after start time");
                return;
            }

            LogMessage($"Creating guest card...");
            LogMessage($"  Room: {roomNumber}");
            LogMessage($"  Guest: {guestName}");
            LogMessage($"  Guest ID: {guestId}");
            LogMessage($"  Valid from: {startTime:yyyy-MM-dd HH:mm}");
            LogMessage($"  Valid until: {endTime:yyyy-MM-dd HH:mm}");

            bool success = cardReader.CreateGuestCard(roomNumber, guestName, guestId, startTime, endTime);

            if (success)
            {
                LogMessage("? Guest card created successfully");
                LogMessage("Please remove the card from the reader");
            }
            else
            {
                LogMessage("? Failed to create guest card");
                LogMessage("Please check:");
                LogMessage("- A blank card is inserted in the reader");
                LogMessage("- The card is compatible with the lock system");
                LogMessage("- The room number is valid");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Error creating card: {ex.Message}");
        }
    }

    private void btnEraseCard_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("Erasing card...");
            
            bool success = cardReader.EraseCard(); // 0 = automatically read card number and erase
            
            if (success)
            {
                LogMessage("? Card erased successfully");
            }
            else
            {
                LogMessage("? Failed to erase card");
                LogMessage("Please ensure a card is properly positioned in the reader");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Error erasing card: {ex.Message}");
        }
    }

    private int GetSelectedLockSystem()
    {
        return cmbLockSystem.SelectedIndex switch
        {
            0 => AdelCardReader.A90,      // A90
            1 => AdelCardReader.A92,      // A92
            2 => AdelCardReader.A30,      // A30
            3 => AdelCardReader.LOCK9200T, // Lock9200T
            4 => AdelCardReader.LOCK3200K, // Lock3200K
            _ => AdelCardReader.A90        // Default
        };
    }

    private string GetCardStatusDescription(int status)
    {
        return status switch
        {
            1 => "Normal use",
            3 => "Normal logout",
            4 => "Lost logout",
            5 => "Destroy logout",
            6 => "Auto logout",
            _ => $"Unknown status ({status})"
        };
    }

    private void EnableCardOperations(bool enabled)
    {
        btnReadCard.Enabled = enabled;
        btnCreateCard.Enabled = enabled;
        btnEraseCard.Enabled = enabled;
    }

    private void LogMessage(string message)
    {
        txtOutput.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        txtOutput.ScrollToCaret();
    }

    private void btnRefreshPorts_Click(object sender, EventArgs e)
    {
        LogMessage("Refreshing COM ports...");
        PopulateComPorts();
    }

    private bool IsRunningAsAdministrator()
    {
        try
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private void btnDiagnostics_Click(object sender, EventArgs e)
    {
        LogMessage("=== DIAGNOSTIC INFORMATION ===");
        
        // System information
        LogMessage($"OS: {Environment.OSVersion}");
        LogMessage($"Architecture: {Environment.Is64BitOperatingSystem} bit OS, {Environment.Is64BitProcess} bit process");
        LogMessage($"User: {Environment.UserName} (Admin: {true})"); // Temporarily simplified
        
        // .NET COM port detection
        try
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            LogMessage($".NET SerialPort.GetPortNames(): {ports.Length} ports");
            if (ports.Length > 0)
                LogMessage($"  Ports: {string.Join(", ", ports)}");
        }
        catch (Exception ex)
        {
            LogMessage($"? .NET SerialPort.GetPortNames() failed: {ex.Message}");
        }
        
        // Registry-based detection
        try
        {
            LogMessage("Registry SERIALCOMM entries:");
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM"))
            {
                if (key != null)
                {
                    foreach (string valueName in key.GetValueNames())
                    {
                        string? portName = key.GetValue(valueName)?.ToString();
                        LogMessage($"  {valueName} -> {portName}");
                    }
                }
                else
                {
                    LogMessage("  Registry key not found");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Registry access failed: {ex.Message}");
        }
        
        // WMI-based detection
        try
        {
            LogMessage("WMI Win32_SerialPort entries:");
            using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SerialPort"))
            {
                int count = 0;
                foreach (System.Management.ManagementObject port in searcher.Get())
                {
                    count++;
                    string deviceId = port["DeviceID"]?.ToString() ?? "N/A";
                    string description = port["Description"]?.ToString() ?? "N/A";
                    string name = port["Name"]?.ToString() ?? "N/A";
                    string pnpDeviceId = port["PNPDeviceID"]?.ToString() ?? "N/A";
                    
                    LogMessage($"  Port {count}:");
                    LogMessage($"    Device ID: {deviceId}");
                    LogMessage($"    Description: {description}");
                    LogMessage($"    Name: {name}");
                    LogMessage($"    PnP Device ID: {pnpDeviceId}");
                }
                
                if (count == 0)
                    LogMessage("  No serial ports found via WMI");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? WMI query failed: {ex.Message}");
        }
        
        // Check for USB devices with driver issues
        CheckUSBDeviceDriverStatus();
        
        // USB devices that might be card readers
        try
        {
            LogMessage("USB devices (potential card readers):");
            using (var searcher = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%USB%' OR Name LIKE '%Serial%' OR Name LIKE '%COM%'"))
            {
                int count = 0;
                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    string name = device["Name"]?.ToString() ?? "N/A";
                    string deviceId = device["DeviceID"]?.ToString() ?? "N/A";
                    string status = device["Status"]?.ToString() ?? "N/A";
                    
                    if (name.ToLower().Contains("com") || name.ToLower().Contains("serial") || name.ToLower().Contains("usb"))
                    {
                        count++;
                        LogMessage($"  USB Device {count}:");
                        LogMessage($"    Name: {name}");
                        LogMessage($"    Device ID: {deviceId}");
                        LogMessage($"    Status: {status}");
                    }
                }
                
                if (count == 0)
                    LogMessage("  No relevant USB devices found");
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? USB device query failed: {ex.Message}");
        }
        
        LogMessage("=== END DIAGNOSTICS ===");
    }

    private void CheckUSBDeviceDriverStatus()
    {
        try
        {
            LogMessage("Checking for devices with driver issues:");
            
            // Check for devices with problems (missing drivers, etc.)
            using (var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode != 0"))
            {
                int problemCount = 0;
                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    problemCount++;
                    string name = device["Name"]?.ToString() ?? "Unknown Device";
                    string deviceId = device["DeviceID"]?.ToString() ?? "N/A";
                    uint errorCode = Convert.ToUInt32(device["ConfigManagerErrorCode"] ?? 0);
                    string status = device["Status"]?.ToString() ?? "N/A";
                    
                    LogMessage($"  ? Problem Device {problemCount}:");
                    LogMessage($"    Name: {name}");
                    LogMessage($"    Device ID: {deviceId}");
                    LogMessage($"    Error Code: {errorCode} ({GetDeviceErrorDescription(errorCode)})");
                    LogMessage($"    Status: {status}");
                    
                    // Check if this might be a USB-to-Serial device
                    if (deviceId.ToUpper().Contains("USB") || deviceId.ToUpper().Contains("VID_") || 
                        name.ToLower().Contains("serial") || name.ToLower().Contains("com"))
                    {
                        LogMessage($"    ? This appears to be a USB-to-Serial device with driver issues!");
                    }
                }
                
                if (problemCount == 0)
                {
                    LogMessage("  ? No devices with driver problems found");
                }
                else
                {
                    LogMessage($"  Found {problemCount} device(s) with driver issues");
                    LogMessage("  To fix driver issues:");
                    LogMessage("    1. Open Device Manager");
                    LogMessage("    2. Look for devices with yellow warning triangles");
                    LogMessage("    3. Right-click the device and select 'Update driver'");
                    LogMessage("    4. Or download drivers from manufacturer's website");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Driver status check failed: {ex.Message}");
        }
        
        // Check for unknown devices
        try
        {
            using (var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%Unknown%' OR Caption LIKE '%Unknown%'"))
            {
                int unknownCount = 0;
                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    unknownCount++;
                    string name = device["Name"]?.ToString() ?? "Unknown";
                    string deviceId = device["DeviceID"]?.ToString() ?? "N/A";
                    
                    LogMessage($"  ? Unknown Device {unknownCount}: {name}");
                    LogMessage($"    Device ID: {deviceId}");
                }
                
                if (unknownCount > 0)
                {
                    LogMessage($"  Found {unknownCount} unknown device(s) - these may need drivers");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? Unknown device check failed: {ex.Message}");
        }
    }

    private string GetDeviceErrorDescription(uint errorCode)
    {
        return errorCode switch
        {
            0 => "No error",
            1 => "Device not configured correctly",
            3 => "Driver corrupted or missing",
            10 => "Device cannot start",
            18 => "Reinstall drivers for this device",
            19 => "Registry returned unknown result",
            21 => "Windows is removing this device",
            22 => "Device is disabled",
            24 => "Device not present, not working, or doesn't have all drivers installed",
            28 => "Drivers not installed",
            31 => "Device not working properly",
            37 => "Windows cannot initialize the device driver",
            39 => "Windows cannot load the device driver",
            40 => "Windows cannot access this hardware",
            41 => "Windows successfully loaded the device driver but cannot find the hardware device",
            42 => "Windows cannot load the device driver because there is a duplicate device already running",
            43 => "Windows has stopped this device because it has reported problems",
            44 => "An application or service has shut down this hardware device",
            45 => "Currently, this hardware device is not connected to the computer",
            46 => "Windows cannot gain access to this hardware device",
            47 => "Windows cannot use this hardware device because it has been prepared for safe removal",
            48 => "Software for this device has been blocked from starting",
            49 => "Windows cannot start new hardware devices because the system hive is too large",
            _ => $"Error code {errorCode}"
        };
    }

    private void btnCheckDrivers_Click(object sender, EventArgs e)
    {
        LogMessage("=== USB-TO-SERIAL DRIVER CHECK ===");
        
        // Check if any USB card reader might be connected
        CheckForUSBCardReaders();
        
        // Provide driver installation guidance
        LogMessage("");
        LogMessage("?? DRIVER INSTALLATION GUIDE:");
        LogMessage("");
        LogMessage("If you have a USB card reader and no COM ports are detected:");
        LogMessage("");
        LogMessage("1?? IDENTIFY YOUR DEVICE:");
        LogMessage("   • Open Device Manager (Windows + X ? Device Manager)");
        LogMessage("   • Look for devices with yellow warning triangles");
        LogMessage("   • Check 'Other devices' or 'Unknown devices' section");
        LogMessage("   • Note the device name and hardware ID");
        LogMessage("");
        LogMessage("2?? COMMON USB-TO-SERIAL CHIPS:");
        LogMessage("   • FTDI (FT232, FT234X): Download from ftdichip.com");
        LogMessage("   • Prolific (PL2303): Download from prolific.com.tw");
        LogMessage("   • Silicon Labs (CP210x): Download from silabs.com");
        LogMessage("   • CH340/CH341: Search for 'CH340 driver download'");
        LogMessage("");
        LogMessage("3?? AUTOMATIC DRIVER INSTALLATION:");
        LogMessage("   • Connect your USB card reader");
        LogMessage("   • Windows may install drivers automatically");
        LogMessage("   • Wait 2-3 minutes for Windows Update to check");
        LogMessage("");
        LogMessage("4?? MANUAL DRIVER INSTALLATION:");
        LogMessage("   • Right-click the unknown device in Device Manager");
        LogMessage("   • Select 'Update driver'");
        LogMessage("   • Choose 'Search automatically for drivers'");
        LogMessage("   • Or 'Browse my computer' if you downloaded drivers");
        LogMessage("");
        LogMessage("5?? VERIFY INSTALLATION:");
        LogMessage("   • After installation, check Device Manager");
        LogMessage("   • Look for 'Ports (COM & LPT)' section");
        LogMessage("   • You should see a new COM port (e.g., COM3, COM4)");
        LogMessage("   • Click 'Refresh' button in this application");
        LogMessage("");
        LogMessage("?? TROUBLESHOOTING TIPS:");
        LogMessage("   • Try different USB ports");
        LogMessage("   • Use USB 2.0 ports instead of USB 3.0");
        LogMessage("   • Restart computer after driver installation");
        LogMessage("   • Run this application as Administrator");
        LogMessage("   • Disable antivirus temporarily during driver installation");
        LogMessage("");
        LogMessage("?? For more help, click 'Diagnostics' button for detailed system info");
        LogMessage("=== END DRIVER CHECK ===");
    }

    private void CheckForUSBCardReaders()
    {
        try
        {
            LogMessage("Scanning for connected USB devices...");
            
            using (var searcher = new System.Management.ManagementObjectSearcher(
                "SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'"))
            {
                int usbDeviceCount = 0;
                int possibleCardReaders = 0;
                
                foreach (System.Management.ManagementObject device in searcher.Get())
                {
                    usbDeviceCount++;
                    string name = device["Name"]?.ToString() ?? "";
                    string deviceId = device["DeviceID"]?.ToString() ?? "";
                    string status = device["Status"]?.ToString() ?? "";
                    uint errorCode = Convert.ToUInt32(device["ConfigManagerErrorCode"] ?? 0);
                    
                    // Check if this might be a card reader or serial device
                    bool mightBeCardReader = 
                        name.ToLower().Contains("card") ||
                        name.ToLower().Contains("reader") ||
                        name.ToLower().Contains("serial") ||
                        name.ToLower().Contains("com") ||
                        name.ToLower().Contains("adel") ||
                        deviceId.ToUpper().Contains("VID_0403") || // FTDI
                        deviceId.ToUpper().Contains("VID_067B") || // Prolific
                        deviceId.ToUpper().Contains("VID_10C4") || // Silicon Labs
                        deviceId.ToUpper().Contains("VID_1A86");   // CH340/CH341
                    
                    if (mightBeCardReader)
                    {
                        possibleCardReaders++;
                        LogMessage($"?? Possible card reader found:");
                        LogMessage($"   Name: {name}");
                        LogMessage($"   Status: {status}");
                        LogMessage($"   Device ID: {deviceId}");
                        
                        if (errorCode != 0)
                        {
                            LogMessage($"   ? Error: {GetDeviceErrorDescription(errorCode)}");
                            LogMessage($"   ?? This device needs driver installation!");
                        }
                        else
                        {
                            LogMessage($"   ? Device appears to be working");
                        }
                        LogMessage("");
                    }
                }
                
                LogMessage($"?? Summary: {usbDeviceCount} USB devices, {possibleCardReaders} possible card readers");
                
                if (possibleCardReaders == 0)
                {
                    LogMessage("? No obvious USB card readers detected");
                    LogMessage("   • Make sure your card reader is connected");
                    LogMessage("   • Try a different USB cable or port");
                    LogMessage("   • Check if the card reader has a power switch");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage($"? USB device scan failed: {ex.Message}");
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        cardReader?.Shutdown();
        base.OnFormClosed(e);
    }
}

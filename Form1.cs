using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdelCardReaderManager;

public partial class Form1 : Form
{
    #region Global Variables
    private readonly AdelCardReader cardReader;
    private readonly string[] RequiredDlls = { "MAINDLL.DLL", "RF_USB.dll", "Mifire.dll", "LOCKDLL.dll", "AdelICK.dll" };

    #endregion

    public Form1()
    {
        InitializeComponent();
        cardReader = new AdelCardReader();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        CheckDLLDependencies();
        LogMessage("ADEL Card Reader Manager - Multiple ReadCard Functions Test");
        LogMessage("Using USB HID connection (Port 0)");
        LogMessage("Click 'Initialize USB' to connect to your card reader");
        LogMessage("Then test individual ReadCard functions with the buttons on the right");
    }

    #region DLL Management
    private void CheckDLLDependencies()
    {
        LogMessage("Checking DLL dependencies...");
        
        foreach (var dll in RequiredDlls)
        {
            CheckSingleDLL(dll, dll == "MAINDLL.DLL");
        }
        
        int foundCount = RequiredDlls.Skip(1).Count(dll => File.Exists(Path.Combine(Application.StartupPath, dll)));
        LogMessage($"SDK Dependencies: {foundCount}/{RequiredDlls.Length - 1} found");
        
        if (foundCount < RequiredDlls.Length - 1)
        {
            LogMessage("⚠ Some SDK dependencies are missing!");
            LogMessage("Copy ALL DLL files from your ADEL app installation to:");
            LogMessage($"  📁 {Application.StartupPath}");
        }
        else
        {
            LogMessage("✓ All essential SDK dependencies found!");
        }
    }

    private void CheckSingleDLL(string dllName, bool isMainDll)
    {
        string dllPath = Path.Combine(Application.StartupPath, dllName);
        
        if (File.Exists(dllPath))
        {
            LogMessage($"✓ {dllName} found");
            if (isMainDll) LogDLLDetails(dllPath);
        }
        else
        {
            LogMessage(isMainDll ? $"⚠ WARNING: {dllName} not found!" : $"⚠ {dllName} not found");
        }
    }

    private void LogDLLDetails(string dllPath)
    {
        try
        {
            var fileInfo = new FileInfo(dllPath);
            LogMessage($"  Size: {fileInfo.Length:N0} bytes");
            LogMessage($"  Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
            
            try
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(dllPath);
                LogMessage($"  File Version: {versionInfo.FileVersion}");
                LogMessage($"  Product Version: {versionInfo.ProductVersion}");
                LogMessage($"  Description: {versionInfo.FileDescription}");
                LogMessage($"  Company: {versionInfo.CompanyName}");
            }
            catch { LogMessage("  Version info not available"); }
        }
        catch (Exception ex) { LogMessage($"  ⚠ Could not read DLL details: {ex.Message}"); }
    }
    #endregion

    #region Initialization
    private void btnInitialize_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TestDLLLoading()) return;
            
            int lockSystem = GetSelectedLockSystem();
            LogMessage($"Initializing USB HID card reader with {cmbLockSystem.SelectedItem} (Code: {lockSystem})");
            LogMessage("Using SetPort() for hardware-only operations (no database)");

            // Use SetPort method for hardware-only initialization
            var (successSetPort, errorCodeSetPort, errorMessageSetPort) = 
                cardReader.InitializeStandaloneWithDetails(lockSystem, 0, AdelCardReader.AUTO_ENCODER, 1);
            
            LogMessage($"SetPort() returned: {errorCodeSetPort} - {errorMessageSetPort}");
            
            if (successSetPort)
            {
                LogMessage("✓ Card reader initialized successfully via SetPort() - Hardware Only");
                LogMessage("ℹ️ Note: This allows ReadCard, ReadCardId, ReadMagCard, ReadIC, Reader_Beep");
                LogMessage("ℹ️ For GetCardInfo (database function), use 'Init with Database' button");
                UpdateStatus("Connected (USB HID - Hardware Only)", Color.Green);
                EnableCardOperations(true);
            }
            else
            {
                LogMessage($"✗ SetPort() failed with error {errorCodeSetPort}");
                UpdateStatus($"Error {errorCodeSetPort}", Color.Red);
                EnableCardOperations(false);
            }
        }
        catch (Exception ex)
        {
            HandleException("initialization", ex);
        }
    }

    // Add new button handler for database initialization
    private void btnInitWithDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TestDLLLoading()) return;
            
            int lockSystem = GetSelectedLockSystem();
            LogMessage($"Initializing with DATABASE connection using {cmbLockSystem.SelectedItem} (Code: {lockSystem})");
            LogMessage("Using Init() for full database access");
            LogMessage("Server: DESKTOP-ILHONN0, User: DESKTOP-ILHONN0, Port: 0, Manual Encoder");

            // Use Init method for database initialization
            var (successInit, errorCodeInit, errorMessageInit) = 
                cardReader.InitializeWithDatabaseDetails(lockSystem, "DESKTOP-ILHONN0", "system", 0, AdelCardReader.AUTO_ENCODER, 1);
            
            LogMessage($"Init() returned: {errorCodeInit} - {errorMessageInit}");
            
            if (successInit)
            {
                LogMessage("✓ Card reader initialized successfully via Init() - Full Database Access");
                LogMessage("✓ All functions available: ReadCard, ReadCardId, ReadMagCard, ReadIC, GetCardInfo, Reader_Beep");
                UpdateStatus("Connected (Full Database Access)", Color.Green);
                EnableCardOperations(true);
            }
            else
            {
                LogMessage($"✗ Init() failed with error {errorCodeInit}");
                if (errorCodeInit == 22)
                {
                    LogMessage("Database connection failed - check SQL Server status");
                }
                else if (errorCodeInit == 33)
                {
                    LogMessage("SQL Server connection issue");
                }
                else if (errorCodeInit == 14)
                {
                    LogMessage("Parameter error - check server name or username");
                }
                UpdateStatus($"Database Error {errorCodeInit}", Color.Red);
                EnableCardOperations(false);
            }
        }
        catch (Exception ex)
        {
            HandleException("database initialization", ex);
        }
    }

    private bool TestDLLLoading()
    {
        var (canLoad, dllError) = AdelCardReader.TestDllLoading();
        if (!canLoad)
        {
            LogMessage($"✗ DLL Loading Failed: {dllError}");
            UpdateStatus("DLL Error", Color.Red);
            return false;
        }
        LogMessage("✓ MAINDLL.DLL loaded successfully");
        return true;
    }
    #endregion

    #region Card Reading Tests - Individual Function Buttons

    // Test 1: Standard ReadCard
    private void btnReadCard_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("🃏 TEST 1: STANDARD READCARD FUNCTION");
            LogMessage("Testing ReadCard(room, gate, stime, guestname, guestid, track1, track2, cardno, status)");
            
            var (readSuccess, cardInfo, errorCode, readDetails) = cardReader.TestCardReading();
            LogMessage($"ReadCard Test: {readDetails}");
            
            if (readSuccess && cardInfo != null)
            {
                LogMessage("🎉 READCARD SUCCESS!");
                DisplayCardInfo(cardInfo);
                UpdateFormFields(cardInfo);
            }
            else
            {
                LogMessage($"❌ ReadCard failed with error {errorCode}");
            }
        }
        catch (Exception ex)
        {
            HandleException("ReadCard test", ex);
        }
    }

    // Test 2: ReadCardId
    private void btnTestReadCardId_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("🆔 TEST 2: READCARDID FUNCTION");
            LogMessage("Testing ReadCardId(ref cardId)");
            
            var (idSuccess, cardId, idDetails) = cardReader.TestCardId();
            LogMessage($"ReadCardId Test: {idDetails}");
            
            if (idSuccess)
            {
                LogMessage($"✓ Card ID detected: {cardId}");
            }
        }
        catch (Exception ex)
        {
            HandleException("ReadCardId test", ex);
        }
    }

    // Test 3: ReadMagCard
    private void btnTestReadMagCard_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("🧲 TEST 3: READMAGCARD FUNCTION");
            LogMessage("Testing ReadMagCard(track1, track2, track3)");
            
            var (magSuccess, magCardInfo, errorCode, magDetails) = cardReader.TestReadMagCard();
            LogMessage($"ReadMagCard Test: {magDetails}");
            
            if (magSuccess && magCardInfo != null)
            {
                LogMessage("🎉 MAGNETIC CARD READ SUCCESS!");
                LogMessage($"Track 1: {magCardInfo.Track1}");
                LogMessage($"Track 2: {magCardInfo.Track2}");
                LogMessage($"Track 3: {magCardInfo.Track3}");
            }
            else
            {
                LogMessage($"❌ ReadMagCard failed with error {errorCode}");
            }
        }
        catch (Exception ex)
        {
            HandleException("ReadMagCard test", ex);
        }
    }

    // Test 4: ReadIC
    private void btnTestReadIC_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("💾 TEST 4: READIC FUNCTION");
            LogMessage("Testing ACTUAL ReadIC(startAddress, length, data) from SDK");
            LogMessage("For IC cards: Lock3200K, Lock4200D, Lock7200D, Adel3200, Adel9200");
            
            var (icSuccess, icCardInfo, errorCode, icDetails) = cardReader.TestReadIC(0x00, 16);
            LogMessage($"ReadIC Test: {icDetails}");
            
            if (icSuccess && icCardInfo != null)
            {
                LogMessage("🎉 IC CARD READ SUCCESS!");
                LogMessage($"Data Length: {icCardInfo.Data.Length} bytes");
                LogMessage($"Hex Data: {Convert.ToHexString(icCardInfo.Data)}");
            }
            else
            {
                LogMessage($"❌ ReadIC failed with error {errorCode}");
            }
        }
        catch (Exception ex)
        {
            HandleException("ReadIC test", ex);
        }
    }

    // Test 5: GetCardInfo
    private void btnTestGetCardInfo_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("📋 TEST 5: GETCARDINFO FUNCTION");
            LogMessage("Testing ACTUAL GetCardInfo(buffer, length, cardtype, cardstatus, cardno, room, username) from SDK");
            LogMessage("Network Energy Saver function");
            
            var (infoSuccess, cardInfoDetailed, errorCode, infoDetails) = cardReader.TestGetCardInfo();
            LogMessage($"GetCardInfo Test: {infoDetails}");
            
            if (infoSuccess && cardInfoDetailed != null)
            {
                LogMessage("🎉 CARD INFO SUCCESS!");
                LogMessage($"Card Type: {cardInfoDetailed.CardTypeDescription}");
                LogMessage($"Card Status: {cardInfoDetailed.CardStatusDescription}");
                LogMessage($"Card Number: {cardInfoDetailed.CardNumber}");
                LogMessage($"Room: {cardInfoDetailed.RoomNumber}");
                LogMessage($"User: {cardInfoDetailed.UserName}");
            }
            else
            {
                LogMessage($"❌ GetCardInfo failed with error {errorCode}");
            }
        }
        catch (Exception ex)
        {
            HandleException("GetCardInfo test", ex);
        }
    }

    // Test 6: Reader_Beep
    private async void btnTestReaderBeep_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("🔊 TEST 6: READER_BEEP FUNCTION");
            LogMessage("Testing ACTUAL Reader_Beep(soundType) from SDK");
            LogMessage("Available for: Lock9200T, A30, A90, A92");
            
            // Test different beep sounds
            var beepTypes = new[]
            {
                (AdelCardReader.BEEP_GREEN_LONG, "Green Long (11)"),
                (AdelCardReader.BEEP_RED_LONG, "Red Long (12)"),
                (AdelCardReader.BEEP_RED_SHORT_DOUBLE, "Red Short Double (15)"),
                (AdelCardReader.BEEP_SHORT_ONE, "Short One (16)")
            };

            foreach (var (beepType, description) in beepTypes)
            {
                LogMessage($"Testing {description} beep...");
                var (beepSuccess, beepDetails) = cardReader.TestReaderBeep(beepType);
                LogMessage($"  {beepDetails}");
                
                if (beepSuccess)
                {
                    LogMessage($"  ✓ {description} beep successful");
                }
                else
                {
                    LogMessage($"  ❌ {description} beep failed");
                }
                
                // Small delay between beeps
                await Task.Delay(1000);
            }
        }
        catch (Exception ex)
        {
            HandleException("Reader_Beep test", ex);
        }
    }
    
    // Direct ReadCard function handler
    private void btnReadCardDirect_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("?? DIRECT READCARD FUNCTION");
            LogMessage("Calling SDK's ReadCard() function directly with StringBuilder parameters");
            
            // We can only reach this point if the button is enabled, which means initialization was successful
            StringBuilder room = new StringBuilder(50);
            StringBuilder gate = new StringBuilder(50);
            StringBuilder stime = new StringBuilder(50);
            StringBuilder guestname = new StringBuilder(100);
            StringBuilder guestid = new StringBuilder(100);
            StringBuilder track1 = new StringBuilder(200);
            StringBuilder track2 = new StringBuilder(200);
            long cardno = 0;
            int status = 0;

            // Call the ReadCard function directly from the ADEL SDK
            LogMessage("Placing card on reader and calling ReadCard() function...");
            int result = AdelCardReader.ReadCard(room, gate, stime, guestname, guestid, track1, track2, ref cardno, ref status);
            
            LogMessage($"ReadCard Direct Result: {result} - {AdelCardReader.GetErrorDescription(result)}");
            
            if (result == AdelCardReader.SUCCESS)
            {
                LogMessage("?? DIRECT READCARD SUCCESS!");
                LogMessage("Card Information from direct ReadCard call:");
                LogMessage($"  Room Number: {room}");
                LogMessage($"  Gate: {gate}");
                LogMessage($"  Start Time: {stime}");
                LogMessage($"  Guest Name: {guestname}");
                LogMessage($"  Guest ID: {guestid}");
                LogMessage($"  Card Number: {cardno}");
                LogMessage($"  Status: {GetCardStatusDescription(status)}");
                LogMessage($"  Track 1: {track1}");
                LogMessage($"  Track 2: {track2}");
                
                // Create sound feedback for success
                try
                {
                    AdelCardReader.Reader_Beep(AdelCardReader.BEEP_GREEN_LONG);
                }
                catch
                {
                    // Ignore beep errors, not critical
                }
                
                // Update form fields with the card data
                txtRoomNumber.Text = room.ToString().Trim();
                txtGuestName.Text = guestname.ToString().Trim();
                
                // Set focus to the guest name field for convenience
                txtGuestName.Focus();
                txtGuestName.SelectAll();
            }
            else
            {
                LogMessage($"? Direct ReadCard failed with error {result}");
                
                // Create sound feedback for failure
                try
                {
                    AdelCardReader.Reader_Beep(AdelCardReader.BEEP_RED_SHORT_DOUBLE);
                }
                catch
                {
                    // Ignore beep errors, not critical
                }
                
                LogMessage($"Partial data - Room: '{room}', Guest: '{guestname}', Card#: {cardno}, Status: {status}");
                
                // Show specific error messages based on error code
                switch (result)
                {
                    case AdelCardReader.NO_CARD:
                        LogMessage("No card detected on the reader. Please place a card.");
                        break;
                    case AdelCardReader.CARD_DAMAGED:
                        LogMessage("Card appears to be damaged or unreadable.");
                        break;
                    case AdelCardReader.READ_WRITE_ERROR:
                        LogMessage("Read error occurred. Try repositioning the card.");
                        break;
                    case AdelCardReader.NON_SYSTEM_CARD:
                        LogMessage("This is not a valid system card for this lock system.");
                        break;
                    case AdelCardReader.NOT_GUEST_CARD:
                        LogMessage("This is not a guest card.");
                        break;
                    case AdelCardReader.SERIAL_PORT_ERROR:
                        LogMessage("Serial port error. Check reader connection and try again.");
                        break;
                    default:
                        LogMessage($"Error code {result}: {AdelCardReader.GetErrorDescription(result)}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            HandleException("Direct ReadCard test", ex);
        }
    }

    // View JBB Table button click handler
    private void btnViewJBBTable_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("📊 Opening JBB Table Viewer...");
            
            // Test database connection first
            bool isConnected = DatabaseHelper.TestDatabaseConnection();
            if (!isConnected)
            {
                LogMessage("❌ Database connection failed. Make sure SQL Server is running and the Adel9200 database exists.");
                MessageBox.Show("Failed to connect to the Adel9200 database. Please check your SQL Server connection.", 
                    "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Open the JBB Table form
            using (var jbbTableForm = new JBBTableForm())
            {
                LogMessage("✓ JBB Table Viewer opened");
                jbbTableForm.ShowDialog();
            }
            
            LogMessage("✓ JBB Table Viewer closed");
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Error opening JBB Table Viewer: {ex.Message}");
            MessageBox.Show($"An error occurred while opening the JBB Table Viewer: {ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // View XTCS Table button click handler
    private void btnViewXTCSTable_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("📊 Opening XTCS Table Viewer (Authorization Codes)...");
            
            // Test database connection first
            bool isConnected = DatabaseHelper.TestDatabaseConnection();
            if (!isConnected)
            {
                LogMessage("❌ Database connection failed. Make sure SQL Server is running and the Adel9200 database exists.");
                MessageBox.Show("Failed to connect to the Adel9200 database. Please check your SQL Server connection.", 
                    "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // Open the XTCS Table form
            using (var xtcsTableForm = new XTCSTableForm())
            {
                LogMessage("✓ XTCS Table Viewer opened - This table contains authorization codes");
                xtcsTableForm.ShowDialog();
            }
            
            LogMessage("✓ XTCS Table Viewer closed");
        }
        catch (Exception ex)
        {
            LogMessage($"❌ Error opening XTCS Table Viewer: {ex.Message}");
            MessageBox.Show($"An error occurred while opening the XTCS Table Viewer: {ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    #endregion

    #region Original Card Operations
    private void btnCreateCard_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text))
            {
                LogMessage("✗ Please enter a room number");
                return;
            }

            string roomNumber = txtRoomNumber.Text.Trim();
            string guestName = txtGuestName.Text.Trim();
            DateTime startTime = DateTime.Now;
            DateTime endTime = DateTime.Now.AddDays(1);

            LogMessage($"Creating guest card: Room {roomNumber}, Guest {guestName}");
            LogMessage($"Valid: {startTime:yyyy-MM-dd HH:mm} to {endTime:yyyy-MM-dd HH:mm}");

            bool success = cardReader.CreateGuestCard(roomNumber, guestName, "", startTime, endTime);
            LogMessage(success ? "✓ Guest card created successfully" : "✗ Failed to create guest card");
        }
        catch (Exception ex)
        {
            HandleException("card creation", ex);
        }
    }

    private void btnTestAllSystems_Click(object sender, EventArgs e)
    {
        try
        {
            LogMessage("🔍 TESTING A90 AND A92 WITH SERVER AUTHENTICATION");
            LogMessage("Server: DESKTOP-ILHONN0, User: EFD4DCC8D485F8");
            LogMessage("Port: 0, Encoder: Manual, TM Encoder: 1");
            
            // Test A90 with server authentication
            LogMessage("\n🎯 Testing A90 Lock System...");
            var (a90Success, a90CardInfo, a90Log) = cardReader.TestA90WithServerAuth();
            
            // Show A90 test log
            var a90Lines = a90Log.Split('\n');
            foreach (var line in a90Lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    LogMessage(line.Trim());
            }
            
            if (a90Success && a90CardInfo != null)
            {
                LogMessage("\n🎉 A90 SUCCESS! Card reading works with server authentication!");
                DisplayCardInfo(a90CardInfo);
                UpdateFormFields(a90CardInfo);
                LogMessage("✅ Use A90 lock system for future operations");
                return;
            }
            
            // Test A92 with server authentication
            LogMessage("\n🎯 Testing A92 Lock System...");
            var (a92Success, a92CardInfo, a92Log) = cardReader.TestA92WithServerAuth();
            
            // Show A92 test log
            var a92Lines = a92Log.Split('\n');
            foreach (var line in a92Lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    LogMessage(line.Trim());
            }
            
            if (a92Success && a92CardInfo != null)
            {
                LogMessage("\n🎉 A92 SUCCESS! Card reading works with server authentication!");
                DisplayCardInfo(a92CardInfo);
                UpdateFormFields(a92CardInfo);
                LogMessage("✅ Use A92 lock system for future operations");
                return;
            }
            
            // Summary if both failed
            LogMessage("\n❌ BOTH A90 AND A92 TESTS COMPLETED");
            LogMessage("\n🔍 SUMMARY:");
            
            if (!a90Success && !a92Success)
            {
                LogMessage("✗ A90 initialization failed");
                LogMessage("✗ A92 initialization failed");
                LogMessage("\n💡 POSSIBLE ISSUES:");
                LogMessage("1. Server 'DESKTOP-ILHONN0' not accessible");
                LogMessage("2. Username 'EFD4DCC8D485F8' not valid or encrypted");
                LogMessage("3. SQL Server service not running");
                LogMessage("4. Database permissions issue");
                LogMessage("5. Network connectivity problem");
            }
            else
            {
                LogMessage("ℹ️ Initialization succeeded but card reading failed");
                LogMessage("Check:");
                LogMessage("1. Card is inserted properly");
                LogMessage("2. Card type matches the lock system");
                LogMessage("3. Card is not expired or damaged");
            }
        }
        catch (Exception ex)
        {
            HandleException("A90/A92 server authentication testing", ex);
        }
    }
    #endregion

    #region Helper Methods
    private void DisplayCardInfo(AdelCardReader.CardInfo cardInfo)
    {
        LogMessage("✓ Card Information:");
        LogMessage($"  Room Number: {cardInfo.Room}");
        LogMessage($"  Guest Name: {cardInfo.GuestName}");
        LogMessage($"  Guest ID: {cardInfo.GuestId}");
        LogMessage($"  Card Number: {cardInfo.CardNumber}");
        LogMessage($"  Valid From: {cardInfo.StartTime}");
        LogMessage($"  Valid Until: {cardInfo.EndTime}");
        LogMessage($"  Status: {GetCardStatusDescription(cardInfo.Status)}");
        LogMessage($"  Track 1: {cardInfo.Track1}");
        LogMessage($"  Track 2: {cardInfo.Track2}");
    }

    private void UpdateFormFields(AdelCardReader.CardInfo cardInfo)
    {
        txtRoomNumber.Text = cardInfo.Room;
        txtGuestName.Text = cardInfo.GuestName;
    }

    private void UpdateStatus(string status, Color color)
    {
        lblStatus.Text = $"Status: {status}";
        lblStatus.ForeColor = color;
    }

    private void HandleException(string operation, Exception ex)
    {
        LogMessage($"✗ Error in {operation}: {ex.Message}");
        LogMessage($"Exception type: {ex.GetType().Name}");
        UpdateStatus("Exception", Color.Red);
        EnableCardOperations(false);
    }

    private void EnableCardOperations(bool enabled)
    {
        // Enable/disable all card operation buttons
        btnReadCard.Enabled = enabled;
        btnCreateCard.Enabled = enabled;
        btnReadCardDirect.Enabled = enabled;
        btnTestReadCardId.Enabled = enabled;
        btnTestReadMagCard.Enabled = enabled;
        btnTestReadIC.Enabled = enabled;
        btnTestGetCardInfo.Enabled = enabled;
        btnTestReaderBeep.Enabled = enabled;
        btnTestAllSystems.Enabled = enabled;
        btnTestAuth.Enabled = enabled; // This is our "Change User to Admin" button
    }


    private int GetSelectedLockSystem() => cmbLockSystem.SelectedIndex switch
    {
        0 => AdelCardReader.A90,
        1 => AdelCardReader.A92,
        2 => AdelCardReader.A30,
        3 => AdelCardReader.LOCK9200T,
        4 => AdelCardReader.LOCK3200K,
        _ => AdelCardReader.A90
    };

    private string GetCardStatusDescription(int status) => status switch
    {
        1 => "Normal use",
        3 => "Normal logout",
        4 => "Lost logout",
        5 => "Destroy logout",
        6 => "Auto logout",
        _ => $"Unknown status ({status})"
    };

    private void LogMessage(string message)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        txtOutput.AppendText($"[{timestamp}] {message}\r\n");
        txtOutput.ScrollToCaret();
        Application.DoEvents();
    }

    private void btnClearLog_Click(object sender, EventArgs e)
    {
        txtOutput.Clear();
        LogMessage("Log cleared");
    }
    #endregion

    // Focused authentication test with ReadCard validation
    private void btnTestAuth_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TestDLLLoading()) return;
            
            LogMessage("👤 Changing user name to \"admin\"...");
            
            // Call the ChangeUser function with "admin" as the username
            AdelCardReader.ChangeUser("admin");
            
            LogMessage("✅ User successfully changed to \"admin\"");
            LogMessage("The lock system will now record operations with this username");
        }
        catch (Exception ex)
        {
            HandleException("changing user", ex);
        }
    }

    // ChangeUser button handler
    private void btnChangeUser_Click(object sender, EventArgs e)
    {
        try
        {
            if (!TestDLLLoading()) return;
            
            LogMessage("👤 Changing user name to \"admin\"...");
            
            // Call the ChangeUser function with "admin" as the username
            AdelCardReader.ChangeUser("admin");
            
            LogMessage("✅ User successfully changed to \"admin\"");
            LogMessage("The lock system will now record operations with this username");
        }
        catch (Exception ex)
        {
            HandleException("changing user", ex);
        }
    }
}

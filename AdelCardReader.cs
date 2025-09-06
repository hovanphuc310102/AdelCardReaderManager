using System.Runtime.InteropServices;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace AdelCardReaderManager
{
    public class AdelCardReader
    {
        #region Constants
        // Lock system constants
        public const int LOCK3200 = 1, LOCK3200K = 2, LOCK4200 = 3, LOCK4200D = 4;
        public const int LOCK5200_A50 = 5, LOCK6200 = 6, LOCK7200 = 7, LOCK7200D = 8;
        public const int LOCK9200 = 9, LOCK9200T = 10, A30 = 11, A50 = 14, A90 = 18, A92 = 22;

        // Encoder types
        public const int MANUAL_ENCODER = 0, AUTO_ENCODER = 1, MSR206_MAGNETIC = 2;

        // Return codes
        public const int SUCCESS = 0, READ_WRITE_ERROR = 1, CARD_DAMAGED = 2, NO_CARD = 3;
        public const int SERIAL_PORT_ERROR = 4, CARD_REPLACED = 5, NO_NEW_CARD = 6, NEW_CARD = 7;
        public const int NON_SYSTEM_CARD = 8, NOT_GUEST_CARD = 9;

        // Reader_Beep sound constants
        public const int BEEP_GREEN_LONG = 11;      // Green light for 1 second, long scream
        public const int BEEP_RED_LONG = 12;        // Red light for 1 second, long scream
        public const int BEEP_RED_SHORT_DOUBLE = 15; // Red light for 1 second, short scream for two
        public const int BEEP_SHORT_ONE = 16;       // Short for one
        #endregion

        #region Global Variables
        private bool _isInitialized = false;
        private bool _isInitWithDatabase = false;
        #endregion

        #region P/Invoke Declarations - ALL SDK Functions
        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int Init(int software, string server, string username, int port, int encoder, int tmEncoder);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int EndSession();

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetPort(int software, int port, int encoder, int tmEncoder);

        // MAIN READCARD FUNCTIONS FROM SDK MANUAL
        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int ReadCard(StringBuilder room, StringBuilder gate, StringBuilder stime, 
            StringBuilder guestname, StringBuilder guestid, StringBuilder track1, StringBuilder track2, 
            ref long cardno, ref int st);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadCardId(ref uint pid);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int ReadMagCard(StringBuilder track1, StringBuilder track2, StringBuilder track3);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadIC(uint start, uint len, byte[] str);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int GetCardInfo(byte[] buff, int len, ref int cardtype, ref int cardst, 
            ref int cardno, StringBuilder roomno, StringBuilder username);

        // Reader_Beep function (Lock9200T, A30, A90, A92)
        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int Reader_Beep(int Sound);

        // Additional SDK functions
        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int NewKey(string room, string gate, string stime, string guestname, 
            string guestid, int overflag, ref long cardno, string track1, string track2);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteIC(uint start, uint len, byte[] str);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int WriteMagCard(string track1, string track2, string track3);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int CheckSC(byte[] sc);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int PopCard();

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern void ChangeUser(string username);
        #endregion

        #region Helper Classes
        public class CardInfo
        {
            public string Room { get; set; } = "";
            public string Gate { get; set; } = "";
            public string StartTime { get; set; } = "";
            public string EndTime { get; set; } = "";
            public string GuestName { get; set; } = "";
            public string GuestId { get; set; } = "";
            public long CardNumber { get; set; }
            public int Status { get; set; }
            public string Track1 { get; set; } = "";
            public string Track2 { get; set; } = "";
        }

        public class MagCardInfo
        {
            public string Track1 { get; set; } = "";
            public string Track2 { get; set; } = "";
            public string Track3 { get; set; } = "";
        }

        public class ICCardInfo
        {
            public byte[] Data { get; set; } = new byte[0];
            public uint StartAddress { get; set; }
            public uint Length { get; set; }
        }

        public class CardInfoDetailed
        {
            public int CardType { get; set; }
            public string CardTypeDescription { get; set; } = "";
            public int CardStatus { get; set; }
            public string CardStatusDescription { get; set; } = "";
            public int CardNumber { get; set; }
            public string RoomNumber { get; set; } = "";
            public string UserName { get; set; } = "";
        }
        #endregion

        #region Core Methods
        public static (bool canLoad, string error) TestDllLoading()
        {
            try
            {
                EndSession();
                return (true, "DLL loaded successfully");
            }
            catch (Exception ex) 
            { 
                return (false, $"DLL error: {ex.Message}"); 
            }
        }

        // SetPort initialization (hardware only - no database)
        public (bool success, int errorCode, string errorMessage) InitializeStandaloneWithDetails(
            int lockSystem = A90, int comPort = 0, int encoderType = AUTO_ENCODER, int tmEncoder = 1)
        {
            try
            {
                
                int result = SetPort(lockSystem, comPort, encoderType, tmEncoder);
                _isInitialized = (result == SUCCESS);
                _isInitWithDatabase = false; // SetPort doesn't provide database access
                return (_isInitialized, result, GetErrorDescription(result));
            }
            catch (Exception ex) 
            { 
                return (false, -1, $"Exception: {ex.Message}"); 
            }
        }

        // Init with database connection (required for GetCardInfo and database functions)
        public (bool success, int errorCode, string errorMessage) InitializeWithDatabaseDetails(
            int lockSystem = A90, string server = "DESKTOP-ILHONN0", string username = "system", 
            int comPort = 0, int encoderType = MANUAL_ENCODER, int tmEncoder = 1)
        {
            int setPortResult = SetPort(lockSystem, comPort, encoderType, tmEncoder);
            if(setPortResult == SUCCESS)
            {
                try
                {
                    EndSession();
                    int testLockSystem = 18;
                    string testUsername = null;
                    string testServer = "DESKTOP-ILHONN0";
                    //ChangeUser(testUsername);

                    // Use our debug diagnostic wrapper instead of direct Init call
                    int result = Init(testLockSystem, testServer, testUsername, comPort, encoderType, tmEncoder);

                    if (result==SUCCESS)
                    {
                        _isInitialized = true;
                        _isInitWithDatabase = true;
                        return (true, result, $"Success with server: '{testServer}', username: '{testUsername}'");
                    }
                    else
                    {
                        _isInitialized = false;
                        _isInitWithDatabase = false;
                        
                        // If we got error 33 (Invalid authorization code), log the detailed diagnostic info
                        if (result == 33)
                        {
                            return (false, result, $"Invalid authorization code. Diagnostic info:\n{GetErrorDescription(result)}");
                        }
                        
                        return (false, result, GetErrorDescription(result));
                    }
                }
                catch (Exception ex)
                {
                    return (false, -1, $"Exception during Init: {ex.Message}");
                }
            }
            else
            {
                _isInitialized = false;
                _isInitWithDatabase = false;
                return (false, setPortResult, GetErrorDescription(setPortResult));
            }
        }

        public void Shutdown()
        {
            if (_isInitialized)
            {
                EndSession();
                _isInitialized = false;
                _isInitWithDatabase = false;
            }
        }
        #endregion

        #region ACTUAL SDK ReadCard Function Tests

        // Test 1: Standard ReadCard function from SDK
        public (bool success, CardInfo? cardInfo, int errorCode, string details) TestReadCard()
        {
            if (!_isInitialized) 
                return (false, null, -1, "SDK not initialized - call SetPort or Init first");

            try
            {
                var room = new StringBuilder(50);
                var gate = new StringBuilder(50);
                var stime = new StringBuilder(50);
                var guestname = new StringBuilder(100);
                var guestid = new StringBuilder(100);
                var track1 = new StringBuilder(200);
                var track2 = new StringBuilder(200);
                long cardno = 0;
                int status = 0;

                int result = ReadCard(room, gate, stime, guestname, guestid, track1, track2, ref cardno, ref status);
                
                string details = $"ReadCard returned: {result} - {GetErrorDescription(result)}";
                
                if (result == SUCCESS)
                {
                    var cardInfo = new CardInfo
                    {
                        Room = room.ToString().Trim(),
                        Gate = gate.ToString().Trim(),
                        StartTime = stime.ToString().Trim(),
                        EndTime = "",
                        GuestName = guestname.ToString().Trim(),
                        GuestId = guestid.ToString().Trim(),
                        CardNumber = cardno,
                        Status = status,
                        Track1 = track1.ToString().Trim(),
                        Track2 = track2.ToString().Trim()
                    };
                    
                    details += $"\nCard Data - Room: '{cardInfo.Room}', Guest: '{cardInfo.GuestName}', Card#: {cardInfo.CardNumber}, Status: {cardInfo.Status}";
                    return (true, cardInfo, result, details);
                }
                else
                {
                    details += $"\nPartial data - Room: '{room}', Guest: '{guestname}', Card#: {cardno}, Status: {status}";
                    return (false, null, result, details);
                }
            }
            catch (Exception ex)
            {
                return (false, null, -1, $"Exception in ReadCard: {ex.Message}");
            }
        }

        // Test 2: ReadCardId function from SDK
        public (bool success, uint cardId, string details) TestReadCardId()
        {
            if (!_isInitialized) 
                return (false, 0, "SDK not initialized - call SetPort or Init first");

            try
            {
                uint cardId = 0;
                int result = ReadCardId(ref cardId);
                
                string message = result switch
                {
                    SUCCESS => $"ReadCardId SUCCESS: Card ID = {cardId}",
                    NO_CARD => "ReadCardId: No card detected in reader",
                    SERIAL_PORT_ERROR => "ReadCardId: Serial port error - check hardware connection",
                    _ => $"ReadCardId Error {result}: {GetErrorDescription(result)}"
                };
                
                return (result == SUCCESS, cardId, message);
            }
            catch (Exception ex)
            {
                return (false, 0, $"Exception in ReadCardId: {ex.Message}");
            }
        }

        // Test 3: ReadMagCard function from SDK
        public (bool success, MagCardInfo? magCardInfo, int errorCode, string details) TestReadMagCard()
        {
            if (!_isInitialized) 
                return (false, null, -1, "SDK not initialized - call SetPort or Init first");

            try
            {
                var track1 = new StringBuilder(200);
                var track2 = new StringBuilder(200);
                var track3 = new StringBuilder(200);

                int result = ReadMagCard(track1, track2, track3);
                
                string details = $"ReadMagCard returned: {result} - {GetErrorDescription(result)}";
                
                if (result == SUCCESS)
                {
                    var magCardInfo = new MagCardInfo
                    {
                        Track1 = track1.ToString().Trim(),
                        Track2 = track2.ToString().Trim(),
                        Track3 = track3.ToString().Trim()
                    };
                    
                    details += $"\nMagnetic Card Data:";
                    details += $"\n  Track 1: '{magCardInfo.Track1}'";
                    details += $"\n  Track 2: '{magCardInfo.Track2}'";
                    details += $"\n  Track 3: '{magCardInfo.Track3}'";
                    return (true, magCardInfo, result, details);
                }
                else
                {
                    details += $"\nFailed to read magnetic card tracks";
                    details += $"\n  Track 1: '{track1}'";
                    details += $"\n  Track 2: '{track2}'";
                    details += $"\n  Track 3: '{track3}'";
                    return (false, null, result, details);
                }
            }
            catch (Exception ex)
            {
                return (false, null, -1, $"Exception in ReadMagCard: {ex.Message}");
            }
        }

        // Test 4: ReadIC function from SDK (for Lock3200K, Lock4200D, Lock7200D, Adel3200, Adel9200)
        public (bool success, ICCardInfo? icCardInfo, int errorCode, string details) TestReadIC(uint startAddress = 0x00, uint length = 16)
        {
            if (!_isInitialized) 
                return (false, null, -1, "SDK not initialized - call SetPort or Init first");

            try
            {
                byte[] data = new byte[length];
                
                int result = ReadIC(startAddress, length, data);
                
                string details = $"ReadIC(0x{startAddress:X4}, {length}) returned: {result} - {GetErrorDescription(result)}";
                
                if (result == SUCCESS)
                {
                    var icCardInfo = new ICCardInfo
                    {
                        Data = data,
                        StartAddress = startAddress,
                        Length = length
                    };
                    
                    string hexData = Convert.ToHexString(data);
                    details += $"\nIC Card Data Read SUCCESS:";
                    details += $"\n  Start Address: 0x{startAddress:X4}";
                    details += $"\n  Length: {length} bytes";
                    details += $"\n  Hex Data: {hexData}";
                    details += $"\n  ASCII: {System.Text.Encoding.ASCII.GetString(data).Replace('\0', '.')}";

                    // Attempt to convert to UTF8 and fall back to ASCII if exception
                    try
                    {
                        string utf8String = System.Text.Encoding.UTF8.GetString(data);
                        details += $"\n  UTF8: {utf8String}";
                    }
                    catch
                    {
                        details += $"\n  UTF8 conversion failed, using ASCII instead";
                    }

                    return (true, icCardInfo, result, details);
                }
                else
                {
                    details += $"\nFailed to read IC card data from address 0x{startAddress:X4}";
                    details += $"\nNote: ReadIC requires IC card support (Lock3200K, Lock4200D, Lock7200D, Adel3200, Adel9200)";
                    return (false, null, result, details);
                }
            }
            catch (Exception ex)
            {
                return (false, null, -1, $"Exception in ReadIC: {ex.Message}");
            }
        }

        // Test 5: GetCardInfo function from SDK (Network Energy Saver function) - REQUIRES Init()
        public (bool success, CardInfoDetailed? cardInfoDetailed, int errorCode, string details) TestGetCardInfo()
        {
            // GetCardInfo requires Init() to be called, not just SetPort()
            if (!_isInitWithDatabase) 
                return (false, null, 20, "GetCardInfo requires Init() with database connection - call InitializeWithDatabaseDetails() first");

            try
            {
                byte[] buff = new byte[256]; // Card data buffer
                int cardtype = 0, cardst = 0, cardno = 0;
                var roomno = new StringBuilder(20);
                var username = new StringBuilder(50);

                int result = GetCardInfo(buff, buff.Length, ref cardtype, ref cardst, ref cardno, roomno, username);
                
                string details = $"GetCardInfo returned: {result} - {GetErrorDescription(result)}";
                
                string cardTypeDesc = cardtype switch
                {
                    1 => "System card",
                    2 => "Layer card", 
                    3 => "Program card",
                    4 => "Master card",
                    5 => "Clock card",
                    6 => "Foreman card",
                    7 => "Floor card",
                    8 => "Maid card",
                    9 => "Guest card",
                    10 => "Lockout card",
                    11 => "Meeting card",
                    12 => "Emergency card",
                    13 => "Inhibit card",
                    14 => "Spare card",
                    _ => $"Unknown type ({cardtype})"
                };

                string cardStatusDesc = cardst switch
                {
                    1 => "Normal",
                    3 => "Erased",
                    4 => "Lost",
                    5 => "Damaged",
                    6 => "Expired",
                    _ => $"Unknown status ({cardst})"
                };
                
                if (result == SUCCESS)
                {
                    var cardInfoDetailed = new CardInfoDetailed
                    {
                        CardType = cardtype,
                        CardTypeDescription = cardTypeDesc,
                        CardStatus = cardst,
                        CardStatusDescription = cardStatusDesc,
                        CardNumber = cardno,
                        RoomNumber = roomno.ToString().Trim(),
                        UserName = username.ToString().Trim()
                    };
                    
                    details += $"\nGetCardInfo SUCCESS (with Init database connection):";
                    details += $"\n  Card Type: {cardTypeDesc} ({cardtype})";
                    details += $"\n  Card Status: {cardStatusDesc} ({cardst})";
                    details += $"\n  Card Number: {cardno}";
                    details += $"\n  Room Number: '{roomno}'";
                    details += $"\n  User Name: '{username}'";
                    return (true, cardInfoDetailed, result, details);
                }
                else if (result == 20)
                {
                    details += $"\nError 20: Please call Init function first";
                    details += $"\nGetCardInfo requires Init() with database connection, not just SetPort()";
                    details += $"\nUse InitializeWithDatabaseDetails() method first";
                    return (false, null, result, details);
                }
                else
                {
                    details += $"\nGetCardInfo failed - Network Energy Saver function";
                    details += $"\n  Card Type: {cardTypeDesc} ({cardtype})";
                    details += $"\n  Card Status: {cardStatusDesc} ({cardst})";
                    details += $"\n  Card Number: {cardno}";
                    return (false, null, result, details);
                }
            }
            catch (Exception ex)
            {
                return (false, null, -1, $"Exception in GetCardInfo: {ex.Message}");
            }
        }

        // Test 6: Reader_Beep function from SDK (Lock9200T, A30, A90, A92)
        public (bool success, string details) TestReaderBeep(int soundType = BEEP_GREEN_LONG)
        {
            if (!_isInitialized) 
                return (false, "SDK not initialized - call SetPort or Init first");

            try
            {
                int result = Reader_Beep(soundType);
                
                string soundDesc = soundType switch
                {
                    BEEP_GREEN_LONG => "Green light for 1 second, long scream",
                    BEEP_RED_LONG => "Red light for 1 second, long scream", 
                    BEEP_RED_SHORT_DOUBLE => "Red light for 1 second, short scream for two",
                    BEEP_SHORT_ONE => "Short for one",
                    _ => $"Custom sound type {soundType}"
                };
                
                string details = $"Reader_Beep({soundType}) - {soundDesc}";
                details += $"\nResult: {result} - {GetErrorDescription(result)}";
                
                if (result == SUCCESS)
                {
                    details += $"\n🔊 BEEP SUCCESS! Sound played: {soundDesc}";
                }
                else
                {
                    details += $"\n❌ BEEP FAILED! Available for: Lock9200T, A30, A90, A92";
                }
                
                return (result == SUCCESS, details);
            }
            catch (Exception ex)
            {
                return (false, $"Exception in Reader_Beep: {ex.Message}");
            }
        }
        #endregion

        #region Backward Compatibility Methods
        public (bool success, uint cardId, string details) TestCardId()
        {
            return TestReadCardId();
        }

        public (bool success, CardInfo? cardInfo, int errorCode, string details) TestCardReading()
        {
            return TestReadCard();
        }

        public (bool success, string workingConfig, CardInfo? cardInfo, string log) TestAllLockSystems()
        {
            var log = new StringBuilder();
            log.AppendLine("=== TESTING ALL LOCK SYSTEMS (SIMPLE) ===");
            log.AppendLine("Using SetPort with port 0 as requested");
            
            var lockSystems = new []
            {
                (A90, "A90"),
                (A92, "A92"),
                (LOCK9200T, "Lock9200T"),
                (A30, "A30"),
                (LOCK9200, "Lock9200")
            };

            foreach (var (lockCode, lockName) in lockSystems)
            {
                log.AppendLine($"\nTesting {lockName} ({lockCode}):");
                
                try
                {
                    int result = SetPort(lockCode, 0, AUTO_ENCODER, 1);
                    log.AppendLine($"  SetPort result: {result} - {GetErrorDescription(result)}");
                    
                    if (result == SUCCESS)
                    {
                        _isInitialized = true;
                        log.AppendLine($"  ✓ {lockName} initialization successful");
                        
                        // Test card ID first
                        var (idSuccess, cardId, idDetails) = TestReadCardId();
                        log.AppendLine($"  Card ID test: {idDetails}");
                        
                        // Test card reading
                        var (readSuccess, cardInfo, errorCode, readDetails) = TestReadCard();
                        log.AppendLine($"  Card reading: {readDetails}");
                        
                        if (readSuccess && cardInfo != null)
                        {
                            log.AppendLine($"  🎉 SUCCESS! {lockName} can read cards!");
                            return (true, lockName, cardInfo, log.ToString());
                        }
                        else if (idSuccess)
                        {
                            log.AppendLine($"  ✓ Hardware works with {lockName}, ReadCard error: {errorCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.AppendLine($"  Exception: {ex.Message}");
                }
            }
            
            log.AppendLine("\n=== TEST COMPLETE ===");
            return (false, "None", null, log.ToString());
        }

        public bool CreateGuestCard(string roomNumber, string guestName, string guestId, 
            DateTime startTime, DateTime endTime, string publicChannels = "00", bool overwrite = true)
        {
            if (!_isInitialized) return false;

            try
            {
                string timeFormat = $"{startTime:yyyyMMddHHmm}{endTime:yyyyMMddHHmm}";
                long cardno = 0;
                int result = NewKey(roomNumber, publicChannels, timeFormat, guestName, guestId, 
                    overwrite ? 1 : 0, ref cardno, null!, null!);
                return result == SUCCESS;
            }
            catch 
            { 
                return false; 
            }
        }

        // Test A90 lock system with specific server authentication
        public (bool success, CardInfo? cardInfo, string log) TestA90WithServerAuth()
        {
            var log = new StringBuilder();
            log.AppendLine("=== TESTING A90 WITH SERVER AUTHENTICATION ===");
            log.AppendLine("Server: DESKTOP-ILHONN0");
            log.AppendLine("Username: EFD4DCC8D485F8");
            log.AppendLine("Port: 0, Encoder: 0 (Manual), TM Encoder: 1");
            
            try
            {
                // End any existing session
                EndSession();
                
                // Test A90 with your specific parameters using Init() for database access
                log.AppendLine("\nTesting A90 with Init() for database access:");
                log.AppendLine("Init(A90, \"DESKTOP-ILHONN0\", \"TheHerriott\", 0, MANUAL_ENCODER, 1)");
                ChangeUser("TheHerriott");
                int initResult = Init(A90, "(local)", "TheHerriott", 0, AUTO_ENCODER, 1);
                log.AppendLine($"Init result: {initResult} - {GetErrorDescription(initResult)}");
                
                if (initResult == SUCCESS)
                {
                    log.AppendLine("✓ A90 initialization successful with server authentication");
                    _isInitialized = true;
                    _isInitWithDatabase = true;
                    
                    // Test card ID reading
                    var (idSuccess, cardId, idDetails) = TestReadCardId();
                    log.AppendLine($"Card ID test: {idDetails}");
                    
                    // Test card reading
                    var (readSuccess, cardInfo, errorCode, readDetails) = TestReadCard();
                    log.AppendLine($"Card reading: {readDetails}");
                    
                    if (readSuccess && cardInfo != null)
                    {
                        log.AppendLine("🎉 A90 SERVER AUTH SUCCESS! Card reading works!");
                        return (true, cardInfo, log.ToString());
                    }
                    else
                    {
                        log.AppendLine($"A90 init successful but ReadCard failed: {errorCode}");
                        if (idSuccess)
                        {
                            log.AppendLine("Hardware connection confirmed, ReadCard issue with card/system compatibility");
                        }
                    }
                }
                else
                {
                    log.AppendLine($"A90 initialization failed: {initResult} - {GetErrorDescription(initResult)}");
                    
                    if (initResult == 22)
                    {
                        log.AppendLine("Database connection issue - server found but connection failed");
                    }
                    else if (initResult == 33)
                    {
                        log.AppendLine("SQL Server connection issue");
                    }
                    else if (initResult == 14)
                    {
                        log.AppendLine("Parameter error - check username or server name");
                    }
                }
                
                log.AppendLine("\n=== A90 SERVER AUTH TEST COMPLETE ===");
                return (false, null, log.ToString());
            }
            catch (Exception ex)
            {
                log.AppendLine($"Exception in A90 server auth test: {ex.Message}");
                return (false, null, log.ToString());
            }
        }

        // Test A92 lock system with specific server authentication
        public (bool success, CardInfo? cardInfo, string log) TestA92WithServerAuth()
        {
            var log = new StringBuilder();
            log.AppendLine("=== TESTING A92 WITH SERVER AUTHENTICATION ===");
            log.AppendLine("Server: DESKTOP-ILHONN0");
            log.AppendLine("Username: EFD4DCC8D485F8");
            log.AppendLine("Port: 0, Encoder: 0 (Manual), TM Encoder: 1");
            
            try
            {
                // End any existing session
                EndSession();
                
                // Test A92 with your specific parameters using Init() for database access
                log.AppendLine("\nTesting A92 with Init() for database access:");
                log.AppendLine("Init(A92, \"DESKTOP-ILHONN0\", \"EFD4DCC8D485F8\", 0, MANUAL_ENCODER, 1)");
                
                int initResult = Init(A92, "DESKTOP-ILHONN0", "EFD4DCC8D485F8", 0, MANUAL_ENCODER, 1);
                log.AppendLine($"Init result: {initResult} - {GetErrorDescription(initResult)}");
                
                if (initResult == SUCCESS)
                {
                    log.AppendLine("✓ A92 initialization successful with server authentication");
                    _isInitialized = true;
                    _isInitWithDatabase = true;
                    
                    // Test card ID reading
                    var (idSuccess, cardId, idDetails) = TestReadCardId();
                    log.AppendLine($"Card ID test: {idDetails}");
                    
                    // Test card reading
                    var (readSuccess, cardInfo, errorCode, readDetails) = TestReadCard();
                    log.AppendLine($"Card reading: {readDetails}");
                    
                    if (readSuccess && cardInfo != null)
                    {
                        log.AppendLine("🎉 A92 SERVER AUTH SUCCESS! Card reading works!");
                        return (true, cardInfo, log.ToString());
                    }
                    else
                    {
                        log.AppendLine($"A92 init successful but ReadCard failed: {errorCode}");
                        if (idSuccess)
                        {
                            log.AppendLine("Hardware connection confirmed, ReadCard issue with card/system compatibility");
                        }
                    }
                }
                else
                {
                    log.AppendLine($"A92 initialization failed: {initResult} - {GetErrorDescription(initResult)}");
                    
                    if (initResult == 22)
                    {
                        log.AppendLine("Database connection issue - server found but connection failed");
                    }
                    else if (initResult == 33)
                    {
                        log.AppendLine("SQL Server connection issue");
                    }
                    else if (initResult == 14)
                    {
                        log.AppendLine("Parameter error - check username or server name");
                    }
                }
                
                log.AppendLine("\n=== A92 SERVER AUTH TEST COMPLETE ===");
                return (false, null, log.ToString());
            }
            catch (Exception ex)
            {
                log.AppendLine($"Exception in A92 server auth test: {ex.Message}");
                return (false, null, log.ToString());
            }
        }
        #endregion

        public static string GetErrorDescription(int errorCode) => errorCode switch
        {
            SUCCESS => "Operation successful",
            READ_WRITE_ERROR => "Read/write error or data error",
            CARD_DAMAGED => "Card is damaged",
            NO_CARD => "No card detected",
            SERIAL_PORT_ERROR => "Serial port error",
            CARD_REPLACED => "Card is replaced",
            NO_NEW_CARD => "No new card",
            NEW_CARD => "New card detected",
            NON_SYSTEM_CARD => "Non-system card",
            NOT_GUEST_CARD => "Not a guest card",
            10 => "Card format error",
            11 => "Invalid card data",
            12 => "Card encryption error",
            13 => "Card authentication failed",
            14 => "Card expired",
            15 => "Card access denied",
            16 => "Card locked",
            17 => "Card invalid checksum",
            18 => "Card wrong type",
            19 => "Card initialization error",
            20 => "Please call Init function first",
            21 => "Card data corrupted",
            22 => "Failed to connect to database",
            23 => "Lock parameter not exist",
            24 => "Failed to initialize",
            25 => "No guest/Specified guest not exist",
            26 => "Guest room full",
            27 => "No records of the card",
            28 => "Please call SetPort function first",
            29 => "Invalid room number",
            30 => "Incorrect time range",
            31 => "Failed to register the existing card number (Lock9200)",
            32 => "Unavailable to call",
            33 => "Invalid authorization code",
            34 => "Network connection error",
            35 => "Authentication server error",
            _ => $"Unknown error code: {errorCode}"
        };

        //#region Diagnostic Functions
        //        public (string diagnosticInfo, bool hasMDAC, bool hasSqlDrivers) DiagnoseInitRequirements()
        //        {
        //            StringBuilder info = new StringBuilder();

        //            // Check if running as 32-bit or 64-bit
        //            info.AppendLine($"Process Architecture: {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}");
        //            info.AppendLine($"OS Architecture: {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}");

        //            // Check MDAC
        //            bool hasMDAC = false;
        //            try
        //            {
        //                Type oleDbType = Type.GetTypeFromProgID("ADODB.Connection");
        //                hasMDAC = (oleDbType != null);
        //                info.AppendLine($"MDAC Components: {(hasMDAC ? "Available" : "Not Available")}")
        //;
        //            }
        //            catch (Exception ex) 
        //            { 
        //                info.AppendLine($"MDAC Check Error: {ex.Message}"); 
        //            }

        //            // Check SQL Server drivers
        //            bool hasSqlDrivers = false;
        //            try
        //            {
        //                // Direct check: Can we create a SqlConnection?
        //                using (var sqlConnection = new SqlConnection())
        //                {
        //                    // If we get here, the SQL driver is installed
        //                    hasSqlDrivers = true;
        //                    info.AppendLine("SQL Client direct test: Driver is available");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                info.AppendLine($"SQL Client direct test failed: {ex.Message}");
        //            }

        //            // Keep the existing DbProviderFactories check as a fallback
        //            if (!hasSqlDrivers)
        //            {
        //                try
        //                {
        //                    // Traditional check via provider factories
        //                    foreach (DataRow factory in System.Data.Common.DbProviderFactories.GetFactoryClasses().Rows)
        //                    {
        //                        string name = factory["InvariantName"].ToString() ?? "";
        //                        info.AppendLine($"Data Provider: {name}");
        //                        if (name.Contains("SqlClient"))
        //                        {
        //                            hasSqlDrivers = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    info.AppendLine($"Provider Factories check failed: {ex.Message}");
        //                }
        //            }

        //            info.AppendLine($"SQL Server Drivers: {(hasSqlDrivers ? "Available" : "Not Available")}");
        //            // Check if SQL Server instance is accessible
        //            try
        //            {
        //                string[] serverNames = { "(local)", "localhost", ".", Environment.MachineName, "DESKTOP-ILHONN0" };

        //                foreach (string server in serverNames)
        //                {
        //                    try
        //                    {
        //                        using var connection = new System.Data.SqlClient.SqlConnection($"Server={server};Integrated Security=True;Connection Timeout=3");
        //                        connection.Open();
        //                        info.AppendLine($"SQL Server instance '{server}' is accessible");
        //                        connection.Close();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        info.AppendLine($"SQL Server '{server}' connection failed: {ex.Message}");
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                info.AppendLine($"SQL Connection Tests Error: {ex.Message}");
        //            }

        //            return (info.ToString(), hasMDAC, hasSqlDrivers);
        //        }

        //        // Debug wrapper for Init
        //        public (bool success, int errorCode, string details) DebugInit(
        //            int lockSystem, string server, string username, int port, int encoder, int tmEncoder)
        //        {
        //            StringBuilder debug = new StringBuilder();
        //            debug.AppendLine($"Debug Init Call:");
        //            debug.AppendLine($"  lockSystem: {lockSystem} ({GetLockSystemName(lockSystem)})");
        //            debug.AppendLine($"  server: '{server}'");
        //            debug.AppendLine($"  username: '{username}'");
        //            debug.AppendLine($"  port: {port}");
        //            debug.AppendLine($"  encoder: {encoder} ({(encoder == AUTO_ENCODER ? "AUTO_ENCODER" : encoder == MANUAL_ENCODER ? "MANUAL_ENCODER" : "MSR206_MAGNETIC")})");
        //            debug.AppendLine($"  tmEncoder: {tmEncoder}");

        //            // Run diagnostic checks
        //            var (diagnosticInfo, hasMDAC, hasSqlDrivers) = DiagnoseInitRequirements();
        //            debug.AppendLine("\nSystem Diagnostics:");
        //            debug.AppendLine(diagnosticInfo);

        //            try
        //            {
        //                debug.AppendLine("\nCalling Init function...");
        //                int result = Init(lockSystem, server, username, port, encoder, tmEncoder);
        //                debug.AppendLine($"Init result: {result} - {GetErrorDescription(result)}");

        //                return (result == SUCCESS, result, debug.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                debug.AppendLine($"\nException in Init: {ex.Message}");
        //                debug.AppendLine($"Exception Type: {ex.GetType().FullName}");
        //                if (ex.InnerException != null)
        //                {
        //                    debug.AppendLine($"Inner Exception: {ex.InnerException.Message}");
        //                }

        //                return (false, -1, debug.ToString());
        //            }
        //        }

        //        private string GetLockSystemName(int lockSystem) => lockSystem switch
        //        {
        //            LOCK3200 => "LOCK3200",
        //            LOCK3200K => "LOCK3200K",
        //            LOCK4200 => "LOCK4200",
        //            LOCK4200D => "LOCK4200D",
        //            LOCK5200_A50 => "LOCK5200_A50",
        //            LOCK6200 => "LOCK6200",
        //            LOCK7200 => "LOCK7200",
        //            LOCK7200D => "LOCK7200D",
        //            LOCK9200 => "LOCK9200",
        //            LOCK9200T => "LOCK9200T",
        //            A30 => "A30",
        //            A50 => "A50",
        //            A90 => "A90",
        //            A92 => "A92",
        //            _ => $"Unknown({lockSystem})"
        //        };
        //        #endregion
    }
}
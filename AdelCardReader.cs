using System.Runtime.InteropServices;
using System.Text;

namespace AdelCardReaderManager
{
    public class AdelCardReader
    {
        // Lock system constants from documentation
        public const int LOCK3200 = 1;
        public const int LOCK3200K = 2;
        public const int LOCK4200 = 3;
        public const int LOCK4200D = 4;
        public const int LOCK5200_A50 = 5;
        public const int LOCK6200 = 6;
        public const int LOCK7200 = 7;
        public const int LOCK7200D = 8;
        public const int LOCK9200 = 9;
        public const int LOCK9200T = 10;
        public const int A30 = 11;
        public const int A50 = 14;
        public const int A90 = 18;
        public const int A92 = 22;

        // Encoder types
        public const int MANUAL_ENCODER = 0;
        public const int AUTO_ENCODER = 1;
        public const int MSR206_MAGNETIC = 2;

        // Return codes
        public const int SUCCESS = 0;
        public const int READ_WRITE_ERROR = 1;
        public const int CARD_DAMAGED = 2;
        public const int NO_CARD = 3;
        public const int SERIAL_PORT_ERROR = 4;
        public const int CARD_REPLACED = 5;
        public const int NO_NEW_CARD = 6;
        public const int NEW_CARD = 7;
        public const int NON_SYSTEM_CARD = 8;
        public const int NOT_GUEST_CARD = 9;

        // P/Invoke declarations for MAINDLL.DLL functions
        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int Init(int software, string server, string username, int port, int encoder, int tmEncoder);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int EndSession();

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern void ChangeUser(string username);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetPort(int software, int port, int encoder, int tmEncoder);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int NewKey(string room, string gate, string stime, string guestname, 
            string guestid, int overflag, ref long cardno, string track1, string track2);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadCard(StringBuilder room, StringBuilder gate, StringBuilder stime, 
            StringBuilder guestname, StringBuilder guestid, StringBuilder track1, StringBuilder track2, 
            ref long cardno, ref int st);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int EraseCard(long cardno, string track1, string track2);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadCardId(ref uint pid);

        [DllImport("MAINDLL.DLL", CallingConvention = CallingConvention.StdCall)]
        public static extern int PopCard();

        // Helper class for card information
        public class CardInfo
        {
            public string Room { get; set; } = "";
            public string Gate { get; set; } = "";
            public string StartTime { get; set; } = "";
            public string EndTime { get; set; } = "";
            public string GuestName { get; set; } = "";
            public string GuestId { get; set; } = "";
            public long CardNumber { get; set; }
            public int Status { get; set; } // 1-normal, 3-logout, 4-lost, 5-destroy, 6-expired
            public string Track1 { get; set; } = "";
            public string Track2 { get; set; } = "";
        }

        private bool _isInitialized = false;

        public bool Initialize(int lockSystem = A90, string server = "", string username = "Admin", 
            int comPort = 1, int encoderType = AUTO_ENCODER, int tmEncoder = 1)
        {
            try
            {
                int result = Init(lockSystem, server, username, comPort, encoderType, tmEncoder);
                _isInitialized = (result == SUCCESS);
                return _isInitialized;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InitializeStandalone(int lockSystem = A90, int comPort = 1, 
            int encoderType = AUTO_ENCODER, int tmEncoder = 1)
        {
            try
            {
                int result = SetPort(lockSystem, comPort, encoderType, tmEncoder);
                _isInitialized = (result == SUCCESS);
                return _isInitialized;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CardInfo? ReadCardInfo()
        {
            if (!_isInitialized) return null;

            try
            {
                var room = new StringBuilder(20);
                var gate = new StringBuilder(20);
                var stime = new StringBuilder(30);
                var guestname = new StringBuilder(50);
                var guestid = new StringBuilder(50);
                var track1 = new StringBuilder(100);
                var track2 = new StringBuilder(100);
                long cardno = 0;
                int status = 0;

                int result = ReadCard(room, gate, stime, guestname, guestid, track1, track2, ref cardno, ref status);

                if (result == SUCCESS)
                {
                    var timeStr = stime.ToString();
                    var startTime = "";
                    var endTime = "";
                    
                    // Parse time format: yyyymmddhhnnyyyy mmddhhnn
                    if (timeStr.Length == 24)
                    {
                        startTime = $"{timeStr.Substring(0, 4)}-{timeStr.Substring(4, 2)}-{timeStr.Substring(6, 2)} {timeStr.Substring(8, 2)}:{timeStr.Substring(10, 2)}";
                        endTime = $"{timeStr.Substring(12, 4)}-{timeStr.Substring(16, 2)}-{timeStr.Substring(18, 2)} {timeStr.Substring(20, 2)}:{timeStr.Substring(22, 2)}";
                    }

                    return new CardInfo
                    {
                        Room = room.ToString(),
                        Gate = gate.ToString(),
                        StartTime = startTime,
                        EndTime = endTime,
                        GuestName = guestname.ToString(),
                        GuestId = guestid.ToString(),
                        CardNumber = cardno,
                        Status = status,
                        Track1 = track1.ToString(),
                        Track2 = track2.ToString()
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreateGuestCard(string roomNumber, string guestName, string guestId, 
            DateTime startTime, DateTime endTime, string publicChannels = "00", bool overwrite = true)
        {
            if (!_isInitialized) return false;

            try
            {
                // Format time as required: yyyymmddhhnnyyyy mmddhhnn
                string timeFormat = $"{startTime:yyyyMMddHHmm}{endTime:yyyyMMddHHmm}";
                
                long cardno = 0;
                int result = NewKey(roomNumber, publicChannels, timeFormat, guestName, guestId, 
                    overwrite ? 1 : 0, ref cardno, null!, null!);

                return result == SUCCESS;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EraseCard(long cardNumber = 0)
        {
            if (!_isInitialized) return false;

            try
            {
                int result = EraseCard(cardNumber, null!, null!);
                return result == SUCCESS;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public uint? ReadRFCardId()
        {
            if (!_isInitialized) return null;

            try
            {
                uint cardId = 0;
                int result = ReadCardId(ref cardId);
                return result == SUCCESS ? cardId : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool EjectCard()
        {
            try
            {
                int result = PopCard();
                return result == SUCCESS;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Shutdown()
        {
            if (_isInitialized)
            {
                EndSession();
                _isInitialized = false;
            }
        }

        public static string GetErrorDescription(int errorCode)
        {
            return errorCode switch
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
                _ => $"Unknown error code: {errorCode}"
            };
        }
    }
}
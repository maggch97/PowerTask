namespace VtConnect.Telnet
{
    public enum ETelnetCode
    {
        BinaryTransmission = 0,     // RFC856
        Echo = 1,                   // RFC857
        SuppressGoAhead = 3,        // RFC858
        Status = 5,                 // RFC859
        TimingMark = 6,             // RFC860
        ExtendedOptionsList = 255,  // RFC861 (not supported)
        TelnetEndOfRecord = 255,    // RFC885 (conflict with Extended options?)
        NegotiateAboutWindowSize = 31, // RFC1073
        TerminalSpeed = 32,         // RFC1079
        TerminalType = 24,          // RFC1091
        XDisplayLocation = 35,      // RFC1096
        TransmitBinary = 0,         // RFC856
        LineMode = 34,              // RFC1184 (Review to see if this is important.. someday)
        RemoteFlowControl = 33,     // RFC1372
        TerminalEnvironmentOption = 39, // RFC1572
        TelnetAuthentication = 37,  // RFC1416, RFC 2942, RFC2943, RFC2944, 
        Encrypt = 38,               // RFC2946


        Send = 0,                   // SEND                0      Common subnegotiation value
        Is = 1,                     // IS                  1      Common subnegotiation value

        Off = 0,                    // Off                 0      Telnet Remote Flow Control Option
        On = 1,                     // On                  1      Telnet Remote Flow Control Option
        RestartAny = 2,             // Restart any         2      Telnet Remote Flow Control Option
        RestartXon = 3,             // Restart-xon         3      Telnet Remote Flow Control Option
    }
}

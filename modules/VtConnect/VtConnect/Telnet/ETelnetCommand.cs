namespace VtConnect.Telnet
{
    /// <summary>
    /// Telnet commands as per RFC854
    /// </summary>
    /// <remarks>
    /// In order to make this easier to implement, I've copied from the RFC
    /// excerpts verbatim. I can't find a copyright statement in the document
    /// to properly reproduce as is suggested in the IETF Trusts faq. If someone
    /// finds the "fair use" of this reproduction "not fair", please file
    /// an issue on the active git of this project and it will be resolved.
    /// </remarks>
    public enum ETelnetCommand
    {
        /// <summary>
        /// End of subnegotiation parameters.
        /// </summary>
        SE = 240,

        /// <summary>
        /// No operation.
        /// </summary>
        NOP = 241,

        /// <summary>
        /// The data stream portion of a Synch.
        /// </summary>
        /// <remarks>
        /// This should always be accompanied by a TCP Urgent notification.
        /// </remarks>
        DataMark = 241,

        /// <summary>
        /// NVT character BRK. (Network Virtual Terminal)
        /// </summary>
        Break = 243,

        /// <summary>
        /// The function IP.
        /// </summary>
        /// <remarks>
        /// Many systems provide a function which suspends, interrupts,
        /// aborts, or terminates the operation of a user process.  This
        /// function is frequently used when a user believes his process is
        /// in an unending loop, or when an unwanted process has been
        /// inadvertently activated.  IP is the standard representation for
        /// invoking this function.  It should be noted by implementers
        /// that IP may be required by other protocols which use TELNET,
        /// and therefore should be implemented if these other protocols
        /// are to be supported.
        /// </remarks>
        InterruptProcess = 244,

        /// <summary>
        /// The function AO.
        /// </summary>
        /// <remarks>
        /// Many systems provide a function which allows a process, which
        /// is generating output, to run to completion (or to reach the
        /// same stopping point it would reach if running to completion)
        /// but without sending the output to the user's terminal.
        /// Further, this function typically clears any output already
        /// produced but not yet actually printed (or displayed) on the
        /// user's terminal.  AO is the standard representation for
        /// invoking this function.  For example, some subsystem might
        /// normally accept a user's command, send a long text string to
        /// the user's terminal in response, and finally signal readiness
        /// to accept the next command by sending a "prompt" character
        /// (preceded by 'CR''LF') to the user's terminal.  If the AO were
        /// received during the transmission of the text string, a
        /// reasonable implementation would be to suppress the remainder of
        /// the text string, but transmit the prompt character and the
        /// preceding 'CR''LF'.  (This is possibly in distinction to the
        /// action which might be taken if an IP were received; the IP
        /// might cause suppression of the text string and an exit from the
        /// subsystem.)
        /// 
        /// It should be noted, by server systems which provide this
        /// function, that there may be buffers external to the system (in
        /// the network and the user's local host) which should be cleared;
        /// the appropriate way to do this is to transmit the "Synch"
        /// signal (described below) to the user system.
        /// </remarks>
        AbortOutput = 254,

        /// <summary>
        /// The function AYT.
        /// </summary>
        /// <remarks>
        /// Many systems provide a function which provides the user with
        /// some visible (e.g., printable) evidence that the system is
        /// still up and running.  This function may be invoked by the user
        /// when the system is unexpectedly "silent" for a long time,
        /// because of the unanticipated (by the user) length of a
        /// computation, an unusually heavy system load, etc.  AYT is the
        /// standard representation for invoking this function.
        /// </remarks>
        AreYouThere = 246,

        /// <summary>
        /// The function EC.
        /// </summary>
        /// <remarks>
        /// Many systems provide a function which deletes the last
        /// preceding undeleted character or "print position"* from the
        /// stream of data being supplied by the user.  This function is
        /// typically used to edit keyboard input when typing mistakes are
        /// made.  EC is the standard representation for invoking this
        /// function.
        /// 
        ///    *NOTE:  A "print position" may contain several characters
        ///    which are the result of overstrikes, or of sequences such as
        ///    'char1' BS 'char2'...
        /// </remarks>
        EraseCharacter = 247,

        /// <summary>
        /// The function EL.
        /// </summary>
        /// <remarks>
        /// Many systems provide a function which deletes all the data in
        /// the current "line" of input.  This function is typically used
        /// to edit keyboard input.  EL is the standard representation for
        /// invoking this function.
        /// </remarks>
        EraseLine = 248,

        /// <summary>
        /// The GA signal.
        /// </summary>
        GoAhead = 249,

        /// <summary>
        /// Indicates that what follows is subnegotiation of the indicated option.
        /// </summary>
        SB = 250,

        /// <summary>
        /// Indicates the desire to begin performing, or confirmation that you are now performing,
        /// the indicated option.
        /// </summary>
        Will = 251,

        /// <summary>
        /// Indicates the refusal to perform, or continue performing, the indicated option.
        /// </summary>
        Wont = 252,

        /// <summary>
        /// Indicates the request that the other party perform, or confirmation that you
        /// are expecting indicated option.
        /// </summary>
        Do = 253,

        /// <summary>
        /// Indicates the demand that the other party stop performing, or confirmation that
        /// you are no longer expecting the other party to perform, the indicated option.
        /// </summary>
        Dont = 254,

        /// <summary>
        /// Data Byte 255.
        /// </summary>
        IAC = 255,
    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VtNetCore.XTermParser;
using static MiniTerm.Native.ConsoleApi;

namespace MiniTerm
{
    /// <summary>
    /// The UI of the terminal. It's just a normal console window, but we're managing the input/output.
    /// In a "real" project this could be some other UI.
    /// </summary>
    public sealed class Terminal
    {
        private readonly DataConsumer terminalStream;
        private const string ExitCommand = "exit\r";
        private const string CtrlC_Command = "\x3";
        public bool status = false;
        public Terminal(DataConsumer terminalStream)
        {
            EnableVirtualTerminalSequenceProcessing();
            this.terminalStream = terminalStream;
        }

        /// <summary>
        /// Newer versions of the windows console support interpreting virtual terminal sequences, we just have to opt-in
        /// </summary>
        private static void EnableVirtualTerminalSequenceProcessing()
        {
            var hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            // TODO
            //if (!GetConsoleMode(hStdOut, out uint outConsoleMode))
            //{
            //    throw new InvalidOperationException("Could not get console mode");
            //}

            //outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            //if (!SetConsoleMode(hStdOut, outConsoleMode))
            //{
            //    throw new InvalidOperationException("Could not enable virtual terminal processing");
            //}
        }
        PseudoConsolePipe inputPipe = new PseudoConsolePipe();
        PseudoConsolePipe outputPipe = new PseudoConsolePipe();
        PseudoConsole pseudoConsole;
        Process process;
        public StreamWriter writer;

        /// <summary>
        /// Start the pseudoconsole and run the process as shown in 
        /// https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#creating-the-pseudoconsole
        /// </summary>
        /// <param name="command">the command to run, e.g. cmd.exe</param>
        public void Run(string command,int width, int height)
        {
            pseudoConsole = PseudoConsole.Create(inputPipe.ReadSide, outputPipe.WriteSide, (short)width, (short)height);
            process = ProcessFactory.Start(command, PseudoConsole.PseudoConsoleThreadAttribute, pseudoConsole.Handle);
            writer = new StreamWriter(new FileStream(inputPipe.WriteSide, FileAccess.Write));
            // copy all pseudoconsole output to stdout
            Task.Run(async () =>
            {
                await CopyPipeToOutput(outputPipe.ReadSide);
            });
            OnClose(() => DisposeResources(process, pseudoConsole, outputPipe, inputPipe));
            status = true;

        }
        public void Resize(int width, int height)
        {
            if(pseudoConsole != null)
            {
                pseudoConsole.Resize(width, height);
            }
        }

        public void Input(byte[] data)
        {
            writer.BaseStream.Write( data,0,data.Length);
            writer.Flush();
        }

        /// <summary>
        /// Don't let ctrl-c kill the terminal, it should be sent to the process in the terminal.
        /// </summary>
        private static void ForwardCtrlC(StreamWriter writer)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                writer.Write(CtrlC_Command);
            };
        }

        /// <summary>
        /// Reads PseudoConsole output and copies it to the terminal's standard out.
        /// </summary>
        /// <param name="outputReadSide">the "read" side of the pseudo console output pipe</param>
        async Task CopyPipeToOutput(SafeFileHandle outputReadSide)
        {
            using (var pseudoConsoleOutput = new FileStream(outputReadSide, FileAccess.Read))
            {
                terminalStream.Push(new byte[1], pseudoConsoleOutput);
            }
        }

        /// <summary>
        /// Get an AutoResetEvent that signals when the process exits
        /// </summary>
        private static AutoResetEvent WaitForExit(Process process) =>
            new AutoResetEvent(false)
            {
                SafeWaitHandle = new SafeWaitHandle(process.ProcessInfo.hProcess, ownsHandle: false)
            };

        /// <summary>
        /// Set a callback for when the terminal is closed (e.g. via the "X" window decoration button).
        /// Intended for resource cleanup logic.
        /// </summary>
        private static void OnClose(Action handler)
        {
            SetConsoleCtrlHandler(eventType =>
            {
                if(eventType == CtrlTypes.CTRL_CLOSE_EVENT)
                {
                    handler();
                }
                return false;
            }, true);
        }

        private void DisposeResources(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}

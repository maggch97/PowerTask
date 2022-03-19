using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using VtNetCore.VirtualTerminal;
using VtNetCore.XTermParser;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PowerTask
{
    public sealed class CommandItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string Command { get; set; }
        private string result;
        public string Result
        {
            get { 
                return result; 
            }
            set
            {
                result = value;
                NotifyPropertyChanged("Result");
            }
        }
        public CommandItem(string command, String result) { Command = command; Result = result; }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        private readonly DataConsumer terminalStream;
        private readonly VirtualTerminalController terminalController = new VirtualTerminalController();
        MiniTerm.Terminal miniTerm;

        public async Task<string> GetIpAddressTask(string command)
        {
            PowerShell _proc = PowerShell.Create();
            _proc.AddCommand(command);
            var result = await _proc.InvokeAsync();
            var str = "";
            foreach(var item in result)
            {
                str += item.ToString().Trim();
            }
            return str;
        }

        ObservableCollection<CommandItem> items = new ObservableCollection<CommandItem>() {new CommandItem("1","2")};
        public BlankPage1()
        {
            this.InitializeComponent();

            terminal.Terminal = terminalController;
            terminalController.SendData += SendDataEvent;
            terminalController.WindowTitleChanged += OnWindowTitleChanged;


            terminalStream = new DataConsumer(terminal.Terminal);
            miniTerm = new MiniTerm.Terminal(terminalStream);
            terminalController.SizeChanged += TerminalSizeChanged;
        }
        
        private void OnWindowTitleChanged(object sender, TextEventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
            });
        }


        int width = 0;
        int height = 0;

        private void TerminalSizeChanged(object sender, SizeEventArgs e)
        {
            width = e.Width;
            height = e.Height;
            if (!miniTerm.status)
            {
                miniTerm.Run("powershell.exe", width, height);
            }
            // var info = new WindowChangeRequestInfo((uint)e.Width, (uint)e.Height, 800, 600);
            // miniTerm.Input(info.GetBytes());
            miniTerm.Resize(e.Width, e.Height);
        }

        private void SendDataEvent(object sender, SendDataEventArgs e)
        {
            try
            {
                if (e.Data[0] == 10)
                {
                    e.Data = new byte[1] { 13 };
                }
                miniTerm.Input(e.Data);
                // connection.SendData(e.Data);
            }
            catch
            {

            }
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            var ctx = (sender as Button).DataContext;
            int index = CommandList.Items.IndexOf(ctx);
            var x = CommandList.Items.ElementAt(index);
            miniTerm.Input(Encoding.Default.GetBytes(items.ElementAt(index).Command+"\r"));
            // var ip = await GetIpAddressTask(items.ElementAt(index).Command);
            // items[index].Result = ip;
        }

        private void CommandList_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            int x = 0;
        }

        private void CommandList_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int x = 0;

        }

        public void Grid_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            terminal.CoreWindow_CharacterReceived(sender, args);
        }

        private void AddCommandButton_Click(object sender, RoutedEventArgs e)
        {
            items.Add(new CommandItem("pwd", ""));
        }

        private void RunAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(var item in items)
            {
                miniTerm.Input(Encoding.Default.GetBytes(item.Command + "\r"));
            }
        }
    }
}

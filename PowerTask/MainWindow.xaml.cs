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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PowerTask
{
    public class TaskItem
    {
        public string Name { get; set; }
        public TaskItem(string name) { Name = name; }
    }

    public class TaskNavigationViewModel
    {
        public string Content { get; set; }
        public string Tag { get; set; }
        public TaskNavigationViewModel(string content) { Content = content; }
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<TaskNavigationViewModel> NavigationItems = new ObservableCollection<TaskNavigationViewModel> { new TaskNavigationViewModel("Task 1"), new TaskNavigationViewModel("Task 2") };

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "PowerTask";
        }

        private void Add_Tapped(object sender, TappedRoutedEventArgs e)
        {
             
            NavigationItems.Add(new TaskNavigationViewModel("New Task"));
            nvSample.UpdateLayout();

            nvSample.SelectedItem = NavigationItems.Last();
            nvSample.UpdateLayout();
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer;
            contentFrame.Navigate(typeof(BlankPage1)); 
            switch (item.Tag)
            {
            }
        }

        private void Grid_CharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
        {
            (contentFrame.Content as BlankPage1).Grid_CharacterReceived(sender, args);
        }
    }
}

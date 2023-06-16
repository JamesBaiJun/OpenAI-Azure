using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;
using Wpf.Ui.Mvvm.Services;

namespace GPTChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            GPTRequest.OnReceiveReply += GPTRequest_OnReceiveReply;
            GPTRequest.OnReceivePartial += GPTRequest_OnReceivePartial;
        }

        private async void GPTRequest_OnReceivePartial(string obj)
        {
            if (scrollSet.IsChecked == false)
            {
                return;
            }
            await Task.Delay(50);
            scroll.ScrollToBottom();
        }

        private async void GPTRequest_OnReceiveReply(string obj)
        {
            if (scrollSet.IsChecked == false)
            {
                return;
            }
            await Task.Delay(200);
            scroll.ScrollToBottom();
        }

        private void themeComb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(themeComb.Text))
            {
                return;
            }

            var theme = Enum.Parse<ThemeType>(((ContentControl)((object[])e.AddedItems)[0]).Content.ToString());
            Theme.Apply(theme, BackgroundType.None, false);
        }
    }
}

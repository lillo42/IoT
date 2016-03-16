using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ClientCrypto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SocketClient _client;

        public MainPage()
        {
            this.InitializeComponent();

            _client = new SocketClient();
            _client.OnDataReceive += Socket_OnDataRecive;
            _client.OnError += Socket_OnError;
        }

        private void Socket_OnError(SocketClient sender, string args)
        {
            NotifyUser(args);
        }

        private void Socket_OnDataRecive(SocketClient sender, string args)
        {
            txbMessageRecived.Text = args;
        }

        private void NotifyUser(string strMessage)
        {
            //StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            //StatusBlock.Text = strMessage;

            //// Collapse the StatusBlock if it has no text to conserve real estate.
            //StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            //StatusBorder.Visibility = Visibility.Visible;
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnConnect.Content.ToString().StartsWith("C"))
                {
                    await _client.ConnectAsync(txtIp.Text, Convert.ToInt32(txtPort.Text));
                    btnConnect.Content = "Disconnect";
                }
                else
                {
                    btnConnect.Content = "Connect";
                    await _client.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                NotifyUser(ex.Message);
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            await _client.SendAsync(txtMessage.Text);
        }
    }
}

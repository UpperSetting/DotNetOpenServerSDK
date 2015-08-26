/*
Copyright 2015 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using US.OpenServer.Protocols.Hello;
using US.OpenServer.Protocols.KeepAlive;
using US.OpenServer.Protocols.WinAuth;
using US.OpenServer.WindowsMobile;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HelloMobileClient
{
    public sealed partial class MainPage : Page
    {
        private const string CONNECT = "Connect";
        private const string DISCONNECT = "Disconnect";

        private Client client;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string)btnConnect.Content == DISCONNECT)
                {
                    if (client != null)
                        client.Close();

                    btnConnect.Content = CONNECT;
                }
                else
                {
                    Connect();
                    btnConnect.Content = DISCONNECT;
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void Connect()
        {
            ServerConfiguration cfg = new ServerConfiguration();
            cfg.Host = txtHost.Text;
            //cfg.TlsConfiguration.Enabled = true;

            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                new Dictionary<ushort, ProtocolConfiguration>();

            protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

            protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, typeof(WinAuthProtocolClient)));

            protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolClient)));

            client = new Client(cfg, protocolConfigurations);
            client.OnConnectionLost += client_OnConnectionLost;
            client.Connect();

            try
            {
                WinAuthProtocolClient wap = (WinAuthProtocolClient)client.Initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
                if (!wap.Authenticate(txtUserName.Text, txtPassword.Password))
                    throw new Exception("Access denied.");

                client.Initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);

                HelloProtocolClient hpc = (HelloProtocolClient)client.Initialize(HelloProtocol.PROTOCOL_IDENTIFIER);
                ShowMessageBox(hpc.Hello(txtUserName.Text));
            }
            catch (Exception)
            {
                client.Close();
                throw;
            }            
        }

        private async void client_OnConnectionLost(object sender, Exception ex)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnConnect.Content = CONNECT;
                ShowMessageBox(string.Format("Connection lost. {0}", ex.Message));
            });
        }

        private async void hpc_OnHelloComplete(string serverResponse)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ShowMessageBox(serverResponse);
            });
        }

        private async void ShowMessageBox(string message)
        {
            MessageDialog dlg = new MessageDialog(message);
            dlg.DefaultCommandIndex = 1;
            await dlg.ShowAsync();
        }
    }
}

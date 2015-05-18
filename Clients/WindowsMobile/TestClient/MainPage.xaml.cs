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
using US.OpenServer;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using US.OpenServer.Protocols.Hello;
using US.OpenServer.Protocols.KeepAlive;
using US.OpenServer.Protocols.WinAuth;
using US.OpenServer.WindowsMobile;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace TestClient
{
    public sealed partial class MainPage : Page
    {
        #region Variables
        private Client client;
        private ILogger logger;
        private bool grayback;
        #endregion

        #region Constructor
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            
            logger = new ILogger();
            logger.LogDebug = true;
            logger.LogPackets = true;
            logger.OnLogMessage += logger_OnLogMessage;
        }
        #endregion

        #region Connect/Disconnect
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((string)btnConnect.Content == "Disconnect")
                {
                    if (client != null)
                        client.Close();

                    btnConnect.Content = "Connect";
                }
                else
                    Connect();
            }
            catch (Exception ex)
            {
                logger.Log(ex);
            }
        }

        private void Connect()
        {
            ServerConfiguration cfg = new ServerConfiguration();
            cfg.Host = txtRemoteHost.Text;
            cfg.TlsConfiguration.Enabled = false;
            cfg.TlsConfiguration.AllowCertificateChainErrors = true;
            cfg.TlsConfiguration.AllowSelfSignedCertificate = true;
            cfg.TlsConfiguration.CheckCertificateRevocation = false;
            cfg.TlsConfiguration.RequireRemoteCertificate = true;

            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                    new Dictionary<ushort, ProtocolConfiguration>();

            protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

            protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, typeof(WinAuthProtocolClient)));

            protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolClient)));
            
            client = new Client(cfg, protocolConfigurations, logger);

            try
            {
                client.OnConnectionLostEvent += client_OnConnectionLostEvent;
                client.Connect();

                WinAuthProtocolClient wap = (WinAuthProtocolClient)client.Initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
                if (!wap.Authenticate(txtUserName.Text, txtPassword.Password, null))
                    throw new Exception("Access denied.");

                client.Initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);

                HelloProtocolClient hpc = (HelloProtocolClient)client.Initialize(HelloProtocol.PROTOCOL_IDENTIFIER);
                hpc.OnHelloComplete += hpc_OnHelloComplete;
                hpc.Hello(txtUserName.Text);

                btnConnect.Content = "Disconnect";
            }
            catch (Exception)
            {
                client.Close();
                throw;
            }
        }

        private async void client_OnConnectionLostEvent(object sender, Exception ex)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                btnConnect.Content = "Connect";
            });
        }

        private void hpc_OnHelloComplete(string serverResponse)
        {
            logger_OnLogMessage(Level.Info, serverResponse);            
        }
        #endregion

        #region Logger
        private async void logger_OnLogMessage(Level level, string message)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ListViewItem itm = new ListViewItem();
                itm.Content = message;
                itm.Height = 20;
                if (grayback)
                    itm.Background = new SolidColorBrush(Colors.Gray);
                else
                    itm.Background = new SolidColorBrush(Colors.DarkGray);

                grayback = !grayback;

                if (lvwMessages.Items.Count == 16)
                    lvwMessages.Items.RemoveAt(0);

                lvwMessages.Items.Add(itm);
            });
        }
        #endregion
    }
}

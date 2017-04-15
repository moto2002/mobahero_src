using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketTcp : IPhotonSocket, IDisposable
	{
		private Socket sock;

		private readonly object syncer = new object();

		public SocketTcp(PeerBase npeer) : base(npeer)
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.ALL);
			if (flag)
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "SocketTcp: TCP, DotNet, Unity.");
			}
			base.Protocol = ConnectionProtocol.Tcp;
			this.PollReceive = false;
		}

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			bool flag = this.sock != null;
			if (flag)
			{
				try
				{
					bool connected = this.sock.Connected;
					if (connected)
					{
						this.sock.Close();
					}
				}
				catch (Exception arg)
				{
					base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + arg);
				}
			}
			this.sock = null;
			base.State = PhotonSocketState.Disconnected;
		}

		public override bool Connect()
		{
			bool flag = base.Connect();
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				base.State = PhotonSocketState.Connecting;
				new Thread(new ThreadStart(this.DnsAndConnect))
				{
					Name = "photon dns thread",
					IsBackground = true
				}.Start();
				result = true;
			}
			return result;
		}

		public override bool Disconnect()
		{
			bool flag = base.ReportDebugOfLevel(DebugLevel.INFO);
			if (flag)
			{
				base.EnqueueDebugReturn(DebugLevel.INFO, "SocketTcp.Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			object obj = this.syncer;
			lock (obj)
			{
				bool flag2 = this.sock != null;
				if (flag2)
				{
					try
					{
						this.sock.Close();
					}
					catch (Exception arg)
					{
						base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + arg);
					}
					this.sock = null;
				}
			}
			base.State = PhotonSocketState.Disconnected;
			return true;
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			bool flag = !this.sock.Connected;
			PhotonSocketError result;
			if (flag)
			{
				result = PhotonSocketError.Skipped;
			}
			else
			{
				try
				{
					this.sock.Send(data);
				}
				catch (Exception ex)
				{
					bool flag2 = base.ReportDebugOfLevel(DebugLevel.ERROR);
					if (flag2)
					{
						base.EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
					}
					base.HandleException(StatusCode.Exception);
					result = PhotonSocketError.Exception;
					return result;
				}
				result = PhotonSocketError.Success;
			}
			return result;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		public void DnsAndConnect()
		{
			try
			{
				IPAddress ipAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
				bool flag = ipAddress == null;
				if (flag)
				{
					throw new ArgumentException("Invalid IPAddress. Address: " + base.ServerAddress);
				}
				this.sock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.sock.NoDelay = true;
				this.sock.ReceiveTimeout = this.peerBase.DisconnectTimeout;
				this.sock.SendTimeout = this.peerBase.DisconnectTimeout;
				this.sock.Connect(ipAddress, base.ServerPort);
				base.AddressResolvedAsIpv6 = base.IsIpv6SimpleCheck(ipAddress);
				base.State = PhotonSocketState.Connected;
				this.peerBase.OnConnect();
			}
			catch (SecurityException ex)
			{
				bool flag2 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag2)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex.ToString());
				}
				base.HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				bool flag3 = base.ReportDebugOfLevel(DebugLevel.ERROR);
				if (flag3)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex2.ToString());
				}
				base.HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			new Thread(new ThreadStart(this.ReceiveLoop))
			{
				Name = "photon receive thread",
				IsBackground = true
			}.Start();
		}

		public void ReceiveLoop()
		{
			StreamBuffer streamBuffer = new StreamBuffer(base.MTU);
			while (base.State == PhotonSocketState.Connected)
			{
				streamBuffer.Position = 0L;
				streamBuffer.SetLength(0L);
				try
				{
					int i = 0;
					byte[] array = new byte[9];
					while (i < 9)
					{
						int num = this.sock.Receive(array, i, 9 - i, SocketFlags.None);
						i += num;
						bool flag = num == 0;
						if (flag)
						{
							throw new SocketException(10054);
						}
					}
					bool flag2 = array[0] == 240;
					if (flag2)
					{
						base.HandleReceivedDatagram(array, array.Length, false);
					}
					else
					{
						int num2 = (int)array[1] << 24 | (int)array[2] << 16 | (int)array[3] << 8 | (int)array[4];
						bool trafficStatsEnabled = this.peerBase.TrafficStatsEnabled;
						if (trafficStatsEnabled)
						{
							bool flag3 = array[5] == 0;
							bool flag4 = flag3;
							if (flag4)
							{
								this.peerBase.TrafficStatsIncoming.CountReliableOpCommand(num2);
							}
							else
							{
								this.peerBase.TrafficStatsIncoming.CountUnreliableOpCommand(num2);
							}
						}
						bool flag5 = base.ReportDebugOfLevel(DebugLevel.ALL);
						if (flag5)
						{
							base.EnqueueDebugReturn(DebugLevel.ALL, "message length: " + num2);
						}
						streamBuffer.Write(array, 7, i - 7);
						i = 0;
						num2 -= 9;
						array = new byte[num2];
						while (i < num2)
						{
							int num = this.sock.Receive(array, i, num2 - i, SocketFlags.None);
							i += num;
							bool flag6 = num == 0;
							if (flag6)
							{
								throw new SocketException(10054);
							}
						}
						streamBuffer.Write(array, 0, i);
						bool flag7 = streamBuffer.Length > 0L;
						if (flag7)
						{
							base.HandleReceivedDatagram(streamBuffer.ToArray(), (int)streamBuffer.Length, false);
						}
						bool flag8 = base.ReportDebugOfLevel(DebugLevel.ALL);
						if (flag8)
						{
							base.EnqueueDebugReturn(DebugLevel.ALL, "TCP < " + streamBuffer.Length + ((streamBuffer.Length == (long)(num2 + 2)) ? " OK" : " BAD"));
						}
					}
				}
				catch (SocketException ex)
				{
					bool flag9 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag9)
					{
						bool flag10 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag10)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, "Receiving failed. SocketException: " + ex.SocketErrorCode);
						}
						bool flag11 = ex.SocketErrorCode == SocketError.ConnectionReset || ex.SocketErrorCode == SocketError.ConnectionAborted;
						if (flag11)
						{
							base.HandleException(StatusCode.DisconnectByServer);
						}
						else
						{
							base.HandleException(StatusCode.ExceptionOnReceive);
						}
					}
				}
				catch (Exception ex2)
				{
					bool flag12 = base.State != PhotonSocketState.Disconnecting && base.State > PhotonSocketState.Disconnected;
					if (flag12)
					{
						bool flag13 = base.ReportDebugOfLevel(DebugLevel.ERROR);
						if (flag13)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"Receive issue. State: ",
								base.State,
								". Server: '",
								base.ServerAddress,
								"' Exception: ",
								ex2
							}));
						}
						base.HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			this.Disconnect();
		}
	}
}

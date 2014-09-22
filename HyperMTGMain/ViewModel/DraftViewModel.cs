﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Input;
using HyperKore.Common;
using HyperKore.Utilities;
using HyperMTGMain.DraftSR;
using HyperMTGMain.Helper;
using HyperMTGMain.Model;
using HyperMTGMain.View;
using HyperServer.Common;
using HyperServer.Draft;
using IDraftCallback = HyperMTGMain.DraftSR.IDraftCallback;

namespace HyperMTGMain.ViewModel
{
	public class DraftViewModel : ObservableClass, IDraftCallback
	{
		private static DraftViewModel _instance;
		private List<ImgCard> _cards;
		private int _maxPlayers;
		private string _messageInput;
		private string _messages;
		private List<DraftPlayer> _players;
		private DraftClient _proxy;
		private DraftService _service;
		private string _setList;
		private List<Set> _setSource;
		private int _timeLimit;

		private DraftViewModel()
		{
			Sets = new Set[3];
			ZoomSize = new CardSize {Ratio = 0.62};
			MaxPlayers = 2;
		}

		internal static DraftViewModel Instance
		{
			get { return _instance ?? (_instance = new DraftViewModel()); }
		}

		public string IP { get; set; }
		public string Name { get; set; }
		public Set[] Sets { get; set; }
		public CardSize ZoomSize { get; set; }
		public DraftPlayer Player { get; set; }

		public ICommand ConnectCommand
		{
			get { return new RelayCommand(Connect, CanConnect); }
		}

		public ICommand HostCommand
		{
			get { return new RelayCommand(Host, CanHost); }
		}

		public ICommand SendMessageCommand
		{
			get { return new RelayCommand(SendMessage); }
		}

		#region Notifiable Prop

		public int MaxPlayers
		{
			get { return _maxPlayers; }
			set
			{
				_maxPlayers = value;
				OnPropertyChanged("MaxPlayers");
			}
		}

		public string SetList
		{
			get { return _setList; }
			set
			{
				_setList = value;
				OnPropertyChanged("SetList");
			}
		}

		public List<Set> SetSource
		{
			get { return _setSource; }
			set
			{
				_setSource = value;
				OnPropertyChanged("SetSource");
			}
		}

		public int TimeLimit
		{
			get { return _timeLimit; }
			set
			{
				_timeLimit = value;
				OnPropertyChanged("TimeLimit");
			}
		}

		public string Messages
		{
			get { return string.Format("<Section>{0}</Section>", _messages); }
			set
			{
				_messages = value;
				OnPropertyChanged("Messages");
			}
		}

		public List<DraftPlayer> Players
		{
			get { return _players; }
			set
			{
				_players = value;
				OnPropertyChanged("Players");
			}
		}

		public List<ImgCard> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				OnPropertyChanged("Cards");
			}
		}

		public string MessageInput
		{
			get { return _messageInput; }
			set
			{
				_messageInput = value;
				OnPropertyChanged("MessageInput");
			}
		}

		#endregion

		private void Connect()
		{
			var context = new InstanceContext(this);
			_proxy = new DraftClient(context);
			if (Player == null || !Player.IsHost)
			{
				string servicePath = _proxy.Endpoint.ListenUri.AbsolutePath;
				int serviceListenPort = _proxy.Endpoint.Address.Uri.Port;
				_proxy.Endpoint.Address =
					new EndpointAddress(string.Format("net.tcp://{0}:{1}{2}", IP, serviceListenPort, servicePath));
				Player = new DraftPlayer(Name);
			}
			try
			{
				_proxy.Open();
				_proxy.ConnectAsync((Player));
			}
			catch (EndpointNotFoundException)
			{
				ViewModelManager.MessageViewModel.Message("Connection Timeout");
			}
		}

		private bool CanConnect()
		{
			return !string.IsNullOrWhiteSpace(IP) && IP.IsLegalIPAddress() && !string.IsNullOrWhiteSpace(Name);
		}

		private void Host()
		{
			if (_service != null)
			{
				_service.StopService();
			}
			_service = new DraftService();
			_service.StartService();
			Player = new DraftPlayer(Name) {IsHost = true};
			Connect();
		}

		private bool CanHost()
		{
			return !string.IsNullOrWhiteSpace(Name) && Sets.All(s => s != null);
		}

		private void SendMessage()
		{
			if (string.IsNullOrWhiteSpace(MessageInput))
			{
				return;
			}

			var msg = new Message
			{
				Sender = Player.ID,
				Content = MessageInput,
				Time = DateTime.Now
			};

			_proxy.SendMessageAsync(msg);
			MessageInput = string.Empty;
		}

		public void CloseConnection()
		{
			if (_proxy != null)
			{
				_proxy.Disconnect(Player);
			}
			if (_service != null)
			{
				_service.StopService();
			}
		}

		public void LoadSets()
		{
			SetSource = DataManager.Sets.ToList();
		}

		private void DispMessage(string name, DateTime time, string content)
		{
			Messages = _messages +
			           BBCodeHelper.ToXaml(string.Format("[b]{0}[/b]([i]{1}[/i]) : {2}", name, time.ToString("hh:mm:ss"), content));
		}

		#region Implementation of IDraftCallback

		public void OnConnect(ConnectionResult result)
		{
			if (result == ConnectionResult.Success)
			{
				ViewManager.DraftWindow.Show();
			}
			else
			{
				ViewModelManager.MessageViewModel.Message(result.ToString());
			}
		}

		public IAsyncResult BeginOnConnect(ConnectionResult result, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnConnect(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void RefreshGame(int maxPlayers, int timeLimit, List<string> setCodes)
		{
			MaxPlayers = maxPlayers;
			TimeLimit = timeLimit;
			SetList = setCodes.Aggregate((a, b) => string.Format("{0}+{1}", a, b));
			//Cards = new List<ImgCard>();
			Cards = DataManager.Cards.Take(20).Select(s => new ImgCard(s)).ToList();
		}

		public IAsyncResult BeginRefreshGame(int maxPlayers, int timeLimit, List<string> setCodes, AsyncCallback callback,
			object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndRefreshGame(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnJoin(DraftPlayer client)
		{
			if (client.IsHost)
			{
				_proxy.HostGameAsync(MaxPlayers, TimeLimit, Sets.Select(s => s.SetCode).ToList());
			}
			else
			{
				DispMessage("System", DateTime.Now, string.Format("{0} Entered", client.Name));
			}
		}

		public IAsyncResult BeginOnJoin(DraftPlayer client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnJoin(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnLeave(DraftPlayer client)
		{
			DispMessage("System", DateTime.Now, string.Format("{0} Left", client.Name));
		}

		public IAsyncResult BeginOnLeave(DraftPlayer client, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnLeave(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void RefreshClients(List<DraftPlayer> clients)
		{
			Players = clients;
		}

		public IAsyncResult BeginRefreshClients(List<DraftPlayer> clients, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndRefreshClients(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnStart()
		{
		}

		public IAsyncResult BeginOnStart(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnStart(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnMessage(Message msg)
		{
			Client sender = Players.FirstOrDefault(c => c.ID == msg.Sender);
			if (sender != null)
			{
				DispMessage(sender.Name, msg.Time, msg.Content);
			}
		}

		public IAsyncResult BeginOnMessage(Message msg, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnMessage(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnReady(DraftPlayer player)
		{
			DispMessage("System", DateTime.Now, string.Format("{0} is ready", player.Name));
		}

		public IAsyncResult BeginOnReady(DraftPlayer player, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnReady(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public void OnEnd()
		{
		}

		public IAsyncResult BeginOnEnd(AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public void EndOnEnd(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
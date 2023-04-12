using A;
using Discord;
using Discord.WebSocket;
using log4net;
using System.Net;
using System.Text;
//using XGames105.WebTools.StatusApi.Models;
//using XGames105.WebTools.StatusServerLib;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.conf")]

public static class Program
{
	public static DiscordSocketClient? Client;

	private static IPAddress PiAddress = new IPAddress(new byte[] { 192, 168, 1, 176 });
	private static IPAddress DebugAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });

	//public static TcpStatusServer StatusServer;
	//private static UserSpammer _userSpammer;

	private static SocketApplicationCommand ACommand;
	private static ILog c_log = LogManager.GetLogger(nameof(Program));
	private static CancellationTokenSource _exitCancelationSource = new CancellationTokenSource();
	private static AGenerator _aGenerator;

	private static IEmote[] _angeryEmotes = new IEmote[]
	{
		Emoji.Parse("🤬"),
		Emoji.Parse("😡"),
		Emoji.Parse("😠"),
		Emoji.Parse("❌"),
		Emoji.Parse("🛑"),
	};

	public static CancellationToken ExitCancelationToken => _exitCancelationSource.Token;

	public static async Task Main()
	{
		Console.CancelKeyPress += Console_CancelKeyPress;

		_aGenerator = new AGenerator();

		Console.WriteLine(_aGenerator.GenerateAsciiString(false, false));

		//StatusServer = new TcpStatusServer(GetStatus, new IPEndPoint(DebugAddress, 6968));
		//StatusServer.StartAsync();

		//_userSpammer = new UserSpammer();

		var config = new DiscordSocketConfig()
		{
			GatewayIntents = 
				GatewayIntents.Guilds | 
				GatewayIntents.GuildMessages | 
				GatewayIntents.MessageContent | 
				GatewayIntents.DirectMessages
		};
		Client = new DiscordSocketClient(config);

		Client.MessageReceived += OnMessage;
		Client.SlashCommandExecuted += SlashCommandExecuted;
		Client.Ready += Ready;

		string? token = Environment.GetEnvironmentVariable("TOKEN");

		if (File.Exists("Token.txt"))
		{
			token = File.ReadAllText("Token.txt");
		}

		if (token == null)
		{
			c_log.Fatal("Could not find token");
			Environment.ExitCode = -1;
			return;
		}

		c_log.Info("Starting up A Bot...");
		c_log.Debug("Logging in...");
		await Client.LoginAsync(TokenType.Bot, token);
		c_log.Debug("Done logging in");

		c_log.Debug("Starting discord client...");
		await Client.StartAsync();

		
		while (!ExitCancelationToken.IsCancellationRequested)
		{
			Thread.Sleep(1000);
		}

		await Cleanup();
	}

	private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
	{
		c_log.Info("Shutting down A Bot....");
		_exitCancelationSource.Cancel();
		c_log.Info("Done shutting down A Bot....");

	}


	private static async Task Ready()
	{
		if (Client == null)
		{
			c_log.Fatal("Client is null. Stopping...");
			Environment.ExitCode = -1;
			_exitCancelationSource.Cancel();
			return;
		}

		c_log.Debug("Setting status to Online");
		await Client.SetStatusAsync(UserStatus.Online);
		c_log.Debug("Done setting status to Online");

		c_log.Debug("Creating slash commands");

		var commandProps = new List<SlashCommandProperties>();

		commandProps.Add(new SlashCommandBuilder()
			.WithName("a")
			.WithDescription("A aaaaa a aa aaaaaa")
			.WithDefaultPermission(true)
			.Build());

		var commands = await Client.BulkOverwriteGlobalApplicationCommandsAsync(commandProps.ToArray());
		foreach (var command in commands)
		{
			if (command.Name == "a")
				ACommand = command;
		}

		c_log.Debug("Done creating slash commands");
		c_log.Info("A bot ready");
	}
	private static async Task Cleanup()
	{
		if (Client == null)
		{
			return;
		}

		await Client.SetStatusAsync(UserStatus.Offline);

		await Client.StopAsync();

		await Client.LogoutAsync();
		Client.Dispose();
		Client = null;
		//StatusServer.Stop();
	}



	private static async Task SlashCommandExecuted(SocketSlashCommand arg)
	{
		if (ACommand == null)
			return;

		if(arg.CommandName == ACommand.Name)
		{
			var randomized = Random.Shared.Next(0, 3) == 2;

			var str = _aGenerator.GenerateAsciiString(randomized, false);
			
			try
			{
				await arg.RespondAsync(embed:new EmbedBuilder()
					.WithDescription($"```\n{ str}\n```")
					.Build());
			}catch(Exception e)
			{
				c_log.Error($"Failed to send command response to '{arg.User.Username}#{arg.User.Discriminator}' Id: '{arg.User.Id}' ", e);
			}
			
		}

	}





	private static async Task OnMessage(SocketMessage message)
	{
		if(message.Channel is IPrivateChannel)
		{
			if (!_aGenerator.ValidateString(message.Content))
			{
				await message.AddReactionAsync(_angeryEmotes[Random.Shared.Next(0, _angeryEmotes.Length)]);
				
			}
			return;
		}

		if (!_aGenerator.ValidateString(message.Channel.Name)) //not an A channel
		{
			return;
		}
		

		if (!_aGenerator.ValidateString(message.Content))
		{

			c_log.Debug($"Deleting a message");
			await message.DeleteAsync();

			c_log.Info($"Banning '{message.Author.Username}#{message.Author.Discriminator}' Id: '{message.Author.Id}'");
			var guild = message.Channel as SocketGuildChannel;
			
			if(guild == null)
			{
				c_log.Error("Can not ban user because the channel is not a SocketGuildChannel");
				return;
			}

			SendBannMessage(message.Author);
			await guild.Guild.AddBanAsync(message.Author, reason: "Used a non acceptable character");
			c_log.Debug($"Banned '{message.Author.Username}'");

			
		}


	}


	
	private static async void SendBannMessage(SocketUser socketUser)
	{
		var sb = new StringBuilder();
		for (int i = 0; i < Random.Shared.Next(20, 30); i++)
		{
			sb.AppendLine(_aGenerator.GenerateRandomString(Random.Shared.Next(30, 50)));
		}

		await socketUser.SendMessageAsync(sb.ToString());
	}

	

	//private static JsonStatusV0001 GetStatus()
	//{
	//	try
	//	{
	//		if (Client.ConnectionState != ConnectionState.Connected)
	//		{
	//			return JsonStatusV0001.EverythingNotOk("Not connected to discord");
	//		}

	//		return JsonStatusV0001.EverythingOk();
	//	}catch(Exception e)
	//	{
	//		return JsonStatusV0001.EverythingNotOk("Exception thrown in get status function");
	//	}
		
	//}


	

}
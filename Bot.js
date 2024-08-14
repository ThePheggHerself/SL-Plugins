// Require the necessary discord.js classes

const { Client, GatewayIntentBits, MessageEmbed, Events, ActivityType, PermissionsBitField } = require('discord.js');
const config = require('./config.json');

// Create a new client instance
const botClient = new Client({
	intents: [
		GatewayIntentBits.Guilds,
		GatewayIntentBits.GuildMessages,
	],
});

const net = require('net');
const tcpServer = net.createServer();
const connection = tcpServer.listen(config.port, config.address)

var messages = []
var mainSocket;

console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Bot starting up. Node version: ${process.version}`);


// #region Bot events
botClient.on("ready", () => {
	console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Bot online`)

	setInterval(() => {
		if (messages.length > 0) {
			var DiscordMessages = [];
			var discordMessage = "";
			messages.forEach(message => {
				if (discordMessage.length < 1)
					discordMessage = message;
				else if (discordMessage.length + message.length < 1950) {
					discordMessage += "\n" + message
				}
				else {
					DiscordMessages.push(discordMessage);

					discordMessage = message;
				}
			})

			if (discordMessage.length > 0)
				DiscordMessages.push(discordMessage);

			DiscordMessages.forEach(dMessage => {
				botClient.channels.cache.get(config.channel).send(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}]\n` + dMessage);
			})
			messages.length = 0
		}
	}, 2000)

	setInterval(() => {
		var object = { Type: "alive", channel: "RandomPacketToKeepAlive!" }

		if (mainSocket) {
			mainSocket.write(JSON.stringify(object))
		}
	}, 1000 * 60) //Every 60 seconds
});

botClient.on("warn", warn => console.warn(warn));
botClient.on("error", error => console.error(error.message));
botClient.on("unhandledRejection", error => console.error(error));
botClient.on('disconnect', () => console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Connection to the Discord API has been lost. I will attempt to reconnect momentarily`));
botClient.on('reconnecting', () => console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}]  Attempting to reconnect to the Discord API now. Please stand by...`));

tcpServer.on("connection", (socket) => {
	socket.setEncoding('utf8');

	console.log(`Bot listening at ${config.address}:${config.port}`)

	messages.push(`**++ - - - - - ++ SERVER ONLINE ++ - - - - -++**`
		+ "\n```"
		+ "\nThe connection between the bot and the server has been established```");

	mainSocket = socket;

	socket.on("data", (data) => {
		var string = data.toString();

		try {
			///You may be thinking why on earth this has been put here, however it's due to the way the system works.
			///Sometimes the JSON messages get mashed together while sending it, causing the bot to error out.
			///So this solves the issue. With this, forces a split whenever the recieved data recieves "{", splitting the JSON strings and fixing the issue

			console.log(data);

			var SplitJson = string.replace(/{/gi, '[[{');
			var array = SplitJson.split('[[');
			array.shift();

			array.forEach(json => {
				var object = JSON.parse(json)

				console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Recieved message. Type: ${object.Type}`)

				if (object.Type === 0) {
					if (object.Message) messages.push(object.Message);
				}
				else if (object.Type === 3) {
					if (botClient.user != null) {
						if (object.CurrentPlayers === "0/30") {
							botClient.user.setPresence({
								status: 'idle',
								activities: [{
									type: ActivityType.Custom,
									name: 'customstatus',
									state: `${object.CurrentPlayers} Players`
								}]
							})
						}
						else {
							botClient.user.setPresence({
								status: 'online',
								activities: [{
									type: ActivityType.Custom,
									name: 'customstatus',
									state: `${object.CurrentPlayers} Players`
								}]
							})
						}
					}
				}
				//plist (Used for when players check who is online)
				else if (object.Type === 2) {

					if (object.PlayerNames.startsWith("**")) {
						botClient.channels.cache.get(object.ChannelID).send(`${object.PlayerNames}`);
						return;
					}
					else {

						botClient.channels.cache.get(object.ChannelID).send(`**${object.CurrentPlayers}**\n\`\`\`${object.PlayerNames}\`\`\``);
					}		
				}
				else if (object.Type === 1) {
					console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] ${object.CommandMessage}`)
					botClient.channels.cache.get(object.ChannelID).send(`<@${object.StaffID}>\n` + object.CommandMessage)
				}
				else return
			});


		}
		catch (e) {
			console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] ${e}`);
		}
	})

	socket.on("close", () => {
		messages.push(`**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has been terminated (The server is possibly offline or restarting)```");
		console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Socket closed!`)

		botClient.user.setPresence({
			status: 'dnd',
			activities: [{
				type: ActivityType.Custom,
				name: 'customstatus',
				state: `Server Offline!`
			}]
		})
	});

	socket.on("error", error => {
		messages.push(`\n**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has errored!"
			+ `\n${error.message}\`\`\``);
		console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Socket closed: ${error}`);

		botClient.user.setPresence({
			status: 'dnd',
			activities: [{
				type: ActivityType.Custom,
				name: 'customstatus',
				state: `Server Offline!`
			}]
		})
	});

	socket.on("timeout", timeout => {
		messages.push(`\n**++ - - - - - ++ SERVER OFFLINE ++ - - - - -++**`
			+ "\n```"
			+ "\nThe connection between the bot and the server has timed out!```");
		console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Socket closed: timeout!`);

		botClient.user.setPresence({
			status: 'dnd',
			activities: [{
				type: ActivityType.Custom,
				name: 'customstatus',
				state: `Server Offline!`
			}]
		})
	});
});

botClient.on("messageCreate", (message) => {
	try {
		if (message.member == null || message.author == null || message.author.bot || !message.guild || message.content == null) return

		if (message.mentions.has(botClient.user)) {

			if (!mainSocket) {
				console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Socket Offline!`)
				message.channel.send("The bot is not connected to the server");
			}
			else {
				if (message.content.split(" ").length === 1) {
					try {
						var object = { Type: "plist", channel: message.channel.id }

						console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Requesting player list`)
						mainSocket.write(JSON.stringify(object))
						//Waits half a second
					}
					catch (e) { message.channel.send(e.message) }
				}

				else if (message.member.roles.cache.has(config.staffRoleID) || message.member.permissions.has('ADMINISTRATOR')) {
					try {
						if (!message.channel.permissionsFor(message.guild.roles.everyone).has(PermissionsBitField.Flags.ViewChannel)) {
							var object = { Type: "cmd", channel: message.channel.id, Message: message.content, StaffID: message.author.id, Staff: message.member.user.tag }
							console.log(`[${new Date().toLocaleDateString()} ${new Date().toLocaleTimeString()}] Sending remote command (${object.StaffID}): ${object.Message}`)
							mainSocket.write(JSON.stringify(object))
						}
					}
					catch (e) { message.channel.send(e.message) }
				}
			}
		}
	}
	catch (e) {
		console.log(e);
	}
})


botClient.login(config.token);
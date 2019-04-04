using Discord;
using DradonBotSharp.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace DradonBotSharp.Services
{
    public class SQLDatabaseService
    {
        private readonly IServiceProvider _services;
        private MySqlConnection _connection;

        public static string infoString = "SQLDatabaseService made by dradonhunter11 using MySql.Data. Version 0.0.1";

        public SQLDatabaseService(IServiceProvider service)
        {
            _services = service;
        }

        public async Task Initialize()
        {
            string connectionInfo = "server=localhost;userid=root;password=eWwWeGrVi3Hr02Fj;database=discordbot";
            try
            {
                _connection = new MySqlConnection(connectionInfo);
                _connection.Open();
                Console.WriteLine($"SQL version : {_connection.ServerVersion}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task InsertNewGuild(IGuild guild)
        {
            try
            {
                string commandText =
                    $"INSERT INTO `discord server` (GuildID, ServerName) VALUES ({guild.Id}, '{guild.Name}')";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                command.ExecuteNonQuery();
                IReadOnlyCollection<IGuildChannel> channelList = await guild.GetChannelsAsync();
                foreach (var guildChannel in channelList)
                {
                    commandText =
                        $"INSERT INTO channel (ChannelID, ChannelName, GuildID) VALUES ({guildChannel.Id}, '{guildChannel.Name}', {guild.Id})";
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Embed GetGuildChannelList(IGuild guild)
        {
            try
            {
                string commandText = $"SELECT ChannelName " +
                                     $"FROM channel ch JOIN `discord server` ds ON ch.GuildID = ds.GuildID " +
                                     $"ORDER BY ch.ChannelName ASC";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                MySqlDataReader commandOutput = command.ExecuteReader();

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithDescription($"Channel list for {guild.Name}");


                string channelList = "";

                if (commandOutput.HasRows)
                {
                    while (commandOutput.Read())
                    {
                        channelList += commandOutput.GetString("ChannelName");
                    }

                }

                builder.AddField(BotUtils.CreateEmbdedField("Number of channel", commandOutput.FieldCount));

                commandOutput.Close();
                return builder.Build();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public bool CreateCustomCommand(IGuild guild, SocketUser user, string commandName, string commandMessage)
        {
            try
            {
                string commandText =
                    $"INSERT INTO customcommand(GuildID, Command, Message, CreatorID) VALUES ({guild.Id}, '{commandName}', '{commandMessage}', {user.Id})";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool UpdateCustomCommand(IGuild guild, SocketUser user, string commandName, string commandMessage)
        {
            try
            {
                string commandText =
                    $"UPDATE customcommand SET Message = '{commandMessage}' " +
                    $"WHERE customcommand.GuildID = {guild.Id} AND customcommand.Command = '{commandName}' AND customcommand.CreatorID = {user.Id}";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public string GetCommand(IGuild guild, string commandName)
        {
            MySqlDataReader commandOutput = null;
            try
            {
                string commandText = $"SELECT Message " +
                                     $"FROM customcommand " +
                                     $"WHERE customcommand.Command = '{commandName}' AND customcommand.GuildID = {guild.Id}";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                commandOutput = command.ExecuteReader();

                if (commandOutput.HasRows)
                {
                    commandOutput.Read();
                    String output = commandOutput.GetString("Message");
                    commandOutput.Close();
                    return output;
                }
                commandOutput.Close();
                return "Custom command could not be found in the database.";

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (commandOutput != null)
                {
                    commandOutput.Close();
                }

                return "Custom command could not be found in the database.";
            }
        }

        public bool DeleteCustomCommand(IGuild guild, SocketUser user, string commandName)
        {
            try
            {
                string commandText = $"DELETE FROM customcommand " +
                                     $"WHERE customcommand.GuildID = {guild.Id} AND customcommand.CreatorID = {user.Id} AND customcommand.command = '{commandName}'";
                MySqlCommand command = new MySqlCommand(commandText, _connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task Close()
        {
            _connection.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HypixelSharp.Utils
{
    public sealed class GameType
    {
        
        public static readonly GameType QUAKECRAFT = new GameType("Quakecraft", "Quake", 2);
        public static readonly GameType WALLS = new GameType("Walls", "Walls", 3);
        public static readonly GameType PAINTBALL = new GameType("Paintball", "Paintball", 4);
        public static readonly GameType SURVIVAL_GAMES = new GameType("Blitz Survival Games", "HungerGames", 5);
        public static readonly GameType TNTGAMES = new GameType("The TNT Games", "TNTGames", 6);
        public static readonly GameType VAMPIREZ = new GameType("VampireZ", "VampireZ", 7);
        public static readonly GameType WALLS3 = new GameType("Mega Walls", "Walls3", 13);
        public static readonly GameType ARCADE = new GameType("Arcade", "Arcade", 14);
        public static readonly GameType ARENA = new GameType("Arena Brawl", "Arena", 17);
        public static readonly GameType MCGO = new GameType("Cops and Crims", "MCGO", 21);
        public static readonly GameType UHC = new GameType("UHC Champions", "UHC", 20);
        public static readonly GameType BATTLEGROUND = new GameType("Warlords", "Battleground", 23);
        public static readonly GameType SUPER_SMASH = new GameType("Smash Heroes", "SuperSmash", 24);
        public static readonly GameType GINGERBREAD = new GameType("Turbo Kart Racers", "GingerBread", 25);
        public static readonly GameType HOUSING = new GameType("Housing", "Housing", 26);
        public static readonly GameType SKYWARS = new GameType("SkyWars", "SkyWars", 51);
        public static readonly GameType TRUE_COMBAT = new GameType("Crazy Walls", "TrueCombat", 52);
        public static readonly GameType SPEED_UHC = new GameType("Speed UHC", "SpeedUHC", 54);
        public static readonly GameType SKYCLASH = new GameType("SkyClash", "SkyClash", 55);
        public static readonly GameType LEGACY = new GameType("Classic Games", "Legacy", 56);
        public static readonly GameType PROTOTYPE = new GameType("Prototype", "Prototype", 57);
        public static readonly GameType BEDWARS = new GameType("Bed Wars", "Bedwars", 58);
        public static readonly GameType MURDER_MYSTERY = new GameType("Murder Mystery", "MurderMystery", 59);
        public static readonly GameType BUILD_BATTLE = new GameType("Build Battle", "BuildBattle", 60);
        public static readonly GameType DUELS = new GameType("Duels", "Duels", 61);
        public static readonly GameType SKYBLOCK = new GameType("SkyBlock", "SkyBlock", 63);
        public static readonly GameType PIT = new GameType("Pit", "Pit", 64); 


        private readonly string name, dbName;
        private readonly int id;

        public static IEnumerable<GameType> Values 
        { 
            get
            {
                yield return QUAKECRAFT;
                yield return WALLS;
                yield return PAINTBALL;
                yield return SURVIVAL_GAMES;
                yield return TNTGAMES;
                yield return VAMPIREZ;
                yield return WALLS3;
                yield return ARCADE;
                yield return ARENA;
                yield return MCGO;
                yield return UHC;
                yield return BATTLEGROUND;
                yield return SUPER_SMASH;
                yield return GINGERBREAD;
                yield return HOUSING;
                yield return SKYWARS;
                yield return TRUE_COMBAT;
                yield return SPEED_UHC;
                yield return SKYCLASH;
                yield return LEGACY;
                yield return PROTOTYPE;
                yield return BEDWARS;
                yield return MURDER_MYSTERY;
                yield return BUILD_BATTLE;
                yield return DUELS;
                yield return SKYBLOCK;
                yield return PIT;
            }
        }

        public GameType(string name, string dbName, int id) 
        {
            this.name = name;
            this.dbName = dbName;
            this.id = id;
        }

        public static GameType FromDataBase(string dbName) { 
            foreach(GameType gameType in GameType.Values)
            {
                if (gameType.dbName == dbName) {
                    return gameType;
                }
            }
            return null;
        }

        public static GameType FromID(int id) {
            foreach (GameType gameType in GameType.Values)
            {
                if (gameType.id == id)
                {
                    return gameType;
                }
            }
            return null;
        }

        public string GetName() => name;
        public string GetDBName() => dbName;
        public int GetId() => id;
    }
}

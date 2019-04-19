using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Packets.Server.GameMessages;

namespace Uchu.Core.Scriptable
{
    /// <summary>
    ///     Manage chat commends.
    /// </summary>
    public static class ChatCommands
    {
        /*
         * Purely for experimental purposes. Has to be developed on.
         */

        public static async Task<string> GiveCommand(string[] args, Player player)
        {
            if (args.Length > 2) player = player.World.Players.First(p => p.ReplicaPacket.Name == args[2]);

            if (args.Length < 1) return "Command Syntax: /give <lot> <count(optional)> <player(optional)>\0";

            try
            {
                await player.AddItemAsync(int.Parse(args[0]), args.Length > 1 ? int.Parse(args[1]) : 1);
            }
            catch
            {
                return $"{args[0]} is not a LOT.\0";
            }

            return $"Gave LOT: {args[0]}{(args.Length > 1 ? $" x {args[1]}" : "")} to {player.ReplicaPacket.Name}.\0";
        }

        public static string FlyCommand(string[] args, Player player)
        {
            /*
             * Flight does not work.
             */

            if (args.Length == 0) return "Command Syntax: /fly <true/false>\0";
            bool on;
            switch (args[0].ToLower())
            {
                case "true":
                    on = true;
                    break;
                case "false":
                    on = false;
                    break;
                default:
                    return "Command Syntax: /fly <true/false>\0";
            }

            player.World.Server.Send(new SetJetPackModeMessage
            {
                BypassChecks = true,
                DoHover = true,
                Use = on
            }, player.EndPoint);
            return $"Flight set to {on}\0";
        }
    }
}
using System;
using UnityEngine;
using CommandHandler;
using System.Text;
using UnturnedNetworkAPI.NetworkRCON;

namespace BasicNetworkCommands
{
	public class BasicNetworkCommandsMain : MonoBehaviour
	{
		public void Start()
        {
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(GetPlayerPosition), new string[] { "pchart", "playerchart" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(RespawnNPC), new string[] { "respawnnpc", "respawnallnpc" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(Announce), new string[] { "announce", "anounce" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(GetUserIP), new string[] { "getuserip", "getip", "getplayerip" }));
			NetworkCommandsList.add(new NetworkCommand(4, new NetworkCommandDelegate(GetTime), new string[] { "gettime", "getime", "time" }));
			NetworkCommandsList.add(new NetworkCommand(4, new NetworkCommandDelegate(GetZombiesCount), new string[] { "getzombiecount", "getzombiescount", "getzombiecounts" })); 
			NetworkCommandsList.add(new NetworkCommand(4, new NetworkCommandDelegate(GetStructuresCount), new string[] { "getstructures", "getstructurescount", "getstructurecount" }));
			NetworkCommandsList.add(new NetworkCommand(4, new NetworkCommandDelegate(GetAnimalsCount), new string[] { "getanimals", "getanimalscount", "getanimalcount" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(KillAllAnimals), new string[] { "killallanimals", "killallanimal", "killanimals" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(KillAllZombies), new string[] { "killallzombies", "killallzombie", "killzombies" }));
			NetworkCommandsList.add(new NetworkCommand(4, new NetworkCommandDelegate(GetAllPlayersList), new string[] { "playerslist", "allplayersdump", "playerslist" }));
			NetworkCommandsList.add(new NetworkCommand(8, new NetworkCommandDelegate(TeleportPlayer), new string[] { "tp", "teleport", "teleportplayer", "teleportuser" }));
			NetworkCommandsList.add(new NetworkCommand(6, new NetworkCommandDelegate(GetAllZombiesPositions), new string[] { "zombiedump", "zombiesdump", "zombiesposition", "zombiespositions" }));
			NetworkCommandsList.add(new NetworkCommand(6, new NetworkCommandDelegate(GetAllAnimalsPositions), new string[] { "animaldump", "animalsdump", "animalsposition", "animalspositions" }));
			NetworkCommandsList.add(new NetworkCommand(10, new NetworkCommandDelegate(GetAllStructuresPositions), new string[] { "structuresdump", "structuredump", "structuresposition", "structurespositions" }));
			NetworkCommandsList.add(new NetworkCommand(6, new NetworkCommandDelegate(GetAllPlayersPositions), new string[] { "playersdump", "playerdump", "getallplayers", "playerspositions" }));
			NetworkCommandsList.add(new NetworkCommand(10, new NetworkCommandDelegate(Ban), new string[] { "ban", "Ban", "banplayer", "banuser" }));
			NetworkCommandsList.add(new NetworkCommand(10, new NetworkCommandDelegate(Kick), new string[] { "kick", "Kick", "kickplayer", "kickuser" }));
		}

        private void Ban(NetworkCommandArgs args)
        {
			string pkg = String.Empty;
			try
			{
				BetterNetworkUser player = UserList.getUserFromName(args.Parameters[0]);
				NetworkTools.ban(player.networkPlayer, player.name, player.steamid, args.Parameters[1]);
				pkg = $"Successfully banned player {player.name}, reason: {args.Parameters[1]}";
			}
			catch(Exception ex)
            {
				pkg = ex.Message;
            }
			args.sender.SendBytes(Encoding.UTF8.GetBytes(pkg));
        }
		private void Kick(NetworkCommandArgs args)
		{
			string pkg = String.Empty;
			try
			{
				BetterNetworkUser player = UserList.getUserFromName(args.Parameters[0]);
				NetworkTools.kick(player.networkPlayer, args.Parameters[1]);
				pkg = $"Successfully kicked player {player.name}, reason: {args.Parameters[1]}";
			}
			catch (Exception ex)
			{
				pkg = ex.Message;
			}
			args.sender.SendBytes(Encoding.UTF8.GetBytes(pkg));
		}
		private void GetPlayerPosition(NetworkCommandArgs args)
        {
			string pkg = String.Empty;
			try
			{
				Vector3 playerpos = UserList.getUserFromName(args.Parameters[0]).position;
				int player_x = Convert.ToInt32(playerpos.x);
				int player_z = Convert.ToInt32(playerpos.z);
				pkg = $"|{player_x}|{player_z}|";
			}
			catch
			{
				pkg = "An error occured while finding position of this player";
			}
			byte[] serverbuffer = Encoding.UTF8.GetBytes(pkg);
			args.sender.SendBytes(serverbuffer);
		}
		private void RespawnNPC(NetworkCommandArgs args)
        {
			string text = String.Empty;
			try
			{
				SpawnAnimals.reset();
				text = "Successfully respawned all zombies & animals";
				byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				text = "Error occured while tried to Respawn all NPC";
				byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void Announce(NetworkCommandArgs args)
        {
			string text = String.Empty;
			try
			{
				var announce = args.Parameters[0];
				NetworkChat.sendAlert(announce);
				text = "Successfully sent announce to Unturned Chat";
				byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				text = "Error occured while sending announce to Unturned chat";
				byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetUserIP(NetworkCommandArgs args)
        {
			try
			{
				NetworkChat.sendAlert($"User's Arg: {args.Parameters[0]}");
				string pIP = UserList.getUserFromName(args.Parameters[0]).networkPlayer.ipAddress;
				byte[] serverbuffer = Encoding.UTF8.GetBytes(pIP);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("Error occured while tried to get IP");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetTime(NetworkCommandArgs args)
        {
			float day = Sun.float_5;
			string time = Sun.getTime();
			string hour = "";
			if (time[1] == ':')
			{
				hour = $"{time[0]}";
			}
			else
			{
				hour = $"{time[0]}" + time[1];
			}
			string text = "Unknown";
			switch (hour)
			{
				case "6":
				case "7":
				case "8":
					if ((double)day < 0.05)
					{
						text = $"Unturned time: {time} AM (Dawn)";
					}
					else if (day < Sun.float_1 - 0.05f)
					{
						text = $"Unturned time: {time} AM (Dawn)";
					}
					else if ((double)day >= 0.95)
					{
						text = $"Unturned time: {time} PM (Dusk)";
					}
					else if (day < Sun.float_1)
					{
						text = $"Unturned time: {time} PM (Dusk)";
					}
					else if (day < Sun.float_1 + 0.05f)
					{
						text = $"Unturned time: {time} PM (Dusk)";
					}
					else
					{
						text = $"Unturned time: {time} PM (Dusk)";
					}
					break;
				case "9":
				case "10":
				case "11":
					if ((double)day < 0.05)
					{
						text = $"Unturned time: {time} AM (Morning)";
					}
					else if (day < Sun.float_1 - 0.05f)
					{
						text = $"Unturned time: {time} AM (Morning)";
					}
					else if ((double)day >= 0.95)
					{
						text = $"Unturned time: {time} PM (Evening)";
					}
					else if (day < Sun.float_1)
					{
						text = $"Unturned time: {time} PM (Evening)";
					}
					else if (day < Sun.float_1 + 0.05f)
					{
						text = $"Unturned time: {time} PM (Evening)";
					}
					else
					{
						text = $"Unturned time: {time} PM (Evening)";
					}
					break;
				case "12":
				case "13":
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
					if (day < Sun.float_1 - 0.05f)
					{
						text = $"Unturned time: {time} PM (Midday)";
					}
					else if (day < Sun.float_1 + 0.05f)
					{
						text = $"Unturned time: {time} AM (Midnight)";
					}
					else if (day < Sun.float_1)
					{
						text = $"Unturned time: {time} PM (Midday)";
					}
					else
					{
						text = $"Unturned time: {time} AM (Midnight)";
					}
					break;
			}
			byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
			args.sender.SendBytes(serverbuffer);
		}
		private void GetZombiesCount(NetworkCommandArgs args)
        {
			Zombie[] zombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
			int zombie_count = zombies.Length;
			int deathzomb = 0;
			for (int i = 0; i < zombies.Length; i++)
			{
				Zombie zombie = zombies[i];
				if (zombie.int_2 <= 0 || zombie.int_3 <= 0)
				{
					deathzomb++;
				}
			}
			string text = $"Zombies Alive: {zombies.Length - deathzomb} , Dead Zombies: {deathzomb}";
			byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
			args.sender.SendBytes(serverbuffer);
		}
		private void GetStructuresCount(NetworkCommandArgs args)
        {
			ServerStructure[] structures = SpawnStructures.list_0.ToArray();
			string structures_count = $"Structures on Server: {structures.Length}";
			byte[] serverbuffer = Encoding.UTF8.GetBytes(structures_count);
			args.sender.SendBytes(serverbuffer);
		}
		private void GetAnimalsCount(NetworkCommandArgs args)
        {
			Animal[] animals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
			int animals_count = animals.Length;
			int deadanim = 0;
			for (int i = 0; i < animals.Length; i++)
			{
				Animal animal = animals[i];
				if (animal.int_2 <= 0 || animal.int_3 <= 0)
				{
					deadanim++;
				}
			}
			string text = $"Animals Alive: {animals.Length - deadanim} , Dead Animals: {deadanim}";
			byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
			args.sender.SendBytes(serverbuffer);
		}
		private void KillAllAnimals(NetworkCommandArgs args)
        {
			Animal[] animals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
			int animals_count = animals.Length;
			for (int i = 0; i < animals.Length; i++)
			{
				Animal animal = animals[i];
				animal.damage(500);
			}
			string text = $"Successfully killed {animals_count} Animals";
			byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
			args.sender.SendBytes(serverbuffer);
		}
		private void KillAllZombies(NetworkCommandArgs args)
		{
			Zombie[] zombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
			int zombies_count = zombies.Length;
			for (int i = 0; i < zombies.Length; i++)
			{
				Zombie zombie = zombies[i];
				zombie.damage(500);
			}
			string text = $"Successfully killed {zombies_count} Zombies";
			byte[] serverbuffer = Encoding.UTF8.GetBytes(text);
			args.sender.SendBytes(serverbuffer);
		}
		private void GetAllPlayersList(NetworkCommandArgs args)
		{
			string ptext = String.Empty;
			try
			{
				for (int i = 0; i < NetworkUserList.list_0.Count; i++)
				{
					NetworkUser p = NetworkUserList.list_0[i];
					ptext += p.string_0;
					ptext += ",";
				}
				byte[] serverbuffer = Encoding.UTF8.GetBytes(ptext);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("Error");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void TeleportPlayer(NetworkCommandArgs args)
        {
			string text = String.Empty;
			try
			{
				TeleportToCoords(Convert.ToSingle(args.Parameters[1]), Convert.ToSingle(args.Parameters[2]), Convert.ToSingle(args.Parameters[3]), UserList.getUserFromName(args.Parameters[0]));
				text = $"Successfully teleported {UserList.getUserFromName(args.Parameters[0]).name} To {Convert.ToSingle(args.Parameters[1])} {Convert.ToSingle(args.Parameters[2])} {Convert.ToSingle(args.Parameters[3])}";
				args.sender.SendBytes(Encoding.UTF8.GetBytes(text));
			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("An error occured with teleportation of player");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetAllZombiesPositions(NetworkCommandArgs args)
		{
			string _pkg = String.Empty;
			try
			{
				Zombie[] zombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
				for (int i = 0; i < zombies.Length; i++)
				{
					Zombie zombie = zombies[i];
					if (zombie.int_2 > 0 && zombie.int_3 > 0)
					{
						_pkg += $"{Convert.ToInt32(zombie.GetComponent<Transform>().position.x)}|{Convert.ToInt32(zombie.GetComponent<Transform>().position.z)}/";
					}
				}
				byte[] serverbuffer = Encoding.UTF8.GetBytes(_pkg);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("An error occured with dumping zombies");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetAllAnimalsPositions(NetworkCommandArgs args)
        {
			string _pkg = String.Empty;
			try
			{
				Animal[] animals_objects = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
				for (int i = 0; i < animals_objects.Length; i++)
				{
					Animal _animal = animals_objects[i];
					if (_animal.int_2 > 0 && _animal.int_3 > 0)
					{
						_pkg += $"{Convert.ToInt32(_animal.GetComponent<Transform>().position.x)}|{Convert.ToInt32(_animal.GetComponent<Transform>().position.z)}/";
					}
				}
				byte[] serverbuffer = Encoding.UTF8.GetBytes(_pkg);
				args.sender.SendBytes(serverbuffer);
			}
			catch (Exception err)
			{
				NetworkChat.sendAlert($"Error: {err.Message}");
				byte[] serverbuffer = Encoding.UTF8.GetBytes("An error occured with dumping animals");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetAllStructuresPositions(NetworkCommandArgs args)
        {
			string _pkg = String.Empty;
			try
			{
				ServerStructure[] structures = SpawnStructures.list_0.ToArray();
				for (int i = 0; i < structures.Length; i++)
				{

					ServerStructure structure = structures[i];
					_pkg += $"{Convert.ToInt32(structure.vector3_0.x)}|{Convert.ToInt32(structure.vector3_0.z)}/";
				}
				byte[] serverbuffer = Encoding.UTF8.GetBytes(_pkg);
				args.sender.SendBytes(serverbuffer);

			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("An error occured with dumping animals");
				args.sender.SendBytes(serverbuffer);
			}
		}
		private void GetAllPlayersPositions(NetworkCommandArgs args)
		{
			string _pkg = String.Empty;
			try
			{
				for (int i = 0; i < UserList.users.Count; i++)
				{
					BetterNetworkUser player = UserList.users[i];
					_pkg += $"{Convert.ToInt32(player.position.x)}|{Convert.ToInt32(player.position.z)}|{player.model.GetComponent<Clothes>().int_0}|{player.name}/";
				}
				byte[] serverbuffer = Encoding.UTF8.GetBytes(_pkg);
				args.sender.SendBytes(serverbuffer);
			}
			catch
			{
				byte[] serverbuffer = Encoding.UTF8.GetBytes("An error occured with dumping animals");
				args.sender.SendBytes(serverbuffer);
			}
		}
		public static void TeleportToCoords(float x, float y, float z, BetterNetworkUser player)
		{
			Vector3 target = new Vector3(x, y, z);
			player.position = (target);
			player.player.gameObject.GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, target, player.rotation);
			player.player.gameObject.GetComponent<NetworkInterpolation>().tellStatePosition_Pizza(target, player.rotation);
			Network.SetReceivingEnabled(player.networkPlayer, 0, false);
			System.Threading.Timer timer = new System.Threading.Timer(delegate
			{
				Network.SetReceivingEnabled(player.networkPlayer, 0, true);
			}, null, 2000, -1);
		}

	}
}

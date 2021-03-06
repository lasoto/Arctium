﻿/*
 * Copyright (C) 2012-2013 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Framework.Console;
using WorldServer.Game.Packets.PacketHandler;
using WorldServer.Game.Spawns;
using WorldServer.Game.WorldEntities;
using WorldServer.Network;

namespace WorldServer.Game.Chat.Commands
{
    public class CreatureCommands : Globals
    {
        [ChatCommand("addnpc")]
        public static void AddNpc(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            int creatureId = CommandParser.Read<int>(args, 1);

            Creature creature = DataMgr.FindCreature(creatureId);
            if (creature != null)
            {
                CreatureSpawn spawn = new CreatureSpawn()
                {
                    Guid     = CreatureSpawn.GetLastGuid() + 1,
                    Id       = creatureId,
                    Creature = creature,
                    Position = pChar.Position,
                    Map      = pChar.Map
                };

                if (spawn.AddToDB())
                {
                    spawn.AddToWorld();
                    ChatHandler.SendMessageByType(ref session, 0, 0, "Spawn successfully added.");
                }
                else
                    ChatHandler.SendMessageByType(ref session, 0, 0, "Spawn can't be added.");
            }
        }

        [ChatCommand("delnpc")]
        public static void DeleteNpc(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            var spawn = SpawnMgr.FindSpawn(pChar.TargetGuid);

            if (spawn != null)
            {
                SpawnMgr.RemoveSpawn(spawn);

                WorldMgr.SendToInRangeCharacter(pChar, ObjectHandler.HandleDestroyObject(ref session, pChar.TargetGuid));
                ChatHandler.SendMessageByType(ref session, 0, 0, "Selected Spawn successfully removed.");
            }
            else
                ChatHandler.SendMessageByType(ref session, 0, 0, "Not a creature.");
        }
    }
}

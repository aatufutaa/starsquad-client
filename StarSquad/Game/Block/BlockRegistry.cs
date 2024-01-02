using System;
using System.Collections.Generic;
using StarSquad.Game.Block.Blocks;
using StarSquad.Net.Packet;

namespace StarSquad.Game.Block
{
    public class BlockRegistry
    {
        private static readonly Dictionary<int, Type> blocks = new Dictionary<int, Type>();

        static BlockRegistry()
        {
            blocks.Add(0, typeof(BlockBox));
            blocks.Add(1, typeof(BlockGrass));
            blocks.Add(2, typeof(BlockWater));
            blocks.Add(3, typeof(BlockTree));
        }

        public static BlockBase ReadBlock(int id, ByteBuf buf)
        {
            if (!blocks.TryGetValue(id, out var type))
                throw new Exception("cant find block " + id);
            
            var block = (BlockBase)Activator.CreateInstance(type);
            block.ReadStatic(buf);
            return block;
        }
    }
}
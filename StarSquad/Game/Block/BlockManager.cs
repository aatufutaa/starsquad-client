using System.Collections.Generic;
using StarSquad.Game.Block.Blocks;
using StarSquad.Game.Misc;
using StarSquad.Loader.Asset;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StarSquad.Game.Block
{
    public class BlockManager
    {
        public int mapSizeX;
        public int mapSizeY;

        private BlockCollision[] collisions;
        private BlockBase[] specialBlocks;

        private readonly HashSet<BlockBase> tickingBlocks;

        public readonly TileManager tileManager;

        private string mapName;
        private CustomAssetBundle mapBundle;
        private string mapAccessPath;

        public BlockManager()
        {
            this.tickingBlocks = new HashSet<BlockBase>();
            this.tileManager = new TileManager();
        }

        public void Init(string name, int mapSizeX, int mapSizeY)
        {
            this.mapSizeX = mapSizeX;
            this.mapSizeY = mapSizeY;

            this.collisions = new BlockCollision[mapSizeX * mapSizeY];
            this.specialBlocks = new BlockBase[mapSizeX * mapSizeY];

            name = "Default"; //remove
            this.mapName = name;
            this.mapBundle = AssetManager.LoadAssetBundle0("default_map_bundle"); // TODO
            this.mapAccessPath = "Assets/Remote/Game/Maps/" + this.mapName + "/Access/";

            var waterSprites = this.LoadAllMapAssets<Sprite>("Blocks/water_tiles.png");
            this.tileManager.Init(waterSprites, mapSizeX, mapSizeY);

            // load terrain
            this.LoadTerrain();
        }

        public T LoadMapAsset<T>(string path) where T : Object
        {
            return this.mapBundle.LoadAsset<T>(this.mapAccessPath + path);
        }

        private T[] LoadAllMapAssets<T>(string path) where T : Object
        {
            return this.mapBundle.LoadAllAssets<T>(this.mapAccessPath + path);
        }

        public GameObject LoadBlockAsset(string path)
        {
            return this.LoadMapAsset<GameObject>("Blocks/" + path);
        }

        private void LoadTerrain()
        {
            var sizeFixX = this.mapSizeX / 2;
            var sizeFixY = this.mapSizeY / 2;
            var terrainTilemap = GameObject.Find("EdgeTilemap").GetComponent<Tilemap>();

            var terrainSprites = this.LoadAllMapAssets<Sprite>("forest_tile_v2.png");

            var x1 = sizeFixX / 2;
            var y1 = sizeFixY / 2;

            var minX = -x1 - 1;
            var maxX = x1 + 1;
            var minY = -y1 - 1;
            var maxY = y1 + 1;

            terrainTilemap.SetTile(new Vector3Int(minX, maxY, 0), CreateTile(terrainSprites, "top_left")); // top left
            terrainTilemap.SetTile(new Vector3Int(maxX, maxY, 0), CreateTile(terrainSprites, "top_right")); // top right
            terrainTilemap.SetTile(new Vector3Int(minX, minY, 0),
                CreateTile(terrainSprites, "bottom_left")); // bottom left
            terrainTilemap.SetTile(new Vector3Int(maxX, minY, 0),
                CreateTile(terrainSprites, "bottom_right")); // bottom right

            var top = CreateTile(terrainSprites, "top");
            var bottom = CreateTile(terrainSprites, "bottom");
            for (var x = minX + 3; x < maxX - 2; x += 2)
            {
                terrainTilemap.SetTile(new Vector3Int(x, maxY, 0), top);
                terrainTilemap.SetTile(new Vector3Int(x, minY, 0), bottom);
            }

            var left = CreateTile(terrainSprites, "left");
            var right = CreateTile(terrainSprites, "right");
            for (var y = minY + 3; y < maxY - 2; y += 2)
            {
                terrainTilemap.SetTile(new Vector3Int(minX, y, 0), left);
                terrainTilemap.SetTile(new Vector3Int(maxX, y, 0), right);
            }

            var groundTilemap = GameObject.Find("GroundTilemap").GetComponent<Tilemap>();
            var groundTile = CreateTileFromSprite(this.LoadMapAsset<Sprite>("ground.png"));

            var gridStartX = Mathf.FloorToInt(sizeFixX / 8f) + 1;
            var gridStartY = Mathf.FloorToInt(sizeFixY / 8f) + 1;

            for (var x = -gridStartX; x < gridStartX; x++)
            {
                for (var y = -gridStartY; y < gridStartY; y++)
                {
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
            }

            groundTilemap.SetTile(new Vector3Int(0, 0, 0), groundTile);
        }

        private static Tile CreateTile(Sprite[] sprites, string name)
        {
            foreach (var s in sprites)
            {
                if (s.name != name) continue;
                return CreateTileFromSprite(s);
            }
            return null;
        }

        private static Tile CreateTileFromSprite(Sprite sprite)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            return tile;
        }

        public void OnBlocksFinished()
        {
            this.tileManager.AddWaterTiles();
        }

        public BlockBase GetSpecialBlock(int x, int y)
        {
            return this.IsInBounds(x, y) ? this.specialBlocks[this.GetCoordKey(x, y)] : null;
        }

        public BlockCollision GetCollision(int x, int y)
        {
            return this.IsInBounds(x, y) ? this.collisions[this.GetCoordKey(x, y)] : null;
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= -this.mapSizeX / 2 && x < this.mapSizeX / 2 && y >= -this.mapSizeY / 2 && y < this.mapSizeY / 2;
        }

        public void AddBlock(BlockBase block)
        {
            block.Add(this);
            this.specialBlocks[this.GetCoordKey(block.x, block.y)] = block; // TODO: static check
            //this.collisions[this.GetCoordKey(block.x, block.y)] = block.GetCollision();
        }

        public void AddCollision(int x, int y, BlockCollision collision)
        {
            this.collisions[this.GetCoordKey(x, y)] = collision;
        }

        public void RemoveCollision(int x, int y)
        {
            this.AddCollision(x, y, null);
        }

        private int GetCoordKey(int x, int y)
        {
            return this.mapSizeX * (y + this.mapSizeY / 2) + (x + this.mapSizeX / 2);
        }

        public void Tick()
        {
            var removeList = new List<BlockBase>();
            foreach (var block in this.tickingBlocks)
            {
                block.Tick();

                if (!block.ticking)
                    removeList.Add(block);
            }

            foreach (var block in removeList)
            {
                this.tickingBlocks.Remove(block);
            }
        }

        public void Render()
        {
            foreach (var block in this.tickingBlocks)
            {
                block.Render();
            }
        }

        // starts ticking block
        public void TickBlock(BlockBase block)
        {
            if (block.ticking) return;
            block.ticking = true;
            this.tickingBlocks.Add(block);
        }
    }
}
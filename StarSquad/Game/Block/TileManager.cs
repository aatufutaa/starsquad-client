using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StarSquad.Game.Block
{
    public class TileManager
    {
        private Tilemap tilemap;

        private Tile topLeft;
        private Tile top;
        private Tile topRight;

        private Tile left;
        private Tile center;
        private Tile right;

        private Tile bottomLeft;
        private Tile bottom;
        private Tile bottomRight;

        private Tile topLeftCorner;
        private Tile topRightCorner;
        private Tile bottomLeftCorner;
        private Tile bottomRightCorner;

        private WaterTile[] serverTiles;
        private int mapSizeX;
        private int mapSizeY;

        public class WaterTile
        {
            public WaterTile left;
            public WaterTile top;
            public WaterTile right;
            public WaterTile bottom;

            public int x;
            public int y;
        }
        
        public void Init(Sprite[] sprites, int mapSizeX, int mapSizeY)
        {
            this.tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();

            this.topLeft = CreateTile(sprites, "top_left");
            this.top = CreateTile(sprites, "top");
            this.topRight = CreateTile(sprites, "top_right");

            this.left = CreateTile(sprites, "left");
            this.center = CreateTile(sprites, "center");
            this.right = CreateTile(sprites, "right");

            this.bottomLeft = CreateTile(sprites, "bottom_left");
            this.bottom = CreateTile(sprites, "bottom");
            this.bottomRight = CreateTile(sprites, "bottom_right");

            this.topLeftCorner = CreateTile(sprites, "top_left_corner");
            this.topRightCorner = CreateTile(sprites, "top_right_corner");
            this.bottomLeftCorner = CreateTile(sprites, "bottom_left_corner");
            this.bottomRightCorner = CreateTile(sprites, "bottom_right_corner");

            this.serverTiles = new WaterTile[mapSizeX * mapSizeY];
            this.mapSizeX = mapSizeX;
            this.mapSizeY = mapSizeY;
        }

        private static Tile CreateTile(IEnumerable<Sprite> sprites, string name)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();

            try
            {
                foreach (var sprite in sprites)
                {
                    if (sprite.name != name) continue;

                    tile.sprite = sprite;
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Failed to load tile " + name);
                Debug.Log(e);
            }

            return tile;
        }

        private int GetServerTileK(int x, int y)
        {
            return (y + this.mapSizeY / 2) * this.mapSizeX + (x + this.mapSizeX / 2);
        }

        private WaterTile GetServerTile(int x, int y)
        {
            return this.serverTiles[this.GetServerTileK(x, y)];
        }

        public void AddWater(int x, int y)
        {
            var tile = new WaterTile();
            tile.x = x;
            tile.y = y;
            this.serverTiles[this.GetServerTileK(x, y)] = tile;

            // has left
            if (x != -20 / 2)
            {
                var left = this.GetServerTile(x - 1, y);
                if (left != null)
                {
                    tile.left = left;
                    left.right = tile;
                }
            }

            // has bottom
            if (y != -20 / 2)
            {
                var bottom = this.GetServerTile(x, y - 1);
                if (bottom != null)
                {
                    tile.bottom = bottom;
                    bottom.top = tile;
                }
            }
        }

        public void AddWaterTiles()
        {
            foreach (var waterTile in this.serverTiles)
            {
                if (waterTile == null) continue;

                var x = waterTile.x;
                var y = waterTile.y;

                var hasLeft = waterTile.left != null;
                var hasBottom = waterTile.bottom != null;
                var hasTop = waterTile.top != null;
                var hasRight = waterTile.right != null;

                // bottom
                if (hasBottom)
                {
                    // left
                    if (hasLeft)
                    {
                        var hasBottomLeftCorner = waterTile.left.bottom != null;
                        if (hasBottomLeftCorner)
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2, y * 2, 0), this.center);
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2, y * 2, 0), this.bottomLeftCorner);
                        }
                    }
                    /*else
                    {
                        var hasTopLeftCorner = waterTile.bottom.left != null;
                        if (hasTopLeftCorner)
                        {
                        }
                        else
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2, y * 2, 0), this.left);
                        }
                    }
                    */

                    // r
                    if (hasRight)
                    {
                        var hasBottomRightCorner = waterTile.right.bottom != null;
                        if (hasBottomRightCorner)
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2, 0), this.center);
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2, 0), this.bottomRightCorner);
                        }
                    }
                    /*else
                    {
                        var hasTopRightCorner = waterTile.bottom.right != null;
                        if (hasTopRightCorner)
                        {
                            // skip
                        }
                        else
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2, 0), this.right);
                        }
                    }*/
                }
                else
                {
                    // left
                    if (hasLeft)
                    {
                        /*var hasBottomRightCorner = waterTile.left.bottom != null;
                        if (hasBottomRightCorner)
                        {
                            // skip
                        }
                        else
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2, y * 2, 0), this.bottom);
                        }*/
                    }
                    else
                    {
                        this.tilemap.SetTile(new Vector3Int(x * 2, y * 2, 0), this.bottomLeft);
                    }

                    // right
                    if (hasRight)
                    {
                        var hasBottomLeftCorner = waterTile.right.bottom != null;
                        if (hasBottomLeftCorner)
                        {
                            // skip
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2, 0), this.bottom);
                        }
                    }
                    else
                    {
                        this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2, 0), this.bottomRight);
                    }
                }

                // top
                if (hasTop)
                {
                    // left
                    if (hasLeft)
                    {
                        var hasTopLeftCorner = waterTile.top.left != null;
                        if (hasTopLeftCorner)
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2, y * 2 + 1, 0), this.center);
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2, y * 2 + 1, 0), this.topLeftCorner);
                        }
                    }
                    else
                    {
                        var hasBottomLeftCorner = waterTile.top.left != null;
                        if (hasBottomLeftCorner)
                        {
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2, y * 2 + 1, 0), this.left);
                        }
                    }

                    // r
                    if (hasRight)
                    {
                        var hasTopRightCorner = waterTile.top.right != null;
                        if (hasTopRightCorner)
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2 + 1, 0), this.center);
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2 + 1, 0), this.topRightCorner);
                        }
                    }
                    else
                    {
                        var hasBottomRightCorner = waterTile.top.right != null;
                        if (hasBottomRightCorner)
                        {
                            // skip
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2 + 1, 0), this.right);
                        }
                    }
                }
                else
                {
                    // left
                    if (hasLeft)
                    {
                        /*var hasTopRightCorner = waterTile.left.top != null;
                        if (hasTopRightCorner)
                        {
                            // skip
                        }
                        else
                        {
                            //this.tilemap.SetTile(new Vector3Int(x * 2, y * 2 + 1, 0), this.top);
                        }
                        
                        
                        
                        
                        
                    */
                    }
                    else
                    {
                        this.tilemap.SetTile(new Vector3Int(x * 2, y * 2 + 1, 0), this.topLeft);
                    }

                    // ri
                    if (hasRight)
                    {
                        var hasTopLeftCorner = waterTile.right.top != null;
                        if (hasTopLeftCorner)
                        {
                        }
                        else
                        {
                            this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2 + 1, 0), this.top);
                        }
                    }
                    else
                    {
                        this.tilemap.SetTile(new Vector3Int(x * 2 + 1, y * 2 + 1, 0), this.topRight);
                    }
                }
            }
        }
    }
}
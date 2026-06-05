using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public struct NeighborPattern : IEquatable<NeighborPattern>
    {
        public static readonly NeighborPattern Up = new(new Vector2Int[]{new(0, 1)}, new Vector2Int(1,1), 3, 3);
        public static readonly NeighborPattern Down = new(new Vector2Int[]{new(0, -1)}, new Vector2Int(1,1), 3, 3);
        public static readonly NeighborPattern Left = new(new Vector2Int[]{new(-1, 0)}, new Vector2Int(1,1), 3, 3);
        public static readonly NeighborPattern Right = new(new Vector2Int[]{new(1, 0)}, new Vector2Int(1,1), 3, 3);
        
        public Vector2Int[] positions;
        public Vector2Int center;
        public int width;
        public int height;

        public NeighborPattern(Texture2D texture, Color neighborColor, Color centerColor)
        {
            const float tolerance = 0.1f;
            Color32[] pixels = texture.GetPixels32();
            List<Vector2Int> foundPositions = new();

            this.width = texture.width;
            this.height = texture.height;
            this.center = new Vector2Int(this.width / 2, this.height / 2);
            this.positions = null;
            
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color32 pixel = pixels[y * texture.width + x];

                    if (!CheckColor(pixel, centerColor)) continue;
                    this.center = new Vector2Int(x, y);
                    break;
                }
            }
                
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color32 pixel = pixels[y * texture.width + x];

                    if (!CheckColor(pixel, neighborColor))
                        continue;
                    foundPositions.Add(new Vector2Int(x - this.center.x, y - this.center.y));
                }
            }
            this.positions = foundPositions.ToArray();
            return;

            bool CheckColor(Color a, Color b) => 
                Math.Abs(a.r - b.r) < tolerance && 
                Math.Abs(a.g - b.g) < tolerance && 
                Math.Abs(a.b - b.b) < tolerance && 
                Math.Abs(a.a - b.a) < tolerance;
        }

        public NeighborPattern(Vector2Int[] positions, Vector2Int center, int width, int height)
        {
            this.positions = positions;
            this.center = center;
            this.width = width;
            this.height = height;
        }

        public bool Equals(NeighborPattern other)
        {
            return Equals(positions, other.positions) && center.Equals(other.center) && width == other.width && height == other.height;
        }

        public override bool Equals(object obj)
        {
            return obj is NeighborPattern other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(positions, center, width, height);
        }
    }
}
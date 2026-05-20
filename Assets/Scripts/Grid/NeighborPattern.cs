using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    [Serializable]
    public struct NeighborPattern
    {
        public Vector2Int[] positions;
        public int width;
        public int height;

        public NeighborPattern(Texture2D texture, Color toCheckColor)
        {
            Color32[] pixels = texture.GetPixels32();
            const float tolerance = 0.1f;
            
            int centerX = texture.width / 2;
            int centerY = texture.height / 2;
            
            List<Vector2Int> foundPositions = new();
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color32 pixel = pixels[y * texture.width + x];

                    if (!CheckColor(pixel, toCheckColor)) continue;
                    foundPositions.Add(new Vector2Int(x - centerX, y - centerY));
                }
            }
            this.positions = foundPositions.ToArray();
            
            this.width = texture.width;
            this.height = texture.height;
            return;

            bool CheckColor(Color a, Color b) => 
                Math.Abs(a.r - b.r) < tolerance && 
                Math.Abs(a.g - b.g) < tolerance && 
                Math.Abs(a.b - b.b) < tolerance && 
                Math.Abs(a.a - b.a) < tolerance;
        }
    }
}
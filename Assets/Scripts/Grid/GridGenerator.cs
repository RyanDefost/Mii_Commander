using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Grid
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField]
        private Material[] materials;
        [SerializeField]
        private Bounds boardBounds;
        [SerializeField]
        private Vector2Int boardSize;
        [SerializeField]
        private Vector3[] positions;
        [SerializeField]
        private Vector2 cellSize;
        [SerializeField]
        private Mesh cellMesh;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.enabled = this.materials is { Length: > 0 } && this.cellMesh;
        
            this.boardBounds.center = this.transform.position + Vector3.back * 0.01f;

            this.positions = new Vector3[this.boardSize.x * this.boardSize.y];
            CalculateGrid((offset, cellSize, bottomLeft, position) =>
            {
                if (this.cellSize != (Vector2)cellSize)
                    this.cellMesh = null;
                this.cellSize = cellSize;
                int index = position.x + (position.y * this.boardSize.x);
                this.positions[index] = bottomLeft + offset;
            });

            if (this.cellMesh) return;
            this.cellMesh = GenerateCellMesh();
            if (this.cellMesh)
                SaveMeshAsset();
        }
#endif

        private void OnDrawGizmosSelected()
        {
            if (this.boardSize.x <= 0 || this.boardSize.y <= 0) return;

            // Draw the outer container
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.boardBounds.center, this.boardBounds.size);

            CalculateGrid((offset, cellSize, bottomLeft, _) => Gizmos.DrawWireCube(bottomLeft + offset, cellSize));

            foreach (Vector3 position in this.positions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(position, 0.1f);
            }
        }

        private void CalculateGrid(Action<Vector3, Vector3, Vector3, Vector2Int> atCell)
        {
            if (this.boardSize.x <= 0 || this.boardSize.y <= 0) return;

            // Calculate dimensions
            float cellWidth = this.boardBounds.size.x / this.boardSize.x;
            float cellHeight = this.boardBounds.size.y / this.boardSize.y;
            Vector3 cellSize = new(cellWidth, cellHeight, 0.01f);
            Vector3 bottomLeft = this.boardBounds.center - (this.boardBounds.size * 0.5f);

            for (int x = 0; x < this.boardSize.x; x++)
            {
                for (int y = 0; y < this.boardSize.y; y++)
                {
                    Vector3 offset = new(
                        (x + 0.5f) * cellWidth,
                        (y + 0.5f) * cellHeight,
                        0
                    );
                
                    atCell.Invoke(offset, cellSize, bottomLeft, new Vector2Int(x, y));
                }
            }
        }
    
        private Mesh GenerateCellMesh()
        {
            Mesh mesh = new() { name = "GridCellMesh" };

            float halfWidth = this.cellSize.x * 0.5f;
            float halfHeight = this.cellSize.y * 0.5f;

            // 1. Define Vertices (relative to center 0,0)
            Vector3[] vertices = {
                new(-halfWidth, -halfHeight, 0), // Bottom Left  (Index 0)
                new( halfWidth, -halfHeight, 0), // Bottom Right (Index 1)
                new(-halfWidth,  halfHeight, 0), // Top Left     (Index 2)
                new( halfWidth,  halfHeight, 0)  // Top Right    (Index 3)
            };

            // 2. Define Triangles (Clockwise winding order)
            int[] triangles = {
                0, 2, 1, // First Triangle
                2, 3, 1  // Second Triangle
            };

            // 3. Define UVs (for textures)
            Vector2[] uvs = {
                new(0, 0),
                new(1, 0),
                new(0, 1),
                new(1, 1)
            };

            // Assign to mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
    
            // Calculate normals so it reacts to light
            mesh.RecalculateNormals();

            return mesh;
        }
    
#if UNITY_EDITOR
        private void SaveMeshAsset()
        {
            var scene = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(scene.path))
            {
                Debug.LogError("Save the scene before generating/saving the mesh!");
                return;
            }

            // 1. Get the directory and name of the scene
            string scenePath = scene.path; // e.g., Assets/Scenes/MainLevel.unity
            string sceneDirectory = Path.GetDirectoryName(scenePath);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        
            // 2. Define the target folder (SceneName folder)
            string folderPath = Path.Combine(sceneDirectory, sceneName);

            // 3. Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(sceneDirectory, sceneName);
            }

            // 4. Generate/Prepare the mesh
            this.cellMesh = GenerateCellMesh();
        
            // 5. Save the mesh as an .asset file
            string assetPath = Path.Combine(folderPath, "GridCellMesh.asset");
        
            // Use CreateAsset if new, or overwrite if it exists
            AssetDatabase.CreateAsset(this.cellMesh, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Mesh saved successfully to: {assetPath}");
        }
#endif

        private void Start()
        {
            int state = 0;
            foreach (Vector3 position in this.positions)
            {
                GameObject cell = new()
                {
                    transform =
                    {
                        parent = this.transform,
                        position = position + Vector3.back * 0.01f,
                        name = "gridCell"
                    }
                };

                cell.AddComponent<MeshFilter>().sharedMesh = this.cellMesh;
                MeshRenderer renderer = cell.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = this.materials[state];
            
                state++;
                if (state >= this.materials.Length) state = 0;
            }
        }

        public List<Vector3?> GetAllPositions() => this.positions.Select(pos => (Vector3?)pos).ToList();
        public Vector3 GetPosAt(int index) => this.positions[index];
    }
}

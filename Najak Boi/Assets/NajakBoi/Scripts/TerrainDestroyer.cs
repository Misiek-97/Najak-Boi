using UnityEngine;
using UnityEngine.Tilemaps;

namespace NajakBoi.Scripts
{
    public class TerrainDestroyer : MonoBehaviour
    {
        public Tilemap terrain1;
        public Tilemap terrain2;
        public static TerrainDestroyer Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void DestroyTerrain(Vector3 boomPos, float radius, Tilemap terrain)
        {
            for (var x = -(int) radius; x < radius; x++)
            {
                for (var y = -(int) radius; y < radius; y++)
                {
                    if (!(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) < Mathf.Pow(radius, 2))) continue;
                    
                    var tilePos = terrain.WorldToCell(boomPos + new Vector3(x, y, 0));

                    if (terrain.GetTile(tilePos) != null)
                        DestroyTile(tilePos, terrain);
                }
            }
        
        }

        private static void DestroyTile(Vector3Int tilePos, Tilemap terrain)
        {
            terrain.SetTile(tilePos, null);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace NajakBoi.Scripts
{
    public class TerrainGenerator : MonoBehaviour
    {
        public Tilemap tilemap;
        public TileBase tile;
        private int[,] _mapArray;
        public int terrainWidth, terrainHeight;

        public int intervalMin = 1;
        public int intervalMax = 8;
        public float seedMin = 0.1f;
        public float seedMax = 3.0f;
        // Start is called before the first frame update
        void Start()
        {
            tilemap = GetComponent<Tilemap>();
            GenerateTerrain();
        }

        // Update is called once per frame
        void Update()
        {
            var kb = Keyboard.current;
            if (kb.gKey.wasPressedThisFrame)
                GenerateTerrain();
        }

        public void GenerateTerrain()
        {
            var seed = Random.Range(seedMin, seedMax);
            var interval = Random.Range(intervalMin, intervalMax);
            _mapArray = GenerateArray(terrainWidth, terrainHeight, true);
            StartCoroutine(RenderMap(PerlinNoiseSmooth(_mapArray, seed, interval), tilemap, tile));
        }

        private static int[,] GenerateArray(int width, int height, bool empty)
        {
            var map = new int[width, height];
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                for (var y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (empty)
                    {
                        map[x, y] = 0;
                    }
                    else
                    {
                        map[x, y] = 1;
                    }
                }
            }
            return map;
        }

        public static void UpdateMap(int[,] map, Tilemap tilemap) //Takes in our map and tilemap, setting null tiles where needed
        {
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                for (var y = 0; y < map.GetUpperBound(1); y++)
                {
                    //We are only going to update the map, rather than rendering again
                    //This is because it uses less resources to update tiles to null
                    //As opposed to re-drawing every single tile (and collision data)
                    if (map[x, y] == 0)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }

        private static IEnumerator RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
        {
            //Clear the map (ensures we dont overlap)
            tilemap.ClearAllTiles();
            //Loop through the width of the map
            for (var x = 0; x < map.GetUpperBound(0) ; x++)
            {
                //Loop through the height of the map
                for (var y = 0; y < map.GetUpperBound(1); y++)
                {
                    // 1 = tile, 0 = no tile
                    if (map[x, y] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    }
                }
            }
            yield return new WaitForSeconds(.1f);
        }

        private static int[,] PerlinNoise(int[,] map, float seed)
        {
            //Used to reduced the position of the Perlin point
            const float reduction = 0.5f;
            //Create the Perlin
            for (var x = 0; x < map.GetUpperBound(0); x++)
            {
                var newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

                //Make sure the noise starts near the halfway point of the height
                newPoint += (map.GetUpperBound(1) / 2);
                for (var y = newPoint; y >= 0; y--)
                {
                    map[x, y] = 1;
                }
            }
            return map;
        }

        private static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
        {
            //Smooth the noise and store it in the int array
            if (interval > 1)
            {
                //Used to reduced the position of the Perlin point
                const float reduction = 0.5f;

                //Used in the smoothing process
                //The corresponding points of the smoothing. One list for x and one for y
                var noiseX = new List<int>();
                var noiseY = new List<int>();

                //Generate the noise
                for (var x = 0; x < map.GetUpperBound(0); x += interval)
                {
                    var newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                    noiseY.Add(newPoint);
                    noiseX.Add(x);
                }

                var points = noiseY.Count;
                for (var i = 1; i < points; i++)
                {
                    //Get the current position
                    var currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                    //Also get the last position
                    var lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                    //Find the difference between the two
                    Vector2 diff = currentPos - lastPos;

                    //Set up what the height change value will be
                    var heightChange = diff.y / interval;
                    //Determine the current height
                    var currHeight = (float)lastPos.y;

                    //Work our way through from the last x to the current x
                    for (var x = lastPos.x; x < currentPos.x; x++)
                    {
                        for (var y = Mathf.FloorToInt(currHeight); y > 0; y--)
                        {
                            map[x, y] = 1;
                        }

                        currHeight += heightChange;
                    }
                }
            }
            else
            {
                //Defaults to a normal Perlin gen
                map = PerlinNoise(map, seed);
            }

            return map;
        }
    }
}

using UnityEngine;

namespace NajakBoi.Scripts
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public GameObject bombPrefab;

        void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            
            var spawnPos = SmoothCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
            spawnPos.z = 0;
            SpawnProjectileAtLocation(spawnPos);
        }

        private void SpawnProjectileAtLocation(Vector3 spawnPos)
        {
            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }
}

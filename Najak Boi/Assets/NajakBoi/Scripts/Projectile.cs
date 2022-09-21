using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NajakBoi.Scripts
{
    public class Projectile : MonoBehaviour
    {
        public Transform owner;
        public float detonateDelay = 1.0f;
        public float explosionPower = 20.0f;
        public float radius;
        public LayerMask terrainLayer;
        private Tilemap _hitTilemap;
        public Rigidbody2D rb;

        public SpriteRenderer spriteRenderer;

        public static Projectile Instance;

        void Awake()
        {
            Instance = this;
            
            if(!rb)
                rb = GetComponent<Rigidbody2D>();
        }
        // Start is called before the first frame update
        private void Start()
        {
            if(spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        IEnumerator TickBomb()
        {
            while (detonateDelay > 0.0f) 
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                
            }
            
            Detonate();
        }

        private void Update()
        {
            if (SmoothCamera.Camera.orthographicSize > 15.0f)
                SmoothCamera.Camera.orthographicSize -= Time.deltaTime * 2.5f;
            if (detonateDelay > 0)
                detonateDelay -= Time.deltaTime;
        }
        private void OnCollisionEnter2D(Collision2D other)
        {
            StartCoroutine(TickBomb());
            if (other.gameObject.GetComponent<Tilemap>() != null)
                _hitTilemap = other.gameObject.GetComponent<Tilemap>();
        }

        private void Detonate()
        {
            if(_hitTilemap)
                TerrainDestroyer.Instance.DestroyTerrain(transform.position, radius, _hitTilemap);
            var hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), radius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("NajakBoi")) continue;
                
                var najak = hit.gameObject.GetComponent<NajakPlayer>();
                najak.hp -= 10;

                var heading = najak.transform.position - transform.position;
                var distance = heading.magnitude;
                var dir = heading / distance;
                
                najak.rb.AddForce(new Vector2(dir.x, dir.y) * explosionPower, ForceMode2D.Impulse);
            }
            SmoothCamera.Camera.orthographicSize = 15.0f;
            NajakBoiGameController.Instance.SwitchPlayers(owner);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

namespace NajakBoi.Scripts
{
    public class NajakPlayer : MonoBehaviour
    {
        public int bombs = 99;
        public int hp = 100;
        public float throwChargeMultiplier = 100.0f;
        public float cameraZoomOutMulti = 5.0f;
        private float _throwCharge;
        private Vector3 _dir;
        public GameObject bombPrefab;
        public Rigidbody2D rb;
        void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() || SmoothCamera.Target != transform || Projectile.Instance != null || NajakBoiGameController.switchingDeuts) return;
        
            if (Input.GetButton("Fire1"))
            {
                var clickedPos = SmoothCamera.Camera.ScreenToWorldPoint(Input.mousePosition);
                clickedPos.z = 0;

                var heading = clickedPos - transform.position;
                var distance = heading.magnitude;
                _dir = heading / distance;
                _throwCharge += Time.deltaTime;

                if (SmoothCamera.Camera.orthographicSize < 80.0f)
                    SmoothCamera.Camera.orthographicSize += Time.deltaTime * cameraZoomOutMulti;
                else
                    SmoothCamera.Camera.orthographicSize = 80.0f;

            }

            if (!Input.GetButtonUp("Fire1")) return;
            if (bombs > 0)
                ThrowBomb(_dir);
        }

        private void ThrowBomb(Vector3 dir)
        {
            var spawnPos = transform.position + _dir * 1.0f;
            var bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);
            var projectile = bomb.GetComponent<Projectile>();
            var prb = projectile.rb;
            var dirV2 = new Vector2(_dir.x, dir.y);
            
            projectile.owner = transform;
            prb.AddForce(dirV2 * (_throwCharge * throwChargeMultiplier));
        
            Debug.Log($"Bomb Thrown to {dirV2} * ({_throwCharge} * {throwChargeMultiplier} ({_throwCharge * throwChargeMultiplier})) = {dirV2 * (_throwCharge * throwChargeMultiplier)}");
            
            SmoothCamera.Target = bomb.transform;
            bombs--;
            _throwCharge = 0.0f;
        }
    
    }
}

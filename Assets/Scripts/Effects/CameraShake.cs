using System.Collections;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour
    {
        [SerializeField, HideInInspector] private new Camera camera;
        
        [SerializeField] private float amount;
        [SerializeField] private float duration;
        [SerializeField] private float period;

        private Coroutine _coroutine;
        private Vector3 _rootPosition;
        
        private void Awake()
        {
            camera = GetComponent<Camera>();
        }
        
        [ContextMenu("Start")]
        public void Shake()
        {
            Stop();
            
            _rootPosition = transform.position;
            _coroutine = StartCoroutine(ShakeCoroutine());
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (_coroutine is null)
                return;
            
            StopCoroutine(_coroutine);
            _coroutine = null;

            transform.position = _rootPosition;
        }

        private IEnumerator ShakeCoroutine()
        {
            float remaining = duration;

            while (remaining > 0)
            {
                float startTime = Time.time;
                transform.position = _rootPosition + (Vector3) (Random.insideUnitCircle * amount);

                yield return new WaitForSeconds(period);
                remaining -= Time.time - startTime;
            }

            transform.position = _rootPosition;
        }
    }
}

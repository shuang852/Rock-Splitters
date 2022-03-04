using System.Collections;
using Managers;
using ToolSystem;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(Camera))]
    public class CameraShake : MonoBehaviour
    {
        [SerializeField, HideInInspector] private new Camera camera;
        
        [SerializeField] private float hammerAmount;
        [SerializeField] private float hammerDuration;
        [SerializeField] private float hammerPeriod;
        [SerializeField] private float drillAmount;
        [SerializeField] private float drillDuration;
        [SerializeField] private float drillPeriod;

        public bool ShakeEnabled = true;
        private Coroutine _coroutine;
        private Vector3 _rootPosition;

        private ToolManager toolManager;
        
        private void Awake()
        {
            camera = GetComponent<Camera>();
            toolManager = M.GetOrThrow<ToolManager>();
            
            toolManager.toolDown.AddListener(ShakeOnce);
            toolManager.toolInUse.AddListener(ShakeContinuous);
        }
        
        // TODO: make it more generic and support any values. Derive the shake amounts elsewhere
        [ContextMenu("Start")]
        private void ShakeOnce(Vector2 pos)
        {
            if (!ShakeEnabled) return;
            
            Stop();
            
            _rootPosition = transform.position;
            _coroutine = StartCoroutine(ShakeCoroutine(hammerDuration, hammerAmount, hammerPeriod));
        }

        private void ShakeContinuous(Vector2 pos)
        {
            if (!ShakeEnabled) return;
            if (_coroutine != null)
                return;

            _rootPosition = transform.position;
            _coroutine = StartCoroutine(ShakeCoroutine(drillDuration, drillAmount, drillPeriod));
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

        private IEnumerator ShakeCoroutine(float duration, float amount, float period)
        {
            float remaining = duration;

            while (remaining > 0)
            {
                float startTime = Time.time;
                transform.position = _rootPosition + (Vector3) (Random.insideUnitCircle * amount);

                yield return new WaitForSeconds(period);
                remaining -= Time.time - startTime;
            }

            _coroutine = null;
            transform.position = _rootPosition;
        }
    }
}

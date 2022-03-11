using UI.Core;
using UnityEngine;

namespace UI.Generic
{
    public class FloatyText : DialogueComponent<Dialogue>
    {
        [SerializeField] private float amplitude;
        [SerializeField] private float wavelength;
        [SerializeField] private bool randomWaveOffset;

        private Vector2 initialPosition;
        private float waveOffset = 0;

        protected override void OnComponentStart()
        {
            base.OnComponentStart();

            initialPosition = transform.position;

            if (randomWaveOffset)
                waveOffset = Random.Range(0, wavelength);
        }

        protected override void Subscribe() { }

        protected override void Unsubscribe() { }

        private void Update()
        {
            float posOffset = amplitude * Mathf.Sin(1 / wavelength * (Time.time + waveOffset));
            
            transform.position = initialPosition + Vector2.up * posOffset ;
        }
    }
}
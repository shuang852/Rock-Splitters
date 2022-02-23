using System.Collections.Generic;
using System.Linq;
using Managers;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace RockSystem.Artefacts
{
    public class ArtefactShapeManager : Manager
    {
        public UnityEvent initialised = new UnityEvent();
        public UnityEvent<ArtefactShape, Vector2Int> artefactDamaged = new UnityEvent<ArtefactShape, Vector2Int>();
        public UnityEvent artefactExposed = new UnityEvent();

        // TODO: Try to avoid using this whereever possible, it should be switched out for a list of main artefacts
        public ArtefactShape MainArtefactShape => artefacts.FirstOrDefault();
        
        // TODO: Health and exposure can be updated to only include certain "main" artefacts
        public float Exposure => (float) artefacts.Sum(a => a.ExposedChunks) / artefacts.Sum(a => a.NumOfChunks);
        public float Health => artefacts.Sum(a => a.CurrentTotalHealth) / artefacts.Sum(a => a.MaxTotalHealth);
        
        private ChunkManager chunkManager;

        private readonly List<GameObject> artefactShapeGameObjects = new List<GameObject>();
        private readonly List<ArtefactShape> artefacts = new List<ArtefactShape>();
        
        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();

            chunkManager.damageOverflow.AddListener(OnDamageOverflow);
        }

        // TODO: Should support being passed multiple Artefacts
        public void Initialise(Artefact artefact)
        {
            Deinitialise();

            GameObject go = new GameObject(artefact.DisplayName);
            go.transform.parent = transform;
            artefactShapeGameObjects.Add(go);
            ArtefactShape artefactShape = go.AddComponent<ArtefactShape>();

            artefactShape.Initialise(artefact);
            
            RegisterArtefact(artefactShape);

            initialised.Invoke();
        }

        private void Deinitialise()
        {
            while (artefacts.Count > 0)
            {
                UnregisterArtefact(artefacts.First());
            }

            artefactShapeGameObjects.ForEach(Destroy);
        }

        private void OnDamageOverflow(Vector2Int flatPosition, float damage)
        {
            ArtefactShape artefact = GetExposedArtefactAtFlatPosition(flatPosition);

            if (artefact == null) return;
            
            artefact.DamageArtefactChunk(flatPosition, damage);
            
            artefactDamaged.Invoke(artefact, flatPosition);
        }
        
        private void RegisterArtefact(ArtefactShape artefact)
        {
            artefacts.Add(artefact);
        }

        private void UnregisterArtefact(ArtefactShape artefact)
        {
            artefacts.Remove(artefact);
        }

        public ArtefactShape GetExposedArtefactAtFlatPosition(Hexagons.OddrChunkCoord oddrChunkCoord)
        {
            Chunk chunk = chunkManager.ChunkStructure.GetOrNull(oddrChunkCoord);

            if (chunk == null)
                return artefacts.Find(f => f.IsHitAtFlatPosition(oddrChunkCoord));

            return null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Deinitialise();
            
            chunkManager.damageOverflow.RemoveListener(OnDamageOverflow);
        }

        public void CheckHealthAndExposure()
        {
            bool healthChanged = false;
            bool exposureChanged = false;
            
            foreach (var artefactShape in artefacts)
            {
                if (artefactShape.CheckHealth()) healthChanged = true;

                if (artefactShape.CheckExposure()) exposureChanged = true;
            }
            
            if (healthChanged)
            {
                // TODO: Should we be doing something here?
            }

            if (exposureChanged)
            {
                artefactExposed.Invoke();
            }
        }
    }
}
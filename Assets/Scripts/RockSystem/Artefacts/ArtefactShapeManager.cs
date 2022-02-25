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
        public UnityEvent artefactDamaged = new UnityEvent();
        public UnityEvent artefactExposed = new UnityEvent();
        public UnityEvent<ArtefactShape, Vector2Int> artefactChunkDamaged = new UnityEvent<ArtefactShape, Vector2Int>();

        // TODO: Try to avoid using this whereever possible, it should be switched out for a list of main artefacts
        public ArtefactShape MainArtefactShape => artefacts.FirstOrDefault();
        
        // TODO: Health and exposure can be updated to only include certain "main" artefacts
        public float Exposure => (float) artefacts.Sum(a => a.ExposedChunks) / artefacts.Sum(a => a.NumOfChunks);
        public float Health => artefacts.Sum(a => a.CurrentTotalHealth) / artefacts.Sum(a => a.MaxTotalHealth);

        public bool ArtefactShapesCanBeDamaged
        {
            get => artefactShapesCanBeDamaged;
            set
            {
                artefacts.ForEach(a => a.CanBeDamaged = value);
                
                artefactShapesCanBeDamaged = value;
            }
        }

        private ChunkManager chunkManager;

        private readonly List<GameObject> artefactShapeGameObjects = new List<GameObject>();
        private readonly List<ArtefactShape> artefacts = new List<ArtefactShape>();
        private bool artefactShapesCanBeDamaged = true;

        protected override void Start()
        {
            base.Start();

            chunkManager = M.GetOrThrow<ChunkManager>();
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

        private void OnArtefactShapeChunkDamaged(ChunkShape chunkShape, Vector2Int flatPosition)
        {
            artefactChunkDamaged.Invoke((ArtefactShape) chunkShape, flatPosition);
        }

        private void Deinitialise()
        {
            while (artefacts.Count > 0)
            {
                UnregisterArtefact(artefacts.First());
            }

            artefactShapeGameObjects.ForEach(Destroy);
            
            artefacts.Clear();
            artefactShapeGameObjects.Clear();
        }

        private void RegisterArtefact(ArtefactShape artefactShape)
        {
            artefacts.Add(artefactShape);
            
            chunkManager.RegisterChunkShape(artefactShape);
            
            artefactShape.chunkDamaged.AddListener(OnArtefactShapeChunkDamaged);

            artefactShape.CanBeDamaged = ArtefactShapesCanBeDamaged;
        }

        private void UnregisterArtefact(ArtefactShape artefactShape)
        {
            artefacts.Remove(artefactShape);

            chunkManager.UnregisterChunkShape(artefactShape);
            
            artefactShape.chunkDamaged.RemoveListener(OnArtefactShapeChunkDamaged);
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
                artefactDamaged.Invoke();

            if (exposureChanged)
                artefactExposed.Invoke();
        }
    }
}
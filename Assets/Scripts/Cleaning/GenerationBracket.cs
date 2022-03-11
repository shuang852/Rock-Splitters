using System.Collections.Generic;
using RockSystem.Chunks;
using Stored;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cleaning
{
    [CreateAssetMenu]
    public class GenerationBracket : ScriptableObject
    {
        public int bracketLength;
        [FormerlySerializedAs("antiquities")] public List<Artefact> artefacts;
        public List<RockShape> rockShapes;
        public List<ChunkDescription> chunkDescriptions;
        public Color rockColor;
        public int minMines;
        public int maxMines;
    }
}
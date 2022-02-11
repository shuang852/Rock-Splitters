using System.Collections.Generic;
using RockSystem.Chunks;
using Stored;
using UnityEngine;

namespace Cleaning
{
    [CreateAssetMenu]
    public class GenerationBracket : ScriptableObject
    {
        public int bracketLength;
        public List<Antiquity> antiquities;
        public List<RockShape> rockShapes;
        public List<ChunkDescription> chunkDescriptions;
        public Color rockColor;
    }
}
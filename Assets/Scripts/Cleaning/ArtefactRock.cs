using RockSystem.Chunks;
using Stored;
using UnityEngine;

namespace Cleaning
{
    public class ArtefactRock
    {
        // Artefact Properties
        public Antiquity Antiquity { get; }
        // public Vector2 Position;
        // public Vector2 Rotation;
        // public Vector2 Scale;
        
        // Rock Properties
        public RockShape RockShape { get; }
        public ChunkDescription ChunkDescription { get; }
        public Color RockColor { get; }

        public ArtefactRock(Antiquity antiquity, RockShape rockShape, ChunkDescription chunkDescription, Color rockColor)
        {
            Antiquity = antiquity;
            RockShape = rockShape;
            ChunkDescription = chunkDescription;
            RockColor = rockColor;
        }
    }
}
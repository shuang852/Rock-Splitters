using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility
{
    // TODO: Could make this an instancable class with a grid field.
    public static class Hexagons
    {
        public struct OddrChunkCoord
        {
            public OddrChunkCoord(int col, int row)
            {
                coord = new Vector2Int(col, row);
            }
            
            public Vector2Int coord;
            
            public int col => coord.x;
            public int row => coord.y;
            
            public static implicit operator OddrChunkCoord(Vector2Int v) => new OddrChunkCoord(v.x, v.y);
            public static implicit operator Vector2Int(OddrChunkCoord o) => new Vector2Int(o.col, o.row);
            
            public static implicit operator OddrChunkCoord(Vector3Int v) => new OddrChunkCoord(v.x, v.y);
            public static implicit operator Vector3Int(OddrChunkCoord o) => new Vector3Int(o.col, o.row, 0);
        }

        public struct AxialChunkCoord
        {
            public AxialChunkCoord(int q, int r)
            {
                coord = new Vector2Int(q, r);
            }
            
            public Vector2Int coord;

            public int q => coord.x;
            public int r => coord.y;
            
            public static AxialChunkCoord operator +(AxialChunkCoord a, AxialChunkCoord b) => new AxialChunkCoord(a.q + b.q, a.r + b.r);
        }

        public static IEnumerable<Vector2> GetHexagonCornerPositions(Vector2 centerWorldPosition, float radius) => new[]
        {
            new Vector2(radius, 0),
            new Vector2(-radius, 0),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 60f), radius * Mathf.Sin(Mathf.Deg2Rad * 60f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 120f), radius * Mathf.Sin(Mathf.Deg2Rad * 120f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 240f), radius * Mathf.Sin(Mathf.Deg2Rad * 240f)),
            new Vector2(radius * Mathf.Cos(Mathf.Deg2Rad * 300f), radius * Mathf.Sin(Mathf.Deg2Rad * 300f)),
        }.Select(p => p + centerWorldPosition);
        
        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given chunk.
        /// </summary>
        /// <param name="axialChunkCoord"></param>
        /// <param name="range"></param>
        /// <returns>A list of axial chunk coordinates.</returns>
        public static List<AxialChunkCoord> GetChunksInRange(AxialChunkCoord axialChunkCoord, int range)
        {
            List<AxialChunkCoord> chunks = new List<AxialChunkCoord>();

            for (int q = -range; q <= range; q++)
            {
                var lowerBound = Mathf.Max(-range, -q - range);
                var upperBound = Mathf.Min(range, -q + range);
                
                for (int r = lowerBound; r <= upperBound; r++)
                {
                    var chunkOffset = new AxialChunkCoord(q, r);
                    
                    chunks.Add(axialChunkCoord + chunkOffset);
                }
            }

            return chunks;
        }

        public static AxialChunkCoord OddrToAxial(OddrChunkCoord oddrChunkCoord)
        {
            var q = oddrChunkCoord.col - (oddrChunkCoord.row - (oddrChunkCoord.row & 1)) / 2;
            var r = oddrChunkCoord.row;
            return new AxialChunkCoord(q, r);
        }

        public static OddrChunkCoord AxialToOddr(AxialChunkCoord axialChunkCoord)
        {
            var col = axialChunkCoord.q + (axialChunkCoord.r - (axialChunkCoord.r & 1)) / 2;
            var row = axialChunkCoord.r;
            return new OddrChunkCoord(col, row);
        }

        /// <summary>
        /// Returns all the chunks inside a circle with the given centre and radius.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public static List<OddrChunkCoord> GetChunksInRadius(Grid grid, Vector2 centreWorldPosition, float radius)
        {
            var chunksInRange = GetChunksInEnclosingRange(grid, centreWorldPosition, radius);

            List<OddrChunkCoord> chunksInRadius = new List<OddrChunkCoord>();

            foreach (var chunk in chunksInRange)
            {
                Vector2 chunkWorldPosition = grid.CellToWorld(chunk);
                
                float distanceToPointer = Vector2.Distance(chunkWorldPosition, centreWorldPosition);

                if (distanceToPointer < radius)
                    chunksInRadius.Add(chunk);
            }

            return chunksInRadius;
        }

        /// <summary>
        /// Returns all the chunks in a range that encloses the given radius. Makes searching for chunks more efficient.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="centreWorldPosition"></param>
        /// <param name="radius"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        private static List<OddrChunkCoord> GetChunksInEnclosingRange(Grid grid, Vector2 centreWorldPosition, float radius)
        {
            OddrChunkCoord oddrChunkCoord = grid.WorldToCell(centreWorldPosition);

            AxialChunkCoord axialChunkCoord = OddrToAxial(oddrChunkCoord);

            // The outer radius (measured centre to corner) of the hexagon that fully encloses the given radius
            float outerHexRadius = (float)(radius * 2 / Math.Sqrt(3));
            
            int outerHexRange = Mathf.CeilToInt(outerHexRadius / grid.cellSize.x);

            List<AxialChunkCoord> axialChunksInRange = GetChunksInRange(axialChunkCoord, outerHexRange);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        /// <summary>
        /// Returns all the chunks that are no more than x adjacent moves from the given world position.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="worldPosition"></param>
        /// <param name="range"></param>
        /// <returns>A list of odd-r chunk coordinates.</returns>
        public static List<OddrChunkCoord> GetChunksInRange(Grid grid, Vector2 worldPosition, int range)
        {
            OddrChunkCoord oddrChunkCoord = grid.WorldToCell(worldPosition);

            AxialChunkCoord axialChunkCoord = OddrToAxial(oddrChunkCoord);
            
            List<AxialChunkCoord> axialChunksInRange = GetChunksInRange(axialChunkCoord, range);

            return axialChunksInRange.Select(AxialToOddr).ToList();
        }

        public static bool HexagonOverlapsCollider(Grid grid, OddrChunkCoord flatPosition, Collider2D collider)
        {
            Vector3 centerWorldPosition = grid.CellToWorld(flatPosition);
            float radius = grid.cellSize.x / 2f;
            
            var cornerPositions = GetHexagonCornerPositions(centerWorldPosition, radius)
                .Append((Vector2Int) flatPosition);

            return cornerPositions.Any(collider.OverlapPoint);
        }
    }
}
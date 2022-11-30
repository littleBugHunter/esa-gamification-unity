using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation
{
    public struct CraterEntry 
    {
        public Vector2 Position;
        public float Radius;

        public CraterEntry(Vector2 position, float radius) : this()
        {
            Position = position;
            Radius = radius;
        }
    }
}

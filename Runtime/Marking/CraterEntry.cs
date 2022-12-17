using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    public class CraterEntry 
    {
        public Vector2 Position;
        public float Radius;

        public CraterEntry(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }
        public CraterEntry Clone()
        {
            return new CraterEntry(Position, Radius);
        }
    }
}

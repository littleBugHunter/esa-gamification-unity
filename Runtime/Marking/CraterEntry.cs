/* Crater Entry in the CraterLogger
 * <author>Paul Nasdalack</author>
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    public class CraterEntry 
    {
        /// <summary>
        /// The Position in Pixel Coordinates
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// The Radius in Pixel Coordinates
        /// </summary>
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

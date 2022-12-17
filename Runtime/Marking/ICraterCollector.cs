/* A C# Component
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Marking
{
    public interface ICraterCollector 
    {
        void AddCraters(string id, CraterEntry[] entries);
    }
}

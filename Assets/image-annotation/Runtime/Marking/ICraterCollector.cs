/* Interface for an Object that accepts Data from a Crater Logger
 * <author>Paul Nasdalack</author>
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Marking
{
	/// <summary>
	/// Interface for an Object that accepts Data from a Crater Logger
	/// </summary>
	public interface ICraterCollector 
    {
        void AddCraters(string id, CraterEntry[] entries);
    }
}

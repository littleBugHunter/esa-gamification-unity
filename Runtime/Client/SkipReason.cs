using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImageAnnotation.Client
{
    public enum SkipReason
	{
		TooDark,
		TooBright, 
		ImageCorrupted, 
		TooManyCraters, 
		Other
    }
}

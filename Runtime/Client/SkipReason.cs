using System;
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

	public static class SkipReasonExtensions
	{
		public static string ToServerString(this SkipReason reason)
		{
			switch(reason)
			{
				case SkipReason.TooDark:
					return "too_dark";
				case SkipReason.TooBright:
					return "too_bright";
				case SkipReason.ImageCorrupted:
					return "image_corrupted";
				case SkipReason.TooManyCraters:
					return "too_many_craters";
				case SkipReason.Other:
					return "other";
			}
			throw new NotImplementedException($"Missing Server String for reason {reason.ToString()}");
		}
	}
}

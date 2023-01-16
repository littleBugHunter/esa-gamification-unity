/* Interface for Deletable Crater Mark Visuals
 * exposes functionality for previewing delete and deletion
 * <author>Paul Nasdalack</author>
 */

namespace ImageAnnotation.Marking.Visuals
{
	/// <summary>
	/// Interface for Deletable Crater Mark Visuals
	/// </summary>
	public interface IDeletableCraterVisual
	{
		/// <summary>
		/// Set the Visual to preview it's own deletion (by blinking or greying out)
		/// </summary>
		/// <param name="delete">Wether or not the delete state should be set</param>
		void SetPreviewDelete(bool delete);
		/// <summary>
		/// Delete the Visual.
		/// This does not delete the entry in the logger. Consider Calling CraterLogger.RemoveCrater() instead
		/// </summary>
		void Delete();
    }
}

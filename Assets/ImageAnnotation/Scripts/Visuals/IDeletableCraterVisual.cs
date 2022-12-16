/* Interface for Deletable Crater Mark Visuals
 * exposes functionality for previewing delete and deletion
 * <author>Paul Nasdalack</author>
 */

namespace ImageAnnotation
{
    public interface IDeletableCraterVisual
    {
        void SetPreviewDelete(bool delete);
        void Delete();
    }
}

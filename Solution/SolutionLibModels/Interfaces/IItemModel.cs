namespace SolutionModelsLib.Interfaces
{
    using SolutionModelsLib.Enums;
    using System.Xml.Serialization;

    /// <summary>
    /// Implements an interface for an abstract class that implements items
    /// that CANNOT have child items of their own. These items typically have
    /// just a name and an id (a file for example) but no item collections of
    /// their owm.
    /// </summary>
    public interface IItemModel : IModelBase, IXmlSerializable
    {
        #region properties
        /// <summary>
        /// Gets/Sets the Id for this item.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Gets/sets the parent item of this item.
        /// </summary>
        IItemModel Parent { get; set; }

        /// <summary>
        /// Gets/sets a technical type of this item to id the item
        /// in terms of its capabilities that are associated with this type of item.
        /// </summary>
        SolutionModelItemType ItemType { get; set; }

        /// <summary>
        /// Gets/sets a name for display in UI.
        /// </summary>
        string DisplayName { get; set; }

////        /// <summary>
////        /// Gets/sets whether the <see cref="DisplayName"/> of this treeview item
////        /// can be edit by the user or not.
////        /// </summary>
////        bool IsReadOnly { get; set; }
        #endregion properties

        #region methods
        /// <summary>
        /// Returns the string path either:
        /// 1) for the <paramref name="current"/> item or
        /// 2) for this item (if optional parameter <paramref name="current"/> is not set).
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        string GetStackPath(IItemModel current = null);
        #endregion methods
        
        bool? IsChecked { get; set; }
    }
}
namespace SolutionLib.ViewModels.Browser
{
    using SolutionLib.Interfaces;

    /// <summary>
    /// Implements a viewmodel class for the first visible item in the treeview.
    /// Normally, there is only one root in any given tree - so this class implements
    /// that one item visually representing that root (eg.: Computer item in Windows Explorer).
    /// </summary>
    internal class SolutionRootItemViewModel : Base.ItemChildrenViewModel, ISolutionRootItem
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="parent"></param>
        /// <param name="addDummyChild"></param>
        public SolutionRootItemViewModel(IItemChildren parent
                                       , string displayName
                                       , bool addDummyChild = true)
            : base(parent, Models.SolutionItemType.SolutionRootItem, addDummyChild)
        {
            SetDisplayName(displayName);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected SolutionRootItemViewModel()
            : base(null, Models.SolutionItemType.SolutionRootItem)
        {
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Rename the display item of the root item.
        /// </summary>
        /// <param name="newName"></param>
        public void RenameRootItem(string newName)
        {
            SetDisplayName(newName);
        }
        #endregion methods
        }
    }

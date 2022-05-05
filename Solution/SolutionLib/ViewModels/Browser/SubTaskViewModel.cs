namespace SolutionLib.ViewModels.Browser
{
    using SolutionLib.Interfaces;

    /// <summary>
    /// Implements a viewmodel for sub task items in a tree structured viewmodel collection.
    /// </summary>
    internal class SubTaskViewModel : Base.ItemViewModel, ISubTask
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public SubTaskViewModel(IItemChildren parent, string displayName)
            : base(parent, Models.SolutionItemType.SubTask)
        {
            SetDisplayName(displayName);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected SubTaskViewModel()
           : base(null, Models.SolutionItemType.SubTask)
        {
        }
        #endregion constructors
    }
}

namespace SolutionLib.ViewModels.Browser
{
    using SolutionLib.Interfaces;

    internal class TopTaskViewModel : Base.ItemChildrenViewModel, ITopTask
    {
        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="displayName"></param>
        /// <param name="addDummyChild"></param>
        public TopTaskViewModel(IItemChildren parent
                             , string displayName
                             , bool addDummyChild = true)
           : base(parent, Models.SolutionItemType.TopTask, addDummyChild)
        {
            SetDisplayName(displayName);
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        protected TopTaskViewModel()
           : base(null, Models.SolutionItemType.TopTask)
        {
        }
        #endregion constructors
    }
}

using System.Linq;

namespace SolutionLib.ViewModels.Browser.Base
{
    using SolutionLib.Interfaces;
    using SolutionLib.Models;
    using SolutionLib.ViewModels.Collections;
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;

    /// <summary>
    /// Implements the base functionality for all items that can in turn
    /// have <see cref="Children"/> collections (typically bound to ItemSource
    /// in Treeview or HierarchicalDataTemplate).
    /// </summary>
    internal class ItemChildrenViewModel  : ItemViewModel, IItemChildren
    {
        #region fields
        private static readonly ItemChildrenViewModel DummyChild = new ItemChildrenViewModel();

        private readonly SortableObservableDictionaryCollection _Children;
        private readonly object _itemsLock = new object();
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="itemType"></param>
        /// <param name="addDummyChild"></param>
        protected ItemChildrenViewModel(IItemChildren parent
                                      , SolutionItemType itemType
                                      , bool addDummyChild = true)
            : base(parent, itemType)
        {
            _Children = new SortableObservableDictionaryCollection();
            BindingOperations.EnableCollectionSynchronization(_Children, _itemsLock);

            ResetChildren(addDummyChild); // Lets lazy Load child items
        }

        /// <summary>
        /// Hidden class constructor can only be used to instantiate static
        /// <see cref="DummyChild"/> item.
        /// </summary>
        private ItemChildrenViewModel()
            : base()
        {
            _Children = new SortableObservableDictionaryCollection();
            BindingOperations.EnableCollectionSynchronization(_Children, _itemsLock);

            // Don't do this with true as it will
            // add a dummy child below the dummy child
            // and so forth ...
            ResetChildren(false); // Lets NOT lazy Load child items in this ctor
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets a sorted collection of child items that can
        /// be retreived from this parent item.
        /// </summary>
        public IEnumerable<IItem> Children
        {
            get
            {
                return _Children;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Finds a child item by the given key or returns null.
        /// </summary>
        /// <param name="displyName"></param>
        /// <returns></returns>
        public IItem FindChild(string displyName)
        {
            return _Children.TryGet(displyName);
        }

        /// <summary>
        /// Adding a new next child item via In-place Edit Box requires that
        /// we know whether 'New TopTask','New TopTask 1', 'New TopTask 2' ...
        /// is the next appropriate name - this method determines that name
        /// and returns it for a given type of a (to be created) child item.
        /// </summary>
        /// <param name="nextTypeTpAdd"></param>
        /// <returns></returns>
        public string SuggestNextChildName(SolutionItemType nextTypeTpAdd)
        {
            string suggestMask = null;

            switch (nextTypeTpAdd)
            {
                case SolutionItemType.SolutionRootItem:
                    suggestMask = "New Solution";
                    break;
                case SolutionItemType.SubTask:
                    suggestMask = "New SubTask";
                    break;
                case SolutionItemType.TopTask:
                    suggestMask = "New TopTask";
                    break;
                case SolutionItemType.Project:
                    suggestMask = "New Project";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nextTypeTpAdd.ToString());
            }

            var nextChild = _Children.TryGet(suggestMask);
            if (nextChild == null)
                return suggestMask;

            string suggestChild = null;
            for (int i = 1; i < _Children.Count + 100; i++)
            {
                suggestChild = string.Format("{0} {1}", suggestMask, i);

                nextChild = _Children.TryGet(suggestChild);
                if (nextChild == null)
                    break;
            }

            return suggestChild;
        }

        /// <summary>
        /// Adds a child item of type <see cref="IItem"/> to this parent
        /// which can also be typed with <see cref="IItem"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IItem AddChild(IItem item)
        {
            return AddChild(item.DisplayName, item);
        }

        /// <summary>
        /// Adds a child item with the given type
        /// (<see cref="SolutionItemType.SolutionRootItem"/> cannot be added here).
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IItem AddChild(string displayName, SolutionItemType type)
        {
            if (HasDummyChild == true)
                ResetChildren(false);

            switch (type)
            {
                case SolutionItemType.SubTask:
                    return AddChild(displayName, new SubTaskViewModel(this, displayName));

                case SolutionItemType.TopTask:
                    return AddChild(displayName, new TopTaskViewModel(this, displayName, false));

                case SolutionItemType.Project:
                    return AddChild(displayName, new ProjectViewModel(this, displayName, false));

                default:
                case SolutionItemType.SolutionRootItem:
                    // this is only constructed in the SolutionViewModel
                    throw new System.ArgumentOutOfRangeException(type.ToString());
            }
        }

        /// <summary>
        /// Removes a child item from the collection of children in this item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveChild(IItem item)
        {
            if (item == null)
                return false;

            lock (_itemsLock)
            {
                item.SetParent(null);

                var itemIsSelected = item.IsItemSelected;
                var idx = _Children.IndexOf(item);
                var removedItem = _Children.RemoveItem(item);

                if (itemIsSelected == false)
                    return removedItem;

                // Removed item was selected so lets try and select something nearby
                if (idx <= 0)
                    this.IsItemSelected = true;
                else
                    _Children[idx - 1].IsItemSelected = true;

                return removedItem;
            }
        }

        /// <summary>
        /// Renames a child item in the collection of children in this item.
        /// A re-sort and IsItemSelected should be applied after the rename such that
        /// the renamed item should re-appear at the correct position in the sorted
        /// list of items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public void RenameChild(IItem item, string newName)
        {
            if (item == null)
                return;

            lock (_itemsLock)
            {
                _Children.RenameItem(item, newName);
            }
        }

        /// <summary>
        /// Removes all children (if any) below this item.
        /// </summary>
        public void RemoveAllChild()
        {
            ResetChildren(false);
        }

        /// <summary>
        /// Sorts all items for display in a sorted fashion.
        /// </summary>
        public void SortChildren()
        {
            lock (_itemsLock)
            {
                _Children.SortItems();
            }
        }

        /// <summary>
        /// Adds another folder (child) item in the given collection of items.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        IItem IItemChildren.AddFolder(string displayName)
        {
            return AddChild(displayName, SolutionItemType.TopTask);
        }

        /// <summary>
        /// Adds another project (child) item in the given collection of items.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        IItem IItemChildren.AddProject(string displayName)
        {
            return AddChild(displayName, SolutionItemType.Project);
        }

        /// <summary>
        /// Adds another file (child) item in the given collection of items.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        IItem IItemChildren.AddFile(string displayName)
        {
            return AddChild(displayName, SolutionItemType.SubTask);
        }

        protected virtual bool HasDummyChild
        {
            get
            {
                if (this._Children.Count == 1)
                {
                    if (this._Children[0] == DummyChild)
                        return true;
                }

                return false;
            }
        }

        protected virtual void ResetChildren(bool addDummyChild = true)
        {
            lock (_itemsLock)
            {
                _Children.Clear();
            }

            if (addDummyChild == true)
            {
                lock (_itemsLock)
                {
                    _Children.Add(DummyChild);
                }
            }
        }

        /// <summary>
        /// Adds a child item of type <see cref="IItem"/> to this parent
        /// which can also be typed with <see cref="IItem"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected IItem AddChild(string key, IItem value)
        {
            try
            {
                lock (_itemsLock)
                {
                    _Children.AddItem(value);
                }
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Failed to add item key:{0} - '{1}' below {2} - '{3}'"
                    , key, value, DisplayName, this));
            }

            return value;
        }
        #endregion methods
        
        #region IsChecked

        public bool? _isChecked=false;
        private bool reentrancyCheck = false; 
        public bool? IsChecked 
            { 
                get 
                { 
                    return this._isChecked; 
                } 
                set 
                { 
                    if (this._isChecked != value) 
                    { 
                        if (reentrancyCheck) 
                            return; 
                        this.reentrancyCheck = true; 
                        this._isChecked = value; 
                        this.UpdateCheckState(); 
                        OnPropertyChanged("IsChecked"); 
                        this.reentrancyCheck = false; 
                    } 
                } 
            }

        private void UpdateCheckState() 
            { 
                // update all children: 
                if (this._Children.Count!= 0) 
                { 
                    this.UpdateChildrenCheckState(); 
                } 
                //update parent item 
                if (this._Parent != null) 
                { 
                    bool? parentIsChecked = this._Parent.DetermineCheckState(); 
                    this._Parent.IsChecked = parentIsChecked; 
 
                } 
            } 
 
            private void UpdateChildrenCheckState() 
            { 
                foreach (var item in this._Children) 
                { 
                    if (this.IsChecked != null) 
                    { 
                        item.IsChecked = this.IsChecked; 
                    } 
                } 
            } 
 
            public bool? DetermineCheckState() 
            { 
                bool allChildrenChecked = this._Children.Count(x => x.IsChecked == true) == this._Children.Count; 
                if (allChildrenChecked) 
                { 
                    return true; 
                } 
 
                bool allChildrenUnchecked = this._Children.Count(x => x.IsChecked == false) == this._Children.Count; 
                if (allChildrenUnchecked) 
                { 
                    return false; 
                } 
 
                return null; 
            } 
        
        #endregion IsChecked
    }
}

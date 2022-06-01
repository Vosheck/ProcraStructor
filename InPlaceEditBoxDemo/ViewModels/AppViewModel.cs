using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ServiceLocator;
using SolutionLib;

namespace InPlaceEditBoxDemo.ViewModels
{
    using SolutionLib.Interfaces;
    using System.Windows.Input;
    using System;
    using ExplorerLib;
    using SolutionModelsLib.Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Windows;
    using SolutionModelsLib.Interfaces;
    using SolutionModelsLib.Xml;

    /// <summary>
    /// Manages backend objects and functions for the Application.
    /// </summary>
    internal class AppViewModel : Base.BaseViewModel
    {
        #region fields
        /*private ISolution _currentTreeView;*/
        private ObservableCollection<ISolution> listOfTreeViews=new ObservableCollection<ISolution>();

        private ICommand _SaveSolutionCommand;
        private ICommand _LoadSolutionCommand;
        private ICommand _NewSolutionCommand;
        private ICommand _NextSolutionCommand;
        private ICommand _PreviousSolutionCommand;
        private bool _IsProcessing;
        private int currentTreeViewIndex;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        public AppViewModel()
        {
            _IsProcessing = false;
            listOfTreeViews.Add( SolutionLib.Factory.RootViewModel());
            LastFileAccess = UserDocDir + "\\" + "New Solution";

            // This is the Selected FilterIndex + 1 (starting at 1)
            SelectedFileExtFilterIndex = 1;
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets the root viewmodel of the <see cref="ISolution"/> TreeView.
        /// </summary>
        public ISolution Solution
        {
            get
            {
                return listOfTreeViews[((currentTreeViewIndex % listOfTreeViews.Count) + listOfTreeViews.Count)%listOfTreeViews.Count];
            }
        }

        public ObservableCollection<ISolution> ListOfTreeViews => listOfTreeViews;


        public string NextSolutionName => listOfTreeViews.Count < 2 ||IsProcessing ? "": listOfTreeViews[(((currentTreeViewIndex + 1) % listOfTreeViews.Count) + listOfTreeViews.Count)%listOfTreeViews.Count].GetRootItem().DisplayName+">";

        public string PreviousSolutionName => listOfTreeViews.Count < 2|| IsProcessing ? "":"<" + listOfTreeViews[(((currentTreeViewIndex - 1) % listOfTreeViews.Count) + listOfTreeViews.Count)%listOfTreeViews.Count].GetRootItem().DisplayName;

        /// <summary>
        /// Gets a property to determine if application is currently processing
        /// data (loading or searching for matches in the tree view) or not.
        /// </summary>
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            protected set
            {
                if (_IsProcessing != value)
                {
                    _IsProcessing = value;
                    NotifyPropertyChanged(() => IsProcessing);
                }
            }
        }

        public ICommand NewSolutionCommand
        {
            get
            {
                if (_NewSolutionCommand == null)
                {
                    _NewSolutionCommand = new Base.RelayCommand<object>(async (p) =>
                    {
                        /*var solutionRoot = p as ISolution;

                        if (solutionRoot == null)
                            return;*/

                        await NewSolutionCommand_ExecutedAsync();
                    });
                }
                return _NewSolutionCommand;
            }
        }
        public ICommand NextSolutionCommand
        {
            get
            {
                if (_NextSolutionCommand == null)
                {
                    _NextSolutionCommand = new Base.RelayCommand<object>(async (p) =>
                    {
                        /*var solutionRoot = p as ISolution;

                        if (solutionRoot == null)
                            return;*/

                        await NextSolutionCommand_ExecutedAsync();
                    });
                }

                return _NextSolutionCommand;
            }
        }
        public ICommand PreviousSolutionCommand
        {
            get
            {
                if (_PreviousSolutionCommand == null)
                {
                    _PreviousSolutionCommand = new Base.RelayCommand<object>(async (p) =>
                    {
                        /*var solutionRoot = p as ISolution;

                        if (solutionRoot == null)
                            return;*/

                        await PreviousSolutionCommand_ExecutedAsync();
                    });
                }

                return _PreviousSolutionCommand;
            }
        }

        /// <summary>
        /// Gets a command that save the current <see cref="Solution"/> to storge.
        /// </summary>
        public ICommand SaveSolutionCommand
        {
            get
            {
                if (_SaveSolutionCommand == null)
                {
                    _SaveSolutionCommand = new Base.RelayCommand<object>(async (p) =>
                    {
                        var solutionRoots = p as ObservableCollection<ISolution>;

                        if (solutionRoots == null&&solutionRoots.Count<1)
                            return;

                        await SaveSolutionCommand_ExecutedAsync(solutionRoots);
                    });
                }

                return _SaveSolutionCommand;
            }
        }

        /// <summary>
        /// Gets a command that save the current <see cref="Solution"/> to storge.
        /// </summary>
        public ICommand LoadSolutionCommand
        {
            get
            {
                if (_LoadSolutionCommand == null)
                {
                    _LoadSolutionCommand = new Base.RelayCommand<object>((p) =>
                    {
                        var solutionRoot = p as ObservableCollection<ISolution>;

                        if (solutionRoot == null&&solutionRoot.Count<1)
                            return;

                        LoadSolutionCommand_ExecutedAsync(solutionRoot);
                    });
                }

                return _LoadSolutionCommand;
            }
        }

        private string LastFileAccess { get; set; }

        private int SelectedFileExtFilterIndex { get; set; }

        /// <summary>
        /// Gets the default directory for opening the Save As ... dialog.
        /// </summary>
        private string UserDocDir => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        #endregion properties

        #region methods
        public void ResetDefaults()
        {
            listOfTreeViews[currentTreeViewIndex].ResetToDefaults();
        }

        /// <summary>
        /// Loads initial data items for the Solution demo.
        /// </summary>
        internal async Task LoadSampleDataAsync()
        {
            try
            {
                IsProcessing = true;
                await Demo.Create.ObjectsAsync(listOfTreeViews[currentTreeViewIndex],true);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void RefreshOnPropertyChangeds()
        {
            OnPropertyChanged(nameof(Solution));
            OnPropertyChanged(nameof(NextSolutionName));
            OnPropertyChanged(nameof(PreviousSolutionName));
        }

        private async Task NewSolutionCommand_ExecutedAsync()
        {
            try
            {
                IsProcessing = true;
                listOfTreeViews.Add(Factory.RootViewModel());
                currentTreeViewIndex = listOfTreeViews.Count - 1;
                await Demo.Create.ObjectsAsync(listOfTreeViews[currentTreeViewIndex],false);
            }
            finally
            {
                IsProcessing = false;
                RefreshOnPropertyChangeds();
            }
        }
        
        private async Task NextSolutionCommand_ExecutedAsync()
        {
            try
            {
                IsProcessing = true;
                currentTreeViewIndex++;
            }
            finally
            {
                IsProcessing = false;
                RefreshOnPropertyChangeds();
            }
        }
        private async Task PreviousSolutionCommand_ExecutedAsync()
        {
            try
            {
                IsProcessing = true;
                currentTreeViewIndex--;
            }
            finally
            {
                IsProcessing = false;
                RefreshOnPropertyChangeds();
            }
        }

        private async Task SaveSolutionCommand_ExecutedAsync(ObservableCollection<ISolution> solutionRoots)
        {
            IsProcessing = true;
                try
                {
                    for (int i = 0; i < solutionRoots.Count; i++)
                    {
                        string currentName = solutionRoots[i].Root.ElementAt(0).DisplayName;
                        for (int j = i+1; j < solutionRoots.Count; j++)
                        {
                            if (currentName == solutionRoots[j].Root.ElementAt(0).DisplayName)
                            {
                                MessageBox.Show("Project names can't be identical");
                                return;
                            }
                        }
                    }
                    for (int i = 0; i < solutionRoots.Count; i++)
                    {
                        SolutionModelsLib.Xml.Storage.WriteXmlToFile(solutionRoots[i].Root.ElementAt(0).DisplayName+".solxml", new ViewModelModelConverter().ToModel(solutionRoots[i]));  
                    }
                }
                finally
                {
                    IsProcessing = false;
                }

        }

        /// <summary>
        /// Method is executed to save a solutions content into the filesystem
        /// (Save As dialog should be called before this function if required
        /// This method executes after a user approved a dialog to Save in this
        /// location with this name).
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="solutionRoot"></param>
        /// <returns></returns>
        private async Task<bool> SaveSolutionFileAsync(string sourcePath
                                                     , ISolutionModel solutionRoot)
        {
            return await Task.Run<bool>(() =>
            {
                return SaveSolutionFile(sourcePath, solutionRoot);
            });
        }

        /// <summary>
        /// Method is executed to save a solutions content into the filesystem
        /// (Save As dialog should be called before this function if required
        /// This method executes after a user approved a dialog to Save in this
        /// location with this name).
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="solutionRoot"></param>
        /// <returns></returns>
        private bool SaveSolutionFile(string sourcePath
                                     ,ISolutionModel solutionRoot)
        {
            SolutionModelsLib.SQLite.SolutionDB db = new SolutionModelsLib.SQLite.SolutionDB();
            db.SetFileNameAndPath(sourcePath);

            Console.WriteLine("1) Writting data into SQLite file: '{0}'", db.DBFileNamePath);
            int recordCount = 0;
            int itemTypeCount = 0;

            try
            {
                // Overwrites the existing file (if any)
                db.OpenConnection(true);

                if (db.ConnectionState == false)
                {
                    Console.WriteLine("ERROR: Cannot open Database connectiton.\n" + db.Status);
                    return false;
                }

                db.ReCreateDBTables(db);

                // Write itemtype enumeration into file
                var names = Enum.GetNames(typeof(SolutionModelsLib.Enums.SolutionModelItemType));
                var values = Enum.GetValues(typeof(SolutionModelsLib.Enums.SolutionModelItemType));
                itemTypeCount = db.InsertItemTypeEnumeration(names, values);

                // Write solution tree data file
                recordCount = db.InsertSolutionData(solutionRoot);
            }
            catch (Exception exp)
            {
                Console.WriteLine("\n\nAN ERROR OCURRED: " + exp.Message + "\n");
            }
            finally
            {
                db.CloseConnection();
            }

            Console.WriteLine("{0:000} records written to itemtype enumeration table...", itemTypeCount);
            Console.WriteLine("{0:000} records written to solution data table...", recordCount);

            return true;
        }

         internal async Task LoadSolutionCommand_ExecutedAsync(ObservableCollection<ISolution> solutionRoots)
        {
           // ObservableCollection<ISolution> solutionRoots = solutionRoot;
           IsProcessing = true;
            string[] files=Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.solxml");
            if (files.Length < 1)
            {
                try
                {

                    await Demo.Create.ObjectsAsync(listOfTreeViews[currentTreeViewIndex],true);
                }
                finally
                {
                    IsProcessing = false;
                }
                return;
            }
            
            for (int i = 0; i < files.Length; i++)
            {
                ISolutionModel solutionModel = Storage.ReadXmlFromFile<ISolutionModel>(files[i]);
                
                if(solutionRoots.Count<files.Length)
                    solutionRoots.Add(Factory.RootViewModel());
                new ViewModelModelConverter().ToViewModel(solutionModel, solutionRoots[i]);

                var rootItem = solutionRoots[i].GetRootItem();  // Show items below root by default
                if (rootItem != null)
                    rootItem.IsItemExpanded = true;
            }

            IsProcessing = false;
            RefreshOnPropertyChangeds();
        }

        /// <summary>
        /// Method is executed to load a solutions content from the filesystem
        /// (Open file dialog should be called before this function if required).
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        private ISolutionModel LoadSolutionFile(string sourcePath, out int recordCount)
        {
            recordCount = 0;
            ISolutionModel solutionRoot = null;

            var db = new SolutionModelsLib.SQLite.SolutionDB();
            try
            {
                db.SetFileNameAndPath(sourcePath);

                db.OpenConnection();

                if (db.ConnectionState == false)
                {
                    MessageBox.Show("ERROR: Cannot open Database connectiton.\n" + db.Status);
                    return null;
                }

                solutionRoot = SolutionModelsLib.Factory.CreateSolutionModel();  // Select Result from Database

                var mapKeyToItem = db.ReadItemTypeEnum();
                bool checkResult = CompareItemTypeEnums(mapKeyToItem);

                if (checkResult == false)
                {
                    MessageBox.Show("ERROR: Cannot open file: itemtype enumeration is not consistent.");
                    return null;
                }

                recordCount = db.ReadSolutionData(solutionRoot, db);
            }
            catch (Exception exp)
            {
                MessageBox.Show("\n\nAN ERROR OCURRED: " + exp.Message + "\n");
            }
            finally
            {
                db.CloseConnection();
            }

            return solutionRoot;
        }

        /// <summary>
        /// Compares a dictionary of long, string values to a given enumeration
        /// and returns true if all values and string names of the enumeration
        /// are present in the dictionary, otherwise false.
        /// </summary>
        /// <param name="mapKeyToItem"></param>
        /// <returns></returns>
        private bool CompareItemTypeEnums(Dictionary<long, string> mapKeyToItem)
        {
            var names = Enum.GetNames(typeof(SolutionModelsLib.Enums.SolutionModelItemType));
            var values = Enum.GetValues(typeof(SolutionModelsLib.Enums.SolutionModelItemType));

            for (int i = 0; i < names.Length; i++)
            {
                string name;
                if (mapKeyToItem.TryGetValue((int)values.GetValue(i), out name) == false)
                    return false;

                if (name != names[i].ToString())
                    return false;
            }

            return true;
        }
        #endregion methods
    }
}


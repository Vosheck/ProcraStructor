namespace InPlaceEditBoxDemo.Demo
{
    using SolutionLib.Interfaces;
    using SolutionLib.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class Create
    {
        internal static Task ObjectsAsync(ISolution solutionRoot,bool firstProject)
        {
            return Task.Run(() => { Objects(solutionRoot,firstProject); });
        }

        internal static void Objects(ISolution solutionRoot,bool firstProject)
        {
            var root = solutionRoot.AddSolutionRootItem("New Project");

            root.IsItemExpanded = true;

            // Assume for this demo that the root item cannot be renamed
            // root.SetIsReadOnly(true);
            if (firstProject)
            {
                var xmlFolder = solutionRoot.AddRootChild("Enter to add a task", SolutionItemType.TopTask) as IItemChildren;
                var anotherFolder= solutionRoot.AddRootChild("Delete to remove a task", SolutionItemType.TopTask) as IItemChildren;
                
                if (xmlFolder == null)
                    throw new System.NotSupportedException();

                xmlFolder.IsItemExpanded = true;
            }

        }

        /// <summary>
        /// Creates a mockup project structure with items below it.
        /// </summary>
        /// <param name="solutionRoot"></param>
        /// <param name="project"></param>
        /// <param name="parent"></param>
        /// <param name="folders"></param>
        /// <param name="files"></param>
        private static void CreateProject(
            ISolution solutionRoot
            , string project
            , IItemChildren parent
            , List<string> folders
            , List<string> files
            )
        {
            var projectItem = solutionRoot.AddChild(project, SolutionItemType.Project, parent) as IItemChildren;

            if (projectItem == null)
                throw new System.NotImplementedException();

            var projectItemChanged = false;

            for (int i = 0; i < 13; i++)
            {
                solutionRoot.AddChild(string.Format("file_{0}", i), SolutionItemType.SubTask, projectItem);
                projectItemChanged = true;
            }


            if (folders != null)
            {
                foreach (var item in folders)
                {
                    var folder = solutionRoot.AddChild(item, SolutionItemType.TopTask, projectItem) as IItemChildren;

                    if (folder == null)
                        throw new System.NotImplementedException();

                    for (int i = 0; i < 123; i++)
                    {
                        solutionRoot.AddChild(string.Format("file_{0}",i), SolutionItemType.SubTask, folder);
                        projectItemChanged = true;
                    }
                }
            }

            if (files != null)
            {
                foreach (var item in files)
                    solutionRoot.AddChild(item,SolutionItemType.SubTask , projectItem);

                projectItemChanged = true;
            }

            parent.SortChildren();

            if (projectItemChanged == true)
                projectItem.SortChildren();
        }
    }
}

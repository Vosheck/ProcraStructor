namespace SolutionLib.Models
{
    /// <summary>
    /// Determines the typed id for an item in a solution.
    /// </summary>
    public enum SolutionItemType
    {
        /// <summary>
        /// Represents the root of the items in the solution tree.
        /// </summary>
        SolutionRootItem = 0,

        /// <summary>
        /// A generic file.
        /// </summary>
        SubTask = 100,

        /// <summary>
        /// A generic solution folder.
        /// </summary>
        TopTask = 200,

        /// <summary>
        /// A generic project.
        /// </summary>
        Project = 300
    }
}

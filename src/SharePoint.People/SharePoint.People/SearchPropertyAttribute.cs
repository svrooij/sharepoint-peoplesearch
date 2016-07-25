using System;
namespace SharePoint.People
{
    /// <summary>
    /// Use this atribute if you want 'extra' properties to be loaded from search.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SearchPropertyAttribute : Attribute
    {
        readonly string searchPropertyKey;
        readonly bool trySplitValue;

        /// <summary>
        /// Mark a property as retrievable from search. Used to define the search query and to set the properties.
        /// </summary>
        /// <param name="searchPropertyKey">Name of this property (in the search, can be found at http://centralAdminUrl:port/_admin/search/ListManagedProperties.aspx)</param>
        /// <param name="trySplitValue">Should we try to spilt this value into a array?</param>
        public SearchPropertyAttribute(string searchPropertyKey, bool trySplitValue = false)
        {
            this.searchPropertyKey = searchPropertyKey;
            this.trySplitValue = trySplitValue;

        }

        public string SearchPropertyKey
        {
            get { return searchPropertyKey; }
        }

        public bool TrySplitValue { get { return trySplitValue; } }
    }
}

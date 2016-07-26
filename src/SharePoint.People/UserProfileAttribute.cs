using System;

namespace SharePoint.People
{
    /// <summary>
    /// Use this attribute if you want properties to be loaded from the user profile.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class UserProfileAttribute : Attribute
    {
        readonly string userProfileKey;
        readonly bool trySplitValue;

        /// <summary>
        /// Mark a property as retrievable from the UserProfile
        /// </summary>
        /// <param name="userProfileKey">Key of the property in the UserProfile dictionary</param>
        /// <param name="trySplitValue">Should we split this value into an string array</param>
        public UserProfileAttribute(string userProfileKey, bool trySplitValue = false)
        {
            if (string.IsNullOrWhiteSpace(userProfileKey))
                throw new ArgumentNullException("userProfileKey", "The userProfileKey cannot be empty!");

            this.userProfileKey = userProfileKey;
            this.trySplitValue = trySplitValue;

        }

        /// <summary>
        /// Key in the UserProfile dictionary
        /// </summary>
        public string UserProfileKey
        {
            get { return userProfileKey; }
        }

        /// <summary>
        /// Is this value a string array devided by '|'
        /// </summary>
        public bool TrySplitValue { get { return trySplitValue; } }
    }
}

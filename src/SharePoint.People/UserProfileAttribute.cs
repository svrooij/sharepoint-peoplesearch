using System;

namespace SharePoint.People
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class UserProfileAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string userProfileKey;
        readonly bool trySplitValue;

        // This is a positional argument
        public UserProfileAttribute(string userProfileKey, bool trySplitValue = false)
        {
            this.userProfileKey = userProfileKey;
            this.trySplitValue = trySplitValue;

        }

        public string UserProfileKey
        {
            get { return userProfileKey; }
        }

        public bool TrySplitValue { get { return trySplitValue; } }
    }
}

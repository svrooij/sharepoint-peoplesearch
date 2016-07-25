using System;
using System.Collections.Generic;
using System.Linq;

namespace SharePoint.People
{
    internal static class ObjectCreator
    {
        internal static T CreateNewFromSearchResult<T>(IDictionary<string, object> searchResult)
        {
            T person = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SearchPropertyAttribute)));

            if (!properties.Any())
                throw new InvalidCastException("No search property attributes defined in this class");

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(SearchPropertyAttribute), false).Single() as SearchPropertyAttribute;
                if (searchResult.ContainsKey(attribute.SearchPropertyKey))
                {
                    if (attribute.TrySplitValue)
                    {
                        var value = searchResult[attribute.SearchPropertyKey] as string;
                        property.SetValue(person, value.SplitEmpty(';'));
                    }
                    else
                    {
                        property.SetValue(person, searchResult[attribute.SearchPropertyKey]);
                    }
                }
            }

            return person;
        }

        internal static T CreateNewFromUserProperties<T>(Microsoft.SharePoint.Client.UserProfiles.PersonProperties profile)
        {
            T person = Activator.CreateInstance<T>();

            var properties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(UserProfileAttribute)));

            if (!properties.Any())
                throw new InvalidCastException("No profile attributes defined in this class");

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(UserProfileAttribute), false).Single() as UserProfileAttribute;
                if (profile.UserProfileProperties.ContainsKey(attribute.UserProfileKey))
                {
                    if (attribute.TrySplitValue)
                    {
                        var value = profile.UserProfileProperties[attribute.UserProfileKey] as string;
                        property.SetValue(person, value.SplitEmpty('|'));
                    }
                    else
                    {
                        property.SetValue(person, profile.UserProfileProperties[attribute.UserProfileKey]);
                    }
                }
            }

            return person;
        }
    }
}
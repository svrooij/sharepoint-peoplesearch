using System;
using System.Collections.Generic;
using System.Linq;
/* =======================================
 * This code uses reflection to get all the properties with the defined attributes.
 * I'm open to suggestions, on how to improve this any further.
 * please email me at github@svrooij.nl
 * Stephan van Rooij
 * =======================================
 */

namespace SharePoint.People
{
    internal static class ObjectCreator
    {
        internal static T CreateNewFromSearchResult<T>(IDictionary<string, object> searchResult) where T : class, new()
        {
            // This can be used for any type, so we need the Activator.
            T person = Activator.CreateInstance<T>();

            // Get all properties with the 'SearchPropertyAttribute' defined.
            var properties = typeof(T).GetProperties().Where(property => Attribute.IsDefined(property, typeof(SearchPropertyAttribute)));
            // TODO Try to match on PropertyName.
            //  || searchResult.ContainsKey(property.Name)

            // There should be at least one property defined.
            if (!properties.Any())
                throw new InvalidCastException("No search property attributes defined in this class");

            // Loop through the properties and try to set the value on 'person'.
            foreach (var property in properties)
            {

                var attribute = property.GetCustomAttributes(typeof(SearchPropertyAttribute), false).SingleOrDefault() as SearchPropertyAttribute;
                if (searchResult.ContainsKey(attribute.SearchPropertyKey))
                {
                    // Some properties are returned as a string, but are actually a multi-value field. Like the Responsibilities.
                    if (attribute.TrySplitValue)
                    {
                        var value = searchResult[attribute.SearchPropertyKey] as string;

                        // TODO check the property type to match the value.
                        // This might crash if they don't match.
                        property.SetValue(person, value.SplitEmpty(';'));
                    }
                    else
                    {
                        // TODO check the property type to match the value.
                        property.SetValue(person, searchResult[attribute.SearchPropertyKey]);
                    }
                }
            }

            return person;
        }

        internal static T CreateNewFromUserProperties<T>(Microsoft.SharePoint.Client.UserProfiles.PersonProperties profile) where T : class, new()
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
                        // TODO check the property type to match the value.
                        property.SetValue(person, value.SplitEmpty('|'));
                    }
                    else
                    {
                        // TODO check the property type to match the value.
                        property.SetValue(person, profile.UserProfileProperties[attribute.UserProfileKey]);
                    }
                }
            }

            return person;
        }
    }
}
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
            var properties = typeof(T).GetProperties().Where(property => 
                Attribute.IsDefined(property, typeof(SearchPropertyAttribute))
                || searchResult.ContainsKey(property.Name)
            ).ToList();
            
            // There should be at least one property defined.
            if (!properties.Any())
                throw new InvalidCastException("No SearchPropertyAttributes or matching property names found in "+typeof(T).Name);

            // Loop through the properties and try to set the value on 'person'.
            foreach (var property in properties)
            {

                var attribute = property.GetCustomAttributes(typeof(SearchPropertyAttribute), false).SingleOrDefault() as SearchPropertyAttribute;
                if(attribute == null) // The attribute could not be loaded so the PropertyName should match. The attribute always has precedence!
                {
                    property.SetValue(person, searchResult[property.Name]);
                }
                else if (searchResult.ContainsKey(attribute.SearchPropertyKey))
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

            var properties = typeof(T).GetProperties().Where(property => 
                Attribute.IsDefined(property, typeof(UserProfileAttribute)) ||
                profile.UserProfileProperties.ContainsKey(property.Name)
            ).ToList();

            if (!properties.Any())
                throw new InvalidCastException("No UserProfileAttributes or matching property names found in " + typeof(T).Name);

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes(typeof(UserProfileAttribute), false).SingleOrDefault() as UserProfileAttribute;
                if (attribute == null)// The attribute could not be loaded so the PropertyName should match. The attribute always has precedence!
                {
                    property.SetValue(person, profile.UserProfileProperties[property.Name]);
                }
                else if (profile.UserProfileProperties.ContainsKey(attribute.UserProfileKey))
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.Search.Query;
using Microsoft.SharePoint.Client.UserProfiles;
namespace SharePoint.People
{
    /// <summary>
    /// A SharePoint client.
    /// </summary>
    public class SharePointClient : IDisposable
    {
        ClientContext context;

        /// <summary>
        /// Create a new SharePoint Client
        /// </summary>
        /// <param name="webFullUrl">The url to the sharepoint site.</param>
        /// <remarks>This should use the credentials of the current user, but this might not work in some situations.</remarks>
        public SharePointClient(string webFullUrl)
        {
            context = new ClientContext(webFullUrl);
        }

        /// <summary>
        /// Create a new SharePoint Client
        /// </summary>
        /// <param name="webFullUrl">The url to the sharepoint site.</param>
        /// <param name="credentials">The credentials to use to connect to SharePoint.</param>
        public SharePointClient(string webFullUrl, ICredentials credentials)
        {
            context = new ClientContext(webFullUrl)
            {
                Credentials = credentials
            };
        }

        /// <summary>
        /// Access the ClientContext in your own project.
        /// </summary>
        /// <remarks>Use with caution!!</remarks>
        public ClientContext Context
        {
            get { return context; }
        }

        /// <summary>
        /// Get all terms in the default termstore. Can be used to make an autocomplete to query users.
        /// </summary>
        public IEnumerable<Term> GetAllTerms()
        {
            var taxonomySession = TaxonomySession.GetTaxonomySession(context);
            TermStore termStore = taxonomySession.GetDefaultKeywordsTermStore();

            TermSet termSet = termStore.KeywordsTermSet;
            TermCollection terms = termSet.GetAllTerms();


            context.Load(terms);
            context.ExecuteQuery();

            return terms.ToArray();
        }

        /// <summary>
        /// Query sharepoint for people. (short for GetPeopleByQuery Person(string query..))
        /// </summary>
        /// <param name="query">Your sharepoint query string</param>
        /// <param name="rowLimit">Maximum number of results</param>
        /// <param name="loadPropertiesFromAttributes">By default not all properties are returned by the search. Set to true to load all properties with SeachPropertyAttribute defined.</param>
        /// <param name="extraProperties">Extra properties that will be loaded from sharepoint</param>
        /// <remarks>You can only return properties marked as 'Retrievable' in Central Admin => Manage service applications => Search Service => Searche schema => Managed properties</remarks>
        public IEnumerable<Person> GetPeopleByQuery(string query, int rowLimit = 250, bool loadPropertiesFromAttributes = false, params string[] extraProperties)
        {
            return GetPeopleByQuery<Person>(query, rowLimit, loadPropertiesFromAttributes,extraProperties);
        }

        /// <summary>
        /// Query sharepoint for people.
        /// </summary>
        /// <typeparam name="T">The type to which the search results will be casted to. Be sure to include at least one SearchPropertyAttribute or property with a matching name!</typeparam>
        /// <param name="query">Your sharepoint query string</param>
        /// <param name="rowLimit">Maximum number of results</param>
        /// <param name="loadPropertiesFromAttributes">By default not all properties are returned by the search. Set to true to load all properties with SeachPropertyAttribute defined.</param>
        /// <param name="extraProperties">Extra properties that will be loaded from sharepoint</param>
        /// <remarks>You can only return properties marked as 'Retrievable' in Central Admin => Manage service applications => Search Service => Searche schema => Managed properties</remarks>
        public IEnumerable<T> GetPeopleByQuery<T>(string query, int rowLimit = 250, bool loadPropertiesFromAttributes = false,params string[] extraProperties) where T : class, new()
        {
            // Create a new query, the guid is static to only return people
            var searchQuery = new KeywordQuery(context)
            {
                QueryText = query,
                SourceId = new Guid("B09A7990-05EA-4AF9-81EF-EDFAB16C4E31"),
                RowLimit = rowLimit,
                HiddenConstraints = "scope:\"People\"",
            };

            //TODO Need help selecting custom attributes. Some extra properties are loaded this way, some don't.
            if (loadPropertiesFromAttributes)
            {
                var properties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SearchPropertyAttribute))).ToList();
                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttributes(typeof(SearchPropertyAttribute), false).Single() as SearchPropertyAttribute;                    
                    searchQuery.SelectProperties.Add(attribute.SearchPropertyKey);
                }
            }

            if (extraProperties.Any())
            {
                foreach (var property in extraProperties)
                {
                    searchQuery.SelectProperties.Add(property);
                }
            }


            var searchExecutor = new SearchExecutor(context);
            ClientResult<ResultTableCollection> results = searchExecutor.ExecuteQuery(searchQuery);
            context.ExecuteQuery();
            return results.Value.FirstOrDefault().ResultRows.Select(r => ObjectCreator.CreateNewFromSearchResult<T>(r)).ToList();

        }

        /// <summary>
        /// Get a userprofile by accountName. (Short for client.GetPersonByAccount Person(accountName))
        /// </summary>
        /// <param name="accountName">The name of the account your want to load the profile for.</param>
        public Person GetPersonByAccount(string accountName)
        {
            return GetPersonByAccount<Person>(accountName);
        }

        /// <summary>
        /// Get a userprofile by accountName, with PersonProperties. (Short for client.GetPersonByAccount Person(accountName,out personProperties))
        /// </summary>
        /// <param name="accountName">The name of the account your want to load the profile for.</param>
        /// <param name="personProperties">The loaded person properties, used internally to set all properties on Person.</param>
        /// <returns></returns>
        public Person GetPersonByAccount(string accountName,out PersonProperties personProperties)
        {
            return GetPersonByAccount<Person>(accountName,out personProperties);
        }

        /// <summary>
        /// Get a userprofile
        /// </summary>
        /// <typeparam name="T">The type to which the profile will be casted to. Be sure to include at least one UserProfileAttribute or property with a matching name!</typeparam>
        /// <param name="accountName">The name of the account your want to load the profile for.</param>
        public T GetPersonByAccount<T>(string accountName) where T : class, new()
        {
            var pm = new PeopleManager(context);
            var personProperties = pm.GetPropertiesFor(accountName);
            context.Load(personProperties);
            context.ExecuteQuery();
            return ObjectCreator.CreateNewFromUserProperties<T>(personProperties);
        }

        /// <summary>
        /// Get a userprofile, with PersonProperties.
        /// </summary>
        /// <typeparam name="T">The type to which the profile will be casted to. Be sure to include at least one UserProfileAttribute or property with a matching name!</typeparam>
        /// <param name="accountName">The name of the account your want to load the profile for.</param>
        /// <param name="personProperties">The loaded person properties, used internal to set all properties on <typeparamref name="T"/></param>
        public T GetPersonByAccount<T>(string accountName,out PersonProperties personProperties) where T : class, new()
        {
            var pm = new PeopleManager(context);
            personProperties = pm.GetPropertiesFor(accountName);
            context.Load(personProperties);
            context.ExecuteQuery();
            return ObjectCreator.CreateNewFromUserProperties<T>(personProperties);
        }

        /// <summary>
        /// Dispose the SharePointClient. This will render the object in an unusable state.
        /// </summary>
        public void Dispose()
        {
            context.Dispose();
        }
    }
}

using System;
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
        /// <param name="webFullUrl">The url to the sharepoint site. To load a user profile, make sure you use the mysite url.</param>
        /// <param name="credentials">The credentials to use to connect to SharePoint.</param>
        public SharePointClient(string webFullUrl, ICredentials credentials)
        {
            context = new ClientContext(webFullUrl)
            {
                Credentials = credentials
            };
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
        /// <param name="loadExtraProperties">By default not all properties are returned by the search. Set to true to load all properties with SeachPropertyAttribute defined.</param>
        /// <param name="rowLimit">Maximum number of results</param>
        public IEnumerable<Person> GetPeopleByQuery(string query, bool loadExtraProperties = false, int rowLimit = 250)
        {
            return GetPeopleByQuery<Person>(query, loadExtraProperties, rowLimit);
        }

        /// <summary>
        /// Query sharepoint for people.
        /// </summary>
        /// <typeparam name="T">The type to which the search results will be casted to. Be sure to include at least one SearchPropertyAttribute!</typeparam>
        /// <param name="query">Your sharepoint query string</param>
        /// <param name="loadExtraProperties">By default not all properties are returned by the search. Set to true to load all properties with SeachPropertyAttribute defined.</param>
        /// <param name="rowLimit">Maximum number of results</param>
        public IEnumerable<T> GetPeopleByQuery<T>(string query, bool loadExtraProperties = false, int rowLimit = 250) where T : class, new()
        {
            // Create a new query, the guid is static to only return people
            var searchQuery = new KeywordQuery(context)
            {
                QueryText = query,
                SourceId = new Guid("B09A7990-05EA-4AF9-81EF-EDFAB16C4E31"),
                RowLimit = rowLimit
            };

            if (loadExtraProperties)
            {
                var properties = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SearchPropertyAttribute)));
                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttributes(typeof(SearchPropertyAttribute), false).Single() as SearchPropertyAttribute;
                    searchQuery.SelectProperties.Add(attribute.SearchPropertyKey);

                }
            }


            var searchExecutor = new SearchExecutor(context);
            var results = searchExecutor.ExecuteQuery(searchQuery);
            context.ExecuteQuery();
            return results.Value.FirstOrDefault().ResultRows.Select(r => ObjectCreator.CreateNewFromSearchResult<T>(r));

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
        /// Get a userprofile
        /// </summary>
        /// <typeparam name="T">The type to which the profile will be casted to. Be sure to include at least one UserProfileAttribute!</typeparam>
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
        /// Dispose the SharePointClient. This will rended the object in an unusable state.
        /// </summary>
        public void Dispose()
        {
            context.Dispose();
        }
    }
}

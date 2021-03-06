﻿using System;
using System.ComponentModel;
namespace SharePoint.People
{
    /// <summary>
    /// A person that is returned either from search or from the profile manager.
    /// </summary>
    /// <remarks>Some properties can only be returned from the user profile, and some can only be returned from the search results.</remarks>
    public class Person
    {
        /// <summary>
        /// About me field on the user profile
        /// </summary>
        [DefaultValue("")]
        public string AboutMe { get; set; }

        /// <summary>
        /// Users department
        /// </summary>
        [DefaultValue("")]
        public string Department { get; set; }

        /// <summary>
        /// The DisplayName of the user.
        /// </summary>
        [SearchProperty("PreferredName")]
        [UserProfile("PreferredName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Work Email Address of the user.
        /// </summary>
        [DefaultValue("")]
        [SearchProperty("WorkEmail")]
        [UserProfile("WorkEmail")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// FirstName of the user.
        /// </summary>
        [DefaultValue("")]
        [SearchProperty("FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// User defined interests (multi-value)
        /// </summary>
        [SearchProperty("Interests", true)]
        [UserProfile("SPS-Interests", true)]
        public string[] Interests { get; set; }

        /// <summary>
        /// The date this entry was last modified in the search engine.
        /// </summary>
        /// <remarks>This property doesn't seem to be available in the user profile database, please let us know if you've found a way.</remarks>
        [SearchProperty("LastModifiedTime")]
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// LastName of the user.
        /// </summary>
        [SearchProperty("LastName")]
        public string LastName { get; set; }

        /// <summary>
        /// The Office of the user.
        /// </summary>
        /// <remarks>By default this property is not loaded from search,  set 'loadPropertiesFromAttributes = true' if you need this property right from search.</remarks>
        [DefaultValue("")]
        [SearchProperty("OfficeNumber")]
        public string Office { get; set; }

        /// <summary>
        /// User defined past projects (multi-value)
        /// </summary>
        [SearchProperty("PastProjects", true)]
        [UserProfile("SPS-PastProjects", true)]
        public string[] PastProjects { get; set; }

        /// <summary>
        /// User defined responsibilities (multi-value)
        /// </summary>
        [SearchProperty("Responsibilities", true)]
        [UserProfile("SPS-Responsibility", true)]
        public string[] Responsibilities { get; set; }

        /// <summary>
        /// User defined skills (multi-value)
        /// </summary>
        [SearchProperty("Skills", true)]
        [UserProfile("SPS-Skills", true)]
        public string[] Skills { get; set; }

        /// <summary>
        /// JobTitle of the user
        /// </summary>
        [DefaultValue("")]
        [SearchProperty("JobTitle")]
        public string Title { get; set; }

        /// <summary>
        /// The Username of the user, can be used to query the user from the profile service.
        /// </summary>
        [SearchProperty("AccountName")]
        [UserProfile("UserName")]
        public string Username { get; set; }
        
    }
}

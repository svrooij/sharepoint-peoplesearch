namespace SharePoint.People
{
    public class Person
    {
        [UserProfile("AboutMe")]
        [SearchProperty("AboutMe")]
        public string AboutMe { get; set; }

        [UserProfile("Department")]
        [SearchProperty("Department")]
        public string Department { get; set; }

        [UserProfile("DisplayName")]
        [SearchProperty("PreferredName")]
        public string DisplayName { get; set; }

        [UserProfile("WorkEmail")]
        [SearchProperty("WorkEmail")]
        public string EmailAddress { get; set; }

        [UserProfile("SPS-Interests", true)]
        [SearchProperty("Interests", true)]
        public string[] Interests { get; set; }

        [UserProfile("SPS-PastProjects", true)]
        [SearchProperty("PastProjects", true)]
        public string[] PastProjects { get; set; }

        [UserProfile("SPS-Responsibility", true)]
        [SearchProperty("Responsibilities", true)]
        public string[] Responsibilities { get; set; }

        [UserProfile("SPS-Skills", true)]
        [SearchProperty("Skills", true)]
        public string[] Skills { get; set; }

        [UserProfile("Title")]
        [SearchProperty("JobTitle")]
        public string Title { get; set; }

        [UserProfile("UserName")]
        [SearchProperty("AccountName")]
        public string Username { get; set; }
    }
}

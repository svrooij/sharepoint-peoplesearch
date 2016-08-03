# sharepoint-peoplesearch
Use this library to query the Sharepoint search for People.

## Install this package
In the package console ``install-package SharePoint.People``.

## Sample query people by Keyword
```C#
using System.Net;
using SharePoint.People;
...

using(var client = new SharePointClient("https://your.sharepoint.url",new NetworkCredentials("username","password","domain"))){
  var people = client.GetPeopleByQuery("cool");
  // Do something with the found people.
  // array will be empty when no people are found.
}
```

## Sample get one person from UserProfile (more properties available)
```C#
using System.Net;
using SharePoint.People;
...

using(var client = new SharePointClient("https://your.sharepoint.url",new NetworkCredentials("username","password","domain"))){
  var person = client.GetPersonByAccount("domain\\username"); // Should be the same format as profile url.
  // Do something with the person.
  Console.WriteLine(person.DisplayName);
}
```

## Custom properties
Both methods support returning your own class (with custom properties).

Take a look at [Person.cs](src/SharePoint.People/Person.cs) how you can get the SharePoint properties. Setting the right values happens automatically by either the SearchPropertyAttribute the UserProfileAttribute or the property name.

This implementation is heavily based on Reflection (and can possibly be improved)! See [ObjectCreator.cs](src/SharePoint.People/ObjectCreator.cs) for more information.

```C#
using SharePoint.People;
..
public class ExtendedPerson : Person {
  // If you defined a custom property named twitter, your class could look like this.
  [SearchProperty("twitter")]
  [UserProfile("twitter")]
  public string Twitter { get; set; }
}
...
using(var client = new SharePointClient("https://your.sharepoint.url",new NetworkCredentials("username","password","domain"))){
  var person = client.GetPersonByAccount<ExtendedPerson>("domain\\username"); // Should be the same format as profile url.
  // Do something with the person.
  Console.WriteLine(person.DisplayName + " " + person.Twitter);
}

```

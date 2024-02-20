# Getting Started

DrillSergeant integrates with whichever unit testing framework you're already using and does not require any extensions to be installed for your IDE. Refer to the matrix below to determine which package you need.

| Framework | Package                 |
| --------- | ----------------------- |
| Xunit     | DrillSergeant.Xunit2    |
| NUnit     | DrillSergeant.NUnit3    |
| MSTest    | DrillSergeant.MSTest    |
| Analyzers | DrillSergeant.Analyzers |

For example, to install DrillSergeant to an existing Xunit project, simply `cd` into the folder where your test project is located and run the following command:

```sh
dotnet add package DrillSergeant.Xunit2
```

## Analyzer Support (Optional)

As of version 1.1.0, DrillSergeant now has an optional analyzers package (DrillSergeant.Analyzers) that can provide real-time static analysis of behaviors to look for common mistakes and ensure best practices are being followed. You can read more about them in the wiki section:

```sh
dotnet add package DrillSergeant.Analyzers
```

## Writing a Behavior Test

Any `public` method that returns `void` can be turned into a behavior test by adding the `[Behavior]` attribute to it.

<Notes>To make a behavior test asynchronous, simply return `async Task` instead of `void`.</Notes>

```CSharp
[Behavior]
public async Task TestLogin()
{
    var client = Given("Create client", () => new Client());
    var result = WhenAsync("Authentiate with site", () => client.Resolve().Login());
    Then("Check result", () => Assert.True((await result.Resolve()).Result));

    return Task.CompletedTask;
}
```

## Data-Driven Behavior Tests

Because DrillSergeant behavior tests are just tests, many of the features of the testing framework it integrates with will continue to work as normal. Using Xunit as an example, here's how you can feed data into a parameterized test.

```CSharp
[Behavior]
[InlineData("johndoe", "valid_password", true)]
[InlineData("johndoe", "invalid_password", false)]
public async Task TestLogin(string username, string password, bool expected)
{
    var client = Given("Create client", () => new Client(username, password));
    var result = WhenAsync("Authentiate with site", () => client.Resolve().Login());
    Then("Check result", () => Assert.Equal(expected, ((await result.Resolve()).Result));

    return Task.CompletedTask;
}
```

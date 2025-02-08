namespace EventSignupApi.Tests;
using EventSignupApi.Services.LevenShteinService;

public class UnitTest1
{
    [Theory]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("flaw", "lawn", 2)]
    [InlineData("", "", 0)]
    [InlineData("a", "", 1)]
    [InlineData("", "a", 1)]
    public void Distance_ExpectedResults(string source, string target, int expected)
    {
        var ls = new LS();
        int actual = ls.Distance(source.AsSpan(), target.AsSpan());
        Assert.Equal(expected, actual); 
    }
    
}
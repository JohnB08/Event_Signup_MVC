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
    public void DistanceRec_ExpectedResults(string source, string target, int expected)
    {
        var ls = new LS();
        int actual = ls.DistanceRec(source.AsSpan(), target.AsSpan());
        Assert.Equal(expected, actual); 
    }

    [Theory]
    [InlineData("kitten", "sitting", 3)]
    [InlineData("flaw", "lawn", 2)]
    [InlineData("", "", 0)]
    [InlineData("a", "", 1)]
    [InlineData("", "a", 1)]
    public void DistanceIter_ExpectedResults(string source, string target, int expected)
    {
        var ls = new LS();
        int actual = ls.DistanceIter(source.AsSpan(), target.AsSpan());
        Assert.Equal(expected, actual);
    }
    
}
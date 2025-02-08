using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EventSignupApi.Services.LevenShteinService;

public class LS
{
    /// <summary>
    /// Implementasjon av Wikipedia sin formel for Levenshtein distanser
    /// https://en.wikipedia.org/wiki/Levenshtein_distance
    /// En Rekursiv algoritme som gradvis jobber seg gjennom et readonlyspan av chars,
    /// for å finne Levenshein distansen mellom to source strenger. 
    /// Man kan kjøre en test via EventSignupApi.Test prosjektet.
    /// </summary>
    /// <param name="source">string a</param>
    /// <param name="target">string b</param>
    /// <returns></returns>
    public int Distance(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        // a.Length, eller som på matematisk: |a|
        int sourceLength = source.Length;

        //b.Length, eller som på matematisk: |b|
        int targetLength = target.Length;

        //Hvis |a| = 0, return |b|
        if (sourceLength == 0) return targetLength;

        //Hvis |b| = 0, return |a|
        if (targetLength == 0) return sourceLength;

        //Hvis a.head = b.head, return lev(a.tail, b.tail)
        if (source[0] == target[0])
        {
            return Distance(source[1..], target[1..]);
        }

        /* 
        Otherwise return:
            1 + minimumsverdien av: 
                Lev(a.tail, b),
                Lev(a, b.tail),
                Lev(a.tail, b.tail)
         */
        return 1 + Min(
            Distance(source[1..], target),
            Distance(source, target[1..]),
            Distance(source[1..], target[1..])
        );
    }
    //En hjelpemetode for å returnere minimumverdien av tre integers. Kan også bruke en Math.Min chain. 
    private static int Min(int a, int b, int c)
    {
        int min;
        if (b < c) min = b < a ? b : a;
        else min = c < a ? c : a;
        return min;
    }
}

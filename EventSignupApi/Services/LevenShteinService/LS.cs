

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
    public int DistanceRec(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
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
            return DistanceRec(source[1..], target[1..]);
        }

        /* 
        Otherwise return:
            1 + minimumsverdien av: 
                Lev(a.tail, b),
                Lev(a, b.tail),
                Lev(a.tail, b.tail)
         */
        return 1 + Min(
            DistanceRec(source[1..], target),
            DistanceRec(source, target[1..]),
            DistanceRec(source[1..], target[1..])
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

    /// <summary>
    /// En implementasjon av Two Matrix Row Levenshtein fra wikipedia
    /// https://en.wikipedia.org/wiki/Levenshtein_distance
    /// Lager to vektorer, itererer gjennom disse mens den sammenligner karakterer i
    /// source og target.
    /// hvis source er større en target, blir disse flippet.
    /// </summary>
    /// <param name="source">source string som skal sammenlignes med et target</param>
    /// <param name="target">target som skal sammenlignes med source</param>
    /// <returns>Levenshtein distansen mellom to strings</returns>
    public int DistanceIter(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        //Siden denne implementasjonen krever at source er mindre eller lik i lengde på target,
        //Må vi potensielt flippe disse hvis source er lengre enn target. Distansen er lik uansett.
        var accTarget = source.Length < target.Length ? source : target;
        var accSource = source.Length < target.Length ? target : source;


        //Vi initialiserer to vektorer som skal brukes for å regne ut avstanden mellom source og target.
        Span<int> v0 = stackalloc int[accTarget.Length + 1];
        Span<int> v1 = stackalloc int[accTarget.Length + 1];

        //Vi setter alle elementene i vektor 0 til index i, som skal brukes som basis for hvor mye det "koster" å appende en
        //bokstav fra en gitt posisjon i, fra target til source for å gjøre source lik target. 
        for (int i = 0; i < accTarget.Length+1; i++)
        {
            v0[i] = i;
        }



        for (int i = 0; i < accSource.Length; i++)
        {   

            //Vi setter så første element i v1 lik i +1, for å vise "kosten" for å slette et element i source for å gjøre lik target.
            v1[0] = i + 1;


            //Vi bruker så formelen for å fylle inn resten av kosten for karakter s[i] sammenlignet med t[j]
            for (int j = 0; j < accTarget.Length; j++)
            {
                int deletionCost = v0[j + 1] + 1;
                int insertCost = v1[j] + 1;
                int subCost;
                if (accSource[i] == accTarget[j])
                {
                    subCost = v0[j];
                }
                else subCost = v0[j] + 1;


                v1[j + 1] = Min(deletionCost, insertCost, subCost);
            }

            //vi swapper så verdien fra v1 til v0, for å beholde utreningen før neste step.
            v1.CopyTo(v0);      
        }
        //Etter siste swappet, er hele distansen flyttet til index = t.Length i v0, og vi kan returnere den ut. 
        return v0[accTarget.Length];
    }
}

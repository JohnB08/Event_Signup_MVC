
namespace EventSignupApi.Models.HandlerResult;

/// <summary>
/// Representerer et resultat av en operasjon som enten er en suksess med data eller en feil med en feilmelding.
/// Dette sikrer at et resultat alltid er enten en suksess eller en feil, men aldri begge.
/// Vi kan da gjøre en patternmatch på typen for å finne ut om en operasjon gikk bra eller dårlig.
/// 
/// Vi kan gjøre en if (return is HandlerResult<T>.Success s) for å se om vi kan gjøre en variabel av Type HandleResult<T> om til
/// en variabel kalt s som er av HandlerResult<T>.Success. Vi kan se for oss dette som en form for casting, men med en boolean return test. 
/// 
/// vi kan også patternmatche i en switch statement via
/// 
/// return result switch
/// {
///     HandlerResult<T>.Success s => Do something with s;
///     HandlerResult<T>.Failure f => Do something with f;
/// }
/// Der vi ser på hva type av HandlerResult<T> result er, og switcher på typen. 
/// </summary>
public abstract record HandlerResult<T>
{
    /* 
    Privat Constructor forhindrer direkte opprettelse av HandlerResult<T>.
    Dette sikrer at vi kun kan opprette objekter av typen Success<T> eller Failure.
    Vi kan enten si new HandlerResult<T>.Success, eller bruke den statiske HandlerResult<T>.Ok metoden for å lage et success objekt.
    Det motsatte for Failure.
     */
    private HandlerResult() { }

    /// <summary>
    /// Representerer et vellykket resultat med tilhørende data.
    /// </summary>
    /// <param name="Data">Den returnerte verdien når operasjonen lykkes, av Generisk type T.</param>
    public sealed record Success(T Data) : HandlerResult<T>;

    /// <summary>
    /// Representerer et mislykket resultat med en feilmelding.
    /// </summary>
    /// <param name="ErrorMessage">En beskrivelse av feilen. Enten fra en exception, eller en direkteskreven feil.</param>
    public sealed record Failure(string ErrorMessage) : HandlerResult<T>;

    /// <summary>
    /// Oppretter en suksessfull instans av HandlerResult<T> med den gitte dataen.
    /// </summary>
    /// <param name="data">Data som returneres ved suksess.</param>
    /// <returns>En success-instans av HandlerResult<T>, som garanterer for Data som et felt.</returns>
    public static HandlerResult<T> Ok(T data) => new Success(data);

    /// <summary>
    /// Oppretter en feil-instans av HandlerResult<T> med en feilmelding.
    /// </summary>
    /// <param name="message">Feilmeldingen som beskriver hva som gikk galt.</param>
    /// <returns>En error-instans av HandlerResult<T>, Data er IKKE tilgjengelig her.</returns>
    public static HandlerResult<T> Error(string message) => new Failure(message);
}

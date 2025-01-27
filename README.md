# MVC fullstack prosjekt
Dette er en del av kodehode fullstack undervisningen.
Dette er et guidet prosjekt hvor vi ser på hvordan vi kan planlegge og sette opp et mvc prosjekt i helhet.
Vi skal først planlegge og sketche ut så godt vi kan, utformingen, arkitekturen og utformingen vår så godt vi kan, før vi begynner å kode. 
Det skal vi gjøre via en kombinasjon av pseudokoding og plantegning. 
Vi skal jobbe i dette repoet sammen over flere uker for å få prosjektet på plass.


## Dag 1
 - Vi gikk gjennom hvorfor det er viktig å planlegge et stort prosjekt, og gikk gjennom i felles og laget et førsteutkast for et designdokument.
 - Vi satt opp krav til prosjektet, både front-end og back-end, så godt vi kunne.
 - Vi så på forskjellige muligheter og ønsker vi vil oppnå, og laget en generell plan for gjennomførelse.

 ## Dag 4
 - Vi så på templaten
 - Vi laget modeller basert på ERDiagrammet vårt i planen, vi endret noen av de, men husket å endre ERDiagrammet vårt for å reflektere endringene vi gjorde.
 - Vi installerte EFCore via følgende kommandolinjeoperasjoner:
    1. Vi initierte et nytt tool-manifest, slik at vi kan installere egne cli verktøy kun til dette prosjektet via `dotnet new tool-manifest`
    2. Vi instalerte entity framework cli verktøy via `dotnet tool install dotnet-ef`
    3. Vi installerte entity framework for sqlite via `dotnet add package Microsoft.EntityFrameworkCore.Sqlite`
    4. Vi installerte entity framework verktøyene via `dotnet add package Microsoft.EntityFrameworkCore.Tools`
    5. Vi installerte verktøy for å håndtere schemadesign via `dotnet add package Microsoft.EntityFrameworkCore.Design`
 - Vi laget en DatabaseContext fil, som skal holde databaseconteksten i dataminnet.
 - vi la til databasecontexten som en service i Builderen vår i Program.cs, slik at den kan brukes og injekteres inn i andre controllers og services.
 - Vi la til en enkel hjelpemetode (EnsureCreated) for å se at databasefilen vår kan bli generert av schemafilene, her skal vi skifte ut med migrering.


 ## Dag 5
 - Vi repeterte litt om hvorfor vi har modellert som vi har
 - Vi begynte å implementere controllers.
   1. Vi laget først simple endepunkter som kunne get og post til /event
   2. Vi laget et event dto objekt, som representerer tenkt data postet av en bruker.
   3. Vi traff noen problemer når vi skulle knytte relasjoner mellom en bruker og et event, kanskje vi bør lage en robust relationHandler eller relationService for å håndtere dette?
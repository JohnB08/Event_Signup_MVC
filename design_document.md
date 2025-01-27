# Event_Signup

Hva ønsker vi å oppnå?
1. Vi skal ha oversikt over events, og brukere som både eier og signer opp til events.
2. En bruker skal kunne signe opp til flere events.
3. En bruker skal kunne eie ett event. 
4. Vi skal bruke SQLite som vår database, for å holde en felles state, samt relasjoner mellom brukere og events.
5. Vi skal ha en front-end, som har forskjellig ui basert på om brukeren eier eller er signet opp til et event.

## Krav for datahåndtering.
1. Vi må kunne ta imot kryptert data fra en httpForm, så skal vi lagre brukernavn, og et hashet passord. Samt returnere en login token tilbake til bruker, slik at vi kan ha persistent login. 
    - Brukere skal kunne lage en ny sidebruker, basert på brukernavn og passord. 
    - Passordet skal aldri lagres i plaintext, og skal kun refereres til i scopet det leses av fra httpForm. 
    - Brukernavn skal også anonymiseres.
2. Vi skal kunne ha oversikt over events.
    - Navn på event.
    - Dato til event.
    - Sjanger på event.
    - lokasjon for event.
    - Maks antall deltagere på eventet. 
    - Eier av eventet.
    - Admin av eventet (ett eller flere).
    - Signups.
3. Vi skal ha oversikt over brukere, og deres relasjoner til et event. Brukere skal kunne gjøre CRUD opperasjoner på disse.
    - En bruker skal kunne lage og eie ett event. (create)
    - En bruker skal kunne lese og signe opp til et eller flere event (Read)
    - Eier av eventet skal kunne oppdatere innhold (update)
    - Eier skal kunne assigne Admins til event.
        - Admins skal kunne Endre innhold i eventet (update).
    - Eier av eventet skal kunne slette eventet (delete)
    Et event skal slette seg selv, når eventet er ferdig. 

## Utførelse og krav til moduler.
Vi ønsker å utføre arbeidet vårt via en .NET backend, en sqlite database som er vår source of truth for dataen vår, samt en front-end i html, css, javascript.

### Backend:
    
 - Vi bruker Entity Framework Core for å holde oversikt og holde databasen vår i memory, og for å gjennomføre migrasjoner og endringer av dataen vår. 
 - Vi lager modeller for all data:
    1. Brukere.
    2. Eventer.
    3. SignupRelasjoner.
    4. AdminRelasjoner.
    5. OwnerRelasjoner.
    6. BrukerDTO.
    7. EventDTO.
 - Vi bruker DTO (data transfer objects) for å flytte data fra forskjellige requests til data som matcher databasemodellene vår. 
 - Vi lager en controller for å hente ut brukerdata. Og en kontroller for å hente ut Event Data.
    - Dette for å gi oss god oversikt over, og klar separasjon mellom, de forskjellige metodene og servicene som brukes av hver.
 - Vi skal lage services som kan holde avansert business logikk.
    - Singleton service for databasen.
    - Singleton service for å håndtere logins.
 - Vi skal serve frontenden vår som en static file. 
    - Den skal være default route til back-enden vår. 
    - Den skal også være Fallback route til back-end.
 - Vi skal ha separat route til en login side, som også er levert som en static fil fra backend, basert på route. 

 ### Frontend:

 - Vi bruker html, css og javascript for å generer vår frontend.
 - Vi skal kun generere Views basert på data fra databasen vår. 
    - Vi skal ikke anta at noe eksisterer, vi skal kunne stole på dataen vi mottar. 
        - Vi skal lage modeller på dataen vi mottar fra backend, og generere views basert på det. 
        - Vi skal bruke .map og object.keys for å mappe over resultater, isteden for å indexe inn etter spesifikke nøkler (anta eksistens).
 - Vi skal ha en frontpage / hero page, som viser forskjellige ting basert på om bruker er logget inn eller ikke.
    - Hvis bruker er ikke logget inn skal hero pagen ikke vise noen data som er koblet til noen bruker. 
        - Kanskje vi kan sette events som public private?
    - Vi skal i Nav elementet vårt, vise tydlig om bruker er logget inn eller ikke. (Vise "Log Inn" / {brukernavn}).
 - Vi skal lage en separat login side, som getter login sin html, og kan Poste brukernavn og passord via en htmlform til login controlleren vår. Her skal vi og redirekte tilbake til "home" ved en suksessfull login. 
 - Vi skal følge moderne stiltyper, for oppsett av css. 
 - Vi skal følge standarder for semnatisk html.


 ## Sekvensdiagrammer:

1. Default Route til api, standard /GET:

```mermaid
sequenceDiagram
    httpRequest ->>+ ApiDefaultRoute: /GET
    ApiDefaultRoute ->>+ StaticFolder: Find index.html
    StaticFolder -->>- httpRequest: Serve Index.html
```

2. Sekvens for eksisterende login token. En skjekk som gjennomføres når vi entrer siden.

```mermaid
sequenceDiagram
    actor User
    participant View(Index)
    participant LoginController
    participant LoginService
    participant UserDatabase

    User ->>+ View(Index): "Enters Index View"
    View(Index) ->>+ LoginService: "Checks for existing token in cookies"
    LoginService ->>+ UserDatabase: "Validates token"
    UserDatabase -->>- LoginService: "Returns validation result"
    LoginService -->>- View(Index): "Sends validation status (valid/invalid)"
    alt Token is valid
        View(Index) -->> User: "Displays authenticated view"
    else Token is invalid or absent
        View(Index) ->>+ User: "Display UnAuthenticated(anonymous) view"
    end
```

3. Sekvens for login hvor token ikke eksisterer.

```mermaid
sequenceDiagram
    actor User
    participant View(Index)
    participant View(Login)
    participant LoginController
    participant LoginService
    participant UserDatabase

    User ->>+ View(Index): "Clicks Login"
    View(Index) ->>+ View(Login): "Redirects to Login View"
    View(Login) ->>+ LoginController: "Submits username and password /POST"
    LoginController ->>+ LoginService: "Triggers login action"
    LoginService ->>+ UserDatabase: "Validates credentials (hashed formData)"
    UserDatabase -->>- LoginService: "Returns validation result"
    alt Credentials are valid
        LoginService -->> LoginController: "Returns success with token"
        LoginController -->> View(Login): "Responds with success and token"
        View(Login) -->> User: "Displays success message and authenticated view"
    else Credentials are invalid
        LoginService -->>- LoginController: "Returns error"
        LoginController -->>- View(Login): "Responds with error"
        View(Login) -->>- User: "Displays error feedback"
    end
```

4. Sekvens for å lage en ny user.

```mermaid
sequenceDiagram
    actor AnonymousUser
    participant LoginController
    participant LoginService
    participant UserDatabase

    AnonymousUser ->>+ LoginController: /POST /New (formData)
    LoginController ->>+ LoginService: Validate existence of required fields, and hash.
    alt validation succeeds
        LoginService ->> UserDatabase: Insert new hashed user data into the database
        UserDatabase -->> LoginService: OK (User created)
        LoginService -->> LoginController: Success response
        LoginController -->> AnonymousUser: HTTP 201 Created (New user created)
    else validation fails
        LoginService -->>- LoginController: Validation error
        LoginController -->>- AnonymousUser: HTTP 400 Bad Request with error message
    end
```

5. Sekvens for å hente eventdata til hovedsiden når en bruker entrer defaultRoute.

```mermaid
sequenceDiagram
    actor User
    participant EventController
    participant DatabaseContext
    participant EventDatabase

    User ->>+ EventController: /GET request
    EventController ->>+ DatabaseContext: Trigger fetch for public events
    DatabaseContext ->>+ EventDatabase: Fetch events marked as public
    EventDatabase -->>- DatabaseContext: Return public events

    alt User is authenticated
        EventController ->> DatabaseContext: Trigger fetch for user-specific events
        DatabaseContext ->> EventDatabase: Fetch events where user is owner
        DatabaseContext ->> EventDatabase: Fetch events where user is admin
        DatabaseContext ->> EventDatabase: Fetch events where user is signed up
        EventDatabase -->> DatabaseContext: Return owner events
        EventDatabase -->> DatabaseContext: Return admin events
        EventDatabase -->> DatabaseContext: Return signed-up events
        DatabaseContext -->> EventController: Combine public and user-specific events
        EventController -->> User: Return JSON (DTO with public and user-specific events)
    else User is anonymous
    DatabaseContext -->>- EventController: Return public events only
        EventController -->>- User: Return JSON (DTO with public events)
    end
```

6. Sekvens for å lage en ny event.

```mermaid
sequenceDiagram
    actor User
    participant EventController
    participant EventHandler
    participant DatabaseContext
    participant EventDatabase

    User ->>+ EventController: /POST /(jsonData)
    EventController ->>+ EventHandler: Validate and construct DTO from jsonData
    alt DTO validation succeeds
        EventHandler ->> DatabaseContext: Pass DTO with User as Owner
        DatabaseContext ->> EventDatabase: Insert event into database
        DatabaseContext ->> EventDatabase: Update relation table (Owner, Event)
        EventDatabase -->> DatabaseContext: OK (Event created)
        EventDatabase -->> DatabaseContext: OK (Relation updated)
        DatabaseContext -->> EventController: Success response
        EventController -->> User: HTTP 201 Created with location of new event
    else DTO validation fails
        EventHandler -->>- EventController: Validation error
        EventController -->>- User: HTTP 400 Bad Request with error message
    end
```

7. Sekvens for å Edite et event. 
```mermaid
sequenceDiagram
    actor User
    participant EventController
    participant AuthorizationService
    participant EventHandler
    participant DatabaseContext
    participant EventDatabase

    User ->>+ EventController: /PATCH /{id} (jsonData)
    EventController ->>+ AuthorizationService: Check if user is admin or owner of event
    alt User has editing privileges
        AuthorizationService -->> EventController: Authorized
        EventController ->> EventHandler: Validate and construct DTO from jsonData
        alt DTO validation succeeds
            EventHandler ->> DatabaseContext: Pass DTO and Event ID for update
            DatabaseContext ->> EventDatabase: Update event with new data
            EventDatabase -->> DatabaseContext: OK (Event updated)
            DatabaseContext -->> EventController: Success response
            EventController -->> User: HTTP 200 OK with updated event details
        else DTO validation fails
            EventHandler -->> EventController: Validation error
            EventController -->> User: HTTP 400 Bad Request with error message
        end
    else User lacks privileges
        AuthorizationService -->>- EventController: Unauthorized
        EventController -->>- User: HTTP 403 Forbidden with error message
    end
```

8. Sekvens for å signe up til en event.

```mermaid
sequenceDiagram
    actor User
    participant EventController
    participant AuthorizationService
    participant DatabaseContext
    participant EventDatabase

    User ->>+ EventController: /POST /Signup/{id} (signup request)
    EventController ->>+ AuthorizationService: Validate if event is public or user has privileges (admin, owner, or existing participant)
    alt Event is public or user has privileges
        AuthorizationService -->> EventController: Authorized
        EventController ->> DatabaseContext: Insert user-event relation (signup action)
        DatabaseContext ->> EventDatabase: Add entry to participant relation table
        EventDatabase -->> DatabaseContext: OK (User successfully signed up)
        DatabaseContext -->> EventController: Success response
        EventController -->> User: HTTP 201 Created (Signup successful)
    else User lacks privileges or event is restricted
        AuthorizationService -->>- EventController: Unauthorized
        EventController -->>- User: HTTP 403 Forbidden (Signup not allowed)
    end
```

## Entitetsrelasjonsdiagram

```mermaid
erDiagram
    User {
        int UserId
        string UserName
        string HashPassword
        int EventId
        set SignupEvents
        set AdminEvents
    }
    Event {
        int EventId
        string EventName
        DateTime EventDate
        bool Public
        int GenreId
        int UserId
        set Signups
        set Admins
    }
    EventGenreLookupTable{
        int Id
        string Genre
    }
    User ||--o| Event: "En EventId En UserId som Owner"
    User }|--|{ Event: "Mange AdminEvents, Mange Admins"
    User }|--|{ Event: "Mange SignupEvents, Mange Signups"
    Event ||--o| EventGenreLookupTable: "En sjanger"
```

Vi lager relasjonstabeller som skal holde oversikt over hvilken bruker som har en relasjon til hvilken event.
 - User 1..n UserSignupEventRelations n..m Event
 - User 1..n UserAdminEventRelations n..m Event
 - User 1..1 UserOwnerEventRelation 1..1 Event


## Routing

Vi følger prinsippet /Area/Controller/Action/Parameter url prinsippet når det kommer til Routing.<br>
Det vil si, sekvenser knyttet til EventController har url: api.com/Event/...<br>
Sekvenser knyttet til LoginController har url: api.com/Login/... <br>
<br>

Endepunktene brukt i sekvensdiagrammet er ikke endelige, men representerer godt hva forventningene til backendfunksjonaliteten er for hvert endepunkt:
 - /GET til EventController viser tydlig at vi vil hente data for eventer knyttet til en (potensiell anonym) bruker.
 - /POST til EventController viser tydlig at vi skal Legge til et nytt event.
 - /PATCH/{id} til eventController viser tydlig at vi vil edite/Patche data til en spesifik event med id == {id}.
 - /POST til EventController sin /Signup/{id} action viser tydlig at vi vil poste en ny relation mellom en bruker som "signee" eller "subscriber" og en event. 
 - /POST til LoginController Poster en loginrequest.
 - /Post til LoginController /New poster Ny brukerdata til loginController.
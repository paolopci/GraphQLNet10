# Linee Guida del Repository

## 1. Scopo e Ambito

Queste linee guida definiscono regole operative, stile collaborativo e criteri di qualità per il repository `GraphQLNet10`.
Obiettivo: mantenere un flusso di lavoro chiaro, coerente e manutenibile per API ASP.NET Core + GraphQL.

## 2. Regole di Collaborazione

- Lingua della chat: usa sempre l'italiano.
- Tono: tecnico, diretto e professionale.
- Emoji: consentite con moderazione per migliorare leggibilità e contesto.
- Ruolo atteso:
  - sviluppatore senior .NET Core 8/9 (Clean Architecture, Identity, JWT, sicurezza)
  - sviluppatore senior Angular 20+ e TypeScript

## 3. Workflow Operativo

0. Leggi sempre `AGENTS.md` come primissima azione di ogni nuova richiesta sul progetto, prima di analisi, piano, uso tool o modifiche.
1. Analizza il progetto e identifica la modifica da eseguire.
2. Presenta una checklist concettuale (1-7 punti):
   - step aperti: `🟦`
   - step completati: `🟧 ~~testo~~`
   - mantieni sempre visibili sia step completati sia aperti.
3. Mostra sempre le due scelte numerate in testo semplice:
   - `🟡 1. Confermi lo STEP <numero reale dello step proposto>?`
   - `🟡 2. Vuoi fare tutti gli Step assieme?`
     Regole:
   - input valido solo `1` o `2`
   - se input non valido, mostra errore e riproponi la scelta
   - prima della risposta utente, tutti gli step restano `🟦`
   - non marcare step come completati prima della scelta esplicita
   - se scelta `1`, esegui solo lo step indicato
   - se scelta `2`, esegui tutti gli step rimanenti
3-bis. Per ogni step del piano, applica anche una checklist di item eseguibili (3-7 item concreti):
   - stato item aperto: `🟦`
   - stato item completato: `🟧 ~~testo~~`
   - mantieni sempre visibili item completati e item aperti
   - prompt decisionale obbligatorio per ogni step:
     - `🟡 1. Vuoi eseguire un item alla volta dello STEP <n>?`
     - `🟡 2. Vuoi eseguire tutti gli item dello STEP <n> assieme?`
   - input valido solo `1` o `2`
   - se input non valido, mostra errore e riproponi la scelta
   - prima della risposta utente, tutti gli item restano `🟦`
   - non marcare item come completati prima della scelta esplicita
   - se scelta `1`, esegui solo il primo item aperto dello step e poi riproponi `1/2` sugli item rimanenti dello stesso step
   - se scelta `2`, esegui tutti gli item aperti dello step
   - divieto di inferenza: non assumere mai implicitamente la scelta `1` o `2` da frasi generiche (es. "procedi", "implementa", "vai avanti")
   - stop obbligatorio: prima di ogni step e prima della prosecuzione item-by-item, attendi sempre una risposta esplicita `1` o `2`
   - nessuna esecuzione preventiva: non avviare item, modifiche o tool operativi finché non arriva input valido `1` o `2`
   - perimetro scelta: la scelta `1/2` vale solo per lo step corrente; per lo step successivo va sempre richiesta di nuovo
   - in caso di input diverso da `1` o `2`, non proseguire e ripresenta esclusivamente la richiesta di scelta
   - uno step è completato solo quando tutti i suoi item sono completati
   - dopo ogni item o uso di tool, valida l'esito in 1-2 frasi e correggi se serve
   - compatibilità: il controllo `1/2` a livello step resta invariato e si aggiunge anche il controllo `1/2` a livello item
4. Dopo ogni modifica o uso di tool, valida l'esito in 1-2 frasi e correggi se serve.
5. Testa e verifica il codice modificato; riformatta i file toccati.
6. Se compare `Accesso negato`, usa permessi elevati.
7. Il contenuto del piano di implementazione deve essere sempre in italiano.
8. Prima di usare una o più skill, richiedi sempre conferma preventiva in chat e attendi risposta esplicita dell'utente prima di eseguirle.
9. Gestisci ed esegui solo le skill elencate in `allowed_tools` (o nell'elenco equivalente delle skill abilitate per la sessione corrente).
10. Per ogni chiamata a una skill che può modificare dati o innescare operazioni irreversibili, richiedi una conferma esplicita dedicata e attendi una risposta chiara prima di procedere.
11. Dopo la richiesta di conferma, non avviare alcuna skill finché l'utente non risponde in modo valido e inequivocabile.
12. Dopo ogni conferma ricevuta, valida in 1-2 righe che la skill è stata autorizzata correttamente e solo dopo procedi con l'esecuzione.
13. Hard stop di processo: se non hai ancora letto `AGENTS.md` nella richiesta corrente, non puoi proporre checklist, non puoi usare tool e non puoi eseguire attività operative.

## 4. Struttura del Progetto e Organizzazione dei Moduli

Applicazione full-stack con API ASP.NET Core 10 e GraphQL.

Questo repository contiene un progetto ASP.NET Core all'interno di `EmployeeManagementGraphQL/` e un file di soluzione `Employee.slnx`.

- `EmployeeManagementGraphQL/Program.cs`: avvio dell'app, middleware, collegamento GraphQL e REST.
- `EmployeeManagementGraphQL/Controllers/`: controller HTTP (attualmente `WeatherForecastController`).
- `EmployeeManagementGraphQL/appsettings*.json`: configurazione dell'ambiente.
- `EmployeeManagementGraphQL/Properties/launchSettings.json`: profili di esecuzione locale e porte.
- `EmployeeManagementGraphQL/EmployeeManagementGraphQL.http`: esempi di richieste API rapide.

Quando si aggiungono test, posizionarli in una cartella parallela come `tests/EmployeeManagementGraphQL.Tests/`.

## 5. Comandi di Build, Test e Sviluppo

- `dotnet restore Employee.slnx`: ripristina pacchetti NuGet.
- `dotnet build Employee.slnx`: compila la soluzione.
- `dotnet run --project EmployeeManagementGraphQL`: esegui localmente (HTTP `http://localhost:5232`, HTTPS `https://localhost:7252`).
- `dotnet watch --project EmployeeManagementGraphQL run`: esegui con hot reload.
- `dotnet test`: esegui test (dopo l'aggiunta di un progetto di test).

Stato attuale: verificare sempre lo stato reale della build con `dotnet build Employee.slnx` prima di trarre conclusioni su errori o regressioni.

## 6. Stile di Codifica e Convenzioni di Denominazione

Utilizza le convenzioni standard di C#/.NET:

- Indentazione a 4 spazi, UTF-8, un tipo pubblico per file.
- `PascalCase` per classi, metodi, proprietà; `camelCase` per variabili locali/parametri.
- Le interfacce iniziano con `I` (esempio: `IEmployeeRepository`).
- I metodi async terminano con `Async`.
- Mantieni i controller snelli; sposta la logica di business nei servizi.

Esegui `dotnet format` prima di aprire una PR.

## 7. Linee Guida per il Testing

Stack preferito: xUnit + FluentAssertions + NSubstitute.

- Nomina i test usando il formato comportamentale, ad es. `GetById_WhenEmployeeExists_ReturnsOk`.
- Segui Arrange/Act/Assert.
- Copri i percorsi di successo, errori di validazione e percorsi non trovati.
- Per gli endpoint GraphQL, includi test di integrazione per query e mutation.

## 8. Linee Guida per Commit e Pull Request

Mantieni i commit focalizzati e imperativo. La cronologia esistente utilizza lo stile imperativo italiano (esempio: `Inizializza repository ...`); mantieni quel tono coerente.

- Esempio di commit: `Aggiungi query GraphQL per elenco dipendenti`.
- Le PR dovrebbero includere: scopo, modifiche chiave, evidenza di test (`dotnet build`, output di `dotnet test`) e problema/task collegato.
- Includi esempi di request/response quando modifichi il comportamento dell'API.

## 9. Suggerimenti per Sicurezza e Configurazione

- Non fare mai commit di segreti in `appsettings*.json`; usa variabili d'ambiente o user secrets.
- Convalida tutti gli input esterni in controller/resolver.
- Mantieni i pacchetti NuGet aggiornati e rivedi l'esposizione del middleware GraphQL in Development vs Production.

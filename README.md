# Ricerca Treni

Applicazione web che cerca soluzioni di viaggio ferroviario su una data o un intervallo di date,
con prezzi, offerte e filtri (solo Frecce, solo regionali, diretti, Super Economy, prezzo massimo).

È il porting in **ASP.NET Core 8 (Razor Pages)** dell'omonimo progetto storico WebForms:
usa le API non ufficiali di lefrecce (la BFF `Channels.Website.BFF.WEB` che alimenta il sito di Trenitalia),
quindi è a scopo didattico e informativo, non collegato al servizio ufficiale.

## Struttura

- `RicercaTreni/` — applicazione .NET 8
- `RicercaTreni.Tests/` — test xunit (paginazione, filtri, deserializzazione)
- `legacy/` — progetti WebForms storici, conservati per memoria
  - `legacy/RicercaTreni/` — versione originale (API `msite`, chiuse nel 2022)
  - `legacy/RicercaTreniBff/` — versione 2022 aggiornata alla BFF attuale

## Storia

Nella prima versione ho preso spunto dalla wiki messa a disposizione da
[SimoDax](https://github.com/SimoDax) presente a questo
[link](https://github.com/SimoDax/Trenitalia-API/wiki/API-Trenitalia---lefrecce.it#dettaglio-prezzi---standard).

Nella successiva versione ho sperimentato le API fino a riuscire ad utilizzarle per effettuare delle chiamate.
Allego direttamente la wiki di [SimoDax](https://github.com/SimoDax) disponibile in questa
[pagina](https://github.com/SimoDax/Trenitalia-API/wiki/Nuove-API-Trenitalia-lefrecce.it),
perché è molto dettagliata come la precedente e potrebbe essere utile a chi vuole utilizzare queste API.

In questa pagina l'utente può effettuare queste operazioni:
- Indicare la stazione di partenza
- Indicare la stazione di arrivo
- Indicare un intervallo di date in modo da visualizzare tutte le soluzioni disponibili

Effettuata la ricerca vengono mostrati i risultati con la possibilità di visualizzare tutte le possibilità di acquisto per quella soluzione.

## Disclaimer

I contenuti di questa pagina sono a solo scopo informato. Ho voluto sperimentare l'implementazione di queste API a scopo didattico.

## Sviluppo

```
dotnet run --project RicercaTreni/RicercaTreni.csproj
```

L'applicazione risponde su `https://localhost:5003` in sviluppo.

## Deploy

Il workflow `.github/workflows/deploy.yml` pubblica l'applicazione e la carica via FTP
sull'hosting ServerPlan (sottodominio dedicato). I dati di connessione sono nei secret della repo.
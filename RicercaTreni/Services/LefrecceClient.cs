using System.Net.Http.Json;
using System.Text.Json;
using RicercaTreni.Models;

namespace RicercaTreni.Services;

/// <summary>Parametri di una ricerca su un giorno specifico.</summary>
public record TrattaRicerca(
    Stazione Partenza,
    Stazione Arrivo,
    DateTime Giorno,
    int Adulti,
    int Bambini,
    bool SoloFrecce,
    bool SoloRegionali,
    bool SoloDiretti);

/// <summary>
/// Client della BFF non ufficiale di lefrecce, le stesse API che usa il sito di Trenitalia.
/// Porting del vecchio TreniHelper e di TrattaTreniController del portfolio storico.
/// </summary>
public class LefrecceClient
{
    private const string BaseBff = "Channels.Website.BFF.WEB/website";

    // La BFF restituisce le soluzioni a blocchi: il vecchio sito paginava da 10
    private const int DimensionePagina = 10;

    // Paracadute: senza limite un loop di pagine rotte girerebbe all'infinito
    private const int MassimoPagine = 20;

    private static readonly JsonSerializerOptions OpzioniJson = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http;

    public LefrecceClient(HttpClient http) => _http = http;

    /// <summary>Cerca le stazioni il cui nome contiene il testo indicato.</summary>
    public async Task<List<Stazione>> CercaStazioniAsync(string nome)
    {
        using var risposta = await _http.GetAsync($"{BaseBff}/locations/search?name={Uri.EscapeDataString(nome)}&limit=100");
        risposta.EnsureSuccessStatusCode();
        return await risposta.Content.ReadFromJsonAsync<List<Stazione>>(OpzioniJson) ?? [];
    }

    /// <summary>Restituisce la stazione col nome esatto (a meno di maiuscole), se la ricerca la trova.</summary>
    public async Task<Stazione?> TrovaStazioneAsync(string nome)
    {
        var stazioni = await CercaStazioniAsync(nome);
        return stazioni.FirstOrDefault(s =>
            string.Equals(s.name?.Trim(), nome.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Cerca tutte le soluzioni di un giorno, a blocchi come faceva il vecchio sito.</summary>
    public async Task<List<Soluzione>> CercaSoluzioniAsync(TrattaRicerca tratta)
    {
        var soluzioni = new List<Soluzione>();

        for (var pagina = 0; pagina < MassimoPagine; pagina++)
        {
            var richiesta = new RicercaSoluzioni
            {
                departureLocationId = tratta.Partenza.id,
                arrivalLocationId = tratta.Arrivo.id,
                departureTime = tratta.Giorno.ToString("yyyy'-'MM'-'dd'T'00':'00':'ss"),
                adults = tratta.Adulti,
                children = tratta.Bambini,
                criteria = new Criteri
                {
                    frecceOnly = tratta.SoloFrecce,
                    regionalOnly = tratta.SoloRegionali,
                    noChanges = tratta.SoloDiretti,
                    order = "DEPARTURE_DATE",
                    offset = pagina * DimensionePagina,
                    limit = DimensionePagina
                },
                advancedSearchRequest = new RicercaAvanzata { bestFare = false }
            };

            using var risposta = await _http.PostAsJsonAsync($"{BaseBff}/ticket/solutions", richiesta, OpzioniJson);
            risposta.EnsureSuccessStatusCode();
            var risultato = await risposta.Content.ReadFromJsonAsync<RispostaSoluzioni>(OpzioniJson);

            var blocco = risultato?.solutions;
            if (blocco is not { Count: > 0 })
                break;

            soluzioni.AddRange(blocco);

            if (blocco.Count < DimensionePagina)
                break;
        }

        return soluzioni;
    }

    /// <summary>Filtri applicati dopo la ricerca: solo Super Economy e prezzo massimo.</summary>
    public static List<Soluzione> FiltraSoluzioni(List<Soluzione> soluzioni, bool soloSuperEconomy, double? prezzoMassimo)
    {
        IEnumerable<Soluzione> risultato = soluzioni;

        if (soloSuperEconomy)
            risultato = risultato.Where(EOffertaSuperEconomy);

        if (prezzoMassimo is > 0)
            risultato = risultato.Where(s => s.solution?.price?.amount <= prezzoMassimo);

        return risultato.ToList();
    }

    /// <summary>Vero se tra i servizi della soluzione compare un'offerta Super Economy.</summary>
    public static bool EOffertaSuperEconomy(Soluzione soluzione) =>
        soluzione.tooltip?.nodeServices?.Any(n => n?.offer == "Super Economy") == true;
}
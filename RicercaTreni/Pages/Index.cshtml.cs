using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RicercaTreni.Models;
using RicercaTreni.Services;

namespace RicercaTreni.Pages;

public class IndexModel : PageModel
{
    // La ricerca su un intervallo fa una chiamata lefrecce per ogni giorno:
    // limito l'intervallo per non martellare le API dall'hosting condiviso.
    private const int MassimoGiorni = 14;

    private readonly LefrecceClient _lefrecce;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(LefrecceClient lefrecce, ILogger<IndexModel> logger)
    {
        _lefrecce = lefrecce;
        _logger = logger;
    }

    [BindProperty]
    public RicercaForm Form { get; set; } = new();

    public List<Soluzione> Soluzioni { get; private set; } = [];

    public string? Messaggio { get; private set; }

    public void OnGet()
    {
    }

    /// <summary>Autocomplete delle stazioni: restituisce i nomi alle datalist del form.</summary>
    public async Task<IActionResult> OnGetStazioniAsync(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 2)
            return new JsonResult(Array.Empty<string>());

        try
        {
            var stazioni = await _lefrecce.CercaStazioniAsync(nome.Trim());
            return new JsonResult(stazioni.Select(s => s.name).ToList());
        }
        catch (Exception exc)
        {
            // L'elenco resta quello che era: l'autocomplete non deve mai rompere la pagina
            _logger.LogWarning(exc, "Autocomplete stazioni non riuscito per {Nome}", nome);
            return new JsonResult(Array.Empty<string>());
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!DateOnly.TryParseExact(Form.DataInizio, "yyyy-MM-dd", out var dataInizio))
        {
            Messaggio = "Impostare la data di inizio.";
            return Page();
        }

        DateOnly? dataFine = null;
        if (!string.IsNullOrWhiteSpace(Form.DataFine))
        {
            if (!DateOnly.TryParseExact(Form.DataFine, "yyyy-MM-dd", out var fine))
            {
                Messaggio = "La data di fine non è valida.";
                return Page();
            }
            dataFine = fine;
        }

        if (Form.Adulti < 1)
        {
            Messaggio = "Impostare almeno un adulto per avviare la ricerca.";
            return Page();
        }

        if (dataFine is { } fineData && fineData < dataInizio)
        {
            Messaggio = "La data di fine è precedente a quella di inizio.";
            return Page();
        }

        if (dataFine is { } ultimaData && ultimaData.DayNumber - dataInizio.DayNumber + 1 > MassimoGiorni)
        {
            Messaggio = $"L'intervallo massimo di ricerca è di {MassimoGiorni} giorni.";
            return Page();
        }

        Stazione? partenza;
        Stazione? arrivo;
        try
        {
            partenza = await _lefrecce.TrovaStazioneAsync(Form.StazionePartenza);
            arrivo = await _lefrecce.TrovaStazioneAsync(Form.StazioneArrivo);
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Ricerca stazioni non riuscita");
            Messaggio = "Il servizio di ricerca stazioni non risponde, riprovare tra poco.";
            return Page();
        }

        if (partenza is null)
        {
            Messaggio = "La stazione di partenza non è corretta.";
            return Page();
        }

        if (arrivo is null)
        {
            Messaggio = "La stazione di arrivo non è corretta.";
            return Page();
        }

        var tratta = new TrattaRicerca(partenza, arrivo, default, Form.Adulti, Form.Bambini,
            Form.SoloFrecce, Form.SoloRegionali, Form.SoloDiretti);

        var soluzioni = new List<Soluzione>();
        try
        {
            var giorni = dataFine is null
                ? [dataInizio]
                : Enumerable.Range(0, dataFine.Value.DayNumber - dataInizio.DayNumber + 1)
                    .Select(offset => dataInizio.AddDays(offset));

            foreach (var giorno in giorni)
            {
                var delGiorno = await _lefrecce.CercaSoluzioniAsync(
                    tratta with { Giorno = giorno.ToDateTime(TimeOnly.MinValue) });
                soluzioni.AddRange(delGiorno);
            }
        }
        catch (Exception exc)
        {
            _logger.LogWarning(exc, "Ricerca soluzioni non riuscita");
            Messaggio = "Il servizio di Trenitalia non risponde, riprovare tra poco.";
            return Page();
        }

        soluzioni = LefrecceClient.FiltraSoluzioni(soluzioni, Form.SoloSuperEconomy, Form.PrezzoMassimo);

        Soluzioni = soluzioni;
        Messaggio = soluzioni.Count > 0
            ? $"Ci sono {soluzioni.Count} risultati."
            : "Nessun risultato trovato.";

        return Page();
    }
}
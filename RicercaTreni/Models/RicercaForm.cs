namespace RicercaTreni.Models;

/// <summary>Dati del form di ricerca.</summary>
public class RicercaForm
{
    public string StazionePartenza { get; set; } = "";
    public string StazioneArrivo { get; set; } = "";
    public int Adulti { get; set; } = 1;
    public int Bambini { get; set; }

    /// <summary>Data di inizio nel formato del campo date (yyyy-MM-dd).</summary>
    public string DataInizio { get; set; } = "";

    /// <summary>Data di fine facoltativa: impostarla cerca su tutto l'intervallo.</summary>
    public string DataFine { get; set; } = "";

    public bool SoloSuperEconomy { get; set; }
    public bool SoloFrecce { get; set; }
    public bool SoloRegionali { get; set; }
    public bool SoloDiretti { get; set; }
    public double? PrezzoMassimo { get; set; }
}
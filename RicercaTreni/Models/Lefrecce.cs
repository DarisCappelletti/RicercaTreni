namespace RicercaTreni.Models;

// ============================================================
// Modelli della BFF non ufficiale di lefrecce
// (https://www.lefrecce.it/Channels.Website.BFF.WEB/website)
// I nomi delle proprietà sono quelli del JSON: servono così
// alla serializzazione/deserializzazione.
// ============================================================

/// <summary>Stazione restituita dalla ricerca lefrecce.</summary>
public class Stazione
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string displayName { get; set; } = "";
    public string timezone { get; set; } = "";
    public bool multistation { get; set; }
}

/// <summary>Corpo della POST di ricerca soluzioni.</summary>
public class RicercaSoluzioni
{
    public int departureLocationId { get; set; }
    public int arrivalLocationId { get; set; }
    public string departureTime { get; set; } = "";
    public int adults { get; set; }
    public int children { get; set; }
    public Criteri? criteria { get; set; }
    public RicercaAvanzata? advancedSearchRequest { get; set; }
}

/// <summary>Criteri di ricerca: filtri, ordinamento e paginazione.</summary>
public class Criteri
{
    public bool frecceOnly { get; set; }
    public bool regionalOnly { get; set; }
    public bool noChanges { get; set; }
    public string order { get; set; } = "DEPARTURE_DATE";
    public int offset { get; set; }
    public int? limit { get; set; }
}

public class RicercaAvanzata
{
    public bool bestFare { get; set; }
}

/// <summary>Risposta della POST di ricerca soluzioni.</summary>
public class RispostaSoluzioni
{
    public string? searchId { get; set; }
    public string? cartId { get; set; }
    public List<Soluzione>? solutions { get; set; }
}

/// <summary>Una soluzione di viaggio trovata.</summary>
public class Soluzione
{
    public SoluzioneDettaglio? solution { get; set; }
    public List<Griglia>? grids { get; set; }
    public TooltipSoluzione? tooltip { get; set; }
}

/// <summary>Dati principali della soluzione: tratta, orari, durata e prezzo minimo.</summary>
public class SoluzioneDettaglio
{
    public string? id { get; set; }
    public string? origin { get; set; }
    public string? destination { get; set; }
    public DateTime departureTime { get; set; }
    public DateTime arrivalTime { get; set; }
    public string? duration { get; set; }
    public List<Treno>? trains { get; set; }
    public Prezzo? price { get; set; }
}

/// <summary>Prezzo con valuta.</summary>
public class Prezzo
{
    public string? currency { get; set; }
    public double? amount { get; set; }
}

public class Treno
{
    public string? trainCategory { get; set; }
    public string? acronym { get; set; }
    public string? denomination { get; set; }
    public string? name { get; set; }
}

/// <summary>Tratta elementare della soluzione (un treno che collega due nodi).</summary>
public class NodoTratta
{
    public string? id { get; set; }
    public string? origin { get; set; }
    public string? destination { get; set; }
    public DateTime departureTime { get; set; }
    public DateTime arrivalTime { get; set; }
    public Treno? train { get; set; }
}

/// <summary>Griglia di una soluzione: riepiloghi dei treni e offerte di vendita.</summary>
public class Griglia
{
    public string? id { get; set; }
    public List<Riepilogo>? summaries { get; set; }
    public List<Servizio>? services { get; set; }
}

/// <summary>Riepilogo di un treno della soluzione.</summary>
public class Riepilogo
{
    public string? name { get; set; }
    public string? departureLocationName { get; set; }
    public string? arrivalLocationName { get; set; }
    public DateTime departureTime { get; set; }
    public DateTime arrivalTime { get; set; }
    public string? duration { get; set; }
    public Treno? trainInfo { get; set; }
}

/// <summary>Servizio di vendita (es. Standard, Business) con le sue offerte.</summary>
public class Servizio
{
    public string? name { get; set; }
    public List<Offerta>? offers { get; set; }
}

/// <summary>Offerta di vendita con prezzo e disponibilità.</summary>
public class Offerta
{
    public int offerId { get; set; }
    public string? serviceName { get; set; }
    public string? name { get; set; }
    public string? description { get; set; }
    public Prezzo? price { get; set; }
    public int availableAmount { get; set; }
}

/// <summary>Tooltip della soluzione: offerte per nodo (es. Super Economy).</summary>
public class TooltipSoluzione
{
    public List<NodoServizio>? nodeServices { get; set; }
}

public class NodoServizio
{
    public string? denomination { get; set; }
    public string? name { get; set; }
    public string? offer { get; set; }
    public string? service { get; set; }
}
using System.Net;
using System.Text;
using System.Text.Json;
using RicercaTreni.Models;
using RicercaTreni.Services;

namespace RicercaTreni.Tests;

/// <summary>Handler HTTP finto: registra le chiamate e risponde con i JSON preparati.</summary>
internal class HttpHandlerFinto : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _rispondi;

    public List<string> UrlChiamate { get; } = [];

    public HttpHandlerFinto(Func<HttpRequestMessage, HttpResponseMessage> rispondi) => _rispondi = rispondi;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage richiesta, CancellationToken token)
    {
        UrlChiamate.Add(richiesta.RequestUri!.ToString());
        return Task.FromResult(_rispondi(richiesta));
    }
}

public class CoreTests
{
    private static LefrecceClient ClientCon(HttpHandlerFinto handler) =>
        new(new HttpClient(handler) { BaseAddress = new Uri("https://www.lefrecce.it/") });

    private static StringContent Json(string corpo) => new(corpo, Encoding.UTF8, "application/json");

    /// <summary>JSON di una risposta con n soluzioni identiche, con offerta e prezzo a piacere.</summary>
    private static string JsonSoluzioni(int quanti, string? offerta = null, double prezzo = 29.90)
    {
        var soluzioni = Enumerable.Range(0, quanti).Select(i => new
        {
            solution = new
            {
                id = $"s{i}",
                origin = "MILANO CENTRALE",
                destination = "ROMA TERMINI",
                departureTime = "2026-10-20T10:00:00",
                arrivalTime = "2026-10-20T12:30:00",
                duration = "2:30",
                price = new { amount = prezzo, currency = "EUR" }
            },
            grids = Array.Empty<object>(),
            tooltip = offerta is null ? null : new { nodeServices = new[] { new { offer = offerta } } }
        });
        return JsonSerializer.Serialize(new { searchId = "ricerca", solutions = soluzioni });
    }

    private static Soluzione SoluzioneConPrezzo(double prezzo, string? offerta = null) => new()
    {
        solution = new SoluzioneDettaglio
        {
            id = "s",
            origin = "MILANO CENTRALE",
            destination = "ROMA TERMINI",
            price = new Prezzo { amount = prezzo, currency = "EUR" }
        },
        tooltip = offerta is null ? null : new TooltipSoluzione
        {
            nodeServices = [new NodoServizio { offer = offerta }]
        }
    };

    private static TrattaRicerca Tratta() => new(
        new Stazione { id = 830001700, name = "Milano Centrale" },
        new Stazione { id = 830008409, name = "Roma Termini" },
        new DateTime(2026, 10, 20),
        Adulti: 1, Bambini: 0,
        SoloFrecce: false, SoloRegionali: false, SoloDiretti: false);

    [Fact]
    public async Task CercaSoluzioni_piu_di_dieci_soluzioni_pagina_con_offset()
    {
        // Prima risposta piena (10), seconda con 3: il client deve chiamare due volte
        var chiamata = 0;
        var handler = new HttpHandlerFinto(_ => ++chiamata == 1
            ? new HttpResponseMessage(HttpStatusCode.OK) { Content = Json(JsonSoluzioni(10)) }
            : new HttpResponseMessage(HttpStatusCode.OK) { Content = Json(JsonSoluzioni(3)) });

        var client = ClientCon(handler);
        var soluzioni = await client.CercaSoluzioniAsync(Tratta());

        Assert.Equal(13, soluzioni.Count);
        Assert.Equal(2, chiamata);
    }

    [Fact]
    public async Task CercaSoluzioni_meno_di_dieci_soluzioni_una_sola_chiamata()
    {
        var handler = new HttpHandlerFinto(_ =>
            new HttpResponseMessage(HttpStatusCode.OK) { Content = Json(JsonSoluzioni(4)) });

        var client = ClientCon(handler);
        var soluzioni = await client.CercaSoluzioniAsync(Tratta());

        Assert.Equal(4, soluzioni.Count);
        Assert.Single(handler.UrlChiamate);
    }

    [Fact]
    public async Task CercaSoluzioni_risposta_vuota_termina_subito()
    {
        var handler = new HttpHandlerFinto(_ =>
            new HttpResponseMessage(HttpStatusCode.OK) { Content = Json("""{"searchId":"x","solutions":[]}""") });

        var client = ClientCon(handler);
        var soluzioni = await client.CercaSoluzioniAsync(Tratta());

        Assert.Empty(soluzioni);
        Assert.Single(handler.UrlChiamate);
    }

    [Fact]
    public async Task CercaStazioni_deserializza_i_nomi()
    {
        var handler = new HttpHandlerFinto(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = Json("""[{"id":830001700,"name":"Milano Centrale","displayName":"Milano Centrale","timezone":"Europe/Rome","multistation":false}]""")
            });

        var client = ClientCon(handler);
        var stazioni = await client.CercaStazioniAsync("milano");

        var stazione = Assert.Single(stazioni);
        Assert.Equal("Milano Centrale", stazione.name);
        Assert.Equal(830001700, stazione.id);
    }

    [Fact]
    public async Task TrovaStazione_corrisponde_al_nome_ignorando_maiuscole()
    {
        var handler = new HttpHandlerFinto(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = Json("""[{"id":830001700,"name":"Milano Centrale"}]""")
            });

        var client = ClientCon(handler);
        var stazione = await client.TrovaStazioneAsync("milano centrale");

        Assert.NotNull(stazione);
        Assert.Equal(830001700, stazione.id);
    }

    [Fact]
    public void FiltraSoluzioni_solo_super_economy_tiene_le_offerte_giuste()
    {
        var conSuper = SoluzioneConPrezzo(19.90, "Super Economy");
        var conStandard = SoluzioneConPrezzo(29.90, "Standard");

        var risultato = LefrecceClient.FiltraSoluzioni([conSuper, conStandard], soloSuperEconomy: true, prezzoMassimo: null);

        var unica = Assert.Single(risultato);
        Assert.True(LefrecceClient.EOffertaSuperEconomy(unica));
    }

    [Fact]
    public void FiltraSoluzioni_prezzo_massimo_esclude_le_soluzioni_care()
    {
        var economica = SoluzioneConPrezzo(9.90);
        var cara = SoluzioneConPrezzo(99.90);

        var risultato = LefrecceClient.FiltraSoluzioni([economica, cara], soloSuperEconomy: false, prezzoMassimo: 50);

        Assert.Single(risultato);
        Assert.Equal(9.90, risultato[0].solution!.price!.amount);
    }

    [Fact]
    public void FiltraSoluzioni_senza_filtri_rientrano_tutte()
    {
        var soluzioni = new List<Soluzione> { SoluzioneConPrezzo(1), SoluzioneConPrezzo(100) };

        var risultato = LefrecceClient.FiltraSoluzioni(soluzioni, soloSuperEconomy: false, prezzoMassimo: null);

        Assert.Equal(2, risultato.Count);
    }
}
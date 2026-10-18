// Ricerca Treni: il collante del form — scambio stazioni, date rapide,
// autocomplete con debounce e stato di ricerca.

(function () {
    'use strict';

    // -----------------------------------------------------------
    // Scambia le due stazioni
    // -----------------------------------------------------------
    var tastoScambia = document.querySelector('.btn-scambia');
    if (tastoScambia) {
        tastoScambia.addEventListener('click', function () {
            var partenza = document.getElementById('Form_StazionePartenza');
            var arrivo = document.getElementById('Form_StazioneArrivo');
            if (!partenza || !arrivo) return;
            var scambio = partenza.value;
            partenza.value = arrivo.value;
            arrivo.value = scambio;
            partenza.dispatchEvent(new Event('input', { bubbles: true }));
            arrivo.dispatchEvent(new Event('input', { bubbles: true }));
        });
    }

    // -----------------------------------------------------------
    // Date rapide: Oggi, Domani
    // -----------------------------------------------------------
    document.querySelectorAll('[data-data-rapida]').forEach(function (chip) {
        chip.addEventListener('click', function () {
            var campo = document.getElementById('Form_DataInizio');
            if (!campo) return;
            var salto = parseInt(chip.getAttribute('data-data-rapida'), 10);
            var giorno = new Date();
            giorno.setDate(giorno.getDate() + salto);
            campo.value = formattaData(giorno);
            campo.dispatchEvent(new Event('input', { bubbles: true }));
        });
    });

    function formattaData(data) {
        var mese = String(data.getMonth() + 1).padStart(2, '0');
        var giorno = String(data.getDate()).padStart(2, '0');
        return data.getFullYear() + '-' + mese + '-' + giorno;
    }

    // -----------------------------------------------------------
    // Autocomplete delle stazioni, con debounce di 250 ms
    // -----------------------------------------------------------
    var ultimeRichieste = {};

    document.querySelectorAll('.inputStazioniDropdown').forEach(function (input) {
        var datalist = document.getElementById(input.getAttribute('list'));
        if (!datalist) return;

        var attesa = null;

        input.addEventListener('input', function () {
            clearTimeout(attesa);
            var testo = input.value.trim();
            if (testo.length < 2) return;
            attesa = setTimeout(function () { cercaStazioni(input, datalist, testo); }, 250);
        });
    });

    function cercaStazioni(input, datalist, testo) {
        if (ultimeRichieste[testo] && input.dataset.ultima === testo) return;
        input.dataset.ultima = testo;
        fetch('./?handler=Stazioni&nome=' + encodeURIComponent(testo))
            .then(function (risposta) { return risposta.json(); })
            .then(function (nomi) {
                datalist.innerHTML = '';
                nomi.forEach(function (nome) {
                    var opzione = document.createElement('option');
                    opzione.value = nome;
                    datalist.appendChild(opzione);
                });
            })
            .catch(function () {
                // l'elenco resta quello che era
            });
    }

    // -----------------------------------------------------------
    // Stato di ricerca: il tasto si spegne e gira
    // -----------------------------------------------------------
    document.querySelectorAll('form[data-cerca]').forEach(function (modulo) {
        modulo.addEventListener('submit', function () {
            var tasto = modulo.querySelector('.btn-freccia');
            if (!tasto) return;
            tasto.disabled = true;
            tasto.classList.add('cercando-on');
            tasto.setAttribute('aria-busy', 'true');
        });
    });
})();
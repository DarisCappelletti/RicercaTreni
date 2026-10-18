<%@ Page
    Title="Ricerca treni"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ricerca.aspx.cs"
    Inherits="PortFolio.ricerca" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server" CssClass="container-lg">
    <style>
        #messaggio {
            position: fixed;
            top: 0;
            right: 0;
            z-index: 999999;
        }

        .modal:nth-of-type(even) {
            z-index: 1062 !important;
        }

        .modal-backdrop.show:nth-of-type(even) {
            z-index: 1061 !important;
        }

        th, td {
            word-break: normal;
        }

        .rwd-table {
          margin: 1em 0;
          min-width: 300px;
        }
        .rwd-table tr {
          border-top: 1px solid #ddd;
          border-bottom: 1px solid #ddd;
        }
        .rwd-table th {
          display: none;
        }
        .rwd-table td {
          display: block;
        }
        .rwd-table td:first-child {
          padding-top: .5em;
        }
        .rwd-table td:last-child {
          padding-bottom: .5em;
        }
        .rwd-table td:before {
          content: attr(data-th);
          font-weight: bold;
          width: 6.5em;
          display: inline-block;
        }
        @media (min-width: 635px) {
          .rwd-table td:before {
            display: none;
          }
        }
        .rwd-table th, .rwd-table td {
          text-align: left;
        }
        @media (min-width: 635px) {
          .rwd-table th, .rwd-table td {
            display: table-cell;
            padding: .25em .5em;
          }
          .rwd-table th:first-child, .rwd-table td:first-child {
            padding-left: 0;
          }
          .rwd-table th:last-child, .rwd-table td:last-child {
            padding-right: 0;
          }
        }
    </style>
    <script
        src="https://code.jquery.com/jquery-3.6.0.min.js"
        integrity="sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4="
        crossorigin="anonymous"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" integrity="sha512-9usAa10IRO0HhonpyAIVpjrylPvoDwiPUiKdWk5t3PyolY1cOd4DSE0Ga+ri4AuTroPR5aQvXU9xC6qOPnzFeg==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <asp:Label ID="messaggio" ClientIDMode="Static" runat="server"></asp:Label>
    <div class="container">
        <div>
            <h1>Ricerca treni</h1>
            <h2>Tool per la ricerca di treni</h2>
            <div class="accordion mb-3" id="accordionDettagli">
                <div class="accordion-item">
                <h2 class="accordion-header" id="headingThree">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#collapseThree" aria-expanded="false" aria-controls="collapseThree">
                        Funzionalità disponibili
                    </button>
                </h2>
                <div id="collapseThree" class="accordion-collapse collapse" aria-labelledby="headingThree" data-bs-parent="#accordionDettagli">
                    <div class="accordion-body">
                        <div style="font-size: small;">È possibile effettuare le seguenti operazioni: </div>
                        <ul style="font-size: small;">
                            <li>
                                Ricercare le soluzioni di un giorno specifico impostando una data di inizio
                            </li>
                            <li>
                                Ricercare tutte le soluzioni in un range di giorni impostando una data di inizio e di fine
                            </li>
                            <li>
                                Ricercare le soluzioni dove sono presenti biglietti "Super economy"
                            </li>
                            <li>
                                Ricercare le soluzioni dove sono presenti treni ad alta velocità
                            </li>
                            <li>
                                Ricercare le soluzioni dove sono presenti treni regionali
                            </li>
                            <li>
                                Ricercare le soluzioni dove sono presenti treni diretti
                            </li>
                            <li>
                                Ricercare le soluzioni impostando un tetto massimo di prezzo
                            </li>
                        </ul>
                    </div>
                </div>
                </div>
            </div>

            <asp:Panel ID="panRicercaTreni" runat="server">
                <div class="row">
                    <div class="col-md-6">
                        <label for="ddlStazionePartenza" class="form-label">
                            <strong>Stazione di partenza <span style="color: red;">*</span></strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta una stazione di partenza"></i>
                        </label>
                        <input class="form-control inputStazioniDropdown" list="ddlStazionePartenzaVoci" id="ddlStazionePartenza" runat="server" ClientIDMode="static" placeholder="Scrivi per ricercare la stazione desiderata...">
                        <datalist id="ddlStazionePartenzaVoci" class="stazioniDropdown">
                        </datalist>

                    </div>
                    <div class="col-md-6">
                        <label for="ddlStazionePartenza" class="form-label">
                            <strong>Stazione di arrivo <span style="color: red;">*</span></strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta una stazione di arrivo"></i>
                        </label>
                        <input class="form-control inputStazioniDropdown" list="ddlStazioneArrivoVoci" id="ddlStazioneArrivo" runat="server" ClientIDMode="static" placeholder="Scrivi per ricercare la stazione desiderata...">
                        <datalist id="ddlStazioneArrivoVoci" class="stazioniDropdown">
                        </datalist>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="txtNumeroAdulti" class="form-label">
                            <strong>Numero adulti <span style="color: red;">*</span></strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta il numero di adulti (almeno 1 adulto)"></i>
                        </label>
                        <asp:TextBox TextMode="Number" ID="txtNumeroAdulti" runat="server" CssClass="form-control">0</asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label for="txtNumeroBambini" class="form-label">
                            <strong>Numero bambini</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta il numero di bambini"></i>
                        </label>
                        <asp:TextBox TextMode="Number" ID="txtNumeroBambini" runat="server" CssClass="form-control">0</asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label for="txtDataInizio" class="form-label">
                            <strong>Data di inizio <span style="color: red;">*</span></strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la data desiderata (impostando soltanto la data di inizio verranno visualizzate le soluzioni di quel giorno)"></i>
                        </label>
                        <asp:TextBox TextMode="Date" ID="txtDataInizio" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label for="txtDataFine" class="form-label">
                            <strong>Data di fine</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la data di fine (permette di ricercare le soluzioni in un range di date partendo dalla data di inizio fino a quella di fine)"></i>
                        </label>
                        <asp:TextBox TextMode="Date" ID="txtDataFine" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <label for="chkSuperEconomy" class="form-label">
                            <strong>Solo Super Economy</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la visione di soluzioni con biglietti Super Economy"></i>
                        </label>
                        <asp:CheckBox ID="chkSuperEconomy" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-2">
                        <label for="chkFrecce" class="form-label">
                            <strong>Solo Frecce</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la visione di soluzioni con treni ad alta velocità"></i>
                        </label>
                        <asp:CheckBox ID="chkFrecce" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-2">
                        <label for="chkRegionali" class="form-label">
                            <strong>Solo Regionali</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la visione di soluzioni con treni regionali"></i>
                        </label>
                        <asp:CheckBox ID="chkRegionali" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-2">
                        <label for="chkDiretto" class="form-label">
                            <strong>Diretto</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la visione di soluzioni con treni diretti"></i>
                        </label>
                        <asp:CheckBox ID="chkDiretto" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label for="txtPrezzoMassimo" class="form-label">
                            <strong>Prezzo massimo</strong> 
                            <i class="fa-solid fa-circle-info text-primary" 
                                data-bs-toggle="tooltip" 
                                data-bs-placement="top" 
                                title="Imposta la visione di soluzioni con prezzi inferiori al numero impostato"></i>
                        </label>
                        <asp:TextBox TextMode="Number" ID="txtPrezzoMassimo" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>

                <asp:Button ID="btnRicercaTreni" runat="server" OnClick="btnRicercaTreni_Click" CssClass="mt-3 btn btn-primary" Text="Ricerca" />
            </asp:Panel>

            <!-- stato -->
            <div class="stato text-center pt-2">
                <asp:Label ID="stato" runat="server"></asp:Label>
            </div>

            <!-- Tabella -->
            <asp:GridView
                ID="gdvTreni"
                runat="server"
                AlternatingRowStyle-CssClass="alt"
                AutoGenerateColumns="false"
                Width="100%" border="1" CellPadding="3" CssClass="rwd-table table table-striped table-bordered table-hover"
                Style="border: 1px solid #E5E5E5; word-break: break-all; word-wrap: break-word"
                OnRowDataBound="gdvTreni_RowDataBound">
                <HeaderStyle CssClass="StickyHeader" />
                <PagerStyle CssClass="Footer" />
                <Columns>
                    <asp:BoundField DataField="solution.origin" HeaderText="Stazione partenza" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="sequid" />
                    <asp:BoundField DataField="solution.destination" HeaderText="Stazione arrivo" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="source" />
                    <asp:BoundField DataField="solution.departureTime" HeaderText="Data partenza" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="type" />
                    <asp:BoundField DataField="solution.arrivalTime" HeaderText="Data arrivo" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="start" />
                    <asp:TemplateField HeaderText="Prezzo">
                        <ItemTemplate>
                            <%# Eval("solution.price.amount") + " " + Eval("solution.price.currency") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="solution.duration" HeaderText="Durata" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="end" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target='<%# "#modalDettaglio-" + Eval("solution.id") %>'>
                                Dettagli
                            </button>
                            <!-- Prima modal col dettaglio delle tratte -->
                            <div class="modal fade" id='<%# "modalDettaglio-" + Eval("solution.id") %>' tabindex="-1" aria-labelledby="modalDettaglioLabel" aria-hidden="true">
                                <div class="modal-dialog modal-lg">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="modalDettaglioLabel">Dettagli soluzione</h5>
                                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                        </div>
                                        <div class="modal-body">
                                            <table class="table">
                                                <thead>
                                                    <tr>
                                                        <th scope="col" style="width: 145px;">Treno</th>
                                                        <th scope="col">Partenza</th>
                                                        <th scope="col">Orario</th>
                                                        <th scope="col">Arrivo</th>
                                                        <th scope="col">Orario</th>
                                                        <%--<th scope="col">Prezzo</th>
                                                        <th scope="col">Descrizione</th>--%>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater runat="server" DataSource='<%# Eval("grids") %>'
                                                                OnItemDataBound="RepeaterInner_ItemDataBound">
                                                        <ItemTemplate>
                                                            <asp:Repeater
                                                                ID="RepeaterInner"
                                                                runat="server"
                                                                DataSource='<%# Eval("summaries") %>'>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td data-th="Treno: "><%# Eval("name") %></td>
                                                                        <td data-th="Partenza: "><%# Eval("departureLocationName") %></td>
                                                                        <td data-th="Orario: "><%# Eval("departureTime") %></td>
                                                                        <td data-th="Arrivo: "><%# Eval("arrivalLocationName") %></td>
                                                                        <td data-th="Orario: "><%# Eval("arrivalTime") %></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                    <tr>
                                                        <td colspan="8" class="text-center">
                                                            <button
                                                                    id="btnModalDettagliOfferta"
                                                                    runat="server"
                                                                    ClientIDMode="static"
                                                                    type="button"
                                                                    class="btn btn-primary btnModalDettagliOfferta">
                                                                    Dettagli
                                                                </button>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8">
                                                            <div
                                                                style="display: none;"
                                                                id="modalDettagliOfferta"
                                                                ClientIDMode="static"
                                                                runat="server">
                                                                <table class="table">
                                                                    <thead>
                                                                        <tr>
                                                                            <th scope="col" style="min-width: 90px;">Nome</th>
                                                                            <th scope="col" style="min-width: 90px;">Offerta</th>
                                                                            <th scope="col" style="min-width: 90px;">Prezzo</th>
                                                                            <th scope="col" style="min-width: 90px;">Disponibiltà</th>
                                                                            <th scope="col" style="min-width: 90px;">Descrizione</th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                        <asp:Repeater runat="server" DataSource='<%# Eval("services") %>'>
                                                                            <ItemTemplate>
                                                                                <asp:Repeater runat="server" DataSource='<%# Eval("offers") %>'>
                                                                                    <ItemTemplate>
                                                                                        <%# (int)Eval("availableAmount") > 0 
                                                                                                ? 
                                                                                                    "<tr>" + 
                                                                                                    "<td data-th=\"Nome: \">" + Eval("serviceName") + "</td>" +
                                                                                                    "<td data-th=\"Offerta: \">" + Eval("name") + "</td>" +
                                                                                                    "<td data-th=\"Prezzo: \">" + Eval("price.amount") + " " + Eval("price.currency") + "</td>" +
                                                                                                    "<td data-th=\"Disponibiltà: \">" + Eval("availableAmount") + "</td>" +
                                                                                                    "<td data-th=\"Descrizione: \">" + Eval("description") + "</td>" +
                                                                                                    "</tr>"
                                                                                                : "" 
                                                                                        %>
                                                                                    </ItemTemplate>
                                                                                </asp:Repeater>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>

                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tbody>
                                            </table>
                                        </div>
                                        <div class="modal-footer">
                                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Indietro</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <script>
        $(".btnModalDettagliOfferta").click(function () {
            var div = $(this).attr("data-classe")
            if ($("." + div).hasClass("active")) {
                $("." + div).slideUp("fast", function () {
                    $(this).css("display", "none");
                    $(this).removeClass("active");
                });
            }
            else {
                $("." + div).slideDown("fast", function () {
                    $(this).css("display", "block");
                    $(this).addClass("active");
                });

            }
        });
    </script>

    <script>
        $(".inputStazioniDropdown").keyup(function () {
            console.log($(this).val().length)
            if ($(this).val().length >= 2) {
                var params = { parametro: $(this).val() };
                var dropdown = $(this).siblings('.stazioniDropdown')

                $.ajax({
                    type: "POST",
                    async: true,
                    url: "ricerca.aspx/getStazioni",
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, status) {
                        dropdown.empty();
                        data = JSON.parse(data.d);
                        $.each(data, function (dt) {
                            dropdown.append('<option value="' + this.name + '">')
                        });
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
        });
        $(document).ready(function () {
            
        });
        
    </script>
</asp:Content>

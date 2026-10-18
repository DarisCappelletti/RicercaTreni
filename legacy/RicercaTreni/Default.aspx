<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="RicercaTreni._Default" %>

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
    </style>
    <script
        src="https://code.jquery.com/jquery-3.6.0.min.js"
        integrity="sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4="
        crossorigin="anonymous"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" integrity="sha512-9usAa10IRO0HhonpyAIVpjrylPvoDwiPUiKdWk5t3PyolY1cOd4DSE0Ga+ri4AuTroPR5aQvXU9xC6qOPnzFeg==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <asp:Label ID="messaggio" ClientIDMode="Static" runat="server"></asp:Label>
    <div class="container">
        <div>
            <h2>Ricerca treni</h2>
            <div style="font-size: small;">
                Tool per la ricerca di treni<br />
                È possibile effettuare le seguenti operazioni:
            </div>
            <ul style="font-size: small;">
                <li>ricercare un treno impostando la stazione di partenza e di destinazione
                </li>
                <li>ricercare un treno impostando la data
                </li>
                <li>ricercare un treno impostando una data inizio e una data fine
                </li>
            </ul>
            <asp:Panel ID="panRicercaTreni" runat="server">
                <div class="row">
                    <div class="col-md-2">
                        <asp:Label ID="lblTreni" runat="server"><strong>Seleziona le stazioni:</strong></asp:Label>
                    </div>
                    <div class="col-md-4">
                        <asp:DropDownList ID="ddlStazionePartenza" runat="server"></asp:DropDownList>
                    </div>
                    <div class="col-md-6">
                        <asp:DropDownList ID="ddlStazioneArrivo" runat="server"></asp:DropDownList>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-2">
                        <asp:Label ID="Label1" runat="server"><strong>Seleziona un intervallo di date:</strong></asp:Label>
                    </div>
                    <div class="col-md-4">
                        <asp:TextBox TextMode="Date" ID="txtDataInizio" runat="server"></asp:TextBox>
                    </div>
                    <div class="col-md-6">
                        <asp:TextBox TextMode="Date" ID="txtDataFine" runat="server"></asp:TextBox>
                    </div>
                </div>
                <asp:Button ID="btnRicercaTreni" runat="server" OnClick="btnRicercaTreni_Click" Text="Ricerca" />
            </asp:Panel>

            <!-- Tabella -->
            <asp:GridView
                ID="gdvTreni"
                runat="server"
                AlternatingRowStyle-CssClass="alt"
                AutoGenerateColumns="false"
                Width="100%" border="1" CellPadding="3" CssClass="table table-striped table-bordered table-hover"
                Style="border: 1px solid #E5E5E5; word-break: break-all; word-wrap: break-word">
                <HeaderStyle CssClass="StickyHeader" />
                <PagerStyle CssClass="Footer" />
                <Columns>
                    <asp:BoundField DataField="origin" HeaderText="Stazione partenza" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="sequid" />
                    <asp:BoundField DataField="destination" HeaderText="Stazione arrivo" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="source" />
                    <asp:BoundField DataField="DataPartenza" HeaderText="Data partenza" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="type" />
                    <asp:BoundField DataField="DataArrivo" HeaderText="Data arrivo" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="start" />
                    <asp:BoundField DataField="minprice" HeaderText="Prezzo minimo senza offerte" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="end" />
                    <asp:BoundField DataField="duration" HeaderText="Durata" ItemStyle-CssClass="short" HeaderStyle-CssClass="short" SortExpression="end" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target='<%# "#modalDettaglio-" + Eval("idSolution") %>'>
                                Dettagli
                            </button>
                            <div class="modal fade" id='<%# "modalDettaglio-" + Eval("idSolution") %>' tabindex="-1" aria-labelledby="modalDettaglioLabel" aria-hidden="true">
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
                                                        <th scope="col">Offerta</th>
                                                        <th scope="col">Prezzo</th>
                                                        <th scope="col">Descrizione</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:Repeater runat="server" DataSource='<%# Eval("Dettaglio.leglist") %>'>
                                                        <ItemTemplate>
                                                            <asp:Repeater
                                                                ID="RepeaterInner"
                                                                runat="server"
                                                                DataSource='<%# Eval("Servicelist") %>'
                                                                OnItemDataBound="RepeaterInner_ItemDataBound">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.trainidentifier")%></td>
                                                                        <td><%# DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.departurestation")%></td>
                                                                        <td><%# DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.departuretime")%></td>
                                                                        <td><%# DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.arrivalstation")%></td>
                                                                        <td><%# DataBinder.Eval(Container.NamingContainer.NamingContainer, "DataItem.arrivaltime")%></td>
                                                                        <td><%# Eval("name") %></td>
                                                                        <td><%# Eval("minprice") %></td>
                                                                        <td>
                                                                            <button
                                                                                id="btnModalDettagliOfferta"
                                                                                runat="server"
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
                                                                                runat="server">
                                                                                <table class="table">
                                                                                    <thead>
                                                                                        <tr>
                                                                                            <th scope="col" style="min-width: 90px;">Nome</th>
                                                                                            <th scope="col" style="min-width: 90px;">Prezzo</th>
                                                                                            <th scope="col" style="min-width: 90px;">Disponibiltà</th>
                                                                                            <th scope="col" style="min-width: 90px;">Descrizione</th>
                                                                                        </tr>
                                                                                    </thead>
                                                                                    <tbody>
                                                                                        <asp:Repeater runat="server" DataSource='<%# Eval("subservicelist") %>'>
                                                                                            <ItemTemplate>
                                                                                                <asp:Repeater runat="server" DataSource='<%# Eval("offerlist") %>'>
                                                                                                    <ItemTemplate>
                                                                                                        <tr>
                                                                                                            <td><%# Eval("name") %></td>
                                                                                                            <td><%# Eval("price") %></td>
                                                                                                            <td><%# Eval("available") %></td>
                                                                                                            <td><%# Eval("description") %></td>
                                                                                                        </tr>
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
                $("." + div).slideUp( "fast", function() {
                    $(this).css("display", "none");
                    $(this).removeClass( "active" );
                });
            }
            else {
                $("." + div).slideDown( "fast", function() {
                    $(this).css("display", "block");
                    $(this).addClass( "active" );
                });
                
            }
        });
    </script>
</asp:Content>

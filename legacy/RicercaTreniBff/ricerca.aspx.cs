

using PortFolio.Enums;
using PortFolio.RicercaTreni.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using static PortFolio.RicercaTreni.Models.PostSoluzioneViaggio;
using static RicercaTreni.Models.DettaglioSoluzione;
using static RicercaTreni.Models.GetSoluzioneViaggio;

namespace PortFolio
{
    public partial class ricerca : Page
    {
        int i = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

            }
        }

        [WebMethod]
        public static string getStazioni(string parametro)
        {
            string api = "https://www.lefrecce.it/Channels.Website.BFF.WEB/website/locations/search?name=" + parametro + "&limit=100";
            var request = (HttpWebRequest)WebRequest.Create(api);
            request.ContentType = "application/json";
            request.Method = "GET";

            WebResponse response = request.GetResponse() as HttpWebResponse;
            var stream = response.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream);
            // Leggo la risposta.
            string streamJson2 = reader2.ReadToEnd();

            return streamJson2;
        }

        public static string getStazioniOnline(CookieContainer cookieContainer, string nome)
        {
            try
            {
                //using (WebClient wc = new WebClient())
                //{
                //    var json = wc.DownloadString("https://www.lefrecce.it/Channels.Website.BFF.WEB/website/locations/search?name=" + nome + "&limit=100");

                //    //var listaStazioni = JsonSerializer.Deserialize<List<Stazione>>(json);

                //    return json;
                //}

                string api = "https://www.lefrecce.it/Channels.Website.BFF.WEB/website/locations/search?name=" + nome + "&limit=100";
                var request = (HttpWebRequest)WebRequest.Create(api);
                request.ContentType = "application/json";
                request.Method = "GET";
                request.CookieContainer = cookieContainer;

                WebResponse response = request.GetResponse() as HttpWebResponse;
                var stream = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream);
                // Leggo la risposta.
                string streamJson2 = reader2.ReadToEnd();

                return streamJson2;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        public List<PostSolution> ricercoTratta(DateTime giorno, CookieContainer cookieContainer, Stazione partenza, Stazione arrivo)
        {
            var soluzioni = new List<PostSolution>();

            // Prendo la lista delle stazioni
            //WebClient client = new WebClient();
            //Stream stream = client.OpenRead(ConfigurationManager.AppSettings["SiteUrl"].ToString() + "RicercaTreni/stazioni.json");
            //StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/RicercaTreni/stazioni.json"));
            //string streamJson = reader.ReadToEnd();

            SiteMaster MasterPage = (SiteMaster)Page.Master;

            // Imposto i parametri di ricerca
            Ricerca data2 = new Ricerca();
            data2.departureLocationId = Convert.ToInt32(partenza.id);
            data2.arrivalLocationId = Convert.ToInt32(arrivo.id);

            // Impostata come stringa a causa del JavaScriptSerializer
            data2.departureTime = giorno.Date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            data2.adults = txtNumeroAdulti.Text != null && txtNumeroAdulti.Text != "" ? Convert.ToInt32(txtNumeroAdulti.Text) : 0;
            data2.children = txtNumeroBambini.Text != null && txtNumeroBambini.Text != "" ? Convert.ToInt32(txtNumeroBambini.Text) : 0;

            data2.advancedSearchRequest = new AdvancedSearchRequest()
            {
                bestFare = false
            };

            bool ciSonoElementi = true;
            int offset = 0;

            while (ciSonoElementi)
            {
                try
                {
                    // Imposto i criteri di ricerca (nello specifico l'offset)
                    data2.criteria = new Criteria()
                    {
                        frecceOnly = chkFrecce.Checked,
                        regionalOnly = chkRegionali.Checked,
                        noChanges = chkDiretto.Checked,
                        order = "DEPARTURE_DATE",
                        offset = offset,
                        limit = 10
                    };

                    // serializzo in json
                    var json = new JavaScriptSerializer().Serialize(data2);
                    // effettuo la chiamata POST
                    var listaSoluzioni = ricercoSoluzioni(cookieContainer, json);
                    soluzioni.AddRange(listaSoluzioni.solutions);

                    if (listaSoluzioni.solutions.Count >= 10)
                    {
                        // ci sono ancora elementi quindi effettuo di nuovo la chiamata aumentando l'offset
                        offset = offset + 10;
                    }
                    else
                    {
                        // non ci sono più elementi
                        ciSonoElementi = false;
                        break;
                    }
                }
                catch (Exception exc)
                {
                    // Errore quando non ci sono più elementi ed effettuo la chiamata
                    ciSonoElementi = false;
                    break;
                }
            }

            // Filtro i super economy
            if (chkSuperEconomy.Checked)
            {
                soluzioni =
                    soluzioni.Where(x =>
                        x.tooltip.nodeServices != null && x.tooltip.nodeServices.Count != 0 &&
                        x.tooltip.nodeServices.Any(y => y != null && y.offer != null && y.offer == "Super Economy"))
                    .ToList();
            }

            return soluzioni;
        }

        public RootPostSoluzioneViaggio ricercoSoluzioni(CookieContainer cookieContainer, string json)
        {
            string api = "https://www.lefrecce.it/Channels.Website.BFF.WEB/website/ticket/solutions";
            var request = (HttpWebRequest)WebRequest.Create(api);
            request.ContentType = "application/json";
            request.Headers.Add("accept-language", "it-IT");
            request.Method = "POST";
            request.CookieContainer = cookieContainer;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            WebResponse response = request.GetResponse() as HttpWebResponse;
            var stream = response.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream);
            // Leggo la risposta.
            string streamJson2 = reader2.ReadToEnd();

            var listaSoluzioni = new JavaScriptSerializer().Deserialize<RootPostSoluzioneViaggio>(streamJson2);

            return listaSoluzioni;
        }

        //private List<Soluzione> GetListaSoluzioni(string api)
        //{
        //    // creo la richiesta in get con i parametri
        //    CookieContainer cookieContainer = new CookieContainer();
        //    var request = (HttpWebRequest)WebRequest.Create(api);
        //    request.CookieContainer = cookieContainer;

        //    // Imposto un timeout di 3 secondi
        //    //request.Timeout = 1500;

        //    // Effettuo la chiamata GET
        //    WebResponse response = request.GetResponse();
        //    var stream = response.GetResponseStream();
        //    StreamReader reader = new StreamReader(stream);
        //    // Leggo la risposta.
        //    string streamJson = reader.ReadToEnd();

        //    // Estraggo i dati dal json di risposta
        //    //var root = JsonSerializer.Deserialize<Root>(streamJson);
        //    var listaSoluzioni = JsonSerializer.Deserialize<List<Soluzione>>(streamJson);
        //    //var listeSinonimi = root.response.Select(x => x.list.synonyms).ToList();
        //    foreach (var soluzione in listaSoluzioni)
        //    {
        //        var dettaglio = GetDettaglioSoluzione(cookieContainer, soluzione.idsolution);
        //        soluzione.Dettaglio = dettaglio;
        //    }
        //    return listaSoluzioni;
        //}

        private RootSoluzioni getDettagli(CookieContainer cookieContainer, PostSolution soluzione, string cartID)
        {
            string api =
                "https://www.lefrecce.it/Channels.Website.BFF.WEB/website/cart?cartId=" + cartID +
                "&currentSolutionId=" + soluzione.solution.id;
            var request = (HttpWebRequest)WebRequest.Create(api);
            request.ContentType = "application/json";
            request.Method = "GET";
            request.CookieContainer = cookieContainer;

            WebResponse response = request.GetResponse() as HttpWebResponse;
            var stream = response.GetResponseStream();
            StreamReader reader2 = new StreamReader(stream);
            // Leggo la risposta.
            string streamJson2 = reader2.ReadToEnd();

            var listaSoluzioni = new JavaScriptSerializer().Deserialize<RootSoluzioni>(streamJson2);

            return listaSoluzioni;
        }

        //private DettaglioSoluzione GetDettaglioSoluzione(CookieContainer cookieContainer, string idSoluzione)
        //{
        //    try
        //    {
        //        var richiesta = "https://www.lefrecce.it/msite/api/solutions/" + idSoluzione + "/standardoffers";
        //        // creo la richiesta in get con i parametri
        //        var request = (HttpWebRequest)WebRequest.Create(richiesta);
        //        request.CookieContainer = cookieContainer;

        //        // Imposto un timeout di 3 secondi
        //        //request.Timeout = 1500;

        //        // Effettuo la chiamata GET
        //        WebResponse response = request.GetResponse();
        //        var stream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(stream);
        //        // Leggo la risposta.
        //        string streamJson = reader.ReadToEnd();

        //        // Estraggo i dati dal json di risposta
        //        var dettaglioSoluzione = JsonSerializer.Deserialize<DettaglioSoluzione.Root>(streamJson);

        //        return dettaglioSoluzione;
        //    }
        //    catch (Exception exc)
        //    {
        //        return null;
        //    }
        //}

        protected void btnRicercaTreni_Click(object sender, EventArgs e)
        {
            SiteMaster MasterPage = (SiteMaster)Page.Master;
            if (txtDataInizio.Text == "")
            {
                MasterPage.ShowMessage(HttpUtility.HtmlEncode("Inserire la data di inizio."), Alerts.Errore);
            }
            else if (txtNumeroAdulti.Text == null || txtNumeroAdulti.Text == "" || Convert.ToInt32(txtNumeroAdulti.Text) == 0)
            {
                MasterPage.ShowMessage(HttpUtility.HtmlEncode("Impostare almeno un adulto per avviare la ricerca."), Alerts.Errore);
            }
            else
            {
                // Imposto il coockiConteiner da utilizzare per le chiamate
                CookieContainer cookieContainer = new CookieContainer();

                string stazionePartenza = getStazioniOnline(cookieContainer, ddlStazionePartenza.Value.Trim());
                string stazioneArrivo = getStazioniOnline(cookieContainer, ddlStazioneArrivo.Value.Trim());

                var listaStazioniPartenza = new JavaScriptSerializer().Deserialize<List<Stazione>>(stazionePartenza);
                var listaStazioniArrivo = new JavaScriptSerializer().Deserialize<List<Stazione>>(stazioneArrivo);

                // Verifico le stazioni
                var partenza =
                    listaStazioniPartenza
                    .Where(x => x.name.ToLower() == ddlStazionePartenza.Value.Trim().ToLower()).FirstOrDefault();
                var arrivo =
                    listaStazioniArrivo
                    .Where(x => x.name.ToLower() == ddlStazioneArrivo.Value.Trim().ToLower()).FirstOrDefault();

                if (partenza == null)
                {
                    MasterPage.ShowMessage(HttpUtility.HtmlEncode("La stazione di partenza non è corretta."), Alerts.Errore);
                }
                else if (arrivo == null)
                {
                    MasterPage.ShowMessage(HttpUtility.HtmlEncode("La stazione di arrivo non è corretta."), Alerts.Errore);
                }
                else if (txtDataFine.Text == "")
                {
                    var dataInizio = Convert.ToDateTime(txtDataInizio.Text);

                    var soluzione = ricercoTratta(dataInizio, cookieContainer, partenza, arrivo);

                    if (soluzione != null)
                    {
                        stato.Text = "Ci sono <strong>" + soluzione.Count + "</strong> risultati.";
                        gdvTreni.DataSource = soluzione;
                        gdvTreni.DataBind();
                    }
                    else
                    {
                        stato.Text = "Nessun risultato trovato.";
                    }
                }
                else
                {
                    var dataInizio = Convert.ToDateTime(txtDataInizio.Text);
                    var dataFine = Convert.ToDateTime(txtDataFine.Text);

                    var listaSoluzioni = new List<PostSolution>();
                    foreach (DateTime day in EachDay(dataInizio, dataFine))
                    {
                        var lista = ricercoTratta(day, cookieContainer, partenza, arrivo);

                        if (lista != null)
                        {
                            listaSoluzioni.AddRange(lista);
                        }
                    }

                    if (listaSoluzioni != null)
                    {
                        stato.Text = "Ci sono <strong>" + listaSoluzioni.Count + "</strong> risultati.";
                        gdvTreni.DataSource = listaSoluzioni;
                        gdvTreni.DataBind();
                    }
                    else
                    {
                        stato.Text = "Nessun risultato trovato.";
                    }
                }
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        //public class Soluzione
        //{
        //    public string idsolution { get; set; }
        //    public string origin { get; set; }
        //    public string destination { get; set; }
        //    public string direction { get; set; }
        //    public double departuretime { get; set; }
        //    public DateTime DataPartenza
        //    {
        //        get
        //        {
        //            return UnixTimeStampToDateTime(this.departuretime);
        //        }
        //    }
        //    public double arrivaltime { get; set; }
        //    public DateTime DataArrivo
        //    {
        //        get
        //        {
        //            return UnixTimeStampToDateTime(this.arrivaltime);
        //        }
        //    }
        //    public double minprice { get; set; }
        //    public object optionaltext { get; set; }
        //    public string duration { get; set; }
        //    public int changesno { get; set; }
        //    public bool bookable { get; set; }
        //    public bool saleable { get; set; }
        //    public List<Trainlist> trainlist { get; set; }
        //    public bool onlycustom { get; set; }
        //    public List<object> extraInfo { get; set; }
        //    public bool showSeat { get; set; }
        //    public object specialOffer { get; set; }
        //    public List<object> transportMeasureList { get; set; }
        //    public double originalPrice { get; set; }
        //    public DettaglioSoluzione.Root Dettaglio { get; set; }
        //}

        public class Trainlist
        {
            public string trainidentifier { get; set; }
            public string trainacronym { get; set; }
            public string traintype { get; set; }
            public string pricetype { get; set; }
        }

        protected void RepeaterInner_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var Servicelist = (PostSoluzioneViaggio.Grid)e.Item.DataItem;
            var btnModalDettagliOfferta = (HtmlButton)e.Item.FindControl("btnModalDettagliOfferta");
            var aModalDettagliOfferta = (HtmlAnchor)e.Item.FindControl("aModalDettagliOfferta");
            HtmlGenericControl modalDettagliOfferta = e.Item.FindControl("modalDettagliOfferta") as HtmlGenericControl;
            var lnkAvverti = (LinkButton)e.Item.FindControl("lnkAvverti");
            var rep = (Repeater)e.Item.FindControl("repRiorganizzatori");

            btnModalDettagliOfferta.Attributes["style"] = Servicelist.services != null && Servicelist.services.Count > 0 ? "" : "display: none;";
            btnModalDettagliOfferta.Attributes["data-classe"] = "modalDettaglioOfferta-" + i;
            modalDettagliOfferta.Attributes["class"] = "table-responsive modalDettaglioOfferta-" + i;
            i++;
        }

        protected void gdvTreni_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TableCellCollection cell = e.Row.Cells;
                cell[0].Attributes.Add("data-th", "Stazione partenza: ");
                cell[1].Attributes.Add("data-th", "Stazione arrivo: ");
                cell[2].Attributes.Add("data-th", "Data partenza: ");
                cell[3].Attributes.Add("data-th", "Data arrivo: ");
                cell[4].Attributes.Add("data-th", "Prezzo: ");
                cell[5].Attributes.Add("data-th", "Durata: ");
                cell[6].Attributes.Add("data-th", "Vai ai dettagli: ");
            }
        }
    }
}
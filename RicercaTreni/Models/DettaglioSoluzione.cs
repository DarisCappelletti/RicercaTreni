using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RicercaTreni.Models
{
    public class DettaglioSoluzione
    {
        public class Root
        {
            public string idsolution { get; set; }
            public List<Leglist> leglist { get; set; }
            public List<object> extraInfo { get; set; }
        }

        public class Credential
        {
            public int credentialid { get; set; }
            public int format { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public object possiblevalues { get; set; }
            public string typeCredential { get; set; }
        }

        public class Leglist
        {
            public string idleg { get; set; }
            public string bookingtype { get; set; }
            public List<Segment> segments { get; set; }
            public List<Servicelist> servicelist { get; set; }
            public bool gift { get; set; }
            public string trainidentifier { get; set; }
            public string trainacronym { get; set; }
            public string departurestation { get; set; }
            public DateTime departuretime { get; set; }
            public string arrivalstation { get; set; }
            public DateTime arrivaltime { get; set; }
        }

        public class Offerid
        {
            public string xmlid { get; set; }
            public double price { get; set; }
            public object eligible { get; set; }
            public List<object> messages { get; set; }
        }

        public class Offeridlist
        {
            public string xmlid { get; set; }
            public double price { get; set; }
            public object eligible { get; set; }
            public List<object> messages { get; set; }
        }

        public class Offerlist
        {
            public string name { get; set; }
            public List<object> extraInfo { get; set; }
            public object transportMeasure { get; set; }
            public double points { get; set; }
            public double price { get; set; }
            public string message { get; set; }
            public List<Offeridlist> offeridlist { get; set; }
            public int available { get; set; }
            public List<Credential> credentials { get; set; }
            public bool selected { get; set; }
            public bool saleable { get; set; }
            public Offerid offerid { get; set; }
            public bool visible { get; set; }
            public List<object> specialOffers { get; set; }
            public bool seatToPay { get; set; }
            public bool disableSeatmapSelection { get; set; }
            public string description { get; set; }
        }

        public class Segment
        {
            public string trainidentifier { get; set; }
            public string trainacronym { get; set; }
            public string departurestation { get; set; }
            public DateTime departuretime { get; set; }
            public string arrivalstation { get; set; }
            public DateTime arrivaltime { get; set; }
            public string nodexmlid { get; set; }
            public bool showseatmap { get; set; }
        }

        public class Servicelist
        {
            public string name { get; set; }
            public List<Offerlist> offerlist { get; set; }
            public List<Subservicelist> subservicelist { get; set; }
            public bool hasGift { get; set; }
            public double minprice { get; set; }
        }

        public class Subservicelist
        {
            public string name { get; set; }
            public List<Offerlist> offerlist { get; set; }
            public bool hasGift { get; set; }
            public double? minprice { get; set; }
        }
    }
}
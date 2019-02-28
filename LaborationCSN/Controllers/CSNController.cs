using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace LaborationCSN.Controllers
{
    public class CSNController : Controller
    {
        string appDataFolder = HostingEnvironment.MapPath("/App_Data/");
        XElement csnXml;

        // XML namespaces
        XNamespace csntyp = "http://schema.csn.se/csntyper";
        XNamespace csnper = "http://schema.csn.se/person";
        XNamespace csnsm = "http://schema.csn.se/studiemedel";
        XNamespace csn = "http://schema.csn.se/kommunfraga";

        public CSNController()
        {
            csnXml = XElement.Load(appDataFolder + "CSN_199999999999.xml");
        }

        //
        // GET: /Csn/Test
        // 
        // Testmetod som visar på hur ni kan arbeta från XML till
        // presentations-xml som sedan används i vyn.
        // Lite överkomplicerat för just detta enkla fall men visar på idén.
        public ActionResult Test()
        {

            // detta är den xml vi utgår från, kan liknas med csnXml ovan.
            var xml =
                new XElement("SvarSelmaAllaKurser",
                    new XElement("AllaKurser",
                        new XElement("Kurs",
                            new XElement("Kursnamn", "Forskningsmetod"),
                            new XElement("Kurskod", "2IS033"),
                            new XElement("Kurspoäng", "7,5")),
                        new XElement("Kurs",
                            new XElement("Kursnamn", "Informationsinfrastruktur"),
                            new XElement("Kurskod", "2IS010"),
                            new XElement("Kurspoäng", "7,5")),
                        new XElement("Kurs",
                            new XElement("Kursnamn", "Examensarbete"),
                            new XElement("Kurskod", "2AD335"),
                            new XElement("Kurspoäng", "15"))));


            // transformerar ursprungs xml för en bättre struktur för den vy vi ska använda
            var presentations_xml =
                // rotelement
                new XElement("Kurser",
                    // för alla "<Kurs>" noder
                    from kurs in xml.Element("AllaKurser").Elements("Kurs")
                    select
                        new XElement("Kurs",
                            // vi vill enbart ha vissa noder och använda
                            // andra namn på noderna (av någon viktig anledning)
                            new XElement("Namn", (string)kurs.Element("Kursnamn")),
                            new XElement("HP", (string)kurs.Element("Kurspoäng")))
                );

            // skicka presentations xml:n till vyn /Views/Csn/Test,
            // i vyn kommer vi åt den genom variabeln "Model"
            return View(presentations_xml);
        }

        //
        // GET: /Csn/Index

        public ActionResult Index()
        {
            return View();
        }


        //
        // GET: /Csn/Uppgift1
        //haha bög
        public ActionResult Uppgift1()
        {
            var xml =
                new XElement("Ärenden",
                    from arende in csnXml.Descendants(csnper + "Arende")
                    select new XElement("Ärende",
                              new XElement("lopnummer", arende.Attribute("lopnummer").Value),
                              new XElement("Bidrag", arende.Element(csnsm + "klartext").Value),
                              
                                    from k in arende.Descendants(csnsm + "Utbetalning")
                                    select new XElement("Utbetalningar",
                                        new XElement("Datum", k.Element(csntyp + "utbetdatum").Value),
                                        new XElement("Status", k.Element(csntyp + "utbetstatus").Value),
                                        new XElement("Summa", k.Element(csntyp + "totbelopp").Value)),

                              new XElement("Summor",
                                    new XElement("TotalSumma",
                                            (from s in arende.Descendants(csnsm + "Utbetalning")
                                             select Int32.Parse(s.Element(csntyp + "totbelopp").Value)).Sum()),

                                    new XElement("UtbSum",
                                            (from s in arende.Descendants(csnsm + "Utbetalning")
                                             where s.Elements(csntyp + "utbetstatus").Any(u => u.Value == "Utbetald")
                                             select Int32.Parse(s.Element(csntyp + "totbelopp").Value)).Sum()),

                                    new XElement("PlanSum",
                                            (from s in arende.Descendants(csnsm + "Utbetalning")
                                             where s.Elements(csntyp + "utbetstatus").Any(u => u.Value == "Planerad")
                                             select Int32.Parse(s.Element(csntyp + "totbelopp").Value)).Sum()))));



            return View(xml);
        }


        //
        // GET: /Csn/Uppgift2

        public ActionResult Uppgift2()
        {
            return View();
        }

        //
        // GET: /Csn/Uppgift3

        public ActionResult Uppgift3()
        {
            return View();
        }
    }
}
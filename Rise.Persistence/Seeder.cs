using Rise.Domain.Machineries;
using Rise.Domain.Translations;
using Rise.Domain.Quotes;
using Rise.Domain.Customers;
using Rise.Domain.Orders;
using Rise.Domain.Inquiries;
using Rise.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using System.Text;
namespace Rise.Persistence;

public class Seeder(ApplicationDbContext dbContext)
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public void Seed()
    {

        if (!HasCategoriesSeeded())
        {
            SeedCategories();
        }

        if (!HasOptionsSeeded())
        {
            SeedOptions();
        }

        if (!HasMachineryTypeSeeded())
        {
            SeedMachineryType();
        }

        if (!HasMachineriesSeeded())
        {
            SeedMachineries();
        }

        if (!HasTranslationsSeeded())
        {
            SeedTranslations();
        }

        if (!HasCustomersSeeded())
        {
            SeedCustomers();
        }

        if (!HasImagesSeeded())
        {
            SeedImages();
        }

        if (!HasMachineryOptionsSeeded())
        {
            SeedMachineryOptions();
        }

        if (!HasQuotesSeeded())
        {
            SeedQuotes();
        }

        if (!HasQuoteOptionsSeeded())
        {
            SeedQuoteOptions();
        }

        if (!HasOrdersSeeded())
        {
            SeedOrders();
        }

        if (!HasInquiriesSeeded())
        {
            SeedInquiries();
        }

        if (!HasInquiryOptionsSeeded())
        {
            SeedInquiryOptions();
        }
        if (!HasLocationsSeeded())
        {
            SeedLocations();
        }

        if (!HasTradedMachineriesSeeded())
        {
            SeedTradedMachineries();
        }

        if (!HasTradedMachineryImagesSeeded())
        {
            SeedTradedMachineryImages();
        }
    }

    private bool HasCategoriesSeeded()
    {
        return dbContext.Categories.Any();
    }

    private bool HasOptionsSeeded()
    {
        return dbContext.Options.Any();
    }

    private bool HasMachineriesSeeded()
    {
        return dbContext.Machineries.Any();
    }

    private bool HasMachineryOptionsSeeded()
    {
        return dbContext.MachineryOptions.Any();
    }

    private bool HasMachineryTypeSeeded()
    {
        return dbContext.MachineryTypes.Any();
    }
    private bool HasImagesSeeded()
    {
        return dbContext.Images.Any();
    }
    private bool HasTranslationsSeeded()
    {
        return dbContext.Translations.Any();
    }
    private bool HasQuotesSeeded()
    {
        return dbContext.Quotes.Any();
    }
    private bool HasCustomersSeeded()
    {
        return dbContext.Customers.Any();
    }

    private bool HasQuoteOptionsSeeded()
    {
        return dbContext.QuoteOptions.Any();
    }

    private bool HasOrdersSeeded()
    {
        return dbContext.Orders.Any();
    }

    private bool HasInquiriesSeeded()
    {
        return dbContext.Inquiries.Any();
    }

    private bool HasInquiryOptionsSeeded()
    {
        return dbContext.InquiryOptions.Any();
    }

    private bool HasLocationsSeeded()
    {
        return dbContext.Location.Any();
    }

    private bool HasTradedMachineriesSeeded()
    {
        return dbContext.TradedMachineries.Any();
    }

    private bool HasTradedMachineryImagesSeeded()
    {
        return dbContext.TradedMachineryImages.Any();
    }

    private void SeedCategories()
    {
        var categories = new List<Category>
        {
            new() { Name = "Belettering / Blinderen", Code = "1200" },
            new() { Name = "Veiligheidsopties", Code = "1300" },
            new() { Name = "Comfortopties", Code = "1400" },
            new() { Name = "Prestaties en Besturing", Code = "1500" },
            new() { Name = "Bescherming en Onderhoud", Code = "1600" },
            new() { Name = "Banden en Wielen", Code = "1700" },
            new() { Name = "Hydraulische Opties", Code = "1800" },
            new() { Name = "Elektronica", Code = "1900" },
            new() { Name = "Verlichting", Code = "2000" },
            new() { Name = "Cabine Accessoires", Code = "2100" }
        };

        dbContext.Categories.AddRange(categories);
        dbContext.SaveChanges();
    }


    private void SeedOptions()
    {
        var categories = dbContext.Categories.ToList();

        if (categories.Count == 0) return;

        var options = new List<Option>
        {
        // Belettering / Blinderen (Categorie: Belettering / Blinderen)
        new() { Name = "Blinderen, Helft cabine ruiten (linker ruit achter de deur, achterruit en de helft van de rechter ruit)", Code = "BB-001", Category = categories.First(c => c.Name == "Belettering / Blinderen") },
        new() { Name = "Beletteren machine", Code = "BB-002", Category = categories.First(c => c.Name == "Belettering / Blinderen") },

        // Veiligheidsopties (Categorie: Veiligheidsopties)
        new() { Name = "ROPS (Roll Over Protection System)", Code = "VEO-001", Category = categories.First(c => c.Name == "Veiligheidsopties") },
        new() { Name = "Achteruitrijcamera", Code = "VEO-002", Category = categories.First(c => c.Name == "Veiligheidsopties") },
        new() { Name = "Extra werklampen (LED)", Code = "VEO-003", Category = categories.First(c => c.Name == "Veiligheidsopties") },

        // Comfortopties (Categorie: Comfortopties)
        new() { Name = "Airconditioning cabine", Code = "CO-001", Category = categories.First(c => c.Name == "Comfortopties") },
        new() { Name = "Geveerde stoel", Code = "CO-002", Category = categories.First(c => c.Name == "Comfortopties") },
        new() { Name = "Verwarmde spiegels", Code = "CO-003", Category = categories.First(c => c.Name == "Comfortopties") },

        // Prestaties en Besturing (Categorie: Prestaties en Besturing)
        new() { Name = "GPS/Telematica systeem", Code = "PB-001", Category = categories.First(c => c.Name == "Prestaties en Besturing") },
        new() { Name = "Smart hydraulisch systeem", Code = "PB-002", Category = categories.First(c => c.Name == "Prestaties en Besturing") },
        new() { Name = "Automatische smeersystemen", Code = "PB-003", Category = categories.First(c => c.Name == "Prestaties en Besturing") },

        // Bescherming en Onderhoud (Categorie: Bescherming en Onderhoud)
        new() { Name = "Rupsbeschermers", Code = "BO-001", Category = categories.First(c => c.Name == "Bescherming en Onderhoud") },
        new() { Name = "Cabinebescherming (FOPS)", Code = "BO-002", Category = categories.First(c => c.Name == "Bescherming en Onderhoud") },
        new() { Name = "Brandblusinstallatie", Code = "BO-003", Category = categories.First(c => c.Name == "Bescherming en Onderhoud") },

        // Banden en Wielen (Categorie: Banden en Wielen)
        new() { Name = "All-terrain banden", Code = "BW-001", Category = categories.First(c => c.Name == "Banden en Wielen") },
        new() { Name = "Run-flat banden", Code = "BW-002", Category = categories.First(c => c.Name == "Banden en Wielen") },
        new() { Name = "Velgversterking", Code = "BW-003", Category = categories.First(c => c.Name == "Banden en Wielen") },

        // Hydraulische Opties (Categorie: Hydraulische Opties)
        new() { Name = "Snelsluitsysteem", Code = "HO-001", Category = categories.First(c => c.Name == "Hydraulische Opties") },
        new() { Name = "Extra hydraulische aansluitingen", Code = "HO-002", Category = categories.First(c => c.Name == "Hydraulische Opties") },
        new() { Name = "Hydraulische oliefilter upgrade", Code = "HO-003", Category = categories.First(c => c.Name == "Hydraulische Opties") },

        // Elektronica (Categorie: Elektronica)
        new() { Name = "Digitale bediening (touchscreen)", Code = "EL-001", Category = categories.First(c => c.Name == "Elektronica") },
        new() { Name = "Stroomaggregaat aansluiting", Code = "EL-002", Category = categories.First(c => c.Name == "Elektronica") },
        new() { Name = "WiFi module", Code = "EL-003", Category = categories.First(c => c.Name == "Elektronica") },

        // Verlichting (Categorie: Verlichting)
        new() { Name = "LED-hoofdverlichting", Code = "VL-001", Category = categories.First(c => c.Name == "Verlichting") },
        new() { Name = "Extra werkverlichting", Code = "VL-002", Category = categories.First(c => c.Name == "Verlichting") },
        new() { Name = "Omgevingsverlichting (360 graden)", Code = "VL-003", Category = categories.First(c => c.Name == "Verlichting") },

        // Cabine Accessoires (Categorie: Cabine Accessoires)
        new() { Name = "Bluetooth radio", Code = "CA-001", Category = categories.First(c => c.Name == "Cabine Accessoires") },
        new() { Name = "Opbergvakken met slot", Code = "CA-002", Category = categories.First(c => c.Name == "Cabine Accessoires") },
        new() { Name = "Koelkast cabine", Code = "CA-003", Category = categories.First(c => c.Name == "Cabine Accessoires") }
        };

        dbContext.Options.AddRange(options);
        dbContext.SaveChanges();
    }


    private void SeedMachineries()
    {

        var types = dbContext.MachineryTypes.ToList();

        if (types.Count == 0) return;

        var machineries = new List<Machinery>
    {
        new()
        {
            Name = "Caterpillar 320",
            SerialNumber = "CAT320-202310001",
            Type = types.First(t => t.Id == 1),
            Description = "Caterpillar 320 - Graafmachine",
            BrochureText = "De Caterpillar 320 graafmachine is ontworpen voor optimale efficiëntie en prestaties. Met verbeterde brandstofzuinigheid en krachtige hydraulica is dit model ideaal voor zware graafwerkzaamheden.",
        },
        new()
        {
            Name = "Komatsu PC210",
            SerialNumber = "KOMPC210-202310002",
            Type = types.First(t => t.Id == 1),
            Description = "Komatsu PC210 - Graafmachine",
            BrochureText = "De Komatsu PC210 biedt hoge productiviteit met een milieuvriendelijke motor en verbeterde graafkracht. Ideaal voor projecten die betrouwbaarheid en precisie vereisen.",
        },
        new()
        {
            Name = "Hitachi ZX350LC-6",
            SerialNumber = "HITZX350LC6-202310003",
            Type = types.First(t => t.Id == 1),
            Description = "Hitachi ZX350LC-6 - Graafmachine",
            BrochureText = "Hitachi ZX350LC-6 combineert kracht en controle voor grote graafprojecten. Uitgerust met duurzame componenten voor langdurige prestaties in veeleisende omstandigheden.",
        },
        new()
        {
            Name = "Volvo EC220E",
            SerialNumber = "VOLEC220E-202310004",
            Type = types.First(t => t.Id == 1),
            Description = "Volvo EC220E - Graafmachine",
            BrochureText = "De Volvo EC220E is efficiënt en duurzaam, ontworpen met het oog op brandstofbesparing en verminderde uitstoot, zonder concessies te doen aan prestaties.",
        },
        new()
        {
            Name = "JCB JS160",
            SerialNumber = "JCBJS160-202310005",
            Type = types.First(t => t.Id == 1),
            Description = "JCB JS160 - Graafmachine",
            BrochureText = "De JCB JS160 biedt een unieke combinatie van kracht en nauwkeurigheid, perfect voor veelzijdige graafprojecten. Gebouwd voor comfort en efficiëntie.",
        },
        new()
        {
            Name = "Liebherr LTM 11200-9.1",
            SerialNumber = "LIEBLTM11200-202310006",
            Type = types.First(t => t.Id == 2),
            Description = "Liebherr LTM 11200-9.1 - Kraan",
            BrochureText = "De Liebherr LTM 11200-9.1 is 's werelds krachtigste mobiele kraan, uitgerust met een telescopische arm die grote hoogtes en gewichten aankan.",
        },
        new()
        {
            Name = "Kobelco CKE2500G",
            SerialNumber = "KOBCKE2500G-202310007",
            Type = types.First(t => t.Id == 2),
            Description = "Kobelco CKE2500G - Kraan",
            BrochureText = "De Kobelco CKE2500G biedt uitzonderlijke hijscapaciteit en mobiliteit. Dit model is perfect voor grote bouwprojecten met hoge eisen aan kracht en veiligheid.",
        },
        new()
        {
            Name = "Caterpillar 950M",
            SerialNumber = "CAT950M-202310008",
            Type = types.First(t => t.Id == 3),
            Description = "Caterpillar 950M - Wiellader",
            BrochureText = "De Caterpillar 950M wiellader staat bekend om zijn veelzijdigheid en kracht, ideaal voor transport en laadwerkzaamheden in verschillende omgevingen.",
        },
        new()
        {
            Name = "Volvo L120H",
            SerialNumber = "VOLL120H-202310009",
            Type = types.First(t => t.Id == 3),
            Description = "Volvo L120H - Wiellader",
            BrochureText = "De Volvo L120H combineert kracht met geavanceerde technologie, ontworpen voor meer efficiëntie en lagere operationele kosten.",
        },
        new()
        {
            Name = "JCB 426 ZX",
            SerialNumber = "JCB426ZX-202310010",
            Type = types.First(t => t.Id == 3),
            Description = "JCB 426 ZX - Wiellader",
            BrochureText = "De JCB 426 ZX biedt robuuste prestaties en lage operationele kosten. Ideaal voor zware ladingen in de bouw en landbouw.",
        },
        new()
        {
            Name = "Caterpillar D8T",
            SerialNumber = "CATD8T-202310011",
            Type = types.First(t => t.Id == 4),
            Description = "Caterpillar D8T - Bulldozer",
            BrochureText = "De Caterpillar D8T bulldozer levert buitengewone kracht en duurzaamheid, geschikt voor zware grondverzetprojecten.",
        },
        new()
        {
            Name = "John Deere 850K",
            SerialNumber = "JD850K-202310012",
            Type = types.First(t => t.Id == 4),
            Description = "John Deere 850K - Bulldozer",
            BrochureText = "John Deere 850K biedt geavanceerde bediening en een krachtige motor, ontworpen om de zwaarste klussen aan te kunnen.",
        },
        new()
        {
            Name = "Liebherr PR 736",
            SerialNumber = "LIEBPR736-202310013",
            Type = types.First(t => t.Id == 4),
            Description = "Liebherr PR 736 - Bulldozer",
            BrochureText = "De Liebherr PR 736 biedt uitzonderlijke stabiliteit en kracht voor intensieve grondverzetwerkzaamheden.",
        },
        new()
        {
            Name = "Linde H50",
            SerialNumber = "LINH50-202310014",
            Type = types.First(t => t.Id == 5),
            Description = "Linde H50 - Heftruck",
            BrochureText = "De Linde H50 heftruck biedt stabiliteit en precisie, ontworpen voor het tillen van zware lasten met veiligheid en gemak.",
        },
        new()
        {
            Name = "Toyota 8FGCU25",
            SerialNumber = "TOY8FGCU25-202310015",
            Type = types.First(t => t.Id == 5),
            Description = "Toyota 8FGCU25 - Heftruck",
            BrochureText = "De Toyota 8FGCU25 is compact en wendbaar, ideaal voor gebruik in magazijnen en smalle ruimtes.",
        },
        new()
        {
            Name = "Caterpillar DP50N",
            SerialNumber = "CATDP50N-202310016",
            Type = types.First(t => t.Id == 5),
            Description = "Caterpillar DP50N - Heftruck",
            BrochureText = "De Caterpillar DP50N biedt een combinatie van duurzaamheid en kracht, geschikt voor zware industriële toepassingen.",
        },
        new()
        {
            Name = "Liebherr LTM 1230",
            SerialNumber = "LTM1230-202310002",
            Type = types.First(t => t.Name == "Kraan"),
            Description = "Liebherr LTM 1230 - Kraan",
            BrochureText = "De Liebherr LTM 1230 kraan biedt hoge mobiliteit en uitzonderlijke hijscapaciteiten. Perfect voor veelzijdige hijstoepassingen."
        },
        new()
        {
            Name = "CAT 950M",
            SerialNumber = "CAT950M-202310003",
            Type = types.First(t => t.Name == "Wiellader"),
            Description = "CAT 950M - Wiellader",
            BrochureText = "De CAT 950M wiellader is een krachtig en efficiënt laadvoertuig, geschikt voor zware toepassingen."
        },
        new()
        {
            Name = "Komatsu D61PX",
            SerialNumber = "D61PX-202310004",
            Type = types.First(t => t.Name == "Bulldozer"),
            Description = "Komatsu D61PX - Bulldozer",
            BrochureText = "De Komatsu D61PX bulldozer biedt optimale prestaties in uitdagende terreinen, met een geavanceerd tractiesysteem."
        },
        new()
        {
            Name = "Toyota 8FGU25",
            SerialNumber = "8FGU25-202310005",
            Type = types.First(t => t.Name == "Heftruck"),
            Description = "Toyota 8FGU25 - Heftruck",
            BrochureText = "De Toyota 8FGU25 heftruck staat bekend om zijn betrouwbaarheid en gebruiksgemak, ideaal voor magazijntoepassingen."
        },
        new()
        {
            Name = "Bobcat S650",
            SerialNumber = "S650-202310006",
            Type = types.First(t => t.Name == "Schranklader"),
            Description = "Bobcat S650 - Schranklader",
            BrochureText = "De Bobcat S650 schranklader is veelzijdig en efficiënt, geschikt voor uiteenlopende taken op de bouwplaats."
        },
        new()
        {
            Name = "Bell B30E",
            SerialNumber = "B30E-202310007",
            Type = types.First(t => t.Name == "Dumper"),
            Description = "Bell B30E - Dumper",
            BrochureText = "De Bell B30E dumper combineert capaciteit met brandstofefficiëntie, ideaal voor zwaar transportwerk."
        },
        new()
        {
            Name = "HAMM HD+ 120",
            SerialNumber = "HD120-202310008",
            Type = types.First(t => t.Name == "Wals"),
            Description = "HAMM HD+ 120 - Wals",
            BrochureText = "De HAMM HD+ 120 wals biedt krachtige triltechnologie voor optimale asfaltverdichting."
        },
        new()
        {
            Name = "CIFA Magnum MK28L",
            SerialNumber = "MK28L-202310009",
            Type = types.First(t => t.Name == "Betonmixer"),
            Description = "CIFA Magnum MK28L - Betonmixer",
            BrochureText = "De CIFA Magnum MK28L is een veelzijdige betonmixer met uitstekende prestaties op de bouwplaats."
        },
        new()
        {
            Name = "Schwing SP 500",
            SerialNumber = "SP500-202310010",
            Type = types.First(t => t.Name == "Betonpomp"),
            Description = "Schwing SP 500 - Betonpomp",
            BrochureText = "De Schwing SP 500 betonpomp is ideaal voor het verpompen van beton over lange afstanden en op hoogte."
        },
        new()
        {
            Name = "Junttan PMx22",
            SerialNumber = "PMx22-202310011",
            Type = types.First(t => t.Name == "Palenrammer"),
            Description = "Junttan PMx22 - Palenrammer",
            BrochureText = "De Junttan PMx22 palenrammer biedt precisie en kracht, geschikt voor funderingswerken."
        },
        new()
        {
            Name = "Volvo EC750E HR",
            SerialNumber = "EC750EHR-202310012",
            Type = types.First(t => t.Name == "Sloopgraafmachine"),
            Description = "Volvo EC750E HR - Sloopgraafmachine",
            BrochureText = "De Volvo EC750E HR is ontworpen voor zware sloopwerken met een hoge reikwijdte."
        },
        new()
        {
            Name = "Herrenknecht TBM 800",
            SerialNumber = "TBM800-202310013",
            Type = types.First(t => t.Name == "Tunneldrilmachine"),
            Description = "Herrenknecht TBM 800 - Tunneldrilmachine",
            BrochureText = "De Herrenknecht TBM 800 is een innovatieve oplossing voor tunnelboringen in diverse grondsoorten."
        },
        new()
        {
            Name = "Atlas Copco QAS 150",
            SerialNumber = "QAS150-202310014",
            Type = types.First(t => t.Name == "Generator"),
            Description = "Atlas Copco QAS 150 - Generator",
            BrochureText = "De Atlas Copco QAS 150 biedt betrouwbare stroomvoorziening, ideaal voor bouwlocaties."
        }
    };

        dbContext.Machineries.AddRange(machineries);
        dbContext.SaveChanges();
    }

    private void SeedImages()
    {
        var machineries = dbContext.Machineries.ToList();

        // Example dictionary with machinery names mapped to Azure Blob URLs
        var imageUrls = new Dictionary<string, List<string>>
        {
            { "Caterpillar 320", new List<string>
                {
                  "https://risea02.blob.core.windows.net/machinery/Catepillar%20320/Catepillar_320_1.jpg",
                  "https://risea02.blob.core.windows.net/machinery/Catepillar%20320/Catepillar_320_2.jpg",
                  "https://risea02.blob.core.windows.net/machinery/Catepillar%20320/Catepillar_320_3.jpg",
                  "https://risea02.blob.core.windows.net/machinery/Catepillar%20320/Catepillar_320_4.jpg"
                }
            },
            { "Caterpillar 950M", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20950M/Caterpillar_950M_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20950M/Caterpillar_950M_2.png"
                }
            },
            { "Caterpillar D8T", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20D8T/Caterpillar_D8T_1.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20D8T/Caterpillar_D8T_2.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20D8T/Caterpillar_D8T_3.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20D8T/Caterpillar_D8T_4.jpg"
                }
            },
            { "Caterpillar DP50N", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20DP50N/Caterpillar_DP50N_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Caterpillar%20DP50N/Caterpillar_DP50N_2.png"
                }
            },
            { "Hitachi ZX350LC-6", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Hitachi%20ZX350LC-6/Hitachi_ZX350LC-6_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Hitachi%20ZX350LC-6/Hitachi_ZX350LC-6_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Hitachi%20ZX350LC-6/Hitachi_ZX350LC-6_3.png",
                }
            },
            { "JCB 426 ZX", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/JCB%20426%20ZX/JCB_426_ZX_1.png",
                    "https://risea02.blob.core.windows.net/machinery/JCB%20426%20ZX/JCB_426_ZX_2.png",
                    "https://risea02.blob.core.windows.net/machinery/JCB%20426%20ZX/JCB_426_ZX_3.png",
                }
            },
            { "JCB JS160", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/JCB%20JS160/JCB_JS160_1.png",
                    "https://risea02.blob.core.windows.net/machinery/JCB%20JS160/JCB_JS160_2.png",
                    "https://risea02.blob.core.windows.net/machinery/JCB%20JS160/JCB_JS160_3.png",
                }
            },
            { "John Deere 850K", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/John%20Deere%20850K/John_Deere_850K_1.png",
                    "https://risea02.blob.core.windows.net/machinery/John%20Deere%20850K/John_Deere_850K_2.jpg",
                }
            },
            { "Kobelco CKE2500G", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Kobelco%20CKE2500G/Kobelco_CKE2500G_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Kobelco%20CKE2500G/Kobelco_CKE2500G_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Kobelco%20CKE2500G/Kobelco_CKE2500G_3.png",
                    "https://risea02.blob.core.windows.net/machinery/Kobelco%20CKE2500G/Kobelco_CKE2500G_4.png"
                }
            },
            { "Komatsu PC210", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20PC210/Komatsu_PC210_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20PC210/Komatsu_PC210_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20PC210/Komatsu_PC210_3.png",
                }
            },
            { "Liebherr LTM 11200-9.1", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%2011200-9.1/Liebherr_LTM_11200-9.1_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%2011200-9.1/Liebherr_LTM_11200-9.1_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%2011200-9.1/Liebherr_LTM_11200-9.1_3.png",
                }
            },
            { "Liebherr PR 736", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20PR%20736/Liebherr_PR_736_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20PR%20736/Liebherr_PR_736_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20PR%20736/Liebherr_PR_736_3.png"
                }
            },
            { "Linde H50", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Linde%20H50/Linde_H50%20_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Linde%20H50/Linde_H50%20_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Linde%20H50/Linde_H50%20_3.png"
                }
            },
            { "Toyota 8FGCU25", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGCU25/Toyota_8FGCU25_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGCU25/Toyota_8FGCU25_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGCU25/Toyota_8FGCU25_3.png",
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGCU25/Toyota_8FGCU25_4.png"
                    }
            },
            { "Volvo EC220E", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC220E/Volvo_EC220E_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC220E/Volvo_EC220E_2.png",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC220E/Volvo_EC220E_3.png"
                }
            },
            { "Volvo L120H", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20L120H/Volvo_L120H_1.png",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20L120H/Volvo_L120H_2.jpg"
                }
            },
            { "Bell B30E", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Bell%20B30E/Bell-B30E-media1.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Bell%20B30E/images.jpeg",
                    "https://risea02.blob.core.windows.net/machinery/Bell%20B30E/kiepwagens-b30e-bell(1).jpeg"
                }
            },
            { "Bobcat S650", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Bobcat%20S650/bobcat-s650-m3-156088-t6k5037-11w37-3840x2560.webp",
                    "https://risea02.blob.core.windows.net/machinery/Bobcat%20S650/compacte-lader-s-650-bobcat(21).jpg",
                    "https://risea02.blob.core.windows.net/machinery/Bobcat%20S650/images.jpeg"
                }
            },
            { "CAT 950M", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/CAT%20950M/2276-CAT-950M-0317-NW2.jpg",
                    "https://risea02.blob.core.windows.net/machinery/CAT%20950M/C10755254.jpg",
                    "https://risea02.blob.core.windows.net/machinery/CAT%20950M/CM20161004-35471-47857.jpg",
                    "https://risea02.blob.core.windows.net/machinery/CAT%20950M/images.jpeg"
                }
            },
            { "CIFA Magnum MK28L", new List<string>
                {
                   "https://risea02.blob.core.windows.net/machinery/CIFA%20Magnum%20MK28L/-MK32L%20(2)%20-%20Copia.JPG",
                   "https://risea02.blob.core.windows.net/machinery/CIFA%20Magnum%20MK28L/50140-13649771.jpg"
                }
            },
            { "HAMM HD+ 120", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/HAMM%20HD+%20120/o5573v80_H259_HD_120_VV_KAB_tr_Kopie.png",
                    "https://risea02.blob.core.windows.net/machinery/HAMM%20HD+%20120/45297480_1729092881_843.jpg",
                    "https://risea02.blob.core.windows.net/machinery/HAMM%20HD+%20120/Hamm_HD__360870188f.jpg",
                    "https://risea02.blob.core.windows.net/machinery/HAMM%20HD+%20120/m00088-01.jpg"
                }
            },
            { "Herrenknecht TBM 800", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Herrenknecht%20TBM%20800/02-bild04-dillinger-c-herrenknecht_800_533-340faee6ab03a895.jpg"
                }
            },
            { "Junttan PMx22", new List<string>
                {
                   "https://risea02.blob.core.windows.net/machinery/Junttan%20PMx22/Junttan-PMx22-piling-rig_Skanska-Finland-1024x492-1.jpg",
                   "https://risea02.blob.core.windows.net/machinery/Junttan%20PMx22/Junttan-piling-rig-PMx22-easy-transportation_Skanska-Finland-2.jpg",
                   "https://risea02.blob.core.windows.net/machinery/Junttan%20PMx22/junttan-pmx22-pile-driving-rig-aarsleff-uk_540x540.jpg"
                }
            },
            { "Komatsu D61PX", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20D61PX/44573189_1709829301_990.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20D61PX/D61EXPX_24_04.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20D61PX/download.jpeg",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20D61PX/images%20(1).jpeg",
                    "https://risea02.blob.core.windows.net/machinery/Komatsu%20D61PX/images.jpeg"
                }
            },
            { "Liebherr LTM 1230", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%201230/1189b318-54c6-4622-926c-6500b2269df7_web.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%201230/aa8dc8b8-c1da-4a4f-be75-fdff1f420bd1_web.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Liebherr%20LTM%201230/liebherr-ltm-1230-5-1-hotspot-stage.jpg"
                }
            },
            { "Atlas Copco QAS 150", new List<string>
                {
                   "https://risea02.blob.core.windows.net/machinery/Atlas%20Copco%20QAS%20150/11950724-01.jpg",
                   "https://risea02.blob.core.windows.net/machinery/Atlas%20Copco%20QAS%20150/20230823_144605.jpg",
                   "https://risea02.blob.core.windows.net/machinery/Atlas%20Copco%20QAS%20150/Ex-rental-Atlas-Copco-QAS-150-stroomgenerator_Euro-Rent-Machineverhuur.007.jpg",
                   "https://risea02.blob.core.windows.net/machinery/Atlas%20Copco%20QAS%20150/G23060214.jpg"
                }
            },
            { "Schwing SP 500", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Schwing%20SP%20500/71778-17625487.webp",
                    "https://risea02.blob.core.windows.net/machinery/Schwing%20SP%20500/PXL_20240216_220328412_page-0001.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Schwing%20SP%20500/csm_sp_500d_720d4d7ead.webp",
                    "https://risea02.blob.core.windows.net/machinery/Schwing%20SP%20500/schwing-SP500.1.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Schwing%20SP%20500/sp_500e.webp",
                }
            },
            { "Toyota 8FGU25", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGU25/3.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGU25/8fgu25.png",
                    "https://risea02.blob.core.windows.net/machinery/Toyota%208FGU25/images%20(2).jpeg"
                }
            },
            { "Volvo EC750E HR", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC750E%20HR/ec750ehr-demolition-undercarriage.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC750E%20HR/images.jpeg",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC750E%20HR/media_post_c3u4pzq_volvo-ec750e_hr.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC750E%20HR/rupsgraafmachines-ec750e-hr-volvo.jpg",
                    "https://risea02.blob.core.windows.net/machinery/Volvo%20EC750E%20HR/volvo-benefits-crawler-excavator-ec480ehr-t4f-volvo-patented-modular-boom-system-2324x1200.jpg"
                }
            }
        };

        // Prepare list for new Image entities
        var images = new List<Image>();

        foreach (var machinery in machineries)
        {
            // Check if URLs exist for this machinery in the dictionary
            if (imageUrls.TryGetValue(machinery.Name, out var urls))
            {
                images.AddRange(urls.Select(url => new Image
                {
                    Url = url,
                    Machinery = machinery
                }));
            }
        }

        dbContext.Images.AddRange(images);
        dbContext.SaveChanges();
    }

    private void SeedMachineryOptions()
    {
        var machineries = dbContext.Machineries.ToList();
        var options = dbContext.Options.ToList();

        if (machineries.Count == 0 || options.Count == 0) return;

        var machineryOptions = new List<MachineryOption>();
        var random = new Random();

        foreach (var machinery in machineries)
        {
            int numberOfOptionsToAssign = random.Next(1, 4);

            var selectedOptions = options.OrderBy(x => random.Next()).Take(numberOfOptionsToAssign).ToList();

            foreach (var option in selectedOptions)
            {
                machineryOptions.Add(new MachineryOption
                {
                    Machinery = machinery,
                    Option = option,
                    Price = 100.00m + (decimal)random.Next(1, 100),
                });
            }
        }

        dbContext.MachineryOptions.AddRange(machineryOptions);
        dbContext.SaveChanges();
    }

    private void SeedMachineryType()
    {
        var machineryTypes = new List<MachineryType>
    {
        new() { Name = "Graafmachine" },
        new() { Name = "Kraan" },
        new() { Name = "Wiellader" },
        new() { Name = "Bulldozer" },
        new() { Name = "Heftruck" },
        new() { Name = "Schranklader" },
        new() { Name = "Dumper" },
        new() { Name = "Wals" },
        new() { Name = "Betonmixer" },
        new() { Name = "Betonpomp" },
        new() { Name = "Palenrammer" },
        new() { Name = "Sloopgraafmachine" },
        new() { Name = "Tunneldrilmachine" },
        new() { Name = "Generator" }
    };

        dbContext.MachineryTypes.AddRange(machineryTypes);
        dbContext.SaveChanges();
    }

    private void SeedTranslations()
    {
        var translations = new List<Translation>
        {
            new() {OriginalText = "Grundgerät", TranslatedText = "Basisapparaat", IsAccepted =  true, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Dieselpartikelfilter", TranslatedText = "Dieselroetfilter", IsAccepted =  true, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Oberwagen", TranslatedText = "Bovenwagen", IsAccepted =  true, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Rohrbruchsicherung", TranslatedText = "Pijpbreukbeveiliging", IsAccepted =  true, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Unterwagen", TranslatedText = "Onderstel", IsAccepted =  true, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Klimaautomatik", TranslatedText = "Automatische klimaatregeling", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Standardverpackung", TranslatedText = "Standaardverpakking", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Arbeitsbremse", TranslatedText = "Werkrem", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Überlastwarneinrichtung", TranslatedText = "Waarschuwingsapparaat voor overbelasting", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Traglasttabelle", TranslatedText = "Tabel draagvermogen", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Zylinder", TranslatedText = "Cilinder", IsAccepted = false, UserEmail = "thomas.vandamme@dozer.be" },
            new() {OriginalText = "Schmiermittel", TranslatedText = "Smeermiddel", IsAccepted = false, UserEmail = "thomas.vandamme@dozer.be" },
            new() {OriginalText = "Drehkranz", TranslatedText = "Draaikrans", IsAccepted = false, UserEmail = "thomas.vandamme@dozer.be" },
            new() {OriginalText = "Fahrerkabine", TranslatedText = "Bestuurderscabine", IsAccepted = false, UserEmail = "laura.peeters@dozer.be" },
            new() {OriginalText = "Gegenballast", TranslatedText = "Tegenwicht", IsAccepted = false, UserEmail = "laura.peeters@dozer.be" },
            new() {OriginalText = "Hydraulikpumpe", TranslatedText = "Hydraulische pomp", IsAccepted = false, UserEmail = "laura.peeters@dozer.be" },
            new() {OriginalText = "Raupenketten", TranslatedText = "Rupskettingen", IsAccepted = false, UserEmail = "laura.peeters@dozer.be" },
            new() {OriginalText = "Kühler", TranslatedText = "Koeler", IsAccepted = false, UserEmail = "mike.janssens@dozer.be" },
            new() {OriginalText = "Kraftstofftank", TranslatedText = "Brandstoftank", IsAccepted = false, UserEmail = "mike.janssens@dozer.be" },
            new() {OriginalText = "Schaufel", TranslatedText = "Schep", IsAccepted = false, UserEmail = "mike.janssens@dozer.be" }
        };

        dbContext.Translations.AddRange(translations);
        dbContext.SaveChanges();
    }

    private void SeedCustomers()
    {
        var customers = new List<Customer>
        {
            new() { Name = "John Doe", Street = "Straat", StreetNumber = "1", City = "Aalst", PostalCode = "9300", Country = "België" },
            new() { Name = "Jane Doe", Street = "AndereStraat", StreetNumber = "2", City = "Brussel", PostalCode = "1000", Country = "België" },
            new() { Name = "Jack Doe", Street = "NogEenStraat", StreetNumber = "3", City = "Gent", PostalCode = "9000", Country = "België" },
            new() { Name = "Jill Doe", Street = "LaatsteStraat", StreetNumber = "4", City = "Antwerpen", PostalCode = "2000", Country = "België" },
            new() { Name = "Michael Smith", Street = "LangeStraat", StreetNumber = "5", City = "Leuven", PostalCode = "3000", Country = "België" },
            new() { Name = "Emily Johnson", Street = "KorteStraat", StreetNumber = "6", City = "Brugge", PostalCode = "8000", Country = "België" },
            new() { Name = "David Brown", Street = "Veldstraat", StreetNumber = "7", City = "Kortrijk", PostalCode = "8500", Country = "België" },
            new() { Name = "Sarah Miller", Street = "Dorpstraat", StreetNumber = "8", City = "Mechelen", PostalCode = "2800", Country = "België" },
            new() { Name = "Chris Wilson", Street = "Zonstraat", StreetNumber = "9", City = "Oostende", PostalCode = "8400", Country = "België" },
            new() { Name = "Laura Taylor", Street = "Kerkstraat", StreetNumber = "10", City = "Hasselt", PostalCode = "3500", Country = "België" }
        };

        dbContext.Customers.AddRange(customers);
        dbContext.SaveChanges();
    }

    private void SeedQuotes()
    {
        var customers = dbContext.Customers.ToList();
        var machineries = dbContext.Machineries.ToList();
        var random = new Random();
        const decimal vatRate = 0.21m;

        if (customers.Count == 0 || machineries.Count == 0) return;

        // List of SalespersonIds to use for generating quotes
        var salespersonIds = new List<string>
    {
        "auth0|675056d07f73e97294453edf",
        "auth0|6745d36a2c52c4ecbcabf607",
        "auth0|6750569540af06fbdfba49aa",
        "auth0|673c7d2a1dbe2a5b8098cbe6",
        "auth0|67505615ba0a33728f377e58",
        "auth0|675055cdba0a33728f377e0e",
        "auth0|673b73fa1e5482be2e942b7f"
    };

        var quotes = new List<Quote>();

        // Generating 3 quotes for each salesperson
        for (int i = 1; i <= 3; i++)
        {
            foreach (var salespersonId in salespersonIds)
            {
                var selectedMachinery = machineries[random.Next(machineries.Count)];
                var selectedCustomer = customers[random.Next(customers.Count)];

                var totalWithoutVat = random.Next(50000, 500000);
                var totalWithVat = totalWithoutVat * (1 + vatRate);
                var randomString = GenerateRandomString(8);

                var quote = new Quote
                {
                    QuoteNumber = $"20231010{randomString}-1",
                    IsApproved = i % 2 == 0,
                    Date = DateTime.Now.AddDays(-random.Next(0, 365)),
                    Customer = selectedCustomer!,
                    Machinery = selectedMachinery,
                    TotalWithoutVat = totalWithoutVat,
                    TotalWithVat = totalWithVat,
                    BasePrice = totalWithoutVat,
                    MainOptions = "Basis:John Deere 850K:Volautomatisch centraal smeersysteem, bovenwagen en uitrusting, 4 kg (exclusief kinematiek):Inklapbare stuurconsole:Automatische bedrijfsrem:Camera aan de achterzijde van de machine;Onderstel:Stempels achterzijde, schild voorzijde, 2550 mm breed:Zuigerstangbescherming achterste klauwen en voorste schild:Speeder 30 km/h:Hydraulische aansluiting voor het laten kippen aanhanger (enkelwerkend):Camera aan de rechterkant;Cabine:Radio Comfort:Comfort stoel:Dakraamwisser:Handsteun (verhoging) voor joystick;Uitrusting:Verstelgiek 5,05 m:Lekolieleiding voor aanbouwdelen;Algemeenheden:Gebruikshandleiding duits + nederlands:Losse stickers",
                    SalespersonId = salespersonId,
                };

                quotes.Add(quote);
            }
        }

        dbContext.Quotes.AddRange(quotes);
        dbContext.SaveChanges();
    }

    private string GenerateRandomString(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(validChars[random.Next(validChars.Length)]);
        }

        return result.ToString();
    }

    private void SeedQuoteOptions()
    {
        var quotes = dbContext.Quotes.Include(x => x.Machinery).ToList();
        var machineryOptions = dbContext.MachineryOptions.Include(x => x.Machinery).ToList();
        var random = new Random();

        if (quotes.Count == 0 || machineryOptions.Count == 0) return;

        var quoteOptions = new List<QuoteOption>();

        foreach (var quote in quotes)
        {
            var applicableOptions = machineryOptions
                .Where(mo => mo.Machinery.Id == quote.Machinery.Id)
                .ToList();

            if (applicableOptions.Count == 0) continue;

            int numberOfOptionsToAssign = random.Next(1, Math.Min(3, applicableOptions.Count) + 1);
            var selectedOptions = applicableOptions
                .OrderBy(_ => random.Next())
                .Take(numberOfOptionsToAssign)
                .ToList();

            foreach (var option in selectedOptions)
            {
                quoteOptions.Add(new QuoteOption
                {
                    Quote = quote,
                    MachineryOption = option
                });
            }
        }

        dbContext.QuoteOptions.AddRange(quoteOptions);
        dbContext.SaveChanges();
    }

    private void SeedOrders()
    {
        var customer = dbContext.Customers.FirstOrDefault();
        var machinery = dbContext.Machineries.FirstOrDefault();
        var quotes = dbContext.Quotes.Where(x => x.IsApproved).ToList();
        var orders = new List<Order>();

        foreach (var quote in quotes)
        {
            var random = new Random();
            string orderNumber = quote.QuoteNumber.Split('-')[0];

            var order = new Order
            {
                OrderNumber = orderNumber,
                Quote = quote,
                Date = quote.Date.AddDays(random.Next(1, 30)),
                IsCancelled = false
            };
            orders.Add(order);
        }

        dbContext.Orders.AddRange(orders);
        dbContext.SaveChanges();
    }

    private void SeedLocations()
    {
        var locations = new List<Location> {

        new Location
        {
            Name = "Amsterdam",
            Street = "Hoofdstraat",
            StreetNumber = "123",
            City = "Amsterdam",
            PostalCode = "1011AB",
            Country = "Nederland",
            Image = "https://risea02.blob.core.windows.net/machinery/Locations/Designer%20(4).png",
            PhoneNumber = "+31-20-1234567",
            VatNumber = "NL123456789B01",
            Code = "BIB001"
        },
        new Location
        {
            Name = "Rotterdam",
            Street = "Parklaan",
            StreetNumber = "456",
            City = "Rotterdam",
            PostalCode = "3012CD",
            Country = "Nederland",
            Image = "https://risea02.blob.core.windows.net/machinery/Locations/Designer.png",
            PhoneNumber = "+31-10-7654321",
            VatNumber = "NL987654321B01",
            Code = "PARK001"
        },
        new Location
        {
            Name = "Eindhoven",
            Street = "Innovatieweg",
            StreetNumber = "789",
            City = "Eindhoven",
            PostalCode = "5612AB",
            Country = "Nederland",
            Image = "https://risea02.blob.core.windows.net/machinery/Locations/Designer%20(2).png",
            PhoneNumber = "+31-40-1234567",
            VatNumber = "NL564738291B01",
            Code = "TECH001"
        },
        new Location
        {
            Name = "Arnhem",
            Street = "Boslaan",
            StreetNumber = "22",
            City = "Arnhem",
            PostalCode = "6811KL",
            Country = "Nederland",
            Image = "https://risea02.blob.core.windows.net/machinery/Locations/Designer%20(1).png",
            PhoneNumber = "+31-26-9876543",
            VatNumber = "NL987654123B02",
            Code = "RESORT001"
        },
        new Location
        {
            Name = "Den Haag",
            Street = "Kadeboulevard",
            StreetNumber = "10",
            City = "Den Haag",
            PostalCode = "2511BB",
            Country = "Nederland",
            Image = "https://risea02.blob.core.windows.net/machinery/Locations/Designer%20(3).png",
            PhoneNumber = "+31-70-5554321",
            VatNumber = "NL123098765B03",
            Code = "CAFE001"
        }
    };
        dbContext.Location.AddRange(locations);
        dbContext.SaveChanges();
    }

    private void SeedTradedMachineries()
    {
        var machineryTypes = dbContext.MachineryTypes.ToList();
        var quotes = dbContext.Quotes.Where(x => x.IsApproved).ToList();

        if (machineryTypes.Count == 0 || quotes.Count == 0) return;

        var random = new Random();
        var tradedMachineries = new List<TradedMachinery>
        {
            new()
        {
            Name = "Caterpillar 320",
            SerialNumber = "CAT320-202310001",
            Type = machineryTypes.First(),
            Description = "Caterpillar 320 - Graafmachine",
            EstimatedValue = 12000m,
            Year = 2020,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Komatsu PC210",
            SerialNumber = "KOMPC210-202310002",
            Type = machineryTypes.First(),
            Description = "Komatsu PC210 - Graafmachine",
            EstimatedValue = 11000m,
            Year = 2019,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Hitachi ZX350LC-6",
            SerialNumber = "HITZX350-202310003",
            Type = machineryTypes.First(),
            Description = "Hitachi ZX350LC-6 - Graafmachine",
            EstimatedValue = 13000m,
            Year = 2021,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Volvo EC950F",
            SerialNumber = "VOLVOEC950-202310004",
            Type = machineryTypes.First(),
            Description = "Volvo EC950F - Graafmachine",
            EstimatedValue = 14000m,
            Year = 2022,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Liebherr R 9800",
            SerialNumber = "LIEBHERRR9800-202310005",
            Type = machineryTypes.First(),
            Description = "Liebherr R 9800 - Graafmachine",
            EstimatedValue = 15000m,
            Year = 2023,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "JCB JS220",
            SerialNumber = "JCBJS220-202310006",
            Type = machineryTypes.First(),
            Description = "JCB JS220 - Graafmachine",
            EstimatedValue = 10000m,
            Year = 2018,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Doosan DX340LCA",
            SerialNumber = "DOOSANDX340-202310007",
            Type = machineryTypes.First(),
            Description = "Doosan DX340LCA - Graafmachine",
            EstimatedValue = 12500m,
            Year = 2020,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Hyundai R500LC-9A",
            SerialNumber = "HYUNDAIR500-202310008",
            Type = machineryTypes.First(),
            Description = "Hyundai R500LC-9A - Graafmachine",
            EstimatedValue = 13500m,
            Year = 2021,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "CASE CX750D",
            SerialNumber = "CASECX750-202310009",
            Type = machineryTypes.First(),
            Description = "CASE CX750D - Graafmachine",
            EstimatedValue = 14500m,
            Year = 2022,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "SANY SY500H",
            SerialNumber = "SANYSY500-202310010",
            Type = machineryTypes.First(),
            Description = "SANY SY500H - Graafmachine",
            EstimatedValue = 15500m,
            Year = 2023,
            Quote = quotes[random.Next(quotes.Count)],
        },
        new()
        {
            Name = "Kubota KX080-4",
            SerialNumber = "KUBOTAKX080-202310011",
            Type = machineryTypes.First(),
            Description = "Kubota KX080-4 - Graafmachine",
            EstimatedValue = 11000m,
            Year = 2018,
            Quote = quotes[random.Next(quotes.Count)],
        }
        };
        dbContext.TradedMachineries.AddRange(tradedMachineries);
        dbContext.SaveChanges();
    }

    private void SeedInquiries()
    {
        var machinery = dbContext.Machineries.FirstOrDefault();
        var random = new Random();

        if (machinery is null) return;

        var inquiries = new List<Inquiry>
        {
            new Inquiry
            {
                CustomerName = "John Doe",
                Machinery = machinery,
                SalespersonId = "auth0|673b73fa1e5482be2e942b7f"
            },
            new Inquiry
            {
                CustomerName = "Jane Doe",
                Machinery = machinery,
                SalespersonId = "auth0|673b73fa1e5482be2e942b7f"
            },
            new Inquiry
            {
                CustomerName = "Jack Doe",
                Machinery = machinery,
                SalespersonId = "auth0|673b73fa1e5482be2e942b7f"
            },
            new Inquiry
            {
                CustomerName = "Jill Doe",
                Machinery = machinery,
                SalespersonId = "auth0|673b73fa1e5482be2e942b7f"
            }
        };

        dbContext.Inquiries.AddRange(inquiries);
        dbContext.SaveChanges();
    }

    private void SeedInquiryOptions()
    {
        var inquiries = dbContext.Inquiries.Include(x => x.Machinery).ToList();
        var machineryOptions = dbContext.MachineryOptions.ToList();
        var random = new Random();

        if (inquiries.Count == 0 || machineryOptions.Count == 0) return;

        var inquiryOptions = new List<InquiryOption>();

        foreach (var inquiry in inquiries)
        {
            var applicableOptions = machineryOptions
                .Where(mo => mo.Machinery.Id == inquiry.Machinery.Id)
                .ToList();

            if (applicableOptions.Count == 0) continue;

            int numberOfOptionsToAssign = random.Next(1, Math.Min(3, applicableOptions.Count) + 1);
            var selectedOptions = applicableOptions
                .OrderBy(_ => random.Next())
                .Take(numberOfOptionsToAssign)
                .ToList();

            foreach (var option in selectedOptions)
            {
                inquiryOptions.Add(new InquiryOption
                {
                    Inquiry = inquiry,
                    MachineryOption = option
                });
            }
        }

        dbContext.InquiryOptions.AddRange(inquiryOptions);
        dbContext.SaveChanges();
    }

    private void SeedTradedMachineryImages()
    {
        var tradedMachineries = dbContext.TradedMachineries.ToList();

        var imageUrls = new Dictionary<string, List<string>>
        {
            { "Caterpillar 320", new List<string>
                {
                  "https://risea02.blob.core.windows.net/machinery/8377058-01.jpg"
                }
            },
            { "Komatsu PC210", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/17539693-05.jpg"
                }
            },
            {"Hitachi ZX350LC-6", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Hitachi%20ZX350LC-6/Hitachi_ZX350LC-6_1.png"
                }
            },
            { "Volvo EC950F", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/Excavators-Volvo-EC950F-50910356.jpg"
                }
            },
            { "Liebherr R 9800", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/tu988NpwYfuNAgIR_yk9poh2eUV2AON-5GmGj_3-Hl4.webp"
                }
            },
            { "JCB JS220", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/01_JCB_JS220LC_BM3035DSC08959.JPG"
                }
            },
            { "Doosan DX340LCA", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/l_05_Doosan_DX340LC_-_BM5456-17.jpg"
                }
            },
            { "Hyundai R500LC-9A", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/01_Hyundai_R500LC-7A_-_BM4664_09.JPEG"
                }
            },
            { "CASE CX750D", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/rupsgraafmachines-cx-750-d-rtc-case2.jpg"
                }
            },
            { "SANY SY500H", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/07dae3de77179e5fff8eefe3f9a03e35be628282-2016x1512.avif"
                }
            },
            { "Kubota KX080-4", new List<string>
                {
                    "https://risea02.blob.core.windows.net/machinery/kubota-kx080-4224528027_1.jpg"
                }
            }

        };

        var images = new List<TradedMachineryImage>();

        foreach (var tradedMachinery in tradedMachineries)
        {
            if (imageUrls.TryGetValue(tradedMachinery.Name, out var urls))
            {
                images.AddRange(urls.Select(url => new TradedMachineryImage
                {
                    Url = url,
                    TradedMachinery = tradedMachinery
                }));
            }
        }

        dbContext.TradedMachineryImages.AddRange(images);
        dbContext.SaveChanges();
    }
}


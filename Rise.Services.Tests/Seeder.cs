using Rise.Domain.Customers;
using Rise.Domain.Machineries;
using Rise.Domain.Quotes;
using Rise.Domain.Translations;
using Rise.Domain.Orders;
using Rise.Persistence;
using Rise.Domain.Inquiries;
using Rise.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;

namespace Rise.Services.Tests
{
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
                SeedTradedMachineryImages();
            }

            if (!HasTradedMachineriesSeeded())
            {
                SeedTradedMachineries();
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

        private bool HasInquiriesSeeded()
        {
            return dbContext.Inquiries.Any();
        }
        private bool HasInquiryOptionsSeeded()
        {
            return dbContext.InquiryOptions.Any();
        }

        private void SeedCategories()
        {
            var categories = new List<Category>
    {
        new() { Name = "Belettering / Blinderen", Code = "1200" },
        new() { Name = "Veiligheidsopties", Code = "1300" },
        new() { Name = "Comfortopties", Code = "1400" },
        new() { Name = "Prestaties en Besturing", Code = "1500" },
        new() { Name = "Bescherming en Onderhoud", Code = "1600" }

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
        new() { Name = "Brandblusinstallatie", Code = "BO-003", Category = categories.First(c => c.Name == "Bescherming en Onderhoud") }
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
        new() { Name = "Heftruck" }
    };

            dbContext.MachineryTypes.AddRange(machineryTypes);
            dbContext.SaveChanges();
        }

        private void SeedTranslations()
        {
            var translations = new List<Translation>
        {
            new() {OriginalText = "Grundgerät", TranslatedText = "Basisapparaat", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Dieselpartikelfilter", TranslatedText = "Dieselroetfilter", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Oberwagen", TranslatedText = "Bovenwagen", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Rohrbruchsicherung", TranslatedText = "Pijpbreukbeveiliging", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Unterwagen", TranslatedText = "Onderstel", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Zentralschmieranlage", TranslatedText = "Centraal smeersysteem", IsAccepted =  false, UserEmail = "aria.maes@dozer.be" },
            new() {OriginalText = "Klimaautomatik", TranslatedText = "Automatische klimaatregeling", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Standardverpackung", TranslatedText = "Standaardverpakking", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Arbeitsbremse", TranslatedText = "Werkrem", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Überlastwarneinrichtung", TranslatedText = "Waarschuwingsapparaat voor overbelasting", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Traglasttabelle", TranslatedText = "Tabel draagvermogen", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
            new() {OriginalText = "Kabine", TranslatedText = "Cabine", IsAccepted =  true, UserEmail = "pieter.desmet@dozer.be" },
        };

            dbContext.Translations.AddRange(translations);
        }

        private void SeedCustomers()
        {
            var customers = new List<Customer>
        {
            new() { Name = "John Doe", Street = "Straat", StreetNumber = "1", City = "Aalst", PostalCode = "9300", Country = "België" },
            new() { Name = "Jane Doe", Street = "AndereStraat", StreetNumber = "2", City = "Brussel", PostalCode = "1000", Country = "België" },
            new() { Name = "Jack Doe", Street = "NogEenStraat", StreetNumber = "3", City = "Gent", PostalCode = "9000", Country = "België" },
            new() { Name = "Jill Doe", Street = "LaatsteStraat", StreetNumber = "4", City = "Antwerpen", PostalCode = "2000", Country = "België" }
        };

            dbContext.Customers.AddRange(customers);
            dbContext.SaveChanges();
        }

        private void SeedQuotes()
        {
            var customers = dbContext.Customers.ToList();
            var customer = customers.FirstOrDefault();
            var machineries = dbContext.Machineries.ToList();
            var random = new Random();
            const decimal vatRate = 0.21m;

            if (customers.Count == 0 || machineries.Count == 0) return;

            var quotes = new List<Quote>();

            for (int i = 1; i < 3; i++)
            {
                var selectedMachinery = machineries.Where(x => x.Id == 12).First();
                var totalWithoutVat = random.Next(50000, 500000);
                var totalWithVat = totalWithoutVat * (1 + vatRate);

                var quote = new Quote
                {
                    QuoteNumber = $"{i}dd-1",
                    IsApproved = i % 2 == 0,
                    Date = DateTime.Now.AddDays(-random.Next(0, 365)),
                    Customer = customer!,
                    Machinery = selectedMachinery,
                    TotalWithoutVat = totalWithoutVat,
                    TotalWithVat = totalWithVat,
                    BasePrice = totalWithoutVat,
                    MainOptions = "Basis:John Deere 850K:Volautomatisch centraal smeersysteem, bovenwagen en uitrusting, 4 kg (exclusief kinematiek):Inklapbare stuurconsole:Automatische bedrijfsrem:Camera aan de achterzijde van de machine;Onderstel:Stempels achterzijde, schild voorzijde, 2550 mm breed:Zuigerstangbescherming achterste klauwen en voorste schild:Speeder 30 km/h:Hydraulische aansluiting voor het laten kippen aanhanger (enkelwerkend):Camera aan de rechterkant;Cabine:Radio Comfort:Comfort stoel:Dakraamwisser:Handsteun (verhoging) voor joystick;Uitrusting:Verstelgiek 5,05 m:Lekolieleiding voor aanbouwdelen;Algemeenheden:Gebruikshandleiding duits + nederlands:Losse stickers",
                    SalespersonId = "auth0|673b73fa1e5482be2e942b7f",
                };

                quotes.Add(quote);
            }

            dbContext.Quotes.AddRange(quotes);
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
            var quote = new Quote
            {
                QuoteNumber = $"3dd-1",
                IsApproved = true,
                Date = DateTime.Today,
                Customer = customer!,
                Machinery = machinery!,
                TotalWithoutVat = 5000,
                TotalWithVat = 5000,
                BasePrice = 5000,
                MainOptions = "Main options",
                SalespersonId = "auth0|673b73fa1e5482be2e942b7f",
            };

            var random = new Random();

            var order = new Order
            {
                OrderNumber = "3dd",
                Quote = quote,
                Date = quote.Date.AddDays(random.Next(1, 30)),
                IsCancelled = false
            };

            dbContext.Orders.AddRange(order);
            dbContext.SaveChanges();
        }

        private void SeedLocations()
        {
            var locations = new List<Location> {

        new Location
        {
            Name = "Bibliotheek Centraal",
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
            Name = "Stadspark",
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
            Name = "Technologie Centrum",
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
            Name = "Groene Vallei Resort",
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
            Name = "Havenzicht Café",
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
                Quote = quotes.First(),
            },
            new()
            {
                Name = "Komatsu PC210",
                SerialNumber = "KOMPC210-202310002",
                Type = machineryTypes.First(),
                Description = "Komatsu PC210 - Graafmachine",
                EstimatedValue = 11000m,
                Year = 2019,
                Quote = quotes.First(),
            }
        };
            dbContext.TradedMachineries.AddRange(tradedMachineries);
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
            var inquiries = dbContext.Inquiries.ToList();
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
    }
}
using Rise.Domain.Machineries;
using Shouldly;
using System;
using Xunit;

namespace Rise.Domain.Tests.Machineries
{
    public class MachineryShould
    {
        [Fact]
        public void BeCreated()
        {
            Machinery m = new()
            {
                Name = "Caterpillar 320",
                SerialNumber = "CAT320-202310001",
                Type = new MachineryType { Name = "Graafmachine" },
                Description = "Caterpillar 320 - Graafmachine",
                BrochureText = "De Caterpillar 320 graafmachine is ontworpen voor zware bouwprojecten waar efficiëntie en betrouwbaarheid van cruciaal belang zijn. Deze graafmachine biedt een ongeëvenaarde combinatie van kracht, duurzaamheid en brandstofbesparing dankzij de nieuwste hydraulische technologieën. De 320 is uitgerust met een geavanceerd controlesysteem waarmee operators complexe taken moeiteloos kunnen uitvoeren. Met het geavanceerde hydraulische systeem, een comfortabel ontwerp, en verminderde uitstoot is de Caterpillar 320 perfect voor langdurige graaf- en sloopwerkzaamheden, zelfs in uitdagende omgevingen. Dit model verhoogt de productiviteit en minimaliseert de operationele kosten, met speciale voorzieningen voor onderhoudsgemak en duurzaamheid in veeleisende omgevingen.",
			};

            m.Name.ShouldBe("Caterpillar 320");
            m.SerialNumber.ShouldBe("CAT320-202310001");
            m.Type.Name.ShouldBe("Graafmachine");
            m.Description.ShouldBe("Caterpillar 320 - Graafmachine");
            m.BrochureText.ShouldBe("De Caterpillar 320 graafmachine is ontworpen voor zware bouwprojecten waar efficiëntie en betrouwbaarheid van cruciaal belang zijn. Deze graafmachine biedt een ongeëvenaarde combinatie van kracht, duurzaamheid en brandstofbesparing dankzij de nieuwste hydraulische technologieën. De 320 is uitgerust met een geavanceerd controlesysteem waarmee operators complexe taken moeiteloos kunnen uitvoeren. Met het geavanceerde hydraulische systeem, een comfortabel ontwerp, en verminderde uitstoot is de Caterpillar 320 perfect voor langdurige graaf- en sloopwerkzaamheden, zelfs in uitdagende omgevingen. Dit model verhoogt de productiviteit en minimaliseert de operationele kosten, met speciale voorzieningen voor onderhoudsgemak en duurzaamheid in veeleisende omgevingen.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithAnInvalidName(string? name)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = name!,
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithAnInvalidSerialNumber(string? serialNumber)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = serialNumber!,
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        public void NotBeCreatedWithAnInvalidType(MachineryType? type)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = type!,
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithAnInvalidDescription(string? description)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = description!,
                    BrochureText = "Brochure text.",
				};
            };
            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedToHaveAnInvalidName(string? name)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
                m.Name = name!;
            };

            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedToHaveAnInvalidSerialNumber(string? serialNumber)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
                m.SerialNumber = serialNumber!;
            };

            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        public void NotBeChangedToHaveAnInvalidType(MachineryType? type)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
                m.Type = type!;
            };

            act.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedToHaveAnInvalidDescription(string? description)
        {
            Action act = () =>
            {
                Machinery m = new()
                {
                    Name = "TestMachine 1",
                    SerialNumber = "TestSN1234",
                    Type = new MachineryType { Name = "TestType" },
                    Description = "TestDescription",
                    BrochureText = "Brochure text.",
				};
                m.Description = description!;
            };

            act.ShouldThrow<ArgumentException>();
        }
    }
}

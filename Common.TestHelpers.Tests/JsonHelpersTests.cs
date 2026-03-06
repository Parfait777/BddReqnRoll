using Common.TestHelpers.Interfaces;
using Moq;
using System.Text.Json;

namespace Common.TestHelpers.UnitTests
{
    [TestClass]
    public class JsonHelpersTests
    {
        [TestMethod]
        public void Parse_ShouldReturnEmptyObject_WhenJsonIsNull()
        {
            using JsonDocument result = JsonHelpers.Parse(null);

            Assert.AreEqual(JsonValueKind.Object, result.RootElement.ValueKind);
            Assert.AreEqual(0, result.RootElement.EnumerateObject().Count());
        }

        [TestMethod]
        public void Parse_ShouldReturnEmptyObject_WhenJsonIsEmpty()
        {
            using JsonDocument result = JsonHelpers.Parse(string.Empty);

            Assert.AreEqual(JsonValueKind.Object, result.RootElement.ValueKind);
            Assert.AreEqual(0, result.RootElement.EnumerateObject().Count());
        }

        [TestMethod]
        public void Parse_ShouldReturnEmptyObject_WhenJsonIsWhitespace()
        {
            using JsonDocument result = JsonHelpers.Parse("   ");

            Assert.AreEqual(JsonValueKind.Object, result.RootElement.ValueKind);
            Assert.AreEqual(0, result.RootElement.EnumerateObject().Count());
        }

        [TestMethod]
        public void Parse_ShouldParseValidJson_WhenJsonIsProvided()
        {
            const string json = """{"id":123,"name":"Parfait"}""";

            using JsonDocument result = JsonHelpers.Parse(json);

            Assert.AreEqual("123", result.RootElement.GetProperty("id").GetRawText());
            Assert.AreEqual("Parfait", result.RootElement.GetProperty("name").GetString());
        }

        [TestMethod]
        public void GetString_ShouldReturnStringValue_ForStringProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"name":"Parfait"}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "name");

            Assert.AreEqual("Parfait", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnNumericRawText_ForNumberProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"amount":12.5}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "amount");

            Assert.AreEqual("12.5", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnTrue_ForBooleanTrueProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"active":true}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "active");

            Assert.AreEqual("true", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnFalse_ForBooleanFalseProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"active":false}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "active");

            Assert.AreEqual("false", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnRawJson_ForObjectProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"data":{"id":1}}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "data");

            Assert.AreEqual("""{"id":1}""", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnRawJson_ForArrayProperty()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"items":[1,2,3]}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "items");

            Assert.AreEqual("[1,2,3]", result);
        }

        [TestMethod]
        public void GetString_ShouldReturnNull_WhenPathDoesNotExist()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"name":"Parfait"}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "missing");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetString_ShouldReturnNull_WhenNestedPathDoesNotExist()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"data":{"id":1}}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "data.name");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetString_ShouldReturnNestedString_WhenNestedPathExists()
        {
            using JsonDocument doc = JsonDocument.Parse("""{"data":{"name":"Parfait"}}""");

            string? result = JsonHelpers.GetString(doc.RootElement, "data.name");

            Assert.AreEqual("Parfait", result);
        }

        [TestMethod]
        public void SetJsonField_ShouldSetStringValue_WhenQuotedStringProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Name", "\"Parfait\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("Parfait", doc.RootElement.GetProperty("Name").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetUnquotedTokenAsString_WhenNotSpecialOrNumber()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Name", "Parfait");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("Parfait", doc.RootElement.GetProperty("Name").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetNull_WhenLegacyNullTokenProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "MiddleName", SpecialValueTokens.Null);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(JsonValueKind.Null, doc.RootElement.GetProperty("MiddleName").ValueKind);
        }

        [TestMethod]
        public void SetJsonField_ShouldSetBooleanTrue_WhenLegacyTrueTokenProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Active", SpecialValueTokens.True);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.IsTrue(doc.RootElement.GetProperty("Active").GetBoolean());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetBooleanFalse_WhenLegacyFalseTokenProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Active", SpecialValueTokens.False);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.IsFalse(doc.RootElement.GetProperty("Active").GetBoolean());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetLong_WhenIntegerLiteralProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Count", "123");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(123L, doc.RootElement.GetProperty("Count").GetInt64());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetDecimal_WhenDecimalLiteralProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Amount", "12.5");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(12.5m, doc.RootElement.GetProperty("Amount").GetDecimal());
        }

        [TestMethod]
        public void SetJsonField_ShouldCreateNestedObjects_WhenPathDoesNotExist()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "ReportingPeriod.ReportingPeriodFrom", "\"2025-01-01\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-01-01", doc.RootElement
                .GetProperty("ReportingPeriod")
                .GetProperty("ReportingPeriodFrom")
                .GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldCreateArrayAndObject_WhenArrayPathDoesNotExist()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Attachments[0].FileName", "\"test.pdf\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement attachments = doc.RootElement.GetProperty("Attachments");

            Assert.AreEqual(JsonValueKind.Array, attachments.ValueKind);
            Assert.AreEqual(1, attachments.GetArrayLength());
            Assert.AreEqual("test.pdf", attachments[0].GetProperty("FileName").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldResizeArray_WhenSettingLaterIndex()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Attachments[2].FileName", "\"test.pdf\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement attachments = doc.RootElement.GetProperty("Attachments");

            Assert.AreEqual(3, attachments.GetArrayLength());
            Assert.AreEqual(JsonValueKind.Null, attachments[0].ValueKind);
            Assert.AreEqual(JsonValueKind.Null, attachments[1].ValueKind);
            Assert.AreEqual("test.pdf", attachments[2].GetProperty("FileName").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetArrayPrimitiveValue_WhenLeafIsArrayIndex()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Items[1]", "\"value-2\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement items = doc.RootElement.GetProperty("Items");

            Assert.AreEqual(2, items.GetArrayLength());
            Assert.AreEqual(JsonValueKind.Null, items[0].ValueKind);
            Assert.AreEqual("value-2", items[1].GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldReplaceExistingPropertyUsingCaseInsensitiveMatch()
        {
            string json = """{"reportingPeriod":{"reportingPeriodFrom":"2024-01-01"}}""";

            string result = JsonHelpers.SetJsonField(json, "ReportingPeriod.ReportingPeriodFrom", "\"2025-01-01\"");

            using JsonDocument doc = JsonDocument.Parse(result);

            Assert.AreEqual("2025-01-01", doc.RootElement
                .GetProperty("reportingPeriod")
                .GetProperty("reportingPeriodFrom")
                .GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldReplaceNonObjectWithObject_WhenPathRequiresObject()
        {
            string json = """{"ReportingPeriod":"invalid"}""";

            string result = JsonHelpers.SetJsonField(json, "ReportingPeriod.ReportingPeriodFrom", "\"2025-01-01\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-01-01", doc.RootElement
                .GetProperty("ReportingPeriod")
                .GetProperty("ReportingPeriodFrom")
                .GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldReplaceNonArrayWithArray_WhenPathRequiresArray()
        {
            string json = """{"Attachments":"invalid"}""";

            string result = JsonHelpers.SetJsonField(json, "Attachments[0].FileName", "\"test.pdf\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("test.pdf", doc.RootElement
                .GetProperty("Attachments")[0]
                .GetProperty("FileName")
                .GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedBooleanTrue()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Active", "<boolean:true>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.IsTrue(doc.RootElement.GetProperty("Active").GetBoolean());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedBooleanFalse()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Active", "<boolean:false>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.IsFalse(doc.RootElement.GetProperty("Active").GetBoolean());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedString()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Name", "<string:Example>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("Example", doc.RootElement.GetProperty("Name").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedInteger()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Count", "<integer:100>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(100L, doc.RootElement.GetProperty("Count").GetInt64());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedDecimal()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Amount", "<decimal:2.5>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(2.5m, doc.RootElement.GetProperty("Amount").GetDecimal());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedDate_WhenIsoDateProvided()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "StartDate", "<date:2025-01-01>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-01-01", doc.RootElement.GetProperty("StartDate").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedDateToday_WhenDateTimeWrapperProvided()
        {
            Mock<IDateTimeWrapper> dateTimeWrapperMock = new();
            dateTimeWrapperMock
                .Setup(x => x.GetUtcNow())
                .Returns(new DateTime(2025, 3, 10, 14, 30, 0, DateTimeKind.Utc));

            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "StartDate", "<date:today>", dateTimeWrapperMock.Object);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-03-10", doc.RootElement.GetProperty("StartDate").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedDateTodayPlusOffset_WhenDateTimeWrapperProvided()
        {
            Mock<IDateTimeWrapper> dateTimeWrapperMock = new();
            dateTimeWrapperMock
                .Setup(x => x.GetUtcNow())
                .Returns(new DateTime(2025, 3, 10, 14, 30, 0, DateTimeKind.Utc));

            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "StartDate", "<date:today:+1>", dateTimeWrapperMock.Object);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-03-11", doc.RootElement.GetProperty("StartDate").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldSetTypedDateTodayMinusOffset_WhenDateTimeWrapperProvided()
        {
            Mock<IDateTimeWrapper> dateTimeWrapperMock = new();
            dateTimeWrapperMock
                .Setup(x => x.GetUtcNow())
                .Returns(new DateTime(2025, 3, 10, 14, 30, 0, DateTimeKind.Utc));

            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "StartDate", "<date:today:-1>", dateTimeWrapperMock.Object);

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("2025-03-09", doc.RootElement.GetProperty("StartDate").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldFallbackToSystemDate_WhenDateTimeWrapperNotProvided()
        {
            string json = "{}";
            string expected = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");

            string result = JsonHelpers.SetJsonField(json, "StartDate", "<date:today>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual(expected, doc.RootElement.GetProperty("StartDate").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedBooleanTokenIsInvalid()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "Active", "<boolean:maybe>"));

            StringAssert.Contains(ex.Message, "Invalid boolean token");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedIntegerTokenIsInvalid()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "Count", "<integer:abc>"));

            StringAssert.Contains(ex.Message, "Invalid integer token");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedDecimalTokenIsInvalid()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "Amount", "<decimal:abc>"));

            StringAssert.Contains(ex.Message, "Invalid decimal token");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedDateTokenIsInvalidIsoDate()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "StartDate", "<date:2025-99-99>"));

            StringAssert.Contains(ex.Message, "Invalid date token");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedDateTokenHasInvalidTodayFormat()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "StartDate", "<date:today|+1>"));

            StringAssert.Contains(ex.Message, "Invalid date token");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenTypedDateTokenHasInvalidTodayOffset()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "StartDate", "<date:today:+abc>"));

            StringAssert.Contains(ex.Message, "Invalid date offset");
        }

        [TestMethod]
        public void SetJsonField_ShouldTreatUnknownTypedTokenAsPlainString()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Value", "<unknown:test>");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("<unknown:test>", doc.RootElement.GetProperty("Value").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenFieldPathIsEmpty()
        {
            string json = "{}";

            Assert.Throws<ArgumentException>(
                () => JsonHelpers.SetJsonField(json, string.Empty, "\"value\""));
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenArrayIndexIsInvalid()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "Attachments[abc].FileName", "\"test.pdf\""));

            StringAssert.Contains(ex.Message, "Invalid array index");
        }

        [TestMethod]
        public void SetJsonField_ShouldThrow_WhenArrayIndexBracketIsMissing()
        {
            string json = "{}";

            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(
                () => JsonHelpers.SetJsonField(json, "Attachments[0.FileName", "\"test.pdf\""));

            StringAssert.Contains(ex.Message, "missing ']'");
        }

        [TestMethod]
        public void SetJsonField_ShouldSupportNestedArrayThenObjectPath()
        {
            string json = "{}";

            string result = JsonHelpers.SetJsonField(json, "Funds[0].Investors[1].Name", "\"Investor B\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            JsonElement funds = doc.RootElement.GetProperty("Funds");

            Assert.AreEqual(1, funds.GetArrayLength());
            Assert.AreEqual("Investor B", funds[0]
                .GetProperty("Investors")[1]
                .GetProperty("Name")
                .GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldOverwriteExistingValue()
        {
            string json = """{"Name":"Old"}""";

            string result = JsonHelpers.SetJsonField(json, "Name", "\"New\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("New", doc.RootElement.GetProperty("Name").GetString());
        }

        [TestMethod]
        public void SetJsonField_ShouldPreserveOtherProperties_WhenUpdatingSingleField()
        {
            string json = """{"Name":"Old","Age":40}""";

            string result = JsonHelpers.SetJsonField(json, "Name", "\"New\"");

            using JsonDocument doc = JsonDocument.Parse(result);
            Assert.AreEqual("New", doc.RootElement.GetProperty("Name").GetString());
            Assert.AreEqual(40, doc.RootElement.GetProperty("Age").GetInt32());
        }
    }
}
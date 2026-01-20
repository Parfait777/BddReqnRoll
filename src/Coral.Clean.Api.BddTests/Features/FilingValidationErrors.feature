
Feature: Filing Form API - Validation Rules
Background:
	Given the API base url is configured
	#And the endpoint "/filings" is available # /filings is not a separate endpoint but part of /api/v1/filings
	And I authenticate

@mandatoryField @mandatoryValidation
Scenario Outline: To Validate Mandatory field for <fieldPath>
  # Optional setup for dynamic arrays or baseline payload
	Given I load request template "/api/v1/filings.json"
	And I ensure any required parent objects/arrays exist for "<fieldPath>"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response error code "code" should be "<expectedErrorCode>"
	And the response should include field "<responseField>"
	
Examples:
	| fieldPath                                                                          | responseField                                                            | value | expectedResponseCode | expectedErrorCode |
	| data.hasLei                                                                        | HasLei                                                                   | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.isMoneyMarketFund                                         | IsMoneyMarketFund                                                        | null  |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].type                                                         | Data.FundOperators[0].Type                                               | null  |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].naturalOrEntity                                              | Data.FundOperators[0].NaturalOrEntity                                    | null  |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].firstNameOrEntityName                                        | Data.DundOperators[0].FirstNameOrEntityName                              | null  |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].emailAddress                                                 | Data.FundOperators[0].EmailAddress                                       | null  |                  400 | FIL-VAL-E400      |
	| data.governance.isTradingOfEquityInterestsSuspended                                | Governance.IsTradingOfEquityInterestsSuspended                           | null  |                  400 | FIL-VAL-E400      |
	| data.governance.haveSubstantiallyAllEquityInterestsBeenRedeemed                    | Governance.HaveSubstantiallyAllEquityInterestsBeenRedeemed               | null  |                  400 | FIL-VAL-E400      |
	| data.governance.hasAnyRegulatoryInvestigationDuringFilingPeriod                    | Governance.HasAnyRegulatoryInvestigationDuringFilingPeriod               | null  |                  400 | FIL-VAL-E400      |
	| data.governance.hasDecisionBeenMadeToTerminateFund                                 | Governance.HasDecisionBeenMadeToTerminateFund                            | null  |                  400 | FIL-VAL-E400      |
	| data.governance.hasFundSidePocketedInvestments                                     | Governance.HasFundSidePocketedInvestments                                | null  |                  400 | FIL-VAL-E400      |
	| data.governance.hasFundImplementedGateOnRedemptions                                | Governance.HasFundImplementedGateOnRedemptions                           | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.investmentStrategy                                        | FundCharacteristics.InvestmentStrategy                                   | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.stockExchange                                             | FundCharacteristics.StockExchange                                        | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.accountingPrinciples                                      | FundCharacteristics.AccountingPrinciples                                 | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.auditingStandards                                         | FundCharacteristics.AuditingStandards                                    | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.legalStructure                                            | FundCharacteristics.LegalStructure                                       | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfInvestors                               | InvestorDetails.investors[0].CountryOfInvestors                          | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfNetAssetValue                           | InvestorDetails.investors[0].CountryOfNetAssetValue                      | null  |                  400 | FIL-VAL-E400      |
	| data.governance.numberOfBoardMeetingsHeld                                          | Governance.NumberOfBoardMeetingsHeld                                     | null  |                  400 | FIL-VAL-E400      |
	| data.governance.totalNumberOfSideLettersSignedWithInvestors                        | Governance.TotalNumberOfSideLettersSignedWithInvestors                   | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].numberOfInvestors                                | InvestorDetails.investors[0].NumberOfInvestors                           | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticInvestor.numberOfInvestors                            | DomesticInvestor.NumberOfInvestors                                       | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.numberOfRetailInvestors                 | ForeignRetailInvestor.NumberOfInvestors                                  | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.numberOfRetailInvestors                | DomesticRetailInvestor.NumberOfInvestors                                 | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.highNetWorthInvestor.numberOfInvestors                        | HighNetWorthInvestor.NumberOfInvestors                                   | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestors                          | PepForeignInvestor.NumberOfInvestors                                     | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestorsNominees                  | PepForeignInvestor.NumberOfInvestorsNominees                             | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestors                         | PepDomesticInvestor.NumberOfInvestors                                    | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestorsNominees                 | PepDomesticInvestor.NumberOfInvestorsNominees                            | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestors                  | OtherNaturalPersonInvestor.NumberOfInvestors                             | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestorsNominees          | OtherNaturalPersonInvestor.NumberOfInvestorsNominees                     | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestors         | NonBankFinancialInstitutionInvestor.NumberOfInvestors                    | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestorsNominees | NonBankFinancialInstitutionInvestor.NumberOfInvestorsNominees            | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestors                               | TrustInvestor.NumberOfInvestors                                          | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestorsNominees                       | TrustInvestor.NumberOfInvestorsNominees                                  | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.numberOfInvestors             | NonFinancialCorporationInvestor.NumberOfInvestors                        | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.numberOfInvestorsNominees     | NonFinancialCorporationInvestor.NumberOfInvestorsNominees                | null  |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.minimumSubscriptionAmountUsd                              | FundCharacteristics.MinimumSubscriptionAmountUsd                         | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].netAssetValue                                    | InvestorDetails.investors[0].NetAssetValue                               | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticInvestor.percentageOfNetAssetValue                    | DomesticInvestor.PercentageOfNetAssetValue                               | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.percentageOfTotalRetailInvestorsValue   | ForeignRetailInvestor.percentageOfTotalRetailInvestorsValue              | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.percentageOfTotalRetailInvestorsValue  | DomesticRetailInvestor.percentageOfTotalRetailInvestorsValue             | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.highNetWorthInvestor.netAssetValue                            | HighNetWorthInvestor.netAssetValue                                       | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.netAssetValue                              | PepForeignInvestor.netAssetValue                                         | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.netAssetValueNominees                      | PepForeignInvestor.netAssetValueNominees                                 | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.netAssetValue                             | PepDomesticInvestor.netAssetValue                                        | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.netAssetValueNominees                     | PepDomesticInvestor.netAssetValueNominees                                | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.netAssetValue                      | OtherNaturalPersonInvestor.netAssetValue                                 | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.netAssetValueNominees              | OtherNaturalPersonInvestor.netAssetValueNominees                         | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.netAssetValue             | NonBankFinancialInstitutionInvestor.netAssetValue                        | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.netAssetValueNominees     | NonBankFinancialInstitutionInvestor.netAssetValueNominees                | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.netAssetValue                                   | TrustInvestor.netAssetValue                                              | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.netAssetValueNominees                           | TrustInvestor.netAssetValueNominees                                      | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.netAssetValue                 | NonFinancialCorporationInvestor.netAssetValue                            | null  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.netAssetValueNominees         | NonFinancialCorporationInvestor.netAssetValueNominees                    | null  |                  400 | FIL-VAL-E400      |
	| data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent           | Data.OnshoreLegalCounsel.UsesInvestmentManagerInHouseAsOnshoreEquivalent | null  |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate                                               | Governance.offeringDocumentDate                                          | null  |                  400 | FIL-VAL-E400      |

# Can be expanded on the basis of fields from the form that will be added in future - Need to get more details from BA
@negative @characterLengthValidation @stringFieldValidation
Scenario Outline: Validate character length and digit count constraints for fields in API response
	Given I load request template "/api/v1/filings.json"
	And I ensure any required parent objects/arrays exist for "<fieldPath>"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response error code "code" should be "<expectedErrorCode>"
	And the response should include field "<responseField>"

Examples:
	| fieldPath                                     | responseField                            | value                                                                                                   | expectedResponseCode | expectedErrorCode |
	| data.legalEntityIdentifier                    | LegalEntityIdentifier                    | "LengthMoreThantwentyOne"                                                                               |                  400 | FIL-VAL-E400      |
	#Validation for error when the character length exceeds 100 characters (Allowed character length : 2-100 characters)
	| data.fundCharacteristics.investmentStrategy   | FundCharacteristics.InvestmentStrategy   | "OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation" |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.stockExchange        | FundCharacteristics.StockExchange        | "OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation" |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.accountingPrinciples | FundCharacteristics.AccountingPrinciples | "OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation" |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.auditingStandards    | FundCharacteristics.AuditingStandards    | "OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation" |                  400 | FIL-VAL-E400      |
	#Validation for error when the character length is less than 2 characters (Allowed character length : 2-100 characters)
	| data.fundCharacteristics.investmentStrategy   | FundCharacteristics.InvestmentStrategy   | "A"                                                                                                     |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.stockExchange        | FundCharacteristics.StockExchange        | "B"                                                                                                     |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.accountingPrinciples | FundCharacteristics.AccountingPrinciples | "C"                                                                                                     |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.auditingStandards    | FundCharacteristics.AuditingStandards    | "D"                                                                                                     |                  400 | FIL-VAL-E400      |

	

@booleanFieldValidation @hasLei @isMoneyMarketFund @onshoreUsesIMInHouse
Scenario Outline: Validate Invalid Input Values for Boolean field <fieldPath> in API response
	Given I load request template "/api/v1/filings.json"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response should include field "<responseField>"
	And the response field "code" should be "<expectedErrorCode>"

Examples:
	| fieldPath                                                                | responsefield                                                       | value  | expectedResponseCode | expectedErrorCode |
	| data.hasLei                                                              | HasLei                                                              | "Test" |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.isMoneyMarketFund                               | FundCharacteristics.IsMoneyMarketFund                               | "Test" |                  400 | FIL-VAL-E400      |
	| data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent | OnshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent | "Test" |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].naturalOrEntity                                    | FundOperators[0].NaturalOrEntity                                    | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.isTradingOfEquityInterestsSuspended                      | Governance.IsTradingOfEquityInterestsSuspended                      | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.haveSubstantiallyAllEquityInterestsBeenRedeemed          | Governance.HaveSubstantiallyAllEquityInterestsBeenRedeemed          | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.hasAnyRegulatoryInvestigationDuringFilingPeriod          | Governance.HasAnyRegulatoryInvestigationDuringFilingPeriod          | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.hasDecisionBeenMadeToTerminateFund                       | Governance.HasDecisionBeenMadeToTerminateFund                       | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.hasFundSidePocketedInvestments                           | Governance.HasFundSidePocketedInvestments                           | "Test" |                  400 | FIL-VAL-E400      |
	| data.governance.hasFundImplementedGateOnRedemptions                      | Governance.HasFundImplementedGateOnRedemptions                      | "Test" |                  400 | FIL-VAL-E400      |


@enumValidation
Scenario Outline: Validate invalid values for enum field "<fieldPath>" in API response
	Given I load request template "/api/v1/filings.json"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response should include field "<responseField>"
	And the response field "code" should be "<expectedErrorCode>"

Examples:
	| fieldPath                                                | responseField                                       | value                  | expectedResponseCode | expectedErrorCode |
	| data.fundCharacteristics.investmentStrategy              | InvestmentStrategy                                  | "Test Strategy"        |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.stockExchange                   | StockExchange                                       | "Test Exchange"        |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.accountingPrinciples            | AccountingPrinciples                                | "Test GAAP"            |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.auditingStandards               | AuditingStandards                                   | "Test GAAS"            |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.legalStructure                  | LegalStructure                                      | "Test Legal Structure" |                  400 | FIL-VAL-E400      |
	| data.fundOperators[0].type                               | fundOperators[0].FundOperatorType                   | "Test Type"            |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfInvestors     | investorDetails.investors[0].CountryOfInvestors     | "Caymnan"              |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfNetAssetValue | investorDetails.investors[0].CountryOfNetAssetValue | "Russia"               |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfInvestors     | investorDetails.investors[0].CountryOfInvestors     | "CAY"                  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].countryOfNetAssetValue | investorDetails.investors[0].CountryOfNetAssetValue | "US"                   |                  400 | FIL-VAL-E400      |


@negative @validation @integer
Scenario Outline: Validation for non-integer input for integer field <responseField> with invalid value <value>
	Given I load request template "/api/v1/filings.json"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response should include field "<responseField>"
	And the response field "code" should be "<expectedErrorCode>"

Examples:
      
	| fieldPath                                                                          | responseField                                                 | value        | expectedResponseCode | expectedErrorCode |
	| data.governance.numberOfBoardMeetingsHeld                                          | Governance.NumberOfBoardMeetingsHeld                          | "abc"        |                  400 | FIL-VAL-E400      |
	| data.governance.numberOfBoardMeetingsHeld                                          | Governance.NumberOfBoardMeetingsHeld                          |         12.5 |                  400 | FIL-VAL-E400      |
	| data.governance.totalNumberOfSideLettersSignedWithInvestors                        | Governance.TotalNumberOfSideLettersSignedWithInvestors        | "12a"        |                  400 | FIL-VAL-E400      |
	| data.governance.totalNumberOfSideLettersSignedWithInvestors                        | Governance.TotalNumberOfSideLettersSignedWithInvestors        | true         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].numberOfInvestors                                | InvestorDetails.investors[0].NumberOfInvestors                | "one"        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors[0].numberOfInvestors                                | InvestorDetails.investors[0].NumberOfInvestors                |         3.14 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticInvestor.numberOfInvestors                            | DomesticInvestor.NumberOfInvestors                            | null         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticInvestor.numberOfInvestors                            | DomesticInvestor.NumberOfInvestors                            | "1,000"      |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.numberOfRetailInvestors                 | ForeignRetailInvestor.NumberOfRetailInvestors                 | " "          |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.numberOfRetailInvestors                 | ForeignRetailInvestor.NumberOfRetailInvestors                 |          7.0 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.numberOfRetailInvestors                | DomesticRetailInvestor.NumberOfRetailInvestors                | "12%"        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.numberOfRetailInvestors                | DomesticRetailInvestor.NumberOfRetailInvestors                | false        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.highNetWorthInvestor.numberOfInvestors                        | HighNetWorthInvestor.NumberOfInvestors                        | "NaN"        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.highNetWorthInvestor.numberOfInvestors                        | HighNetWorthInvestor.NumberOfInvestors                        |        0.001 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestors                          | PEPForeignInvestor.NumberOfInvestors                          | "0x10"       |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestors                          | PEPForeignInvestor.NumberOfInvestors                          | null         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestorsNominees                  | PEPForeignInvestor.NumberOfInvestorsNominees                  | "12%"        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.numberOfInvestorsNominees                  | PEPForeignInvestor.NumberOfInvestorsNominees                  |          5.5 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestors                         | PEPDomesticInvestor.NumberOfInvestors                         | "2e3"        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestors                         | PEPDomesticInvestor.NumberOfInvestors                         | true         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestorsNominees                 | PEPDomesticInvestor.NumberOfInvestorsNominees                 | "0012"       |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.numberOfInvestorsNominees                 | PEPDomesticInvestor.NumberOfInvestorsNominees                 |          9.9 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestors                  | OtherNaturalPersonInvestor.NumberOfInvestors                  | "12 twelve"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestors                  | OtherNaturalPersonInvestor.NumberOfInvestors                  | null         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestorsNominees          | OtherNaturalPersonInvestor.NumberOfInvestorsNominees          | "12.0"       |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.numberOfInvestorsNominees          | OtherNaturalPersonInvestor.NumberOfInvestorsNominees          | true         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestors         | NonBankFinancialInstitutionInvestor.NumberOfInvestors         | "Twelve"     |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestors         | NonBankFinancialInstitutionInvestor.NumberOfInvestors         |          4.2 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestorsNominees | NonBankFinancialInstitutionInvestor.NumberOfInvestorsNominees | "—"          |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.numberOfInvestorsNominees | NonBankFinancialInstitutionInvestor.NumberOfInvestorsNominees | false        |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestors                               | TrustInvestor.NumberOfInvestors                               | "2025-01-01" |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestors                               | TrustInvestor.NumberOfInvestors                               |          2.2 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestorsNominees                       | TrustInvestor.NumberOfInvestorsNominees                       | "12 persons" |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.numberOfInvestorsNominees                       | TrustInvestor.NumberOfInvestorsNominees                       | null         |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.numberOfInvestors             | NonFinancialCorporationInvestor.NumberOfInvestors             | "twelve"     |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.numberOfInvestors             | NonFinancialCorporationInvestor.NumberOfInvestors             |         6.01 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.numberOfInvestorsNominees     | NonFinancialCorporationInvestor.NumberOfInvestorsNominees     | true         |                  400 | FIL-VAL-E400      |

	
@negative @dateFieldValidation @offeringDocumentDate
Scenario Outline: 3.4 Offering Document Date is mandatory, must follow yyyy-mm-dd format, and must not be a future date
	Given I load request template "/api/v1/filings.json"
	And I set json field "<fieldPath>" to <date>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response should include field "<responseField>"
	And the response field "code" should be "<expectedErrorCode>"

Examples:
	| fieldPath                            | responseField                   | date                  | expectedResponseCode | expectedErrorCode |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "February 21st, 2025" |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "2020/01/01"          |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "31-01-2020"          |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "01-31-2020"          |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "2020-13-01"          |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | TODAY+1               |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "01-JAN-2024"         |                  400 | FIL-VAL-E400      |
	| data.governance.offeringDocumentDate | Governance.OfferingDocumentDate | "Some Date"           |                  400 | FIL-VAL-E400      |


@negative @DecimalFieldValidation
Scenario Outline: Validation for invalid input for decimal field <responseField> with invalid value <value>
	Given I load request template "/api/v1/filings.json"
	And I set json field "<fieldPath>" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be <expectedResponseCode>
	And the response should include field "<responseField>"
	And the response field "code" should be "<expectedErrorCode>"

Examples:
	| fieldPath                                                                         | responseField                                                                | value  | expectedResponseCode | expectedErrorCode |
	#Non DEcimal input for decimal fields
	| data.fundCharacteristics.minimumSubscriptionAmountUsd                             | FundCharacteristics.MinimumSubscriptionAmountUsd                             | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.investors.0.netAssetValue                                    | InvestorDetails.Investors[0].NetAssetValue                                   | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.highNetWorthInvestor.netAssetValue                           | InvestorDetails.HighNetWorthInvestor.NetAssetValue                           | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.netAssetValue                             | InvestorDetails.PepForeignInvestor.NetAssetValue                             | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepForeignInvestor.netAssetValueNominees                     | InvestorDetails.PepForeignInvestor.NetAssetValueNominees                     | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.netAssetValue                            | InvestorDetails.PepDomesticInvestor.NetAssetValue                            | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.pepDomesticInvestor.netAssetValueNominees                    | InvestorDetails.PepDomesticInvestor.NetAssetValueNominees                    | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.netAssetValue                     | InvestorDetails.OtherNaturalPersonInvestor.NetAssetValue                     | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.otherNaturalPersonInvestor.netAssetValueNominees             | InvestorDetails.OtherNaturalPersonInvestor.NetAssetValueNominees             | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.netAssetValue            | InvestorDetails.NonBankFinancialInstitutionInvestor.NetAssetValue            | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonBankFinancialInstitutionInvestor.netAssetValueNominees    | InvestorDetails.NonBankFinancialInstitutionInvestor.NetAssetValueNominees    | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.netAssetValue                                  | InvestorDetails.TrustInvestor.NetAssetValue                                  | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.trustInvestor.netAssetValueNominees                          | InvestorDetails.TrustInvestor.NetAssetValueNominees                          | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.netAssetValue                | InvestorDetails.NonFinancialCorporationInvestor.NetAssetValue                | "abc"  |                  400 | FIL-VAL-E400      |
	| data.investorDetails.nonFinancialCorporationInvestor.netAssetValueNominees        | InvestorDetails.NonFinancialCorporationInvestor.NetAssetValueNominees        | "abc"  |                  400 | FIL-VAL-E400      |
	#Mininmun subscription amount greater than zero
	| data.fundCharacteristics.minimumSubscriptionAmountUsd                             | FundCharacteristics.MinimumSubscriptionAmountUsd                             |      0 |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.minimumSubscriptionAmountUsd                             | FundCharacteristics.MinimumSubscriptionAmountUsd                             |     -1 |                  400 | FIL-VAL-E400      |
	| data.fundCharacteristics.minimumSubscriptionAmountUsd                             | FundCharacteristics.MinimumSubscriptionAmountUsd                             |  -0.01 |                  400 | FIL-VAL-E400      |
	#For fields with allowed input values between 0 and 1	
	| data.investorDetails.domesticInvestor.percentageOfNetAssetValue                   | InvestorDetails.DomesticInvestor.PercentageOfNetAssetValue                   |  -0.01 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticInvestor.percentageOfNetAssetValue                   | InvestorDetails.DomesticInvestor.PercentageOfNetAssetValue                   |   1.01 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.percentageOfTotalRetailInvestorsValue  | InvestorDetails.ForeignRetailInvestor.PercentageOfTotalRetailInvestorsValue  | -0.001 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.foreignRetailInvestor.percentageOfTotalRetailInvestorsValue  | InvestorDetails.ForeignRetailInvestor.PercentageOfTotalRetailInvestorsValue  |    1.1 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.percentageOfTotalRetailInvestorsValue | InvestorDetails.DomesticRetailInvestor.PercentageOfTotalRetailInvestorsValue |     -1 |                  400 | FIL-VAL-E400      |
	| data.investorDetails.domesticRetailInvestor.percentageOfTotalRetailInvestorsValue | InvestorDetails.DomesticRetailInvestor.PercentageOfTotalRetailInvestorsValue |      2 |                  400 | FIL-VAL-E400      |


@conditionalField @legalEntityIdentifier
Scenario Outline: Validate field 1.2.1 Legal Entity Identifier required when hasLei = true as well as validation for special chaarcters
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.hasLei" to true
	And I set json field "data.legalEntityIdentifier" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response field "code" should be "FIL-VAL-E400"
	And the response should include field "Data.LegalEntityIdentifier"
Examples:
	| value              |
	| null               |
	| "Test@123"         |
	| "1234567890ABCDE!" |
	| "LEI#2024$%^&*()"  |



@conditional @hasCimaIdMissingAndInvalid
Scenario Outline: Mandatory field validation for field "HasCimaId" when Fund Operator Type is "DIRECTOR" and NaturalOrEntity is "Natural Person"
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.fundOperators[0].type" to <fundOperatorType>
	And I set json field "data.fundOperators[0].naturalOrEntity" to "Natural Person"
	And I set json field "data.fundOperators[0].hasCimaId" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "Data.FundOperators[0].HasCimaId"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value      | fundOperatorType  |
	#---------------------------------- Director ----------------------------------
	| null       | "director"        |
	| "TestFlag" | "director"        |
	#---------------------------------- General Partner ----------------------------------------
	| null       | "General Partner" |
	| "TestFlag" | "General Partner" |
	#---------------------------------- Trustee ----------------------------------------
	| null       | "Trustee"         |
	| "TestFlag" | "Trustee"         |
	#---------------------------------- Managing Member ----------------------------------------
	| null       | "Managing Member" |
	| "TestFlag" | "Managing Member" |

@conditional @fundOperatorCimaIdInvalidAndMissing
Scenario Outline: Mandatory field validation for field "CimaId" when hasCimaID is true
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.fundOperators[0].type" to <fundOperatorType>
	And I set json field "data.fundOperators[0].naturalOrEntity" to "Natural Person"
	And I set json field "data.fundOperators[0].hasCimaId" to "true"
	And I set json field "data.fundOperators[0].fundOperatorCimaId" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "Data.FundOperators[0].FundOperatorCimaId"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value      | fundOperatorType  |
	#---------------------------------- Director ----------------------------------
	| null       | "director"        |
	| "TestFlag" | "director"        |
	#Validation for CIMA ID to be only 7-Digits
	| "123456"   | "director"        |
	| "87654321" | "director"        |
	#---------------------------------- General Partner ----------------------------------------
	| null       | "General Partner" |
	| "TestFlag" | "General Partner" |
	#Validation for CIMA ID to be only 7-Digits
	| "123456"   | "General Partner" |
	| "87654321" | "General Partner" |
	#---------------------------------- Trustee ----------------------------------------
	| null       | "Trustee"         |
	| "TestFlag" | "Trustee"         |
	#Validation for CIMA ID to be only 7-Digits
	| "123456"   | "Trustee"         |
	| "87654321" | "Trustee"         |
	#---------------------------------- Managing Member ----------------------------------------
	| null       | "Managing Member" |
	| "TestFlag" | "Managing Member" |
	#Validation for CIMA ID to be only 7-Digits
	| "123456"   | "Managing Member" |
	| "87654321" | "Managing Member" |


@conditional @LastnameMissingandInvalid
Scenario Outline: Mandatory field validation for field "LastName" when hasCimaID is true and Fund operator is a Natural Person
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.fundOperators[0].type" to <fundOperatorType>
	And I set json field "data.fundOperators[0].naturalOrEntity" to "Natural Person"
	And I set json field "data.fundOperators[0].hasCimaId" to "false"
	And I set json field "data.fundOperators[0].lastName" to <value>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "Data.FundOperators[0].LastName"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value      | fundOperatorType  |
	#---------------------------------- Director ----------------------------------
	| null       | "director"        |
	| "TestFlag" | "director"        |
	#---------------------------------- General Partner ----------------------------------------
	| null       | "General Partner" |
	| "TestFlag" | "General Partner" |
	#---------------------------------- Trustee ----------------------------------------
	| null       | "Trustee"         |
	| "TestFlag" | "Trustee"         |
	#---------------------------------- Managing Member ----------------------------------------
	| null       | "Managing Member" |
	| "TestFlag" | "Managing Member" |
	
	
@conditional @isoCountry @countryOfOtherGaap @countryOfOtherGaas
Scenario Outline: Mandatory field validation for Country of Other GAAP/GAAS when corresponding principle/standard is "Other"
	Given I load request template "/api/v1/filings.json"
	And I set json field "<controllerField>" to "<controllerValue>"
	And I set json field "<dependentField>" to <country>
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "<responseField>"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| controllerField                               | controllerValue | dependentField                              | responseField                               | country |
	| data.fundCharacteristics.accountingPrinciples | Other GAAP      | data.fundCharacteristics.countryOfOtherGaap | Data.FundCharacteristics.CountryOfOtherGaap | null    |
	| data.fundCharacteristics.accountingPrinciples | Other GAAP      | data.fundCharacteristics.countryOfOtherGaap | Data.FundCharacteristics.CountryOfOtherGaap | ZZ      |
	| data.fundCharacteristics.accountingPrinciples | Other GAAP      | data.fundCharacteristics.countryOfOtherGaap | Data.FundCharacteristics.CountryOfOtherGaap | US      |
	| data.fundCharacteristics.auditingStandards    | Other GAAS      | data.fundCharacteristics.countryOfOtherGaas | Data.FundCharacteristics.CountryOfOtherGaas | null    |
	| data.fundCharacteristics.auditingStandards    | Other GAAS      | data.fundCharacteristics.countryOfOtherGaas | Data.FundCharacteristics.CountryOfOtherGaas | ZZ      |
	| data.fundCharacteristics.auditingStandards    | Other GAAS      | data.fundCharacteristics.countryOfOtherGaas | Data.FundCharacteristics.CountryOfOtherGaas | US      |

 
@conditional @OnshoreLegalCounsel @mandatoryAndInvalidValidation @hasPrimaryOnshoreLegalCounsel
Scenario Outline: Mandatory field validation for field "hasPrimaryOnshoreLegalCounsel" when usesInvestmentManagerInHouseAsOnshoreEquivalent is false
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent" to "false"
	And I set json field "data.onshoreLegalCounsel.hasPrimaryOnshoreLegalCounsel" to "<value>"
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "data.onshoreLegalCounsel.hasPrimaryOnshoreLegalCounsel"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value      |
	| null       |
	| "TestFlag" |
 
@conditional @OnshoreLegalCounsel @mandatoryAndInvalidValidation @onshoreLegalCounselName
Scenario Outline: Mandatory field validation for field "onshoreLegalCounselName" when hasPrimaryOnshoreLegalCounsel is true
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent" to "false"
	And I set json field "data.onshoreLegalCounsel.hasPrimaryOnshoreLegalCounsel" to "true"
	And I set json field "data.onshoreLegalCounsel.onshoreLegalCounselName" to "<value>"
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "data.onshoreLegalCounsel.OnshoreLegalCounselName"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value                                                                                                 |
	| null                                                                                                  |
	| "TestFlag"                                                                                            |
	#Validation for Max character length (allowed 2-100 characters)
	| "A"                                                                                                   |
	| OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation |
 
@conditional @OnshoreLegalCounsel @mandatoryAndInvalidValidation @isOnshorePartnerNameKnown
Scenario Outline: Mandatory field validation for field "isOnshorePartnerNameKnown" when onshoreLegalCounselName is not null
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent" to "false"
	And I set json field "data.onshoreLegalCounsel.hasPrimaryOnshoreLegalCounsel" to "true"
	And I set json field "data.onshoreLegalCounsel.onshoreLegalCounselName" to "Test Name"
	And I set json field "data.onshoreLegalCounsel.isOnshorePartnerNameKnown" to "<value>"
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field "Data.OnshoreLegalCounsel.isOnshorePartnerNameKnown"
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value  |
	| null   |
	| "Test" |
	
@conditional @OnshoreLegalCounsel @mandatoryAndInvalidValidation @onshorePartnerName
Scenario Outline: Mandatory field validation for field "onshorePartnerName" and "onshoreLegalCounselCountry" when isOnshorePartnerNameKnown is true
	Given I load request template "/api/v1/filings.json"
	And I set json field "data.onshoreLegalCounsel.usesInvestmentManagerInHouseAsOnshoreEquivalent" to "false"
	And I set json field "data.onshoreLegalCounsel.hasPrimaryOnshoreLegalCounsel" to "true"
	And I set json field "data.onshoreLegalCounsel.onshoreLegalCounselName" to "Test Name"
	And I set json field "data.onshoreLegalCounsel.isOnshorePartnerNameKnown" to "true"
	And I set json field "data.onshoreLegalCounsel.onshorePartnerName" to "<value>"
	And I set json field "data.onshoreLegalCounsel.onshoreLegalCounselCountry" to "<value>"
	When I POST "/api/v1/filings" using the current request json
	Then the response status should be 400
	And the response should include field :
		| Data.OnshoreLegalCounsel.onshorePartnerName         |
		| Data.OnshoreLegalCounsel.onshoreLegalCounselCountry |
	And the response field "code" should be "FIL-VAL-E400"

Examples:
	| value                                                                                                   |
	| null                                                                                                    |
	| "Test"                                                                                                  |
	#Validation for Max character length (allowed 2-100 characters)
	| "A"                                                                                                     |
	| "OnshoreLegalCounselInputStringValueMoreThanHundredCharactersForFieldOnshoreLegalCounselNameValidation" |


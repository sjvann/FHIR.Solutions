Feature: Type framework JSON serialization
  As a FHIR SDK maintainer
  I want primitives and elements to round-trip through JSON
  So that interoperability is preserved

  Scenario: A FHIR primitive serializes its value
    When I verify primitive JSON serialization

  Scenario: Serialize via the non-generic Base overload
    When I verify JSON serialization using the Base overload

  Scenario: Serializer options and deserialization round-trip
    When I verify serializer options and round-trip deserialization

  Scenario: Extension JSON contains a url field
    When I verify Extension JSON contains url

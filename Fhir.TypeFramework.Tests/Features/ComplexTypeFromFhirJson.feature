Feature: Complex datatype JSON (FHIR-shaped)
  As an integrator
  I want complex FHIR datatypes to parse from and emit JSON in FHIR conventions
  So that resources and elements interoperate with standard tooling

  Scenario: Deserialize all registered complex types from FHIR JSON
    When I verify all complex types deserialize from FHIR JSON

  Scenario: Serialize all registered complex types to FHIR-shaped JSON
    When I verify all complex types serialize to FHIR-shaped JSON

  Scenario: Round-trip all registered complex types
    When I verify all complex types JSON round-trip

  Scenario: Primitive JSON converter accepts legacy value object form
    When I verify primitive JSON converter legacy object form

  Scenario: Primitive JSON converter reads JSON boolean and number tokens
    When I verify primitive JSON converter boolean and number tokens

  Scenario: Primitive JSON converter reads null long integer and scientific decimal
    When I verify primitive JSON converter null long and scientific

  Scenario: Primitive JSON converter numeric read and write shapes
    When I verify primitive JSON converter numeric shapes

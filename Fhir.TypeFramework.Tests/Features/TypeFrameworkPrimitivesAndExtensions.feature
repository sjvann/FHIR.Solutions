Feature: Primitives and extensions
  As a FHIR SDK maintainer
  I want primitives and extensions to validate and compose correctly
  So that authoring resources remains predictable

  Scenario: Integer validation skips when unset
    When I verify integer validation skips when string value is null

  Scenario: Invalid id surfaces validation errors via extensions
    When I verify extension validation errors for an invalid id

  Scenario: Extension requires a url
    When I verify extension requires url

  Scenario: Primitive IValue semantics for nullable integers
    When I verify primitive IValue semantics

  Scenario: Element extension CRUD via extension methods
    When I verify extension CRUD on element

  Scenario: ValidateAndThrow throws for invalid id
    When I verify validate and throw for invalid id

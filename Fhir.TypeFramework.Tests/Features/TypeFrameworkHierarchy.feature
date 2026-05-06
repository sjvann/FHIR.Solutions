Feature: Backbone and resource hierarchy
  As a FHIR SDK maintainer
  I want backbone elements and resources to support extensions and equality
  So that the type hierarchy matches FHIR expectations

  Scenario: Backbone element and backbone type modifier extensions
    When I verify backbone element and backbone type modifier extensions

  Scenario: Backbone element equality and modifier extension validation
    When I verify backbone element equality and validation

  Scenario: Base explicit ITypeFramework deep copy and equality
    When I verify base explicit ITypeFramework methods

  Scenario: ComplexTypeBase deep copy list helper
    When I verify complex type base deep copy list

  Scenario: Boolean primitive parsing and helpers
    When I verify boolean primitive parsing and helpers

  Scenario: Element and primitive base class branches
    When I verify element primitive numeric and datetime base classes

  Scenario: String primitive and data type branches
    When I verify string primitive and data type branches

  Scenario: Resource equality and validation branches
    When I verify resource equality and validation branches

  Scenario: Base complex type resource and validation messages
    When I verify cover base complex type resource and messages

  Scenario: Element backbone type resource validation branches
    When I verify cover element backbone type resource validation branches

Feature: Coverage reflection probes
  As a FHIR SDK maintainer
  I want broad reflection-based probes to exercise generated surface area
  So that line coverage remains high for model-heavy assemblies

  Scenario: Touch data type properties via reflection
    When I touch all data type properties for coverage

  Scenario: Exercise concrete Base-derived types
    When I exercise all concrete base derived types

  Scenario: Exercise primitive JSON helpers across representative primitives
    When I exercise primitive type JSON helpers

  Scenario: Exercise performance and serialization utilities together
    When I exercise performance and serialization utilities

  Scenario: Invoke most public methods via reflection
    When I invoke most public methods via reflection

Feature: Performance-oriented utilities
  As a FHIR SDK maintainer
  I want optional performance utilities to behave safely
  So that hot paths can be optimized without breaking callers

  Scenario: Deep copy optimizer and performance monitoring do not throw
    When I verify deep copy optimizer and monitoring

  Scenario: Validation optimizer batch APIs aggregate results
    When I verify validation optimizer batch APIs

  Scenario: Type framework cache supports get-or-add and clear
    When I verify type framework cache get-or-add and clear

Feature: Type framework validation rules
  As a FHIR SDK maintainer
  I want validation helpers to follow FHIR constraints
  So that invalid data is rejected consistently

  Scenario: FHIR id format is enforced
    When I verify FHIR id validation rules

  Scenario: Basic reusable validation helpers work
    When I verify basic validation helpers

  Scenario: FHIR URI and code edge cases are handled
    When I verify FHIR URI and FHIR code validation branches

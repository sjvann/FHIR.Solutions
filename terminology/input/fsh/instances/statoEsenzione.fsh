Instance: statoEsenzione
InstanceOf: ValueSet
Usage: #definition
* text.status = #additional
* text.div = """<div xmlns="http://www.w3.org/1999/xhtml">
Value Set basato sul vocabolario HL7 V3 ActStatus che descrive lo stato delle esenzioni. Questo value set è adottato da HL7 CDA R2 IG 'DOCUMENTO DI ESENZIONE'
</div>
"""

* url = "http://terminology.hl7.it/ValueSet/statoEsenzione"
* version = "0.1.0"
* name = "VsStatoEsenzione"
* title = "Stato Esenzione"
* status = #active
* description = "Value Set basato sul vocabolario HL7 V3 ActStatus che descrive lo stato delle esenzioni. Questo value set è adottato da HL7 CDA R2 IG 'DOCUMENTO DI ESENZIONE'"
* compose.include[0].system = "http://terminology.hl7.org/CodeSystem/v3-ActStatus"
* compose.include[=].concept[0].extension.url = "http://hl7.org/fhir/StructureDefinition/valueset-concept-comments"
* compose.include[=].concept[=].extension.valueString = "Esenzione in corso di validità"
* compose.include[=].concept[=].code = #active
* compose.include[=].concept[=].display = "attivo"
* compose.include[=].concept[+].extension.url = "http://hl7.org/fhir/StructureDefinition/valueset-concept-comments"
* compose.include[=].concept[=].extension.valueString = "Esenzione momentaneamente sospesa (ad esempio in attesa del rinnovo di un’iscrizione temporanea)"
* compose.include[=].concept[=].code = #suspended
* compose.include[=].concept[=].display = "sospeso"
* compose.include[=].concept[+].extension.url = "http://hl7.org/fhir/StructureDefinition/valueset-concept-comments"
* compose.include[=].concept[=].extension.valueString = "Esenzione mai stata valida (ad esempio è stata assegnata per errore e il documento corrispondente era già stato prodotto in stato active)"
* compose.include[=].concept[=].code = #aborted
* compose.include[=].concept[=].display = "abortito"
* compose.include[=].concept[+].extension.url = "http://hl7.org/fhir/StructureDefinition/valueset-concept-comments"
* compose.include[=].concept[=].extension.valueString = "Esenzione non più in corso di validità"
* compose.include[=].concept[=].code = #completed
* compose.include[=].concept[=].display = "completato"
* compose.include[+].system = "http://terminology.hl7.org/CodeSystem/v3-NullFlavor"
* compose.include[=].concept.extension.url = "http://hl7.org/fhir/StructureDefinition/valueset-concept-comments"
* compose.include[=].concept.extension.valueString = "Lo stato dell'esenzione non è noto"
* compose.include[=].concept.code = #UNK
* compose.include[=].concept.display = "sconosciuto"
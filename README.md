# Email Function App

## Introduction

Proof-of-concept HTTP-triggered Azure Function to dynamically generate emails by injecting data into templates based on tokens encapsulated in `[]` pr `{}`.

It processes request payloads containing email templates and replaces these predefined tokens with their corresponding values:

- To
- Cc
- Subject
- Body

The `Tokens` and `RecipientTokens` were separated as I wanted to separate the recipient generation and email generation for flexibility. 

## Details

### Example Payload

```json
{
  "templates": {
    "to": ["Manager", "Employee"],
    "cc": ["HR"],
    "subject": "Onboarding: [EmployeeName]",
    "body": "<h1>Welcome {EmployeeName}</h1><p>Your start date is [StartDate]. You termination date will be [End Date].</p><p>Your manager's name is [ManagerName].</p>"
  },
  "tokens": {
    "ManagerEmail": "test@email.com",
    "EmployeeEmail": "test.manager@email.com",
    "ManagerName": "Homer Simpson",
    "EmployeeName": "Frank Grimes",
    "StartDate": "20/11/2025",
    "End Date": "20/11/2026",
    "Test": "Test"
  },
  "recipientTokens": {
    "Manager": "test.employee@email.com",
    "Employee": "test.manager@email.com",
    "HR": "hr@email.com"
  }
}
```

#### Payload Schema

|Property|Type|Description|
|:-|:-|:-|
|Templates|Object|**Required**. Contains the base email structure|
|Tokens|Dictionary|Key-value pairs mapping the tokens in `Subject` and `Body` to their string replacements|
|RecipientTokens|Dictionary| Key-value pairs mapping the identifiers in `To`/`Cc` to email addresses.

#### Templates Schema

|Property|Type|Description|
|:-|:-|:-|
|To|Array of Strings|A list of recipient identifiers|
|Cc|Array of Strings| A list of CC recipient identifiers|
|Subject|String|The email subject line containing tokens|
|Body|String|The main email body containing tokens|

### Example Response

```json
{
  "To": "test.employee@email.com;test.manager@email.com;test.employee@email.com",
  "Cc": "hr@email.com",
  "Subject": "Onboarding: Frank Grimes",
  "Body": "<h1>Welcome Frank Grimes</h1><p>Your start date is 20/11/2025. You termination date will be 20/11/2026.</p><p>Your manager's name is Homer Simpson.</p>"
}
```
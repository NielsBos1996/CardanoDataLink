# CardanoDataLink
This repository contains code Global LEI System (GLEIF)

## Assumptions
- Data is UTF-8 encoded
- Transaction notional has to be bigger than zero


## Requirements
- Process gets about 1-2k rows per data enrichment. For this amount of data the endpoint must return within a reasonable time
  - Enrichment takes place immediate after the user triggers it
- Data must be self explanatory
  - For the non happy flow (api is down, calculation failed, etc) the error should be returned to the user
- Input data must contain (simple) validation
  - Validate missing values, numbers < 0
- Application must be able to handle CSV input
- Application must return CSV output
- Added data
  - `legalName`
  - `bic`
  - `transaction_costs`
    - notial * rate - notial <-- if country is GB
    - Abs(notional * (1 / rate) - notional) <-- if country is NL
    - Unknown <-- for any other country


## Components
- Logger (LoggerInterface)
- HTTPGleifClient (GleifClientInterface)


## Entities
- Transaction (single CSV record)
- Dataset (holds Transaction[])

## Flow
- POST /api/data-enrichment
  - Data gets loaded into entities
    - If columns are missing, 400 BAD request is returned
    - 

## Steps taken
- Understand case, make requirements
- gleif api can retrieve multiple records with a comma-separated list https://api.gleif.org/api/v1/lei-records?filter[lei]=261700K5E45DJCF5Z735,4469000001AVO26P9X86,5493001KJTIIGC8Y1R12


## Answers to questions

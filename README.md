# CardanoDataLink
This repository contains code Global LEI System (GLEIF)

# Running the program
Running this application will start a http webserver on port 80.
## Local
Make sure you have dotnet 7 installed.

Start the application
```bash
cd CardanoDataLink
dotnet run
```

Run unit tests
```bash
dotnet test
```

## Docker
Start the application
```bash
docker-compose up -d cardanodatalink
```

Run unit tests
```bash
docker-compose up cardanodatalinktests
```

# Interacting with the program
The application implements a single endpoint, `/api/data-enrichment`. To use this endpoint, make a post request with a CSV file in the body.
The CSV file must contain the following fields
- transaction_uti
- isin,notional
- notional_currency
- transaction_type
- transaction_datetime
- rate
- lei

In case the application executed successfully, the response body will contain a CSV file with the added fields. A success response will have status code 200 OK.
- legal_name
- bic
- transaction_cost
- transaction_cost_explained

To get a feeling of how the output data looks, see [this file](./output.csv)

## Example script
For this script to run you will need to have python 3 installed, along with the libraries `pandas` and `requests`.
```python
import pandas as pd
from requests import post
import io
import sys

# define url of the application
url = 'http://localhost/api/data-enrichment'

# load data into python
data = pd.read_csv('./test_data.csv')

# make the api call
response = post(url, data=data.to_csv(index=False))

if response.status_code != 200:
    print(response.text) # api is unable to fetch data
    sys.exit(1)


new_data = pd.read_csv(io.StringIO(response.text))

# transform this column to have a list with bic values
new_data['bic'] = new_data['bic'].apply(lambda x: x.split(';'))

# uncomment to save the file to disk
# new_data.to_csv('./output.csv', index=False)
```

# Project Requirements
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
    - Empty if no bic available
    - In case multiple values are available, these values are seperated by a `;`
  - `transaction_costs`
    - notial * rate - notial <-- if country is GB
    - Abs(notional * (1 / rate) - notional) <-- if country is NL
    - Empty <-- for any other country
  - `transaction_cost_explained`
    - Calculation of the transaction_cost if possible
    - In case the transaction cost cannot be calculated the reason will be given

## Possible next steps
- Add authentication to this endpoint
- Start using CancellationToken
- Improve test coverage
  - Add integration tests

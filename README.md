# CardanoDataLink
This repository contains code Global LEI System (GLEIF)

# Usage
```python
import pandas as pd
from requests import post
import io
import sys

# define url of the application
url = 'http://localhost:5000/api/data-enrichment'

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
    - Unknown <-- for any other country
  - `reason`
    - In case the transaction cost cannot be calculated the reason will be given

## Possible next steps
- Add authentication to this endpoint
- Start using CancellationToken
- Improve test coverage
  - Add integration tests

## Flow
- POST /api/data-enrichment
todo: document a bit more / order this file

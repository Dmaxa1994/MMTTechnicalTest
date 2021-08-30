# MMTTechnicalTest

#MMTStoreAPI
Using VS2019 .Net Core 3.1 LTS
SQL Server 2019 database

Please note: There is no stored connection string for the database, this will need to be added after being cloned

3 data models in the project
- Store Items
- Item Categories
- Feature Item

Each has own table in database.
CRUD has been implemented for:
- Store Items
- Item Categories

Create, Read and Delete for:
- Feature Item

feautre item is a table with 2 columns of ids, 1 auto generated and 1 from Item Categories, a join is performed to get a readable name.
Implementing update on this did not seem reasonable.

Logs to console when entering a method and exiting an API route.
Logs when accessing the database
Logs on query success
In the event of an exception, the exception message is logged

API constraints
get:
- Store Items
   - Unable to search by multiple parameters, such as catergory and name, only able to use 1 parameter and featured item
- All returned values
  - Items returned are sorted by database with no option for custom sort
create:
Each request needs to sent induvidually, so 10 items needing adding will result in 10 calls

Future development
Redesign controller/context interaction, potentially the repository design, allowing unit tests.
Add unit tests
Expand Store Items to include tags for searching
Expand Store Items to to include stock level
Add task to alert user when stock of certain items are low
stock reporting at user defined intervals
Add multi-parameter search
Allow multiple items to be added at once through create methods, potentially using CSV uploads
Add promotions/sales price modifiers
Add Authorisation
Add connection strings for different databases based on if staging or live

**********************************************************************************************

HttpGet
 - Store Items
   - blank to retrieve all
   - using SKU
   - using product name
   - using product category
   - filter by featured item
   - Returns JSON string
- Item Categories
  - No parameters in call
  - Returns JSON string
- Feature Item
  - No parameters in call
  - Returns JSON string

returned status codes:
200
204
400
500

usage
Store Items
/api/MMTShop/Get/StoreItem
/api/MMTShop/Get/StoreItem?productCode=12345
/api/MMTShop/Get/StoreItem?productCode=12345&featuredItem=true

Item Categories
/api/MMTShop/Get/ItemCategories

Feature Item
/api/MMTShop/Get/FeatureItem

**********************************************************************************************

HttpPost : Create

All 3 create endpoints take in json passed in from the request body

returns staus codes:
200
400
409
500

usage
/api/MMTShop/Create/StoreItem
/api/MMTShop/Create/ItemCategories
/api/MMTShop/Create/FeatureItem

**********************************************************************************************

HttpPost : Update

Both end points take in json passed in from the request body as well as a parameter for ID

usage
/api/MMTShop/Create/StoreItem
/api/MMTShop/Create/ItemCategories

returned status codes:
200
204
400
500

**********************************************************************************************

HttpDelete

All endpoints route to the same method, ID parameter is required.

usage
/api/MMTShop/Create/StoreItem
/api/MMTShop/Create/ItemCategories
/api/MMTShop/Create/FeatureItem

returned status codes:
200
204
400
500

**********************************************************************************************

#MMTShopConsole
Using VS2019 .Net Core 3.1 LTS

Interface is controlled using up/down arrow keys and enter to select the item.
User is prompted for data entry on create and update.
On update and delete, the user is select the item from the list they want to alter/delete

All Get request results are displayed in JSON format
User is able to perform all CRUD tasks through the console on store items and item categories.
User is able to perform Create, Read, Delete on Feature Items

future development
Expand on classes as they grow in the api
Potentially merge the projects into 1, to prevent any mismatch in shared classes
Add utility to retrieve file for upload to api or read in content and convert to json for multi-create
Add login
Add auth token to http header






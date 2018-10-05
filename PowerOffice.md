# PowerOffice Go

API documentation: <https://api.poweroffice.net/Web/docs/index.html>.

## Terminology

| webCRM       | PowerOffice    |
|--------------|----------------|
| Organisation | Customer       |
| Person       | Contact person |

## C# SDK Not Used

PowerOffice Go has a C# SDK for their API, but it requires Newtonsoft.Json version 11.0.1, and when this project was started, Azure Function Apps were locked to version 10.0.3, making rendering the SDK incompatible. The latest version of Function Apps is no longer locked, but the code has not been updated to use the SDK. The reasons are that the SDK does not distinguish between objects for reading and writing, and that the methods aren't asynchronous.

## Organisations (Customers)

| Field                    | Comment                                                                                                        |
|--------------------------|----------------------------------------------------------------------------------------------------------------|
| Address, City & Zip code | Main Address in PowerOffice.                                                                                   |
| Country                  | Not synced, because PowerOffice uses country codes, whereas webCRM uses strings.                               |
| Billing address          | PowerOffice has a list of delivery addresses. We sync the primary delivery to a memo field in webCRM. One way. |
| Currency                 | Sync to a custom field in webCRM.                                                                              |
| Debitor group            | Skipping for now.                                                                                              |
| Debitor number           | One way sync to a custom field in webCRM.                                                                      |
| Devision name            | Not supported by PowerOffice, so skipping.                                                                     |
| EAN / IBAN               | Only sync the first bank account. Skipped for now.                                                             |
| Name                     | Name in webCRM.                                                                                                |
| Payment terms            | Not yet exposed through the PowerOffice API. Available on the invoices, though.                                |
| Phone number             | Phone number in webCRM.                                                                                        |
| VAT number               | Synced to a custom field in webCRM. (Danish: CVR-nummer.)                                                      |
| VAT group                | Does not exist in PowerOffice, so skipping.                                                                    |
| Website URL              | Website URL in webCRM.                                                                                         |
| Email address            | Email address in webCRM.                                                                                       |
| Invoice delivery type    | Setting this to email requires a valid email address.                                                          |

<https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_Customer.md>

- We do not sync organisations that are marked as `Archived`.
- We do not sync organisations that are marked as `IsPerson`.
- Fields possibly worth synchronising to webCRM: LegalName, IsVatFree, ContactGroups.
- A list of accepted organisation statuses and types is configured for each system. The first value in each list is the default value. It is possible to define values for both lists, but we expect that customers will only fill in values for one of the lists.
- One of the lists must have at least one value. This is required so that we have a way of identifying the organisations in webCRM that have to be synchronised to PowerOffice.
- Organisations are only syncronised if the status and type has an accepted value.
- When a new a newly created organisation in PowerOffice is copied to webCRM the status and type are set to the default values.
- webCRM requires that some values are trunkated, e.g. the name of the organisation. If this happens, then the name is trunkated in both systems.
- Syncing the payment terms will be a bit tricky, because webCRM requires the full string in order to set the value, e.g. `"07: 7 dage"`, but PowerOffice will most likely only expost the value, e.g. `7`.

## Persons (Contact Persons)

- Direct phone number.
- Email address
- Is key person. Custom checkbox field in webCRM.
- Mobile phone number. Ignore. PowerOffice only has a single phone number field.
- Name (first, last). PowerOffice does not have middle names. We assume that middle names have not been turning on in webCRM.

<https://api.poweroffice.net/Web/docs/index.html#Reference/Rest/Type_ContactPerson.md>

- We assume that the custom field in webCRM containing the reference ID is unique. It is **not** possible to mark a custom field on persons as unique.
- We assume that the middle name feature has not been activated in webCRM. If it has, that part of the name will not be synchronised.
- Only sync the primary contact person from PowerOffice. These persons are marked as primary persons in webCRM using a custom checkbox field ("Is key person"). The users are not meant to set the value of this field manually, and will check the checkbox every time a persons is copied from PowerOffice to webCRM.
- If the primary contact is changed in PowerOffice, the result is that two persons are now sync'ed between the two systems. It doesn't break anything.
- The modified timestamp of the organisation is updated when the primary contact person of an organisation in PowerOffice modified.
- The checkbox has to be a custom field set up a checkbox. CheckMark fields are not supported.

## Linked Data Items (Products)

Quotation lines are only sync'ed one way, from the ERP system to webCRM. The fields to synchronise will vary from client to client.

Most fields are synchronized to custom fields in webCRM, as text fields.

- Cost price.
- Discount.
- Memo (item description).
- Product name.
- Product no.
- Stock (amount).
- Unit.
- Unit price.
- VAT product code/number.
- Possibly more.

## Deliveries (Invoices)

We do not have two way synchronization of deliveries. The customers have to choose if they want to sync from PowerOffice to webCRM or from webCRM to PowerOffice.

We cannot assign responsible persons to the deliveries that we synchronise from PowerOffice, so Configuration > Main Settings > Data Fields > Deliveries > Responsible has to be disabled when synchronising from PowerOffice to webCRM.

The sales person should only be synchronised if we are able to find the person in both systems.

- Contribution margin.
- Currency.
- Debtor number.
- Description.
- Invoice date.
- Invoice number.
- Invoice value/revenue.
- Quotation lines for the order.
- Sales person (user).
- Standard discount.
- Possibly more.

## Quotation Lines (Invoice Lines)

Quotation lines have pretty much the same data as linked data items.

## Data Flow

Slightly different than the Fortnox flow, because when we search for upserted items in PowerOffice, we get the full item with all the properties. Except for the deliveries, that don't have any delivery lines.

The flow below describes data flowing from PowerOffice to webCRM. Flow in the other direction is the same - just switch webCRM with PowerOffice.

1. The PowerOffice heatbeat is triggered by a timer.
2. Get a list of all webCRM systems that have an integration to PowerOffice from the configurations database.
3. For each system:
   1. Get the date & time that we last checked for updates from the configuration.
   2. Fetch upserted items by quering for organisations, persons, products and deliveries.
   3. For each upserted item:
      1. Put a message on the PowerOffice queue containing the ID of the webCRM system, the type of the item and the upserted item itself.
4. For each message on the PowerOffice queue:
   1. Deserialize the message and get the configuration for the system using the webCRM system ID.
   2. Use the type of the item to deserialize the payload into that item.
   3. Look for the corresponding item in webCRM.
   4. If matching item found:
      1. If there are any relevant changes:
         1. Update the matching item in-memory.
         2. Save the updated matching item in webCRM.
   5. If no match found:
      1. Create the item in webCRM.

## Not Using Webhooks

While the webCRM API does have webhooks, were are polling regularely to get recently upserted items. The reasons for this are:

1. It's a simpler solution.
2. This way to the directions look more alike.
3. It's easier to test polling.
4. It's possible to catch-up if the system was down.
5. Possible to do mass synchronisations, where we get all the data from webCRM.
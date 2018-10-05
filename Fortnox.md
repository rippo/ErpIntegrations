# webCRM ERP Integrations - Fortnox Questions

## Terminology

| webCRM       | Fortnox        |
|--------------|----------------|
| Organisation | Customer       |
| Person       | Not available  |

## Copying a Fortnox customer to webCRM

### Custom Mappings per Client?

I know Andreas wanted the Fortnox customer invoice email address to map to webCRM organisation. In order to do this we would need a custom field created on the webCRM organisation.

From a coding perspective, do we need to think about having custom mappings per webCRM client? E.g.

    webcrmOrganisation.Custom11 = fortnoxCustomer.Email;

### Default Mappings?

We need management to provide a list of default mapping between FortNox and webCRM.

From a coding perspective we might then meed to modify the method. `CopyOrganisationProperties` in `FortnoxSyncrhoniser.cs`.

## Data Flow

Slightly different than the PowerOffice flow, because upserted Fortnox items only contain a partial list of their properties.

### From Fortnox to webCRM

The Fortnox API does not have webhooks, so we are polling regularly to get recently upserted (updated or inserted) items.

1. The Fortnox heatbeat is triggered by a timer.
2. Get a list of all webCRM systems that have an integration to Fortnox from the configurations database.
3. For each system:
   1. Get the date & time that we last checked for updates from the configuration (LastSuccessfulHeartbeat)
   2. Get upserted items.
   3. For each item:
      1. Put a message on the Fortnox queue containing the information needed to copy this item to webCRM.
   4. Update the configuration with the LastSuccessfulHeartbeat

```json
{
  "FortnoxCustomerNumber": "9",
  "webcrmSystemId": "27885"
}
```

5. For each message on the Fortnox queue:
   1. Get the configuration for the system.
   2. Get the item from Fortnox.
   3. Look for the corresponding item in webCRM.
   4. If found: Update the item in webCRM if necessary.
   5. If not found: Insert the item into webCRM.

### From webCRM to Fortnox

The webCRM API does has webhooks, but we are still polling regularely instead, because that setup is a lot simpler and easier to test.

We need to check a property to verify that we should update the organisation in Fortnox.

Fortnox expects a two letter ISO code e.g. `SE`. WebCrm only contains country names which are localised. Therefore is it not easy to map `Sweden` or `Sverige` to `SE`.
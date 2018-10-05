using System;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class DeliveryDto
    {
        public DeliveryDto()
        { }

        public DeliveryDto(
            OutgoingInvoice powerofficeDelivery,
            int webcrmOrganisationId,
            PowerofficeConfiguration configuration)
        {
            DeliveryAssignedTo = 0;
            DeliveryAssignedTo2 = 0;
            DeliveryResponsible = 0;

            Update(powerofficeDelivery, webcrmOrganisationId, configuration);
        }

        public Guid? GetPowerofficeDeliveryId(string deliveryIdFieldName)
        {
            string stringValue = this.GetPropertyValue(deliveryIdFieldName);
            if (!Guid.TryParse(stringValue, out Guid powerofficeDeliveryId))
            {
                return null;
            }

            return powerofficeDeliveryId;
        }

        public void SetPowerofficeDeliveryId(string deliveryIdFieldName, Guid powerofficeDeliveryId)
        {
            this.SetPropertyValue(deliveryIdFieldName, powerofficeDeliveryId.ToString());
        }

        public void Update(
            OutgoingInvoice powerofficeDelivery,
            int webcrmOrganisationId,
            PowerofficeConfiguration configuration)
        {
            DeliveryErpSyncDateTime = DateTime.UtcNow;
            DeliveryOrderDate = powerofficeDelivery.OrderDate;
            DeliveryOrganisationId = webcrmOrganisationId;

            SetPowerofficeDeliveryId(configuration.DeliveryIdFieldName, powerofficeDelivery.Id);
        }
    }
}
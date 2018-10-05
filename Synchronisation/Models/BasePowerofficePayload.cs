namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public abstract class BasePowerofficePayload
    {
        protected BasePowerofficePayload(
            string webcrmSystemId)
        {
            WebcrmSystemId = webcrmSystemId;
        }

        public string WebcrmSystemId { get; }
    }
}
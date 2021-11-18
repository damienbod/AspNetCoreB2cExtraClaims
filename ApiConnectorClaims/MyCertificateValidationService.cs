using System.Security.Cryptography.X509Certificates;

namespace ApiConnectorClaims
{
    public class MyCertificateValidationService 
    {
        private readonly ILogger<MyCertificateValidationService> _logger;

        public MyCertificateValidationService(ILogger<MyCertificateValidationService> logger)
        {
            _logger = logger;
        }

        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            return CheckIfThumbprintIsValid(clientCertificate);
        }

        private bool CheckIfThumbprintIsValid(X509Certificate2 clientCertificate)
        {
            var listOfValidThumbprints = new List<string>
            {
                // add thumprints of your allowed clients
                "15D118271F9AE7855778A2E6A00A575341D3D904"
            };

            if (listOfValidThumbprints.Contains(clientCertificate.Thumbprint))
            {
                _logger.LogInformation($"Custom auth-success for certificate  {clientCertificate.FriendlyName} {clientCertificate.Thumbprint}");

                return true;
            }

            _logger.LogWarning($"auth failed for certificate  {clientCertificate.FriendlyName} {clientCertificate.Thumbprint}");

            return false;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ApiConnectorClaims
{
    public class ApiConnectorCertificateValidationService
    {
        public bool ValidateCertificate(X509Certificate2 clientCertificate)
        {
            return CheckIfThumbprintIsValid(clientCertificate);
        }

        private bool CheckIfThumbprintIsValid(X509Certificate2 clientCertificate)
        {
            var listOfValidThumbprints = new List<string>
            {
                "15D118271F9AE7855778A2E6A00A575341D3D904"
            };

            if (listOfValidThumbprints.Contains(clientCertificate.Thumbprint))
            {
                return true;
            }

            return false;
        }
    }
}

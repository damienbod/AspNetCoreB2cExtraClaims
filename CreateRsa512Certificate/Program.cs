using CertificateManager;
using CertificateManager.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CreateIdentityServer4Certificates
{
    class Program
    {
        static CreateCertificates _cc;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();
            var configuration = builder.Build();

            var sp = new ServiceCollection()
               .AddCertificateManager()
               .BuildServiceProvider();

            _cc = sp.GetService<CreateCertificates>();

            var rsaCert = CreateRsaCertificateSha512KeySize2048("localhost", 10);

            string password = configuration["certificateSecret"];
            var iec = sp.GetService<ImportExportCertificate>();

            var rsaCertPfxBytes = iec.ExportSelfSignedCertificatePfx(password, rsaCert);
            File.WriteAllBytes("cert_rsa512.pfx", rsaCertPfxBytes);

            Console.WriteLine("created");
        }

        public static X509Certificate2 CreateRsaCertificateSha512KeySize2048(string dnsName, int validityPeriodInYears)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = false,
                HasPathLengthConstraint = false,
                PathLengthConstraint = 0,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    dnsName,
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.DigitalSignature;

            // only if certification authentication is used
            var enhancedKeyUsages = new OidCollection
            {
                OidLookup.ClientAuthentication,
                // OidLookup.ServerAuthentication 
                // OidLookup.CodeSigning,
                // OidLookup.SecureEmail,
                // OidLookup.TimeStamping  
            };

            var certificate = _cc.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = dnsName },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(validityPeriodInYears)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                { 
                    KeySize = 2048,
                    HashAlgorithmName = HashAlgorithmName.SHA512
                });

            return certificate;
        }
    }
}

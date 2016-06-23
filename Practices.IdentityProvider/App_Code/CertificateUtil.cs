namespace Practices.IdentityProvider {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Web;

    public static class CertificateUtil {
        public static X509Certificate2 GetCertificate(StoreName storeName, StoreLocation storeLocation, string subjectName) {
            X509Store store = new X509Store(storeName, storeLocation);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);
            try {
                X509Certificate2 result = null;
                //
                // Every time we call store.Certificates property, a new collection will be returned.
                //
                certificates = store.Certificates;
                for (int i = 0; i < certificates.Count; i++) {
                    X509Certificate2 cert = certificates[i];

                    if (cert.SubjectName.Name.ToLower() == subjectName.ToLower()) {
                        if (result != null)
                            throw new ApplicationException(string.Format("There are multiple certificate for subject Name {0}", subjectName));

                        result = new X509Certificate2(cert);
                    }
                }
                if (result == null) {
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                }
                return result;
            } finally {
                if (certificates != null) {
                    for (int i = 0; i < certificates.Count; i++) {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }
                store.Close();
            }
        }

        public static X509Certificate2 GetCertificate(string certificatePath, string certificatePassword) {
            certificatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatePath);
            X509Certificate2 certificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.PersistKeySet);
            return certificate;
        }

        public static X509Certificate2 GetCertificate(string certificatePath) {
            certificatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatePath);
            X509Certificate2 certificate = new X509Certificate2(certificatePath);
            return certificate;
        }
    }
}
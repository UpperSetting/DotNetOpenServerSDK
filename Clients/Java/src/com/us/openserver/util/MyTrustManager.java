package com.us.openserver.util;

import java.security.*;
import java.security.cert.*;
import javax.net.ssl.*;

public class MyTrustManager implements X509TrustManager {

    private X509TrustManager standardTrustManager = null;

    public MyTrustManager(KeyStore keystore)
        throws NoSuchAlgorithmException, KeyStoreException
    {
        super();
        TrustManagerFactory factory = TrustManagerFactory.getInstance(TrustManagerFactory.getDefaultAlgorithm());
        factory.init(keystore);
        TrustManager[] trustManagers = factory.getTrustManagers();
        if (trustManagers.length == 0)
            throw new NoSuchAlgorithmException("Default trust manager algorithm not found.");

        this.standardTrustManager = (X509TrustManager) trustManagers[0];
    }

    public void checkClientTrusted(X509Certificate[] certificates, String authType)
        throws CertificateException
    {
        standardTrustManager.checkClientTrusted(certificates, authType);
    }

    public void checkServerTrusted(X509Certificate[] certificates, String authType)
        throws CertificateException
    {
        if ((certificates != null) && (certificates.length == 1))
            certificates[0].checkValidity();
        else
            standardTrustManager.checkServerTrusted(certificates, authType);
    }

    public X509Certificate[] getAcceptedIssuers()
    {
        return this.standardTrustManager.getAcceptedIssuers();
    }
}

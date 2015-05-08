package com.us.openserver.configuration;

public class TlsConfiguration
{
    private boolean enabled;
    private String certificate;
    private boolean requireRemoteCertificate;
    private boolean allowSelfSignedCertificate;
    private boolean checkCertificateRevocation;
    private boolean allowCertificateChainErrors;

    public boolean getAllowCertificateChainErrors()
    {
        return allowCertificateChainErrors;
    }

    public void setAllowCertificateChainErrors(boolean value)
    {
    	allowCertificateChainErrors = value;
    }

    public boolean getCheckCertificateRevocation()
    {
        return checkCertificateRevocation;
    }

    public void setCheckCertificateRevocation(boolean value)
    {
    	checkCertificateRevocation = value;
    }

    public boolean getAllowSelfSignedCertificate()
    {
        return allowSelfSignedCertificate;
    }

    public void setAllowSelfSignedCertificate(boolean value)
    {
    	allowSelfSignedCertificate = value;
    }

    public boolean getRequireRemoteCertificate()
    {
        return requireRemoteCertificate;
    }

    public void setRequireRemoteCertificate(boolean value)
    {
        requireRemoteCertificate = value;
    }

    public String getCertificate()
    {
        return certificate;
    }

    public void setCertificate(String value)
    {
    	certificate = value;
    }

    public boolean isEnabled()
    {
        return enabled;
    }

    public void setEnabled(boolean value)
    {
    	enabled = value;
    }
}

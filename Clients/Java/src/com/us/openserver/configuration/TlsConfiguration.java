/*
Copyright 2015 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/

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

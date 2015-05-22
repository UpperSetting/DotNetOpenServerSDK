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

namespace US.OpenServer.Protocols
{
    public enum CapabilitiesProtocolCommands : byte
    {
        GET_PROTOCOL_IDS              = 0x01,
        PROTOCOL_IDS                  = 0x02,
        ERROR                         = 0xFF,        
    }

    public enum CapabilitiesProtocolErrors : byte
    {
        PROTOCOL_NOT_CONFIGURED         = 0x01,
        PROTOCOL_CLASS_NOT_FOUND        = 0x02,
        PROTOCOL_NOT_AUTHENTICATED      = 0x03,
        PROTOCOL_INITIALIZATION_FAILED  = 0x04,
        GENERAL                         = 0xFF,     
    }
}

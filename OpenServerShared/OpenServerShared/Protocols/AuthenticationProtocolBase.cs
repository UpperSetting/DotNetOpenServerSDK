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

using System;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Base class for all authentication protocol implementations.
    /// </summary>
    public abstract class AuthenticationProtocolBase : IProtocol
    {
        /// <summary>
        /// Gets or sets whether user has been authenticated.
        /// </summary>
        /// <value>A Boolean that specifies whether the user has been
        /// authenticated.</value>
        public bool IsAuthenticated { get; protected set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        /// <value>A string that specifies the user's name.</value>
        public string UserName { get; protected set; }

        /// <summary>
        /// Checks if the authenticated user is a member of the passed role.
        /// </summary>
        /// <param name="role">A String that contains the name of the role.</param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}

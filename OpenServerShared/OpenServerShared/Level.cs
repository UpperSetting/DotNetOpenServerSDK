/*
Copyright 2015-2016 Upper Setting Corporation

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


namespace US.OpenServer
{
    /// <summary>
    /// Enum of log message priorities.
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Specifies a debug message.
        /// </summary>
        Debug,
        /// <summary>
        /// Specifies an information message.
        /// </summary>
        Info,
        /// <summary>
        /// Specifies a notice message.
        /// </summary>
        Notice,
        /// <summary>
        /// Specifies a critical message.
        /// </summary>
        Critical,
        /// <summary>
        /// Specifies an error message.
        /// </summary>
        Error
    }
}

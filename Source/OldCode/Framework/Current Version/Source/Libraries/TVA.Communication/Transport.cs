//*******************************************************************************************************
//  Transport.cs - Gbtc
//
//  Tennessee Valley Authority, 2009
//  No copyright is claimed pursuant to 17 USC � 105.  All Other Rights Reserved.
//
//  This software is made freely available under the TVA Open Source Agreement (see below).
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  06/01/2006 - Pinal C. Patel
//       Original version of source created.
//  09/29/2008 - J. Ritchie Carroll
//       Converted to C#.
//  08/22/2009 - Pinal C. Patel
//       Modified CreateEndPoint() to try parsing IP address first before doing a DNS lookup.
//  09/08/2009 - Pinal C. Patel
//       Modified CreateSocket() to create a socket for the AddressFamily of the endpoint.
//       Modified CreateEndPoint() to use IPv6 if supported when no IP address is specified.
//  09/14/2009 - Stephen C. Wills
//       Added new header and license agreement.
//  10/30/2009 - Pinal C. Patel
//       Added IsIPv6IP() and IsMulticastIP() methods.
//       Fixed bug in CreateSocket() that was breaking one-way communication support in UDP components.
//  04/29/2010 - Pinal C. Patel
//       Added EndpointFormatRegex constant to be used for parsing endpoint strings.
//
//*******************************************************************************************************

#region [ TVA Open Source Agreement ]
/*

 THIS OPEN SOURCE AGREEMENT ("AGREEMENT") DEFINES THE RIGHTS OF USE,REPRODUCTION, DISTRIBUTION,
 MODIFICATION AND REDISTRIBUTION OF CERTAIN COMPUTER SOFTWARE ORIGINALLY RELEASED BY THE
 TENNESSEE VALLEY AUTHORITY, A CORPORATE AGENCY AND INSTRUMENTALITY OF THE UNITED STATES GOVERNMENT
 ("GOVERNMENT AGENCY"). GOVERNMENT AGENCY IS AN INTENDED THIRD-PARTY BENEFICIARY OF ALL SUBSEQUENT
 DISTRIBUTIONS OR REDISTRIBUTIONS OF THE SUBJECT SOFTWARE. ANYONE WHO USES, REPRODUCES, DISTRIBUTES,
 MODIFIES OR REDISTRIBUTES THE SUBJECT SOFTWARE, AS DEFINED HEREIN, OR ANY PART THEREOF, IS, BY THAT
 ACTION, ACCEPTING IN FULL THE RESPONSIBILITIES AND OBLIGATIONS CONTAINED IN THIS AGREEMENT.

 Original Software Designation: openPDC
 Original Software Title: The TVA Open Source Phasor Data Concentrator
 User Registration Requested. Please Visit https://naspi.tva.com/Registration/
 Point of Contact for Original Software: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>

 1. DEFINITIONS

 A. "Contributor" means Government Agency, as the developer of the Original Software, and any entity
 that makes a Modification.

 B. "Covered Patents" mean patent claims licensable by a Contributor that are necessarily infringed by
 the use or sale of its Modification alone or when combined with the Subject Software.

 C. "Display" means the showing of a copy of the Subject Software, either directly or by means of an
 image, or any other device.

 D. "Distribution" means conveyance or transfer of the Subject Software, regardless of means, to
 another.

 E. "Larger Work" means computer software that combines Subject Software, or portions thereof, with
 software separate from the Subject Software that is not governed by the terms of this Agreement.

 F. "Modification" means any alteration of, including addition to or deletion from, the substance or
 structure of either the Original Software or Subject Software, and includes derivative works, as that
 term is defined in the Copyright Statute, 17 USC � 101. However, the act of including Subject Software
 as part of a Larger Work does not in and of itself constitute a Modification.

 G. "Original Software" means the computer software first released under this Agreement by Government
 Agency entitled openPDC, including source code, object code and accompanying documentation, if any.

 H. "Recipient" means anyone who acquires the Subject Software under this Agreement, including all
 Contributors.

 I. "Redistribution" means Distribution of the Subject Software after a Modification has been made.

 J. "Reproduction" means the making of a counterpart, image or copy of the Subject Software.

 K. "Sale" means the exchange of the Subject Software for money or equivalent value.

 L. "Subject Software" means the Original Software, Modifications, or any respective parts thereof.

 M. "Use" means the application or employment of the Subject Software for any purpose.

 2. GRANT OF RIGHTS

 A. Under Non-Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor,
 with respect to its own contribution to the Subject Software, hereby grants to each Recipient a
 non-exclusive, world-wide, royalty-free license to engage in the following activities pertaining to
 the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Modification

 5. Redistribution

 6. Display

 B. Under Patent Rights: Subject to the terms and conditions of this Agreement, each Contributor, with
 respect to its own contribution to the Subject Software, hereby grants to each Recipient under Covered
 Patents a non-exclusive, world-wide, royalty-free license to engage in the following activities
 pertaining to the Subject Software:

 1. Use

 2. Distribution

 3. Reproduction

 4. Sale

 5. Offer for Sale

 C. The rights granted under Paragraph B. also apply to the combination of a Contributor's Modification
 and the Subject Software if, at the time the Modification is added by the Contributor, the addition of
 such Modification causes the combination to be covered by the Covered Patents. It does not apply to
 any other combinations that include a Modification. 

 D. The rights granted in Paragraphs A. and B. allow the Recipient to sublicense those same rights.
 Such sublicense must be under the same terms and conditions of this Agreement.

 3. OBLIGATIONS OF RECIPIENT

 A. Distribution or Redistribution of the Subject Software must be made under this Agreement except for
 additions covered under paragraph 3H. 

 1. Whenever a Recipient distributes or redistributes the Subject Software, a copy of this Agreement
 must be included with each copy of the Subject Software; and

 2. If Recipient distributes or redistributes the Subject Software in any form other than source code,
 Recipient must also make the source code freely available, and must provide with each copy of the
 Subject Software information on how to obtain the source code in a reasonable manner on or through a
 medium customarily used for software exchange.

 B. Each Recipient must ensure that the following copyright notice appears prominently in the Subject
 Software:

          No copyright is claimed pursuant to 17 USC � 105.  All Other Rights Reserved.

 C. Each Contributor must characterize its alteration of the Subject Software as a Modification and
 must identify itself as the originator of its Modification in a manner that reasonably allows
 subsequent Recipients to identify the originator of the Modification. In fulfillment of these
 requirements, Contributor must include a file (e.g., a change log file) that describes the alterations
 made and the date of the alterations, identifies Contributor as originator of the alterations, and
 consents to characterization of the alterations as a Modification, for example, by including a
 statement that the Modification is derived, directly or indirectly, from Original Software provided by
 Government Agency. Once consent is granted, it may not thereafter be revoked.

 D. A Contributor may add its own copyright notice to the Subject Software. Once a copyright notice has
 been added to the Subject Software, a Recipient may not remove it without the express permission of
 the Contributor who added the notice.

 E. A Recipient may not make any representation in the Subject Software or in any promotional,
 advertising or other material that may be construed as an endorsement by Government Agency or by any
 prior Recipient of any product or service provided by Recipient, or that may seek to obtain commercial
 advantage by the fact of Government Agency's or a prior Recipient's participation in this Agreement.

 F. In an effort to track usage and maintain accurate records of the Subject Software, each Recipient,
 upon receipt of the Subject Software, is requested to register with Government Agency by visiting the
 following website: https://naspi.tva.com/Registration/. Recipient's name and personal information
 shall be used for statistical purposes only. Once a Recipient makes a Modification available, it is
 requested that the Recipient inform Government Agency at the web site provided above how to access the
 Modification.

 G. Each Contributor represents that that its Modification does not violate any existing agreements,
 regulations, statutes or rules, and further that Contributor has sufficient rights to grant the rights
 conveyed by this Agreement.

 H. A Recipient may choose to offer, and to charge a fee for, warranty, support, indemnity and/or
 liability obligations to one or more other Recipients of the Subject Software. A Recipient may do so,
 however, only on its own behalf and not on behalf of Government Agency or any other Recipient. Such a
 Recipient must make it absolutely clear that any such warranty, support, indemnity and/or liability
 obligation is offered by that Recipient alone. Further, such Recipient agrees to indemnify Government
 Agency and every other Recipient for any liability incurred by them as a result of warranty, support,
 indemnity and/or liability offered by such Recipient.

 I. A Recipient may create a Larger Work by combining Subject Software with separate software not
 governed by the terms of this agreement and distribute the Larger Work as a single product. In such
 case, the Recipient must make sure Subject Software, or portions thereof, included in the Larger Work
 is subject to this Agreement.

 J. Notwithstanding any provisions contained herein, Recipient is hereby put on notice that export of
 any goods or technical data from the United States may require some form of export license from the
 U.S. Government. Failure to obtain necessary export licenses may result in criminal liability under
 U.S. laws. Government Agency neither represents that a license shall not be required nor that, if
 required, it shall be issued. Nothing granted herein provides any such export license.

 4. DISCLAIMER OF WARRANTIES AND LIABILITIES; WAIVER AND INDEMNIFICATION

 A. No Warranty: THE SUBJECT SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTY OF ANY KIND, EITHER
 EXPRESSED, IMPLIED, OR STATUTORY, INCLUDING, BUT NOT LIMITED TO, ANY WARRANTY THAT THE SUBJECT
 SOFTWARE WILL CONFORM TO SPECIFICATIONS, ANY IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 PARTICULAR PURPOSE, OR FREEDOM FROM INFRINGEMENT, ANY WARRANTY THAT THE SUBJECT SOFTWARE WILL BE ERROR
 FREE, OR ANY WARRANTY THAT DOCUMENTATION, IF PROVIDED, WILL CONFORM TO THE SUBJECT SOFTWARE. THIS
 AGREEMENT DOES NOT, IN ANY MANNER, CONSTITUTE AN ENDORSEMENT BY GOVERNMENT AGENCY OR ANY PRIOR
 RECIPIENT OF ANY RESULTS, RESULTING DESIGNS, HARDWARE, SOFTWARE PRODUCTS OR ANY OTHER APPLICATIONS
 RESULTING FROM USE OF THE SUBJECT SOFTWARE. FURTHER, GOVERNMENT AGENCY DISCLAIMS ALL WARRANTIES AND
 LIABILITIES REGARDING THIRD-PARTY SOFTWARE, IF PRESENT IN THE ORIGINAL SOFTWARE, AND DISTRIBUTES IT
 "AS IS."

 B. Waiver and Indemnity: RECIPIENT AGREES TO WAIVE ANY AND ALL CLAIMS AGAINST GOVERNMENT AGENCY, ITS
 AGENTS, EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT. IF RECIPIENT'S USE
 OF THE SUBJECT SOFTWARE RESULTS IN ANY LIABILITIES, DEMANDS, DAMAGES, EXPENSES OR LOSSES ARISING FROM
 SUCH USE, INCLUDING ANY DAMAGES FROM PRODUCTS BASED ON, OR RESULTING FROM, RECIPIENT'S USE OF THE
 SUBJECT SOFTWARE, RECIPIENT SHALL INDEMNIFY AND HOLD HARMLESS  GOVERNMENT AGENCY, ITS AGENTS,
 EMPLOYEES, CONTRACTORS AND SUBCONTRACTORS, AS WELL AS ANY PRIOR RECIPIENT, TO THE EXTENT PERMITTED BY
 LAW.  THE FOREGOING RELEASE AND INDEMNIFICATION SHALL APPLY EVEN IF THE LIABILITIES, DEMANDS, DAMAGES,
 EXPENSES OR LOSSES ARE CAUSED, OCCASIONED, OR CONTRIBUTED TO BY THE NEGLIGENCE, SOLE OR CONCURRENT, OF
 GOVERNMENT AGENCY OR ANY PRIOR RECIPIENT.  RECIPIENT'S SOLE REMEDY FOR ANY SUCH MATTER SHALL BE THE
 IMMEDIATE, UNILATERAL TERMINATION OF THIS AGREEMENT.

 5. GENERAL TERMS

 A. Termination: This Agreement and the rights granted hereunder will terminate automatically if a
 Recipient fails to comply with these terms and conditions, and fails to cure such noncompliance within
 thirty (30) days of becoming aware of such noncompliance. Upon termination, a Recipient agrees to
 immediately cease use and distribution of the Subject Software. All sublicenses to the Subject
 Software properly granted by the breaching Recipient shall survive any such termination of this
 Agreement.

 B. Severability: If any provision of this Agreement is invalid or unenforceable under applicable law,
 it shall not affect the validity or enforceability of the remainder of the terms of this Agreement.

 C. Applicable Law: This Agreement shall be subject to United States federal law only for all purposes,
 including, but not limited to, determining the validity of this Agreement, the meaning of its
 provisions and the rights, obligations and remedies of the parties.

 D. Entire Understanding: This Agreement constitutes the entire understanding and agreement of the
 parties relating to release of the Subject Software and may not be superseded, modified or amended
 except by further written agreement duly executed by the parties.

 E. Binding Authority: By accepting and using the Subject Software under this Agreement, a Recipient
 affirms its authority to bind the Recipient to all terms and conditions of this Agreement and that
 Recipient hereby agrees to all terms and conditions herein.

 F. Point of Contact: Any Recipient contact with Government Agency is to be directed to the designated
 representative as follows: J. Ritchie Carroll <mailto:jrcarrol@tva.gov>.

*/
#endregion

using System;
using System.Net;
using System.Net.Sockets;

namespace TVA.Communication
{
    /// <summary>
    /// A helper class containing methods related to server-client communication.
    /// </summary>
    public static class Transport
    {
        /// <summary>
        /// Specifies the lowest valid port number for a <see cref="Socket"/>.
        /// </summary>
        public const int PortRangeLow = 0;

        /// <summary>
        /// Specifies the highest valid port number for a <see cref="Socket"/>.
        /// </summary>
        public const int PortRangeHigh = 65535;

        /// <summary>
        /// Regular expression used to validate the format for an endpoint.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Matches the following valid input:<br/>
        /// - localhost:80<br/>
        /// - 127.0.0.1:80<br/>
        /// - [::1]:80<br/>
        /// - [FEDC:BA98:7654:3210:FEDC:BA98:7654:3210]:80
        /// </para>
        /// </remarks>
        public const string EndpointFormatRegex = @"(?<host>.+)\:(?<port>\d+$)";

        /// <summary>
        /// Creates an <see cref="IPEndPoint"/> for the specified host name and port number.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <param name="port">The port number to be associated with the address.</param>
        /// <returns>An <see cref="IPEndPoint"/> object.</returns>
        public static IPEndPoint CreateEndPoint(string hostNameOrAddress, int port)
        {
            if (string.IsNullOrEmpty(hostNameOrAddress))
            {
                // Use all of the local IPs.
                if (Socket.OSSupportsIPv6)
                    return new IPEndPoint(IPAddress.IPv6Any, port);
                else
                    return new IPEndPoint(IPAddress.Any, port);
            }
            else
            {
                IPAddress address;
                if (IPAddress.TryParse(hostNameOrAddress, out address))
                    // Use the provided IP address.
                    return new IPEndPoint(address, port);
                else
                    // Exception will occur if DNS lookup fails.
                    return new IPEndPoint(Dns.GetHostEntry(hostNameOrAddress).AddressList[0], port);
            }
        }

        /// <summary>
        /// Creates a <see cref="Socket"/> for the specified <paramref name="port"/> and <paramref name="protocol"/>.
        /// </summary>
        /// <param name="address">The local address where the <see cref="Socket"/> will be bound.</param>
        /// <param name="port">The port number at which the <see cref="Socket"/> will be bound.</param>
        /// <param name="protocol">One of the <see cref="ProtocolType"/> values.</param>
        /// <returns>An <see cref="Socket"/> object.</returns>
        public static Socket CreateSocket(string address, int port, ProtocolType protocol)
        {
            Socket socket = null;
            IPEndPoint endpoint = null;
            switch (protocol)
            {
                case ProtocolType.Tcp:
                    endpoint = Transport.CreateEndPoint(address, port);
                    socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    socket.Bind(endpoint);
                    break;
                case ProtocolType.Udp:
                    // Allow negative port number to be specified for unbound socket.
                    if (port >= 0)
                    {
                        endpoint = Transport.CreateEndPoint(address, port);
                        socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                        socket.Bind(endpoint);
                    }
                    else
                    {
                        endpoint = Transport.CreateEndPoint(address, 0);
                        socket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} is not supported", protocol));
            }
            return socket;
        }

        /// <summary>
        /// Determines if the specified <paramref name="ipAddress"/> is an IPv6 IP.
        /// </summary>
        /// <param name="ipAddress">IP address to check.</param>
        /// <returns>true if the <paramref name="ipAddress"/> is IPv6 IP; otherwise false.</returns>
        public static bool IsIPv6IP(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");

            if (ipAddress.ToString().Contains(":"))
                // IP is a IPV6 IP.
                return true;
            else
                // IP is a IPV4 IP.
                return false;
        }

        /// <summary>
        /// Determines if the specified <paramref name="ipAddress"/> is a multicast IP.
        /// </summary>
        /// <param name="ipAddress">IP address to check.</param>
        /// <returns>true if the <paramref name="ipAddress"/> is multicast IP; otherwise false.</returns>
        public static bool IsMulticastIP(IPAddress ipAddress)
        {
            if (ipAddress == null)
                throw new ArgumentNullException("ipAddress");

            if (Transport.IsIPv6IP(ipAddress))
            {
                // IP is a IPV6 IP.
                return ipAddress.IsIPv6Multicast;
            }
            else
            {
                // IP is a IPV4 IP.
                int firstOctet = int.Parse(ipAddress.ToString().Split('.')[0]);
                if (firstOctet >= 224 && firstOctet <= 247)
                    // IP is a Class D multicast IP.
                    return true;
                else
                    // IP is not a multicast IP.
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified port is valid.
        /// </summary>
        /// <param name="port">The port number to be validated.</param>
        /// <returns>True if the port number is valid.</returns>
        public static bool IsPortNumberValid(string port)
        {
            int portNumber;

            if (int.TryParse(port, out portNumber))
            {
                // The specified port is a valid integer value.
                if (portNumber >= PortRangeLow && portNumber <= PortRangeHigh)
                    // The port number is within the valid range.
                    return true;
                else
                    return false;
            }
            else
            {
                throw new ArgumentException("Port number is not a valid number");
            }
        }

        /// <summary>
        /// Determines if the specified UDP destination is listening for data.
        /// </summary>
        /// <param name="targetIPEndPoint">The <see cref="IPEndPoint"/> for the UDP destination to be checked.</param>
        /// <returns>true if the UDP destination is listening for data; otherwise false.</returns>
        public static bool IsDestinationReachable(IPEndPoint targetIPEndPoint)
        {
            try
            {
                // We'll check if the target endpoint exist by sending empty data to it and then wait for data from it.
                // If the endpoint doesn't exist then we'll receive a ConnectionReset socket exception.
                EndPoint targetEndPoint = (EndPoint)targetIPEndPoint;
                using (Socket targetChecker = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    targetChecker.ReceiveTimeout = 1;
                    targetChecker.SendTo(new byte[] { }, targetEndPoint);
                    targetChecker.ReceiveFrom(new byte[] { }, ref targetEndPoint);
                }

            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        // This means that the target endpoint is unreachable.
                        return false;
                }
            }
            catch
            {
                // We'll ignore any other exceptions we might encounter.
            }

            return true;
        }
    }
}
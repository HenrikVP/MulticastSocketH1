﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MulticastSocketH1
{
    internal class MC1
    {
        // This is the listener example that shows how to use the MulticastOption class.
        // In particular, it shows how to use the MulticastOption(IPAddress, IPAddress)
        // constructor, which you need to use if you have a host with more than one
        // network card.
        // The first parameter specifies the multicast group address, and the second
        // specifies the local address of the network card you want to use for the data
        // exchange.
        // You must run this program in conjunction with the sender program as
        // follows:
        // Open a console window and run the listener from the command line.
        // In another console window run the sender. In both cases you must specify
        // the local IPAddress to use. To obtain this address run the ipconfig command
        // from the command line.
        //
        private static IPAddress mcastAddress;
        private static int mcastPort;
        private static Socket mcastSocket;
        private static MulticastOption mcastOption;

        private static void MulticastOptionProperties()
        {
            Console.WriteLine("Current multicast group is: " + mcastOption.Group);
            Console.WriteLine("Current multicast local address is: " + mcastOption.LocalAddress);
        }


        private static void StartMulticast()
        {

            try
            {
                mcastSocket = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Dgram,
                                         ProtocolType.Udp);

                Console.Write("Enter the local IP address: ");

                IPAddress localIPAddr = IPAddress.Parse(Console.ReadLine());

                //IPAddress localIP = IPAddress.Any;
                EndPoint localEP = (EndPoint)new IPEndPoint(localIPAddr, mcastPort);

                mcastSocket.Bind(localEP);


                // Define a MulticastOption object specifying the multicast group
                // address and the local IPAddress.
                // The multicast group address is the same as the address used by the server.
                mcastOption = new MulticastOption(mcastAddress, localIPAddr);

                mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                            SocketOptionName.AddMembership,
                                            mcastOption);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveBroadcastMessages()
        {
            bool done = false;
            byte[] bytes = new Byte[100];
            IPEndPoint groupEP = new IPEndPoint(mcastAddress, mcastPort);
            EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (!done)
                {
                    Console.WriteLine("Waiting for multicast packets.......");
                    Console.WriteLine("Enter ^C to terminate.");

                    mcastSocket.ReceiveFrom(bytes, ref remoteEP);

                    Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                      groupEP.ToString(),
                      Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }

                mcastSocket.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Start()
        {
            // Initialize the multicast address group and multicast port.
            // Both address and port are selected from the allowed sets as
            // defined in the related RFC documents. These are the same
            // as the values used by the sender.
            mcastAddress = IPAddress.Parse("224.168.100.2");
            mcastPort = 11000;

            // Start a multicast group.
            StartMulticast();

            // Display MulticastOption properties.
            MulticastOptionProperties();

            // Receive broadcast messages.
            ReceiveBroadcastMessages();
        }
    }
}




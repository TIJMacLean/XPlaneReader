using System;
using System.Globalization;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace UDPTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                string myFileName = String.Format("{0}__{1}", DateTime.Now.ToString("yyyyMMddhhmmss"), "data" + ".txt");
                // string myFullPath = System.IO.Path.Combine("C:\\Users\\Tom.Maclean\\source\\repos\\XPlaneReader\\XPlaneReader", myFileName);
                string myFullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, myFileName);

                byte[] data = new byte[1024];
                string headings = "Time | N/A | N/A | N/A | N/A | N/A | N/A | _Vind,_kias | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | elev,stick | ailrn,stick | ruddr,stick | N/A | N/A | N/A | N/A | N/A | N/A | ___Q,rad/s | ___P,rad/s | ___R,rad/s | N/A | N/A | N/A | N/A | N/A | N/A | pitch,__deg | _roll,__deg | N/A | hding,__mag | N/A | N/A | N/A | N/A | N/A | N/A | _beta,__deg | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | __alt,ftmsl | N/A | N/A | N/A | N/A | N/A | N/A | thro1,_part | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | trq_1,ftlb | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | rpm_1,_prop | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | ptch1,__deg | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N1__1,_pcnt | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N2__1,_pcnt | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | FF_1,__galh | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | ITT_1,__deg | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | N/A | v-spd,__fpm | N/A | N/A | N/A | N/A | N/A | N/A |";

                System.IO.File.WriteAllText(myFullPath, headings);

                Console.WriteLine("X-Plane Data Read: \n");
                Console.WriteLine("Press any key to start recording \n\n");
                Console.ReadKey(true);

                // int i = 0

                // while (i < 10)
                while (Console.KeyAvailable == false)
                {
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 49005);
                    UdpClient newsock = new UdpClient(ipep);
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 49005);
                    data = newsock.Receive(ref sender);

                    for (int index = 5; index < data.Length; index++)   // Skip first five characters "DATA*"
                    {
                        // Console.Write("{0},", data[index]);
                    }

                    string DecimalToHex = BitConverter.ToString(data).Replace("-", "");    // Convert to hex output

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(myFullPath, true))
                    {
                        file.WriteLine("");
                    }

                    for (int index = 34; index < DecimalToHex.Length; index = index + 8)    // Skip first five characters "DATA*" (10) and first two useless times (3*8)
                    {
                        string HexRep = DecimalToHex.Substring(index, 8);   //Convert to hex but now need to change from DCBA to ABCD

                        char[] phraseAsChars = HexRep.ToCharArray();
                        char[] endianChar = new char[8];
                        endianChar[0] = phraseAsChars[6];
                        endianChar[1] = phraseAsChars[7];
                        endianChar[2] = phraseAsChars[4];
                        endianChar[3] = phraseAsChars[5];
                        endianChar[4] = phraseAsChars[2];
                        endianChar[5] = phraseAsChars[3];
                        endianChar[6] = phraseAsChars[0];
                        endianChar[7] = phraseAsChars[1];

                        string HexActual = new string(endianChar);

                        Int32 IntRep = Int32.Parse(HexActual, NumberStyles.AllowHexSpecifier);
                        // Integer to Byte[] and presenting it for float conversion
                        float f = BitConverter.ToSingle(BitConverter.GetBytes(IntRep), 0);
                        // There you go

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(myFullPath, true))
                        {
                            file.Write(f.ToString("N3"));
                            file.Write(" | ");
                        }

                        Console.Write("{0}", f);
                        Console.Write(" | ");
                    }

                    Console.Write("\n");
                    // i++;
                    newsock.Close();
                    Thread.Sleep(100);  // Wait 100ms (10 outputs per second)
                }

                Console.WriteLine("\n The file has been saved as:");
                Console.WriteLine(myFileName);
                Console.WriteLine("Press the spacebar to record again, or any other key to quit \n\n\n\n");
                Console.ReadKey(true);
                ConsoleKeyInfo checkQuit;
                checkQuit = Console.ReadKey(true);
                if (checkQuit.Key == ConsoleKey.Spacebar)
                { exit = false; }
                else
                {
                    exit = true;
                }
 

            }
        }

    }
}
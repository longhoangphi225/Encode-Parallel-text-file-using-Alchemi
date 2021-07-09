using System;
using System.IO;
using System.Text;
using Alchemi.Core;
using Alchemi.Core.Owner;

namespace HelloWorld
{
    class Program
    {
        [Serializable]
        class Encode : GThread
        {
            public readonly int[] Candidate = new int[BytesPerThread];
            public int STT = -1;

            public Encode(int[] candidate, int stt)
            {
                for (int i = 0; i < candidate.Length; i++)
                    Candidate[i] = candidate[i];
                STT = stt;

            }
            public override void Start()
            {
                for (int i = 0; i < Candidate.Length; i++)
                {
                    if (Candidate[i] == 255)
                        break;
                    else
                        Candidate[i] = (Candidate[i] * 2 + 1) % 127;
                }
            }
        }
        public static class Globals
        {
            public static byte[] A;
        }

        static int NumThread = 10;
        static int BytesPerThread;
        static int dem = 0;
        class Generator
        {
            public static GApplication App = new GApplication();
            private static DateTime start;

            [STAThread]
            static void Main(string[] args)
            {
                Console.WriteLine("[Integral Computation Grid Application]\n--------------------------------\n");
                Console.Write("Nhap so luong: ");
                NumThread = Convert.ToInt32(Console.ReadLine());
                Globals.A = File.ReadAllBytes("D:\\Hoc tap\\20202\\TTSS\\testalchemi.txt");
                BytesPerThread = (Globals.A.Length / (NumThread)) + 1;
                Console.WriteLine("\nKich thuoc cua file la {0} bytes", Globals.A.Length);
                Console.WriteLine("Chia moi luong {0} bytes", BytesPerThread);
                Console.WriteLine("\n---------- Start ----------\n");

                if (BytesPerThread > NumThread)
                {
                    int[] B = new int[BytesPerThread];

                    for (int i = 0; i < NumThread - 1; i++)
                    {
                        for (int j = 0; j < BytesPerThread; j++)
                        {
                            B[j] = Convert.ToInt32(Globals.A[i * BytesPerThread + j]);

                        }
                        App.Threads.Add(new Encode(B, i));
                    }

                    for (int j = 0; j < (BytesPerThread); j++)
                    {
                        B[j] = 255;
                    }

                    try
                    {
                        for (int j = 0; j < BytesPerThread - 1; j++)
                        {

                            B[j] = Convert.ToInt32(Globals.A[(NumThread - 1) * BytesPerThread + j]);
                        }
                    }
                    catch 
                    {

                    }

                    App.Threads.Add(new Encode(B, (NumThread - 1)));
                    App.Connection = new GConnection("localhost", 9000, "user", "user");
                    App.Manifest.Add(new ModuleDependency(typeof(Encode).Module));
                    App.ThreadFinish += new GThreadFinish(App_ThreadFinish);
                    App.ApplicationFinish += new GApplicationFinish(App_ApplicationFinish);
                    start = DateTime.Now;
                    App.Start();

                    while (dem < NumThread)
                    {

                    }

                    App.Stop();
                    Console.WriteLine("\n---------- Finish ----------");
                    Console.ReadLine();

                    char[] c = new char[Globals.A.Length];

                    for (int i = 0; i < c.Length; i++)
                    {
                        c[i] = Convert.ToChar(Globals.A[i]);

                    }

                    FileStream sb = new FileStream("D:\\Hoc tap\\20202\\TTSS\\output.txt", FileMode.OpenOrCreate);
                    
                    StreamWriter sw = new StreamWriter(sb);
                    sw.Write(c, 0, c.Length);
                    sw.Close();
                }
                else
                    Console.WriteLine("Chuong trinh khong the chay vi BytesPerThread < NumThread ({0} < {1})",
                           BytesPerThread, NumThread);
            }

            private static void App_ApplicationFinish()
            {
                Console.WriteLine("Hoan thanh sau {0} seconds.", DateTime.Now - start);
            }

            private static void App_ThreadFinish(GThread thread)
            {
                Encode enc = (Encode)thread;
                dem += 1;
                Console.WriteLine("Da xu ly xong {0} luong", dem);
                if(enc.STT == NumThread - 1)
                for (int i = 0; i < Globals.A.Length - (NumThread - 1) * BytesPerThread; i++)
                {
                        Globals.A[enc.STT * (BytesPerThread) + i] = (byte)enc.Candidate[i];
                }
                else
                for (int i = 0; i < enc.Candidate.Length; i++)
                {                  
                        Globals.A[enc.STT * (BytesPerThread) + i] = (byte)enc.Candidate[i];
                }
            }
        }
    }
}

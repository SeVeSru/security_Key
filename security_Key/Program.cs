using System;
using System.Security.Cryptography;
using System.Text;
using System.Numerics;

namespace security_Key
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.WriteLine("Введите сообщение");
            string M = Console.ReadLine();

            int q, p, b;
            bool prost;
            do
            {
                q = random.Next(32769, 65535);
                prost = isPrime(q);
            } while (!prost);
            do
            {
                b = random.Next(16384, 32768);
                p = (b * q) + 1;
                prost = isPrime(p);
            } while(!prost);
            
            int a;
            do
            {
                int g = random.Next(2,p-2);
                BigInteger gb = BigInteger.ModPow(g,b,p);
                a = (int)(gb);
            } while (a <= 1);

            Console.WriteLine("a=" + a + " p=" + p + " q=" + q);

            int x = random.Next(1, q-1);
            BigInteger ax = BigInteger.ModPow(a, x, p);
            int y = (int)(ax);
            Console.WriteLine("Секретное число x=" + x);
            Console.WriteLine("Открытый ключ y=" + y);

            long h = GetHash(M);
            long[] CriptKey = Crypt(a, p, q, h, x);
            Console.WriteLine("<" + M + "," + CriptKey[0] + "," + CriptKey[1] + ">");

            bool res = Check(CriptKey[0], CriptKey[1], q, a, y, p);

            Console.WriteLine(res);


            Console.ReadKey();
        }

        static long[] Crypt(int a, int p, int q, long h, int x)
        {
            long[] res = new long[2];
            Random random = new Random();
            long k, r = 0, s = 0;
            while (r <= 0 || s <= 0)
            {
                k = random.Next(1, q-1);
                //double gb = (Math.Pow(a, k) % p) % q;
                BigInteger gb = BigInteger.ModPow(a, k, p)%q;
                r = (int)gb;
                s = (k * h + x * r) % q;
            }
            res[0] = r;
            res[1] = s;

            return res;

        }
        /*Не удается сделать правильно проверку
         Вроде все верно считает, формулы правильно
        Но в ручную когда считаю u2 не верно высчитывается*/
        static bool Check(long r, long s, int q, int a, int y, int p)
        {
            if (s<0 || s>q || r<0 || r>q) return false;
            
            string M1;
            Console.Write("Введите сообщение: ");
            M1 = Console.ReadLine();
            long h = GetHash(M1);

            //double h1 = Math.Pow(h, -1);
            long h1 = ModPow(h, q);
            long u1 = s * h1 % q;
            long u2 = -r * h1 % q;
            while(u2<=0){
                u2 += q;
            }
            //double v = ((Math.Pow(a, u1) * Math.Pow(y, u2)) % p) % q;

            //double h1 = 1 / h;
            //int r1 = 0 - r;
            //double w = Math.Pow(h, -1) % q;
            //double u1 = (s * w) % q;
            //double u2 = ( q - r ) * w % q;
            //BigInteger v1 = ((BigInteger.Pow(a, (int)u1) * BigInteger.Pow(y, (int)u2)) % p) % q;
            BigInteger v1 = ((BigInteger.ModPow(a, (int)u1, p) * BigInteger.ModPow(y, (int)u2, p)) %p) % q;
            int v = (int)v1;
            /*
            ((a^u1*y*u2)%p)%q= (((a^u1%p)*(y^u2%p))%p)%q

            */
            //BigInteger v = BigInteger.Pow(h, q - 2) % q;
            //BigInteger z1 = s * v % q;
            //BigInteger z2 = (q - r) * v % q;
            //BigInteger u = ((BigInteger.Pow(a, (int)(z1)) * BigInteger.Pow(y, (int)(z2))) % p) % q;

            Console.WriteLine(h + " " + r + " " + s + " " + M1 + " " + u1 + " " + u2 + " " + v);
            if (v == r) return true;
            else return false;
        }

        static long GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            long h = 0;

            for (int i = 0; i < hash.Length; i++)
            {
                h += hash[i];
            }

            //return Convert.ToBase64String(hash);
            return h;
        }
        static bool isPrime(int n)
        {
            if (n > 1)
            {
                for (int i = 2; i < n; i++)
                    if (n % i == 0)
                        return false;
                return true;
            }
            else
                return false;
        }

        private static long ModPow(long a, long b)
        {
            long uf1, uf2, vf1, vf2, tf1, tf2;

            uf1 = a;
            uf2 = 0;
            vf1 = b;
            vf2 = 1;

            long q;

            while (vf1 != 0)
            {
                q = uf1 / vf1;
                tf1 = uf1 % vf1;
                tf2 = uf2 - (q * vf2);
                uf1 = vf1;
                uf2 = vf2;
                vf1 = tf1;
                vf2 = tf2;
            }

            if (uf2 < 0)
                uf2 += b;
            return uf2;

        }

    }
}

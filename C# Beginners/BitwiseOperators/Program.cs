using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitwiseOperators
{
    public enum SomeValues
    {
        Red = 1,
        Blue = 2,
        Green = 4,
        Black = 8,
        White = 16,
        Orange = 32,
        Yellow = 64,
        Pink = 128,
    }

    class Program
    {
        static void Main(string[] args)
        {
            //
            //   Binary
            //

            //
            //   Bitwise operators
            //
            //      And     & (Both)
            //      Or      | (Either)
            //      Xor     ^ (Exclusive or, different)
            //      Not     ~ (Invert)
            //    

            byte a = 122;
            byte b = 7;

            byte result = (byte)(a & b);

            //Console.WriteLine($"{ Convert.ToString(a, 2).PadLeft(8, '0')} &");
            //Console.WriteLine($"{ Convert.ToString(b, 2).PadLeft(8, '0')}");
            //Console.WriteLine($"--------");
            //Console.WriteLine($"{ Convert.ToString(result, 2).PadLeft(8, '0')}");
            //Console.WriteLine();

            //
            //   Bitwise Shifting
            //
            //      Left    <<
            //      Right   >>
            //      


            //byte c = 25;

            //var cResult = (byte)(c << 4);

            //Console.WriteLine($"{ Convert.ToString(c, 2).PadLeft(8, '0')} << 1");
            //Console.WriteLine($"--------");
            //Console.WriteLine($"{ Convert.ToString(cResult, 2).PadLeft(8, '0')}");


            //   Usage
            //   
            //      Toggling boolean
            //      Enum flags
            //      Masking
            //

            // Invert booleans
            var d = true;
            d ^= true;

            // Enum flags
            var someVals = (byte)(SomeValues.Blue);
            Console.WriteLine($"{ Convert.ToString((byte)someVals, 2).PadLeft(8, '0')}");

            if ((someVals & (byte)SomeValues.Blue) == (byte)SomeValues.Blue)
                Console.WriteLine("Blue was included");
            if ((someVals & (byte)SomeValues.White) == (byte)SomeValues.White)
                Console.WriteLine("White was included");

            // Masking 
            //
            // -----1- Input
            // 0000010 < Important bit (the mask)

            // 0000010

            var input = (byte)(SomeValues.White | SomeValues.Blue);
            var mask = (byte)SomeValues.Blue;
            var r = input & mask;


            Console.ReadLine();
        }
    }
}

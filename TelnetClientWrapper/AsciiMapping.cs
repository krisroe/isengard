﻿using System.Collections.Generic;
namespace IsengardClient
{
    public static class AsciiMapping
    {
        public const int ASCII_SPACE = 32;
        public const int ASCII_LEFT_PAREN = 40;
        public const int ASCII_RIGHT_PAREN = 41;
        public const int ASCII_NUMBER_ZERO = 48;
        public const int ASCII_NUMBER_NINE = 57;
        public const int ASCII_COLON = 58;
        public const int ASCII_UPPERCASE_A = 65;
        public const int ASCII_UPPERCASE_C = 67;
        public const int ASCII_UPPERCASE_E = 69;
        public const int ASCII_UPPERCASE_H = 72;
        public const int ASCII_UPPERCASE_I = 73;
        public const int ASCII_UPPERCASE_M = 77;
        public const int ASCII_UPPERCASE_T = 84;
        public const int ASCII_UPPERCASE_V = 86;
        public const int ASCII_LEFT_BRACKET = 91;
        public const int ASCII_RIGHT_BRACKET = 93;

        public static Dictionary<char, int> GetAsciiMapping(out Dictionary<int, char> reverseAsciiMapping)
        {
            Dictionary<char, int> ret = new Dictionary<char, int>();
            ret['\t'] = 9;
            ret[' '] = ASCII_SPACE;
            ret['!'] = 33;
            ret['"'] = 34;
            ret['#'] = 35;
            ret['%'] = 37;
            ret['&'] = 38;
            ret['\''] = 39;
            ret['('] = ASCII_LEFT_PAREN;
            ret[')'] = ASCII_RIGHT_PAREN;
            ret['*'] = 42;
            ret['+'] = 43;
            ret[','] = 44;
            ret['-'] = 45;
            ret['.'] = 46;
            ret['/'] = 47;
            ret['0'] = ASCII_NUMBER_ZERO;
            ret['1'] = 49;
            ret['2'] = 50;
            ret['3'] = 51;
            ret['4'] = 52;
            ret['5'] = 53;
            ret['6'] = 54;
            ret['7'] = 55;
            ret['8'] = 56;
            ret['9'] = ASCII_NUMBER_NINE;
            ret[':'] = ASCII_COLON;
            ret[';'] = 59;
            ret['<'] = 60;
            ret['>'] = 62;
            ret['?'] = 63;
            ret['@'] = 64;
            ret['A'] = ASCII_UPPERCASE_A;
            ret['B'] = 66;
            ret['C'] = ASCII_UPPERCASE_C;
            ret['D'] = 68;
            ret['E'] = ASCII_UPPERCASE_E;
            ret['F'] = 70;
            ret['G'] = 71;
            ret['H'] = ASCII_UPPERCASE_H;
            ret['I'] = ASCII_UPPERCASE_I;
            ret['J'] = 74;
            ret['K'] = 75;
            ret['L'] = 76;
            ret['M'] = ASCII_UPPERCASE_M;
            ret['N'] = 78;
            ret['O'] = 79;
            ret['P'] = 80;
            ret['Q'] = 81;
            ret['R'] = 82;
            ret['S'] = 83;
            ret['T'] = ASCII_UPPERCASE_T;
            ret['U'] = 85;
            ret['V'] = ASCII_UPPERCASE_V;
            ret['W'] = 87;
            ret['X'] = 88;
            ret['Y'] = 89;
            ret['Z'] = 90;
            ret['['] = ASCII_LEFT_BRACKET;
            ret[']'] = ASCII_RIGHT_BRACKET;
            ret['_'] = 95;
            ret['`'] = 96;
            ret['a'] = 97;
            ret['b'] = 98;
            ret['c'] = 99;
            ret['d'] = 100;
            ret['e'] = 101;
            ret['f'] = 102;
            ret['g'] = 103;
            ret['h'] = 104;
            ret['i'] = 105;
            ret['j'] = 106;
            ret['k'] = 107;
            ret['l'] = 108;
            ret['m'] = 109;
            ret['n'] = 110;
            ret['o'] = 111;
            ret['p'] = 112;
            ret['q'] = 113;
            ret['r'] = 114;
            ret['s'] = 115;
            ret['t'] = 116;
            ret['u'] = 117;
            ret['v'] = 118;
            ret['w'] = 119;
            ret['x'] = 120;
            ret['y'] = 121;
            ret['z'] = 122;
            ret['|'] = 124;

            reverseAsciiMapping = new Dictionary<int, char>();
            foreach (KeyValuePair<char, int> next in ret)
            {
                reverseAsciiMapping[next.Value] = next.Key;
            }

            return ret;
        }
    }
}

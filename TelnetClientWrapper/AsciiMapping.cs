using System.Collections.Generic;
namespace IsengardClient
{
    public static class AsciiMapping
    {
        public static Dictionary<char, int> GetAsciiMapping(out Dictionary<int, char> reverseAsciiMapping)
        {
            Dictionary<char, int> ret = new Dictionary<char, int>();
            ret[' '] = 32;
            ret['!'] = 33;
            ret['"'] = 34;
            ret['#'] = 35;
            ret['%'] = 37;
            ret['\''] = 39;
            ret['('] = 40;
            ret[')'] = 41;
            ret['*'] = 42;
            ret['+'] = 43;
            ret[','] = 44;
            ret['-'] = 45;
            ret['.'] = 46;
            ret['/'] = 47;
            ret['0'] = 48;
            ret['1'] = 49;
            ret['2'] = 50;
            ret['3'] = 51;
            ret['4'] = 52;
            ret['5'] = 53;
            ret['6'] = 54;
            ret['7'] = 55;
            ret['8'] = 56;
            ret['9'] = 57;
            ret[':'] = 58;
            ret[';'] = 59;
            ret['?'] = 63;
            ret['@'] = 64;
            ret['A'] = 65;
            ret['B'] = 66;
            ret['C'] = 67;
            ret['D'] = 68;
            ret['E'] = 69;
            ret['F'] = 70;
            ret['G'] = 71;
            ret['H'] = 72;
            ret['I'] = 73;
            ret['J'] = 74;
            ret['K'] = 75;
            ret['L'] = 76;
            ret['M'] = 77;
            ret['N'] = 78;
            ret['O'] = 79;
            ret['P'] = 80;
            ret['Q'] = 81;
            ret['R'] = 82;
            ret['S'] = 83;
            ret['T'] = 84;
            ret['U'] = 85;
            ret['V'] = 86;
            ret['W'] = 87;
            ret['X'] = 88;
            ret['Y'] = 89;
            ret['Z'] = 90;
            ret['['] = 91;
            ret[']'] = 93;
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

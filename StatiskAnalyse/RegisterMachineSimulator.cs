using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StatiskAnalyse
{
    public class RegisterMachineSimulator
    {
        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _dictType = new Dictionary<string, string>();
        private string _latestResult = "";
        private string _latestResultType = "";

        public string Get(string register)
        {
            if (register.StartsWith("p"))
                return "?(" + register + ")";
            string ret;
            return _dict.TryGetValue(register, out ret) ? ret : $"?({register})";
        }
        public string GetType(string register)
        {
            string ret;
            return _dictType.TryGetValue(register, out ret) ? ret : $"?({register})";
        }

        public void ParseLine(string line)
        {
            var m = Util.ConstantStringRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, m.Groups[3].Value, "string");
                return;
            }

            m = Util.ConstantNumberRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[3].Value, m.Groups[4].Value, m.Groups[4].Value.Contains(".") ? "float" : "int");
                return;
            }

            m = Util.NewInstanceRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, "object", m.Groups[2].Value);
                return;
            }

            m = Util.NewArrayRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, "array", $"{m.Groups[3].Value}[{Get(m.Groups[2].Value)}]");
                return;
            }

            m = Util.MoveResultRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, _latestResult, _latestResultType);
                return;
            }

            m = Util.MoveRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[3].Value, Get(m.Groups[4].Value), GetType(m.Groups[4].Value));
                return;
            }

            m = Util.InvokeRegex.Match(line);
            if (m.Success)
            {
                UpdateLatestResult($"?({m.Groups[4].Value}->{m.Groups[5].Value}({m.Groups[6].Value}))", m.Groups[7].Value);
                return;
            }
            
            m = Util.AGetRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, $"?({m.Groups[3].Value}[{Get(m.Groups[4].Value)}])", GetType(m.Groups[3].Value).Substring(1));
                return;
            }

            m = Util.IGetRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, $"?({m.Groups[4].Value}->{m.Groups[5].Value})", m.Groups[6].Value);
                return;
            }

            m = Util.SGetRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, $"?({m.Groups[2].Value}->{m.Groups[3].Value})", m.Groups[4].Value);
                return;
            }

            m = Util.ConversionRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, Get(m.Groups[3].Value), m.Groups[1].Value);
                return;
            }
        }

        private void UpdateLatestResult(string val, string type)
        {
            _latestResult = val;
            _latestResultType = type;
        }

        private void UpdateRegister(string reg, string val, string type)
        {
            _dict[reg] = val;
            _dictType[reg] = type;
        }
    }
}
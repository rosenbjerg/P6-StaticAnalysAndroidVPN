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
                return "\\" + register;
            string ret;
            return _dict.TryGetValue(register, out ret) ? ret : "";
        }
        public string GetType(string register)
        {
            string ret;
            return _dictType.TryGetValue(register, out ret) ? ret : "";
        }

        public void ParseLine(string line)
        {
            var m = Util.ConstantStringRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, m.Groups[3].Value, "Ljava/lang/String");
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
                UpdateRegister(m.Groups[1].Value, "new-instance", m.Groups[2].Value);
                return;
            }

            m = Util.NewArrayRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, "new-array", $"{m.Groups[3].Value}[{Get(m.Groups[2].Value)}]");
                return;
            }

            m = Util.MoveResultRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[1].Value, _latestResult, _latestResultType);
                return;
            }

            m = Util.InvokeVirtualRegex.Match(line);
            if (m.Success)
            {
                UpdateLatestResult("Object", m.Groups[5].Value);
                return;
            }

            m = Util.InvokeStaticRegex.Match(line);
            if (m.Success)
            {
                UpdateLatestResult("Object", m.Groups[4].Value);
                return;
            }

            m = Util.AGetRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, "Object", GetType(m.Groups[3].Value).Substring(1));
                return;
            }

            m = Util.IGetRegex.Match(line);
            if (m.Success)
            {
                UpdateRegister(m.Groups[2].Value, "Object", m.Groups[6].Value);
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
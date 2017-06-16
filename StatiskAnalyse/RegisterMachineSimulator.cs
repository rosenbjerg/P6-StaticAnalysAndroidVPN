using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StatiskAnalyse
{
    class RegisterMachineSimulator
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
            Match m;
            m = Util.ConstantStringRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, m.Groups[2].Value, "Ljava/lang/String");
            m = Util.NewInstanceRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, "new-instance", m.Groups[2].Value);
            m = Util.MoveResultObjectRegex.Match(line);
            if (m.Success)
                UpdateRegister(m.Groups[1].Value, _latestResult, _latestResultType);
            m = Util.InvokeVirtualRegex.Match(line);
            if (m.Success)
            {
                var t = m.Groups[5].Value;
                _latestResult = "Object";
                _latestResultType = t;
            }
            m = Util.InvokeStaticRegex.Match(line);
            if (m.Success)
            {
                var t = m.Groups[4].Value;
                _latestResult = "Object";
                _latestResultType = t;
            }

            //m = Util.InvokeVirtualRegex.Match(line);
            //if (m.Success)
            //{
            //    var v = m.Groups[3].Value;
            //}

        }

        private void UpdateRegister(string reg, string val, string type)
        {
            _dict[reg] = val;
            _dictType[reg] = type;
        }
    }
}
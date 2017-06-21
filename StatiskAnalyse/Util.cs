using System.Text.RegularExpressions;

namespace StatiskAnalyse
{
    internal static class Util
    {
        private const string Reg = "(v\\d+)";
        private const string RegOrParam = "([vp]\\d+)";
        private const string Parameters = "([^}{]*)";
        private const string Type = "([^ :;]+)";
        private const string FieldOrMethod = "([^:\\(]+)";
        private const string InputTypes = "([^ :]*)";


        public static Regex ConstantStringRegex =
            new Regex($" *const-string(/jumbo)? " + Reg + ", \"(.+)\"", 
                RegexOptions.Compiled);

        public static Regex ConstantNumberRegex =
            new Regex($" *const(-wide)?(\\/[^ ])? (v\\d+), ([^ ]+)", 
                RegexOptions.Compiled);

        public static Regex NewInstanceRegex =
            new Regex(" *new-instance " + Reg + ", " + Type + ";?", 
                RegexOptions.Compiled);

        public static Regex NewArrayRegex =
            new Regex(" *new-array (v\\d+), ([vp]\\d+), ([^ :;]+);?", 
                RegexOptions.Compiled);

        public static Regex MoveResultRegex = 
            new Regex(" *move-result-[a-z]+ " + Reg + ";?",
                RegexOptions.Compiled);

        public static Regex MoveRegex =
            new Regex(" *move(-object|-wide)?(/\\w+])? " + Reg + ", " + RegOrParam + ";?",
                RegexOptions.Compiled);

        public static Regex InvokeRegex =
            new Regex(" *invoke-([a-z]+)(/range)? {" + Parameters + "}, " + Type + ";->" + FieldOrMethod + "\\(" + InputTypes + "\\)" + Type + ";?",
                RegexOptions.Compiled);

        public static Regex ConversionRegex =
            new Regex(" *[a-z]+-to-([a-z]+) " + Reg + ", " + RegOrParam + ";?",
                RegexOptions.Compiled);

        public static Regex ClassRegex = 
            new Regex("\\.class ?(public)? ?(final)? ([a-z]+)? ?" + Type + ";", 
                RegexOptions.Compiled);

        public static Regex MethodRegex =
            new Regex("\\.method ?([a-z]+)? ?([a-z]+)? ?([a-z]+)? ?" + FieldOrMethod + "\\(" + InputTypes + "\\)" + Type + ";?",
                RegexOptions.Compiled);

        public static Regex IGetRegex =
            new Regex(" *iget(-[a-z]+)? " + Reg + ", " + RegOrParam + ", " + Type + ";->" + FieldOrMethod + ":" + Type + ";?", 
                RegexOptions.Compiled);

        public static Regex SGetRegex =
            new Regex(" *sget[^ ]* (v\\d+), " + Type + ";->" + FieldOrMethod + ":" + Type + ";?", 
                RegexOptions.Compiled);

        public static Regex APutRegex =
            new Regex(" *aput(-[a-z]+)? " + Reg + ", " + RegOrParam + ", " + RegOrParam,
                RegexOptions.Compiled);

        public static Regex AGetRegex =
            new Regex(" *aget(-[a-z]+)? " + Reg + ", " + RegOrParam + ", " + RegOrParam,
                RegexOptions.Compiled);

        public static Regex FilledNewArrayRegex =
            new Regex(" *filled-new-array {([^}{]*)},([^ :;]+);?",
                RegexOptions.Compiled);
    }
}
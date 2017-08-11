using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttpServerApp
{
    /// <summary>
    /// Utiltiy to parse routes based on a token format.
    /// </summary>
    /// <remarks>
    /// Casing doesn't matter!
    /// </remarks>
    /// <example>
    /// <code>
    /// var parser = new RouteParser("{protocol}://mydomain.com/{itemCategory}/{itemId}");
    /// var variables = parser.Variables; //should be .Count == 3
    /// var values = parser.ParseRouteInstance("https://mydomain.com/foo/1");
    /// //values = { { "protocol" => "https"}, { "itemCategory" => "foo"}, { "itemId" => "1" } }
    /// </code>
    /// </example>
    public class RouteParser
    {
        Regex regex;

        public RouteParser(String route, Char variableStartChar = '{', Char variableEndChar = '}')
        {
            RouteFormat = route;
            this.VariableStartChar = variableStartChar;
            this.VariableEndChar = variableEndChar;
            ParseRouteFormat();

            String formatUrl = new String(RouteFormat.ToArray());
            foreach (String variable in Variables)
            {
                formatUrl = formatUrl.Replace(WrapWithVariableChars(variable), String.Format(VariableTokenPattern, variable));
            }
            regex = new Regex(formatUrl + "/*$", RegexOptions.IgnoreCase);
        }

        //http://my.domain.com:8000?arg1=this&arg2=that
        private static Regex pattern = new Regex("(?<protocol>http(s)?|ftp)://(?<server>([A-Za-z0-9-]+\\\\.)*(?<basedomain>[A-Za-z0-9-]+\\\\.[A-Za-z0-9]+))+((:)?(?<port>[0-9]+)?(/?)(?<path>(?<dir>[A-Za-z0-9\\\\._\\\\-]+)(/){0,1}[A-Za-z0-9.-/]*)){0,1}");
        private const String RouteTokenPattern = @"[{0}].+?[{1}]"; //the 0 and 1 are used by the string.format function, they are the start and end characters.
        private const String VariableTokenPattern = "(?<{0}>[^/].*)"; //the <>'s denote the group name; this is used for reference for the variables later.

        /// <summary>
        /// This is the route template that values are extracted based on.
        /// </summary>
        /// <value>
        /// A string containing variables denoted by the <c>VariableStartChar</c> and the <c>VariableEndChar</c>
        /// </value>
        public String RouteFormat { get; set; }

        /// <summary>
        /// This is the character that denotes the beginning of a variable name.
        /// </summary>
        public Char VariableStartChar { get; set; }

        /// <summary>
        /// This is the character that denotes the end of a variable name.
        /// </summary>
        public Char VariableEndChar { get; set; }

        /// <summary>
        /// A hash set of all variable names parsed from the <c>RouteFormat</c>.
        /// </summary>
        public HashSet<String> Variables { get; set; }

        /// <summary>
        /// Initialize the Variables set based on the <c>RouteFormat</c>
        /// </summary>
        private void ParseRouteFormat()
        {
            var variableList = new List<String>();
            var matchCollection = System.Text.RegularExpressions.Regex.Matches
                (
                    this.RouteFormat
                    , String.Format(RouteTokenPattern, VariableStartChar, VariableEndChar)
                    , RegexOptions.IgnoreCase
                );

            foreach (var match in matchCollection)
            {
                variableList.Add(RemoteVariableChars(match.ToString()));
            }
            Variables = new HashSet<string>(variableList);
        }

        /// <summary>
        /// Extract variable values from a given instance of the route you're trying to parse.
        /// </summary>
        /// <param name="routeInstance">The route instance.</param>
        /// <returns>A dictionary of Variable names mapped to values.</returns>
        public Dictionary<String, String> ParseRouteInstance(String routeInstance)
        {
            var inputValues = new Dictionary<string, string>();

            var matchCollection = regex.Match(routeInstance);

            if (matchCollection.Success)
                foreach (var variable in Variables)
                {
                    var value = ((Group)matchCollection.Groups[variable]).Value;
                    inputValues.Add(variable, value);
                }

            return inputValues;
        }

        /// <summary>
        /// Replace a variable in the <c>RouteFormat</c> with a specified value.
        /// </summary>
        /// <param name="variableName">The variable name to replace.</param>
        /// <param name="variableValue">The value to replace with.</param>
        /// <param name="workingRoute">An 'in progress' route that may contain values that have already been replaced.</param>
        /// <returns>A <c>workingRoute</c></returns>
        public String SetVariable(String variableName, String variableValue, String workingRoute = null)
        {
            if (!variableName.StartsWith(VariableStartChar.ToString()) && !variableName.EndsWith(VariableEndChar.ToString()))
                variableName = String.Format("{1}{0}{2}", variableName, VariableStartChar, VariableEndChar);

            if (!String.IsNullOrEmpty(workingRoute))
                return workingRoute.Replace(variableName, variableValue);
            else
                return RouteFormat.Replace(variableName, variableValue);
        }

        #region Private Helper Methods
        private String RemoteVariableChars(String input)
        {
            if (String.IsNullOrWhiteSpace(input))
                return input;

            string result = new String(input.ToArray());
            result = result.Replace(VariableStartChar.ToString(), String.Empty).Replace(VariableEndChar.ToString(), String.Empty);
            return result;
        }

        private String WrapWithVariableChars(String input)
        {
            return String.Format("{0}{1}{2}", VariableStartChar, input, VariableEndChar);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FLVER_Editor;

/// <summary>
/// Possible ways of matching methods
/// </summary>
public enum StringMatchingMethod
{
    NoWildcards,
    UseWildcards,
    UseRegexs,
}

/// <summary>
/// Allows to decompose a strings against a pattern
/// </summary>
public class StringMatcher
{
    #region fields
    public delegate StringMatch MatchDelegate(string source);
    private readonly object m_pattern;
    #endregion

    /// <summary>
    /// Match against a string
    /// </summary>
    public MatchDelegate Match { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public StringMatcher(StringMatchingMethod method, string pattern)
    {
        switch (method)
        {
            case StringMatchingMethod.NoWildcards:
                m_pattern = prepareNoWildcard(pattern);
                Match = buildNoWildcard;
                break;
            case StringMatchingMethod.UseWildcards:
                m_pattern = prepareWithWildcards(pattern);
                Match = buildWithWildcards;
                break;
            case StringMatchingMethod.UseRegexs:
                m_pattern = prepareRegex(pattern);
                Match = buildRegex;
                break;
            default:
                throw new ArgumentException("Unknown StringMatchingMethod value");
        }
    }

    #region No wildcards
    /// <summary>
    /// Prepare the pattern for a "non wildcard" match
    /// </summary>
    private static object prepareNoWildcard(string pattern)
    {
        return pattern.ToLower();
    }

    /// <summary>
    /// Compare the source against the pattern and if successfull returns a StringMatch
    /// 
    /// There is a match if source contains all the characters of pattern in the right order
    /// but not consecutively
    /// </summary>
    private StringMatch buildNoWildcard(string source)
    {
        string pattern = (string)m_pattern;
        string lsource = source.ToLower();

        List<string> segments = new List<string>();
        int i = 0, j;
        int NP = pattern.Length, NS = source.Length;
        bool startsOnMatch = false, isMatch = false;
        for (j = 0; j < NP; j++)
        {
            int s = i;
            // skip characters until we have a match
            for (; i < NS; i++)
            {
                if (lsource[i] == pattern[j])
                {
                    isMatch = (j == NP - 1);
                    if (s != i)
                    {
                        segments.Add(source.Substring(s, i - s));
                        segments.Add(source.Substring(i, 1));
                    }
                    else
                    {
                        if (segments.Count == 0)
                        {
                            segments.Add(source.Substring(i, 1));
                            startsOnMatch = true;
                        }
                        else
                        {
                            segments[segments.Count - 1] += source[i];
                        }
                    }
                    i++;
                    break; // i loop
                }
            }
            if (i >= NS)
            {
                break; // j loop
            }
        }

        if (!isMatch)
        {
            return null;
        }
        if (i < NS)
        {
            segments.Add(source.Substring(i, NS - i));
        }
        return new StringMatch
        {
            Text = source,
            Segments = segments,
            StartsOnMatch = startsOnMatch,
        };
    }
    #endregion

    #region Wildcards
    /// <summary>
    /// Prepare the pattern for a "wildcard" match
    /// </summary>
    private static object prepareWithWildcards(string pattern)
    {
        // starts the match anywhere
        StringBuilder buf = new StringBuilder(pattern.StartsWith("*") ? "" : "*");

        // reduce consecutive '*'s into a single one
        int NP = pattern.Length;
        for (int i = 0; i < NP; i++)
        {
            char c = pattern[i];
            if (c == '*')
            {
                // "****" == "*"
                for (; (i < NP) && (pattern[i] == '*'); i++) {}
                buf.Append('*');
                // don't need to process a '*' at the end of the pattern
                if (i < NP)
                {
                    buf.Append(pattern[i]);
                }
            }
            else
            {
                buf.Append(c);
            }
        }
        return buf.ToString().ToLower();
    }

    /// <summary>
    /// Add a chunk to list of segments
    /// </summary>
    static void addChunkToStringMatch(StringMatch res, string txt, bool isMatch)
    {
        if (res.Segments.Count == 0)
        {
            res.StartsOnMatch = isMatch;
            res.Segments.Add(txt);
            return;
        }
        bool currentIsMatch = (((res.Segments.Count % 2) != 0) == res.StartsOnMatch);
        if (currentIsMatch == isMatch)
        {
            // we can add to the previous segment
            res.Segments[res.Segments.Count - 1] = res.Segments[res.Segments.Count - 1] + txt;
        }
        else
        {
            res.Segments.Add(txt);
        }
    }

    /// <summary>
    /// Compare the source against the pattern and if successfull returns a StringMatch
    /// 
    /// There match is done against the source which can contain the wildcard '*'
    /// </summary>
    private StringMatch buildWithWildcards(string source)
    {
        StringMatch res = new StringMatch
        {
            Text = source,
            Segments = new List<string>(),
            StartsOnMatch = false,
        };

        string pattern = (string)m_pattern;
        string lsource = source.ToLower();

        int iP = 0, iS = 0, icP = -1, icS = -1;
        int ioS1 = -1, ioS2 = -1;
        int NP = pattern.Length, NS = source.Length;

        // skip all the starting characters untils we either don't match or reach a '*'
        while ((iP != NP) && (iS != NS) && (pattern[iP] != '*'))
        {
            if (lsource[iS] != pattern[iP])
            {
                // mismatch
                return null;
            }
            iS++;
            iP++;
        }

        if (iS != 0)
        {
            addChunkToStringMatch(res, source.Substring(0, iS), true);
        }

        if (iP != NP)
        {
            // we have not finished parsing

            // now we start from a '*'
            // we try to get a sequence of matches to ('*', non '*'s), ('*', non '*'s), ....
            while (iS != NS)
            {
                if (pattern[iP] == '*')
                {
                    // we have reached a new '*', we will now compare against the next pattern character
                    iP++;
                    if (iP == NP)
                    {
                        //we have matched all the pattern
                        break; // while
                    }
                    if (icP != iP)
                    {
                        // this is the 1st time we try to process this '*'
                        if (icP >= 0)
                        {
                            // not the first run
                            // so we have successfully processed the previous '*'
                            if (ioS1 != ioS2)
                            {
                                addChunkToStringMatch(res, source.Substring(ioS1, ioS2 - ioS1), false);
                            }
                            if (ioS2 != iS)
                            {
                                addChunkToStringMatch(res, source.Substring(ioS2, iS - ioS2), true);
                            }
                        }
                        ioS1 = iS;
                        ioS2 = -1;
                    }
                    // we save the current position in case the '*' needs to match more characters
                    icP = iP;
                    icS = iS + 1;
                }
                else if (lsource[iS] == pattern[iP])
                {
                    // if we were processing a '*', we now have a successfull match
                    // we are still matching sucessfully the pattern
                    if (ioS2 == -1)
                    {
                        ioS2 = iS;
                    }
                    iS++;
                    iP++;
                }
                else
                {
                    // we have a mismatch
                    // let's try again with '*' matching one more character
                    iP = icP;
                    iS = icS++;
                }
                if (iP == NP)
                {
                    //we have matched all the pattern
                    break; // while
                }
            }
            // we have processed either the whole source or either the whole pattern
            // let's process any remaining '*' which here matches the empty string
            while ((iP != NP) && (pattern[iP] == '*'))
            {
                iP++;
            }
        }
        if (iP == NP)
        {
            // we have a match!
            if (icP != iP)
            {
                if (ioS1 != -1)
                {
                    if (ioS2 == -1)
                    {
                        addChunkToStringMatch(res, source.Substring(ioS1, iS - ioS1), false);
                    }
                    else
                    {
                        if (ioS2 != ioS1)
                        {
                            addChunkToStringMatch(res, source.Substring(ioS1, ioS2 - ioS1), false);
                        }
                        if (ioS2 != iS)
                        {
                            addChunkToStringMatch(res, source.Substring(ioS2, iS - ioS2), true);
                        }
                    }
                }
            }
            if (iS != NS)
            {
                addChunkToStringMatch(res, source.Substring(iS, NS - iS), false);
            }
            return res;
        }
        return null;
    }
    #endregion

    #region Regexs
    /// <summary>
    /// Prepare the pattern for a "regex" match
    /// </summary>
    private static object prepareRegex(string pattern)
    {
        try
        {
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            // the user has entered an invalid regex, just ignore it
        }
        return null;
    }

    /// <summary>
    /// Compare the source against the pattern and if successfull returns a StringMatch
    /// 
    /// There is a match if source contains all the characters of pattern in the right order
    /// but not consecutively
    /// </summary>
    private StringMatch buildRegex(string source)
    {
        if (m_pattern == null)
        {
            return null;
        }

        Regex re = (Regex)m_pattern;
        List<string> segments = new List<string>();
        bool startsOnMatch = false, isMatch = false;
        int idx = 0;

        // we need only one match
        Match m = re.Match(source, idx);
        if (m.Success)
        {
            isMatch = true;
            if (m.Index == idx)
            {
                if (m.Index == 0)
                {
                    startsOnMatch = true;
                }
                segments.Add(source.Substring(m.Index, m.Length));
            }
            else
            {
                segments.Add(source.Substring(idx, m.Index - idx));
                segments.Add(source.Substring(m.Index, m.Length));
            }
            idx = m.Index + m.Length;
        }

        if (!isMatch)
        {
            return null;
        }

        if (idx < source.Length)
        {
            segments.Add(source.Substring(idx, source.Length - idx));
        }

        return new StringMatch
        {
            Text = source,
            Segments = segments,
            StartsOnMatch = startsOnMatch,
        };
    }
    #endregion
}

/// <summary>
/// The result of a match
/// There are the items we store in the suggestion listbox
/// </summary>
public class StringMatch
{
    /// <summary>
    /// The original source
    /// </summary>
    public string Text { get; internal set; }
    /// <summary>
    /// The source decomposed on match/non matches against the pattern
    /// </summary>
    public List<string> Segments { get; internal set; }
    /// <summary>
    /// Is the first segment a match?
    /// </summary>
    public bool StartsOnMatch { get; internal set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_HTML_Handler.Services
{
    /// <summary>
    /// Line-level three-way merge (diff3) used by "deploy merge".
    /// BASE = live as it was when the developer synced, LIVE = current published file, DEV = developer's file.
    /// Only the lines the developer changed relative to BASE are applied; lines changed on LIVE since BASE are kept.
    /// </summary>
    public static class ThreeWayMerge
    {
        public const string MarkerLive = "<<<<<<< LIVE (current published)";
        public const string MarkerBase = "||||||| BASE (when developer synced)";
        public const string MarkerSplit = "=======";
        public const string MarkerDevPrefix = ">>>>>>> DEV";

        // Above this many edit steps the diff falls back to one whole-region change (coarser, never wrong).
        private const int MaxEditDistance = 2000;

        public class TextFile
        {
            public List<string> Lines = new List<string>();
            public bool HasBom;
            public bool UsesCrLf;
            public bool EndsWithNewline;
        }

        public class MergeResult
        {
            public List<string> Lines = new List<string>();
            public int DevChanges;
            public int LiveChangesKept;
            public int Conflicts;
        }

        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        /// <summary>Decodes bytes as UTF-8 text. Returns null for binary or non-UTF-8 content.</summary>
        public static TextFile TryParseText(byte[] bytes)
        {
            int start = 0;
            bool bom = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
            if (bom) start = 3;

            int probe = Math.Min(bytes.Length, 8000);
            for (int i = start; i < probe; i++)
            {
                if (bytes[i] == 0) return null;
            }

            string text;
            try
            {
                text = StrictUtf8.GetString(bytes, start, bytes.Length - start);
            }
            catch (DecoderFallbackException)
            {
                return null;
            }

            var file = new TextFile { HasBom = bom };
            int crlf = 0, lf = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '\n') continue;
                if (i > 0 && text[i - 1] == '\r') crlf++; else lf++;
            }
            file.UsesCrLf = crlf > lf;
            file.EndsWithNewline = text.EndsWith("\n");

            var parts = text.Split('\n');
            int count = file.EndsWithNewline ? parts.Length - 1 : parts.Length;
            for (int i = 0; i < count; i++)
            {
                file.Lines.Add(parts[i].TrimEnd('\r'));
            }
            if (text.Length == 0) file.Lines.Clear();
            return file;
        }

        /// <summary>Serializes lines using the line-ending, BOM and trailing-newline style of the given template file.</summary>
        public static byte[] ToBytes(List<string> lines, TextFile style)
        {
            string nl = style.UsesCrLf ? "\r\n" : "\n";
            var sb = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                sb.Append(lines[i]);
                if (i < lines.Count - 1 || style.EndsWithNewline) sb.Append(nl);
            }
            byte[] body = StrictUtf8.GetBytes(sb.ToString());
            if (!style.HasBom) return body;
            var withBom = new byte[body.Length + 3];
            withBom[0] = 0xEF; withBom[1] = 0xBB; withBom[2] = 0xBF;
            Buffer.BlockCopy(body, 0, withBom, 3, body.Length);
            return withBom;
        }

        public static bool SameLines(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!string.Equals(a[i], b[i], StringComparison.Ordinal)) return false;
            }
            return true;
        }

        public static bool HasConflictMarkers(List<string> lines)
        {
            return lines.Any(l => l.StartsWith(MarkerLive, StringComparison.Ordinal) || l.StartsWith(MarkerDevPrefix, StringComparison.Ordinal));
        }

        /// <summary>
        /// diff3 merge. Regions changed only by DEV take DEV's lines, regions changed only on LIVE keep LIVE's lines,
        /// identical changes on both sides are taken once, and overlapping different changes become conflict blocks.
        /// </summary>
        public static MergeResult Merge(List<string> baseLines, List<string> live, List<string> dev, string devLabel)
        {
            int[] mapLive = MatchToBase(baseLines, live);
            int[] mapDev = MatchToBase(baseLines, dev);
            var result = new MergeResult();

            int b = 0, l = 0, d = 0;
            int nB = baseLines.Count, nL = live.Count, nD = dev.Count;

            while (b < nB || l < nL || d < nD)
            {
                // Stable run: base line present at the same spot in both versions
                int k = 0;
                while (b + k < nB && mapLive[b + k] == l + k && mapDev[b + k] == d + k) k++;
                if (k > 0)
                {
                    for (int i = 0; i < k; i++) result.Lines.Add(live[l + i]);
                    b += k; l += k; d += k;
                    continue;
                }

                // Next base line that both versions still contain marks the end of this changed region
                int j = b;
                while (j < nB && (mapLive[j] < l || mapDev[j] < d)) j++;

                int bEnd = j, lEnd = j < nB ? mapLive[j] : nL, dEnd = j < nB ? mapDev[j] : nD;
                var baseChunk = baseLines.GetRange(b, bEnd - b);
                var liveChunk = live.GetRange(l, lEnd - l);
                var devChunk = dev.GetRange(d, dEnd - d);

                if (SameLines(liveChunk, baseChunk))
                {
                    result.Lines.AddRange(devChunk);
                    if (!SameLines(devChunk, baseChunk)) result.DevChanges++;
                }
                else if (SameLines(devChunk, baseChunk))
                {
                    result.Lines.AddRange(liveChunk);
                    result.LiveChangesKept++;
                }
                else if (SameLines(liveChunk, devChunk))
                {
                    result.Lines.AddRange(liveChunk);
                }
                else
                {
                    result.Conflicts++;
                    result.Lines.Add(MarkerLive);
                    result.Lines.AddRange(liveChunk);
                    result.Lines.Add(MarkerBase);
                    result.Lines.AddRange(baseChunk);
                    result.Lines.Add(MarkerSplit);
                    result.Lines.AddRange(devChunk);
                    result.Lines.Add($"{MarkerDevPrefix} ({devLabel})");
                }

                b = bEnd; l = lEnd; d = dEnd;
            }

            return result;
        }

        /// <summary>For each base line, the index of the matching line in <paramref name="other"/> (LCS via Myers diff), or -1.</summary>
        private static int[] MatchToBase(List<string> baseLines, List<string> other)
        {
            int n = baseLines.Count, m = other.Count;
            var map = Enumerable.Repeat(-1, n).ToArray();

            int pre = 0;
            while (pre < n && pre < m && baseLines[pre] == other[pre]) { map[pre] = pre; pre++; }
            int suf = 0;
            while (suf < n - pre && suf < m - pre && baseLines[n - 1 - suf] == other[m - 1 - suf])
            {
                map[n - 1 - suf] = m - 1 - suf;
                suf++;
            }

            int an = n - pre - suf, bn = m - pre - suf;
            if (an == 0 || bn == 0) return map;

            // Hash lines to ints so the inner loop compares numbers
            var ids = new Dictionary<string, int>(StringComparer.Ordinal);
            int Id(string s) { if (!ids.TryGetValue(s, out int v)) { v = ids.Count; ids[s] = v; } return v; }
            var a = new int[an];
            var c = new int[bn];
            for (int i = 0; i < an; i++) a[i] = Id(baseLines[pre + i]);
            for (int i = 0; i < bn; i++) c[i] = Id(other[pre + i]);

            foreach (var pair in MyersMatches(a, c))
            {
                map[pre + pair.Key] = pre + pair.Value;
            }
            return map;
        }

        private static List<KeyValuePair<int, int>> MyersMatches(int[] a, int[] b)
        {
            int n = a.Length, m = b.Length, max = n + m;
            int limit = Math.Min(max, MaxEditDistance);
            int offset = max + 1;
            var v = new int[2 * max + 3];
            var trace = new List<int[]>();
            int found = -1;

            for (int dd = 0; dd <= limit; dd++)
            {
                var snap = new int[2 * dd + 1];
                for (int k = -dd; k <= dd; k += 2)
                {
                    int x;
                    if (k == -dd || (k != dd && v[offset + k - 1] < v[offset + k + 1])) x = v[offset + k + 1];
                    else x = v[offset + k - 1] + 1;
                    int y = x - k;
                    while (x < n && y < m && a[x] == b[y]) { x++; y++; }
                    v[offset + k] = x;
                    if (x >= n && y >= m) { found = dd; break; }
                }
                for (int k = -dd; k <= dd; k++) snap[k + dd] = v[offset + k];
                trace.Add(snap);
                if (found >= 0) break;
            }

            var matches = new List<KeyValuePair<int, int>>();
            if (found < 0) return matches; // Too different: treat the whole region as changed

            int cx = n, cy = m;
            for (int dd = found; dd > 0; dd--)
            {
                var prev = trace[dd - 1];
                int k = cx - cy;
                int prevK = (k == -dd || (k != dd && prev[k - 1 + dd - 1] < prev[k + 1 + dd - 1])) ? k + 1 : k - 1;
                int prevX = prev[prevK + dd - 1];
                int prevY = prevX - prevK;
                // Diagonal snake back to the point right after the edit step
                while (cx > prevX && cy > prevY)
                {
                    cx--; cy--;
                    matches.Add(new KeyValuePair<int, int>(cx, cy));
                }
                cx = prevX; cy = prevY;
            }
            while (cx > 0 && cy > 0) { cx--; cy--; matches.Add(new KeyValuePair<int, int>(cx, cy)); }
            matches.Reverse();
            return matches;
        }
    }
}

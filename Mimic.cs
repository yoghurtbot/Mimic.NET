  /// <summary>
  /// Mimic is a class to accurately generate Chrome client hints for a given Chrome version
  /// </summary>
  public class Mimic
  {
      private const string FullVersion = "126.0.6478.184";

      private readonly int[][] _greasyOrders =
      [
          [0,1,2], [0, 2, 1], [1, 0, 2],
          [1, 2, 0], [2, 0, 1], [2, 1, 0]
      ];

      private readonly string[] _greasyChars = [" ", "(", ":", "-", ".", "/", ")", ";", "=", "?", "_"];
      private readonly string[] _greasyVersions = ["8", "99", "24"];

      public Mimic()
      {
          string[] parts = FullVersion.Split('.');
          int majorVersion = Convert.ToInt32(parts[0]);

          int[] order = _greasyOrders[majorVersion % _greasyOrders.Length];
          string[] greased = new string[3];

          greased[order[0]] = GreasedBrand(majorVersion, order);
          greased[order[1]] = FormatBrand("Chromium", majorVersion.ToString());
          greased[order[2]] = FormatBrand("Google Chrome", majorVersion.ToString());

          SecChUa = string.Join(", ", greased);
          SecChUaFullVersionList = GenerateSecChUaFullVersionList(greased, majorVersion, FullVersion);
          SecChUaPlatform = "\"Windows\"";
          SecChUaArch = "\"x86\"";
          SecChUaModel = "\"\""; 
          SecChUaMobile = "?0";
          SecChUaFullVersion = $"\"{FullVersion}\"";
          UserAgent = $"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{majorVersion}.0.0.0 Safari/537.36";
      }
      
      private string GreasedBrand(int seed, int[] permutedOrder)
      {
          string brand = "";
          string version = "";
          switch (seed)
          {
              case >= 105:
                  string brand1 = _greasyChars[seed%_greasyChars.Length];
                  string brand2 = _greasyChars[(seed+1)%_greasyChars.Length]; 
                  brand = $"Not{brand1}A{brand2}Brand"; 
                  version = _greasyVersions[seed % _greasyVersions.Length];
                  break;
          }

          return FormatBrand(brand, version);
      }

      private string GenerateSecChUaFullVersionList(string[] greased, int majorVersion, string fullVersion)
      {
          string greasedVersion = $"{_greasyVersions[majorVersion % _greasyVersions.Length]}.0.0.0";

          return greased.Select((b, i) =>
          {
              if (b.Contains("Not"))
                  return b.Replace($"v=\"{_greasyVersions[majorVersion % _greasyVersions.Length]}\"", $"v=\"{greasedVersion}\"");

              return b.Replace($"v=\"{majorVersion}\"", $"v=\"{fullVersion}\"");
          }).Aggregate((current, next) => $"{current}, {next}");
      }

      private string FormatBrand(string brand, string version) => 
          $"\"{brand}\";v=\"{version}\"";

      public string UserAgent { get; }
      public string SecChUa { get; }
      public string SecChUaFullVersionList { get; }
      public string SecChUaFullVersion { get;  }
      public string SecChUaMobile { get; }
      public string SecChUaModel { get;  }
      public string SecChUaArch { get; }
      public string SecChUaPlatform { get; }
  }

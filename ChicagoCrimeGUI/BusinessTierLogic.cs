//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{
  ///
  /// <summary>
  /// Ways to sort the Areas in Chicago.
  /// </summary>
  /// 
  public enum OrderAreas
  {
    ByNumber,
    ByName
  };


  //
  // Business:
  //
  public class Business
  {
    //
    // Fields:
    //
    private string _DBFile;
    private DataAccessTier.Data dataTier;


    ///
    /// <summary>
    /// Constructs a new instance of the business tier.  The format
    /// of the filename should be either |DataDirectory|\filename.mdf,
    /// or a complete Windows pathname.
    /// </summary>
    /// <param name="DatabaseFilename">Name of database file</param>
    /// 
    public Business(string DatabaseFilename)
    {
      _DBFile = DatabaseFilename;

      dataTier = new DataAccessTier.Data(DatabaseFilename);
    }


    ///
    /// <summary>
    ///  Opens and closes a connection to the database, e.g. to
    ///  startup the server and make sure all is well.
    /// </summary>
    /// <returns>true if successful, false if not</returns>
    /// 
    public bool OpenCloseConnection()
    {
      return dataTier.OpenCloseConnection();
    }


    ///
    /// <summary>
    /// Returns overall stats about crimes in Chicago.
    /// </summary>
    /// <returns>CrimeStats object</returns>
    ///
    public CrimeStats GetOverallCrimeStats()
    {
      CrimeStats cs;

            string sql = @" SELECT (COUNT (CID)) as 'Total Crimes', 
            MIN(Year) as 'Min Year', 
            MAX (Year) as 'Max Year' from Crimes;";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            DataRow row = ds.Tables["TABLE"].Rows[0];
            
            long a = Convert.ToInt64(row["Total Crimes"]);
            int b = Convert.ToInt32(row["Min Year"]);
            int c = Convert.ToInt32(row["Max Year"]);
            cs = new CrimeStats(a, b, c);
      return cs;
    }


    ///
    /// <summary>
    /// Returns all the areas in Chicago, ordered by area # or name.
    /// </summary>
    /// <param name="ordering"></param>
    /// <returns>List of Area objects</returns>
    /// 
    public List<Area> GetChicagoAreas(OrderAreas ordering)
    {
      List<Area> areas = new List<Area>();

      string sql = @"SELECT AreaName as 'Areas', 
                  Area as 'AreaNum' from Areas
                  ORDER BY AreaName;";
      if(ordering == OrderAreas.ByNumber)
        {
                sql = @"SELECT AreaName as 'Areas', 
                  Area as 'AreaNum' from Areas
                  ORDER BY AreaNum;";
        }

      DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
      
      foreach(DataRow row in ds.Tables["TABLE"].Rows)
            {
                Area a;
                int num = Convert.ToInt32(row["AreaNum"]);
                string name = Convert.ToString(row["Areas"]);
                a = new Area(num, name);
                areas.Add(a);
            }

        
      return areas;
    }


    ///
    /// <summary>
    /// Returns all the crime codes and their descriptions.
    /// </summary>
    /// <returns>List of CrimeCode objects</returns>
    ///
    public List<CrimeCode> GetCrimeCodes()
    {
      List<CrimeCode> codes = new List<CrimeCode>();


            string sql = @" SELECT IUCR as 'Code',
            PrimaryDesc as 'Primary', 
            SecondaryDesc as 'Secondary' from Codes;";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach(DataRow row in ds.Tables["TABLE"].Rows)
            {
                string code = Convert.ToString(row["Code"]);
                string p = Convert.ToString(row["Primary"]);
                string s = Convert.ToString(row["Secondary"]);
                CrimeCode c;
                c = new CrimeCode(code, p, s);
                codes.Add(c);
            }      

      return codes;
    }


    ///
    /// <summary>
    /// Returns a hash table of years, and total crimes each year.
    /// </summary>
    /// <returns>Dictionary where year is the key, and total crimes is the value</returns>
    ///
    public Dictionary<int, long> GetTotalsByYear()
    {
      Dictionary<int, long> totalsByYear = new Dictionary<int, long>();

            string sql = @" SELECT Year as 'Years', 
            count(CID) as 'Total Crimes' from Crimes
            GROUP BY Year
            ORDER BY Year;";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                int y = Convert.ToInt32(row["Years"]);
                long c = Convert.ToInt64(row["Total Crimes"]);
                totalsByYear.Add(y, c);
            }
            return totalsByYear;
        }


    ///
    /// <summary>
    /// Returns a hash table of months, and total crimes each month.
    /// </summary>
    /// <returns>Dictionary where month is the key, and total crimes is the value</returns>
    /// 
    public Dictionary<int, long> GetTotalsByMonth()
    {
      Dictionary<int, long> totalsByMonth = new Dictionary<int, long>();

      string sql = @"SELECT MONTH(CrimeDate) AS 'Months', 
                    COUNT(CID) as 'Crimes in Month' from Crimes
                    WHERE YEAR(CrimeDate) = Year AND Crimes.Area > 0
                    GROUP BY MONTH(CrimeDate)
                    ORDER BY MONTH(CrimeDate);";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                int m = Convert.ToInt32(row["Months"]);
                long c = Convert.ToInt64(row["Crimes in Month"]);
                totalsByMonth.Add(m, c);
            }
            return totalsByMonth;
    }


    ///
    /// <summary>
    /// Returns a hash table of areas, and total crimes each area.
    /// </summary>
    /// <returns>Dictionary where area # is the key, and total crimes is the value</returns>
    ///
    public Dictionary<int, long> GetTotalsByArea()
    {
      Dictionary<int, long> totalsByArea = new Dictionary<int, long>();
      
      string sql = @"SELECT Areas.AreaName as 'Areas', 
                    Areas.Area as 'AreaNum', 
                    COUNT(Crimes.CID) as 'Number of Crimes'
                    FROM Crimes
                    INNER JOIN Areas
                    ON Areas.Area=Crimes.Area
                    WHERE Areas.Area >0
                    GROUP BY Areas.Area, Areas.AreaName
                    ORDER BY Areas.Area;";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                int a = Convert.ToInt32(row["AreaNum"]);
                long c = Convert.ToInt64(row["Number of Crimes"]);

                totalsByArea.Add(a, c);
            }


                return totalsByArea;
    }


    ///
    /// <summary>
    /// Returns a hash table of years, and arrest percentages each year.
    /// </summary>
    /// <returns>Dictionary where the year is the key, and the arrest percentage is the value</returns>
    /// 
    public Dictionary<int, double> GetArrestPercentagesByYear()
    {
      Dictionary<int, double> percentagesByYear = new Dictionary<int, double>();

      string sql = @" SELECT Year as 'Years', 
            count(CID) as 'Total Crimes', 
            FORMAT(sum(convert(float,Arrested)*100)/count(CID),'n2') as 'Percentage Arrested' from Crimes
            GROUP BY Year
            ORDER BY Year;";

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                int y = Convert.ToInt32(row["Years"]);
                double p = Convert.ToDouble(row["Percentage Arrested"]);
                percentagesByYear.Add(y, p);
            }

           return percentagesByYear;
    }

        ///
        /// <summary>
        /// Returns a long of totalCrimes by the given combo
        /// </summary>
        /// <returns>A Long value of the total crimes</returns>
        /// 
        public long GetTotalCrimesByCombo(string year, string crimeCode, string AreaNum)
        {
            long totalCrimes;

            string sql = String.Format(@"SELECT Count(*) From Crimes
                    Where Year Like '{0}' AND IUCR Like '{1}' AND Area Like '{2}'
                    ", year, crimeCode, AreaNum);

            Object result = dataTier.ExecuteScalarQuery(sql);

            totalCrimes = Convert.ToInt64(result);


            return totalCrimes;
        }

        public Dictionary<string, long> GetTopAreas(int N)
        {
            Dictionary<string, long> topAreas = new Dictionary<string, long>();

            string sql = String.Format(@"SELECT TOP {0} Areas.AreaName as 'Area Name', Count(Crimes.CID) as '# of Crimes in Area'
                            from Crimes
                            INNER JOIN Areas
                            ON Areas.Area = Crimes.Area
                            WHERE Areas.Area > 0
                            GROUP BY AreaName
                            ORDER BY COUNT(Crimes.CID) DESC;", N);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string a = Convert.ToString(row["Area Name"]);
                long c = Convert.ToInt64(row["# of Crimes In Area"]);
                topAreas.Add(a, c);
            }
        
            return topAreas;

        }


        public Dictionary<CrimeCode,long> GetTopCrimeTypes(int N)
        {
            Dictionary<CrimeCode, long> crimeTypes = new Dictionary<CrimeCode, long>();

            string sql = String.Format(@"SELECT TOP {0} Codes.PrimaryDesc as 'Primary Desc', 
                                Codes.SecondaryDesc as 'Secondary Desc', 
                                COUNT(Crimes.IUCR) as '# of Crime Occured'
                                from Crimes
                                INNER JOIN Codes
                                ON Crimes.IUCR = Codes.IUCR
                                GROUP BY Codes.PrimaryDesc,Codes.SecondaryDesc
                                ORDER BY count(Crimes.IUCR) DESC;", N);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string iucr = "";
                string p = Convert.ToString(row["Primary Desc"]);
                string s = Convert.ToString(row["Secondary Desc"]);
                CrimeCode c = new CrimeCode(iucr, p, s);

                long crime = Convert.ToInt64(row["# of Crime Occured"]);

                crimeTypes.Add(c, crime);
            }
            return crimeTypes;
        }


        public Dictionary<string, long> GetTopAreaByCrime(int N, string crimeCode)
        {
            Dictionary<string, long> crimeTypes = new Dictionary<string, long>();

            string sql = String.Format(@"SELECT TOP {0} Areas.AreaName as 'Area Name', 
                                 count(CID) as 'Total # of Crimes' 
                                 from Crimes
                                 INNER JOIN Areas
                                 ON Crimes.Area = Areas.Area
                                 WHERE Crimes.IUCR = '{1}' AND Areas.Area > 0
                                 GROUP BY Areas.AreaName
                                 ORDER BY count(CID) DESC;", N, crimeCode);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string name = Convert.ToString(row["Area Name"]);
                long crime = Convert.ToInt64(row["Total # of Crimes"]);
                crimeTypes.Add(name, crime);
            }
            return crimeTypes;
        }

        public Dictionary<CrimeCode,long> GetTopByAreaAndYear(int N, string areaName, int min, int max)
        {
            Dictionary<CrimeCode, long> crimeTypes = new Dictionary<CrimeCode, long>();

            string sql = String.Format(@"SELECT TOP {0} Codes.PrimaryDesc as 'Primary Desc', 
                                    Codes.SecondaryDesc as 'Secondary Desc', 
                                    count(Codes.IUCR) as '# of Crime Occured'
                                    from Crimes
                                    INNER JOIN Codes
                                    ON Crimes.IUCR = Codes.IUCR
                                    INNER JOIN Areas
                                    ON Crimes.Area = Areas.Area
                                    WHERE (Areas.AreaName ='{1}') AND (year>={2}) AND (year<={3})
                                    GROUP BY Codes.PrimaryDesc,Codes.SecondaryDesc
                                    ORDER BY count(Crimes.IUCR) DESC;", N, areaName, min, max);

            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                string iucr = "";
                string p = Convert.ToString(row["Primary Desc"]);
                string s = Convert.ToString(row["Secondary Desc"]);
                CrimeCode c = new CrimeCode(iucr, p, s);

                long crime = Convert.ToInt64(row["# of Crime Occured"]);

                crimeTypes.Add(c, crime);
            }

            return crimeTypes;
        }

  }//class
}//namespace

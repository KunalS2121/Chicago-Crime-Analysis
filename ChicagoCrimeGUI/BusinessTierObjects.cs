//
// BusinessTier objects:  these classes define the objects serving 
// as data transfer between UI and business tier.  These objects 
// carry the data that is normally displayed in the presentation 
// tier.  The classes defined here:
//
//    Area
//    CrimeCode
//    CrimeStats
//
// NOTE: the presentation tier should *not* be creating instances 
// of these objects, but instead calling the BusinessTier logic 
// class ("Business") to obtain these objects.
//

using System;
using System.Collections.Generic;


namespace BusinessTier
{

  ///
  /// <summary>
  /// Info about one area of Chicago: area # and name.
  /// </summary>
  /// 
  public class Area
  {
    public readonly int AreaNumber;
    public readonly string AreaName;

    public Area(int number, string name)
    {
      AreaNumber = number;
      AreaName = name;
    }
  }//class


  ///
  /// <summary>
  /// Info about a crime: IUCR code, primary and secondary 
  /// description.
  /// </summary>
  /// 
  public class CrimeCode
  {
    public readonly string IUCR;
    public readonly string PrimaryDescription;
    public readonly string SecondaryDescription;

    public CrimeCode(string iucr, string primarydesc, string secondarydesc)
    {
      IUCR = iucr;
      PrimaryDescription = primarydesc;
      SecondaryDescription = secondarydesc;
    }
  }//class


  ///
  /// <summary>
  /// Overall crime stats: total # of crimes, year range for data.
  /// </summary>
  /// 
  public class CrimeStats
  {
    public readonly long TotalCrimes;
    public readonly int MinYear;
    public readonly int MaxYear;

    public CrimeStats(long total, int minyear, int maxyear)
    {
      TotalCrimes = total;
      MinYear = minyear;
      MaxYear = maxyear;
    }
  }//class

}//namespace

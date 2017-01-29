/*
*
*   Kunal Shah
*   U. of Illinois, Chicago
*   CS341, Spring 2016
*   Homework 8
*
*/





using System;
using System.Collections.Generic;
using System.Windows.Forms;
//using System.Data.SqlClient;
using System.Data;
using CHART = System.Windows.Forms.DataVisualization.Charting;


namespace ChicagoCrimeGUI
{
  public partial class Form1 : Form
  {
    //
    // private data members accessible to all functions:
    //
    private string  ConnectionInfo;


    public Form1()
    {
      InitializeComponent();

      // initialize database connection string:
      string version = "MSSQLLocalDB";
      string filename = this.txtDBFile.Text;

      ConnectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", 
        version, filename);
    }


    //
    // Called when window is about to appear for the first time:
    //
    private void Form1_Load(object sender, EventArgs e)
        {
            string filename = this.txtDBFile.Text;
            BusinessTier.Business biztier;

            biztier = new BusinessTier.Business(filename);

            if(biztier.OpenCloseConnection() == false)
            {
                MessageBox.Show("Error, unable to connect to the database");
                return;
            }

            BusinessTier.CrimeStats stats;

            stats = biztier.GetOverallCrimeStats();

            int minYear = stats.MinYear;
            int maxYear = stats.MaxYear;
            long total = stats.TotalCrimes;

            string title = string.Format("Chicago Crime Analysis from {0} - {1}, Total of {2:#,##0} crimes", minYear, maxYear, total);
            this.Text = title;


            List<BusinessTier.Area> areas;

            areas = biztier.GetChicagoAreas(BusinessTier.OrderAreas.ByName);

            foreach(BusinessTier.Area area in areas)
            {
                this.dropAreas.Items.Add(area.AreaName);
            }

            this.dropAreas.SelectedIndex = 0;

            this.tbarMinYear.Minimum = minYear;
            this.tbarMinYear.Maximum = maxYear;
            this.tbarMinYear.Value = minYear;
            this.lblMinYear.Text = minYear.ToString();
            this.tbarMaxYear.Minimum = minYear;
            this.tbarMaxYear.Maximum = maxYear;
            this.tbarMaxYear.Value = maxYear;
            this.lblMaxYear.Text = maxYear.ToString();
        }

 //------------------------------------------------------------------------------//  

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ResetListBox(string newTitle)
    {
      this.lblListboxTitle.Text = newTitle;
      this.lblListboxTitle.Refresh();

      this.listBox1.Items.Clear();
      this.listBox1.Refresh();
    }


        
//------------------------------------------------------------------------------//


    //
    // Display total crimes and arrest percentages by year in the
    // listbox:
    //
    // GetArrestPercentageByYear
    //
    private void totalAndPercentagesByYearToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.ResetListBox("Totals and Arrest Percentages by Year");

      string filename = this.txtDBFile.Text;
      BusinessTier.Business biztier;

      biztier = new BusinessTier.Business(filename);

      if (biztier.OpenCloseConnection() == false)
      {
          MessageBox.Show("Error, unable to connect to the database");
          return;
      }

      Dictionary<int, double> stats;
      Dictionary<int, long> totalCrimesAtYear;

      stats = biztier.GetArrestPercentagesByYear();
      totalCrimesAtYear = biztier.GetTotalsByYear();
      
      
      foreach(int year in stats.Keys)
      {
                int x = year;
                double y = stats[year];
                long z = totalCrimesAtYear[year];
                string msg = string.Format("{0}:  {1:#,##0} crimes, {2:0.00}% arrested",
                year, z, y);

                this.listBox1.Items.Add(msg);
      }

    }


//------------------------------------------------------------------------------//

    //Use getCrimeCodes()
    private void allCrimeCodesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.ResetListBox("All Crime Codes");

        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        List<BusinessTier.CrimeCode> codes;
        codes = biztier.GetCrimeCodes();

        foreach (BusinessTier.CrimeCode code in codes)
        {
            string msg = string.Format("{0}: {1}:{2}",
            code.IUCR,code.PrimaryDescription,code.SecondaryDescription);
            this.listBox1.Items.Add(msg);
        }
    }

 //------------------------------------------------------------------------------//


    //
    // Display all the Chicago areas by name in the listbox:
    //
    // GetChicagoAreas()
    private void allChicagoAreasToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.ResetListBox("All Chicago Areas by Name");

        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        List<BusinessTier.Area> areas;
        areas = biztier.GetChicagoAreas(BusinessTier.OrderAreas.ByName);

        foreach(BusinessTier.Area area in areas)
        {
            string msg = string.Format("{0}: #{1}",
            area.AreaName,area.AreaNumber);

            this.listBox1.Items.Add(msg);
        }
    }

    //
    // Display all the Chicago areas by number in the listbox:
    //
    private void allChicagoAreasByNumberToolStripMenuItem_Click(object sender, EventArgs e)
    {


        this.ResetListBox("All Chicago Areas by Name");

        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        List<BusinessTier.Area> areas;
        areas = biztier.GetChicagoAreas(BusinessTier.OrderAreas.ByNumber);

        foreach (BusinessTier.Area area in areas)
        {
            string msg = string.Format("#{0}: {1}",
            area.AreaNumber, area.AreaName);

            this.listBox1.Items.Add(msg);
        }
    }


    //
    // Plot total crimes by year:
    //
    // GetTotalsByYear()
    //
    private void plotTotalCrimesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //
      // let's use a separate window to show the graph:
      //
         FormPlot plot = new FormPlot();

      //
      // Compute and plot total # of crimes each year:
      //


        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        // 
        // We have the data, now let's setup and plot in the chart
        // on the other form:
        //
        plot.chart1.Series.Clear();

      // configure chart: 
      var series1 = new CHART.Series
      {
        Name = "Total Crimes",
        Color = System.Drawing.Color.Blue,
        IsVisibleInLegend = true,
        ChartType = CHART.SeriesChartType.Line
      };

      plot.chart1.Series.Add(series1);

        Dictionary<int, long> totals;
        totals = biztier.GetTotalsByYear();

        foreach(int year in totals.Keys)
        {
            int x = year;
            long y = totals[year];
            series1.Points.AddXY(x, y);
        }
    

      //
      // All set, show the window to the user --- note that if user
      // does not close window, it will be closed automatically when
      // this main window exits.
      //
      plot.Text = "** Total Crimes by Year **";
      plot.Location = new System.Drawing.Point(0, 0);  // top-left:

      plot.Show();
    }


    //
    // Plot total crimes by area:
    //
    // GetTotalsByArea()
    private void plotTotalCrimesByAreaToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //
      // let's use a separate window to show the graph:
      //
      FormPlot plot = new FormPlot();

      //
      // Compute and plot total # of crimes by each area:
      //
      string filename = this.txtDBFile.Text;
      BusinessTier.Business biztier;

      biztier = new BusinessTier.Business(filename);

      if (biztier.OpenCloseConnection() == false)
      {
          MessageBox.Show("Error, unable to connect to the database");
          return;
      }

      // 
      // We have the data, now let's setup and plot in the chart
      // on the other form:
      //
      plot.chart1.Series.Clear();

      // configure chart: 
      var series1 = new CHART.Series
      {
        Name = "Total Crimes",
        Color = System.Drawing.Color.Blue,
        IsVisibleInLegend = true,
        IsXValueIndexed = true,
        ChartType = CHART.SeriesChartType.Line
      };

      plot.chart1.Series.Add(series1);
      plot.chart1.ChartAreas[0].AxisX.Interval = 5;

      Dictionary<int, long> totals;
      totals = biztier.GetTotalsByArea();
      foreach(int area in totals.Keys)
      {
                int x = area;
                long y = totals[area];
                series1.Points.AddXY(x, y);
      }


      //
      // All set, show the window to the user --- note that if user
      // does not close window, it will be closed automatically when
      // this main window exits.
      //
      plot.Text = "** Total Crimes by Area **";
      plot.Location = new System.Drawing.Point(20, 20);  // top-left:

      plot.Show();
    }


    //
    // Plot total crimes by month:
    //
    private void plotTotalCrimesByMonthToolStripMenuItem_Click(object sender, EventArgs e)
    {
      //
      // let's use a separate window to show the graph:
      //
      FormPlot plot = new FormPlot();

        //
        // Compute and plot total # of crimes by each area:
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        // 
        // We have the data, now let's setup and plot in the chart
        // on the other form:
        //
        plot.chart1.Series.Clear();

      // configure chart: 
      var series1 = new CHART.Series
      {
        Name = "Total Crimes",
        Color = System.Drawing.Color.Blue,
        IsVisibleInLegend = true,
        ChartType = CHART.SeriesChartType.Line
      };

        plot.chart1.Series.Add(series1);

        // now plot (x, y) coordinates: 
        Dictionary<int, long> totals;
        totals = biztier.GetTotalsByMonth();
        foreach (int month in totals.Keys)
        {
            int x = month;
            long y = totals[month];
            series1.Points.AddXY(x, y);
        }

        //
        // All set, show the window to the user --- note that if user
        // does not close window, it will be closed automatically when
        // this main window exits.
        //
        plot.Text = "** Total Crimes by Month **";
         plot.Location = new System.Drawing.Point(40, 40);  // top-left:

        plot.Show();
    }


        //------------------------------------------------------------------------------//
        //---------------------------------STEP #4--------------------------------------//
        //------------------------------------------------------------------------------//




     // This is the button that searches based on 3 checkboxes:
     //
     // YEAR, IUCR, AND AREA
     //
     // GetTotalsByArea
     // GetTotalsByYear
     // GetTotalsBy
    private void cmdTotalCrimes_Click(object sender, EventArgs e)
    {
      //
      // make sure at least 1 check box is checked:
      //
      if (chkYear.Checked == false &&
        chkIUCR.Checked == false &&
        chkArea.Checked == false)
      {
        MessageBox.Show("Please check at least one search criteria...");
        return;
      }

            //Use these three to pass into the funciton created in biztier
            string y = "%"; ///Year
            string c = "%"; //Iucr Code
            string a= "%"; // AreaNum
      
      
      // 
      // Okay, at least one check box is checked, now build 
      // WHERE clause based on user selection...
      //
      //string where = "";

      if (chkYear.Checked)
      {
        int year;

        if (Int32.TryParse(this.txtYear.Text, out year) == false)
        {
          MessageBox.Show("Year must be numeric...");
          return;
        }

         y = Convert.ToString(year);
      }

      if (chkIUCR.Checked)
      {
        
        string iucr = this.txtIUCR.Text.Replace("'", "''");
         c = iucr;
      }

      if (chkArea.Checked)
      {
        
        int area;

        if (Int32.TryParse(this.txtArea.Text, out area) == false)
        {
          MessageBox.Show("Area must be numeric...");
          return;
        }
        a = Convert.ToString(area);
      }

        // MessageBox.Show(where);

        //
        // We have the where clause ready, so now we can build and
        // execute the SQL query needed to compute the total #
        // of crimes:
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }




     // NOTE: we always get a result back, worst-case it's 0.

      long total = biztier.GetTotalCrimesByCombo(y, c, a);
      
      this.lblTotalCrimes.Text = total.ToString("#,##0");
    }

    private void chkYear_CheckedChanged(object sender, EventArgs e)
    {
      this.lblTotalCrimes.Text = "?";
    }

    private void chkIUCR_CheckedChanged(object sender, EventArgs e)
    {
      this.lblTotalCrimes.Text = "?";
    }

    private void chkArea_CheckedChanged(object sender, EventArgs e)
    {
      this.lblTotalCrimes.Text = "?";
    }


    //
    // Top N Areas in the city for crime:
    //
    private int GetN()
    {
      int N;

      if (Int32.TryParse(this.txtTopN.Text, out N) == false)
      {
        MessageBox.Show("N must be numeric...");
        return -1;
      }

      if (N < 1)
      {
        MessageBox.Show("N must be > 0...");
        return -1;
      }

      return N;
    }

    private void cmdTopAreas_Click(object sender, EventArgs e)
    {
      this.listBox2.Items.Clear();

      //
      // First, get the N value for our top N:
      //
      int N = GetN();

      if (N < 1)
        return;

        //
        // Okay, execute query to retrieve top N areas for crime...
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }

        int i = 1;
        Dictionary<string, long> totals;
        totals = biztier.GetTopAreas(N);
        foreach(string area in totals.Keys)
        {
            string a = area;
            long y = totals[area];
            string msg = string.Format("{0}. {1}: {2:#,##0}",
                i,
                a,
                y);
            this.listBox2.Items.Add(msg);
            i++;
        }
    }


    //
    // Top N types of crime:
    //
    private void cmdTopCrimeTypes_Click(object sender, EventArgs e)
    {
      this.listBox2.Items.Clear();

      //
      // First, get the N value for our top N:
      //
      int N = GetN();

      if (N < 1)
       {
         return;
       }
        

        //
        // Okay, execute query to retrieve top N types of crime...
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }
        int i = 1;
        Dictionary<BusinessTier.CrimeCode, long> totals;
        totals = biztier.GetTopCrimeTypes(N);

        foreach (BusinessTier.CrimeCode code in totals.Keys)
        {
            string msg = string.Format("{0}. {1}", i,
                code.PrimaryDescription);

            this.listBox2.Items.Add(msg);

            msg = string.Format("    {0}",
                code.SecondaryDescription);

            this.listBox2.Items.Add(msg);

            msg = string.Format("    {0:#,##0}",
                totals[code]);

            this.listBox2.Items.Add(msg);

            i++;

        }
    }


    //
    // Top N areas for a particular type of crime:
    //
    private void cmdTopAreasForThisCrimeType_Click(object sender, EventArgs e)
    {
      this.listBox2.Items.Clear();

      //
      // First, get the N value for our top N:
      //
      int N = GetN();

      if (N < 1)
        return;

      //
      // Now retrieve the crime code the user is interested in:
      //
      string iucr = this.txtIUCR2.Text.Replace("'", "''");

        //
        // Okay, execute query to retrieve top N areas for this
        // type of crime:
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        }
        int i = 1;
        Dictionary<string, long> totals;
        totals = biztier.GetTopAreaByCrime(N, iucr);

        foreach (string area in totals.Keys)
        {
            string msg = string.Format("{0}. {1}: {2:#,##0}",
            i,area,totals[area]);

            this.listBox2.Items.Add(msg);

            i++;
        }
   }

    private void tbarMinYear_Scroll(object sender, EventArgs e)
    {
      this.lblMinYear.Text = tbarMinYear.Value.ToString();
    }

    private void tbarMaxYear_Scroll(object sender, EventArgs e)
    {
      this.lblMaxYear.Text = tbarMaxYear.Value.ToString();
    }


    //
    // Top N crimes in a given area across a range of years:
    //
    private void cmdTopCrimesGivenAreaAndYears_Click(object sender, EventArgs e)
    {
      this.listBox2.Items.Clear();

      //
      // First, get the N value for our top N:
      //
      int N = GetN();

      if (N < 1)
        return;

      //
      // Second, what area did the user select?
      //

      if (this.dropAreas.SelectedIndex < 0)
      {
        MessageBox.Show("Please select an area...");
        return;
      }

      string areaname = this.dropAreas.SelectedItem.ToString();
      areaname = areaname.Replace("'", "''");

      //
      // Third, what year range?
      //
      int minyear = this.tbarMinYear.Value;
      int maxyear = this.tbarMaxYear.Value;

      if (minyear > maxyear)
      {
        MessageBox.Show("Please select a non-empty range of years...");
        return;
      }

        //
        // Okay, we have the input we need, so now let's execute a
        // query to retrieve the top N crimes in this area over
        // this range of years:
        //
        string filename = this.txtDBFile.Text;
        BusinessTier.Business biztier;

        biztier = new BusinessTier.Business(filename);

        if (biztier.OpenCloseConnection() == false)
        {
            MessageBox.Show("Error, unable to connect to the database");
            return;
        };

        int i = 1;
        Dictionary<BusinessTier.CrimeCode, long> totals;
        totals = biztier.GetTopByAreaAndYear(N, areaname, minyear, maxyear);

        foreach (BusinessTier.CrimeCode code in totals.Keys)
        {
            string msg = string.Format("{0}. {1}", i,
                code.PrimaryDescription);

            this.listBox2.Items.Add(msg);

            msg = string.Format("    {0}",
                code.SecondaryDescription);

            this.listBox2.Items.Add(msg);

            msg = string.Format("    {0:#,##0}",
                totals[code]);

            this.listBox2.Items.Add(msg);

            i++;

        }



            // MessageBox.Show(sql);
     }

        
    }//class
}//namespace

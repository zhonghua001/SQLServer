/*

  Comprehensive C# class for messing with files - fit for ETL development; most can be used independently - note dependent methods.
  See https://github.com/tmmtsmith/SQLServer/blob/master/ETL/CSharpWriteBadAndGoodLinesByDelimiter.cs
  
*/

// SQL Related to files
// using System.Data.SqlClient;
public static class Connections
{
    public static SqlConnection Connect()
    {
        SqlConnection scon = new SqlConnection("");
        return scon;
    }
    
    public static void BulkCopyTable (DataTable datatable, string table)
    {
        using (var scon = Connections.Connect())
        {
            scon.Open();
            using (SqlBulkCopy copydtab = new SqlBulkCopy(scon))
            {
                copydtab.DestinationTableName = table;
                copydtab.WriteToServer(datatable);
            }
            scon.Close();
            scon.Dispose();
        }
    }
}

// Files
public static class ReadFiles
{
    public static string SelectFirstLine(string file)
    {
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        string line = "";
        for (int i = 1; i < 2; i++)
        {
            line = readfile.ReadLine();
        }
        readfile.Close();
        readfile.Dipose();
        return line;
    }

    public static string GetLineByNumber(string file, int lineNo)
    {
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        string line = "";
        for (int i = 1; i < (lineNo + 1); i++)
        {
            line = readfile.ReadLine();
        }
        readfile.Close();
        readfile.Dispose();
        return line;
    }
    
    public static int GetLastLineNumber(string file)
    {
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        string line;
        int cnt = 0;
        while ((line = readfile.ReadLine()) != null)
        {
            cnt++;
        }
        readfile.Close();
        readfile.Dipose();
        return cnt;
    }

    public static string GetFileName(string file)
    {
        string f = file.Substring(file.LastIndexOf("\\") + 1);
        f = f.Substring(0, f.IndexOf("."));
        return f;
    }

    public static string GetFileNameWithExtension(string file)
    {
        string f = file.Substring(file.LastIndexOf("\\") + 1);
        return f;
    }
    
    // Two of the below four depend on the top two - CountInvalidLines and CountValidLines

    public static int CountInvalidLines(string file, int validcount, char ch)
    {
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        int cnt = 0, total;
        string line;

        while ((line = readfile.ReadLine()) != null)
        {
            total = line.Split(ch).Length - 1;
            if (total != validcount)
            {
                cnt++;
            }
        }
        readfile.Close();
        readfile.Dispose();
        return cnt;
    }

    public static int CountValidLines(string file, int validcount, char ch)
    {
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        int cnt = 0, total;
        string line;

        while ((line = readfile.ReadLine()) != null)
        {
            total = line.Split(ch).Length - 1;
            if (total == validcount)
            {
                cnt++;
            }
        }
        readfile.Close();
        readfile.Dispose();
        return cnt;
    }

    public static double InvalidToValid(string file, int validcount, char ch)
    {
        // Dependent: requires methods CountInvalidLines and CountValidLines
        double x = Convert.ToDouble((CountInvalidLines(file, validcount, ch))) / Convert.ToDouble((CountValidLines(file, validcount, ch)));
        return x;
    }
    
    public static double InvalidCost(double cost, string file, int validcount, char ch)
    {
        // Dependent: requires methods InvalidToValid, CountInvalidLines and CountValidLines
        double x = (InvalidToValid(file, validcount, ch)) * cost);
        return x;
    }

    public static int OutputInvalidandValidData(string file, int validcount, char ch)
    {
        string loc = file.Substring(0, file.LastIndexOf("\\") + 1);
        string f = file.Substring(file.LastIndexOf("\\") + 1);
        f = f.Substring(0, f.IndexOf("."));

        string validfile = loc + f + "_valid.txt";
        string invalidfile = loc + f + "_invalid.txt";

        if (System.IO.File.Exists(validfile) == true)
        {
            System.IO.File.Delete(validfile);
        }

        if (System.IO.File.Exists(invalidfile) == true)
        {
            System.IO.File.Delete(invalidfile);
        }

        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        System.IO.StreamWriter writevalid = new System.IO.StreamWriter(validfile);
        System.IO.StreamWriter writeinvalid = new System.IO.StreamWriter(invalidfile);
        int cnt = 0, total;
        string line;

        while ((line = readfile.ReadLine()) != null)
        {
            total = line.Split(ch).Length - 1;
            if (total == validcount)
            {
                writevalid.WriteLine(line);
                writevalid.Flush();
            }
            else
            {
                writeinvalid.WriteLine(line);
                writeinvalid.Flush();
                cnt++;
            }
        }
        readfile.Close();
        readfile.Dispose();
        writevalid.Close();
        writevalid.Dispose();
        writeinvalid.Close();
        writeinvalid.Dispose();
        return cnt;
    }
    
    public static int EveryNLine(string file, int n, string outputfile)
    {
        string loc = file.Substring(0, file.LastIndexOf("\\") + 1);
        string f = file.Substring(file.LastIndexOf("\\") + 1);
        f = f.Substring(0, f.IndexOf("."));

        string nfile = loc + f + outputfile + ".txt";

        if (System.IO.File.Exists(nfile) == true)
        {
            System.IO.File.Delete(nfile);
        }

        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        System.IO.StreamWriter writenfile = new System.IO.StreamWriter(nfile);

        int cnt = 1, no = 0;
        string line;

        while ((line = readfile.ReadLine()) != null)
        {
            if (cnt == 1)
            {
                writenfile.WriteLine(line);
                writenfile.Flush();
                no++;
            }
            else if ((cnt % n == 0) == true)
            {
                writenfile.WriteLine(line);
                writenfile.Flush();
                no++;
            }
            cnt++;
        }

        readfile.Close();
        readfile.Dispose();
        writenfile.Close();
        writenfile.Dispose();

        return no;
    }
    
    public static double PercentCleared(List<int> yes, List<int> no)
    {
        double per = ((Convert.ToDouble(y.Count()) / Convert.ToDouble(n.Count() + y.Count())) * 100);
        return per;
    }
    
    public static void RemoveInvalid(DataTable dt, List<int> no)
    {
        foreach (int i in no)
        {
            dt.Rows[i].Delete();
        }
    }
    
    public static void CountData (string file, char ch)
    {
        string f = file.Substring(file.LastIndexOf("\\") + 1);
        f = f.Substring(0, f.IndexOf("."));
        
        System.IO.StreamReader readfile = new System.IO.StreamReader(file);
        int lineno = 0;
        
        while ((line = readfile.ReadLine()) != null)
        {
            lineno++;
            total = line.Split(ch).Length - 1;
            // con
            
        }
    }
    
    // Line(s) by key - drive only
    // Tagged lines - drive only
    // Auto, DT, Wrap - drive only
    // Testing exceptions (placement where?)
}

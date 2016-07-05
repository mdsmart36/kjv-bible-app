using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BuildKJVDataTable
{
    public class SqlDataManager
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["BuildKJVDataTable.Properties.Settings.kjv_bibleConnectionString"].ConnectionString;

        public SqlDataManager()
        {

        }

        public void standardizeKjvText()
        {
            // Standardize book titles which consist of more than 1 word
            string[] titlesToStandardize = { "1 Samuel", "2 Samuel", "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Song of Solomon", "1 Corinthians", "2 Corinthians", "1 Thessalonians", "2 Thessalonians", "1 Timothy", "2 Timothy", "1 Peter", "2 Peter", "1 John", "2 John", "3 John" };
            string[] standardizedTitles = { "1Samuel", "2Samuel", "1Kings", "2Kings", "1Chronicles", "2Chronicles", "SongOfSolomon", "1Corinthians", "2Corinthians", "1Thessalonians", "2Thessalonians", "1Timothy", "2Timothy", "1Peter", "2Peter", "1John", "2John", "3John" };
            
            var inputFileName = @"c:\Users\Matthew\Downloads\bible.txt\bible.txt";
            var outputFileName = @"c:\Users\Matthew\Downloads\bible.txt\kjv-standard.txt";

            // read a line from kjv
            // while readline != null
            // foreach title in titles_to_standardize
            //    if title matches beginning of line, replace with standardized title (title with spaces removed)
            //  
            string pattern = @"^";
            string line;
            using (StreamReader sr = new StreamReader(inputFileName))
            {
                using (StreamWriter sw = new StreamWriter(outputFileName))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < titlesToStandardize.Length; i++)
                        {
                            Match match = Regex.Match(line, pattern + titlesToStandardize[i]);
                            if (match.Success)
                            {
                                line = line.Replace(titlesToStandardize[i], standardizedTitles[i]);
                            }
                        }
                        sw.WriteLine(line);
                    }
                }
            }
        }

        public void buildKjvTable(string filename)
        {
#if DEBUG
            this.TruncateMySQLTable("ReferenceAndText");
#endif
            char referenceDelimiter = '\t';
            var chapterVerseDelimiter = ':';
            var wordDelimiter = ' ';
            
            string commandText = @"INSERT INTO [dbo].[ReferenceAndText] ( Book, Chapter, Verse, Content) VALUES (@Book, @Chapter, @Verse, @Content)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        // part1        text
                        // Genesis 1:1\tIn the beginning, God created the heaven and the earth.
                        string line, book, chapterVerse, chapter, verse, text, part1, part2;
                        while ((line = sr.ReadLine()) != null)
                        {
                            part1 = line.Split(referenceDelimiter)[0];
                            chapterVerse = part1.Split(wordDelimiter)[1];
                            book = part1.Split(wordDelimiter)[0];
                            chapter = chapterVerse.Split(chapterVerseDelimiter)[0];
                            verse = chapterVerse.Split(chapterVerseDelimiter)[1];
                            text = line.Split(referenceDelimiter)[1];

                            SqlCommand cmd = new SqlCommand(commandText, connection);
                            cmd.Parameters.AddWithValue("@Book", book);
                            cmd.Parameters.AddWithValue("@Chapter", chapter);
                            cmd.Parameters.AddWithValue("@Verse", verse);
                            cmd.Parameters.AddWithValue("@Content", text);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool TruncateMySQLTable(string tableName)
        {
            string commandText = @"truncate table " + tableName;
            bool success;
            try
            {
                using (SqlConnection connection = new SqlConnection(this.connectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(commandText, connection);
                    cmd.ExecuteNonQuery();                    
                    success = true;
                }                

            }
            catch (Exception ex)
            {
                throw;
            }

            return success;

        }
    }
}

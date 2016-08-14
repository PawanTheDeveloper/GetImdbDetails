using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
//using log4net;
//using log4net.Config;

namespace ImdbDetails
{
    public class GetImdbDetails
    {
        /*OMDB Request Parameters.
         * s (NEW!)	 string (optional)	 title of a movie to search for
           i	 string (optional)	 a valid IMDb movie id
           t	 string (optional)	 title of a movie to return
           y	 year (optional)	 year of the movie
           r	 JSON, XML	 response data type (JSON default)
         * */

        private const string IMDB_TITLE_START = "http://www.omdbapi.com/?t=";
        private const string IMDB_ID_START = "http://www.omdbapi.com/?i=";
        private const string IMDB_URL_END = "&r=Xml";
        private Dictionary<string, string> movieDetailsDictionary = new Dictionary<string, string>();
        private const string movie = "movie";
        private const string byID = "byID";
        private const string byTitle = "byTitle";
        
        #region Private Methods

        //private static readonly ILog logger = LogManager.GetLogger(typeof(GetImdbDetails));

        private void removeUnwantedCharacters(ref string name)
        {
            name = name.Replace("(", "  ");
            name = name.Replace("[", "  ");
            name = name.Replace("_", " ");
            name = name.Replace("-", " ");
            name = name.Replace("{", "  ");
            name = name.Replace(".", " ");
            name = name.Trim();
        }
        private void MovieDetails(string name)
        {
            bool result = false;
            char[] seperator = { ' ', '|', };
            string[] name_split;
            if (name.Contains("."))
            {
                int index = name.LastIndexOf('.');
                name = name.Remove(index);
            }
            if (GetResultByDirectName(name))
                return;
            removeUnwantedCharacters(ref name);
            name_split = name.Split(seperator);
            if (GetResultByDateModifying(name_split))
                return;
            //Send the name splited as string and keep creating the name and check on imdb.
            if (GetResultByAddingNames(name_split))
                return;
        }
         private bool GetResultByAddingNames(string[] name_split)
        {
            int i = 0;
            string name_build = string.Empty;
            bool done = false;
            foreach (string s in name_split)
            {
                name_build = name_build + name_split[i]+" ";
                name_build.ToString().Trim();
                getResponse(name_build,byTitle);
                if (movieDetailsDictionary.Count == 0)
                {
                    if (i < 4)
                    {
                        i++;
                        continue;
                    }
                    else
                        return false;
                }
                else
                    return true;
            }
            return false;
        }
        
        private bool GetResultByDirectName(string movie_name)
        {
            getResponse(movie_name,byTitle);
            if(movieDetailsDictionary.Count==0)
                return false;
            else
                return true;
        }

        private bool GetResultByDateModifying(string[] movie_name)
        {
            StringBuilder name_build=new StringBuilder();
            int number;
            foreach (string name in movie_name)
            {
                if (name.Equals(""))
                    break;
                
                else if (!(int.TryParse(name, out number)))
                {
                    name_build.Append(name + " ");
                    continue;
                }
                else
                {
                    break;
                }
            }
            string send_name = name_build.ToString().Trim();
            getResponse(send_name,byTitle);
            if (movieDetailsDictionary.Count == 0)
                return false;
            else
                return true;
        }

        /*
        * Summary:
        * This function will create the proper URL and send it to collect Attributes.
        * */

        private string getFullURL(string name,string parameter)
        {
            if (parameter.Equals(byTitle))
                return IMDB_TITLE_START + name + IMDB_URL_END;
            else
                return IMDB_ID_START + name + IMDB_URL_END;
        }
        
//All the required attributes will be extracted from XML file (reader object) and Dictionary mapping is sent.
        private Dictionary<string,string> getAllAttributes(XmlTextReader reader)
        {
            Dictionary<string, string> tempDictionaryToHold = new Dictionary<string, string>();
            try
            {
                while (reader.Read())
                {
                    if (reader.Name.Equals(movie))
                    {
                        reader.MoveToNextAttribute();
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            tempDictionaryToHold.Add(reader.Name, reader.Value);
                            reader.MoveToNextAttribute();
                        }
                    }
                }
            }
            catch (XmlException exception)
            {
            }
            return tempDictionaryToHold;
        }

        //This function will get the Response from OMDB.
        private void getResponse(string name,string parameter)
        {
            string finalURL = string.Empty;
            finalURL = getFullURL(name, parameter);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Uri urlToSend = new Uri(finalURL);
            try
            {
                request = (HttpWebRequest)WebRequest.Create(urlToSend);
                response = (HttpWebResponse)request.GetResponse();
                Stream respStream = response.GetResponseStream();
                XmlTextReader reader = new XmlTextReader(respStream);

                //Get the Attribute List to send.
                movieDetailsDictionary = getAllAttributes(reader);
            }
            catch (WebException excep)
            {
            }
            catch (Exception e)
            {
            }
        }

        #endregion
        
        #region Public Methods

        public GetImdbDetails()
        {
           // BasicConfigurator.Configure();
        }
        //Summary:
        //URL will be created and sent for further analysis.
        public Dictionary<string, string> GetDetailsByName(string movieTitle)
        {
            MovieDetails(movieTitle);
            return movieDetailsDictionary;
        }

        public Dictionary<string, string> GetDetailsById(string movieID)
        {
            getResponse(movieID, byID);        
            return movieDetailsDictionary;
        }
        #endregion
    }
}

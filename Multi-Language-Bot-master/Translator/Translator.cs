﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using HotelBot.Models;
using HotelBot.Utilities;
using Microsoft.Bot.Connector;

namespace HotelBot.Translator
{
    public class Translator
    {
        internal AdmAccessToken Bearer;

        internal Translator()
        {
            if (GlobalVars.Bearer == null)
            {
                Bearer = Task.Run(GetBearerTokenForTranslator).Result;
            }
            else
            {
                if (GlobalVars.Bearer.IsExpired)
                {
                    Bearer = Task.Run(GetBearerTokenForTranslator).Result;
                }
                else
                {
                    Bearer = GlobalVars.Bearer;
                }
            }
        }

        public string Translate(string inputText, string inputLocale, string outputLocale)
        {
            //try
            //{
            //    string uri =
            //        $"{Settings.GetTranslator()}Translate?text={HttpUtility.UrlEncode(inputText)}&from={inputLocale}&to={outputLocale}";

            //    WebRequest translationWebRequest = WebRequest.Create(uri);
            //    translationWebRequest.Headers.Add("Authorization", Bearer.Header);

            //    WebResponse response = null;
            //    response = translationWebRequest.GetResponse();
            //    Stream stream = response.GetResponseStream();
            //    Encoding encode = Encoding.GetEncoding("utf-8");

            //    StreamReader translatedStream = new StreamReader(stream, encode);
            //    XmlDocument xTranslation = new XmlDocument();
            //    xTranslation.LoadXml(translatedStream.ReadToEnd());

            //    return xTranslation.InnerText;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
           
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(inputText) + "&from=" + inputLocale + "&to=" + outputLocale;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", Bearer.Header);
            WebResponse response = null;
            string result;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    //result="Translation for source text"+text+" from en to de";
                    result = translation;
                    return result;
                }
              
            }
            catch
            {
                throw;
            }
            finally
            {
                //if (response != null)
                //{
                //    response.Close();
                //    response = null;

                //}
               
            }

        }

        internal string Detect(string input)
        {
            try
            {
                string uri = $"{Settings.GetTranslatorUri()}Detect?text=" + HttpUtility.UrlEncode(input);
                WebRequest translationWebRequest = WebRequest.Create(uri);
                translationWebRequest.Headers.Add("Authorization", Bearer.Header);

                WebResponse response = null;
                response = translationWebRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");

                StreamReader translatedStream = new StreamReader(stream, encode);
                XmlDocument xTranslation = new XmlDocument();
                xTranslation.LoadXml(translatedStream.ReadToEnd());

                return xTranslation.InnerText;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            
        }

        internal async Task<AdmAccessToken> GetBearerTokenForTranslator()
        {
            try
            {
                var azureDataMarket = new AzureDataMarket();
                var token = await azureDataMarket.GetAccessToken();
                GlobalVars.Bearer = token;
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
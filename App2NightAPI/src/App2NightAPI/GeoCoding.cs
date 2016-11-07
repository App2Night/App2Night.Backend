using App2Night.Shared;
using App2NightAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace App2NightAPI
{
    public class GeoCoding
    {
        private static HttpClient client = new HttpClient();
        private static readonly string httpUrl = "https://maps.googleapis.com/maps/api/geocode/json?";

        public static async void GetLocationByCoordinates(double lat, double lon)
        {
            if (lat != 0 && lon != 0)
            {
                String Url = httpUrl + "latlng=" + lat.ToString() + "," + lon.ToString() + "&key=" + new Secrets().GoogleMapsApiKey;
                _initializeHttpClient();
                HttpResponseMessage message = await client.GetAsync(Url);
                if (!message.IsSuccessStatusCode)
                {
                    //Something went wrong.
                }
                else
                {
                    String response = message.Content.ReadAsStringAsync().Result;
                    //if (String.IsNullOrEmpty(response))
                    //{
                    //    //TODO return string is empty
                    //}
                    //else
                    //{
                          //Begin with json parsing
                       try
                        {
                            if (response.StartsWith("["))
                                {
                                    response = response.Remove(0, 1);
                                }
                            if (response.EndsWith("]"))
                                {
                                    response = response.Remove((response.Length - 1), 1);
                                }

                            GoogleGeoCodingResponse test = JsonConvert.DeserializeObject<GoogleGeoCodingResponse>(response);

                        //        var test = JObject.Parse(response);
                        //        var partId = test["PartId"];
                    }
                        catch (Exception)
                        {
                            //TODO Something went wrong during the parsing process
                        }
                    //}
                    
                }
            }
            else
            {
                //TODO Errorhandling: latitude or longitude = 0
            }
        }

        private static void _initializeHttpClient()
        {
            client.BaseAddress = new Uri("http://localhost:5001");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static double? GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
                //Test coordinates
                lat1 = 48.2086369;
                lon1 = 8.7548875;
                lat2 = 48.4456403;
                lon2 = 8.6942879;

                var R = 6371; //Radius Earth
                var deltaLat = deg2rad(lat2 - lat1);
                var deltaLon = deg2rad(lon2 - lon1);
                var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var d = R * c; //Distance in km
                return d;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static double deg2rad(double deg)
        {
            return deg * (Math.PI / 100);
        }
    }
}

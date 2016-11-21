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
        private static HttpClient client;
        private static readonly string httpUrl = "https://maps.googleapis.com/maps/api/geocode/json?";

        public static async Task<Location> GetLocationByCoordinates(double lat, double lon)
        {
            if (lat != 0 && lon != 0)
            {
                String Url = httpUrl + "latlng=" + lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + lon.ToString(System.Globalization.CultureInfo.InvariantCulture) + "&key=" + new Secrets().GoogleMapsApiKey;
                _initializeHttpClient();
                HttpResponseMessage message = await client.GetAsync(Url);
                if (!message.IsSuccessStatusCode)
                {
                    //Something went wrong.
                    return null;
                }
                else
                {
                    Location l = _executeResponseMessage(message);
                    return l;
                }
            }
            else
            {
                //Errorhandling: latitude or longitude = 0
                return null;
            }
        }

        public static async Task<Location> GetLocationByAdress(string housNo, string street, string cityName)
        {
            String Url = httpUrl + "address=" + housNo + "+" + street + "+" + cityName + "&key=" + new Secrets().GoogleMapsApiKey;

            _initializeHttpClient();
            HttpResponseMessage message = await client.GetAsync(Url);
            if (!message.IsSuccessStatusCode)
            {
                //Something went wrong.
                return null;
            }
            else
            {
                Location l = _executeResponseMessage(message);
                return l;
            }
        }

        private static Location _executeResponseMessage(HttpResponseMessage message)
        {
            String response = message.Content.ReadAsStringAsync().Result;

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

                GoogleGeoCodingResponse googleResponse = JsonConvert.DeserializeObject<GoogleGeoCodingResponse>(response);

                if (googleResponse.status == "ZERO_RESULTS")
                {
                    return null;
                }
                else if (googleResponse.status == "OK")
                {
                    Location loc = _getLocationFromJson(googleResponse);
                    return loc;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //Something went wrong during the parsing process
                return null;
            }
        }

        private static Location _getLocationFromJson(GoogleGeoCodingResponse googleResponse)
        {
            try
            {
                Location loc = new Location();

                results res = googleResponse.results[0];
                loc.Latitude = Convert.ToDouble(res.geometry.location.lat, System.Globalization.CultureInfo.InvariantCulture);
                loc.Longitude = Convert.ToDouble(res.geometry.location.lng, System.Globalization.CultureInfo.InvariantCulture);

                foreach (address_component adress in res.address_components)
                {
                    if (adress.types.Contains("street_number"))
                    {
                        loc.HouseNumber = adress.long_name;
                    }
                    else if (adress.types.Contains("route"))
                    {
                        loc.StreetName = adress.long_name;
                    }
                    else if (adress.types.Contains("locality"))
                    {
                        loc.CityName = adress.long_name;
                    }
                    else if (adress.types.Contains("country"))
                    {
                        loc.CountryName = adress.long_name;
                    }
                    else if (adress.types.Contains("postal_code"))
                    {
                        loc.Zipcode = adress.long_name;
                    }
                }
                return loc;
            }
            catch(Exception)
            {
                return null;
            }
        }

        private static void _initializeHttpClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5001");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static double? GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            try
            {
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

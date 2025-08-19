using AdvertisingWebService.Models;

namespace AdvertisingWebService.Services
{
    public class AdvertisingService
    {
        private readonly List<AdvertisingModel> _adsPlatforms = new List<AdvertisingModel>();

        public void LoadFromFile(string path)
        {
            var newAdsPlatforms = new List<AdvertisingModel>();

            if (!File.Exists(path))
                return;

            foreach (var line in File.ReadLines(path))
            {
                try
                {
                    var splitedLine = line.Split(':', 2);
                    var name = splitedLine[0].Trim();
                    var locations = splitedLine[1].Split(',').Select(x => x.Trim()).ToList();

                    if ((name != null) && (locations.Count() > 0))
                        newAdsPlatforms.Add(new AdvertisingModel { Locations = locations, Name = name });
                }catch(Exception ex)
                {
                    continue;
                }
            }
            
            _adsPlatforms.Clear();
            _adsPlatforms.AddRange(newAdsPlatforms);
        
        }

        public List<string> GetPlatformsList(string location)
        {
            if (location == null)
            {
                return new List<string>();
            }

            var tempList = _adsPlatforms;

            //var result = new List<string>();
            //foreach (var p in tempList)
            //{
            //    foreach (var loc in p.Locations)
            //    {
            //        if (location.StartsWith(loc))
            //        {
            //            result.Add(p.Name);
            //            break; 
            //        }
            //    }
            //}
            //result = result.Distinct().ToList();

            //Более эфективно будет использовать линкью
            var result = tempList.Where(x=> x.Locations.Any(l=> location.StartsWith(l))).Select(x=>x.Name).Distinct().ToList();
            return result;
        }
        
    }
}

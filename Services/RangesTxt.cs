using EmailSender.Interfaces;
using EmailSender.Model;
using System.IO;
using System.Linq;

namespace EmailSender.Services
{
    public class RangesTxt : ILoadRanges
    {
        public async void Load(string filePath, Pauses pauses)
        {
            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string line;
                int i = 10;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var mas = line.Split('-');
                    if(mas.Length>1)
                    {
                        int start = 0;
                        int end = 0;                        
                        int.TryParse(mas[0], out start);
                        int.TryParse(mas[1], out end);
                        
                        if (start > end)
                        {
                            int k = start;
                            start = end;
                            end = k;
                        }

                        pauses.Ranges.Add(new RangePause() { Start = start, End = end, Id = i });
                        i++;
                    }                    
                }
                if(pauses.Ranges != null)
                {
                    pauses.ActiveRange = pauses.Ranges.FirstOrDefault();
                }
            }
        }
       
    }
}

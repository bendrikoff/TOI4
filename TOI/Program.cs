using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOI
{
    class Program
    {
       
        static List<string> startDocs = new List<string>();
        static void Main(string[] args)
        {
            //startDocs.Add("Британская полиция знает о местонахождении основателя WikiLeaks");
            //startDocs.Add("В суде США начинается процесс против россиянина, рассылавшего спам");
            //startDocs.Add("Церемонию вручения Нобелевской премии мира бойкотируют 19 стран");
            //startDocs.Add("В Великобритании арестован основатель сайта Wikileaks Джулиан Ассандж");
            //startDocs.Add("Украина игнорирует церемонию вручения Нобелевской премии");
            //startDocs.Add("Шведский суд отказался рассматривать апелляцию основателя Wikileaks");
            //startDocs.Add("НАТО и США разработали планы обороны стран Балтии против России");


            Console.WriteLine("Введите путь к файлу:");

            //string FilePath = "C:/Users/admin/Desktop/TOI.txt";

           string FilePath = Console.ReadLine();

            Console.WriteLine("Введите запрос:");
            string request = Console.ReadLine().ToLower();
           // string request = "основатель сайта";

            using (StreamReader sr = new StreamReader(FilePath))
            {
                while (!sr.EndOfStream)
                {
                    startDocs.Add(sr.ReadLine());
                  
                }

            }

            BoolSearch(request);
            CalcScore(request);


            Console.ReadKey();

        }
        //Составление списка слов по одному
        public static void BoolSearch(string request)
        {
            
            List<string> document = new List<string>();
            foreach (var docs in startDocs)
            {
                document.Add(docs);
            }
            StemmAllText(ref document);

            List<string> wordList = CreateWordList(ref document);
            DelStopWordsText(ref wordList);
            ListCreate(ref wordList);

            Dictionary<string, List<int>> invIndex = new Dictionary<string, List<int>>();
            foreach (var a in wordList)
            {
                //Console.WriteLine(a);
                invIndex.Add(a, new List<int>());
            }
            foreach (var a in invIndex)
            {
                for (int i = 0; i < document.Count; i++)
                {
                    if (document[i].Contains(a.Key))
                    {
                        a.Value.Add(i);
                    }

                }
            }
            Console.WriteLine("Инвертированный индекс:");
            foreach (var a in invIndex)
            {
                Console.Write(a.Key + "-");
                foreach (var b in a.Value)
                {
                    Console.Write(b + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();

            Dictionary<string, List<int>> invIndexGood = new Dictionary<string, List<int>>();

           
            List<string> requestWords = request.ToLower().Split(' ').ToList<string>();
            for (int i = 0; i < requestWords.Count; i++)
            {
                requestWords[i] = Stemming.TransformingWord(requestWords[i]);
            }
            DelStopWordsText(ref requestWords);


            foreach (var requestWord in requestWords)
            {
               
                if (invIndex.ContainsKey(requestWord))
                {
                    foreach (var a in invIndex)
                    {
                        // Console.WriteLine(":"+a.Key+" "+requestWord+ a.Key.Length + " " + requestWord.Length);
                        if (a.Key == requestWord)
                        {

                            invIndexGood.Add(a.Key, a.Value);
                        }
                    }
                }
                else
                {
                    invIndexGood.Add(requestWord,new List<int>());
                }
                
            }

            Console.WriteLine("Булев поиск:");
            List<int> result = new List<int>();
           foreach(var goodInd in invIndexGood)
            {
                foreach(var number in goodInd.Value)
                {
                    int count = 0;
                    foreach(var goodInd2 in invIndexGood)
                    {
                        if (goodInd2.Value.Contains(number))
                        {
                            count++;
                        }

                    }
                    if (count == invIndexGood.Count)
                    {
                        if (!result.Contains(number))
                        {
                            result.Add(number);
                        }
                    }


                }

            }
           foreach(var a in result)
            {
                Console.WriteLine(startDocs[a]);
            }

            Console.WriteLine();
        }
        public static void ListCreate(ref List<string> words)
        {
            List<string> temp = new List<string>();

            foreach (var a in words)
            {
                if (!temp.Contains(a))
                {
                    temp.Add(a);
                }
            }
            words = temp;
        }
        
        static void DelStopWordsText(ref List<string> AllWords)
        {
            string StopWordsStr = File.ReadAllText("StopWords.txt");

            List<string> StopWords = StopWordsStr.Split(' ').ToList<string>();

  
            for (int i = 0; i < AllWords.Count; i++)
            {
                for (int j = 0; j < StopWords.Count; j++)
                {
                    try
                    {
                        if (AllWords[i] == StopWords[j])
                        {
                            AllWords.Remove(AllWords[i]);
                        }
                    }
                    catch
                    {

                    }

                }
            }
            for (int i = 0; i < AllWords.Count; i++)
            {
                if (AllWords[i] == "")
                {
                    AllWords.Remove(AllWords[i]);
                }
            }
            


         }
        static void StemmAllText(ref List<string> AllWords)
        {
            
            for (int i = 0; i < AllWords.Count; i++)
            {
                
                List<string> words = AllWords[i].Split(' ').ToList<string>();
                AllWords[i] = "";
                for (int j = 0; j < words.Count; j++)
                {
                    words[j] = Stemming.TransformingWord(words[j]);
                    AllWords[i] += words[j] + " ";

                }
                
            }

        }
        static List<string> CreateWordList(ref List<string> document)
        {
            string words = "";

            foreach (string str in document)
            {
                words += str + " ";
            }

            words = words.ToLower().Replace(",", string.Empty);

            List<string> wordList = words.Split(' ').ToList<string>();
            for(int i = 0; i < wordList.Count; i++)
            {
                if (wordList[i] == "")
                {
                    wordList.Remove(wordList[i]);
                }
            }
            return wordList;
        }
        static int CountWords(string S, string S0)
        {
            string[] temp = S0.Split(new[] { S }, StringSplitOptions.None);
            return temp.Length - 1;
        }
        static void CalcScore(string request)
        {
            List<string> document = new List<string>();
            foreach (var docs in startDocs)
            {
                document.Add(docs);
            }
            StemmAllText(ref document);




            List<double> scores = new List<double>();
            List<string> requestWords = request.ToLower().Split(' ').ToList<string>();
            
            for (int i = 0; i < requestWords.Count; i++)
            {
                requestWords[i] = Stemming.TransformingWord(requestWords[i]);
            }

            DelStopWordsText(ref requestWords);

            //single
           

            for (int i = 0; i < document.Count; i++)
            {
                scores.Add(0);
                foreach (var a in requestWords)
                {
                    scores[i] += CountWords(a, document[i]);
                }
               


            }
            //pair
          
            for (int i = 0; i < document.Count; i++)
            {
                
                for (int j = 0; j < requestWords.Count - 1; j++)
                {
                    scores[i] += CountWords(requestWords[j]+" "+requestWords[j+1],document[i]);
                }
               

            }
            //AllWords
            
            for (int i = 0; i < document.Count; i++)
            {
                bool f = true;
                foreach(var a in requestWords)
                {
                    if (!document[i].Contains(a)){
                        f = false;
                    }
                }
                if (f)
                {
                    scores[i]++;
                }
               
            }

            string str = "";
            foreach(var a in requestWords)
            {
                str += a + " ";
            }
            for(int i = 0; i < document.Count; i++)
            {

                if (document[i].Contains(str))
                {
                    scores[i] += 1f/350f;
                   
                }
                
            }
            double temp;
            string tempStr;
            List<string> result = startDocs;
            for (int i = 0; i < scores.Count - 1; i++)
            {
                for (int j = i + 1; j < scores.Count; j++)
                {
                    if (scores[i] < scores[j])
                    {
                        tempStr = result[i];
                        result[i] = result[j];
                        result[j] = tempStr;
                        temp = scores[i];
                        scores[i] = scores[j];
                        scores[j] = temp;
                    }
                }
            }
            Console.WriteLine("Ранжирование с помощью Score:");
            for (int i = 0; i < scores.Count; i++)
            {
                Console.WriteLine(i+1+"."+result[i]) ;
            }
        }
    }
}

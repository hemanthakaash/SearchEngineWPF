using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
namespace SearchEngine {
    class fileInfo {
        public string Name;
        public int ID;
        public int uniqueWords;
        public int largestFreq;
        public fileInfo(string name, int Id,int u,int lf) {
            Name = name; ID = Id;
            uniqueWords = u;
            largestFreq = lf;
        }
    }
    class Catalogue {
        public int largestIndex { set; get; }
        public IList<fileInfo> files { get;set; }
    }
    class eachDoc {
        public int docId, frequency;
        public eachDoc(int id,int f) {
            docId = id;
            frequency = f;
        }
    }
    class index {
        public string term;
        public int IDF, total;
        public List<eachDoc> head;
        public index(string word,int f,int id) {
            term = word;
            IDF = 1;
            total = f;
            head = new List<eachDoc>();
            head.Add(new eachDoc(id, f));
        }
        public void addDoc(int id,int f) {
            head.Add(new eachDoc(id, f));
            IDF++;
            total += f;
        }
    }
    class store {
        public List<index> Index { get; set; }
        public store() {
            Index = new List<index>();
        }
        public void join(List<eachTerm> terms,int fileId) {
            int m;
            for(int i = 0; i < terms.Count; i++) {
                m = isPresent(terms[i].term);
                if (m == -1) {
                    Index.Add(new index(terms[i].term, terms[i].frequency, fileId));
                }
                else if (string.Compare(Index[m].term,terms[i].term) == 0) {
                    Index[m].addDoc(fileId, terms[i].frequency);
                }
                else {
                    Index.Insert(m,new index(terms[i].term, terms[i].frequency, fileId));
                }
            }
        }
        public int isPresent(string word) {
            int i;
            for (i = 0; i < Index.Count; i++) {
                switch(string.Compare(Index[i].term, word)) {
                    case 0: 
                    case 1: return i;
                }
            }
            return -1;
        }
        void removeFileEntry(int fileID,int j,int frequency) {
            Index[j].IDF--;
            Index[j].total -= frequency;
            for(int i = 0; i < Index[j].head.Count; i++) {
                if (Index[j].head[i].docId == fileID) {
                    Index[j].head.RemoveAt(i);
                    break;
                }
            }
        }
        public void removeWords(int fileID,List<eachTerm> terms) {
            int i = 0, j = 0;
            while (i < terms.Count) {
                if (string.Compare(Index[j].term, terms[i].term) == 0) {
                    removeFileEntry(fileID, j,terms[i].frequency);
                    i++;
                    j++;
                }
                else {
                    j++;
                }
            }
        }
    }
    class eachTerm {
        public string term;
        public int frequency;
        public eachTerm(string w,int c) {
            term = w;frequency = c;
        }
    }
    class termDetails {
        public int fileId;
        public List<eachTerm> list;
        public termDetails() {
            list = new List<eachTerm>();
        }
    }
    class termFrequency {
        public List<termDetails> termList;
        public termFrequency() {
            termList = new List<termDetails>();
        }
    }
    class Indexer {
        string Read(string file) {
            StreamReader reader = new StreamReader(file);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }
        void Write(string file,string str) {
            StreamWriter writer = new StreamWriter(file);
            writer.Write(str);
            writer.Close();
        }
        int addFileToCatalogue(string fileName,int u,int lf) {
            string str=Read("Storage\\Catalogue.JSON");
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(str);
            catalogue.files.Add(new fileInfo(fileName, catalogue.largestIndex++, u, lf));
            str = JsonConvert.SerializeObject(catalogue);
            Write("Storage\\Catalogue.JSON", str);
            return catalogue.largestIndex-1;
        }
        public void removeStopWords(List<string> wordList,string[] stopWords) {
            int w = wordList.Count,k=0;
            int s = stopWords.Length,l=0;
            while (l < s && k < w) {
                switch (string.Compare(wordList[k], stopWords[l])) {
                    case 0: wordList.RemoveAt(k);
                        w--;
                        break;
                    case 1: l++;
                        break;
                    case -1: k++;
                        break;
                }
            }
        }
        public List<string> getWordList(string str) {
            string str1 = "A,ABOUT,ABOVE,AFTER,AGAIN,AGAINST,ALL,AM,AN,AND,ANY,ARE,AS,AT,BE,BECAUSE,BEEN,BEFORE,BEING,BELOW,BETWEEN," +
                "BOTH,BUT,BY,CAN,DID,DO,DOES,DOING,DON,DOWN,DURING,EACH,FEW,FOR,FROM,FURTHER,HAD,HAS,HAVE,HAVING,HE,HER,HERE,HERS," +
                "HERSELF,HIM,HIMSELF,HIS,HOW,I,IF,IN,INTO,IS,IT,ITS,ITSELF,JUST,ME,MORE,MOST,MY,MYSELF,NO,NOR,NOT,NOW,OF,OFF,ON,ONCE," +
                "ONLY,OR,OTHER,OUR,OURS,OUT,OVER,OWN,S,SAME,SHE,SHOULD,SO,SOME,SUCH,T,THAN,THAT,THE,THEIR,THEIRS,THEM,THEMSELVES,THEN," +
                "THERE,THESE,THEY,THIS,THOSE,THROUGH,TO,TOO,UNDER,UNTIL,UP,VERY,WAS,WE,WERE,WHAT,WHEN,WHERE,WHICH,WHILE,WHO,WHOM,WHY,WILL," +
                "WITH,YOU,YOUR,YOURS,YOURSELF,YOURSELVES,OURSELVES";
            string[] stopWords = str1.Split(',');
            char[] delimiters = new char[] { '\r', '\n', '\t', ' ', ',', '.', '\'', '?', '\"', ':', ';', '`', '~', '!', '(', ')', '<', '>', '}', '{', '[', ']', '=', '_', '|', '\\', '/' };
            List<string> wordList = new List<string>(str.Split(delimiters,StringSplitOptions.RemoveEmptyEntries));
            //string[] dummy = new string[] { "always", "1234jfljf234", "HELLO", "THE", "THE", "YOU" };
            //List<string> wordList = new List<string>(dummy);
            for (int i = 0; i < wordList.Count; i++) {
                wordList[i] = wordList[i].ToUpper();
            }
            wordList.Sort();
            removeStopWords(wordList, stopWords);
            return wordList;
        }
        public int formTerms(List<string> wordList,termDetails terms) {
            int c = 1, max = 1;
            string word = wordList[0];
            for (int i = 1; i < wordList.Count; i++) {
                if (string.Compare(wordList[i], wordList[i - 1]) != 0) {
                    terms.list.Add(new eachTerm(word, c));
                    if (c > max) {
                        max = c;
                    }
                    word = wordList[i];
                    c = 1;
                }
                else {
                    c++;
                }
            }
            terms.list.Add(new eachTerm(word, c));
            return max;
        }
        public int tempFunction(List<string> wordList,termDetails terms) {
            int max = formTerms(wordList, terms);
            int l;
            porterStemmer stemmer = new porterStemmer();
            for (int i = 0; i < terms.list.Count; i++) {
                terms.list[i].term = stemmer.stem(terms.list[i].term);
            }
            for (int i = 1; i < terms.list.Count;) {
                if (i<terms.list.Count-1 && string.Compare(terms.list[i].term, terms.list[i + 1].term) == 0) {
                    terms.list[i].frequency += terms.list[i + 1].frequency;
                    terms.list.RemoveAt(i + 1);
                    if (i < terms.list.Count && max < terms.list[i].frequency) {
                        max = terms.list[i].frequency;
                    }
                }
                else {
                    if (i > 0 && string.Compare(terms.list[i - 1].term, terms.list[i].term) == 1) {
                        for (l = i - 2; l >= 0; l--) {
                            if (string.Compare(terms.list[i - 1].term, terms.list[l].term) == 1) {
                                break;
                            }
                        }
                        terms.list.Insert(l + 1, terms.list[i]);
                        terms.list.RemoveAt(i + 1);
                        i = l - 1;
                    }
                    i++;
                }
            }
            return max;
        }
        public void addIndex(string path) {
            List<string> wordList = getWordList(Read("TestDirectory\\" + path));
            if (wordList.Count < 1) {
                return;
            }
            termDetails terms = new termDetails();
            int max = tempFunction(wordList, terms);
            int fileID = addFileToCatalogue(path, terms.list.Count, max);
            terms.fileId = fileID;
            string str = Read("Storage\\INDEX.JSON");
            store indexer = JsonConvert.DeserializeObject<store>(str);
            indexer.join(terms.list,fileID);
            str = JsonConvert.SerializeObject(indexer);
            Write("Storage\\INDEX.JSON",str);
            str = Read("Storage\\TermFrequency.JSON");
            termFrequency frequency = JsonConvert.DeserializeObject<termFrequency>(str);
            frequency.termList.Add(terms);
            str = JsonConvert.SerializeObject(frequency);
            Write("Storage\\TermFrequency.JSON", str);
        }
        int removeFileFromCatalogue(string fileName) {
            string str = Read("Storage\\Catalogue.JSON");
            Catalogue catalogue = JsonConvert.DeserializeObject<Catalogue>(str);
            int index = -1;
            for(int i = 0; i < catalogue.files.Count; i++) {
                if (catalogue.files[i].Name.Equals(fileName)) {
                    index = catalogue.files[i].ID;
                    catalogue.files.RemoveAt(i);
                    break;
                }
            }
            str = JsonConvert.SerializeObject(catalogue);
            Write("Storage\\Catalogue.JSON",str);
            return index;
        }
        public List<eachTerm> getWordsToRemove(int fileID) {
            List<eachTerm> words = null;
            string str = Read("Storage\\TermFrequency.JSON");
            termFrequency terms = JsonConvert.DeserializeObject<termFrequency>(str);
            for (int i = 0; i < terms.termList.Count; i++) {
                if (terms.termList[i].fileId == fileID) {
                    words = terms.termList[i].list;
                    terms.termList.RemoveAt(i);
                    break;
                }
            }
            str = JsonConvert.SerializeObject(terms);
            Write("Storage\\TermFrequency.JSON", str);
            return words;
        }
        void removeWordsFromIndex(int fileID,List<eachTerm> terms) {
            string str = Read("Storage\\INDEX.JSON");
            store index = JsonConvert.DeserializeObject<store>(str);
            index.removeWords(fileID, terms);
            str = JsonConvert.SerializeObject(index);
            Write("Storage\\INDEX.JSON", str);
        }
        public void removeIndex(string path) {
            int fileID = removeFileFromCatalogue(path);
            List<eachTerm> terms=getWordsToRemove(fileID);
            removeWordsFromIndex(fileID,terms);
        }
    }
}

using System;
using System.IO;
using SearchEngine;
using SearchEngineWPF;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace SearchEngine {
    class docScore {
        int fileId;
        public string fileName;
        public double score;
        public docScore(int id,string name,double s) {
            fileId = id;
            fileName = name;
            score = s;
        }
    }
    class search {
        List<index> Index;
        List<termDetails> frequency;
        Catalogue catalogue;
        Indexer helper;
        int Df;
        string Read(string name) {
            StreamReader reader = new StreamReader(name);
            string str = reader.ReadToEnd();
            reader.Close();
            return str;
        }
        public search() {
            helper = new Indexer();
        }
        List<eachTerm> getTerms(fileInfo f) {
            for (int i = 0; i < frequency.Count; i++) {
                if (frequency[i].fileId == f.ID) {
                    return frequency[i].list;
                }
            }
            return null;
        }
        docScore getScore(fileInfo f,List<docScore> query,double value) {
            if (value == 0) {
                return new docScore(f.ID, f.Name, 0);
            }
            double rms = 0,temp,numerator=0,score;
            int j = 0,m,k=0;
            List<eachTerm> terms = getTerms(f);
            for (int i = 0; i < Index.Count && j < terms.Count;) {
                m = string.Compare(Index[i].term, terms[j].term);
                if (m == 0) {
                    temp = (double)(Index[i].IDF) > 0 ? (Math.Log((double)Df / Index[i].IDF) * terms[j].frequency) / f.largestFreq : 0;
                    while (true) {
                        if(k >= query.Count) {
                            break;
                        }
                        m = string.Compare(Index[i].term, query[k].fileName);
                        if (m == 0) {
                            numerator += temp * query[k].score;
                            k++;
                            break;
                        }
                        else if (m == 1) {
                            k++;
                        }
                        else {
                            break;
                        }
                    }
                    rms += (temp * temp);
                    i++;
                    j++;
                }
                else if (m == -1) {
                    i++;
                }
                else {
                    query.Add(new docScore(-1, terms[j].term, 0));
                    j++;
                }
            }
            score = numerator / (Math.Sqrt(rms) * value);
            return new docScore(f.ID,f.Name,score);
        }
        void readFiles() {
            string str = Read("Storage\\INDEX.JSON");
            store index = JsonConvert.DeserializeObject<store>(str);
            Index = index.Index;
            str = Read("Storage\\TermFrequency.JSON");
            termFrequency tempObj = JsonConvert.DeserializeObject<termFrequency>(str);
            frequency=tempObj.termList;
            str = Read("Storage\\Catalogue.JSON");
            catalogue = JsonConvert.DeserializeObject<Catalogue>(str);
            Df = catalogue.files.Count;
        }
        double getQueryValues(List<eachTerm> terms,List<docScore> query,int max) {
            int j=0,m;
            double rms = 0, temp;
            for (int i = 0; i < Index.Count && j<terms.Count;) {
                m = string.Compare(Index[i].term, terms[j].term);
                if (m == 0) {
                    //temp = Index[i].IDF > 0 ? (double)(Math.Log(Df / Index[i].IDF) * terms[j].frequency) / max : 0;
                    if(Index[i].IDF > 0) {
                        temp = (double)(Math.Log((double)Df / Index[i].IDF) * terms[j].frequency) / max;
                    }
                    else {
                        temp = 0;
                    }
                    query.Add(new docScore(i, terms[j].term, temp));
                    rms += (temp*temp);
                    i++;
                    j++;
                }
                else if (m == -1) {
                    i++;
                }
                else {
                    terms.RemoveAt(j);
                }
            }
            return Math.Sqrt(rms);
        }
        List<docScore> getScoreList(List<docScore> queryVar,double queryValue) {
            bool flag;
            List<docScore> scoresList = new List<docScore>();
            foreach (fileInfo file in catalogue.files) {
                docScore score = getScore(file, queryVar, queryValue);
                flag = true;
                for (int i = 0; i < scoresList.Count; i++) {
                    if (scoresList[i].score < score.score) {
                        scoresList.Insert(i, score);
                        flag = false;
                        break;
                    }
                }
                if (flag) {
                    scoresList.Add(score);
                }
            }
            return scoresList;
        }
        public List<string> getResults(string query) {
            readFiles();
            List<string> result = new List<string>();
            List<string> words = helper.getWordList(query);
            if (words.Count < 1) {
                return result;
            }
            termDetails terms = new termDetails();
            int max = helper.tempFunction(words, terms);
            List<docScore> queryVar = new List<docScore>();
            double queryValue = getQueryValues(terms.list,queryVar, max);
            List<docScore> scoresList = getScoreList(queryVar, queryValue);
            foreach(docScore doc in scoresList) {
                if (doc.score <= 0) {
                    break;
                }
                result.Add(doc.fileName);
            }
            return result;
        }
    }
}
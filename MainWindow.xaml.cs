using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SearchEngine;

namespace SearchEngineWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        int totalF;
        void Write(string path,string str) {
            StreamWriter writer = new StreamWriter(path);
            writer.Write(str);
            writer.Close();
        }
        void firstTimeUser() {
            Directory.CreateDirectory("TestDirectory");
            Directory.CreateDirectory("Storage");
            Write("Storage\\Catalogue.JSON", "{\"largestIndex\":0,\"files\":[]}");
            Write("Storage\\INDEX.JSON", "{}");
            Write("Storage\\TermFrequency.JSON", "{}");
        }
        public MainWindow() {
            InitializeComponent();
            if (!File.Exists("Storage\\Catalogue.JSON")) {
                firstTimeUser();
            }
            string[] files = Directory.GetFiles("TestDirectory");
            totalF = 0;
            foreach (string f in files) {
                string[] path = f.Split('\\');
                totalF++;
                Collection.Items.Add(path[path.Length-1]);
            }
            Total.Content = "No. of files present in the Collection = " + totalF.ToString();
        }
        private void Delete_Button(object sender, RoutedEventArgs e) {
            string text;
            Indexer indexing = new Indexer();
            try {
                text = Collection.SelectedItem.ToString();
                indexing.removeIndex(text);
                File.Delete("TestDirectory\\" + text);
                Collection.Items.RemoveAt(Collection.Items.IndexOf(Collection.SelectedItem));
                MessageBox.Show("File Deleted.");
                Total.Content = "No. of files present in the Collection = " + (--totalF).ToString();
            }
            catch(ArgumentOutOfRangeException ex) {
                MessageBox.Show("Select a file to be deleted.\n" + ex.Message);
            }
            catch (NullReferenceException ex) {
                MessageBox.Show("Select a file to be deleted.\n" + ex.Message);
            }
        }
        private void Add_Button(object sender, RoutedEventArgs e) {
            if (File.Exists(Path.Text) == true) {
                string[] names = Path.Text.Split('\\');
                string FileName = names[names.Length - 1];
                Indexer indexing = new Indexer();
                try {
                    File.Copy(Path.Text, "TestDirectory\\" + FileName);
                    Collection.Items.Add(FileName);
                    MessageBox.Show("File Added", "Success");
                    Total.Content = "No. of files present in the Collection = " + (++totalF).ToString();
                    Path.Text = "";
                    indexing.addIndex(FileName);
                }
                catch (IOException ex) {
                    MessageBox.Show("A file with same name is present in the collection. \n" + ex.Message);
                }
                catch (Exception f) {
                    MessageBox.Show("Exception Caught" + f.Message);
                }
            }
            else {
                Console.WriteLine(Path.Text + " is not a valid File.");
            }
        }
        private void Browse_Button(object sender, RoutedEventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true) {
                Path.Text = dialog.FileName;
            }
        }
        private void displayResults(List<string> query) {
            if (query.Count == 0) {
                Results.Items.Add("No document in the collection is relevant to the search query");
                return;
            }
            foreach(string str in query) {
                Results.Items.Add(str);
            }
            Count.Content = "No. of relevant documents present = " + query.Count.ToString();
        }
        private void Search_Button(object sender, RoutedEventArgs e) {
            Results.Items.Clear();
            string query = Query.Text;
            search Engine = new search();
            List<string> result = Engine.getResults(query);
            displayResults(result);
        }
    }
}
